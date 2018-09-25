// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DnsSdServer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DnsSdServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery.Server
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;

    using ARSoft.Tools.Net.Dns;

    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Simple implementation of a DNS-SD server.
    /// </summary>
    public class DnsSdServer : IDnsSdProvider
    {
        private const int DefaultTtl = 75 * 60;
        private const int RenewTtl = 15 * 60; // 80% of the above

        private const string ProtocolsPointerName = "_services._dns-sd._udp.local";

        private static readonly Logger Logger = LogHelper.GetLogger<DnsSdServer>();

        private readonly Dictionary<string, DnsServiceInfo> localServices = new Dictionary<string, DnsServiceInfo>();

        private readonly Dictionary<string, DnsServiceInfo> networkServices = new Dictionary<string, DnsServiceInfo>();

        private readonly List<ServiceRegistration> registering = new List<ServiceRegistration>();

        private readonly List<ExponentialTimer> runningAnnouncementTimers = new List<ExponentialTimer>();

        private readonly List<Query> runningQueries = new List<Query>();

        private readonly MulticastDnsServer server;

        private readonly string localName;

        private readonly ITimer maintenanceTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsSdServer"/> class.
        /// </summary>
        public DnsSdServer()
        {
            this.server = new MulticastDnsServer(1, this.ProcessMessage);
            this.localName = ApplicationHelper.MachineName + ".local";

            this.maintenanceTimer = TimerFactory.Current.CreateTimer("DNS-SD.Maintenance");
            this.maintenanceTimer.Interval = TimeSpan.FromSeconds(1);
            this.maintenanceTimer.AutoReset = true;
            this.maintenanceTimer.Elapsed += this.MaintenanceTimerOnElapsed;
        }

        /// <summary>
        /// Starts this server.
        /// </summary>
        public void Start()
        {
            Logger.Debug("Starting");
            this.server.Start();
            this.maintenanceTimer.Enabled = true;
        }

        /// <summary>
        /// Stops this server.
        /// </summary>
        public void Stop()
        {
            Logger.Debug("Stopping");
            this.maintenanceTimer.Enabled = false;

            lock (this.runningAnnouncementTimers)
            {
                foreach (var timer in this.runningAnnouncementTimers)
                {
                    timer.Stop();
                }

                this.runningAnnouncementTimers.Clear();
            }

            foreach (var registration in this.registering)
            {
                registration.Stop();
            }

            foreach (var query in this.runningQueries.ToArray())
            {
                query.Stop();
            }

            // send "clear" messages for all registered services
            var messages = new List<MulticastDnsMessage>();
            lock (this.localServices)
            {
                foreach (var localService in this.localServices.Values)
                {
                    var message = CreateResponseMessage();
                    message.AnswerRecords.Add(new PtrRecord(localService.Protocol, 0, localService.FullName));
                    messages.Add(message);
                }

                this.localServices.Clear();
            }

            foreach (var message in messages)
            {
                LogMessage("Sending", message, message.Questions, message.AnswerRecords, message.AuthorityRecords);
                this.SendMulticastMessage(message);
            }

            lock (this.networkServices)
            {
                this.networkServices.Clear();
            }

            var sleep = (messages.Count * 100) + 500;
            Logger.Trace("Waiting {0} ms to make sure {1} messages were sent", sleep, messages.Count);
            Thread.Sleep(sleep);

            this.server.Stop();
            Logger.Debug("Stopped");
        }

        /// <summary>
        /// Registers a new service with the given identity.
        /// </summary>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        /// <param name="protocol">
        /// The protocol name (without ".local").
        /// </param>
        /// <param name="port">
        /// The port number.
        /// </param>
        /// <param name="attributes">
        /// The service attributes.
        /// </param>
        /// <returns>
        /// The service registration to be used with <see cref="DeregisterService"/>.
        /// </returns>
        /// <exception cref="DnsSdException">
        /// If a service with the same name and protocol was already registered.
        /// </exception>
        public IServiceRegistration RegisterService(
            string serviceName, string protocol, int port, IDictionary<string, string> attributes)
        {
            var protocolFullName = protocol + ".local";
            var serviceInfo = new DnsServiceInfo(DefaultTtl)
                                  {
                                      Name = serviceName,
                                      Protocol = protocolFullName,
                                      Port = port,
                                      Addresses = this.server.GetLocalIPAddresses()
                                  };

            if (this.localServices.ContainsKey(serviceInfo.FullName))
            {
                throw new DnsSdException("Service already registered: " + serviceInfo.FullName);
            }

            Logger.Info("Registering service '{0}' on port {1}", serviceInfo.FullName, serviceInfo.Port);

            foreach (var attribute in attributes)
            {
                Logger.Debug("  {0}={1}", attribute.Key, attribute.Value);
                serviceInfo.Attributes.Add(attribute.Key, attribute.Value);
            }

            var registration = new ServiceRegistration(serviceInfo, this);
            lock (this.registering)
            {
                this.registering.Add(registration);
            }

            registration.Verified += (s, e) =>
                {
                    lock (this.registering)
                    {
                        this.registering.Remove(registration);
                        if (registration.Cancelled)
                        {
                            return;
                        }
                    }

                    this.Register(registration.Info);
                    registration.IsRegistered = true;
                };
            registration.Start();

            return registration;
        }

        /// <summary>
        /// Deregisters a previously registered service.
        /// <seealso cref="RegisterService"/>
        /// </summary>
        /// <param name="registration">
        /// The registration.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the <see cref="registration"/> didn't come from <see cref="RegisterService"/>.
        /// </exception>
        public void DeregisterService(IServiceRegistration registration)
        {
            var reg = registration as ServiceRegistration;
            if (reg == null)
            {
                throw new ArgumentException("Unknown registration");
            }

            Logger.Info("Deregistering service '{0}' on port {1}", reg.Info.FullName, reg.Info.Port);

            lock (this.registering)
            {
                if (this.registering.Remove(reg))
                {
                    reg.Stop();
                }
            }

            this.RemoveService(this.localServices, reg.Info);
            reg.IsRegistered = false;
        }

        /// <summary>
        /// Starts a query for all services of a protocol.
        /// When this method returns, the returned query might already contain services.
        /// </summary>
        /// <param name="protocol">
        /// The protocol name without the ".local".
        /// </param>
        /// <returns>
        /// The <see cref="IQuery"/> that can be used to get the results.
        /// </returns>
        public IQuery CreateQuery(string protocol)
        {
            protocol += ".local";

            return new Query(this, protocol);
        }

        private static void LogMessage(
            string text,
            DnsMessageBase message,
            List<DnsQuestion> questions,
            List<DnsRecordBase> answerRecords,
            List<DnsRecordBase> authorityRecords)
        {
            if (!Logger.IsTraceEnabled)
            {
                return;
            }

            Logger.Trace(
                "{0}: QR={1} AA={2} ID={3}", text, message.IsQuery, message.AdditionalRecords, message.TransactionID);
            LogEntries("Questions", questions.ConvertAll(q => (DnsMessageEntryBase)q));
            LogEntries("Answers", answerRecords.ConvertAll(q => (DnsMessageEntryBase)q));
            LogEntries("Authority", authorityRecords.ConvertAll(q => (DnsMessageEntryBase)q));
            LogEntries("Additional", message.AdditionalRecords.ConvertAll(q => (DnsMessageEntryBase)q));
        }

        private static void LogEntries(string text, IEnumerable<DnsMessageEntryBase> entries)
        {
            Logger.Trace("  {0}:", text);
            foreach (var entry in entries)
            {
                Logger.Trace("    {0}", entry);
            }
        }

        private static MulticastDnsMessage CreateResponseMessage()
        {
            return new MulticastDnsMessage { IsQuery = false, IsAuthoritiveAnswer = true };
        }

        private static T FindRecord<T>(Predicate<T> predicate, params List<DnsRecordBase>[] sections)
            where T : DnsRecordBase
        {
            foreach (var section in sections)
            {
                var result = section.Find(r => r is T && predicate((T)r)) as T;
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static IEnumerable<T> FindAllRecords<T>(Predicate<T> predicate, params List<DnsRecordBase>[] sections)
            where T : DnsRecordBase
        {
            foreach (var section in sections)
            {
                foreach (var record in section)
                {
                    var typeRecord = record as T;
                    if (typeRecord != null && predicate(typeRecord))
                    {
                        yield return typeRecord;
                    }
                }
            }
        }

        private void Register(DnsServiceInfo serviceInfo)
        {
            this.AddService(this.localServices, serviceInfo);

            var message = CreateResponseMessage();

            this.AddServiceAnnouncementRecords(serviceInfo, message);

            var timer = new ExponentialTimer(2, 8);
            timer.Elapsed += (s, e) =>
                {
                    serviceInfo.UpdateTtl(DefaultTtl);
                    LogMessage(
                        "Registering", message, message.Questions, message.AnswerRecords, message.AuthorityRecords);
                    this.SendMulticastMessage(message);
                };
            timer.Completed += (s, e) =>
                {
                    lock (this.runningAnnouncementTimers)
                    {
                        this.runningAnnouncementTimers.Remove(timer);
                    }
                };
            lock (this.runningAnnouncementTimers)
            {
                timer.Start();
                this.runningAnnouncementTimers.Add(timer);
            }
        }

        private void AddServiceAnnouncementRecords(DnsServiceInfo serviceInfo, MulticastDnsMessage message)
        {
            this.AddSrvRecord(serviceInfo, message.AnswerRecords);
            this.AddTxtRecord(serviceInfo, message.AnswerRecords);
            this.AddPtrRecord(serviceInfo, message.AnswerRecords);

            if (FindRecord<PtrRecord>(
                r => r.PointerDomainName == serviceInfo.Protocol && r.Name == ProtocolsPointerName,
                message.AnswerRecords) == null)
            {
                message.AnswerRecords.Add(new PtrRecord(ProtocolsPointerName, DefaultTtl, serviceInfo.Protocol));
            }

            this.AddARecords(serviceInfo, message.AdditionalRecords);
        }

        private void AddSrvRecord(DnsServiceInfo serviceInfo, List<DnsRecordBase> records)
        {
            var port = (ushort)serviceInfo.Port;
            records.Add(
                new SrvRecord(serviceInfo.FullName, 120, 0, 0, port, this.localName) { CacheFlush = true });
        }

        private void AddTxtRecord(DnsServiceInfo serviceInfo, List<DnsRecordBase> records)
        {
            var texts = new List<string>(serviceInfo.Attributes.Count);
            foreach (var attribute in serviceInfo.Attributes)
            {
                texts.Add(attribute.Key + "=" + attribute.Value);
            }

            records.Add(new TxtRecord(serviceInfo.FullName, DefaultTtl, texts) { CacheFlush = true });
        }

        private void AddPtrRecord(DnsServiceInfo serviceInfo, List<DnsRecordBase> records)
        {
            records.Add(new PtrRecord(serviceInfo.Protocol, DefaultTtl, serviceInfo.FullName));
        }

        private void AddARecords(DnsServiceInfo serviceInfo, List<DnsRecordBase> records)
        {
            // TODO: send the right IP address(es) on the right interface
            this.AddARecords(serviceInfo.Addresses, records);
        }

        private void AddARecords(IEnumerable<IPAddress> addresses, List<DnsRecordBase> records)
        {
            foreach (var addr in addresses)
            {
                var address = addr;
                if (FindRecord<ARecord>(r => r.Address.Equals(address), records) == null)
                {
                    records.Add(new ARecord(this.localName, 120, addr) { CacheFlush = true });
                }
            }
        }

        private void RemoveQuery(Query query)
        {
            this.runningQueries.Remove(query);
        }

        private DnsMessageBase ProcessMessage(DnsMessageBase messageBase, ref IPEndPoint remoteEndPoint)
        {
            var message = messageBase as DnsMessage;
            if (message == null)
            {
                return null;
            }

            LogMessage("Received", messageBase, message.Questions, message.AnswerRecords, message.AuthorityRecords);

            if (message.IsQuery)
            {
                return this.ProcessQuery(message, ref remoteEndPoint);
            }

            this.ProcessResponse(message, remoteEndPoint);
            return null;
        }

        private DnsMessageBase ProcessQuery(DnsMessage message, ref IPEndPoint remoteEndPoint)
        {
            var response = CreateResponseMessage();
            var unicast = true;
            foreach (var question in message.Questions)
            {
                if (!question.QueryUnicast)
                {
                    unicast = false;
                }

                this.ProcessQuestion(question, message, response);
            }

            if (response.AnswerRecords.Count == 0)
            {
                return null;
            }

            if (!unicast)
            {
                // we had at least one non-unicast question, let's multicast the answer
                remoteEndPoint = new IPEndPoint(MulticastDnsServer.MulticastAddress, MulticastDnsServer.MdnsPort);
            }

            return response;
        }

        private void ProcessQuestion(DnsQuestion question, DnsMessage query, MulticastDnsMessage response)
        {
            if (question.RecordClass != RecordClass.INet || string.IsNullOrEmpty(question.Name))
            {
                return;
            }

            switch (question.RecordType)
            {
                case RecordType.Ptr:
                    this.ProcessPtrQuestion(question.Name, query, response);
                    break;
                case RecordType.Srv:
                    this.ProcessSrvQuestion(question.Name, query, response);
                    break;
                case RecordType.Txt:
                    this.ProcessTxtQuestion(question.Name, query, response);
                    break;
                case RecordType.A:
                    this.ProcessAQuestion(question.Name, query, response);
                    break;
                case RecordType.Any:
                    this.ProcessAnyQuestion(question.Name, query, response);
                    break;
            }
        }

        private void ProcessPtrQuestion(string name, DnsMessage query, MulticastDnsMessage response)
        {
            lock (this.localServices)
            {
                foreach (var service in this.localServices.Values)
                {
                    if (service.Protocol != name)
                    {
                        continue;
                    }

                    var s = service;
                    if (FindRecord<PtrRecord>(
                        r => r.Name == s.Protocol && r.PointerDomainName == s.FullName, query.AnswerRecords) == null)
                    {
                        // didn't find our answer in the AnswerRecords, let's add it
                        this.AddSrvRecord(service, response.AdditionalRecords);
                        this.AddTxtRecord(service, response.AdditionalRecords);
                        this.AddPtrRecord(service, response.AnswerRecords);
                        this.AddARecords(service, response.AdditionalRecords);
                    }
                }
            }
        }

        private void ProcessSrvQuestion(string name, DnsMessage query, MulticastDnsMessage response)
        {
            lock (this.localServices)
            {
                DnsServiceInfo service;
                if (!this.localServices.TryGetValue(name, out service))
                {
                    return;
                }

                if (FindRecord<SrvRecord>(r => r.Name == service.FullName, query.AnswerRecords) == null)
                {
                    this.AddSrvRecord(service, response.AnswerRecords);
                    this.AddARecords(service, response.AdditionalRecords);
                }
            }
        }

        private void ProcessTxtQuestion(string name, DnsMessage query, MulticastDnsMessage response)
        {
            lock (this.localServices)
            {
                DnsServiceInfo service;
                if (!this.localServices.TryGetValue(name, out service))
                {
                    return;
                }

                if (FindRecord<TxtRecord>(r => r.Name == service.FullName, query.AnswerRecords) == null)
                {
                    this.AddTxtRecord(service, response.AnswerRecords);
                    this.AddARecords(service, response.AdditionalRecords);
                }
            }
        }

        private void ProcessAQuestion(string name, DnsMessage query, MulticastDnsMessage response)
        {
            if (name == this.localName)
            {
                if (FindRecord<ARecord>(r => r.Name == this.localName, query.AnswerRecords) == null)
                {
                    this.AddARecords(this.server.GetLocalIPAddresses(), response.AnswerRecords);
                }
            }
        }

        private void ProcessAnyQuestion(string name, DnsMessage query, MulticastDnsMessage response)
        {
            this.ProcessAQuestion(name, query, response);

            lock (this.localServices)
            {
                foreach (var service in this.localServices.Values)
                {
                    this.AddAnyAnswer(name, query, response, service);
                }
            }
        }

        private void AddAnyAnswer(string name, DnsMessage query, MulticastDnsMessage response, DnsServiceInfo service)
        {
            if (service.Protocol == name)
            {
                if (FindRecord<PtrRecord>(
                    r => r.Name == service.Protocol && r.PointerDomainName == service.FullName, query.AnswerRecords)
                    == null)
                {
                    // didn't find our answer in the AnswerRecords, let's add it
                    this.AddSrvRecord(service, response.AdditionalRecords);
                    this.AddTxtRecord(service, response.AdditionalRecords);
                    this.AddPtrRecord(service, response.AnswerRecords);
                    this.AddARecords(service, response.AdditionalRecords);
                }
            }

            if (service.FullName == name)
            {
                if (FindRecord<PtrRecord>(
                    r => r.Name == service.Protocol && r.PointerDomainName == service.FullName, query.AnswerRecords)
                    == null)
                {
                    // didn't find our answer in the AnswerRecords, let's add it
                    this.AddSrvRecord(service, response.AnswerRecords);
                    this.AddTxtRecord(service, response.AnswerRecords);
                    this.AddARecords(service, response.AdditionalRecords);
                }
            }
        }

        private void ProcessResponse(DnsMessage message, IPEndPoint remoteEndPoint)
        {
            lock (this.registering)
            {
                foreach (var registration in this.registering)
                {
                    registration.HandlePossibleConflict(message);
                }
            }

            foreach (var record in message.AnswerRecords)
            {
                if (record.RecordType != RecordType.Ptr || record.RecordClass != RecordClass.INet)
                {
                    continue;
                }

                this.ProcessPtrRecord((PtrRecord)record, message);
            }
        }

        private void ProcessPtrRecord(PtrRecord ptr, DnsMessage message)
        {
            DnsServiceInfo foundService = null;
            lock (this.networkServices)
            {
                foreach (var netService in this.networkServices.Values)
                {
                    if (netService.FullName == ptr.PointerDomainName)
                    {
                        foundService = netService;
                        break;
                    }
                }
            }

            if (foundService != null)
            {
                // already known, just update TTL
                foundService.UpdateTtl(ptr.TimeToLive);
                return;
            }

            var fullName = ptr.PointerDomainName;
            var protocol = ptr.Name;
            if (!fullName.EndsWith(protocol))
            {
                return;
            }

            var name = fullName.Substring(0, fullName.Length - protocol.Length - 1);
            var service = new DnsServiceInfo(ptr.TimeToLive) { Name = name, Protocol = protocol };

            var txt = FindRecord<TxtRecord>(
                r => r.Name == service.FullName, message.AnswerRecords, message.AdditionalRecords);
            if (txt != null)
            {
                foreach (var pair in txt.TextParts)
                {
                    var index = pair.IndexOf('=');
                    if (index >= 0)
                    {
                        service.Attributes[pair.Substring(0, index)] = pair.Substring(index + 1);
                    }
                }
            }

            var srv = FindRecord<SrvRecord>(
                r => r.Name == service.FullName, message.AnswerRecords, message.AdditionalRecords);
            var addresses = new List<IPAddress>();
            if (srv != null)
            {
                service.Port = srv.Port;
                foreach (
                    var a in
                        FindAllRecords<ARecord>(
                            r => r.Name == srv.Target, message.AnswerRecords, message.AdditionalRecords))
                {
                    addresses.Add(a.Address);
                }
            }

            service.Addresses = addresses.ToArray();

            // TODO: if some of the above information is not available in the additional records,
            // we should actually query the source service (like normal DNS)
            this.AddService(this.networkServices, service);
        }

        private void AddService(Dictionary<string, DnsServiceInfo> services, DnsServiceInfo service)
        {
            lock (services)
            {
                services[service.FullName] = service;
            }

            foreach (var query in this.runningQueries)
            {
                query.AddService(service);
            }
        }

        private void RemoveService(Dictionary<string, DnsServiceInfo> services, DnsServiceInfo service)
        {
            lock (services)
            {
                services.Remove(service.FullName);
            }

            foreach (var query in this.runningQueries)
            {
                query.RemoveService(service);
            }
        }

        private void MaintenanceTimerOnElapsed(object sender, EventArgs e)
        {
            var message = CreateResponseMessage();

            lock (this.localServices)
            {
                foreach (var service in this.localServices.Values)
                {
                    if (service.Ttl < RenewTtl)
                    {
                        service.UpdateTtl(DefaultTtl);
                        this.AddServiceAnnouncementRecords(service, message);
                    }
                }
            }

            if (message.AnswerRecords.Count > 0)
            {
                LogMessage("Renewing", message, message.Questions, message.AnswerRecords, message.AuthorityRecords);
                this.SendMulticastMessage(message);
            }

            var remove = new List<DnsServiceInfo>();
            lock (this.networkServices)
            {
                foreach (var service in this.networkServices.Values)
                {
                    if (service.Ttl <= 0)
                    {
                        remove.Add(service);
                    }
                }
            }

            foreach (var service in remove)
            {
                this.RemoveService(this.networkServices, service);
            }
        }

        private void SendMulticastMessage(DnsMessageBase message)
        {
            var endPoint = new IPEndPoint(MulticastDnsServer.MulticastAddress, MulticastDnsServer.MdnsPort);
            this.server.SendMessage(message, endPoint);
        }

        private class ServiceRegistration : IServiceRegistration
        {
            private readonly DnsSdServer owner;

            private readonly ITimer conflictResolutionTimer;

            private readonly string originalName;

            private int conflictTestCount;

            private int nextUniqueNameIdentifier = 2;

            public ServiceRegistration(DnsServiceInfo serviceInfo, DnsSdServer owner)
            {
                this.owner = owner;
                this.Info = serviceInfo;

                this.conflictResolutionTimer =
                    TimerFactory.Current.CreateTimer("DNS-SD.ConflictResolution>" + this.Info.FullName);
                this.conflictResolutionTimer.AutoReset = true;
                this.conflictResolutionTimer.Interval = TimeSpan.FromMilliseconds(250);
                this.conflictResolutionTimer.Elapsed += this.ConflictResolutionTimerOnElapsed;

                this.originalName = this.Info.Name;
            }

            public event EventHandler Verified;

            public DnsServiceInfo Info { get; private set; }

            public bool Cancelled { get; private set; }

            public bool IsRegistered { get; set; }

            public void Start()
            {
                this.conflictTestCount = 0;
                this.QueryConflicts(true);
                this.conflictResolutionTimer.Enabled = true;
            }

            public void Stop()
            {
                this.Cancelled = true;
                this.conflictResolutionTimer.Enabled = false;
            }

            public void HandlePossibleConflict(DnsMessage message)
            {
                if (message.IsQuery)
                {
                    return;
                }

                if (FindRecord<TxtRecord>(
                        r => r.Name == this.Info.FullName,
                        message.AnswerRecords,
                        message.AuthorityRecords,
                        message.AdditionalRecords) == null
                    && FindRecord<SrvRecord>(
                        r => r.Name == this.Info.FullName,
                        message.AnswerRecords,
                        message.AuthorityRecords,
                        message.AdditionalRecords) == null)
                {
                    return;
                }

                this.conflictResolutionTimer.Enabled = false;
                this.Info.Name = string.Format("{0} ({1})", this.originalName, this.nextUniqueNameIdentifier);

                this.nextUniqueNameIdentifier++;
                this.Start();
            }

            private void QueryConflicts(bool unicast)
            {
                if (++this.conflictTestCount > 3)
                {
                    // no conflict found
                    this.conflictResolutionTimer.Enabled = false;
                    var handler = this.Verified;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }

                    return;
                }

                var request = new DnsMessage();
                request.Questions.Add(
                    new DnsQuestion(this.Info.FullName, RecordType.Any, RecordClass.INet) { QueryUnicast = unicast });
                LogMessage(
                    "Querying conflicts", request, request.Questions, request.AnswerRecords, request.AuthorityRecords);
                this.owner.SendMulticastMessage(request);
            }

            private void ConflictResolutionTimerOnElapsed(object sender, EventArgs e)
            {
                this.QueryConflicts(false);
            }
        }

        private class Query : IQuery
        {
            private readonly DnsSdServer owner;

            private readonly string protocol;

            private readonly List<IServiceInfo> services;

            private readonly ExponentialTimer requestTimer;

            public Query(DnsSdServer owner, string protocol)
            {
                this.owner = owner;
                this.protocol = protocol;
                this.services = new List<IServiceInfo>();

                this.requestTimer = new ExponentialTimer(3, 5);
                this.requestTimer.Elapsed += this.RequestTimerOnElapsed;
            }

            public event EventHandler ServicesChanged;

            IServiceInfo[] IQuery.Services
            {
                get
                {
                    return this.services.ToArray();
                }
            }

            public void AddService(IServiceInfo service)
            {
                if (service.Protocol != this.protocol)
                {
                    return;
                }

                this.services.Add(service);
                this.RaiseServicesChanged();
            }

            public void RemoveService(IServiceInfo service)
            {
                if (this.services.Remove(service))
                {
                    this.RaiseServicesChanged();
                }
            }

            public void Start()
            {
                this.AddServices(this.owner.localServices);
                this.AddServices(this.owner.networkServices);
                this.owner.runningQueries.Add(this);

                this.requestTimer.Start();
            }

            public void Stop()
            {
                this.requestTimer.Stop();
                this.owner.RemoveQuery(this);
            }

            void IDisposable.Dispose()
            {
                this.Stop();
            }

            private void RaiseServicesChanged()
            {
                var handler = this.ServicesChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }

            private void AddServices(Dictionary<string, DnsServiceInfo> infos)
            {
                DnsServiceInfo[] copy;
                lock (infos)
                {
                    copy = new DnsServiceInfo[infos.Count];
                    infos.Values.CopyTo(copy, 0);
                }

                foreach (var info in copy)
                {
                    this.AddService(info);
                }
            }

            private void AddKnownServices(DnsMessage request, Dictionary<string, DnsServiceInfo> infos)
            {
                lock (infos)
                {
                    foreach (var info in infos.Values)
                    {
                        var ttl = info.Ttl;
                        if (ttl > 0 && info.Protocol == this.protocol)
                        {
                            request.AnswerRecords.Add(new PtrRecord(info.Protocol, ttl, info.FullName));
                        }
                    }
                }
            }

            private void RequestTimerOnElapsed(object sender, EventArgs eventArgs)
            {
                var request = new DnsMessage();
                request.Questions.Add(new DnsQuestion(this.protocol, RecordType.Ptr, RecordClass.INet));
                this.AddKnownServices(request, this.owner.localServices);
                this.AddKnownServices(request, this.owner.networkServices);

                LogMessage("Querying", request, request.Questions, request.AnswerRecords, request.AuthorityRecords);
                this.owner.SendMulticastMessage(request);
            }
        }

        private class DnsServiceInfo : IServiceInfo
        {
            private long endOfLife;

            public DnsServiceInfo(int timeToLive)
            {
                this.Attributes = new Dictionary<string, string>();
                this.UpdateTtl(timeToLive);
            }

            public string Name { get; set; }

            public string Protocol { get; set; }

            public string FullName
            {
                get
                {
                    return this.Name + "." + this.Protocol;
                }
            }

            public int Port { get; set; }

            public Dictionary<string, string> Attributes { get; private set; }

            public IPAddress[] Addresses { get; set; }

            public int Ttl
            {
                get
                {
                    return (int)(this.endOfLife - (TimeProvider.Current.TickCount / 1000));
                }
            }

            IServiceAttribute[] IServiceInfo.Attributes
            {
                get
                {
                    var result = new IServiceAttribute[this.Attributes.Count];
                    var i = 0;
                    foreach (var attribute in this.Attributes)
                    {
                        result[i++] = new Attribute { Name = attribute.Key, Value = attribute.Value };
                    }

                    return result;
                }
            }

            public void UpdateTtl(int timeToLive)
            {
                this.endOfLife = (TimeProvider.Current.TickCount / 1000) + timeToLive;
            }

            string IServiceInfo.GetAttribute(string name)
            {
                string value;
                this.Attributes.TryGetValue(name, out value);
                return value;
            }

            private class Attribute : IServiceAttribute
            {
                public string Name { get; set; }

                public string Value { get; set; }
            }
        }
    }
}

// -----------------------------------------------------------------------
// <copyright file="CommsTester.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ProxyProviderTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.ServiceModel;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core;
    using Gorba.Center.CommS.Core.ComponentModel;
    using Gorba.Center.CommS.Core.ComponentModel.Messages;
    using Gorba.Center.CommS.Core.ComponentModel.Messages.Activities;
    using Gorba.Center.CommS.Wcf.ServiceModel;
    using Gorba.Center.Common.Core.Communication;
    using Gorba.Center.Common.ServiceModel.DTO.Units;
    using Gorba.Common.Utility.ConcurrentPriorityQueue;

    using NLog;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CommsTester
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly TimeSpan UnitStatusPollingPeriod = TimeSpan.FromSeconds(10);
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private bool isRunning;
        private IDisposable unitStatusPollingSubscription;
        private DuplexProxyWrapper<ICommsMessageSubscription, SubscriptionState> commsProxy;

        public void Start()
        {
            Logger.Info("Starting CommsTester.");
            this.SubscribeToComms();
            this.isRunning = true;
        }

        public void Stop()
        {
            this.isRunning = false;
            if (this.unitStatusPollingSubscription != null)
            {
                this.unitStatusPollingSubscription.Dispose();
                this.unitStatusPollingSubscription = null;
            }

            if (this.commsProxy != null)
            {
                this.commsProxy.Faulted -= this.OnCommsProxyFaulted;
                this.commsProxy.Dispose();
                this.commsProxy = null;
            }
        }

        public void SendMessage(CommsMessage message)
        {
            var service = ProxyProvider<ICommsMessagingService>.Current.Provide();
            Logger.Trace("Sending a message to {0}", message.Address);
            service.SendMessage(message);
        }

        private void StartProxyTask(
    Func<ICommsMessageSubscription, SubscriptionState> initialization,
    Func<CommsMessageSubscriptionProxy> commsSubscriptionConstructor,
    PeriodicAction<ICommsMessageSubscription> periodicAction)
        {
            var proxyParams =
                new DuplexProxyWrapper<ICommsMessageSubscription, SubscriptionState>.DuplexProxyWrapperParams(
                    "UnitManagerToCommSSubscription",
                    initialization,
                    this.DisposeCommsSubscription,
                    commsSubscriptionConstructor,
                    periodicAction,
                    restartImmediatelyOnFault: true);

            this.commsProxy = new DuplexProxyWrapper<ICommsMessageSubscription, SubscriptionState>(proxyParams);
            this.commsProxy.Faulted += this.OnCommsProxyFaulted;
            var task = new Task(
                () => this.commsProxy.Proxy.AliveNotification(),
                this.cancellationTokenSource.Token,
                TaskCreationOptions.PreferFairness);
            task.ContinueWith(
                this.HandleError,
                this.cancellationTokenSource.Token,
                TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Default);
            task.Start();
        }

        private void HandleError(Task previousTask)
        {
            if (previousTask.Exception != null)
            {
                Logger.WarnException("An error occurred in a proxy task.", previousTask.Exception.InnerException);
            }
            else
            {
                Logger.Warn("An error occurred in a proxy task without an exception.");
            }
        }

        private void OnCommsProxyFaulted(object sender, ProxyWrapperFaultedEventArgs e)
        {
            Logger.Error("CommsProxy is faulted!");
        }

        private void DisposeCommsSubscription(ICommsMessageSubscription service, SubscriptionState state)
        {
            try
            {
                service.Unregister();
                state.Dispose();
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Exception while disposing SubscriptionState.", exception);
            }
        }

        private class CommSReactiveSubscriptionContext<T>
      where T : CommsMessage
        {
            /// <summary>
            /// Gets or sets the scheduler to be used in the subscription (ObserveOn).
            /// </summary>
            /// <value>
            /// The scheduler.
            /// </value>
            public IScheduler Scheduler { get; set; }

            /// <summary>
            /// Gets or sets the action to be executed to handle each item of type <typeparamref name="T"/>.
            /// </summary>
            /// <value>
            /// The action.
            /// </value>
            public Action<T> Action { get; set; }

            /// <summary>
            /// Gets or sets the action to be executed when the subscription stream is completed.
            /// </summary>
            /// <value>
            /// The on completed.
            /// </value>
            public Action OnCompleted { get; set; }

            /// <summary>
            /// Gets or sets the action to be executed when the subscription stream raises an exception.
            /// </summary>
            /// <value>
            /// The on error.
            /// </value>
            public Action<Exception> OnError { get; set; }
        }

        private StatusUpdateCycle PollUnitStatus(long time)
        {
            Logger.Debug("Polling unit status ({0})", time);
            try
            {
                var comms = ProxyProvider<ICommsService>.Current.Provide();
                var onlineUnitAddresses =
                    comms.GetUnitsConnectionStatus(null)
                         .OrderBy(u => u.NetworkAddress)
                         .Select(
                             u => new UnitStatus(u.NetworkAddress, u.ConnectionStatus == ConnectionStatus.Connected))
                         .ToList();
                Logger.Trace("{0} units are online", onlineUnitAddresses.Count);
                var cycle = new StatusUpdateCycle(onlineUnitAddresses, false);
                return cycle;
            }
            catch (Exception exception)
            {
                Logger.WarnException("Error while polling the status for units.", exception);
                return null;
            }
        }

        private static void AcknowledgeAlarm(string localAddress, string unitAddress)
        {
            try
            {
                Logger.Trace("Acknowledge alarm for unit {0}", unitAddress);
                var commsMessagingService = ProxyProvider<ICommsMessagingService>.Current.Provide();
                var ack = new AlarmAckMessage(localAddress, unitAddress) { Priority = QueuePriority.High };
                commsMessagingService.SendMessage(ack);
            }
            catch (Exception exception)
            {
                const string Format = "Error while acknowledging alarm for unit '{0}'";
                var message = string.Format(Format, unitAddress);
                Logger.ErrorException(message, exception);
            }
        }

        private IDisposable CreateCommSSubscription<T>(
             IObservable<CommsMessage> messageSubject, string subscriptionName, Action<T> action) where T : CommsMessage
        {
            var subscriptionContext = CreateCommSReactiveSubscriptionContext(subscriptionName, action);
            var activitySubscription =
                messageSubject.OfType<T>()
                              .ObserveOn(subscriptionContext.Scheduler)
                              .SubscribeOn(Scheduler.Immediate)
                              .Subscribe(
                                  subscriptionContext.Action,
                                  subscriptionContext.OnError,
                                  subscriptionContext.OnCompleted);
            return activitySubscription;
        }

        private void SubscribeToComms()
        {
            try
            {
                var pollingScheduler =
                    new EventLoopScheduler(
                        start => new Thread(start) { IsBackground = true, Name = "Polling unit status" });
                var observable =
                    Observable.Interval(UnitStatusPollingPeriod).StartWith(-1L).ObserveOn(pollingScheduler).Select(
                        this.PollUnitStatus).Where(c => c != null);
                this.unitStatusPollingSubscription = observable.Subscribe(this.UpdateUnits);
                var messageSubject = new Subject<CommsMessage>();
                var callback = new CommsMessageCallback<CommsMessage>(messageSubject);
                var instanceContext = new InstanceContext(callback);

                var activityStatusSubscription = this.CreateCommSSubscription<ActivityStatusMessage>(
                    messageSubject, "Activity status messages", this.HandleActivityStatusMessage);
                var activitySuccessSubscription = this.CreateCommSSubscription<ActivitySuccessMessage>(
                    messageSubject, "Activity success messages", this.HandleActivitySuccessMessage);
                var connectionSubscription = this.CreateCommSSubscription<ConnectionStatusMessage>(
                    messageSubject, "Connection status messages", this.HandleConnectionStatusMessage);
                var alarmSubscription = this.CreateCommSSubscription<AlarmMessage>(
                    messageSubject, "Alarm messages", this.HandleAlarmMessage);
                var realtimeSubscription = this.CreateCommSSubscription<RealtimeMonitorDataMessage>(
                    messageSubject, "Realtime messages", this.HandleRealtimeMessage);
                var subscriptionContext = CreateCommSReactiveSubscriptionContext<CommsMessage>(
                    "Data request messages", this.HandleDataRequestMessage);

                // This is a special subscription that filters using two data types
                var dataRequestSubscription =
                    messageSubject.Where(
                        message => message is RefTextDataRequestMessage || message is ScheduledDataRequestMessage)
                                  .ObserveOn(subscriptionContext.Scheduler)
                                  .SubscribeOn(Scheduler.Immediate)
                                  .Subscribe(
                                      subscriptionContext.Action,
                                      subscriptionContext.OnError,
                                      subscriptionContext.OnCompleted);
                Func<CommsMessageSubscriptionProxy> commsSubscriptionConstructor =
                    () => new CommsMessageSubscriptionProxy(instanceContext, "commsMessageSubscriptionService");
                Func<ICommsMessageSubscription, SubscriptionState> initialization = service =>
                {
                    var id = service.Register();
                    CreateSubscription(service);
                    Logger.Debug("Duplex client subscribed to Comms with Id '{0}' (alarm messages)", id);
                    var subscription = new SubscriptionState(
                        id,
                        activityStatusSubscription,
                        activitySuccessSubscription,
                        alarmSubscription,
                        connectionSubscription,
                        realtimeSubscription,
                        dataRequestSubscription);
                    return subscription;
                };
                Action<ICommsMessageSubscription> aliveNotification = s =>
                {
                    Logger.Trace("Sending the alive notification");
                    s.AliveNotification();
                };

                var periodicAction = new PeriodicAction<ICommsMessageSubscription>(
                    aliveNotification, TimeSpan.FromMinutes(1));
                this.StartProxyTask(initialization, commsSubscriptionConstructor, periodicAction);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("An error occurred while creating the Comms message subscription.", exception);
            }
        }

        private void HandleRealtimeMessage(RealtimeMonitorDataMessage obj)
        {
            Logger.Trace("Received Realtime message from {0}", obj.SenderAddress);
        }

        private void HandleAlarmMessage(AlarmMessage obj)
        {
            Logger.Trace("Received Alarm message from {0}", obj.SenderAddress);
            AcknowledgeAlarm(obj.Address, obj.SenderAddress);
        }

        private void HandleConnectionStatusMessage(ConnectionStatusMessage obj)
        {
           Logger.Trace("Received connection status Unit: {0}, Status: {1}", obj.SenderAddress, obj.ConnectionStatus);
        }

        private void HandleActivitySuccessMessage(ActivitySuccessMessage obj)
        {
            Logger.Trace("Received ActivitySuccess");
            Thread.Sleep(10000);
            AcknowledgeAlarm(obj.Address, obj.SenderAddress);
        }

        private void HandleDataRequestMessage(CommsMessage obj)
        {
            var referenceTextMessage = new ReferenceTextMessage(obj.SenderAddress)
            {
                DisplayText = "Test",
                ReferenceTextId = 1,
                ReferenceType = ReferenceTextType.TextForDestination,
                Priority = QueuePriority.High
            };
        }

        private void HandleActivityStatusMessage(ActivityStatusMessage obj)
        {
            Logger.Trace("Received ActivityStatusMessage");
            Thread.Sleep(1000);
            AcknowledgeAlarm(obj.Address, obj.SenderAddress);
        }

        private static CommSReactiveSubscriptionContext<T> CreateCommSReactiveSubscriptionContext<T>(
        string subscriptionName, Action<T> action) where T : CommsMessage
        {
            Action<Exception> onError =
                exception =>
                Logger.ErrorException(
                    string.Concat("Error in the Comm.S reactive subscription '", subscriptionName, "'"), exception);
            Action onCompleted = () => Logger.Info(
                "Comm.S reactive subscription '{0}' was completed", subscriptionName);
            var scheduler =
                new EventLoopScheduler(start => new Thread(start) { IsBackground = true, Name = subscriptionName });
            Action<T> safeAction = message =>
            {
                try
                {
                    action(message);
                }
                catch (Exception exception)
                {
                    Logger.ErrorException(
                        string.Concat(
                            "Error while handling message in Comm.S reactive subscription '",
                            subscriptionName,
                            "'"),
                        exception);
                }
            };
            var subscriptionContext = new CommSReactiveSubscriptionContext<T>
            {
                Action = safeAction,
                OnCompleted = onCompleted,
                OnError = onError,
                Scheduler = scheduler
            };
            return subscriptionContext;
        }

        private static void CreateSubscription(ICommsMessageSubscription service)
        {
            service.Subscribe(
                new[]
                    {
                        typeof(AlarmMessage).Name, typeof(ConnectionStatusMessage).Name,
                        typeof(ActivityStatusMessage).Name, typeof(ActivitySuccessMessage).Name,
                        typeof(RealtimeMonitorDataMessage).Name, typeof(RefTextDataRequestMessage).Name,
                        typeof(ScheduledDataRequestMessage).Name
                    });
        }

        private void UpdateUnits(StatusUpdateCycle cycle)
        {
            Logger.Trace("Updating units with a {0}polling cycle.", cycle.IsEvent ? "non-" : string.Empty);
        }

        private class StatusUpdateCycle
        {
            public StatusUpdateCycle(IEnumerable<UnitStatus> statuses, bool isDelta)
            {
                this.Statuses = statuses;
                this.IsEvent = isDelta;
            }

            public IEnumerable<UnitStatus> Statuses { get; private set; }

            public bool IsEvent { get; private set; }
        }
    }
}

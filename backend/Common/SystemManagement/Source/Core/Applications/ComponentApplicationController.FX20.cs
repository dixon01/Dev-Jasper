// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentApplicationController.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ComponentApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Applications
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.Host.Path;

    /// <summary>
    /// Application controller that handles a component (object implementing
    /// <see cref="IApplication"/>).
    /// </summary>
    public partial class ComponentApplicationController
    {
        private IApplication CreateInstanceInNewAppDomain()
        {
            this.appDomain = AppDomain.CreateDomain("Launch " + this.config.ClassName);
            this.Logger.Debug("Created app domain \"{0}\"", this.appDomain.FriendlyName);

            var wrapperType = typeof(AppDomainApplicationWrapper);
            Debug.Assert(wrapperType.FullName != null, "Invalid type");

            var wrapper =
                (AppDomainApplicationWrapper)
                this.appDomain.CreateInstanceAndUnwrap(
                    wrapperType.Assembly.FullName,
                    wrapperType.FullName,
                    false,
                    BindingFlags.Default,
                    null,
                    new object[] { this.config },
                    null,
                    null,
                    null);

            var mediConfig = this.CreateAppDomainMediConfig() ?? new MediConfig();

            using (var writer = new StringWriter())
            {
                new XmlSerializer(typeof(MediConfig)).Serialize(writer, mediConfig);
                writer.Flush();
                wrapper.ConfigureMedi(writer.ToString());
            }

            return wrapper;
        }

        private MediConfig CreateAppDomainMediConfig()
        {
            var file = PathManager.Instance.GetPath(FileType.Config, "medi.config");
            if (!File.Exists(file))
            {
                return null;
            }

            var configMgr = new ConfigManager<MediConfig>();
            configMgr.FileName = file;
            var localMediConfig = configMgr.Config;

            var server =
                localMediConfig.Peers.Find(
                    p => p is ServerPeerConfig && ((ServerPeerConfig)p).Transport is TcpTransportServerConfig) as
                ServerPeerConfig;
            if (server != null)
            {
                var tcpTransportServer = (TcpTransportServerConfig)server.Transport;

                return new MediConfig
                {
                    Peers =
                                   {
                                       new ClientPeerConfig
                                           {
                                               Codec = server.Codec,
                                               Transport = new TcpTransportClientConfig
                                                       {
                                                           RemoteIp = IPAddress.Loopback.ToString(),
                                                           RemotePort = tcpTransportServer.LocalPort
                                                       }
                                           }
                                   }
                };
            }

            var client =
                localMediConfig.Peers.Find(
                    p => p is ClientPeerConfig && ((ClientPeerConfig)p).Transport is TcpTransportClientConfig) as
                ClientPeerConfig;
            if (client != null)
            {
                return new MediConfig { Peers = { client } };
            }

            this.Logger.Warn("Couldn't create client medi config, only TCP is supported");
            return null;
        }
    }
}
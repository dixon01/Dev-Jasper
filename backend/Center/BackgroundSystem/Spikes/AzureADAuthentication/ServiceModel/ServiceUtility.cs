// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceUtility.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The service utility.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ServiceModel
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    /// <summary>
    /// The service utility.
    /// </summary>
    public static class ServiceUtility
    {
        /// <summary>
        /// The get net tcp binding.
        /// </summary>
        /// <returns>
        /// The <see cref="NetTcpBinding"/>.
        /// </returns>
        public static NetTcpBinding GetNetTcpBinding()
        {
            return new NetTcpBinding
            {
                MaxBufferPoolSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                MaxConnections = 1024,
                MaxReceivedMessageSize = int.MaxValue,
                ReaderQuotas =
                    new XmlDictionaryReaderQuotas
                    {
                        MaxArrayLength = int.MaxValue,
                        MaxBytesPerRead = int.MaxValue,
                        MaxDepth = int.MaxValue,
                        MaxNameTableCharCount = int.MaxValue,
                        MaxStringContentLength = int.MaxValue
                    },
                ReceiveTimeout = TimeSpan.FromMinutes(30),
                Security =
                    new NetTcpSecurity
                    {
                        Message =
                            new MessageSecurityOverTcp
                            {
                                ClientCredentialType
                                    =
                                    MessageCredentialType
                                    .UserName
                            },
                        Mode = SecurityMode.TransportWithMessageCredential,
                        Transport =
                            new TcpTransportSecurity
                            {
                                ClientCredentialType
                                    =
                                    TcpClientCredentialType
                                    .None
                            }
                    },
                SendTimeout = TimeSpan.FromMinutes(30),
                TransferMode = TransferMode.Streamed,
            };
        }
    }
}

// -----------------------------------------------------------------------
// <copyright file="CommsServiceProxy.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ProxyProviderTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ServiceModel;

    using Gorba.Center.CommS.Core.ComponentModel;
    using Gorba.Center.CommS.Wcf.ServiceModel;

    /// <summary>
    /// Proxy for the <see cref="ICommsService"/>.
    /// </summary>
    public class CommsServiceProxy : ClientBase<ICommsService>, ICommsService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommsServiceProxy"/> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public CommsServiceProxy(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommsServiceProxy"/> class.
        /// </summary>
        public CommsServiceProxy()
        {
        }

        /// <summary>
        /// Starts the internal CommS engine
        /// </summary>
        public void Start()
        {
            this.Channel.Start();
        }

        /// <summary>
        /// Stops the internal CommS engine
        /// </summary>
        public void Stop()
        {
            this.Channel.Stop();
        }

        /// <summary>
        /// Gets the list of units connection status (DTO units). The DTOUnitConnectionStatus contains the connection status, the last received bytes, and more. 
        /// Please, see <see cref="DTOUnitConnectionStatus"/> for more details.   
        /// </summary>
        /// <param name="networkAddressList">
        /// List of unit network addresses to filter the result.
        /// </param>
        /// <param name="protocol">
        /// The <see cref="CommsHandledProtocol"/> to filter the result.
        /// </param>
        /// <param name="filter">
        /// Enable to filter the resulting list by the status of the connection. See <see cref="ConnectionFilter"/> for available values.
        /// </param>
        /// <returns>
        /// The list of DTOUnit according to the specified network address list and the protocol.
        /// If the list is empty or null, all units for the specified protocol are returned.
        /// </returns>
        public IEnumerable<DTOUnitConnectionStatus> GetUnitsConnectionStatusByProtocol(IList<string> networkAddressList, CommsHandledProtocol protocol, ConnectionFilter filter = ConnectionFilter.All)
        {
            var result = this.Channel.GetUnitsConnectionStatusByProtocol(networkAddressList, protocol, filter);
            return result;
        }

        /// <summary>
        /// Gets the list of units (DTO units). The DTOUnit contains the connection status, the last received bytes, and more. 
        /// Please, see <see cref="DTOUnitConnectionStatus"/> for more details.   
        /// </summary>
        /// <param name="networkAddressList">
        /// List of unit network addresses to filter the result.
        /// </param>
        /// <param name="filter">
        /// Enable to filter the resulting list by the status of the connection. See <see cref="ConnectionFilter"/> for available values.
        /// </param>
        /// <returns>
        /// The list of DTOUnit according to the specified network address list.
        /// If the list is empty or null, all units are returned.
        /// </returns>
        public IEnumerable<DTOUnitConnectionStatus> GetUnitsConnectionStatus(IList<string> networkAddressList, ConnectionFilter filter = ConnectionFilter.All)
        {
            var result = this.Channel.GetUnitsConnectionStatus(networkAddressList, filter);
            return result;
        }
    }
}

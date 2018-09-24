// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestHandlerContext.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestHandlerContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Tests
{
    using System;

    using Gorba.Common.Configuration.Protran.VDV301;
    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Common.Protocols.Ximple.Generic;

    /// <summary>
    /// Implementation of <see cref="IHandlerContext"/> for unit testing.
    /// </summary>
    public class TestHandlerContext : IHandlerContext
    {
        private ICustomerInformationService customerInformationService;

        /// <summary>
        /// Event that is fired when the <see cref="CustomerInformationService"/> changes.
        /// </summary>
        public event EventHandler CustomerInformationServiceChanged;

        /// <summary>
        /// Gets or sets the VDV 301 protocol configuration.
        /// </summary>
        public Vdv301ProtocolConfig Config { get; set; }

        /// <summary>
        /// Gets or sets the generic dictionary.
        /// </summary>
        public Dictionary Dictionary { get; set; }

        /// <summary>
        /// Gets or sets the customer information service.
        /// </summary>
        public ICustomerInformationService CustomerInformationService
        {
            get
            {
                return this.customerInformationService;
            }

            set
            {
                if (this.customerInformationService == value)
                {
                    return;
                }

                this.customerInformationService = value;
                this.RaiseCustomerInformationServiceChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="CustomerInformationServiceChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseCustomerInformationServiceChanged(EventArgs e)
        {
            var handler = this.CustomerInformationServiceChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
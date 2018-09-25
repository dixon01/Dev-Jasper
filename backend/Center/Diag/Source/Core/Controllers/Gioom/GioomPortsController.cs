// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GioomPortsController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GioomPortsController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers.Gioom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Diag.Core.ViewModels.Gioom;
    using Gorba.Common.Gioom.Core;

    /// <summary>
    /// The controller for all GIOoM ports of a unit or an application.
    /// </summary>
    public class GioomPortsController : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GioomPortsController"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        public GioomPortsController(GioomPortsViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.PortControllers = new List<GioomPortControllerBase>();
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public GioomPortsViewModel ViewModel { get; private set; }

        /// <summary>
        /// Gets the list of GIOoM port controllers responsible for the ports.
        /// </summary>
        public List<GioomPortControllerBase> PortControllers { get; private set; }

        /// <summary>
        /// Updates this controller and all its children with the given ports.
        /// </summary>
        /// <param name="ports">
        /// The list of ports received from the remote unit.
        /// </param>
        /// <param name="gioomClient">
        /// The GIOoM client from which the ports were received.
        /// </param>
        public void UpdateFrom(IEnumerable<IPortInfo> ports, GioomClientBase gioomClient)
        {
            foreach (var portInfo in ports)
            {
                this.CreateController(portInfo, gioomClient);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var controller in this.PortControllers)
            {
                controller.Dispose();
            }

            this.PortControllers.Clear();
            this.ViewModel.Ports.Clear();
        }

        private void CreateController(IPortInfo portInfo, GioomClientBase gioomClient)
        {
            if (!portInfo.CanRead
                || this.PortControllers.Any(
                    c => c.ViewModel.Address.Equals(portInfo.Address) && c.ViewModel.Name.Equals(portInfo.Name)))
            {
                // don't add ports that we can't read (e.g. SystemTime) and don't add ports twice
                return;
            }

            var controller = GioomPortControllerBase.Create(portInfo, this.ViewModel.Unit, gioomClient);
            this.PortControllers.Add(controller);
            this.ViewModel.Ports.Add(controller.ViewModel);
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GioomPortControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GioomPortControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers.Gioom
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Diag.Core.ViewModels.Gioom;
    using Gorba.Center.Diag.Core.ViewModels.Unit;
    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;

    using NLog;

    /// <summary>
    /// Base class for all controllers for a single remote GIOoM port.
    /// </summary>
    public abstract class GioomPortControllerBase : SynchronizableControllerBase, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private GioomClientBase gioomClient;

        private IPort port;

        private IOValue currentValue;

        /// <summary>
        /// Gets the view model that is managed by this controller.
        /// </summary>
        public GioomPortViewModelBase ViewModel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GioomPortControllerBase"/> class.
        /// </summary>
        /// <param name="portInfo">
        /// The GIOoM port information.
        /// </param>
        /// <param name="unit">
        /// The unit to which this port belongs.
        /// </param>
        /// <param name="gioomClient">
        /// The GIOoM client.
        /// </param>
        /// <returns>
        /// The <see cref="GioomPortControllerBase"/> implementation.
        /// </returns>
        public static GioomPortControllerBase Create(
            IPortInfo portInfo, UnitViewModelBase unit, GioomClientBase gioomClient)
        {
            var integer = portInfo.ValidValues as IntegerValues;
            if (integer != null)
            {
                var controller = new IntegerGioomPortController();
                controller.Initialize(portInfo, integer, gioomClient, unit);
                return controller;
            }

            var flag = portInfo.ValidValues as FlagValues;
            if (flag != null)
            {
                var controller = new FlagGioomPortController();
                controller.Initialize(portInfo, flag, gioomClient, unit);
                return controller;
            }

            var enumFlag = portInfo.ValidValues as EnumFlagValues;
            if (enumFlag != null)
            {
                var controller = new EnumFlagGioomPortController();
                controller.Initialize(portInfo, enumFlag, gioomClient, unit);
                return controller;
            }

            var enums = portInfo.ValidValues as EnumValues;
            if (enums != null)
            {
                var controller = new EnumGioomPortController();
                controller.Initialize(portInfo, enums, gioomClient, unit);
                return controller;
            }

            throw new ArgumentException("Unsupported port values type: " + portInfo.ValidValues.GetType().Name);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.port == null)
            {
                return;
            }

            this.ViewModel.PropertyChanged -= this.ViewModelOnPropertyChanged;
            this.port.ValueChanged -= this.PortOnValueChanged;
            this.port.Dispose();
            this.port = null;
        }

        /// <summary>
        /// Initializes this controller with the given view model and the GIOoM client.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        protected void Initialize(GioomPortViewModelBase viewModel, GioomClientBase client)
        {
            this.ViewModel = viewModel;
            this.gioomClient = client;
            this.gioomClient.BeginOpenPort(this.ViewModel.Address, this.ViewModel.Name, this.PortOpened, null);
        }

        /// <summary>
        /// Updates the view model with the given value.
        /// </summary>
        /// <param name="value">
        /// The new value.
        /// </param>
        protected virtual void UpdateViewModel(IOValueViewModel value)
        {
            this.ViewModel.Value = value;
        }

        /// <summary>
        /// This method is called when one of the <see cref="ViewModel"/>'s property changes.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the changed property.
        /// </param>
        protected virtual void HandleViewModelPropertyChange(string propertyName)
        {
            if (propertyName == "Value")
            {
                this.UpdatePortValue(this.ViewModel.Value.Value);
            }
        }

        /// <summary>
        /// Updates the remote port's value with the given value.
        /// </summary>
        /// <param name="value">
        /// The new integer value.
        /// </param>
        protected void UpdatePortValue(int value)
        {
            var p = this.port;
            if (p == null)
            {
                return;
            }

            if (this.currentValue != null && value.Equals(this.currentValue.Value))
            {
                return;
            }

            p.Value = p.CreateValue(value);
        }

        private void UpdateViewModel()
        {
            var value = this.port.Value;
            this.currentValue = value;
            this.StartNew(() => this.UpdateViewModel(new IOValueViewModel(value.Name, value.Value)));
        }

        private void PortOpened(IAsyncResult ar)
        {
            try
            {
                this.port = this.gioomClient.EndOpenPort(ar);
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't open port " + this.ViewModel.Name, ex);
                return;
            }

            if (this.port == null)
            {
                Logger.Warn("Couldn't find port " + this.ViewModel.Name);
                return;
            }

            this.port.ValueChanged += this.PortOnValueChanged;
            this.UpdateViewModel();
            this.ViewModel.PropertyChanged += this.ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.HandleViewModelPropertyChange(e.PropertyName);
        }

        private void PortOnValueChanged(object sender, EventArgs e)
        {
            this.UpdateViewModel();
        }
    }
}
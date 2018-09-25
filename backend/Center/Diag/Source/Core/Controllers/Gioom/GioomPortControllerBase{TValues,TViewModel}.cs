// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GioomPortControllerBase{TValues,TViewModel}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GioomPortControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers.Gioom
{
    using Gorba.Center.Diag.Core.ViewModels.Gioom;
    using Gorba.Center.Diag.Core.ViewModels.Unit;
    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// Generic base class for GIOoM port controllers.
    /// </summary>
    /// <typeparam name="TValues">
    /// The type of value supported by the I/O.
    /// </typeparam>
    /// <typeparam name="TViewModel">
    /// The view model that represents the I/O.
    /// </typeparam>
    public abstract class GioomPortControllerBase<TValues, TViewModel> : GioomPortControllerBase
        where TValues : ValuesBase
        where TViewModel : GioomPortViewModelBase, new()
    {
        /// <summary>
        /// Gets the typed view model.
        /// </summary>
        protected new TViewModel ViewModel { get; private set; }

        /// <summary>
        /// Initializes this controller.
        /// </summary>
        /// <param name="portInfo">
        /// The GIOoM port information.
        /// </param>
        /// <param name="values">
        /// The possible values of the port.
        /// </param>
        /// <param name="gioomClient">
        /// The GIOoM client.
        /// </param>
        /// <param name="unit">
        /// The unit to which the port belongs.
        /// </param>
        internal void Initialize(
            IPortInfo portInfo, TValues values, GioomClientBase gioomClient, UnitViewModelBase unit)
        {
            this.ViewModel = new TViewModel
                                 {
                                     Name = portInfo.Name,
                                     Address = portInfo.Address,
                                     IsReadable = portInfo.CanRead,
                                     IsWritable = portInfo.CanWrite,
                                     Unit = unit
                                 };
            this.PrepareViewModel(values);
            this.Initialize(this.ViewModel, gioomClient);
        }

        /// <summary>
        /// Prepares the view model with the given possible values.
        /// </summary>
        /// <param name="values">
        /// The possible values of the port.
        /// </param>
        protected abstract void PrepareViewModel(TValues values);
    }
}

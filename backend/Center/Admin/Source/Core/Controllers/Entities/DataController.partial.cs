// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities
{
    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Common.Wpf.Client.Controllers;

    /// <summary>
    /// The controller responsible for all entity data in the icenter.admin application.
    /// </summary>
    public partial class DataController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataController"/> class.
        /// </summary>
        /// <param name="factory">
        /// The data view model factory.
        /// </param>
        public DataController(DataViewModelFactory factory)
        {
            this.Factory = factory;
            this.CreateControllers();
        }

        /// <summary>
        /// Gets the connection controller.
        /// </summary>
        public IConnectionController ConnectionController { get; private set; }

        /// <summary>
        /// Gets the data view model factory.
        /// </summary>
        public DataViewModelFactory Factory { get; private set; }

        /// <summary>
        /// Initializes this controller with the given <see cref="ConnectionController"/>.
        /// </summary>
        /// <param name="connectionController">
        /// The connection controller.
        /// </param>
        public void Initialize(IConnectionController connectionController)
        {
            this.ConnectionController = connectionController;
            this.InitializeControllers();
        }
    }
}

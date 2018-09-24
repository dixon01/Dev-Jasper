// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisControlPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.ThorebC90
{
    using System;
    using System.IO;
    using System.Text;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.ServiceModel.Units;

    /// <summary>
    /// The IBIS control part controller.
    /// </summary>
    public class IbisControlPartController : XmlEditorPartControllerBase
    {
        private const string IbisControlConfigurationKey = "IbisConfiguration";

        private const string IbisControlConfigFileName = "ibis.xml";

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisControlPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public IbisControlPartController(CategoryControllerBase parent)
            : base(
                UnitConfigKeys.ThorebC90C74.Ibis,
                parent,
                IbisControlConfigurationKey,
                IbisControlConfigFileName,
                AdminStrings.UnitConfig_ThorebC90_Ibis,
                AdminStrings.UnitConfig_ThorebC90_Ibis_Description)
        {
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="XmlPartViewModel"/>.
        /// </returns>
        protected override XmlPartViewModel CreateViewModel()
        {
            var viewModel = base.CreateViewModel();
            viewModel.IsVisible = this.Parent.Parent.UnitConfiguration.ProductType.UnitType == UnitTypes.Obu;
            return viewModel;
        }

        /// <summary>
        /// Gets the default configuration.
        /// </summary>
        /// <returns>
        /// The default configuration as string
        /// </returns>
        protected override string GetDefaultConfig()
        {
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "ibis.xml");
            return stream == null ? null : new StreamReader(stream, Encoding.UTF8).ReadToEnd();
        }
    }
}

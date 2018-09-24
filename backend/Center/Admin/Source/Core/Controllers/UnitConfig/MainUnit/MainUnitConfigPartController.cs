// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainUnitConfigPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.MainUnit
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Common.Configuration.EPaper.MainUnit;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The main unit config part controller.
    /// </summary>
    public class MainUnitConfigPartController : XmlEditorPartControllerBase
    {
         private const string MainUnitConfigurationKey = "MainUnitConfiguration";

        private const string MainUnitConfigFileName = "MainUnit.xml";

        private readonly DateTime defaultOperationDayStartUtc = new DateTime(2000, 1, 1, 4, 0, 0, DateTimeKind.Utc);

        private MainUnitConfig defaultConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainUnitConfigPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public MainUnitConfigPartController(CategoryControllerBase parent)
            : base(
                UnitConfigKeys.MainUnit.Category,
                parent,
                MainUnitConfigurationKey,
                MainUnitConfigFileName,
                AdminStrings.UnitConfig_MainUnit_Configuration,
                AdminStrings.UnitConfig_MainUnit_Configuration_Description)
        {
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            base.Load(partData);
            try
            {
                var schema = MainUnitConfig.Schema;
                this.ViewModel.Editor.Config.Schema = schema;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while getting the xsd for MainUnit.");
            }
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            base.Prepare(descriptor);
            var unitDescriptor = descriptor.Platform as PowerUnitPlatformDescriptor;
            if (unitDescriptor == null)
            {
                return;
            }

            this.defaultConfig = new MainUnitConfig
                                     {
                                         FirmwareHash = string.Empty,
                                         OperationDayStartUtc = this.defaultOperationDayStartUtc
                                     };
            this.defaultConfig.DisplayUnits.Clear();
            foreach (var displayUnitDescriptor in unitDescriptor.DisplayUnits)
            {
                this.defaultConfig.DisplayUnits.Add(new DisplayUnitConfig { FirmwareHash = string.Empty });
                if (displayUnitDescriptor.HasInfoline)
                {
                    this.defaultConfig.LcdConfig = new LcdConfig { RefreshInterval = TimeSpan.FromMinutes(5) };
                }
            }
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
            viewModel.IsVisible = this.Parent.Parent.UnitConfiguration.ProductType.UnitType == UnitTypes.EPaper;
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
            if (this.defaultConfig == null)
            {
                var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), MainUnitConfigFileName);
                return stream == null ? null : new StreamReader(stream, Encoding.UTF8).ReadToEnd();
            }

            var xmlSerializer = new XmlSerializer(typeof(MainUnitConfig));
            using (var memory = new MemoryStream())
            {
                xmlSerializer.Serialize(new StreamWriter(memory, Encoding.UTF8), this.defaultConfig);
                return Encoding.UTF8.GetString(memory.ToArray());
            }
        }
    }
}

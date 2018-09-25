// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenPartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SplashScreenPartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.SplashScreens
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Media;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Common.Configuration.SystemManager.SplashScreen;
    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;

    /// <summary>
    /// The part controller base class for all splash screen configurations.
    /// </summary>
    public abstract class SplashScreenPartControllerBase : MultiEditorPartControllerBase
    {
        private const string EnabledKey = "Enabled";
        private const string UseDefaultLogoKey = "UseDefaultLogo";
        private const string LogoKey = "Logo";
        private const string ForegroundKey = "Foreground";
        private const string BackgroundKey = "Background";
        private const string SelectedItemsKey = "SelectedItems";

        private const string TemperatureIoName = "Temperature";
        private const string IbisAddressIoName = "IbisAddress0";
        private const string IbisScreenLabel = "IBIS Address";
        private const string AtmelScreenLabel = "Atmel Controller Version";

        private const string HardwareScreenLabel = "HardwareManager";

        private const string TemperaturScreenLabel = "{0}°C";
        private const string AtRevisionPath = @"\HardwareManager\MGI\At91Rev";

        private CheckableEditorViewModel enabled;

        private CheckableEditorViewModel useDefaultLogo;

        private ResourceUploadEditorViewModel logo;

        private ColorEditorViewModel foreground;

        private ColorEditorViewModel background;

        private CheckableTreeEditorViewModel showItems;

        private CheckableTreeNodeViewModel ibisAddressItem;

        private CheckableTreeNodeViewModel temperatureItem;

        private CheckableTreeNodeViewModel atRevisionItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreenPartControllerBase"/> class.
        /// </summary>
        /// <param name="key">
        /// The unique key to identify this part.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected SplashScreenPartControllerBase(string key, CategoryControllerBase parent)
            : base(key, parent)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this splash screen is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return !this.CanDisable || (this.enabled.IsChecked.HasValue && this.enabled.IsChecked.Value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether to use the default logo.
        /// </summary>
        public bool UseDefaultLogo
        {
            get
            {
                return this.useDefaultLogo.IsChecked.HasValue && this.useDefaultLogo.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets the logo image resource hash.
        /// </summary>
        public string LogoResourceHash
        {
            get
            {
                return this.logo.ResourceHash;
            }
        }

        /// <summary>
        /// Gets the foreground color.
        /// </summary>
        public Color Foreground
        {
            get
            {
                return this.foreground.Color;
            }
        }

        /// <summary>
        /// Gets the background color.
        /// </summary>
        public Color Background
        {
            get
            {
                return this.background.Color;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this splash screen can be disabled.
        /// </summary>
        protected virtual bool CanDisable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the list of all splash screen items that have at least some selected items.
        /// </summary>
        /// <returns>
        /// The list of all splash screen items (except the logo item).
        /// </returns>
        public IEnumerable<SplashScreenItemBase> GetSelectedItems()
        {
            if (this.temperatureItem.IsChecked.HasValue && this.temperatureItem.IsChecked.Value)
            {
                yield return new GioomSplashScreenItem
                                 {
                                     Name = TemperatureIoName, ValueFormat = TemperaturScreenLabel
                                 };
            }

            if (this.ibisAddressItem.IsChecked.HasValue && this.ibisAddressItem.IsChecked.Value)
            {
                yield return new GioomSplashScreenItem { Name = IbisAddressIoName, Label = IbisScreenLabel };
            }

            if (this.atRevisionItem.IsChecked.HasValue && this.atRevisionItem.IsChecked.Value)
            {
                yield return
                    new ManagementSplashScreenItem
                        {
                            Path = AtRevisionPath,
                            Application = HardwareScreenLabel,
                            Label = AtmelScreenLabel
                        };
            }

            foreach (
                var parent in
                    this.showItems.Root.Children.Where(
                        c => c.Value is Type && (!c.IsChecked.HasValue || c.IsChecked.Value)))
            {
                var type = (Type)parent.Value;
                var item = (SplashScreenItemBase)Activator.CreateInstance(type);
                foreach (var child in parent.Children)
                {
                    var property = (PropertyInfo)child.Value;
                    property.SetValue(item, child.IsChecked.HasValue && child.IsChecked.Value);
                }

                yield return item;
            }
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            this.enabled.IsChecked = !this.CanDisable || partData.GetValue(true, EnabledKey);
            this.logo.ResourceHash = partData.GetValue((string)null, LogoKey);
            this.useDefaultLogo.IsChecked = partData.GetValue(true, UseDefaultLogoKey);
            this.foreground.Color = partData.GetValue(Colors.Black, ForegroundKey);
            this.background.Color = partData.GetValue(Color.FromRgb(0xE6, 0xEC, 0xF0), BackgroundKey);
            var splashScreenConfig =
                partData.GetXmlValue(
                    new SplashScreenConfig
                        {
                            Items =
                                {
                                    new GioomSplashScreenItem
                                        {
                                            ValueFormat = TemperaturScreenLabel,
                                            Name = TemperatureIoName
                                        },
                                    new GioomSplashScreenItem
                                        {
                                            Label = IbisScreenLabel,
                                            Name = IbisAddressIoName
                                        },
                                    new ManagementSplashScreenItem
                                        {
                                            Label =
                                                AtmelScreenLabel,
                                            Application =
                                                HardwareScreenLabel,
                                            Path = AtRevisionPath
                                        },
                                    new SystemSplashScreenItem
                                        {
                                            MachineName = true,
                                            Uptime = true,
                                            Serial = true
                                        },
                                    new NetworkSplashScreenItem
                                        {
                                            Name = true,
                                            Ip = true,
                                            Mac = true,
                                            Status = true
                                        },
                                    new ApplicationsSplashScreenItem
                                        {
                                            Version = true,
                                            State = true
                                        }
                                }
                        },
                    SelectedItemsKey);

            this.temperatureItem.IsChecked =
                splashScreenConfig.Items.OfType<GioomSplashScreenItem>().Any(i => i.Name == TemperatureIoName);
            this.ibisAddressItem.IsChecked =
                splashScreenConfig.Items.OfType<GioomSplashScreenItem>().Any(i => i.Name == IbisAddressIoName);
            this.atRevisionItem.IsChecked =
                splashScreenConfig.Items.OfType<ManagementSplashScreenItem>().Any(i => i.Path == AtRevisionPath);

            foreach (var parent in this.showItems.Root.Children.Where(c => c.Value is Type))
            {
                var itemType = (Type)parent.Value;
                var selected = splashScreenConfig.Items.FirstOrDefault(i => i.GetType() == itemType);
                if (selected == null)
                {
                    parent.IsChecked = false;
                    continue;
                }

                foreach (var child in parent.Children.Where(c => c.Value is PropertyInfo))
                {
                    var property = (PropertyInfo)child.Value;
                    child.IsChecked = (bool)property.GetValue(selected);
                }
            }

            this.UpdateEditors();
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.IsEnabled, EnabledKey);
            partData.SetValue(this.LogoResourceHash, LogoKey);
            partData.SetValue(this.UseDefaultLogo, UseDefaultLogoKey);
            partData.SetValue(this.Foreground, ForegroundKey);
            partData.SetValue(this.Background, BackgroundKey);
            partData.SetXmlValue(new SplashScreenConfig { Items = this.GetSelectedItems().ToList() }, SelectedItemsKey);
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            var viewModel = new MultiEditorPartViewModel { IsVisible = true };

            this.enabled = new CheckableEditorViewModel();
            this.enabled.IsThreeState = false;
            this.enabled.Label = AdminStrings.UnitConfig_SplashScreens_Enabled;
            this.enabled.IsEnabled = this.CanDisable;
            viewModel.Editors.Add(this.enabled);

            this.useDefaultLogo = new CheckableEditorViewModel();
            this.useDefaultLogo.IsThreeState = false;
            this.useDefaultLogo.Label = AdminStrings.UnitConfig_SplashScreens_UseDefaultLogo;
            viewModel.Editors.Add(this.useDefaultLogo);

            this.logo =
                new ResourceUploadEditorViewModel(
                    this.Parent.Parent.DataController.ConnectionController.CreateChannelScope<IResourceService>());
            this.logo.FileFilters = AdminStrings.ImageFileFilters;
            this.logo.Label = AdminStrings.UnitConfig_SplashScreens_Logo;
            viewModel.Editors.Add(this.logo);

            this.foreground = new ColorEditorViewModel();
            this.foreground.Label = AdminStrings.UnitConfig_SplashScreens_Foreground;
            viewModel.Editors.Add(this.foreground);

            this.background = new ColorEditorViewModel();
            this.background.Label = AdminStrings.UnitConfig_SplashScreens_Background;
            viewModel.Editors.Add(this.background);

            this.showItems = new CheckableTreeEditorViewModel();
            this.showItems.Label = AdminStrings.UnitConfig_SplashScreens_ShowItems;

            var misc = new CheckableTreeNodeViewModel();
            misc.Label = AdminStrings.UnitConfig_SplashScreens_ShowItems_Misc;
            this.showItems.Root.Children.Add(misc);

            this.ibisAddressItem = new CheckableTreeNodeViewModel();
            this.ibisAddressItem.Label = AdminStrings.UnitConfig_SplashScreens_ShowItems_Misc_IbisAddress;
            misc.Children.Add(this.ibisAddressItem);

            this.temperatureItem = new CheckableTreeNodeViewModel();
            this.temperatureItem.Label = AdminStrings.UnitConfig_SplashScreens_ShowItems_Misc_Temperature;
            misc.Children.Add(this.temperatureItem);

            this.atRevisionItem = new CheckableTreeNodeViewModel();
            this.atRevisionItem.Label = AdminStrings.UnitConfig_SplashScreens_ShowItems_Misc_AtRevision;
            misc.Children.Add(this.atRevisionItem);

            this.AddShowItems<SystemSplashScreenItem>();
            this.AddShowItems<NetworkSplashScreenItem>();
            this.AddShowItems<ApplicationsSplashScreenItem>();
            viewModel.Editors.Add(this.showItems);

            return viewModel;
        }

        /// <summary>
        /// Raises the <see cref="PartControllerBase.ViewModelUpdated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseViewModelUpdated(EventArgs e)
        {
            base.RaiseViewModelUpdated(e);
            this.UpdateEditors();
        }

        /// <summary>
        /// Updates all editors (errors and enabled flag).
        /// </summary>
        protected virtual void UpdateEditors()
        {
            this.useDefaultLogo.IsEnabled = this.IsEnabled;
            this.foreground.IsEnabled = this.IsEnabled;
            this.background.IsEnabled = this.IsEnabled;
            this.showItems.IsEnabled = this.IsEnabled;

            this.logo.IsEnabled = this.IsEnabled && !this.UseDefaultLogo;

            var errorState = this.logo.IsEnabled && string.IsNullOrEmpty(this.LogoResourceHash)
                                 ? ErrorState.Error
                                 : ErrorState.Ok;
            this.logo.SetError("Filename", errorState, AdminStrings.Errors_NoResourceSelected);
        }

        private static string GetItemsString(string namePart)
        {
            return AdminStrings.ResourceManager.GetString(
                "UnitConfig_SplashScreens_ShowItems_" + namePart,
                AdminStrings.Culture) ?? namePart;
        }

        private void AddShowItems<T>() where T : SplashScreenItemBase
        {
            var type = typeof(T);
            var name = type.Name.Replace("SplashScreenItem", string.Empty);
            var parent = new CheckableTreeNodeViewModel();
            parent.Label = GetItemsString(name);
            parent.Value = type;
            this.showItems.Root.Children.Add(parent);

            foreach (var property in type.GetProperties().Where(p => p.PropertyType == typeof(bool)))
            {
                var child = new CheckableTreeNodeViewModel();
                child.Label = GetItemsString(name + "_" + property.Name);
                child.Value = property;
                parent.Children.Add(child);
            }
        }
    }
}
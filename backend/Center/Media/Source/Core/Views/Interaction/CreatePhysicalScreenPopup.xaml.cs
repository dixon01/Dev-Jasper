// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreatePhysicalScreenPopup.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for CreatePhysicalScreenPopup.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Interaction
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Interaction logic for CreatePhysicalScreenPopup.xaml
    /// </summary>
    public partial class CreatePhysicalScreenPopup
    {
        /// <summary>
        /// The create command property.
        /// </summary>
        public static readonly DependencyProperty CreateCommandProperty = DependencyProperty.Register(
            "CreateCommand",
            typeof(ICommand),
            typeof(CreatePhysicalScreenPopup),
            new PropertyMetadata(default(ICommand)));

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ExtendedObservableCollection<ResolutionConfiguration> resolutions;

        private readonly ExtendedObservableCollection<MasterLayout> masterLayouts;

        private readonly Lazy<MediaConfiguration> lazyMediaConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePhysicalScreenPopup"/> class.
        /// </summary>
        public CreatePhysicalScreenPopup()
        {
            this.resolutions = new ExtendedObservableCollection<ResolutionConfiguration>();
            this.masterLayouts = new ExtendedObservableCollection<MasterLayout>();
            this.ScreenTypes = new ExtendedObservableCollection<string>();
            this.lazyMediaConfiguration = new Lazy<MediaConfiguration>(GetMediaConfiguration);
            this.InitializeComponent();
            this.Loaded += this.OnPopupLoaded;
        }

        /// <summary>
        /// Gets or sets the create command.
        /// </summary>
        public ICommand CreateCommand
        {
            get
            {
                return (ICommand)this.GetValue(CreateCommandProperty);
            }

            set
            {
                this.SetValue(CreateCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the screen types.
        /// </summary>
        public ExtendedObservableCollection<string> ScreenTypes { get; set; }

        /// <summary>
        /// Gets the available resolutions.
        /// </summary>
        public ExtendedObservableCollection<ResolutionConfiguration> AvailableResolutions
        {
            get
            {
                return this.resolutions;
            }
        }

        /// <summary>
        /// Gets the master layout templates.
        /// </summary>
        public ExtendedObservableCollection<MasterLayout> MasterLayoutTemplates
        {
            get
            {
                return this.masterLayouts;
            }
        }

        /// <summary>
        /// Gets the create cycle command wrapper.
        /// </summary>
        public ICommand CreatePhysicalScreenCommandWrapper => new RelayCommand(this.OnCreatePhysicalScreen, this.CanCreatePhysicalScreen);

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(this.Cancel);
            }
        }

        /// <summary>
        /// Gets the media configuration.
        /// </summary>
        public MediaConfiguration MediaConfiguration
        {
            get
            {
                return this.lazyMediaConfiguration.Value;
            }
        }

        private static MediaConfiguration GetMediaConfiguration()
        {
            return ServiceLocator.Current.GetInstance<MediaConfiguration>();
        }

        private void Cancel()
        {
            this.Close();
        }

        private void OnPopupLoaded(object sender, RoutedEventArgs e)
        {
            this.ScreenTypeCombobox.SelectedIndex = 1;
            this.ScreenTypeCombobox.SelectedIndex = 0;
        }

        private bool CanCreatePhysicalScreen(object o)
        {
            var selectedType = this.ScreenTypeCombobox.SelectedItem as PhysicalScreenTypeConfig;
            if (this.CreateCommand != null && !this.CreateCommand.CanExecute(null))
            {
                return false;
            }

            if (selectedType != null)
            {
                if (selectedType.AvailableResolutions.Count == 0)
                {
                    return true;
                }

                var selectedResolution = this.ResolutionCombobox.SelectedItem as ResolutionConfiguration;
                if (selectedResolution != null)
                {
                    if (selectedResolution.MasterLayouts.Count == 0)
                    {
                        return true;
                    }

                    var layout = this.MasterLayoutList.SelectedItem as MasterLayout;
                    if (layout != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void OnCreatePhysicalScreen()
        {
            if (this.ScreenTypeCombobox.SelectedItem == null)
            {
                return;
            }

            var selectedScreenType = this.ScreenTypeCombobox.SelectedItem as PhysicalScreenTypeConfig;

            if (selectedScreenType == null)
            {
                Logger.Error("Unknown combobox value selected.");
                return;
            }

            PhysicalScreenType screenType;
            var isMonochrome = false;
            if (selectedScreenType.Name.StartsWith("LED", StringComparison.InvariantCultureIgnoreCase))
            {
                screenType = PhysicalScreenType.LED;
                isMonochrome = !selectedScreenType.Name.EndsWith("Color", StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                screenType = (PhysicalScreenType)Enum.Parse(typeof(PhysicalScreenType), selectedScreenType.Name);
            }

            var parameters = new CreatePhysicalScreenParameters
                                 {
                                     Type = screenType,
                                     IsMonochrome = isMonochrome
                                 };
            if (selectedScreenType.AvailableResolutions.Count > 0)
            {
                var selectedResolution = this.ResolutionCombobox.SelectedItem as ResolutionConfiguration;
                if (selectedResolution == null)
                {
                    Logger.Error("Unknown combobox value selected.");
                    return;
                }

                parameters.Resolution = selectedResolution;
                if (selectedResolution.MasterLayouts.Count > 0)
                {
                    var selectedMasterLayout = this.MasterLayoutList.SelectedItem as MasterLayout;
                    if (selectedMasterLayout == null)
                    {
                        Logger.Error("MasterLayout not selected.");
                        return;
                    }

                    parameters.MasterLayout = selectedMasterLayout;
                }
            }

            var physicalScreenPrompt = this.DataContext as CreatePhysicalScreenPrompt;
            if (physicalScreenPrompt != null)
            {
                physicalScreenPrompt.CreatePhysicalScreenCommand.Execute(parameters);
            }
            else
            {
                var projectPrompt = this.DataContext as MainMenuPrompt;
                if (projectPrompt != null)
                {
                    this.CreateCommand.Execute(parameters);
                }
            }

            this.Close();
        }

        private void ScreenTypeCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext == null)
            {
                return;
            }

            var selectedScreenType = this.ScreenTypeCombobox.SelectedItem as PhysicalScreenTypeConfig;
            if (selectedScreenType == null)
            {
                return;
            }

            this.AvailableResolutions.Clear();

            this.MasterLayoutListLabel.Visibility = Visibility.Hidden;
            this.MasterLayoutList.Visibility = Visibility.Hidden;
            if (selectedScreenType.AvailableResolutions.Count == 0)
            {
                this.ResolutionComboboxLabel.Visibility = Visibility.Hidden;
                this.ResolutionCombobox.Visibility = Visibility.Hidden;
                return;
            }

            this.ResolutionComboboxLabel.Visibility = Visibility.Visible;
            this.ResolutionCombobox.Visibility = Visibility.Visible;
            selectedScreenType.AvailableResolutions.ForEach(this.AvailableResolutions.Add);
            this.ResolutionCombobox.SelectedIndex = 0;
        }

        private void ResolutionCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedResolution = this.ResolutionCombobox.SelectedItem as ResolutionConfiguration;
            if (selectedResolution == null)
            {
                return;
            }

            this.MasterLayoutTemplates.Clear();
            if (selectedResolution.MasterLayouts.Count == 0)
            {
                this.MasterLayoutListLabel.Visibility = Visibility.Hidden;
                this.MasterLayoutList.Visibility = Visibility.Hidden;
                return;
            }

            selectedResolution.MasterLayouts.ForEach(this.MasterLayoutTemplates.Add);
            this.MasterLayoutListLabel.Visibility = Visibility.Visible;
            this.MasterLayoutList.Visibility = Visibility.Visible;
            this.MasterLayoutList.SelectedItem = this.MasterLayoutTemplates.FirstOrDefault();
        }

        private void ComboboxOnDropDownOpened(object sender, EventArgs e)
        {
            var physicalScreenPrompt = this.DataContext as CreatePhysicalScreenPrompt;
            if (physicalScreenPrompt != null)
            {
                physicalScreenPrompt.SuppressMouseEvents = true;
                return;
            }

            var prompt = this.DataContext as MainMenuPrompt;
            if (prompt != null)
            {
                prompt.SuppressMouseEvents = true;
            }
        }

        private void ComboboxOnDropDownClosed(object sender, EventArgs e)
        {
            var physicalScreenPrompt = this.DataContext as CreatePhysicalScreenPrompt;
            if (physicalScreenPrompt != null)
            {
                physicalScreenPrompt.SuppressMouseEvents = false;
                return;
            }

            var prompt = this.DataContext as MainMenuPrompt;
            if (prompt != null)
            {
                prompt.SuppressMouseEvents = false;
            }
        }
    }
}

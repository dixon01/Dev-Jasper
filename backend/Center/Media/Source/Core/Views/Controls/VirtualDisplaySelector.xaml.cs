// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualDisplaySelector.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for VirtualDisplaySelector.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Controls
{
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;

    using NLog;

    /// <summary>
    /// Interaction logic for VirtualDisplaySelector.xaml
    /// </summary>
    public partial class VirtualDisplaySelector
    {
        /// <summary>
        /// The command property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(VirtualDisplaySelector), new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// The master layout property.
        /// </summary>
        public static readonly DependencyProperty MasterLayoutProperty = DependencyProperty.Register(
            "MasterLayout",
            typeof(MasterLayout),
            typeof(VirtualDisplaySelector),
            new PropertyMetadata(default(MasterLayout), MasterLayoutChanged));

        /// <summary>
        /// The selected virtual display property.
        /// </summary>
        public static readonly DependencyProperty SelectedVirtualDisplayProperty =
            DependencyProperty.Register(
                "SelectedVirtualDisplay",
                typeof(VirtualDisplayConfigDataViewModel),
                typeof(VirtualDisplaySelector),
                new PropertyMetadata(default(VirtualDisplayConfigDataViewModel), OnSelectedVirtualDisplayChanged));

        /// <summary>
        /// The virtual display references property.
        /// </summary>
        public static readonly DependencyProperty VirtualDisplayReferencesProperty =
            DependencyProperty.Register(
                "VirtualDisplayReferences",
                typeof(ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel>),
                typeof(VirtualDisplaySelector),
                new PropertyMetadata(
                    default(ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel>),
                    OnVirtualDisplayReferencesChanged));

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualDisplaySelector"/> class.
        /// </summary>
        public VirtualDisplaySelector()
        {
            InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }

            set
            {
                SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the master layout configuration.
        /// </summary>
        public MasterLayout MasterLayout
        {
            get
            {
                return (MasterLayout)GetValue(MasterLayoutProperty);
            }

            set
            {
                SetValue(MasterLayoutProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected virtual display.
        /// </summary>
        public VirtualDisplayConfigDataViewModel SelectedVirtualDisplay
        {
            get
            {
                return (VirtualDisplayConfigDataViewModel)GetValue(SelectedVirtualDisplayProperty);
            }

            set
            {
                SetValue(SelectedVirtualDisplayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the virtual display references.
        /// </summary>
        public ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel> VirtualDisplayReferences
        {
            get
            {
                return
                    (ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel>)
                    GetValue(VirtualDisplayReferencesProperty);
            }

            set
            {
                SetValue(VirtualDisplayReferencesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the buttons grid.
        /// </summary>
        public Grid ButtonsGrid { get; set; }

        /// <summary>
        /// Creates the buttons for the virtual displays.
        /// </summary>
        public void CreateButtons()
        {
            if (this.MasterLayout == null)
            {
                return;
            }

            this.ButtonsGrid = this.GenerateGrid();

            var buttonIndex = 0;
            for (var row = 0; row < this.MasterLayout.RowHeights.Count; row++)
            {
                for (var column = 0; column < this.MasterLayout.ColumnWidths.Count; column++)
                {
                    var button = new RadioButton
                    {
                        Content = buttonIndex.ToString(CultureInfo.InvariantCulture),
                        Command = this.Command,
                    };

                    Grid.SetColumn(button, column);
                    Grid.SetRow(button, row);
                    this.ButtonsGrid.Children.Add(button);
                    buttonIndex++;
                }
            }

            this.SetButtonTooltips();
            this.ResetButtonCommandParameters();
            this.RootGrid.Children.Clear();
            this.RootGrid.Children.Add(this.ButtonsGrid);
        }

        /// <summary>
        /// The reset button command parameters.
        /// </summary>
        public void ResetButtonCommandParameters()
        {
            var buttonIndex = 0;
            if (this.ButtonsGrid == null)
            {
                return;
            }

            foreach (var button in this.ButtonsGrid.Children.Cast<RadioButton>())
            {
                if (this.VirtualDisplayReferences != null && this.VirtualDisplayReferences.Count > buttonIndex)
                {
                    var virtualDisplay = this.VirtualDisplayReferences[buttonIndex];
                    button.CommandParameter = virtualDisplay;
                    if (this.SelectedVirtualDisplay != null
                        && virtualDisplay.ReferenceName == this.SelectedVirtualDisplay.Name.Value)
                    {
                        button.IsChecked = true;
                    }
                }

                buttonIndex++;
            }
        }

        private static void MasterLayoutChanged(
        DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (VirtualDisplaySelector)dependencyObject;
            control.CreateButtons();
        }

        private static void OnSelectedVirtualDisplayChanged(
            DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (VirtualDisplaySelector)dependencyObject;

            control.SetButtonTooltips();
            control.ResetButtonCommandParameters();
            foreach (var child in control.ButtonsGrid.Children)
            {
                var button = child as RadioButton;
                if (button == null)
                {
                    continue;
                }

                if (control.SelectedVirtualDisplay != null && button.CommandParameter != null)
                {
                    button.IsChecked = control.SelectedVirtualDisplay.Name.Value
                                       == ((VirtualDisplayRefConfigDataViewModel)button.CommandParameter).ReferenceName;
                }
                else
                {
                    button.IsChecked = false;
                }
            }
        }

        private static void OnVirtualDisplayReferencesChanged(
            DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (VirtualDisplaySelector)dependencyObject;
            control.SetButtonTooltips();
            control.ResetButtonCommandParameters();
        }

        private void SetButtonTooltips()
        {
            var buttonIndex = 0;
            if (this.ButtonsGrid == null || this.VirtualDisplayReferences == null)
            {
                return;
            }

            foreach (var button in this.ButtonsGrid.Children.Cast<RadioButton>())
            {
                if (this.VirtualDisplayReferences.Count > buttonIndex)
                {
                    var virtualDisplay = this.VirtualDisplayReferences[buttonIndex];
                    button.ToolTip = virtualDisplay.Reference.Width.Value + "x" + virtualDisplay.Reference.Height.Value;
                }

                buttonIndex++;
            }
        }

        private Grid GenerateGrid()
        {
            var dynamicGrid = new Grid();
            foreach (var row in this.MasterLayout.RowHeights)
            {
                GridLength rowHeight;
                if (row == "*")
                {
                    rowHeight = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    int height;
                    if (int.TryParse(row, out height))
                    {
                        rowHeight = new GridLength(height);
                    }
                    else
                    {
                        Logger.Warn(
                            "RowHeight {0} from MediaConfiguration is not an integer value. Stretching the row.", row);
                        rowHeight = new GridLength(1, GridUnitType.Star);
                    }
                }

                var rowDefinition = new RowDefinition { Height = rowHeight };
                dynamicGrid.RowDefinitions.Add(rowDefinition);
            }

            foreach (var column in this.MasterLayout.ColumnWidths)
            {
                GridLength columnWidth;
                if (column == "*")
                {
                    columnWidth = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    int width;
                    if (int.TryParse(column, out width))
                    {
                        columnWidth = new GridLength(width);
                    }
                    else
                    {
                        Logger.Warn(
                            "ColumnWidth {0} from MediaConfiguration is not an integer value. Stretching the column.",
                            column);
                        columnWidth = new GridLength(1, GridUnitType.Star);
                    }
                }

                var columnDefinition = new ColumnDefinition { Width = columnWidth };
                dynamicGrid.ColumnDefinitions.Add(columnDefinition);
            }

            return dynamicGrid;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.CreateButtons();
        }
    }
}

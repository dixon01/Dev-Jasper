// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoListControl.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for ApplicationIoControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views.Controls
{
    using System.Collections.ObjectModel;
    using System.Windows;

    using Gorba.Center.Diag.Core.ViewModels.Gioom;

    /// <summary>
    /// Interaction logic for ApplicationIoControl.xaml
    /// </summary>
    public partial class IoListControl
    {
        /// <summary>
        /// The ports property.
        /// </summary>
        public static readonly DependencyProperty PortsProperty = DependencyProperty.Register(
            "Ports",
            typeof(ObservableCollection<GioomPortViewModelBase>),
            typeof(IoListControl),
            new PropertyMetadata(default(ObservableCollection<GioomPortViewModelBase>)));

        /// <summary>
        /// Initializes a new instance of the <see cref="IoListControl"/> class.
        /// </summary>
        public IoListControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the ports.
        /// </summary>
        public ObservableCollection<GioomPortViewModelBase> Ports
        {
            get
            {
                return (ObservableCollection<GioomPortViewModelBase>)this.GetValue(PortsProperty);
            }

            set
            {
                this.SetValue(PortsProperty, value);
            }
        }
    }
}

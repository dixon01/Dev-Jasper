// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationInfoTabView.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ApplicationInfoTabView.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using Telerik.Windows;
    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for ApplicationInfoTabView.xaml
    /// </summary>
    public partial class ApplicationInfoTabView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInfoTabView" /> class.
        /// </summary>
        public ApplicationInfoTabView()
        {
            InitializeComponent();
        }

        private void OnTileStateChanged(object sender, RadRoutedEventArgs e)
        {
            var item = e.OriginalSource as RadTileViewItem;
            if (item != null)
            {
                var fluid = item.ChildrenOfType<RadFluidContentControl>().FirstOrDefault();
                if (fluid != null)
                {
                    switch (item.TileState)
                    {
                        case TileViewItemState.Maximized:
                            fluid.State = FluidContentControlState.Large;
                            break;
                        case TileViewItemState.Minimized:
                            fluid.State = FluidContentControlState.Normal;
                            break;
                        case TileViewItemState.Restored:
                            fluid.State = FluidContentControlState.Normal;
                            break;
                    }
                }
            }
        }

        private void OnTileMouseUp(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).GetVisualParent<RadTileViewItem>();
            if (item != null)
            {
                var fluid = item.ChildrenOfType<RadFluidContentControl>().FirstOrDefault();
                if (fluid != null)
                {
                    fluid.State = FluidContentControlState.Large;
                    item.TileState = TileViewItemState.Maximized;
                }
            }
        }
    }
}

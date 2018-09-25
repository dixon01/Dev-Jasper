// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrawableComposerBase.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DrawableComposerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation.Composers
{
    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// Composer handling <see cref="DrawableElementDataViewModelBase"/>.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of data view model this composer should handle.
    /// </typeparam>
    /// <typeparam name="TItem">
    /// The type of item this composer should generate.
    /// </typeparam>
    public partial class DrawableComposerBase<TViewModel, TItem>
    {
        partial void Initialize()
        {
            // currently we don't have groups, so no calculation is required
            this.Item.X = this.ViewModel.X.Value;
            this.Item.Y = this.ViewModel.Y.Value;
            this.Item.Width = this.ViewModel.Width.Value;
            this.Item.Height = this.ViewModel.Height.Value;

            this.Item.Visible = this.ViewModel.Visible.Value;
        }

        partial void PreHandlePropertyChange(string propertyName, ref bool handled)
        {
            switch (propertyName)
            {
                case "X":
                    this.Item.X = this.ViewModel.X.Value;
                    break;
                case "Y":
                    this.Item.Y = this.ViewModel.Y.Value;
                    break;
                case "Width":
                    this.Item.Width = this.ViewModel.Width.Value;
                    break;
                case "Height":
                    this.Item.Height = this.ViewModel.Height.Value;
                    break;
                case "Visible":
                    this.Item.Visible = this.ViewModel.Visible.Value;
                    break;
                default:
                    return;
            }

            handled = true;
        }
    }
}

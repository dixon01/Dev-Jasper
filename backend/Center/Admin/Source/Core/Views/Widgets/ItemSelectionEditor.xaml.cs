// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemSelectionEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for ItemSelectionEditor.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.Widgets
{
    using System.Windows;

    using Gorba.Center.Admin.Core.DataViewModels;

    /// <summary>
    /// Interaction logic for ItemSelectionEditor.xaml
    /// </summary>
    public partial class ItemSelectionEditor
    {
        /// <summary>
        /// The entity reference property.
        /// </summary>
        public static readonly DependencyProperty ItemSelectionProperty = DependencyProperty.Register(
            "ItemSelection",
            typeof(ItemSelectionViewModelBase),
            typeof(ItemSelectionEditor),
            new PropertyMetadata(default(ItemSelectionViewModelBase)));

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSelectionEditor"/> class.
        /// </summary>
        public ItemSelectionEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the entity reference.
        /// </summary>
        public ItemSelectionViewModelBase ItemSelection
        {
            get
            {
                return (ItemSelectionViewModelBase)this.GetValue(ItemSelectionProperty);
            }

            set
            {
                this.SetValue(ItemSelectionProperty, value);
            }
        }
    }
}

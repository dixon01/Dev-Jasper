// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridComboBoxEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PropertyGridComboBoxEditor.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System.Windows;

    /// <summary>
    /// The property grid Combo Box editor.
    /// </summary>
    public partial class PropertyGridComboBoxEditor
    {
        /// <summary>
        /// The display member path property.
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                "DisplayMemberPath",
                typeof(string),
                typeof(PropertyGridComboBoxEditor),
                new PropertyMetadata(default(string)));

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridComboBoxEditor"/> class.
        /// </summary>
        public PropertyGridComboBoxEditor()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the display member path.
        /// </summary>
        public object DisplayMemberPath
        {
            get
            {
                return this.GetValue(DisplayMemberPathProperty);
            }

            set
            {
                this.SetValue(DisplayMemberPathProperty, value);
            }
        }
    }
}
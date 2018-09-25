// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaEditorDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FormulaEditorDialog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Interaction
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for EditMenuDialog.xaml
    /// </summary>
    public partial class FormulaEditorDialog
    {
        /// <summary>
        /// specifies the max height for a dropdown popup
        /// </summary>
        public static readonly DependencyProperty MaxFormulaDropDownHeightProperty =
            DependencyProperty.Register(
                "MaxFormulaDropDownHeight",
                typeof(double),
                typeof(FormulaEditorDialog),
                new PropertyMetadata(default(double)));

        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaEditorDialog"/> class.
        /// </summary>
        public FormulaEditorDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the MaxFormulaDropDownHeight
        /// </summary>
        public double MaxFormulaDropDownHeight
        {
            get
            {
                return (double)this.GetValue(MaxFormulaDropDownHeightProperty);
            }

            set
            {
                this.SetValue(MaxFormulaDropDownHeightProperty, value);
            }
        }
    }
}

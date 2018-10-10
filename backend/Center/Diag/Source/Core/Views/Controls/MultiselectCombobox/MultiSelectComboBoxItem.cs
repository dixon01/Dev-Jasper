// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSelectComboBoxItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Item for MultiSelectComboBox
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views.Controls.MultiselectCombobox
{
    using Telerik.Windows.Controls;

    /// <summary>
    /// Item for MultiSelectComboBox
    /// </summary>
    public class MultiSelectComboBoxItem : RadComboBoxItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSelectComboBoxItem"/> class.
        ///  Initializes a new instance of MultiSelectComboBoxItem class
        /// </summary>
        /// <param name="parent">
        /// parent comboBox
        /// </param>
        public MultiSelectComboBoxItem(MultiSelectComboBox parent)
        {
            this.ParentComboBox = parent;
        }

        /// <summary>
        /// Gets the parent comboBox
        /// </summary>
        public MultiSelectComboBox ParentComboBox
        {
            get;
            private set;
        }
    }
}

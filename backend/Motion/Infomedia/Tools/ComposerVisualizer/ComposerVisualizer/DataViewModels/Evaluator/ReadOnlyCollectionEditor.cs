// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyCollectionEditor.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadOnlyCollectionEditor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;

    using Xceed.Wpf.Toolkit.PropertyGrid;
    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
    using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

    /// <summary>
    /// The read only collection editor.
    /// </summary>
    public partial class ReadOnlyCollectionEditor : UserControl, ITypeEditor
    {
        /// <summary>
        /// Gets the evaluator base data view model.
        /// </summary>
        [ExpandableObject]
        public EvaluatorBaseDataViewModel EvaluatorBaseDataViewModel { get; private set; }

        /// <summary>
        /// The resolve editor.
        /// </summary>
        /// <param name="propertyItem">
        /// The property item.
        /// </param>
        /// <returns>
        /// The <see cref="FrameworkElement"/>.
        /// </returns>
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var listBox = new ListBox();

            foreach (var item in (IEnumerable)propertyItem.Value)
            {
                var propertyGrid = new PropertyGrid();
                propertyGrid.Width = 500;
                this.EvaluatorBaseDataViewModel = item as EvaluatorBaseDataViewModel;
                propertyGrid.SelectedObject = this.EvaluatorBaseDataViewModel;
                propertyGrid.DataContext = this;
                listBox.Items.Add(propertyGrid);
            }

            return listBox;
        }
    }
}

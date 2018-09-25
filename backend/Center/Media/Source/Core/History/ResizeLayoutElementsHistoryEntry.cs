// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResizeLayoutElementsHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System.Linq;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// History entry that contains all information to undo / redo the update of layout elements.
    /// </summary>
    public class ResizeLayoutElementsHistoryEntry : HistoryEntryBase
    {
        private readonly GraphicalElementDataViewModelBase element;

        private readonly IEditorViewModel editor;

        private readonly Rect oldSize;

        private readonly Rect newSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeLayoutElementsHistoryEntry"/> class.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="editor">
        /// The editor.
        /// </param>
        /// <param name="oldSize">
        /// The old size.
        /// </param>
        /// <param name="newSize">
        /// The new size.
        /// </param>
        /// <param name="displayText">
        /// The display Text.
        /// </param>
        public ResizeLayoutElementsHistoryEntry(
            GraphicalElementDataViewModelBase element,
            IEditorViewModel editor,
            Rect oldSize,
            Rect newSize,
            string displayText)
            : base(displayText)
        {
            this.editor = editor;
            this.element = element;
            this.oldSize = oldSize;
            this.newSize = newSize;
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            var concreteElement = this.editor.Elements.SingleOrDefault(e => e.ElementName == this.element.ElementName);
            if (concreteElement == null)
            {
                return;
            }

            concreteElement.X.Value = (int)this.oldSize.X;
            concreteElement.Y.Value = (int)this.oldSize.Y;
            concreteElement.Width.Value = (int)this.oldSize.Width;
            concreteElement.Height.Value = (int)this.oldSize.Height;
            this.editor.SelectedElements.Clear();
            this.editor.SelectedElements.Add(concreteElement);
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            var concreteElement = this.editor.Elements.SingleOrDefault(e => e.ElementName == this.element.ElementName);
            if (concreteElement == null)
            {
                return;
            }

            concreteElement.MakeDirty();
            concreteElement.X.Value = (int)this.newSize.X;
            concreteElement.Y.Value = (int)this.newSize.Y;
            concreteElement.Width.Value = (int)this.newSize.Width;
            concreteElement.Height.Value = (int)this.newSize.Height;
            this.editor.SelectedElements.Clear();
            this.editor.SelectedElements.Add(concreteElement);
        }
    }
}

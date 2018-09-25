// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestoreSelectionElementHistoryBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RestoreSelectionElementHistoryBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// Base class for history entries that should restore selection after the Undo.
    /// </summary>
    public abstract class RestoreSelectionElementHistoryBase : HistoryEntryBase
    {
        private IEnumerable<LayoutElementDataViewModelBase> selection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestoreSelectionElementHistoryBase"/> class.
        /// </summary>
        /// <param name="displayText">The display text.</param>
        protected RestoreSelectionElementHistoryBase(string displayText)
            : base(displayText)
        {
        }

        /// <summary>
        /// Gets or sets the editor.
        /// </summary>
        /// <value>
        /// The editor.
        /// </value>
        public IEditorViewModel Editor { get; protected set; }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            this.Editor.SelectedElements.Clear();
            if (this.selection != null && this.selection.Any())
            {
                foreach (var selectedElement in this.selection)
                {
                    this.Editor.SelectedElements.Add(selectedElement);
                }
            }
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            this.selection = this.Editor.SelectedElements.ToArray();
        }
    }
}
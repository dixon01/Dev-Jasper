// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleListScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleListScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System.Collections.Generic;
    using Gorba.Motion.Obc.Terminal.Control.Data;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// Base class for all list screens with simple items (not nested).
    /// </summary>
    internal abstract class SimpleListScreen : ListScreen<SimpleListItem>
    {
        private int lastSelection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleListScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected SimpleListScreen(IList mainField, IContext context)
            : base(mainField, context)
        {
        }

        /// <summary>
        /// Gets the list that will be shown to the user. The user will/may select an item from this list.
        /// </summary>
        protected abstract List<string> List { get; }

        /// <summary>
        /// Gets the caption from the screen
        /// </summary>
        protected abstract string Caption { get; }

        /// <summary>
        /// Gets the list item which will be shown to the user. This list is a tree and may have subentries.
        /// </summary>
        protected override SimpleListItem RootItem
        {
            get
            {
                var root = new SimpleListItem(this.Caption, 0);
                List<string> entries = this.List;
                for (int i = 0; i < entries.Count; i++)
                {
                    var tmp = new SimpleListItem(entries[i], i);
                    tmp.Parent = root;
                    root.Children.Add(tmp);
                }

                if (this.lastSelection < root.Children.Count)
                {
                    root.LastSelection = this.lastSelection;
                }

                return root;
            }
        }

        /// <summary>
        /// This method will be called when the user has selected an entry.
        /// Which has no children -> leaf from the GetListItem(): IListItem
        /// Implement your action here.
        /// </summary>
        /// <param name = "messageId">
        /// The selected index. The index is not really representative if you have submenus!
        /// </param>
        /// <param name = "selectedItem">
        /// The selected item. This is representative
        /// </param>
        protected override void ItemSelected(int messageId, IListItem selectedItem)
        {
            this.lastSelection = messageId;
            this.ItemSelected(messageId);
        }

        /// <summary>
        ///   This method will be called when the user has selected an entry.
        ///   Implement your action here. The index is the selected item from the GetList() method
        /// </summary>
        /// <param name = "index">
        /// The selected index.
        /// </param>
        protected abstract void ItemSelected(int index);
    }
}
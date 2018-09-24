// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ListScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Control.Data;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    /// Base class for all screens that have any kind of list.
    /// </summary>
    /// <typeparam name="TListItem">
    /// The type of item in this list.
    /// </typeparam>
    internal abstract class ListScreen<TListItem> : Screen<IList>
        where TListItem : class, IListItem
    {
        private readonly Logger logger;

        private TListItem root;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListScreen{TListItem}"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected ListScreen(IList mainField, IContext context)
            : base(mainField, context)
        {
            this.logger = LogHelper.GetLogger(this.GetType());
        }

        /// <summary>
        /// Gets the list item which will be shown to the user. This list is a tree and may have subentries.
        /// </summary>
        protected abstract TListItem RootItem { get; }

        /// <summary>
        /// Shows this screen.
        /// </summary>
        public override void Show()
        {
            this.root = this.RootItem;
            this.Init();
            this.MainField.EscapePressed += this.MainFieldEscapePressed;
            this.MainField.SelectedIndexChanged += this.ListSelectedItemIndexEvent;
            base.Show();
        }

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public override void Hide()
        {
            this.MainField.EscapePressed -= this.MainFieldEscapePressed;
            this.MainField.SelectedIndexChanged -= this.ListSelectedItemIndexEvent;
            base.Hide();
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
        protected abstract void ItemSelected(int messageId, IListItem selectedItem);

        /// <summary>
        /// Handles the escape key.
        /// </summary>
        protected virtual void HandleEscapeKey()
        {
            if (this.root.Parent != null)
            {
                this.root = (TListItem)this.root.Parent;
                this.Init();
            }
            else
            {
                this.Context.ShowPreviousScreen();
            }
        }

        private void Init()
        {
            var childLabels = new List<string>();
            foreach (IListItem n in this.root.GetChildren())
            {
                childLabels.Add(n.Label);
            }

            this.MainField.Init(this.root.Caption, childLabels, true, this.root.LastSelection);
            if (this.root.GetChildren().Count == 0)
            {
                this.MainField.ShowMessageBox(
                                              new MessageBoxInfo(
                                                  ml.ml_string(7, "Warning"),
                                                  ml.ml_string(50, "The list doesn't contain any entries..."),
                                                  MessageBoxInfo.MsgType.Warning)); // MLHIDE
            }
        }

        private void ListSelectedItemIndexEvent(object sender, IndexEventArgs e)
        {
            try
            {
                if (this.root.IsLeaf)
                {
                    this.ItemSelected(e.Index, this.root);
                }
                else
                {
                    var tmpNode = (TListItem)this.root.GetChild(e.Index);
                    if (tmpNode.IsLeaf)
                    {
                        this.ItemSelected(e.Index, tmpNode);
                    }
                    else
                    {
                        this.root = tmpNode;
                        this.Init();
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.ErrorException("Exception while handling index change", ex);
            }
        }

        private void MainFieldEscapePressed(object sender, EventArgs e)
        {
            this.HandleEscapeKey();
        }
    }
}
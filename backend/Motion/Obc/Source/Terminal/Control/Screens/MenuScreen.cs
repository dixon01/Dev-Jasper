// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MenuScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Control.Data;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    /// The menu screen.
    /// </summary>
    internal class MenuScreen : ListScreen<MultiLangListItem>
    {
        private static readonly Logger Logger = LogHelper.GetLogger<MenuScreen>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public MenuScreen(IList mainField, IContext context)
            : base(mainField, context)
        {
        }

        /// <summary>
        /// Gets the list item which will be shown to the user. This list is a tree and may have subentries.
        /// </summary>
        protected override MultiLangListItem RootItem
        {
            get
            {
                return this.Context.ConfigHandler.GetMenuConfig();
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
            Logger.Debug("Menu: {0} : {1}", messageId, selectedItem.Caption); // MLHIDE
            this.Context.ActionHandler.HandleAction((CommandName)selectedItem.Tag);
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridContextMenuFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PropertyGridContextMenuFactory.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.Resources;

    /// <summary>
    /// The PropertyGridContextMenuFactory.
    /// </summary>
    public static class PropertyGridContextMenuFactory
    {
        /// <summary>
        /// Helper to create a default context menu for property grids
        /// </summary>
        /// <param name="propertyGrid">the property grid</param>
        /// <param name="openFormulaEditorCallback">the callback to be called to open a formula editor</param>
        /// <param name="removeFormulaCommand">the remove formula command</param>
        public static void SetupDefaultContextMenu(
            PropertyGrid propertyGrid,
            ICommand openFormulaEditorCallback,
            ICommand removeFormulaCommand)
        {
            propertyGrid.ItemContextMenu = new ContextMenu();

            AddFormulaCommands(propertyGrid, openFormulaEditorCallback, removeFormulaCommand);
        }

        /// <summary>
        /// Helper to create a default context menu for property grids
        /// </summary>
        /// <param name="propertyGrid">the property grid</param>
        /// <param name="openFormulaEditorCallback">the callback to be called to open a formula editor</param>
        /// <param name="openAnimationEditorCallback">the callback to be called to open a animation editor</param>
        /// <param name="removeFormulaCommand">the remove formula command</param>
        /// <param name="removeAnimationCommand">the remove animation command</param>
        public static void SetupDefaultContextMenu(
            PropertyGrid propertyGrid,
            ICommand openFormulaEditorCallback,
            ICommand openAnimationEditorCallback,
            ICommand removeFormulaCommand,
            ICommand removeAnimationCommand)
        {
            propertyGrid.ItemContextMenu = new ContextMenu();

            AddFormulaCommands(propertyGrid, openFormulaEditorCallback, removeFormulaCommand);

            AddAnimationCommands(propertyGrid, openAnimationEditorCallback, removeAnimationCommand);
        }

        private static void AddAnimationCommands(
            PropertyGrid propertyGrid,
            ICommand openAnimationEditorCallback,
            ICommand removeAnimationCommand)
        {
            var animationIconUri =
                new Uri(
                    "pack://application:,,,/Gorba.Center.Media.Core;"
                    + "component/Resources/Images/Icons/delete_dark_16x16.png");
            var deleteAnimationIcon = new Image { Source = new BitmapImage(animationIconUri) };
            var animationMenu = new MenuItem
                                {
                                    Header = MediaStrings.PropertyGrid_ContextMenu_Animation,
                                    Command = openAnimationEditorCallback,
                                };
            var removeAnimationMenu = new MenuItem
                                      {
                                          Header = MediaStrings.PropertyGrid_ContextMenu_RemoveAnimation,
                                          Command = removeAnimationCommand,
                                          CommandParameter = propertyGrid.ItemContextMenu,
                                          Icon = deleteAnimationIcon
                                      };

            var flatContextMenuItemStyle = Application.Current.FindResource("FlatContextMenuItem") as Style;
            if (flatContextMenuItemStyle != null)
            {
                animationMenu.Style = flatContextMenuItemStyle;
                removeAnimationMenu.Style = flatContextMenuItemStyle;
            }

            propertyGrid.ItemContextMenu.Items.Add(animationMenu);
            propertyGrid.ItemContextMenu.Items.Add(removeAnimationMenu);
        }

        private static void AddFormulaCommands(
            PropertyGrid propertyGrid,
            ICommand openFormulaEditorCallback,
            ICommand removeFormulaCommand)
        {
            var formulaMenu = new MenuItem
                              {
                                  Header = MediaStrings.PropertyGrid_ContextMenu_Formula,
                                  Command = openFormulaEditorCallback,
                              };

            var iconUri =
                new Uri(
                    "pack://application:,,,/Gorba.Center.Media.Core;"
                    + "component/Resources/Images/Icons/delete_dark_16x16.png");
            var deleteIcon = new Image { Source = new BitmapImage(iconUri) };
            var removeFormulaMenu = new MenuItem
                                    {
                                        Header = MediaStrings.PropertyGrid_ContextMenu_RemoveFormula,
                                        Command = removeFormulaCommand,
                                        CommandParameter = propertyGrid.ItemContextMenu,
                                        Icon = deleteIcon
                                    };

            var flatContextMenuItemStyle = Application.Current.FindResource("FlatContextMenuItem") as Style;
            if (flatContextMenuItemStyle != null)
            {
                formulaMenu.Style = flatContextMenuItemStyle;
                removeFormulaMenu.Style = flatContextMenuItemStyle;
            }

            propertyGrid.ItemContextMenu.Items.Add(formulaMenu);
            propertyGrid.ItemContextMenu.Items.Add(removeFormulaMenu);
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeNodeFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TreeNodeFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core.Config;

    /// <summary>
    /// Creates tree nodes for the config tree.
    /// </summary>
    internal static class TreeNodeFactory
    {
        /// <summary>
        /// Create a node for the given type.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="itemType">
        /// The item type.
        /// </param>
        /// <returns>
        /// a new tree node.
        /// </returns>
        public static TreeNode CreateNode(string name, Type itemType)
        {
            if (itemType.IsGenericType && typeof(List<>).IsAssignableFrom(itemType.GetGenericTypeDefinition()))
            {
                var node = new TreeNode(name);
                var editor = new ConfigListEditor();
                editor.ItemType = itemType.GetGenericArguments()[0];
                editor.Node = node;
                node.Tag = editor;
                return node;
            }

            if (itemType.BaseType != null && itemType.BaseType.IsGenericType &&
                typeof(PeerStackConfig<>).IsAssignableFrom(itemType.BaseType.GetGenericTypeDefinition()))
            {
                var node = new TreeNode(name);
                var editor = new PeerStackConfigEditor();
                editor.Config = (PeerConfig)Activator.CreateInstance(itemType);
                editor.Node = node;
                node.Tag = editor;
                return node;
            }
            else
            {
                var node = new TreeNode(name);
                var editor = new PropertyGrid();
                editor.SelectedObject = Activator.CreateInstance(itemType);
                node.Tag = editor;
                return node;
            }
        }
    }
}

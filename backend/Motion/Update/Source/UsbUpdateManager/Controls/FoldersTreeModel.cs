// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FoldersTreeModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FoldersTreeModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// A tree model for folders used by <see cref="FoldersTreeControl"/>.
    /// </summary>
    public class FoldersTreeModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FoldersTreeModel"/> class
        /// with a default root node.
        /// </summary>
        public FoldersTreeModel()
            : this(new Folder(string.Empty, null))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FoldersTreeModel"/> class.
        /// </summary>
        /// <param name="root">
        /// The root node of this model.
        /// </param>
        public FoldersTreeModel(Folder root)
        {
            this.Root = root;
        }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        public Folder Root { get; private set; }

        /// <summary>
        /// A folder inside the tree model.
        /// </summary>
        public class Folder
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Folder"/> class.
            /// </summary>
            /// <param name="name">
            /// The name.
            /// </param>
            /// <param name="tag">
            /// Object that contains data about the control.
            /// </param>
            public Folder(string name, object tag)
            {
                this.Name = name;
                this.Tag = tag;
                this.Children = new BindingList<Folder>();
                this.Children.ListChanged += (s, e) => this.RaiseChildrenChanged(e);
            }

            /// <summary>
            /// Event that is fired whenever the <see cref="Children"/> list changes.
            /// </summary>
            public event ListChangedEventHandler ChildrenChanged;

            /// <summary>
            /// Gets the name of the folder.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Gets the path from the root to the folder.
            /// </summary>
            public string Path
            {
                get
                {
                    if (this.Parent == null)
                    {
                        return this.Name;
                    }

                    return this.Parent.Path + System.IO.Path.DirectorySeparatorChar + this.Name;
                }
            }

            /// <summary>
            /// Gets the parent.
            /// </summary>
            public Folder Parent { get; private set; }

            /// <summary>
            /// Gets the object that contains data about the control.
            /// </summary>
            public object Tag { get; private set; }

            /// <summary>
            /// Gets the child folders.
            /// </summary>
            public BindingList<Folder> Children { get; private set; }

            /// <summary>
            /// Gets the tree node representing this folder.
            /// </summary>
            public TreeNode TreeNode { get; internal set; }

            /// <summary>
            /// Finds the first folder in the hierarchy of this folder matching a given predicate.
            /// </summary>
            /// <param name="predicate">
            /// The predicate to match.
            /// </param>
            /// <returns>
            /// The <see cref="Folder"/> matching the predicate or null if none matches.
            /// </returns>
            public Folder Find(Predicate<Folder> predicate)
            {
                if (predicate(this))
                {
                    return this;
                }

                foreach (var child in this.Children)
                {
                    var found = child.Find(predicate);
                    if (found != null)
                    {
                        return found;
                    }
                }

                return null;
            }

            /// <summary>
            /// Raises the <see cref="ChildrenChanged"/> event.
            /// </summary>
            /// <param name="e">
            /// The event arguments.
            /// </param>
            protected virtual void RaiseChildrenChanged(ListChangedEventArgs e)
            {
                if (e.ListChangedType == ListChangedType.ItemAdded)
                {
                    this.Children[e.NewIndex].Parent = this;
                }

                var handler = this.ChildrenChanged;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }
    }
}
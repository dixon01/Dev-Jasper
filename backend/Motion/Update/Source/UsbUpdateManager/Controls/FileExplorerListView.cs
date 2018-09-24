// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileExplorerListView.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileExplorerListView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// A <see cref="ListView"/> that displays files and folders.
    /// </summary>
    public partial class FileExplorerListView : ListView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileExplorerListView"/> class.
        /// </summary>
        public FileExplorerListView()
        {
            this.InitializeComponent();

            this.IconManager = new FileIconManager(this.smallFileImages, this.largeFileImages);

            this.ListViewItemSorter = new ItemSorter();
        }

        /// <summary>
        /// Gets the icon manager responsible for this view's icons.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FileIconManager IconManager { get; private set; }

        private class ItemSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                var a = (ListViewItem)x;
                var b = (ListViewItem)y;

                var compareSubItems = a.SubItems.Count.CompareTo(b.SubItems.Count);
                if (compareSubItems != 0)
                {
                    return compareSubItems;
                }

                return string.Compare(a.Text, b.Text, StringComparison.CurrentCultureIgnoreCase);
            }
        }
    }
}

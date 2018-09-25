namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    partial class FileExplorerListView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.largeFileImages = new System.Windows.Forms.ImageList(this.components);
            this.smallFileImages = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 180;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 70;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Resource ID";
            this.columnHeader3.Width = 240;
            // 
            // largeFileImages
            // 
            this.largeFileImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.largeFileImages.ImageSize = new System.Drawing.Size(32, 32);
            this.largeFileImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // smallFileImages
            // 
            this.smallFileImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.smallFileImages.ImageSize = new System.Drawing.Size(16, 16);
            this.smallFileImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // FileExplorerListView
            // 
            this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HideSelection = false;
            this.LargeImageList = this.largeFileImages;
            this.Name = "fileListView";
            this.ShowGroups = false;
            this.Size = new System.Drawing.Size(370, 353);
            this.SmallImageList = this.smallFileImages;
            this.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.UseCompatibleStateImageBehavior = false;
            this.View = System.Windows.Forms.View.Details;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ImageList largeFileImages;
        private System.Windows.Forms.ImageList smallFileImages;
    }
}

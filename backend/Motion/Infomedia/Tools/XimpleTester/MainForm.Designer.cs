namespace XimpleTester
{
    using System.Windows.Forms;

    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItemExit = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItemAddCreator = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItemCloseWindow = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.menuItemCascade = new System.Windows.Forms.MenuItem();
            this.menuItemTileHorizontally = new System.Windows.Forms.MenuItem();
            this.menuItemTileVertically = new System.Windows.Forms.MenuItem();
            this.menuItemArrangeIcons = new System.Windows.Forms.MenuItem();
            this.menuItemDistribute = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem3,
            this.menuItem5});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemExit});
            this.menuItem1.Text = "&File";
            // 
            // menuItemExit
            // 
            this.menuItemExit.Index = 0;
            this.menuItemExit.Text = "E&xit";
            this.menuItemExit.Click += new System.EventHandler(this.MenuItemExitClick);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemAddCreator});
            this.menuItem3.Text = "&View";
            // 
            // menuItemAddCreator
            // 
            this.menuItemAddCreator.Index = 0;
            this.menuItemAddCreator.Text = "Add &New Ximple Creator";
            this.menuItemAddCreator.Click += new System.EventHandler(this.MenuItemAddCreatorClick);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 2;
            this.menuItem5.MdiList = true;
            this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemCloseWindow,
            this.menuItem9,
            this.menuItemCascade,
            this.menuItemTileHorizontally,
            this.menuItemTileVertically,
            this.menuItemArrangeIcons,
            this.menuItemDistribute});
            this.menuItem5.Text = "Window";
            // 
            // menuItemCloseWindow
            // 
            this.menuItemCloseWindow.Enabled = false;
            this.menuItemCloseWindow.Index = 0;
            this.menuItemCloseWindow.Text = "&Close";
            this.menuItemCloseWindow.Click += new System.EventHandler(this.MenuItemCloseWindowClick);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 1;
            this.menuItem9.Text = "-";
            // 
            // menuItemCascade
            // 
            this.menuItemCascade.Index = 2;
            this.menuItemCascade.Text = "Ca&scade";
            this.menuItemCascade.Click += new System.EventHandler(this.MenuItemCascadeClick);
            // 
            // menuItemTileHorizontally
            // 
            this.menuItemTileHorizontally.Index = 3;
            this.menuItemTileHorizontally.Text = "Tile &Horizontally";
            this.menuItemTileHorizontally.Click += new System.EventHandler(this.MenuItemTileHorizontallyClick);
            // 
            // menuItemTileVertically
            // 
            this.menuItemTileVertically.Index = 4;
            this.menuItemTileVertically.Text = "Tile &Vertically";
            this.menuItemTileVertically.Click += new System.EventHandler(this.MenuItemTileVerticallyClick);
            // 
            // menuItemArrangeIcons
            // 
            this.menuItemArrangeIcons.Index = 5;
            this.menuItemArrangeIcons.Text = "&Arrange Icons";
            this.menuItemArrangeIcons.Click += new System.EventHandler(this.MenuItemArrangeIconsClick);
            // 
            // menuItemDistribute
            // 
            this.menuItemDistribute.Index = 6;
            this.menuItemDistribute.Text = "&Distribute";
            this.menuItemDistribute.Click += new System.EventHandler(this.MenuItemDistributeClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 335);
            this.IsMdiContainer = true;
            this.Menu = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "Ximple Tester";
            this.MdiChildActivate += new System.EventHandler(this.MainFormMdiChildActivate);
            this.ResumeLayout(false);

        }

        #endregion

        private MenuItem menuItem1;
        private MenuItem menuItemExit;
        private MenuItem menuItem3;
        private MenuItem menuItemAddCreator;
        private MenuItem menuItem5;
        private MenuItem menuItemCloseWindow;
        private MenuItem menuItem9;
        private MenuItem menuItemCascade;
        private MenuItem menuItemTileHorizontally;
        private MenuItem menuItemTileVertically;
        private MenuItem menuItemArrangeIcons;
        private MenuItem menuItemDistribute;

    }
}


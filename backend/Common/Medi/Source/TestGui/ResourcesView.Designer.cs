namespace Gorba.Common.Medi.TestGui
{
    partial class ResourcesView
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxDeleteLocal = new System.Windows.Forms.CheckBox();
            this.buttonRegister = new System.Windows.Forms.Button();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textBoxRegisterFile = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBoxResources = new System.Windows.Forms.ListBox();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.textBoxSelectedResource = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBoxSentTo = new System.Windows.Forms.TextBox();
            this.buttonBrowseSendTo = new System.Windows.Forms.Button();
            this.buttonSend = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxCheckout = new System.Windows.Forms.TextBox();
            this.buttonBrowseCheckout = new System.Windows.Forms.Button();
            this.buttonCheckout = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBoxExport = new System.Windows.Forms.TextBox();
            this.buttonBrowseExport = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.resourceUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.textBoxCheckin = new System.Windows.Forms.TextBox();
            this.buttonBrowseCheckin = new System.Windows.Forms.Button();
            this.buttonCheckin = new System.Windows.Forms.Button();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.textBoxSendFileDestination = new System.Windows.Forms.TextBox();
            this.buttonBrowseSendFileDestination = new System.Windows.Forms.Button();
            this.buttonSendFile = new System.Windows.Forms.Button();
            this.buttonBrowseSendFile = new System.Windows.Forms.Button();
            this.textBoxSendFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkBoxDeleteLocal);
            this.groupBox1.Controls.Add(this.buttonRegister);
            this.groupBox1.Controls.Add(this.buttonBrowse);
            this.groupBox1.Controls.Add(this.textBoxRegisterFile);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(519, 77);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Registration";
            // 
            // checkBoxDeleteLocal
            // 
            this.checkBoxDeleteLocal.AutoSize = true;
            this.checkBoxDeleteLocal.Location = new System.Drawing.Point(6, 47);
            this.checkBoxDeleteLocal.Name = "checkBoxDeleteLocal";
            this.checkBoxDeleteLocal.Size = new System.Drawing.Size(89, 17);
            this.checkBoxDeleteLocal.TabIndex = 3;
            this.checkBoxDeleteLocal.Text = "Delete locally";
            this.checkBoxDeleteLocal.UseVisualStyleBackColor = true;
            // 
            // buttonRegister
            // 
            this.buttonRegister.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRegister.Enabled = false;
            this.buttonRegister.Location = new System.Drawing.Point(438, 48);
            this.buttonRegister.Name = "buttonRegister";
            this.buttonRegister.Size = new System.Drawing.Size(75, 23);
            this.buttonRegister.TabIndex = 2;
            this.buttonRegister.Text = "Register";
            this.buttonRegister.UseVisualStyleBackColor = true;
            this.buttonRegister.Click += new System.EventHandler(this.ButtonRegisterClick);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Location = new System.Drawing.Point(438, 19);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowse.TabIndex = 1;
            this.buttonBrowse.Text = "Browse...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.ButtonBrowseClick);
            // 
            // textBoxRegisterFile
            // 
            this.textBoxRegisterFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegisterFile.Location = new System.Drawing.Point(6, 21);
            this.textBoxRegisterFile.Name = "textBoxRegisterFile";
            this.textBoxRegisterFile.ReadOnly = true;
            this.textBoxRegisterFile.Size = new System.Drawing.Size(426, 20);
            this.textBoxRegisterFile.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.splitContainer1);
            this.groupBox2.Controls.Add(this.textBoxSelectedResource);
            this.groupBox2.Location = new System.Drawing.Point(3, 86);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(519, 144);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Known resources";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(6, 19);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBoxResources);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid);
            this.splitContainer1.Size = new System.Drawing.Size(507, 93);
            this.splitContainer1.SplitterDistance = 253;
            this.splitContainer1.TabIndex = 2;
            // 
            // listBoxResources
            // 
            this.listBoxResources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxResources.FormattingEnabled = true;
            this.listBoxResources.IntegralHeight = false;
            this.listBoxResources.Location = new System.Drawing.Point(0, 0);
            this.listBoxResources.Name = "listBoxResources";
            this.listBoxResources.Size = new System.Drawing.Size(253, 93);
            this.listBoxResources.TabIndex = 0;
            this.listBoxResources.SelectedIndexChanged += new System.EventHandler(this.ListBoxResourcesSelectedIndexChanged);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(250, 93);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.ToolbarVisible = false;
            // 
            // textBoxSelectedResource
            // 
            this.textBoxSelectedResource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSelectedResource.Location = new System.Drawing.Point(6, 118);
            this.textBoxSelectedResource.Name = "textBoxSelectedResource";
            this.textBoxSelectedResource.ReadOnly = true;
            this.textBoxSelectedResource.Size = new System.Drawing.Size(507, 20);
            this.textBoxSelectedResource.TabIndex = 0;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Enabled = false;
            this.buttonRemove.Location = new System.Drawing.Point(6, 19);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonRemove.TabIndex = 1;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.ButtonRemoveClick);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.buttonRemove);
            this.groupBox3.Location = new System.Drawing.Point(3, 236);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(519, 48);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Remove";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.textBoxSentTo);
            this.groupBox4.Controls.Add(this.buttonBrowseSendTo);
            this.groupBox4.Controls.Add(this.buttonSend);
            this.groupBox4.Location = new System.Drawing.Point(3, 291);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(519, 48);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Send To";
            // 
            // textBoxSentTo
            // 
            this.textBoxSentTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSentTo.Location = new System.Drawing.Point(6, 21);
            this.textBoxSentTo.Name = "textBoxSentTo";
            this.textBoxSentTo.ReadOnly = true;
            this.textBoxSentTo.Size = new System.Drawing.Size(345, 20);
            this.textBoxSentTo.TabIndex = 2;
            // 
            // buttonBrowseSendTo
            // 
            this.buttonBrowseSendTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseSendTo.Enabled = false;
            this.buttonBrowseSendTo.Location = new System.Drawing.Point(357, 19);
            this.buttonBrowseSendTo.Name = "buttonBrowseSendTo";
            this.buttonBrowseSendTo.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseSendTo.TabIndex = 1;
            this.buttonBrowseSendTo.Text = "Browse...";
            this.buttonBrowseSendTo.UseVisualStyleBackColor = true;
            this.buttonBrowseSendTo.Click += new System.EventHandler(this.ButtonBrowseSendToClick);
            // 
            // buttonSend
            // 
            this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSend.Enabled = false;
            this.buttonSend.Location = new System.Drawing.Point(438, 19);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 23);
            this.buttonSend.TabIndex = 1;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.ButtonSendClick);
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.textBoxCheckout);
            this.groupBox5.Controls.Add(this.buttonBrowseCheckout);
            this.groupBox5.Controls.Add(this.buttonCheckout);
            this.groupBox5.Location = new System.Drawing.Point(3, 345);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(519, 48);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Checkout";
            // 
            // textBoxCheckout
            // 
            this.textBoxCheckout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCheckout.Location = new System.Drawing.Point(6, 21);
            this.textBoxCheckout.Name = "textBoxCheckout";
            this.textBoxCheckout.ReadOnly = true;
            this.textBoxCheckout.Size = new System.Drawing.Size(345, 20);
            this.textBoxCheckout.TabIndex = 2;
            // 
            // buttonBrowseCheckout
            // 
            this.buttonBrowseCheckout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseCheckout.Enabled = false;
            this.buttonBrowseCheckout.Location = new System.Drawing.Point(357, 19);
            this.buttonBrowseCheckout.Name = "buttonBrowseCheckout";
            this.buttonBrowseCheckout.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseCheckout.TabIndex = 1;
            this.buttonBrowseCheckout.Text = "Browse...";
            this.buttonBrowseCheckout.UseVisualStyleBackColor = true;
            this.buttonBrowseCheckout.Click += new System.EventHandler(this.ButtonBrowseCheckoutClick);
            // 
            // buttonCheckout
            // 
            this.buttonCheckout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCheckout.Enabled = false;
            this.buttonCheckout.Location = new System.Drawing.Point(438, 19);
            this.buttonCheckout.Name = "buttonCheckout";
            this.buttonCheckout.Size = new System.Drawing.Size(75, 23);
            this.buttonCheckout.TabIndex = 1;
            this.buttonCheckout.Text = "Checkout";
            this.buttonCheckout.UseVisualStyleBackColor = true;
            this.buttonCheckout.Click += new System.EventHandler(this.ButtonCheckoutClick);
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.textBoxExport);
            this.groupBox6.Controls.Add(this.buttonBrowseExport);
            this.groupBox6.Controls.Add(this.buttonExport);
            this.groupBox6.Location = new System.Drawing.Point(3, 453);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(519, 48);
            this.groupBox6.TabIndex = 3;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Export";
            // 
            // textBoxExport
            // 
            this.textBoxExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExport.Location = new System.Drawing.Point(6, 21);
            this.textBoxExport.Name = "textBoxExport";
            this.textBoxExport.ReadOnly = true;
            this.textBoxExport.Size = new System.Drawing.Size(345, 20);
            this.textBoxExport.TabIndex = 2;
            // 
            // buttonBrowseExport
            // 
            this.buttonBrowseExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseExport.Enabled = false;
            this.buttonBrowseExport.Location = new System.Drawing.Point(357, 19);
            this.buttonBrowseExport.Name = "buttonBrowseExport";
            this.buttonBrowseExport.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseExport.TabIndex = 1;
            this.buttonBrowseExport.Text = "Browse...";
            this.buttonBrowseExport.UseVisualStyleBackColor = true;
            this.buttonBrowseExport.Click += new System.EventHandler(this.ButtonBrowseExportClick);
            // 
            // buttonExport
            // 
            this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExport.Enabled = false;
            this.buttonExport.Location = new System.Drawing.Point(438, 19);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(75, 23);
            this.buttonExport.TabIndex = 1;
            this.buttonExport.Text = "Export";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.ButtonExportClick);
            // 
            // resourceUpdateTimer
            // 
            this.resourceUpdateTimer.Enabled = true;
            this.resourceUpdateTimer.Interval = 1000;
            this.resourceUpdateTimer.Tick += new System.EventHandler(this.ResourceUpdateTimerOnTick);
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox7.Controls.Add(this.textBoxCheckin);
            this.groupBox7.Controls.Add(this.buttonBrowseCheckin);
            this.groupBox7.Controls.Add(this.buttonCheckin);
            this.groupBox7.Location = new System.Drawing.Point(3, 399);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(519, 48);
            this.groupBox7.TabIndex = 3;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Checkin";
            // 
            // textBoxCheckin
            // 
            this.textBoxCheckin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCheckin.Location = new System.Drawing.Point(6, 21);
            this.textBoxCheckin.Name = "textBoxCheckin";
            this.textBoxCheckin.ReadOnly = true;
            this.textBoxCheckin.Size = new System.Drawing.Size(345, 20);
            this.textBoxCheckin.TabIndex = 2;
            // 
            // buttonBrowseCheckin
            // 
            this.buttonBrowseCheckin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseCheckin.Enabled = false;
            this.buttonBrowseCheckin.Location = new System.Drawing.Point(357, 19);
            this.buttonBrowseCheckin.Name = "buttonBrowseCheckin";
            this.buttonBrowseCheckin.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseCheckin.TabIndex = 1;
            this.buttonBrowseCheckin.Text = "Browse...";
            this.buttonBrowseCheckin.UseVisualStyleBackColor = true;
            this.buttonBrowseCheckin.Click += new System.EventHandler(this.ButtonBrowseCheckinClick);
            // 
            // buttonCheckin
            // 
            this.buttonCheckin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCheckin.Enabled = false;
            this.buttonCheckin.Location = new System.Drawing.Point(438, 19);
            this.buttonCheckin.Name = "buttonCheckin";
            this.buttonCheckin.Size = new System.Drawing.Size(75, 23);
            this.buttonCheckin.TabIndex = 1;
            this.buttonCheckin.Text = "Checkin";
            this.buttonCheckin.UseVisualStyleBackColor = true;
            this.buttonCheckin.Click += new System.EventHandler(this.ButtonCheckinClick);
            // 
            // groupBox9
            // 
            this.groupBox9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox9.Controls.Add(this.textBoxSendFileDestination);
            this.groupBox9.Controls.Add(this.buttonBrowseSendFileDestination);
            this.groupBox9.Controls.Add(this.buttonSendFile);
            this.groupBox9.Controls.Add(this.buttonBrowseSendFile);
            this.groupBox9.Controls.Add(this.textBoxSendFile);
            this.groupBox9.Controls.Add(this.label2);
            this.groupBox9.Controls.Add(this.label1);
            this.groupBox9.Location = new System.Drawing.Point(3, 507);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(519, 74);
            this.groupBox9.TabIndex = 5;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Send File";
            // 
            // textBoxSendFileDestination
            // 
            this.textBoxSendFileDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSendFileDestination.Location = new System.Drawing.Point(75, 47);
            this.textBoxSendFileDestination.Name = "textBoxSendFileDestination";
            this.textBoxSendFileDestination.ReadOnly = true;
            this.textBoxSendFileDestination.Size = new System.Drawing.Size(276, 20);
            this.textBoxSendFileDestination.TabIndex = 2;
            // 
            // buttonBrowseSendFileDestination
            // 
            this.buttonBrowseSendFileDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseSendFileDestination.Enabled = false;
            this.buttonBrowseSendFileDestination.Location = new System.Drawing.Point(357, 45);
            this.buttonBrowseSendFileDestination.Name = "buttonBrowseSendFileDestination";
            this.buttonBrowseSendFileDestination.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseSendFileDestination.TabIndex = 1;
            this.buttonBrowseSendFileDestination.Text = "Browse...";
            this.buttonBrowseSendFileDestination.UseVisualStyleBackColor = true;
            this.buttonBrowseSendFileDestination.Click += new System.EventHandler(this.ButtonBrowseSendFileDestinationClick);
            // 
            // buttonSendFile
            // 
            this.buttonSendFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSendFile.Enabled = false;
            this.buttonSendFile.Location = new System.Drawing.Point(438, 45);
            this.buttonSendFile.Name = "buttonSendFile";
            this.buttonSendFile.Size = new System.Drawing.Size(75, 23);
            this.buttonSendFile.TabIndex = 1;
            this.buttonSendFile.Text = "Send";
            this.buttonSendFile.UseVisualStyleBackColor = true;
            this.buttonSendFile.Click += new System.EventHandler(this.ButtonSendFileClick);
            // 
            // buttonBrowseSendFile
            // 
            this.buttonBrowseSendFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseSendFile.Location = new System.Drawing.Point(357, 19);
            this.buttonBrowseSendFile.Name = "buttonBrowseSendFile";
            this.buttonBrowseSendFile.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseSendFile.TabIndex = 3;
            this.buttonBrowseSendFile.Text = "Browse...";
            this.buttonBrowseSendFile.UseVisualStyleBackColor = true;
            this.buttonBrowseSendFile.Click += new System.EventHandler(this.ButtonBrowseSendFileClick);
            // 
            // textBoxSendFile
            // 
            this.textBoxSendFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSendFile.Location = new System.Drawing.Point(75, 21);
            this.textBoxSendFile.Name = "textBoxSendFile";
            this.textBoxSendFile.ReadOnly = true;
            this.textBoxSendFile.Size = new System.Drawing.Size(276, 20);
            this.textBoxSendFile.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Destination:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Local File:";
            // 
            // ResourcesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "ResourcesView";
            this.Size = new System.Drawing.Size(525, 584);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonRegister;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.TextBox textBoxRegisterFile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBoxResources;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.CheckBox checkBoxDeleteLocal;
        private System.Windows.Forms.TextBox textBoxSelectedResource;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxSentTo;
        private System.Windows.Forms.Button buttonBrowseSendTo;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBoxCheckout;
        private System.Windows.Forms.Button buttonBrowseCheckout;
        private System.Windows.Forms.Button buttonCheckout;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox textBoxExport;
        private System.Windows.Forms.Button buttonBrowseExport;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Timer resourceUpdateTimer;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox textBoxCheckin;
        private System.Windows.Forms.Button buttonBrowseCheckin;
        private System.Windows.Forms.Button buttonCheckin;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.TextBox textBoxSendFileDestination;
        private System.Windows.Forms.Button buttonBrowseSendFileDestination;
        private System.Windows.Forms.Button buttonSendFile;
        private System.Windows.Forms.Button buttonBrowseSendFile;
        private System.Windows.Forms.TextBox textBoxSendFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}

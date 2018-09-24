namespace XimpleTester
{
    partial class XimpleCreatorForm
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
            this.listBoxCells = new System.Windows.Forms.ListBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxValue = new System.Windows.Forms.TextBox();
            this.numericUpDownRow = new System.Windows.Forms.NumericUpDown();
            this.comboBoxColumn = new System.Windows.Forms.ComboBox();
            this.comboBoxTable = new System.Windows.Forms.ComboBox();
            this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRow)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxCells
            // 
            this.listBoxCells.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxCells.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxCells.FormattingEnabled = true;
            this.listBoxCells.IntegralHeight = false;
            this.listBoxCells.Location = new System.Drawing.Point(3, 32);
            this.listBoxCells.Name = "listBoxCells";
            this.listBoxCells.Size = new System.Drawing.Size(117, 101);
            this.listBoxCells.TabIndex = 0;
            this.listBoxCells.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListBoxCellsDrawItem);
            this.listBoxCells.SelectedIndexChanged += new System.EventHandler(this.ListBoxCellsSelectedIndexChanged);
            // 
            // buttonSend
            // 
            this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSend.Enabled = false;
            this.buttonSend.Location = new System.Drawing.Point(3, 136);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(117, 23);
            this.buttonSend.TabIndex = 1;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.ButtonSendClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.buttonRemove);
            this.splitContainer1.Panel1.Controls.Add(this.buttonAdd);
            this.splitContainer1.Panel1.Controls.Add(this.listBoxCells);
            this.splitContainer1.Panel1.Controls.Add(this.buttonSend);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.numericUpDownRow);
            this.splitContainer1.Panel2.Controls.Add(this.comboBoxColumn);
            this.splitContainer1.Panel2.Controls.Add(this.comboBoxTable);
            this.splitContainer1.Panel2.Controls.Add(this.comboBoxLanguage);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(334, 162);
            this.splitContainer1.SplitterDistance = 123;
            this.splitContainer1.TabIndex = 2;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Enabled = false;
            this.buttonRemove.Location = new System.Drawing.Point(32, 3);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(23, 23);
            this.buttonRemove.TabIndex = 2;
            this.buttonRemove.Text = "-";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.ButtonRemoveClick);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(3, 3);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(23, 23);
            this.buttonAdd.TabIndex = 2;
            this.buttonAdd.Text = "+";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.ButtonAddClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.textBoxValue);
            this.groupBox1.Location = new System.Drawing.Point(3, 110);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(201, 49);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Value";
            // 
            // textBoxValue
            // 
            this.textBoxValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxValue.Enabled = false;
            this.textBoxValue.Location = new System.Drawing.Point(3, 16);
            this.textBoxValue.Multiline = true;
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Size = new System.Drawing.Size(195, 30);
            this.textBoxValue.TabIndex = 0;
            this.textBoxValue.TextChanged += new System.EventHandler(this.TextBoxValueTextChanged);
            // 
            // numericUpDownRow
            // 
            this.numericUpDownRow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownRow.Enabled = false;
            this.numericUpDownRow.Location = new System.Drawing.Point(64, 84);
            this.numericUpDownRow.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownRow.Name = "numericUpDownRow";
            this.numericUpDownRow.Size = new System.Drawing.Size(140, 20);
            this.numericUpDownRow.TabIndex = 2;
            this.numericUpDownRow.ValueChanged += new System.EventHandler(this.NumericUpDownRowValueChanged);
            // 
            // comboBoxColumn
            // 
            this.comboBoxColumn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxColumn.Enabled = false;
            this.comboBoxColumn.FormattingEnabled = true;
            this.comboBoxColumn.Location = new System.Drawing.Point(64, 57);
            this.comboBoxColumn.Name = "comboBoxColumn";
            this.comboBoxColumn.Size = new System.Drawing.Size(140, 21);
            this.comboBoxColumn.TabIndex = 1;
            this.comboBoxColumn.TextUpdate += new System.EventHandler(this.ComboBoxColumnTextUpdate);
            // 
            // comboBoxTable
            // 
            this.comboBoxTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxTable.Enabled = false;
            this.comboBoxTable.FormattingEnabled = true;
            this.comboBoxTable.Location = new System.Drawing.Point(64, 30);
            this.comboBoxTable.Name = "comboBoxTable";
            this.comboBoxTable.Size = new System.Drawing.Size(140, 21);
            this.comboBoxTable.TabIndex = 1;
            this.comboBoxTable.TextUpdate += new System.EventHandler(this.ComboBoxTableTextUpdate);
            // 
            // comboBoxLanguage
            // 
            this.comboBoxLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLanguage.Enabled = false;
            this.comboBoxLanguage.FormattingEnabled = true;
            this.comboBoxLanguage.Location = new System.Drawing.Point(64, 3);
            this.comboBoxLanguage.Name = "comboBoxLanguage";
            this.comboBoxLanguage.Size = new System.Drawing.Size(140, 21);
            this.comboBoxLanguage.TabIndex = 1;
            this.comboBoxLanguage.TextUpdate += new System.EventHandler(this.ComboBoxLanguageTextUpdate);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Row";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Column";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Table";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Language";
            // 
            // XimpleCreatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 162);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(350, 200);
            this.Name = "XimpleCreatorForm";
            this.Text = "Ximple Creator";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxCells;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxValue;
        private System.Windows.Forms.NumericUpDown numericUpDownRow;
        private System.Windows.Forms.ComboBox comboBoxColumn;
        private System.Windows.Forms.ComboBox comboBoxTable;
        private System.Windows.Forms.ComboBox comboBoxLanguage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonAdd;

    }
}
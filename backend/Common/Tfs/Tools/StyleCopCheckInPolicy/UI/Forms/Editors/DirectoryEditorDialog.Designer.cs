﻿// <auto-generated/>
namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Editors
{
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Properties;

    partial class DirectoryEditorDialog
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DirectoryEditorDialog));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ValueLabel = new System.Windows.Forms.Label();
            this.ValueTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ExclusionTypeComboBox
            // 
            this.ExclusionTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("ExclusionTypeComboBox.Items"),
            resources.GetString("ExclusionTypeComboBox.Items1")});
            resources.ApplyResources(this.ExclusionTypeComboBox, "ExclusionTypeComboBox");
            // 
            // ExclusionTypeLabel
            // 
            resources.ApplyResources(this.ExclusionTypeLabel, "ExclusionTypeLabel");
            // 
            // SubmitButton
            // 
            resources.ApplyResources(this.SubmitButton, "SubmitButton");
            // 
            // AbortButton
            // 
            resources.ApplyResources(this.AbortButton, "AbortButton");
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = Resources.Warning;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // ValueLabel
            // 
            resources.ApplyResources(this.ValueLabel, "ValueLabel");
            this.ValueLabel.Name = "ValueLabel";
            // 
            // ValueTextBox
            // 
            resources.ApplyResources(this.ValueTextBox, "ValueTextBox");
            this.ValueTextBox.Name = "ValueTextBox";
            this.ValueTextBox.TextChanged += new System.EventHandler(this.ValueTextBox_TextChanged);
            // 
            // DirectoryEditorDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.ValueLabel);
            this.Controls.Add(this.ValueTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Name = "DirectoryEditorDialog";
            this.Load += new System.EventHandler(this.DirectoryEditorDialog_Load);
            this.Controls.SetChildIndex(this.ExclusionTypeLabel, 0);
            this.Controls.SetChildIndex(this.ExclusionTypeComboBox, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.pictureBox1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.AbortButton, 0);
            this.Controls.SetChildIndex(this.SubmitButton, 0);
            this.Controls.SetChildIndex(this.ValueTextBox, 0);
            this.Controls.SetChildIndex(this.ValueLabel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label ValueLabel;
        private System.Windows.Forms.TextBox ValueTextBox;

    }
}
﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Controls
{
    partial class ExclusionManagerControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExclusionManagerControl));
            this.ExclusionsListView = new System.Windows.Forms.ListView();
            this.ExclusionTypeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ValueColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DisableButton = new System.Windows.Forms.Button();
            this.EnableButton = new System.Windows.Forms.Button();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.EditButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ExclusionsListView
            // 
            resources.ApplyResources(this.ExclusionsListView, "ExclusionsListView");
            this.ExclusionsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ExclusionTypeColumnHeader,
            this.ValueColumnHeader});
            this.ExclusionsListView.FullRowSelect = true;
            this.ExclusionsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ExclusionsListView.HideSelection = false;
            this.ExclusionsListView.Name = "ExclusionsListView";
            this.ExclusionsListView.TabStop = false;
            this.ExclusionsListView.UseCompatibleStateImageBehavior = false;
            this.ExclusionsListView.View = System.Windows.Forms.View.Details;
            this.ExclusionsListView.SelectedIndexChanged += new System.EventHandler(this.ExclusionsListView_SelectedIndexChanged);
            this.ExclusionsListView.DoubleClick += new System.EventHandler(this.ExclusionsListView_DoubleClick);
            // 
            // ExclusionTypeColumnHeader
            // 
            resources.ApplyResources(this.ExclusionTypeColumnHeader, "ExclusionTypeColumnHeader");
            // 
            // ValueColumnHeader
            // 
            resources.ApplyResources(this.ValueColumnHeader, "ValueColumnHeader");
            // 
            // DisableButton
            // 
            resources.ApplyResources(this.DisableButton, "DisableButton");
            this.DisableButton.Name = "DisableButton";
            this.DisableButton.UseVisualStyleBackColor = true;
            this.DisableButton.Click += new System.EventHandler(this.DisableButton_Click);
            // 
            // EnableButton
            // 
            resources.ApplyResources(this.EnableButton, "EnableButton");
            this.EnableButton.Name = "EnableButton";
            this.EnableButton.UseVisualStyleBackColor = true;
            this.EnableButton.Click += new System.EventHandler(this.EnableButton_Click);
            // 
            // RemoveButton
            // 
            resources.ApplyResources(this.RemoveButton, "RemoveButton");
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // EditButton
            // 
            resources.ApplyResources(this.EditButton, "EditButton");
            this.EditButton.Name = "EditButton";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // AddButton
            // 
            resources.ApplyResources(this.AddButton, "AddButton");
            this.AddButton.Name = "AddButton";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // ExclusionManagerControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ExclusionsListView);
            this.Controls.Add(this.DisableButton);
            this.Controls.Add(this.EnableButton);
            this.Controls.Add(this.RemoveButton);
            this.Controls.Add(this.EditButton);
            this.Controls.Add(this.AddButton);
            this.Name = "ExclusionManagerControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView ExclusionsListView;
        private System.Windows.Forms.Button DisableButton;
        private System.Windows.Forms.Button EnableButton;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.Button EditButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.ColumnHeader ExclusionTypeColumnHeader;
        private System.Windows.Forms.ColumnHeader ValueColumnHeader;
    }
}

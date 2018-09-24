//--------------------------------------------------------------------------
// <copyright file="EditPolicyDialog.cs" company="Jeff Winn">
//      Copyright (c) Jeff Winn. All rights reserved.
//
//      The use and distribution terms for this software is covered by the
//      Microsoft Public License (Ms-PL) which can be found in the License.rtf 
//      at the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by
//      the terms of this license.
//
//      You must not remove this notice, or any other, from this software.
// </copyright>
//--------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Properties;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Controls;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Design;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Editors;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Editors.Design;

    using StyleCop;

    /// <summary>
    /// Provides a user interface to edit policy settings. This class cannot be inherited.
    /// </summary>
    internal sealed partial class EditPolicyDialog : BaseDialog
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EditPolicyDialog"/> class.
        /// </summary>
        public EditPolicyDialog()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the evaluation context being modified.
        /// </summary>
        public PolicySettings Settings
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Raises the <see cref="Submit"/> event.
        /// </summary>
        /// <param name="e">An <see cref="CancelEventArgs"/> containing event data.</param>
        protected override void OnSubmit(CancelEventArgs e)
        {
            e.Cancel = !this.SaveSettings();

            base.OnSubmit(e);
        }

        /// <summary>
        /// Disables a <see cref="ListViewItem"/> instance.
        /// </summary>
        /// <param name="item">The <see cref="ListViewItem"/> to disable.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/> is a null reference (<c>Nothing</c> in Visual Basic).</exception>
        private static void DisableListViewItem(ListViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            item.Font = new Font(item.Font, FontStyle.Italic);
        }

        /// <summary>
        /// Enables a <see cref="ListViewItem"/> instance.
        /// </summary>
        /// <param name="item">The <see cref="ListViewItem"/> to enable.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/> is a null reference (<c>Nothing</c> in Visual Basic).</exception>
        private static void EnableListViewItem(ListViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            item.Font = new Font(item.Font, FontStyle.Regular);
        }

        /// <summary>
        /// Loads the policy settings for editing.
        /// </summary>
        private void LoadSettings()
        {
            this.AllowProjectOverrideCheckBox.Checked = this.Settings.AllowProjectToOverridePolicy;
            this.TreatErrorsAsComboBox.SelectedIndex = (int)this.Settings.TaskCategory;

            if ((int)this.Settings.EvaluateOn != (int)EvaluateOnType.NotSet)
            {
                // The user has set which flags to check, load the values.
                this.EvaluateOnAddCheckBox.Checked = Utilities.IsFlagSet(this.Settings.EvaluateOn, EvaluateOnType.Add);
                this.EvaluateOnBranchCheckBox.Checked = Utilities.IsFlagSet(this.Settings.EvaluateOn, EvaluateOnType.Branch);
                this.EvaluateOnEditCheckBox.Checked = Utilities.IsFlagSet(this.Settings.EvaluateOn, EvaluateOnType.Edit);
                this.EvaluateOnMergeCheckBox.Checked = Utilities.IsFlagSet(this.Settings.EvaluateOn, EvaluateOnType.Merge);
                this.EvaluateOnRenameCheckBox.Checked = Utilities.IsFlagSet(this.Settings.EvaluateOn, EvaluateOnType.Rename);
            }

            this.LoadAllPolicyExclusions();
       }

        /// <summary>
        /// Loads all policy exclusions.
        /// </summary>
        private void LoadAllPolicyExclusions()
        {
            this.ExclusionControl.Items.Clear();

            if (this.Settings.Exclusions == null || this.Settings.Exclusions.Count == 0)
            {
                this.ExclusionControl.ListViewTabStop = false;
            }
            else
            {
                this.ExclusionControl.ListViewTabStop = true;

                foreach (PolicyExclusionConfigInfo exclusion in this.Settings.Exclusions)
                {
                    this.LoadPolicyExclusion(exclusion);
                }
            }
        }

        /// <summary>
        /// Loads the policy exclusion.
        /// </summary>
        /// <param name="exclusion">A policy exclusion to load.</param>
        private void LoadPolicyExclusion(PolicyExclusionConfigInfo exclusion)
        {
            if (exclusion == null)
            {
                return;
            }

            ExclusionAttribute attribute = Utilities.GetExclusionAttributeByEnum(exclusion.ExclusionType);
            if (attribute != null)
            {
                ListViewItem item = new ListViewItem();

                item.Text = Resources.ResourceManager.GetString(attribute.NameResourceId);

                StringBuilder sb = new StringBuilder();
                foreach (string key in exclusion.Configuration.AllKeys)
                {
                    if (string.Equals(key, PolicyExclusion.EnabledProperty, StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }

                    string value = exclusion.Configuration[key];

                    sb.Append(key).Append('=').Append(value).Append(';');
                }

                item.SubItems.Add(sb.ToString());
                item.Tag = exclusion;

                if (bool.Parse(exclusion.Configuration[PolicyExclusion.EnabledProperty]))
                {
                    EnableListViewItem(item);
                }
                else
                {
                    DisableListViewItem(item);
                }

                this.ExclusionControl.Items.Add(item);
            }
        }

        /// <summary>
        /// Saves the policy settings.
        /// </summary>
        /// <returns><b>true</b> if the settings were saved, otherwise <b>false</b>.</returns>
        private bool SaveSettings()
        {
            bool retval = true;

            this.Settings.AllowProjectToOverridePolicy = this.AllowProjectOverrideCheckBox.Checked;
            this.Settings.TaskCategory = (PolicyTaskCategory)((ComboBoxItem)this.TreatErrorsAsComboBox.SelectedItem).Tag;

            EvaluateOnType evaluateOn = EvaluateOnType.NotSet;
            if (this.EvaluateOnAddCheckBox.Checked)
            {
                evaluateOn |= EvaluateOnType.Add;
            }

            if (this.EvaluateOnBranchCheckBox.Checked)
            {
                evaluateOn |= EvaluateOnType.Branch;
            }

            if (this.EvaluateOnEditCheckBox.Checked)
            {
                evaluateOn |= EvaluateOnType.Edit;
            }

            if (this.EvaluateOnMergeCheckBox.Checked)
            {
                evaluateOn |= EvaluateOnType.Merge;
            }

            if (this.EvaluateOnRenameCheckBox.Checked)
            {
                evaluateOn |= EvaluateOnType.Rename;
            }

            if ((int)evaluateOn == (int)EvaluateOnType.NotSet)
            {
                // The user has not specified any change events to validate, notify them.
                if (MessageBox.Show(this, Resources.Message_NoChangeTypesWarning, Resources.Message_PolicyType, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return false;
                }
                else
                {
                    evaluateOn = EvaluateOnType.None;
                }
            }

            this.Settings.EvaluateOn = evaluateOn;

            return retval;
        }

        /// <summary>
        /// Occurs when the <see cref="LaunchStyleCopButton"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void LaunchStyleCopButton_Click(object sender, EventArgs e)
        {
            string tempFileName = Path.GetTempFileName();

            try
            {
                // Write the existing settings to the file before creating the UI.
                File.WriteAllText(tempFileName, this.Settings.StyleCopSettings);

                StyleCopCore core = new StyleCopCore(null, null);
                core.Initialize(null, true);
                core.DisplayUI = true;
                core.WriteResultsCache = false;

                if (core.ShowSettings(tempFileName))
                {
                    // Read the new settings from the file.
                    this.Settings.StyleCopSettings = File.ReadAllText(tempFileName);
                }
            }
            finally
            {
                // Destroy the temp file so remnants aren't left lying around.
                File.Delete(tempFileName);
            }
        }

        /// <summary>
        /// Imports existing StyleCop settings.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "A message box will be shown to the user containing the error message.")]
        private void ImportStyleCopSettings()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = string.Format(CultureInfo.CurrentCulture, Resources.Filter_StyleCopSettings, WritableSettings.DefaultFileName, WritableSettings.AlternateFileName);
                dialog.Multiselect = false;
                dialog.ShowHelp = false;
                dialog.Title = Resources.Text_ImportStyleCopSettingsTitle;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        this.Settings.StyleCopSettings = File.ReadAllText(dialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        Utilities.DisplayErrorMessageBox(this, Resources.Message_ErrorOccurredWhileImportingSettings, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Exports the current StyleCop settings to the selected file.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "A message box will be shown to the user containing the error message.")]
        private void ExportStyleCopSettings()
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.FileName = WritableSettings.DefaultFileName;
                dialog.Filter = string.Format(CultureInfo.CurrentCulture, Resources.Filter_StyleCopSettings, WritableSettings.DefaultFileName, WritableSettings.AlternateFileName);
                dialog.OverwritePrompt = true;
                dialog.ShowHelp = false;
                dialog.Title = Resources.Text_ExportStyleCopSettingsTitle;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(dialog.OpenFile()))
                        {
                            writer.Write(this.Settings.StyleCopSettings);
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.DisplayErrorMessageBox(this, Resources.Message_ErrorOccurredWhileExportingSettings, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="ImportStyleCopSettingsButton"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void ImportStyleCopSettingsButton_Click(object sender, EventArgs e)
        {
            this.ImportStyleCopSettings();
        }

        /// <summary>
        /// Occurs when the <see cref="EditPolicyDialog"/> is loading.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void EditPolicyDialog_Load(object sender, EventArgs e)
        {
            this.PopulateControls();
            this.LoadSettings();
        }

        /// <summary>
        /// Populates the controls on the form.
        /// </summary>
        private void PopulateControls()
        {
            this.TreatErrorsAsComboBox.Items.Clear();
            this.TreatErrorsAsComboBox.Items.Add(new ComboBoxItem(Resources.Text_TaskCategoryNone, PolicyTaskCategory.None));
            this.TreatErrorsAsComboBox.Items.Add(new ComboBoxItem(Resources.Text_TaskCategoryError, PolicyTaskCategory.Error));
            this.TreatErrorsAsComboBox.Items.Add(new ComboBoxItem(Resources.Text_TaskCategoryWarning, PolicyTaskCategory.Warning));
            this.TreatErrorsAsComboBox.Items.Add(new ComboBoxItem(Resources.Text_TaskCategoryMessage, PolicyTaskCategory.Message));

            this.ProjectVersionLabel.Text = string.Format(CultureInfo.CurrentCulture, this.ProjectVersionLabel.Text, Assembly.GetCallingAssembly().GetName().Version.ToString());
        }

        /// <summary>
        /// Adds a new policy exclusion.
        /// </summary>
        /// <returns><b>true</b> if the item was added, otherwise <b>false</b>.</returns>
        private bool AddItem()
        {
            bool retval = false;

            PolicyExclusionType exclusionType = this.SelectExclusionType();
            if (exclusionType != PolicyExclusionType.None)
            {
                PolicyExclusionConfigInfo config = new PolicyExclusionConfigInfo();
                config.ExclusionType = exclusionType;

                retval = this.EditPolicyExclusion(EditorMode.Add, exclusionType, ref config);
                if (retval)
                {
                    config.Configuration[PolicyExclusion.EnabledProperty] = "true";

                    this.Settings.Exclusions.Add(config);
                }
            }

            return retval;
        }

        /// <summary>
        /// Edits a policy exclusion.
        /// </summary>
        /// <param name="mode">The editor mode to load.</param>
        /// <param name="exclusionType">The type of exclusion.</param>
        /// <param name="config">The exclusion configuration being modified.</param>
        /// <returns><b>true</b> if the item was modified, otherwise <b>false</b>.</returns>
        private bool EditPolicyExclusion(EditorMode mode, PolicyExclusionType exclusionType, ref PolicyExclusionConfigInfo config)
        {
            bool retval = false;

            if (exclusionType != PolicyExclusionType.None)
            {
                ExclusionAttribute attribute = Utilities.GetExclusionAttributeByEnum(exclusionType);
                if (attribute != null)
                {
                    using (BaseEditorDialog dialog = (BaseEditorDialog)Activator.CreateInstance(attribute.EditorType))
                    {
                        dialog.EditMode = mode;
                        dialog.Value = config.Configuration;

                        retval = dialog.ShowDialog(this) == DialogResult.OK;
                    }
                }
            }

            return retval;
        }
        
        /// <summary>
        /// Displays a user interface so the user can select an exclusion type to add.
        /// </summary>
        /// <returns>The type of exclusion to add.</returns>
        private PolicyExclusionType SelectExclusionType()
        {
            PolicyExclusionType exclusionType = PolicyExclusionType.None;

            using (AddExclusionDialog dialog = new AddExclusionDialog())
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    exclusionType = dialog.Value;
                }
            }

            return exclusionType;
        }

        /// <summary>
        /// Occurs when the <see cref="ExclusionManagerControl"/> is adding an item.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="AddingItemEventArgs"/> containing event data.</param>
        private void ExclusionControl_AddingItem(object sender, ItemEventArgs e)
        {
            e.Cancel = !this.AddItem();
        }

        /// <summary>
        /// Occurs when the <see cref="ExclusionManagerControl"/> is removing an item.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ItemEventArgs"/> containing event data.</param>
        private void ExclusionControl_RemovingItem(object sender, ItemEventArgs e)
        {
            e.Cancel = !this.RemoveItem();
        }

        /// <summary>
        /// Removes the selected item.
        /// </summary>
        /// <returns><b>true</b> if the item was removed, otherwise <b>false</b>.</returns>
        private bool RemoveItem()
        {
            bool retval = false;

            if (this.ExclusionControl.SelectedItems.Count > 0)
            {
                retval = this.Settings.Exclusions.Remove((PolicyExclusionConfigInfo)this.ExclusionControl.SelectedItems[0].Tag);
            }

            return retval;
        }

        /// <summary>
        /// Occurs when the <see cref="ExclusionManagerControl"/> has added an item.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="AddingItemEventArgs"/> containing event data.</param>
        private void ExclusionControl_AddedItem(object sender, EventArgs e)
        {
            // Force the policy exclusions to be reloaded so the new exclusion is picked up.
            this.LoadAllPolicyExclusions();
        }

        /// <summary>
        /// Occurs when the <see cref="ExclusionManagerControl"/> is editing an item.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="AddingItemEventArgs"/> containing event data.</param>
        private void ExclusionControl_EditingItem(object sender, ItemEventArgs e)
        {
            this.EditItem();
        }

        /// <summary>
        /// Occurs when the <see cref="ExclusionManagerControl"/> has edited an item.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="AddingItemEventArgs"/> containing event data.</param>
        private void ExclusionControl_EditedItem(object sender, EventArgs e)
        {
            // Force the policy exclusions to be reloaded so the changed exclusion is picked up.
            this.LoadAllPolicyExclusions();
        }

        /// <summary>
        /// Edits the selected item.
        /// </summary>
        /// <returns><b>true</b> if the item was edited, otherwise <b>false</b>.</returns>
        private bool EditItem()
        {
            bool retval = false;

            if (this.ExclusionControl.SelectedItems.Count > 0)
            {
                ListViewItem item = this.ExclusionControl.SelectedItems[0];

                PolicyExclusionConfigInfo current = (PolicyExclusionConfigInfo)item.Tag;
                if (current != null)
                {
                    retval = this.EditPolicyExclusion(EditorMode.Edit, current.ExclusionType, ref current);
                }
            }

            return retval;
        }

        /// <summary>
        /// Occurs when the <see cref="ExclusionManagerControl"/> has removed an item.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> containing event data.</param>
        private void ExclusionControl_RemovedItem(object sender, EventArgs e)
        {
            this.LoadAllPolicyExclusions();
        }

        /// <summary>
        /// Occurs when the <see cref="StyleCopAddressLabel"/> link is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> containing event data.</param>
        private void StyleCopAddressLabel_Click(object sender, EventArgs e)
        {
            Process.Start(this.StyleCopAddressLabel.Text);
        }

        /// <summary>
        /// Occurs when the <see cref="ExclusionsControl"/> index has changed.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="EnableButtonsEventArgs"/> containing event data.</param>
        private void ExclusionControl_SelectedIndexChanged(object sender, EnableButtonsEventArgs e)
        {
            PolicyExclusionConfigInfo exclusion = (PolicyExclusionConfigInfo)e.Item.Tag;

            if (exclusion != null)
            {
                bool enabled = bool.Parse(exclusion.Configuration[PolicyExclusion.EnabledProperty]);

                e.EnableButton = !enabled;
                e.DisableButton = enabled;
            }
        }

        /// <summary>
        /// Occurs when the <see cref="ExclusionsControl"/> is enabling an item.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ItemEventArgs"/> containing event data.</param>
        private void ExclusionControl_EnablingItem(object sender, ItemEventArgs e)
        {
            PolicyExclusionConfigInfo exclusion = (PolicyExclusionConfigInfo)e.Item.Tag;

            if (exclusion != null)
            {
                exclusion.Configuration[PolicyExclusion.EnabledProperty] = "true";

                EnableListViewItem(e.Item);
            }
        }

        /// <summary>
        /// Occurs when the <see cref="ExclusionsControl"/> is disabling an item.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="ItemEventArgs"/> containing event data.</param>
        private void ExclusionControl_DisablingItem(object sender, ItemEventArgs e)
        {
            PolicyExclusionConfigInfo exclusion = (PolicyExclusionConfigInfo)e.Item.Tag;

            if (exclusion != null)
            {
                exclusion.Configuration[PolicyExclusion.EnabledProperty] = "false";

                DisableListViewItem(e.Item);
            }
        }

        /// <summary>
        /// Occurs when the <see cref="ExporStyleCopSettingsButton"/> is clicked.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void ExporStyleCopSettingsButton_Click(object sender, EventArgs e)
        {
            this.ExportStyleCopSettings();
        }
    }
}
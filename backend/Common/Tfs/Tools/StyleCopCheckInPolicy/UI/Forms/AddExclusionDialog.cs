//--------------------------------------------------------------------------
// <copyright file="AddExclusionDialog.cs" company="Jeff Winn">
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
    using System.Collections.Generic;
    using System.ComponentModel;

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Properties;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.UI.Forms.Design;

    /// <summary>
    /// Provides a user interface for selecting an exclusion type to add. This class cannot be inherited.
    /// </summary>
    internal sealed partial class AddExclusionDialog : BaseDialog
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddExclusionDialog"/> class.
        /// </summary>
        public AddExclusionDialog()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the selected exclusion type.
        /// </summary>
        public PolicyExclusionType Value
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Raises the <see cref="Submit"/> event.
        /// </summary>
        /// <param name="e">An <see cref="CancelEventArgs"/> containing event data.</param>
        protected override void OnSubmit(CancelEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)this.ExclusionTypeComboBox.SelectedItem;
            if (item != null)
            {
                this.Value = (PolicyExclusionType)item.Tag;
            }

            base.OnSubmit(e);
        }

        /// <summary>
        /// Validates the form.
        /// </summary>
        /// <returns><b>true</b> if the form is valid; otherwise, <b>false</b>.</returns>
        protected override bool ValidateForm()
        {
            return base.ValidateForm() && this.ExclusionTypeComboBox.SelectedIndex > 0;
        }

        /// <summary>
        /// Updates the description label for the currently selected item.
        /// </summary>
        private void UpdateDescriptionLabel()
        {
            object exclusionType = ((ComboBoxItem)this.ExclusionTypeComboBox.SelectedItem).Tag;
            if (exclusionType == null)
            {
                this.DescriptionLabel.Text = string.Empty;
            }
            else
            {
                ExclusionAttribute attribute = Utilities.GetExclusionAttributeByEnum((PolicyExclusionType)exclusionType);
                if (attribute != null)
                {
                    this.DescriptionLabel.Text = Resources.ResourceManager.GetString(attribute.DescriptionResourceId);
                }
            }
        }

        /// <summary>
        /// Populates the controls on the form.
        /// </summary>
        private void PopulateControls()
        {
            this.LoadExclusionsTypeComboBox();
        }

        /// <summary>
        /// Loads the exclusions type combo box.
        /// </summary>
        private void LoadExclusionsTypeComboBox()
        {
            this.ExclusionTypeComboBox.Items.Clear();
            this.ExclusionTypeComboBox.Items.Add(new ComboBoxItem(Resources.Text_ChooseOne, null));

            SortedList<string, ComboBoxItem> tempCollection = new SortedList<string, ComboBoxItem>();
            
            foreach (PolicyExclusionType exclusionType in Utilities.GetPolicyExclusionTypes())
            {
                ExclusionAttribute attribute = Utilities.GetExclusionAttributeByEnum(exclusionType);
                if (attribute != null)
                {
                    string name = Resources.ResourceManager.GetString(attribute.NameResourceId);

                    tempCollection.Add(name, new ComboBoxItem(name, exclusionType));
                }
            }

            if (tempCollection.Count > 0)
            {
                foreach (object item in tempCollection.Values)
                {
                    this.ExclusionTypeComboBox.Items.Add(item);
                }
            }

            this.ExclusionTypeComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Occurs when the <see cref="EditPolicyDialog"/> is loading.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void AddExclusionDialog_Load(object sender, EventArgs e)
        {
            this.PopulateControls();
        }

        /// <summary>
        /// Occurs when the <see cref="EditTypeComboBox"/> selected index has changed.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void ExclusionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateDescriptionLabel();
            this.EnableSubmitButton();
        }
    }
}
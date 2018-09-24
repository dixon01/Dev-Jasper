// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourcesView.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourcesView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// View that allows to interact with the <see cref="IResourceService"/>.
    /// </summary>
    public partial class ResourcesView : UserControl
    {
        private MediAddress selectedAddress;

        private MediAddress selectedSendFileAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesView"/> class.
        /// </summary>
        public ResourcesView()
        {
            this.InitializeComponent();

            try
            {
                this.ResourceService.FileReceived += this.ResourceServiceOnFileReceived;
            }
            catch (Exception)
            {
                // ignore
            }
        }

        private IResourceService ResourceService
        {
            get
            {
                return MessageDispatcher.Instance.GetService<IResourceService>();
            }
        }

        private ResourceId SelectedResourceId
        {
            get
            {
                var selectedItem = this.listBoxResources.SelectedItem as ResourceId;
                return selectedItem;
            }
        }

        private void ButtonBrowseClick(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.textBoxRegisterFile.Text = this.openFileDialog.FileName;
            this.buttonRegister.Enabled = true;
        }

        private void ButtonRegisterClick(object sender, EventArgs e)
        {
            this.ResourceService.BeginRegisterResource(
                this.textBoxRegisterFile.Text,
                this.checkBoxDeleteLocal.Checked,
                this.CompleteRegister,
                null);
        }

        private void CompleteRegister(IAsyncResult ar)
        {
            try
            {
                var id = this.ResourceService.EndRegisterResource(ar);
                this.Invoke(new MethodInvoker(() => this.AddResourceId(id)));
            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(
                    () => MessageBox.Show(this, ex.Message, ex.GetType().FullName)));
            }
        }

        private void AddResourceId(ResourceId id)
        {
            if (!this.listBoxResources.Items.Contains(id))
            {
                this.listBoxResources.Items.Add(id);
            }
        }

        private void ListBoxResourcesSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = this.SelectedResourceId;
            var enabled = selectedItem != null;
            this.buttonBrowseSendTo.Enabled = enabled;
            this.buttonRemove.Enabled = enabled;
            this.buttonBrowseCheckout.Enabled = enabled;
            this.buttonBrowseCheckin.Enabled = enabled;
            this.buttonBrowseExport.Enabled = enabled;

            this.propertyGrid.SelectedObject = null;

            if (!enabled)
            {
                this.textBoxSelectedResource.Clear();
                return;
            }

            this.textBoxSelectedResource.Text = selectedItem.ToString();

            this.ResourceService.BeginGetResource(selectedItem, this.CompleteGetResource, null);
        }

        private void CompleteGetResource(IAsyncResult ar)
        {
            try
            {
                var info = this.ResourceService.EndGetResource(ar);
                this.Invoke(new MethodInvoker(() => this.propertyGrid.SelectedObject = info));
            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(
                    () => MessageBox.Show(this, ex.Message, ex.GetType().FullName)));
            }
        }

        private void ButtonRemoveClick(object sender, EventArgs e)
        {
            var id = this.SelectedResourceId;
            if (id == null)
            {
                return;
            }

            this.ResourceService.BeginRemoveResource(id, this.CompleteRemove, id);
        }

        private void CompleteRemove(IAsyncResult ar)
        {
            try
            {
                var id = (ResourceId)ar.AsyncState;
                var result = this.ResourceService.EndRemoveResource(ar);
                this.Invoke(
                    new MethodInvoker(
                        () =>
                            {
                                if (result)
                                {
                                    this.listBoxResources.Items.Remove(id);
                                }

                                MessageBox.Show(this, "Success: " + result, "Remove Resource");
                            }));
            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(
                    () => MessageBox.Show(this, ex.Message, ex.GetType().FullName)));
            }
        }

        private void ButtonBrowseSendToClick(object sender, EventArgs e)
        {
            var peerSelection = new PeerSelectionForm { Text = "Browse..." };
            if (peerSelection.ShowDialog(this) != DialogResult.OK || peerSelection.SelectedAddress == null)
            {
                return;
            }

            this.selectedAddress = peerSelection.SelectedAddress;
            this.textBoxSentTo.Text = this.selectedAddress.ToString();
            this.buttonSend.Enabled = true;
        }

        private void ButtonSendClick(object sender, EventArgs e)
        {
            var info = this.propertyGrid.SelectedObject as ResourceInfo;
            if (info == null)
            {
                MessageBox.Show(
                    this,
                    "Couldn't get resource information",
                    "Resource Info Missing",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            this.ResourceService.BeginSendResource(info, this.selectedAddress, this.CompleteSend, null);
        }

        private void CompleteSend(IAsyncResult ar)
        {
            try
            {
                var result = this.ResourceService.EndSendResource(ar);
                this.Invoke(new MethodInvoker(
                    () => MessageBox.Show(this, "Success: " + result, "Send Resource")));
            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(
                    () => MessageBox.Show(this, ex.Message, ex.GetType().FullName)));
            }
        }

        private void ButtonBrowseCheckoutClick(object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.textBoxCheckout.Text = this.saveFileDialog.FileName;
            this.buttonCheckout.Enabled = true;
        }

        private void ButtonCheckoutClick(object sender, EventArgs e)
        {
            var info = this.propertyGrid.SelectedObject as ResourceInfo;
            if (info == null)
            {
                MessageBox.Show(
                    this,
                    "Couldn't get resource information",
                    "Resource Info Missing",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            this.ResourceService.BeginCheckoutResource(
                info, this.textBoxCheckout.Text, this.CompleteCheckout, null);
        }

        private void CompleteCheckout(IAsyncResult ar)
        {
            try
            {
                var result = this.ResourceService.EndCheckoutResource(ar);
                this.Invoke(new MethodInvoker(
                    () => MessageBox.Show(this, "Success: " + result, "Checkout")));
            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(
                    () => MessageBox.Show(this, ex.Message, ex.GetType().FullName)));
            }
        }

        private void ButtonBrowseCheckinClick(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.textBoxCheckin.Text = this.openFileDialog.FileName;
            this.buttonCheckin.Enabled = true;
        }

        private void ButtonCheckinClick(object sender, EventArgs e)
        {
            this.ResourceService.BeginCheckinResource(
                this.textBoxExport.Text, this.CompleteCheckin, null);
        }

        private void CompleteCheckin(IAsyncResult ar)
        {
            try
            {
                this.ResourceService.EndCheckinResource(ar);
                this.Invoke(new MethodInvoker(
                    () => MessageBox.Show(this, "Successful", "Checkin")));
            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(
                    () => MessageBox.Show(this, ex.Message, ex.GetType().FullName)));
            }
        }

        private void ButtonBrowseExportClick(object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.textBoxExport.Text = this.saveFileDialog.FileName;
            this.buttonExport.Enabled = true;
        }

        private void ButtonExportClick(object sender, EventArgs e)
        {
            var info = this.propertyGrid.SelectedObject as ResourceInfo;
            if (info == null)
            {
                MessageBox.Show(
                    this,
                    "Couldn't get resource information",
                    "Resource Info Missing",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            this.ResourceService.BeginExportResource(
                info, this.textBoxExport.Text, this.CompleteExport, null);
        }

        private void CompleteExport(IAsyncResult ar)
        {
            try
            {
                var result = this.ResourceService.EndExportResource(ar);
                this.Invoke(new MethodInvoker(
                    () => MessageBox.Show(this, "Success: " + result, "Export")));
            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(
                    () => MessageBox.Show(this, ex.Message, ex.GetType().FullName)));
            }
        }

        private void ButtonBrowseSendFileClick(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.textBoxSendFile.Text = this.openFileDialog.FileName;
            this.buttonBrowseSendFileDestination.Enabled = true;
        }

        private void ButtonBrowseSendFileDestinationClick(object sender, EventArgs e)
        {
            var peerSelection = new PeerSelectionForm { Text = "Browse..." };
            if (peerSelection.ShowDialog(this) != DialogResult.OK || peerSelection.SelectedAddress == null)
            {
                return;
            }

            this.selectedSendFileAddress = peerSelection.SelectedAddress;
            this.textBoxSendFileDestination.Text = this.selectedSendFileAddress.ToString();
            this.buttonSendFile.Enabled = true;
        }

        private void ButtonSendFileClick(object sender, EventArgs e)
        {
            this.ResourceService.BeginSendFile(
                this.textBoxSendFile.Text, this.selectedSendFileAddress, this.CompleteSendFile, null);
        }

        private void CompleteSendFile(IAsyncResult ar)
        {
            try
            {
                var result = this.ResourceService.EndSendFile(ar);
                this.Invoke(new MethodInvoker(
                    () => MessageBox.Show(this, "Success: " + result, "Send File")));
            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(
                    () => MessageBox.Show(this, ex.Message, ex.GetType().FullName)));
            }
        }

        private void ResourceServiceOnFileReceived(object sender, FileReceivedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<FileReceivedEventArgs>(this.ResourceServiceOnFileReceived), sender, e);
                return;
            }

            var msg =
                string.Format(
                    "Received file from {0}. Would you like to copy it to a local folder?\r\nOriginal name: {1}",
                    e.Source,
                    e.OriginalFileName);
            if (MessageBox.Show(
                this,
                msg,
                "File Received",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            if (this.saveFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            if (File.Exists(this.saveFileDialog.FileName))
            {
                File.Delete(this.saveFileDialog.FileName);
            }

            e.CopyTo(this.saveFileDialog.FileName);
            MessageBox.Show(this, "File was successfully saved to " + this.saveFileDialog.FileName, "File Received");
        }

        private void ResourceUpdateTimerOnTick(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                return;
            }

            var resourceTable =
                MessageDispatcher.Instance.ManagementProviderFactory.LocalRoot.GetDescendant(
                    "ResourceService") as IManagementTableProvider;
            if (resourceTable == null)
            {
                return;
            }

            // check if a new row was added to the resource service and if so update our list
            foreach (var row in resourceTable.Rows)
            {
                var hash = row.Find(p => p.Name == "Id");
                if (hash == null || !(hash.Value is string))
                {
                    continue;
                }

                var id = new ResourceId(hash.StringValue.Trim('{', '}'));

                // this will only add it if it doesn't exist already
                this.AddResourceId(id);
            }
        }
    }
}

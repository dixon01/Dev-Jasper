namespace AdminDataModelSpike.ViewModel
{
    using Gorba.Common.Medi.Core.Management;

    public class MediManagementTreeViewModel : InfoViewModelBase
    {
        private MediManagementNodeViewModel selectedNode;

        public MediManagementTreeViewModel(IRemoteManagementProvider remoteManagementProvider)
        {
            this.Name = "Management";
            this.Root = MediManagementNodeViewModel.Create(remoteManagementProvider, this);
            this.Root.IsExpanded = true;
        }

        public MediManagementNodeViewModel Root { get; private set; }

        public MediManagementNodeViewModel SelectedNode
        {
            get
            {
                return this.selectedNode;
            }

            set
            {
                this.SetProperty(ref this.selectedNode, value, () => this.SelectedNode);
            }
        }
    }
}
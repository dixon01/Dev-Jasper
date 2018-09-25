namespace AdminDataModelSpike.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;

    using Gorba.Common.Medi.Core.Management;

    public class MediManagementNodeViewModel : SynchronizableViewModelBase
    {
        private static readonly MediManagementNodeViewModel DummyChild = new MediManagementNodeViewModel("Dummy");
        private static readonly MediManagementNodeViewModel LoadingChild =
            new MediManagementNodeViewModel("Loading...");

        private readonly IManagementProvider provider;

        private bool isExpanded;

        private bool isSelected;

        private MediManagementNodeViewModel(string name)
        {
            this.Name = name;
        }

        public MediManagementNodeViewModel(IManagementProvider provider, MediManagementTreeViewModel owner)
        {
            this.Owner = owner;
            this.provider = provider;

            this.Children = new ObservableCollection<MediManagementNodeViewModel> { DummyChild };
            this.Name = this.provider.Name;
        }

        public MediManagementTreeViewModel Owner { get; private set; }

        public ObservableCollection<MediManagementNodeViewModel> Children { get; private set; }

        public string Name { get; private set; }

        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                if (!this.SetProperty(ref this.isExpanded, value, () => this.IsExpanded))
                {
                    return;
                }

                if (!this.isExpanded || this.Children.Count != 1 || this.Children[0] != DummyChild)
                {
                    return;
                }

                this.Children.Clear();
                this.Children.Add(LoadingChild);

                ThreadPool.QueueUserWorkItem(s => this.LoadChildren());
            }
        }

        private void LoadChildren()
        {
            IEnumerable<IManagementProvider> children;
            try
            {
                children = this.provider.Children;
            }
            catch (Exception)
            {
                children = Enumerable.Empty<IManagementProvider>();
            }

            this.TaskFactory.StartNew(() => this.UpdateChildren(children));
        }

        public virtual bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (!this.SetProperty(ref this.isSelected, value, () => this.IsSelected))
                {
                    return;
                }

                if (this.isSelected)
                {
                    this.Owner.SelectedNode = this;
                }
                else if (this.Owner.SelectedNode == this)
                {
                    this.Owner.SelectedNode = null;
                }
            }
        }

        private void UpdateChildren(IEnumerable<IManagementProvider> children)
        {
            this.Children.Clear();
            foreach (var child in children)
            {
                this.Children.Add(Create(child, this.Owner));
            }
        }

        public static MediManagementNodeViewModel Create(
            IManagementProvider provider, MediManagementTreeViewModel owner)
        {
            var obj = provider as IManagementObjectProvider;
            if (obj != null)
            {
                return new MediManagementObjectNodeViewModel(obj, owner);
            }

            var table = provider as IManagementTableProvider;
            if (table != null)
            {
                return new MediManagementTableNodeViewModel(table, owner);
            }

            return new MediManagementNodeViewModel(provider, owner);
        }

        public void Reload()
        {
            var remote = this.provider as IRemoteManagementProvider;
            if (remote != null)
            {
                remote.Reload();
            }
        }
    }
}
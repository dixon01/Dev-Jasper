namespace AdminDataModelSpike.ViewModel
{
    using System.Collections.ObjectModel;

    public abstract class TabableViewModelBase : SynchronizableViewModelBase
    {
        private int selectedIndex;

        protected TabableViewModelBase()
        {
            this.Tabs = new ObservableCollection<InfoViewModelBase>();
        }

        public ObservableCollection<InfoViewModelBase> Tabs { get; private set; }

        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }

            set
            {
                this.SetProperty(ref this.selectedIndex, value, () => this.SelectedIndex);
            }
        }
    }
}
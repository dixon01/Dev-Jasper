namespace AdminDataModelSpike.ViewModel
{
    using System.Collections.ObjectModel;

    public abstract class UnitViewModelBase : TabableViewModelBase
    {
        private bool isConnected;

        private bool isFavorite;

        private string name;

        public UnitViewModelBase()
        {
            this.Applications = new ObservableCollection<ApplicationViewModel>();
        }

        public bool CanConnect { get; protected set; }

        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }

            set
            {
                if (!this.SetProperty(ref this.isConnected, value, () => this.IsConnected))
                {
                    return;
                }

                if (this.isConnected)
                {
                    this.Connect();
                }
                else
                {
                    this.Disconnect();
                }
            }
        }

        protected abstract void Connect();

        protected abstract void Disconnect();

        public bool IsFavorite
        {
            get
            {
                return this.isFavorite;
            }

            set
            {
                this.SetProperty(ref this.isFavorite, value, () => this.IsFavorite);
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.SetProperty(ref this.name, value, () => this.Name);
            }
        }

        public ObservableCollection<ApplicationViewModel> Applications { get; private set; } 
    }
}
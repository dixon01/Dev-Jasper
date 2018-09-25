namespace AdminDataModelSpike.ViewModel
{
    public class InfoViewModelBase : SynchronizableViewModelBase
    {
        private string name;

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
    }
}
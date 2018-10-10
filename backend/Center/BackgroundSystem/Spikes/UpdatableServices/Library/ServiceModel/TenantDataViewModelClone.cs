namespace Library.ServiceModel
{
    public class TenantDataViewModelClone : ViewModelBase
    {
        private string name;

        public int Id { get; set; }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value == this.name)
                {
                    return;
                }

                this.name = value;
                this.OnPropertyChanged();
            }
        }
    }
}
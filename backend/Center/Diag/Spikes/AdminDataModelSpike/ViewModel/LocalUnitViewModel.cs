namespace AdminDataModelSpike.ViewModel
{
    using System;

    public class LocalUnitViewModel : UnitViewModelBase
    {
        public LocalUnitViewModel()
        {
            this.IsConnected = true;
            this.CanConnect = false;
            this.Name = Environment.MachineName;
        }

        protected override void Connect()
        {
            // nothing to do for a local unit
        }

        protected override void Disconnect()
        {
            // nothing to do for a local unit
        }
    }
}
namespace AdminDataModelSpike.ViewModel
{
    using System;
    using System.Threading;

    using Gorba.Common.Medi.Core.Management;

    public abstract class MediManagementDataNodeViewModelBase : MediManagementNodeViewModel
    {
        private bool shouldReload = true;

        protected MediManagementDataNodeViewModelBase(IManagementProvider provider, MediManagementTreeViewModel owner)
            : base(provider, owner)
        {
        }

        public override bool IsSelected
        {
            get
            {
                return base.IsSelected;
            }

            set
            {
                if (value && this.shouldReload)
                {
                    this.shouldReload = false;
                    ThreadPool.QueueUserWorkItem(s =>
                        {
                            try
                            {
                                var action = this.LoadData();
                                this.TaskFactory.StartNew(action);
                            }
                            catch (Exception)
                            {
                            }
                        });
                }

                base.IsSelected = value;
            }
        }

        protected abstract Action LoadData();
    }
}
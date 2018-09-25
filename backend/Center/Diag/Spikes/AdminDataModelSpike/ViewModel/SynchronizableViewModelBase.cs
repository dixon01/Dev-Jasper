namespace AdminDataModelSpike.ViewModel
{
    using System.Threading.Tasks;

    using Gorba.Center.Common.Wpf.Core;

    public abstract class SynchronizableViewModelBase : ViewModelBase
    {
        protected SynchronizableViewModelBase()
        {
            this.TaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected TaskFactory TaskFactory { get; private set; }
    }
}
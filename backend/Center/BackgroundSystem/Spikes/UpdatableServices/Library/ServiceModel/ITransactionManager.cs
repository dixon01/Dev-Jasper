namespace Library.ServiceModel
{
    using System;
    using System.Threading.Tasks;

    public interface ITransactionManager<T> : IDisposable
        where T : Delta
    {
        event EventHandler<NotificationReceivedEventArgs> NotificationReceived;

        Task Send(T delta);

        ITransaction BeginTransaction(int id);
    }

    public class NotificationReceivedEventArgs : EventArgs
    {
        public NotificationReceivedEventArgs(int changeset, bool succeeded)
        {
            this.Changeset = changeset;
            this.Succeeded = succeeded;
        }

        public int Changeset { get; private set; }

        public bool Succeeded { get; private set; }
    }
}

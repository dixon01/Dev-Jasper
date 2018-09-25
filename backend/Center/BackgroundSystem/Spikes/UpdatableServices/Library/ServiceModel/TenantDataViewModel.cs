namespace Library.ServiceModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Library.Annotations;

    using Microsoft.ServiceBus.Messaging;

    public class TenantDataViewModel : ITransactionalNotifyPropertyChanged
    {
        private readonly string sessionId;

        private class TenantTransaction : ITransaction
        {
            private readonly string sessionId;

            private TenantTransaction(string sessionId)
            {
                this.sessionId = sessionId;
            }

            public TenantDelta Delta { get; private set; }

            public static TenantTransaction Begin(string sessionId)
            {
                return new TenantTransaction(sessionId);
            }

            public void Dispose()
            {
            }

            public async Task Complete()
            {
                var stream = this.Delta.Serialize();
                var message = new BrokeredMessage(stream, true) { ReplyToSessionId = this.sessionId };
                //await this.topicClient.SendAsync(message).ConfigureAwait(false);
                //await this.wait.Task.ConfigureAwait(false);
            }
        }

        private string name;

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly TenantDelta tenantDelta;

        private TenantTransaction currentTransaction;

        public TenantDataViewModel(string sessionId, int id)
        {
            this.sessionId = sessionId;
            this.Id = id;
            this.tenantDelta =new TenantDelta(id, new Changeset(1));
        }

        public int Id { get; private set; }

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
                var nameChanged = new PropertyChange<string>(value);
                if (this.IsInTransaction)
                {
                    this.currentTransaction.Delta.Name = nameChanged;
                }
                else
                {
                    using (this.currentTransaction = TenantTransaction.Begin(this.sessionId))
                    {
                        this.currentTransaction.Delta.Name = nameChanged;
                        this.currentTransaction.Complete().GetAwaiter().GetResult();
                    }
                }

                this.OnPropertyChanged();
            }
        }

        public async Task Update()
        {
            this.tenantDelta.Clear();
        }

        public override string ToString()
        {
            return this.name;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Apply(TenantDelta delta)
        {
            if (delta.Name != null)
            {
                this.name = delta.Name.Value;
                this.OnPropertyChanged("Name");
            }
        }

        public event EventHandler<TransactionCompletedEventArgs> TransactionCompleted;

        public bool IsInTransaction
        {
            get
            {
                return this.currentTransaction != null;
            }
        }

        public ITransaction BeginTransaction()
        {
            this.currentTransaction = TenantTransaction.Begin(this.sessionId);
            return this.currentTransaction;
        }
    }
}
namespace Library.ServiceModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;



    /// <summary>
    /// TenantReadableModel
    /// </summary>
    public class UserUpdatableDataViewModel : ViewModelBase, IUpdatableDataViewModel
    {
        internal UserUpdatableDataViewModel(int id, Changeset changeset, string username)
        {
            this.Id = id;
            this.Changeset = changeset;
        }

        public int Id { get; private set; }

        public string Username { get; private set; }

        public UserUpdatableDataViewModel WithUsername(string username)
        {
            if (string.Equals(this.Username, username))
            {
                return this;
            }

            var clone = this.Clone();
            clone.Username = username;
            return clone;
        }

        public Changeset Changeset { get; private set; }

        public UserWritableModel ToWritable()
        {
            //
            throw new NotImplementedException();
        }

        public void UpdateFrom(UserWritableModel model)
        {
            var changeset = new Changeset(this.Changeset.Value + 1);
            var delta = new UserDelta(changeset);
            // set delta properties
            //this.OnUpdated(delta);
            this.OnUpdated();
        }

        public event EventHandler Updated;

        protected virtual void OnUpdated()
        {
            var handler = this.Updated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public void Update(UserDelta delta)
        {
            if (delta.Username != null)
            {
                this.Username = delta.Username.Value;
                this.OnPropertyChanged("Username");
            }

            this.Changeset = delta.Changeset;
        }

        public UserUpdatableDataViewModel Clone()
        {
            var clone = (this as ICloneable).Clone();
            return (UserUpdatableDataViewModel)clone;
        }

        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }

        public class PropertyChange<T>
        {
            public T Value { get; set; }
        }

        public class UserDelta : Delta, ICloneable
        {
            public UsernameChanged Username { get; set; }

            public PropertyChange<string> Name { get; set; }

            public UserDelta(Changeset changeset)
            {
                this.Changeset = changeset;
            }

            public override void Clear()
            {
                this.Username = null;
                this.IsDirty = false;
            }

            public class UsernameChanged
            {
                public string Value { get; set; }
            }

            public UserDelta ChangeUsername(string username)
            {
                if (this.Username == null)
                {
                    if (username == null)
                    {
                        return this;
                    }
                }
                else if (string.Equals(this.Username.Value, username))
                {
                    return this;
                }

                var clone = this.Clone();
                clone.Username = new UsernameChanged { Value = username };
                this.IsDirty = true;
                return clone;
            }

            public UserDelta Clone()
            {
                var clone = (this as ICloneable).Clone();
                return (UserDelta)clone;
            }

            object ICloneable.Clone()
            {
                return this.MemberwiseClone();
            }
        }

        internal class UserTransaction : ITransaction
        {
            private readonly UserUpdatableDataViewModel userUpdatableDataViewModel;

            private readonly ITransactionManager<UserDelta> transactionManager;

            private readonly TaskCompletionSource<bool> wait = new TaskCompletionSource<bool>(); 

            private UserDelta delta;

            private UserTransaction(
                ITransactionManager<UserDelta> transactionManager,
                UserUpdatableDataViewModel userUpdatableDataViewModel)
            {
                this.delta = new UserDelta(new Changeset(userUpdatableDataViewModel.Changeset.Value + 1));
                this.userUpdatableDataViewModel = userUpdatableDataViewModel;
                this.transactionManager = transactionManager;
            }

            public static UserTransaction Begin(
                ITransactionManager<UserDelta> transactionManager,
                UserUpdatableDataViewModel userUpdatableDataViewModel)
            {
                return new UserTransaction(transactionManager, userUpdatableDataViewModel);
            }

            public void ChangeUsername(string username)
            {
                this.delta = this.delta.ChangeUsername(username);
            }

            public void Dispose()
            {
                if (this.transactionManager == null)
                {
                    return;
                }

                this.transactionManager.NotificationReceived -= this.TransactionManagerOnNotificationReceived;
            }

            public async Task Complete()
            {
                this.transactionManager.NotificationReceived += this.TransactionManagerOnNotificationReceived;
                await this.transactionManager.Send(this.delta);
                //var message = new BrokeredMessage(stream, true) { ReplyToSessionId = this.sessionId };
                //await this.transactionManager.SendAsync(message).ConfigureAwait(false);
                await this.wait.Task.ConfigureAwait(false);
                if (this.wait.Task.Result)
                {
                    this.userUpdatableDataViewModel.Update(this.delta);
                }
            }

            private void TransactionManagerOnNotificationReceived(object sender, NotificationReceivedEventArgs notificationReceivedEventArgs)
            {
                if (this.delta.Changeset.Value != notificationReceivedEventArgs.Changeset)
                {
                    return;
                }

                this.wait.SetResult(notificationReceivedEventArgs.Succeeded);
            }
        }
    }

    /// <summary>
    /// TenantWritableModel
    /// </summary>
    public class UserWritableModel
    {
    }

    public class UserDataViewModel
    {
    
    }

    public interface IUpdatableDataViewModel : INotifyPropertyChanged, ICloneable
    {
        void Update(UserUpdatableDataViewModel.UserDelta delta);
    }

    [DataContract]
    public class Changeset
    {
        public Changeset(int value)
        {
            this.Value = value;
        }

        [DataMember]
        public int Value { get; private set; }
    }
}
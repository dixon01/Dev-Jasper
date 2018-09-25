using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Client
{
    using System.Collections.Concurrent;
    using System.ServiceModel;

    using Library.ServiceModel;

    public class UpdatableUserDataServiceProxy : ITransactionManager<UserUpdatableDataViewModel.UserDelta>
    {
        // use weak references
        private readonly ConcurrentDictionary<int, UserUpdatableDataViewModel> users =
            new ConcurrentDictionary<int, UserUpdatableDataViewModel>();

        public UpdatableUserDataServiceProxy(
            ClientServiceBusConfiguration serviceBusConfiguration,
            string applicationName)
        {
            this.SessionId = Guid.NewGuid().ToString("N");
            this.ServiceBusConfiguration = serviceBusConfiguration;
        }

        public ClientServiceBusConfiguration ServiceBusConfiguration { get; set; }

        public string SessionId { get; private set; }

        public ITransaction BeginTransaction(int id)
        {
            var userViewModel = this.users[id];
            return UserUpdatableDataViewModel.UserTransaction.Begin(this, userViewModel);
        }

        public async Task Start()
        {
            await Task.FromResult(0);
        }

        public UserUpdatableDataViewModel Create(UserWritableModel model)
        {
            // convert and store internally
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserUpdatableDataViewModel>> List()
        {
            var list = (await this.GetUsers()).ToList();
            list.ForEach(tenant => this.users.AddOrUpdate(tenant.Id, id => tenant, this.UpdateValueFactory));
            return list;
        }

        private async Task<IEnumerable<UserUpdatableDataViewModel>> GetUsers()
        {
            var channelFactory = new ChannelFactory<IUserDataService>(
                new NetTcpBinding(SecurityMode.None),
                "net.tcp://localhost:9999/TenantDataService");
            var channel = channelFactory.CreateChannel();
            var tenantsList = (await channel.List()).Select(this.Convert).ToList();
            (channel as ICommunicationObject).Close();
            return tenantsList;
        }

        private UserUpdatableDataViewModel Convert(User user)
        {
            // if (we laready have it)
            // apply changes
            // else
            var readableModel = new UserUpdatableDataViewModel(user.Id, new Changeset(user.Changeset), user.Username);
            readableModel.Updated += ReadableModelOnUpdated;
            throw new NotImplementedException();
        }

        private void ReadableModelOnUpdated(object sender, EventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        private UserUpdatableDataViewModel UpdateValueFactory(int id, UserUpdatableDataViewModel userUpdatableDataViewModel)
        {
            return userUpdatableDataViewModel;
        }

        public event EventHandler<NotificationReceivedEventArgs> NotificationReceived;

        protected virtual void OnNotificationReceived(NotificationReceivedEventArgs e)
        {
            var handler = this.NotificationReceived;
            if (handler == null)
            {
                return;
            }

            handler(this, e);
        }

        public int CurrentChangeset { get; private set; }

        public Task Send(UserUpdatableDataViewModel.UserDelta delta)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

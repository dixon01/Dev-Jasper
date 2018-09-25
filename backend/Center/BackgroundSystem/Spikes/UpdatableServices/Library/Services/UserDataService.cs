namespace Library.Services
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using Library.ServiceModel;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class UserDataService : IUserDataService
    {
        private readonly Dictionary<int, User> users = new Dictionary<int, User>
                                                           {
                                                               {
                                                                   1,
                                                                   new User
                                                                       {
                                                                           Changeset = 1,
                                                                           Id = 1,
                                                                           Username = "User01"
                                                                       }
                                                               },
                                                               {
                                                                   2,
                                                                   new User
                                                                       {
                                                                           Changeset = 1,
                                                                           Id = 1,
                                                                           Username = "User01"
                                                                       }
                                                               }
                                                           };
        public Task<User> Add(User User)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<User>> List()
        {
            await Task.FromResult(0);
            return this.users.Values;
        }

        public async Task<User> Update(User user)
        {
            await Task.FromResult(0);
            var existingUser = this.users[user.Id];
            existingUser.Changeset = user.Changeset;
            existingUser.Username = user.Username;
            return existingUser;
        }

        public async Task<User> Get(int id)
        {
            await Task.FromResult(0);
            return this.users[id];
        }
    }
}

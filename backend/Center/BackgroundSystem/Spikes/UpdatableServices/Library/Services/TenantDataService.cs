namespace Library.Services
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using Dapper;

    using Library.Model;
    using Library.ServiceModel;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class TenantDataService : ITenantDataService
    {
        public Task<Tenant> Add(Tenant tenant)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<Tenant>> List()
        {
            using (var connection = this.CreateConnection())
            {
                var tenants =
                    (await connection.QueryAsync<Tenant>("SELECT * FROM [Tenants]")).ToList();
                return tenants;
            }
        }

        public async Task<Tenant> Update(Tenant tenant)
        {
            using (var connection = this.CreateConnection())
            {
                await connection.ExecuteAsync("UPDATE [Tenants] SET [Name] = @Name where [Id] = @Id", tenant);
                return tenant;
            }
        }

        public async Task<Tenant> Get(int id)
        {
            using (var connection = this.CreateConnection())
            {
                return
                    (await connection.QueryAsync<Tenant>("SELECT * FROM [Tenants] WHERE [Id] = @id", new {id}))
                        .SingleOrDefault();
            }
        }

        private IDbConnection CreateConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
            return new SqlConnection(connectionString);
        }
    }
}

using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Library.ServiceModel
{
    using Library.Model;

    [ServiceContract]
    public interface ITenantDataService
    {
        [OperationContract]
        Task<Tenant> Add(Tenant tenant);

        [OperationContract]
        Task<IEnumerable<Tenant>> List();

        [OperationContract]
        Task<Tenant> Update(Tenant tenant);

        [OperationContract]
        Task<Tenant> Get(int id);
    }
}

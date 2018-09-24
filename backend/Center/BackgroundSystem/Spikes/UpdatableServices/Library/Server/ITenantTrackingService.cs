namespace Library.Server
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using Library.Tracking;

    [ServiceContract]
    public interface ITenantTrackingService
    {
        [OperationContract]
        Task<TenantReadableModel> Get(int id);

        [OperationContract]
        Task<IEnumerable<TenantReadableModel>> List();
    }
}
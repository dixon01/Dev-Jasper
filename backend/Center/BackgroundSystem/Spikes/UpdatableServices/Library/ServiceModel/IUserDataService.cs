namespace Library.ServiceModel
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading.Tasks;

    [ServiceContract]
    public interface IUserDataService
    {
        [OperationContract]
        Task<IEnumerable<User>> List();
    }
}
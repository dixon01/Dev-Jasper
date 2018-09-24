namespace ConsoleHost
{
    using System;
    using System.ServiceModel;

    using Library;
    using Library.Client;
    using Library.Model;
    using Library.Server;
    using Library.ServiceModel;
    using Library.Services;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting...");
                //var bootstrapper = new Bootstrapper();
                //AsyncPump.Run(bootstrapper.Bootstrap);
                //var tenantDataService = new TenantDataService();
                //var userDataService = new UserDataService();
                //var updatableDataManager =
                //    new UpdatableDataManager(
                //        UpdatableDataServiceManager.TenantsClientConfiguration,
                //        tenantDataService,
                //        userDataService,
                //        UpdatableDataServiceManager.UsersClientConfiguration);
                //AsyncPump.Run(updatableDataManager.Start);
                var tenantDataService = new TenantTrackingService();
                using (var serviceHost = new ServiceHost(tenantDataService))
                {
                    var binding = Utility.GetBinding();
                    var address = Utility.GetAddress<Tenant>();
                    serviceHost.AddServiceEndpoint(typeof(ITenantTrackingService), binding, address);
                    AsyncPump.Run(tenantDataService.Start);
                    serviceHost.Open();
                    Console.WriteLine("Type <Enter> to shutdown service");
                    Console.ReadLine();
                }
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: {0}", exception.StackTrace);
                Console.ResetColor();
            }

            Console.WriteLine("Type <Enter> to exit");
            Console.ReadLine();
        }
    }
}
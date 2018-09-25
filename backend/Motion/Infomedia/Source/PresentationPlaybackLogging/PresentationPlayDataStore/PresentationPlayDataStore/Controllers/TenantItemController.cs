// PresentationPlayLogging
// PresentationPlayLogging.DataStore
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.PresentationPlayLogging.DataStore.Controllers
{
    using System;
    using System.Linq;

    using Luminator.PresentationPlayLogging.DataStore.Interfaces;
    using Luminator.PresentationPlayLogging.DataStore.Models;

    /// <summary>The tenant controller from the destinations db.</summary>
    public class TenantController : EFControllerBase<DestinationDbContext, Tenant>
    {
        /// <summary>Initializes a new instance of the <see cref="TenantController"/> class.</summary>
        /// <param name="context">The context.</param>
        public TenantController(DestinationDbContext context)
            : base(context)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TenantController"/> class.</summary>
        /// <param name="connectionString">The connection string.</param>
        public TenantController(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>Add a default tenant.</summary>
        /// <param name="tenantName">The tenant name.</param>
        /// <returns>The <see cref="Tenant"/>.</returns>
        public Tenant AddDefaultTenant(string tenantName = "Default")
        {
#if __UseDestinations
            if (!this.Context.Tenants.Any())
            {
                var defaultTenant = new Tenant() { Name = tenantName };
                var item = this.Context.Tenants.Add(defaultTenant);
                this.Context.SaveChanges();
                return item;
            }

            return this.Context.Tenants.FirstOrDefault(m => m.Name == tenantName);
#else
            throw new NotSupportedException();
#endif
        }
    }
}
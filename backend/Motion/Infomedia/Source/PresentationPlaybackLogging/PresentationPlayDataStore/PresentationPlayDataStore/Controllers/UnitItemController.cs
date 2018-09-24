// PresentationPlayLogging
// PresentationPlayLogging.DataStore
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.PresentationPlayLogging.DataStore.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Luminator.PresentationPlayLogging.DataStore.Interfaces;
    using Luminator.PresentationPlayLogging.DataStore.Models;

    /// <summary>The unit controller.</summary>
    public class UnitItemController : EFControllerBase<DestinationDbContext, Unit>, IUnitItemController
    {
        /// <summary>Initializes a new instance of the <see cref="UnitItemController"/> class.</summary>
        /// <param name="context">The context.</param>
        public UnitItemController(DestinationDbContext context)
            : base(context)
        {
        }

        /// <summary>The get unit by name.</summary>
        /// <param name="name">The unit name.</param>
        /// <returns>The <see cref="UnitItem"/>.</returns>
        public override Unit GetByName(string name)
        {
#if __UseDestinations
            var item = this.Context.Units.FirstOrDefault(m => m.Name == name);
            return item;
#else
            return null;
#endif
        }

        public ICollection<Unit> GetList(Guid? tenandId)
        {
#if __UseDestinations
            return tenandId != Guid.Empty ?
                this.Context.Units.Where(m => m.TenantId == tenandId).ToList() :
                       base.GetList();
#else
            return new List<Unit>();
#endif
        }
    }
}
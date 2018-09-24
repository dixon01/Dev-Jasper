namespace Luminator.PresentationPlayLogging.DataStore.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Luminator.PresentationPlayLogging.DataStore.Models;

    public interface IUnitItemController
    {
        Unit GetByName(string name);

        ICollection<Unit> GetList(Guid? tenandId);
    }
}
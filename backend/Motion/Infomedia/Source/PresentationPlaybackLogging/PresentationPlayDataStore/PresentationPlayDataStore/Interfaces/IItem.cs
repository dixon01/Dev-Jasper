namespace Luminator.PresentationPlayLogging.DataStore.Interfaces
{
    using System;

    public interface IItem
    {
        DateTime Created { get; set; }

        Guid Id { get; set; }

        DateTime? Modified { get; set; }
    }
}
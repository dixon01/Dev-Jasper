namespace Luminator.PresentationPlayLogging.DataStore.Interfaces
{
    using System.Collections.Generic;

    using Luminator.PresentationPlayLogging.DataStore.Models;

    public interface IPresentationPlayLogItemController
    {
        int AddRange(IList<PresentationPlayLoggingItem> presentationPlayLoggingItems, int batchSize = 10000);
    }
}
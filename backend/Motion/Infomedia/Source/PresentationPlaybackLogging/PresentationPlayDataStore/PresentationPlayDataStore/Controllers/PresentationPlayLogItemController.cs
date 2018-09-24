// PresentationPlayLogging
// PresentationPlayLogging.DataStore
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.PresentationPlayLogging.DataStore.Controllers
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using EntityFramework.BulkInsert.Extensions;

    using Luminator.PresentationPlayLogging.DataStore.Models;

    using NLog;

    /// <summary>The unit controller.</summary>
    public class PresentationPlayLogItemController // :
    {
        // EFControllerBase<PresentationPlayLoggingDbContext, PresentationPlayLoggingItem>, IPresentationPlayLogItemController
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>Initializes a new instance of the <see cref="PresentationPlayLogItemController"/> class.</summary>
        /// <param name="context">The context.</param>
        public PresentationPlayLogItemController(PresentationPlayLoggingDbContext context)
        {
            this.Context = context;
        }

        private PresentationPlayLoggingDbContext Context { get; }

        /// <summary>Add presentation log items to the database.</summary>
        /// <param name="presentationPlayLoggingItems">The presentation Play Logging Items.</param>
        /// <param name="batchSize">The batch Size.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int AddRange(IList<PresentationPlayLoggingItem> presentationPlayLoggingItems, int batchSize = 10000)
        {
            if (presentationPlayLoggingItems != null)
            {
                Logger.Info("Adding new presentation log records Count={0}", presentationPlayLoggingItems.Count());

                // CAUTION performance!

                // this.Configuration.AutoDetectChangesEnabled = false;
                Logger.Info("Saving SQL Records Start");

                // foreach (var item in presentationPlayLoggingItems)
                // {
                // this.Context.PresentationLogItems.Add(item);
                // }
                this.Context.BulkInsert(presentationPlayLoggingItems, SqlBulkCopyOptions.Default, batchSize);

                Logger.Info("Saving SQL Records Finished");

                // this.Context.SaveChanges();
                return presentationPlayLoggingItems.Count();
            }

            return 0;
        }
    }
}
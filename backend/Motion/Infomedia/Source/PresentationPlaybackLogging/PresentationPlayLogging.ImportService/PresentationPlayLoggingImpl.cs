// InfomediaAll
// PresentationPlayLogging.ImportService
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.PresentationPlayLogging.ImportService
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Runtime.Caching;
    using System.Threading;
    using System.Threading.Tasks;

    using AutoMapper;

    using Gorba.Common.Utility.Core;

    using Luminator.PresentationPlayLogging.Config.Interfaces;
    using Luminator.PresentationPlayLogging.Core;
    using Luminator.PresentationPlayLogging.Core.Models;
    using Luminator.PresentationPlayLogging.DataStore;
    using Luminator.PresentationPlayLogging.DataStore.Controllers;
    using Luminator.PresentationPlayLogging.DataStore.Models;

    using NLog;

    /// <summary>The presentation play logging import from csv files to database implementation.</summary>
    public class PresentationPlayLoggingImpl : IDisposable
    {
        private static readonly object DatabaseCheckLock = new object();
        
        private static readonly object FileImportProcessingLock = new object();

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IPresentationPlayLoggingConfig config;

        private bool disposed;

        // File watcher on new created log files to be imported

        private ITimer timer;

        static PresentationPlayLoggingImpl()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<InfotransitPresentationInfo, PresentationPlayLoggingItem>());
        }

        /// <summary>Initializes a new instance of the <see cref="PresentationPlayLoggingImpl" /> class.</summary>
        /// <param name="config">The presentation play logging config instance.</param>
        public PresentationPlayLoggingImpl(IPresentationPlayLoggingConfig config)
        {
            this.config = config;
            this.CreateTimer();
        }

        /// <summary>Initializes a new instance of the <see cref="PresentationPlayLoggingImpl"/> class.</summary>
        public PresentationPlayLoggingImpl()
        {
        }

        /// <summary>The bad file processed event when a file cannot be processed.</summary>
        public event EventHandler<string> BadFileProcessed;

        private string ConnectionString => this.config?.ServerConfig.ConnectionString;

        private string DestinationsConnectionString => this.config.ServerConfig.DestinationsConnectionString;

        private string FileWatchFolder => this.config?.ServerConfig.FileWatchFolder;

        private string WatchFileFilter => this.config?.ServerConfig.WatchFileFilter;

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                this.Stop();
            }
        }

        /// <summary>
        ///     Start the file watcher and database import process
        /// </summary>
        public void Start()
        {
            if (this.config == null)
            {
                throw new NullReferenceException("PresentationPlayLoggingImpl Invalid Config");
            }

            Logger.Info("Starting PresentationPlayLogging watching for new files in [{0}]", this.WatchFileFilter);
            if (string.IsNullOrEmpty(this.WatchFileFilter))
            {
                throw new DirectoryNotFoundException("Directory not found or path missing for file watcher");
            }

            this.ValidateDatabase(this.ConnectionString);

            this.ImportExistingPresentationPlayLogFiles();

            Logger.Info("Started");
        }

        private void CreateTimer()
        {
            Logger.Debug("Creating polling timer.");
            this.timer = TimerFactory.Current.CreateTimer("PresentationPlayLoggingTimer");
            if (this.config.ServerConfig == null)
            {
                Logger.Warn("Invalid server config, unable to create polling timer.");
                return;
            }

            Logger.Info($"Creating timer, polling interval: {this.config.ServerConfig.PollInterval}");
            
            this.timer.Interval = this.config.ServerConfig.PollInterval;
            this.timer.Elapsed += this.PollingTimerOnElapsed;
            this.timer.Enabled = true;
            this.timer.AutoReset = true;
        }

        /// <summary>Stop processing.</summary>
        public void Stop()
        {
            Logger.Info("Stopping PresentationPlayLogging");
            this.timer.Enabled = false;
        }

        private void PollingTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            Logger.Info("Checking for import files.");
            this.ImportExistingPresentationPlayLogFiles();
        }

        /// <summary>
        /// Test getting full file access
        /// </summary>
        /// <param name="file">Full file name</param>
        /// <returns>True if file access</returns>
        private static bool GetFileReadAccess(string file)
        {
            var count = 10;
            do
            {
                FileStream s = null;
                try
                {
                    s = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
                    if (s.CanRead)
                    {
                        Logger.Info("Ready to process file {0}", file);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("Opening file {0} {1}", file, ex.Message);
                    Thread.Sleep(1000);
                }
                finally
                {
                    s?.Close();
                }
            }
            while (count-- > 0);
            return false;
        }

        private void DeleteFile(string file)
        {
            try
            {
                Logger.Info("Deleting file {0}", file);
                File.Delete(file);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to delete file {0} {1}", file, ex.Message);
            }
        }

        private Guid FindTenant(string unitName)
        {
            var tenantId = Guid.Empty;
            using (var destinationDbContext = new DestinationDbContext(this.DestinationsConnectionString))
            {
                Logger.Info("Find TenantId for Unit {0}", unitName);

                var m = MemoryCache.Default.GetCacheItem(unitName)?.Value as Unit;
                if (m?.TenantId != null)
                {
                    tenantId = m.TenantId.Value;
                }
                else
                {
                    var controller = new UnitItemController(destinationDbContext);
                    var unit = controller.GetByName(unitName);
                    if (unit != null)
                    {
                        var cacheItemPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(10) };
                        MemoryCache.Default.Add(unitName, unit, cacheItemPolicy);
                    }

                    tenantId = unit?.TenantId ?? Guid.Empty;
                }

                Logger.Info("TenantId for Unit {0} = {1}", unitName, tenantId);
            }

            return tenantId;
        }

        Dictionary<string, Unit> GetUnitsCache(IList<string> unitNames, bool addMissingUnits = false)
        {
            var units = new Dictionary<string, Unit>();
            if (unitNames == null || unitNames.Count == 0)
            {
                return units;
            }

#if __UseDestinations
            using (var context = new PresentationPlayLoggingDbContext(this.ConnectionString))
            {
                var dbDestinationConext = new DestinationDbContext(asdf);

                var tenantItemController = new TenantController(dbDestinationConext);
                var tenantItem = tenantItemController.GetByName("Default");
                Guid? tenantId = tenantItem?.Id;
                var controller = new UnitItemController(dbDestinationConext);

                foreach (var unitName in unitNames)
                {
                    var item = controller.GetByName(unitName);
                    if (item != null)
                    {
                        units.Add(unitName, item);
                    }
                    else if (addMissingUnits)
                    {
                        var newUnitItem = context.Units.Add(new Unit()
                        {
                            Name = unitName,
                            TenantId = tenantId
                        });
                        var m = controller.Create(newUnitItem);
                        units.Add(unitName, m);
                    }
                }
            }
#endif

            return units;
        }

        /// <summary>
        /// Get the Units for the Default Tenant
        /// </summary>
        /// <returns></returns>
        ICollection<Unit> GetUnitsForDefaultTenant()
        {
            using (var context = new DestinationDbContext(this.ConnectionString))
            {
                var tenantItemController = new TenantController(context);
                var tenantItem = tenantItemController.GetByName("Default");
                var controller = new UnitItemController(context);
                return controller.GetList(tenantItem?.Id ?? Guid.Empty);
            }
        }

        private void ImportExistingPresentationPlayLogFiles()
        {
            if (Monitor.TryEnter(FileImportProcessingLock, TimeSpan.FromMilliseconds(10)))
            {
                try
                {
                    Logger.Debug("Lock acquired, processing files.");
                    var path = this.FileWatchFolder;
                    string[] searchPatterns = this.WatchFileFilter.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    Logger.Debug($"Processing files, pattern: {this.WatchFileFilter}");
                    foreach (var pattern in searchPatterns)
                    {
                        Logger.Info($"ImportExistingPresentationPlayLogFiles Path={path}, Filter{pattern}");

                        // root then the sub folders
                        var files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
                        if (pattern.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
                        {
                            Parallel.ForEach(files, this.UnZipPresentationPlayLogZipFile);
                        }
                        else
                        {
                            Parallel.ForEach(files, this.ImportPresentationPlayLoggingFile);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Failed to read in the file, take action here, 
                    Logger.Warn("ImportExistingPresentationPlayLogFiles Exception Cause:{0}", ex.Message);
                }
                finally
                {
                    Monitor.Exit(FileImportProcessingLock);
                }
            }
        }

        private void ImportPresentationPlayLoggingFile(string file)
        {
            if (File.Exists(file) == false)
            {
                throw new FileNotFoundException("PresentationPlayLogging file not found", file);
            }

            Logger.Info("Importing Presentation log file {0}", file);
            var doCleanup = false;
            try
            {
                GetFileReadAccess(file);

                // read in the csv file
                var records = PresentationInfotransitCsvLogging.ReadAll(file);

                if (records.Any())
                {
                    // Import the data to the sql database
                    var count = this.ImportPresentationPlayLogRecordsToSql(records);
                    doCleanup = true;
                    Logger.Info("* Successfullying imported {0} records from csv presentaiton log file {1}", count, file);
                }
            }
            catch (Exception ex)
            {
                // Failed to read in the file, take action here, 
                Logger.Warn("Failed to Read file:{0}, Cause:{1}", file, ex.Message);

                if (ex is DbUpdateException)
                {
                    var updateException = (DbUpdateException)ex;
                    Logger.Error("Failed to Read file:{0}, Cause:{1}", file, updateException.Message);
                }
                else
                {
                    this.MoveBadFile(file, ex);
                }
            }
            finally
            {
                if (doCleanup)
                {
                    Logger.Info("Success Importing file:{0}, cleaning up and deleting it", file);
                    this.DeleteFile(file);
                }
            }
        }

        private void MoveBadFile(string file, Exception ex)
        {
            // cleanup, move the bad file 
            if (File.Exists(file))
            {
                var fullFileName = file;
                var badFilesFolder = this.config.ServerConfig.BadFileFolder;
                if (ex is FileNotFoundException)
                {
                    Logger.Error(ex.Message);
                }
                else if (string.IsNullOrEmpty(badFilesFolder) == false)
                {
                    fullFileName = Path.Combine(badFilesFolder, file);
                    Logger.Warn("Moving bad file:{0}, to {1}", file, badFilesFolder);
                    this.MovePresentationLogFile(file, badFilesFolder);
                }

                this.BadFileProcessed?.Invoke(this, fullFileName);
            }
        }

        private int ImportPresentationPlayLogRecordsToSql(IList<InfotransitPresentationInfo> records)
        {
            using (var context = new PresentationPlayLoggingDbContext(this.ConnectionString))
            {
                var playLoggingItems = new List<PresentationPlayLoggingItem>();

                var unitNames = this.config.ServerConfig.LookupUnits ? records.Select(m => m.UnitName).Distinct().ToList() : null;
                var unitsCache = this.GetUnitsCache(unitNames, true);

                // map the classes from the import POCO to the EF db model
                long recordCount = 0;

                foreach (var record in records)
                {
                    try
                    {
                        var playLoggingItem = Mapper.Map<PresentationPlayLoggingItem>(record);

                        // how much time was the play duration, we will use the Duration column or create a new one in the schema
                        // Do we want to store the Desired Play duration time and the actual ?
                        if (playLoggingItem.PlayStarted.HasValue && playLoggingItem.PlayStopped.HasValue)
                        {
                            var ts = playLoggingItem.PlayStopped.Value - playLoggingItem.PlayStarted.Value;
                            playLoggingItem.PlayedDuration = Convert.ToInt32(ts.TotalSeconds);
                        }

                        if (unitsCache?.Count > 0)
                        {
                            // if we want to parent up the record to the Unit set it's parent entity Id
                            var unitItem = unitsCache.FirstOrDefault(m => m.Key == record.UnitName).Value;
                            if (unitItem != null)
                            {
                                playLoggingItem.UnitId = unitItem?.Id ?? null;
                            }
                        }

                        playLoggingItems.Add(playLoggingItem);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn("AutoMap Failed Record# {0} {1}", recordCount, ex.Message);
                    }

                    recordCount++;
                }

                var controller = new PresentationPlayLogItemController(context);
                return controller.AddRange(playLoggingItems);
            }
        }

        private void MovePresentationLogFile(string file, string destinationPath)
        {
            try
            {
                Logger.Info("Moving file {0} => {1}", file, destinationPath);
                if (Directory.Exists(destinationPath) == false)
                {
                    Directory.CreateDirectory(destinationPath);
                }

                var f = Path.GetFileName(file);
                var extension = Path.GetExtension(file);
                if (f == null)
                {
                    return;
                }

                var count = 0;
                var destinationFileName = Path.Combine(destinationPath, f);
                while (File.Exists(destinationFileName))
                {
                    destinationFileName = Path.Combine(destinationPath, $"{Path.GetFileNameWithoutExtension(f)}-{++count}{extension}");
                }

                File.Move(file, destinationFileName);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to move file {0} {1}", file, ex.Message);
            }
        }

        private void UnZipPresentationPlayLogZipFile(string zipFile)
        {
            try
            {
                // Unzip the file and delete the zip
                GetFileReadAccess(zipFile);

                bool zipHasCsvFiles = false;
                try
                {
                    using (var zipArchive = new ZipArchive(File.Open(zipFile, FileMode.Open, FileAccess.Read)))
                    {
                        zipHasCsvFiles = zipArchive.Entries.Any(m => m.Name.EndsWith("csv"));
                        if (zipHasCsvFiles)
                        {
                            var destinationDirectoryName = Path.GetDirectoryName(zipFile);
                            zipArchive.ExtractToDirectory(destinationDirectoryName);
                        }
                    }
                }
                catch (Exception e)
                {
                    this.MoveBadFile(zipFile, e);
                }

                if (zipHasCsvFiles)
                {
                    this.DeleteFile(zipFile);
                }
            }
            catch (Exception ex)
            {
                
                Logger.Error("UnZipPresentationPlayLogZipFile Exception {0}", ex.Message);
            }
        }

        private void ValidateDatabase(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("Invalid or missing Connection string. Database connection failed!");
            }

            lock (DatabaseCheckLock)
            {
                using (var context = new PresentationPlayLoggingDbContext(connectionString))
                {
                    context.CreateDatabase(connectionString);
                    if (context.Database.Exists() == false)
                    {
                        throw new ApplicationException("Presentation Database not found!");
                    }
                }
            }
        }

        private void UnzipFile(string zipFile)
        {
            Task.Run(() => { this.UnZipPresentationPlayLogZipFile(zipFile); });
        }
    }
}
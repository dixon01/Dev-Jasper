// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator LTG" file="CsvFileHelper.cs">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.Utility.CsvFileHelper
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using CsvHelper;
    using CsvHelper.Configuration;

    using NLog;

    /// <summary>Csv file Helper wrapping CsvHelper.</summary>
    /// <typeparam name="T">Class Model type to serialize to Csv file</typeparam>
    public class CsvFileHelper<T> : IDisposable
        where T : class
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>The exit completed.</summary>
        private readonly ManualResetEvent doWritesTaskExitedEvent = new ManualResetEvent(false);

        /// <summary>The quit event.</summary>
        private readonly ManualResetEvent killTaskEvent = new ManualResetEvent(false);

        private readonly AutoResetEvent recordAddedToQueEvent = new AutoResetEvent(false);

        /// <summary>The write que.</summary>
        private readonly ConcurrentQueue<T> writeQue = new ConcurrentQueue<T>();

        /// <summary>
        /// Used for locking when Open() is called.
        /// </summary>
        private readonly object openZipLock = new object();

        /// <summary>The disposed.</summary>
        private bool disposed;

        private bool disposing;

        /// <summary>Set when stream is closing state</summary>
        private bool streamClosing;

        private Type thisModelType = typeof(T);

        private CsvFileWriter writer;

        /// <summary>The writerTask.</summary>
        private Task writerTask;

        /// <summary>Zip file name entries</summary>
        private List<string> zipFileEntries = new List<string>();

        /// <summary>Initializes a new instance of the <see cref="CsvFileHelper{T}" /> class. Construct CsvFileHelper</summary>
        /// <param name="fileName">Csv FileName</param>
        /// <param name="csvClassMappingType">The csv Class Mapping Type.</param>
        /// <param name="csvConfiguration">Optional Configuration to control output</param>
        /// <param name="fileMode">FileMode, default OpenOrCreate</param>
        public CsvFileHelper(
            string fileName,
            Type csvClassMappingType = null,
            Configuration csvConfiguration = null,
            FileMode fileMode = FileMode.OpenOrCreate)
            : this()
        {
            this.Open(fileName, csvClassMappingType, csvConfiguration, fileMode);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CsvFileHelper{T}" /> class.
        ///     Prevents a default instance of the <see cref="CsvFileHelper" /> class from being created.
        ///     Prevents a default instance of the <see cref="MyCsvLogger" /> class from being created. Initializes a new
        ///     instance of the <see cref="CsvFileHelper{T}" /> class.
        /// </summary>
        public CsvFileHelper()
        {
            this.StartWriterTask(); // consider moving to Open()
        }

        /// <summary>Initializes a new instance of the <see cref="CsvFileHelper{T}" /> class.</summary>
        /// <param name="writer">The writer.</param>
        public CsvFileHelper(CsvFileWriter writer)
        {
            this.writer = writer;
            this.StartWriterTask();
        }

        /// <summary>Finalizes an instance of the <see cref="CsvFileHelper{T}" /> class. </summary>
        ~CsvFileHelper()
        {
            Debug.WriteLine("~CsvFileHelper() Enter");
            this.Dispose();
        }

        /// <summary>The on file was closed.</summary>
        public event EventHandler<string> OnFileClosed;

        /// <summary>The on file moved.</summary>
        public event EventHandler<string> OnFileMoved;

        /// <summary>The on file was opened.</summary>
        public event EventHandler<string> OnFileOpened;

        /// <summary>Gets the csv file writer.</summary>
        public CsvFileWriter CsvFileWriter { get; private set; }

        /// <summary>Gets the CsvWriter.</summary>
        public CsvWriter CsvWriter => this.CsvFileWriter?.CsvWriter;

        /// <summary>Gets or sets the current csv class map.</summary>
        /// <exception cref="NullReferenceException"></exception>
        public ClassMap<T> CurrentClassMap
        {
            get
            {
                var map = this.CsvWriter?.Configuration.Maps[typeof(T)] as ClassMap<T>;
                return map;
            }

            set
            {
                if (this.CsvWriter?.Configuration != null)
                {
                    this.CsvWriter.Configuration.RegisterClassMap(value);
                }
                else
                {
                    throw new NullReferenceException("CsvWritter not created see Open");
                }
            }
        }

        /// <summary>Gets the file length.</summary>
        public long FileLength => this.FileStream?.Length ?? 0;

        /// <summary>Gets the file name.</summary>
        public string FileName => this.CsvFileWriter?.FileName;

        /// <summary>Gets or sets the file rollover type optionally used.</summary>
        public FileNameRolloverType FileNameRolloverType { get; set; } = FileNameRolloverType.None;

        /// <summary>Gets a value indicating whether the file stream is opened.</summary>
        public bool IsOpen { get; private set; }

        /// <summary>Gets or sets the max file size.</summary>
        public long MaxFileSize { get; set; }

        /// <summary>Gets or sets the max records.</summary>
        public long MaxRecords { get; set; }

        /// <summary>Gets the total records count in the current file.</summary>
        public long Records { get; private set; }

        /// <summary>Gets or sets the optional file rollover file path.</summary>
        public string RollOverFilePath { get; set; } = string.Empty;

        /// <summary>Gets the file stream.</summary>
        private FileStream FileStream => this.CsvFileWriter?.FileStream;

        private bool WriterTaskStarted { get; set; }

        /// <summary>Get record count from CSV File.</summary>
        /// <param name="fileName">The CSV file name.</param>
        /// <returns>The <see cref="int" />.</returns>
        public static int GetCsvFileRecordCount(string fileName)
        {
            Info("GetCsvFileRecordCount() Enter File:{0}", fileName);
            using (var reader = new CsvReader(new StreamReader(fileName), new Configuration()))
            {
                var records = reader.GetRecords<T>();
                Info("GetCsvFileRecordCount() records={0} File:{1}", records, fileName);
                return records.Count();
            }
        }

        /// <summary>Close the file stream.</summary>
        public void Close()
        {
            Info("Close() File:{0} Enter", this.FileName);
            if (this.streamClosing)
            {
                return;
            }

            this.streamClosing = true;

            // Note this worker thread can't write if locked
            this.recordAddedToQueEvent.Set();

            if (this.FileStream == null || this.FileStream.CanWrite == false)
            {
                // not opened
                Info("File Already closed File {0}", this.FileName);
                return;
            }

            Info("Closing CSV Log File {0}, waiting for writes to complete...", this.FileName);
            var locked = this.WaitForFileLock(TimeSpan.FromSeconds(30));
            var fileName = this.FileName;

            try
            {
                if (locked)
                {
                    if (this.CsvWriter != null)
                    {
                        this.FileStream?.Flush(true);

                        // stream closed after disposed and will need to be recreated or Opened
                        Info("Disposing and closing Stream...{0}", this.FileName);
                        this.CsvWriter.Dispose();
                        this.FileStream?.Close();

                        var fileInfo = new FileInfo(this.FileName);
                        Info(
                            "! Closing CSV Log File {0} Dispose CsvWriter FileSize = {1}, Records = {2}",
                            this.FileName,
                            fileInfo.Length,
                            this.Records);
                    }
                }
                else
                {
                    Logger.Error("Failed to acquire lock in Close Stream and Dispose!");
                    this.CsvWriter?.Dispose();
                    this.FileStream?.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            finally
            {
                this.CsvFileWriter.FileStream = null;
                this.CsvFileWriter.CsvWriter = null;
                this.IsOpen = false;

                if (locked)
                {
                    this.ReleaseFileLock();
                }

                this.FireOnFileClosed(fileName);
            }
        }

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            Info("Dispose() Enter");
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>The get rollover file name used when we move the file.</summary>
        /// <param name="fileName">The file Name.</param>
        /// <param name="fileNameRollover">The file name rollover type to use.</param>
        /// <param name="rolloverFilePath">The optional rollover file Path or empty string to use the current FileName as the path.</param>
        /// <returns>The <see cref="string" />.</returns>
        public string GetRolloverFileName(
            string fileName,
            FileNameRolloverType fileNameRollover,
            string rolloverFilePath = "")
        {
            var count = 0;

            var fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            var path = Path.GetDirectoryName(fileName);

            if (string.IsNullOrEmpty(rolloverFilePath) == false)
            {
                path = rolloverFilePath;
            }
            else if (string.IsNullOrEmpty(this.RollOverFilePath) == false)
            {
                path = this.RollOverFilePath;
            }

            if (string.IsNullOrEmpty(path))
            {
                path = Directory.GetCurrentDirectory();
            }

            if (fileNameRollover == FileNameRolloverType.None)
            {
                return Path.Combine(path, $"{fileNameOnly}{extension}");
            }

            if (fileNameRollover.HasFlag(FileNameRolloverType.TimeStampTicks))
            {
                return Path.Combine(path, $"{fileNameOnly}-{DateTime.Now.Ticks}{extension}");
            }

            if (fileNameRollover.HasFlag(FileNameRolloverType.ZipArchive))
            {
                this.InitZipEntries();
                do
                {
                    fileName = Path.Combine(path, $"{fileNameOnly}-{++count}{extension}");
                }
                while (this.zipFileEntries.Contains(Path.GetFileName(fileName)));
            }
            else
            {
                do
                {
                    if (fileNameRollover.HasFlag(FileNameRolloverType.Numerical))
                    {
                        fileName = Path.Combine(path, $"{fileNameOnly}-{++count}{extension}");
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(fileNameRollover), fileNameRollover, null);
                    }
                }
                while (File.Exists(fileName));
            }

            return fileName;
        }

        public string GetZipFileName(string sourceFileName = "")
        {
            if (string.IsNullOrEmpty(sourceFileName))
            {
                sourceFileName = this.FileName;
            }

            return ZipFileHelper.GetZipFileName(sourceFileName);
        }

        /// <summary>The File Open.</summary>
        /// <param name="fileName">The fileName.</param>
        /// <param name="csvClassMapType">The csv Class Map Type.</param>
        /// <param name="csvConfiguration">The csv Configuration.</param>
        /// <param name="fileMode">The file Mode.</param>
        /// <returns>The <see cref="CsvHelper.CsvWriter" />.</returns>
        public CsvFileHelper<T> Open(
            string fileName,
            Type csvClassMapType = null,
            Configuration csvConfiguration = null,
            FileMode fileMode = FileMode.OpenOrCreate)
        {
            var locked = false;
            try
            {
                if (!this.IsOpen)
                {
                    this.Records = 0;
                    var fileExists = File.Exists(fileName);
                    Debug.WriteLine("Open CSV fileName=" + fileName);

                    // create folder if needed
                    var directoryName = Path.GetDirectoryName(fileName);
                    if (string.IsNullOrEmpty(directoryName) == false && Directory.Exists(directoryName) == false)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    // set the default csv configuration if not provided
                    if (csvConfiguration == null)
                    {
                        csvConfiguration = new MyDefaultCsvConfiguration();
                    }

                    if (fileExists)
                    {
                        fileMode = FileMode.Open;
                    }

                    lock (this.openZipLock)
                    {
                        this.InitZipEntries();

                        this.CsvFileWriter = CsvFileHelperFactory.CreateCsvWriter(
                            fileName,
                            csvConfiguration,
                            fileMode,
                            FileAccess.ReadWrite,
                            csvClassMapType);
                        this.IsOpen = this.CsvWriter.Configuration != null && this.FileStream != null;

                        locked = this.WaitForFileLock(TimeSpan.FromSeconds(30));
                        if (!locked)
                        {
                            throw new SynchronizationLockException(
                                $"Open<{typeof(T)}> field to get mutex lock, File:{fileName}");
                        }
                    }

                    if (fileExists)
                    {
                        // set the initial record count, +1 if header is included
                        using (var sr = new StreamReader(
                            this.CsvFileWriter.FileStream,
                            Encoding.ASCII,
                            false,
                            10240,
                            true))
                        {
                            while (!sr.EndOfStream)
                            {
                                if (string.IsNullOrEmpty(sr.ReadLine()) == false)
                                {
                                    this.Records++;
                                }
                            }
                        }
                    }

                    // do we have a class map type ?
                    var configuration = this.CsvWriter.Configuration;

                    if (configuration != null)
                    {
                        var autoClassMap = configuration.AutoMap<T>();

                        if (this.CsvFileWriter.CsvClassMapType != null)
                        {
                            // Yes register it instead of the auto mapper
                            Info("Register CsvClassType {0}", this.CsvFileWriter.CsvClassMapType);
                            configuration.RegisterClassMap(this.CsvFileWriter.CsvClassMapType);
                        }
                        else
                        {
                            // no, create the default auto mapper
                            configuration.RegisterClassMap(autoClassMap);
                        }
                    }

                    // our option to ignore read only properties on writes
                    this.ExcludeReadOnlyProperties();

                    // Write header once on new file only
                    if (this.IsOpen && !fileExists)
                    {
                        this.FileStream.Seek(0, SeekOrigin.Begin);
                        this.WriteHeader();
                        this.CsvWriter.NextRecord(); // required CsvHelper 2.0 and up

                        // When you use WriteRecords, it will write the header for you if one hasn't already been written. Since you're writing the header by hand you'll need to call NextRecord to end writing to that row.
                    }
                    else
                    {
                        this.FileStream.Seek(0, SeekOrigin.End);
                        var cfg = this.CsvWriter.Configuration;
                        if (cfg != null && cfg.HasHeaderRecord)
                        {
                            // less one for the header row
                            this.Records--;
                        }
                    }

                    this.streamClosing = false;
                    this.FireOnFileOpened(fileName);
                }

                return this;
            }
            catch (Exception ex)
            {
                Logger.Error("CsvHelper<{0}>.Open Exception File:{1} {2}", typeof(T), this.FileName, ex.Message);
                throw;
            }
            finally
            {
                if (locked)
                {
                    this.ReleaseFileLock();
                }
            }
        }

        /// <summary>Read all records.</summary>
        /// <exception cref="IOException">File IO Exception</exception>
        /// <returns>The <see cref="List" />Collection of entities of type T.</returns>
        public List<T> ReadAll()
        {
            Debug.WriteLine("ReadAll() Enter");
            var locked = this.WaitForFileLock(TimeSpan.FromSeconds(30));
            long currentPosition = -1;

            try
            {
                if (locked)
                {
                    if (this.CsvWriter == null || !this.IsOpen)
                    {
                        throw new IOException("ReadAll(T item) failed CsvFileWriter not Opened");
                    }

                    this.FileStream.Flush(true);
                    if (this.FileStream.Position == 0 && this.Records > 0)
                    {
                        // ensure the file is written to disk before reading
                        Info("ReadAll() temp close and open file {0}", this.FileName);
                        this.Close();
                        this.Open();
                    }

                    currentPosition = this.FileStream.Position;
                    Info("ReadAll() Seek Beginning... {0}", this.FileName);
                    this.FileStream.Seek(0, SeekOrigin.Begin);
                    var streamReader = new StreamReader(this.FileStream, Encoding.ASCII, false, 10240, true);
                    var csvReader = new CsvReader(streamReader);

                    csvReader.Configuration.MissingFieldFound = (strings, i, arg3) =>
                        {
                            Info("Missing Field {0}", arg3.Field);
                        };
                    csvReader.Configuration.ReadingExceptionOccurred = exception =>
                        {
                            Warn("Reading CSV exception in file {0}, cause {1}", this.FileName, exception.Message);
                            throw exception;
                        };
                    var records = csvReader.GetRecords<T>().ToList();
                    this.Records = records.Count;
                    Info("ReadAll() Records = {0}", this.Records);
                    return records;
                }

                throw new SynchronizationLockException(
                    $"ReadAll<{typeof(T)}> field to get mutex lock, File:{this.FileName}");
            }
            catch (Exception ex)
            {
                Logger.Error("Exception in RealAll<{0}> File={1} {2}", typeof(T), this.FileName, ex.Message);
                throw;
            }
            finally
            {
                if (locked)
                {
                    if (currentPosition >= 0)
                    {
                        this.FileStream.Position = currentPosition;
                    }

                    this.ReleaseFileLock();
                }
            }
        }

        /// <summary>Register class map for Csv helper.</summary>
        /// <param name="mapClassType">The map Class Type.</param>
        /// <exception cref="NullReferenceException">Invalid Writer, see Open</exception>
        public void RegisterClassMap(Type mapClassType)
        {
            this.CsvFileWriter.CsvClassMapType = mapClassType;

            // Legacy Nuget CsvHelper this.CsvFileWriter.ClassMapType = mapClassType;
            if (this.CsvWriter != null)
            {
                try
                {
                    Debug.WriteLine("RegisterClassMap " + mapClassType);

                    // If the file is already open close if empty to re-write the new header for the changed class map
                    var reopen = false;
                    if (this.IsOpen && this.FileLength == 0)
                    {
                        Logger.Warn(
                            "Closing File{0} to register new class map type {1} and update header",
                            this.FileName,
                            typeof(T));
                        this.Close();
                        reopen = true;
                        if (this.Records == 0)
                        {
                            File.Delete(this.FileName);
                        }
                    }

                    this.CsvWriter?.Configuration.RegisterClassMap(mapClassType);

                    // reopen and write header
                    if (reopen)
                    {
                        this.Open();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("RegisterClassMap Exception {0}", ex.Message);
                    throw;
                }
            }
            else
            {
                throw new NullReferenceException("Open must be called to first create the writter");
            }
        }

        /// <summary>Write item to the CsvWriter.</summary>
        /// <param name="item">The item.</param>
        public void Write(T item)
        {
            if (this.WaitForFileLock(TimeSpan.FromSeconds(10)))
            {
                if (this.CsvWriter == null || !this.IsOpen)
                {
                    throw new IOException(string.Format("Write(T {0}) failed csvwriter not Opened", typeof(T)));
                }

                try
                {
                    this.CsvWriter.WriteRecord(item);
                    this.CsvWriter.NextRecord();
                    this.Records++;

                    // check for the file size and roll over if true
                    if (this.FileNameRolloverType != FileNameRolloverType.None)
                    {
                        this.CheckForFileRollover(this.FileNameRolloverType);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Write<{0}> Exception {1}", typeof(T), ex.Message);
                }
                finally
                {
                    this.ReleaseFileLock();
                }
            }
            else
            {
                if (this.CsvWriter == null || !this.IsOpen)
                {
                    throw new IOException(string.Format("Write(T {0}) failed csvwriter not Opened", typeof(T)));
                }

                throw new SynchronizationLockException(
                    $"Write<{typeof(T)}> field to get mutex lock, File:{this.FileName}");
            }
        }

        /// <summary>Write all records.</summary>
        /// <param name="records">The records collection.</param>
        /// <exception cref="IOException">File Cannot be locked for use.</exception>
        public void WriteAll(ICollection<T> records)
        {
            Info("WriteAll<{0}> Enter", typeof(T));
            var locked = this.WaitForFileLock(TimeSpan.FromSeconds(10));
            try
            {
                if (locked)
                {
                    if (this.CsvWriter == null || !this.IsOpen)
                    {
                        throw new IOException("WriteAll(T item) failed csvwriter not Opened");
                    }

                    if (this.FileNameRolloverType != FileNameRolloverType.None)
                    {
                        foreach (var record in records)
                        {
                            this.CsvWriter.WriteRecord(record);
                            this.CsvWriter.NextRecord();
                            this.Records++;

                            // check for the file size and roll over if true
                            this.CheckForFileRollover(this.FileNameRolloverType);
                        }
                    }
                    else
                    {
                        this.CsvWriter.WriteRecords(records);
                        this.Records += records.Count;
                    }

                    Info("WriteAll<{0}> this.Records={1}", typeof(T), this.Records);
                }
                else
                {
                    throw new SynchronizationLockException(
                        $"WriteAll<{typeof(T)}> field to get mutex lock, File:{this.FileName}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("WriteAll {0}", ex.Message);
                throw;
            }
            finally
            {
                if (locked)
                {
                    this.ReleaseFileLock();
                    Info("WriteAll<{0}> Exit", typeof(T));
                }
            }
        }

        /// <summary>Write all records.</summary>
        /// <param name="records">The records collection.</param>
        /// <exception cref="IOException">File Cannot be locked for use.</exception>
        public void WriteAllAsync(ICollection<T> records)
        {
            if (this.CsvWriter == null || !this.IsOpen)
            {
                throw new IOException("WriteAllAsync(T item) failed CsvFileWriter not Opened");
            }

            var locked = this.WaitForFileLock(TimeSpan.FromSeconds(10));
            try
            {
                if (locked)
                {
                    foreach (var rec in records)
                    {
                        this.writeQue.Enqueue(rec);
                    }

                    this.recordAddedToQueEvent.Set();
                }
                else
                {
                    throw new SynchronizationLockException(
                        $"WriteAllAsync<{typeof(T)}> field to get mutex lock, File:{this.FileName}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("WriteAllAsync {0}", ex.Message);
                throw;
            }
            finally
            {
                if (locked)
                {
                    this.ReleaseFileLock();
                }
            }
        }

        /// <summary>Async Write item to the CsvWriter.</summary>
        /// <param name="item">The item.</param>
        public void WriteAsync(T item)
        {            
            if (this.writeQue != null)
            {
                this.writeQue.Enqueue(item);
                this.recordAddedToQueEvent.Set();
            }
        }

        /// <summary>The info.</summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected static void Info(string format, params object[] args)
        {
            var s = string.Format(format, args);
            Logger.Info(s);
            Debug.WriteLine(s);
        }

        /// <summary>The warn.</summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected static void Warn(string format, params object[] args)
        {
            Logger.Warn(format, args);
            Debug.WriteLine(format, args);
        }

        // Protected implementation of Dispose pattern.

        /// <summary>The dispose.</summary>
        /// <param name="disposing">The object is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            Info("Dispose for type File={0}", this.FileName);
            try
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposing = disposing;

                if (disposing)
                {
                    // Free any other managed objects here. Allow the background to empty the que and signal when exited
                    this.killTaskEvent?.Set();
                    if (this.WriterTaskStarted)
                    {
                        var signaled = this.doWritesTaskExitedEvent?.WaitOne(TimeSpan.FromMinutes(5));
                        if (signaled.HasValue && signaled.Value)
                        {
                            Info("Dispose() doWritesTaskExitedEvent = true");
                        }
                    }
                }
            }
            finally
            {
                // Free any un-managed objects here.
                this.IsOpen = false;
                this.disposed = true;
            }
        }

        /// <summary>The fire on file created.</summary>
        /// <param name="fileName">The file name.</param>
        protected virtual void FireOnFileMoved(string fileName)
        {
            Info("Fire event New Log file Moved {0}", fileName);
            this.OnFileMoved?.Invoke(this, fileName);
        }

        /// <summary>Get zip file name entries.</summary>
        /// <param name="zipFileName">The zip file name.</param>
        /// <returns>The <see cref="List" />.</returns>
        private static List<string> GetZipFileEntries(string zipFileName)
        {
            using (var zip = ZipFile.Open(zipFileName, ZipArchiveMode.Read))
            {
                return zip.Entries.Select(m => m.Name).ToList();
            }
        }

        /// <summary>Test if the file rollover needs to occur</summary>
        /// <param name="fileNameRolloverType">File roll over type</param>
        private void CheckForFileRollover(FileNameRolloverType fileNameRolloverType)
        {
            if (fileNameRolloverType == FileNameRolloverType.None
                || fileNameRolloverType == FileNameRolloverType.ZipArchive)
            {
                return;
            }

            // If MaxRecords is Non zero and we have meet this condition OR
            // the MaxFile size has been exceeded then Roll Over 
            var maxRecords = this.IsMaxRecords();
            var maxFileSize = this.IsMaxFileSize();

            if (maxFileSize || maxRecords)
            {
                try
                {
                    if (maxRecords)
                    {
                        Info("!Max Records {0} reached, current Count {1}, Rolling over as {2}", this.MaxRecords, this.Records, fileNameRolloverType);
                    }
                    else if (maxFileSize)
                    {
                        Info("!Max File Size {0} reached, Rolling over as {1}", this.FileLength, fileNameRolloverType);
                    }

                    var zipEntry = fileNameRolloverType.HasFlag(FileNameRolloverType.ZipArchive);
                    var destinationFileName = this.GetRolloverFileName(this.FileName, fileNameRolloverType);
                    Info("CheckForFileRollover() Moving file {0} => {1}, Records={2}", this.FileName, destinationFileName, this.Records);
                    this.Move(destinationFileName, zipEntry);
                }
                catch (Exception exception)
                {
                    Logger.Error("Move Exception {0}", exception.Message);
                }
            }
        }

        /// <summary>The do writes background task.</summary>
        private void DoWrites()
        {
            try
            {
                Info("DoWrites() background writer Started for type {0}", typeof(T));
                var handles = new WaitHandle[] { this.killTaskEvent, this.recordAddedToQueEvent };
                Thread.CurrentThread.Name = "CvsFileWritesTask_" + typeof(T);
                Thread.CurrentThread.IsBackground = true;
                this.WriterTaskStarted = true;
                while (true)
                {
                    Debug.WriteLine("DoWrites Task idle " + typeof(T));
                    var eventIndex = WaitHandle.WaitAny(handles);
                    var exitThread = eventIndex == 0;
                    var locked = this.WaitForFileLock(TimeSpan.FromSeconds(10));
                    Debug.WriteLine("DoWrites Task eventIndex=" + eventIndex + " locked=" + locked);
                    if (exitThread)
                    {
                        Info("Empty Que to Exit....");
                    }

                    try
                    {
                        if (locked && this.writeQue != null)
                        {
                            Info(
                                "DoWrites Task empty the que... {0}, File {1}, eventIndex = {2}",
                                typeof(T),
                                this.FileName,
                                eventIndex);

                            this.WriteRecordsFromQue();
                            Info("WriteRecordsFromQue() Exited. FileSize = {0}, Records = {1}", this.FileLength, this.Records);
                        }
                        else
                        {
                            Debug.WriteLine("!Lock Not Acquired");
                        }

                        if (exitThread)
                        {
                            // recheck the que for any last writes to empty it
                            var recordsInQue = this.writeQue?.Count;
                            Info("Records left in Que = {0}", recordsInQue);
                            this.WriteRecordsFromQue();
                        }
                    }
                    finally
                    {
                        if (locked)
                        {
                            this.ReleaseFileLock();
                        }
                    }

                    if (exitThread)
                    {
                        Info("Background writer thread killed, exiting...");
                        break;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Info("Thread Aborting normally");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Error("DoWrites Exception {0}", ex.Message);
            }
            finally
            {
                Debug.WriteLine("DoWrites Task Exited");
                this.Close();
                if (this.Records == 0 && !string.IsNullOrEmpty(this.FileName))
                {
                    Info("Deleting Empty CSV FIle {0}", this.FileName);
                    File.Delete(this.FileName);
                }
                else if (this.FileNameRolloverType == FileNameRolloverType.ZipArchive)
                {
                    var destinationFileName = this.GetRolloverFileName(this.FileName, FileNameRolloverType.ZipArchive);
                    this.Move(destinationFileName, true);
                }

                // signal all done and exit
                this.WriterTaskStarted = false;
                this.doWritesTaskExitedEvent.Set();
            }
        }

        private void WriteRecordsFromQue()
        {
            T item;
            if (this.writeQue == null)
            {
                return;
            }

            while (this.writeQue.TryDequeue(out item))
            {
                if (this.CsvWriter != null && this.IsOpen)
                {
                    this.CsvWriter.WriteRecord(item);
                    this.CsvWriter.NextRecord();
                    this.Records++;
                    Debug.WriteLine("WriteRecordsFromQue " + this.Records);
                }

                // check for the file size and roll over if true
                if (this.FileNameRolloverType != FileNameRolloverType.None)
                {
                    this.CheckForFileRollover(this.FileNameRolloverType);
                }
            }

            this.FileStream?.Flush(true);
        }

        /// <summary>Exclude Read only class properties from being written to the csv stream</summary>
        private void ExcludeReadOnlyProperties()
        {
            if (this.CurrentClassMap == null)
            {
                return;
            }

            var memberMaps = this.CurrentClassMap.MemberMaps;
            foreach (var map in memberMaps)
            {
                var name = map.Data.Member.Name;
                var propertyInfo = typeof(T).GetProperty(name);

                // If Prop is read only then ignore it
                if (propertyInfo?.CanWrite == false)
                {
                    map.Ignore(true);
                }
            }
        }

        private void FireOnFileClosed(string fileName)
        {
            Info("Fire event file Closed File:{0}", fileName);
            this.OnFileClosed?.Invoke(this, fileName);
        }

        private void FireOnFileOpened(string fileName)
        {
            Info("Fire event file Opened File:{0}", fileName);
            this.OnFileOpened?.Invoke(this, fileName);
        }

        private void InitZipEntries()
        {
            if (!this.FileNameRolloverType.HasFlag(FileNameRolloverType.ZipArchive))
            {
                return;
            }

            var zipFile = this.GetZipFileName(this.FileName);
            if (!string.IsNullOrEmpty(zipFile))
            {
                if (File.Exists(zipFile) && this.zipFileEntries.Count == 0)
                {
                    this.zipFileEntries = GetZipFileEntries(zipFile);
                }
            }
            else
            {
                this.zipFileEntries = new List<string>();
            }
        }

        /// <summary>Test if Max file size has been reached</summary>
        /// <returns>The <see cref="bool" />.</returns>
        private bool IsMaxFileSize()
        {
            if (this.MaxFileSize <= 0 || this.FileStream == null)
            {
                return false;
            }

            Debug.WriteLine("IsFileSizeToLarge = " + this.FileLength);
            return this.FileLength > this.MaxFileSize;
        }

        /// <summary>Test if Max records has been reached</summary>
        /// <returns>The <see cref="bool" />.</returns>
        private bool IsMaxRecords()
        {
            return this.MaxRecords > 0 && this.Records >= this.MaxRecords;
        }

        /// <summary>Close and move the file.</summary>
        /// <param name="destinationFileName">The destination.</param>
        /// <exception cref="IOException">File IO Exception</exception>
        private void Move(string destinationFileName, bool zipToArchive = false)
        {
            if (string.IsNullOrEmpty(destinationFileName))
            {
                throw new ArgumentException("Invalid destination");
            }

            // delete the destination file
            var path = Path.GetDirectoryName(destinationFileName);
            if (string.IsNullOrEmpty(path))
            {
                path = Directory.GetCurrentDirectory();
            }

            if (!Path.IsPathRooted(destinationFileName))
            {
                destinationFileName = Path.Combine(path, Path.GetFileName(destinationFileName));
            }

            if (File.Exists(destinationFileName))
            {
                Info("Delete {0}", destinationFileName);
                File.Delete(destinationFileName);
            }

            var locked = this.WaitForFileLock(TimeSpan.FromSeconds(60));
            try
            {
                if (locked)
                {
                    if (string.IsNullOrEmpty(path) == false)
                    {
                        if (Directory.Exists(path) == false)
                        {
                            Info("Creating folder {0}", path);
                            Directory.CreateDirectory(path);
                        }
                    }

                    // Close the existing file and open a new one.
                    Info("Closing file {0} to move => {1}", this.FileName, destinationFileName);
                    this.Close();
                    Info("Move File {0} -> {1} Enter", this.FileName, destinationFileName);
                    File.Move(this.FileName, destinationFileName);
                    Info("File Moved to {0}", destinationFileName);

                    if (zipToArchive)
                    {
                        var zipFileName = this.GetZipFileName(this.FileName);
                        var mode = File.Exists(zipFileName) ? ZipArchiveMode.Update : ZipArchiveMode.Create;
                        using (var zipFile = ZipFile.Open(zipFileName, mode))
                        {
                            // find unused zip entry name
                            var zipFileEntry = Path.GetFileName(destinationFileName);
                            this.zipFileEntries.Add(zipFileEntry);
                            zipFile.CreateEntryFromFile(destinationFileName, zipFileEntry);
                        }

                        File.Delete(destinationFileName);
                        destinationFileName = zipFileName;
                    }

                    this.FireOnFileMoved(destinationFileName);

                    // Disposing may be set true but we could have qued data still to write
                    var hasQueuedRecords = this.writeQue.Any();

                    if (!this.disposing || hasQueuedRecords)
                    {
                        this.Open();
                    }
                }
                else
                {
                    throw new IOException("Cannot move File " + this.FileName + " to " + destinationFileName);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception moving file {0}", ex.Message);
                throw;
            }
            finally
            {
                if (locked)
                {
                    this.ReleaseFileLock();
                }
            }
        }

        /// <summary>Open the file using the current default configuration.</summary>
        private void Open()
        {
            this.Open(this.FileName);
        }

        /// <summary>Release the file lock.</summary>
        private void ReleaseFileLock()
        {
            try
            {
                this.CsvFileWriter?.ReleaseFileLock();
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>Create the background writer task and start it.</summary>
        private void StartWriterTask()
        {
            Info("StartWriterTask() Enter");
            if (this.writerTask == null)
            {
                Info("Creating Writer Task Thread: " + Thread.CurrentThread.ManagedThreadId);
                this.writerTask = new Task(() => { this.DoWrites(); });
                this.writerTask.Start();
                Thread.Sleep(0);
            }
        }

        /// <summary>Wait for file lock</summary>
        /// <param name="timeSpan">TimeSpan argument</param>
        /// <returns>The <see cref="bool" />True if lock was acquired.</returns>
        private bool WaitForFileLock(TimeSpan timeSpan)
        {
            return this.WaitForFileLock((int)timeSpan.TotalMilliseconds);
        }

        /// <summary>Wait for file lock</summary>
        /// <param name="millisecondsTimeout">Timeout in milliseconds</param>
        /// <returns>The <see cref="bool" />.</returns>
        private bool WaitForFileLock(int millisecondsTimeout = Timeout.Infinite)
        {
            try
            {
                return this.CsvFileWriter != null && this.CsvFileWriter.WaitForFileLock(millisecondsTimeout);
            }
            catch
            {
                // Ignore
            }

            return false;
        }

        /// <summary>
        ///     Write the Csv file header
        /// </summary>
        private void WriteHeader()
        {
            Info("WriteHeader for type {0}", typeof(T));
            this.CsvWriter.WriteHeader<T>();
        }
    }
}
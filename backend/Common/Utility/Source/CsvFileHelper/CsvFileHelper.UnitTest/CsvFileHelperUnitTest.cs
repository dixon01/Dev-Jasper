namespace Luminator.CsvFileHelper.UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CsvHelper;

    using Faker;

    using Luminator.PresentationPlayLogging.Core.Models;
    using Luminator.Utility.CsvFileHelper;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>The CSV file helper unit test.</summary>
    [TestClass]
    public class CsvFileHelperUnitTest
    {
        private static readonly string TestFolder = @"C:\temp";

        private static readonly string CommonTestFile = Path.Combine(TestFolder, "CsvFileHelperUnitTest.csv");

        /// <summary>The class cleanup.</summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        /// <summary>The class int.</summary>
        /// <param name="context">The context.</param>
        [ClassInitialize]
        public static void ClassInt(TestContext context)
        {
            if (!Directory.Exists(TestFolder))
            {
                Directory.CreateDirectory(TestFolder);
            }

            Console.WriteLine("Test Folder = " + TestFolder);
            File.Delete(CommonTestFile);
        }

        /// <summary>The csv class map test.</summary>
        [TestMethod]
        public void CsvClassMapTest()
        {
            var testFile = Path.Combine(TestFolder, "CsvClassMapTest.csv");
            DeleteTestFiles(testFile);
        }

        /// <summary>The flush test.</summary>
        [TestMethod]
        public void FlushTest()
        {
            var testFile = Path.Combine(TestFolder, "FlushTest.csv");
            File.Delete(testFile);

            using (var log = new CsvFileHelper<UnitTestModel>(testFile))
            {
                Assert.IsNotNull(log);
                log.WriteAsync(new UnitTestModel());
                log.WriteAsync(new UnitTestModel());
                log.WriteAsync(new UnitTestModel());
            }

            Assert.IsTrue(File.Exists(testFile), "File Not Found!");

            Debug.WriteLine("FlushTest Reading...");
            using (var log = new CsvFileHelper<UnitTestModel>(testFile))
            {
                Assert.IsNotNull(log);
                var records = log.ReadAll();
                Assert.AreEqual(3, records.Count());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetZipFileEntriesBogusFile()
        {
            var entries = ZipFileHelper.GetFileEntries("BogusFile.zip");
            Assert.IsNotNull(entries);
        }

        [TestMethod]
        public void GetZipFileName()
        {
            var testFile = Path.Combine(TestFolder, "UnitTestGetZipFileName.csv");
            var zipFile = Path.Combine(
                Path.GetDirectoryName(testFile),
                Path.GetFileNameWithoutExtension(testFile) + ".zip");
            Assert.AreEqual(zipFile, ZipFileHelper.GetZipFileName(testFile));
        }

        /// <summary>The my csv logger_ using statement.</summary>
        [TestMethod]
        public void MyCsvLogger_UsingStatement()
        {
            var testFile = Path.Combine(TestFolder, "CsvFileHelperUsingStatement.csv");
            File.Delete(testFile);

            using (var log = new CsvFileHelper<TestModel2>())
            {
                Assert.IsNotNull(log);
                log.Open(testFile);
                var model = new TestModel2();
                log.WriteAsync(model);
            }

            Assert.IsTrue(FileNotEmpty(testFile));
            var records = CsvFileHelper<TestModel2>.GetCsvFileRecordCount(testFile);
            Assert.AreEqual(1, records); // include header in count
        }

        /// <summary>The open close unit test model 2 test.</summary>
        [TestMethod]
        public void OpenCloseUnitTestModel2Test()
        {
            var testFile = Path.Combine(TestFolder, "OpenCloseUnitTestModel2Test.csv");
            File.Delete(testFile);
            Console.WriteLine("OpenCloseUnitTestModel2Test Start testFile = " + testFile);
            var log = new CsvFileHelper<UnitTestModel2>(testFile);
            Assert.IsNotNull(log);
            Assert.IsTrue(log.IsOpen);
            log.Write(new UnitTestModel2());
            log.Write(new UnitTestModel2());
            log.Write(new UnitTestModel2());
            log.Write(new UnitTestModel2());
            log.Close();
            Assert.IsFalse(log.IsOpen);
            Assert.IsTrue(File.Exists(testFile));
            Assert.AreEqual(4, log.Records);

            var log2 = new CsvFileHelper<UnitTestModel2>(testFile);
            Assert.IsNotNull(log2);
            Assert.IsTrue(log2.IsOpen);
            log2.Write(new UnitTestModel2 { First = "Test 5" });
            log2.Close();
            Assert.IsTrue(File.Exists(testFile));
            Assert.IsFalse(log.IsOpen);
            Assert.AreEqual(5, log2.Records);

            var log3 = new CsvFileHelper<UnitTestModel2>(testFile);
            Assert.IsNotNull(log3);
            Assert.IsTrue(log3.IsOpen);
            var records = log3.ReadAll();
            Assert.AreEqual(5, records.Count);
            log3.Close();
            Assert.IsTrue(File.Exists(testFile));
            Assert.IsFalse(log3.IsOpen);
        }

        [TestMethod]
        [DeploymentItem("ReadAllFileTest.csv")]
        public void ReadAllFileTest()
        {
            const int MaxRecords = 1000;
            var testFile = "ReadAllFileTest.csv";
            Console.WriteLine("ReadAllFileTest Start testFile = " + testFile);
            Assert.IsTrue(File.Exists(testFile), "File Not found " + testFile);

            Console.WriteLine("Open to read test");
            Assert.IsTrue(File.Exists(testFile));
            var count = 0;
            using (var csvFileHelper = new CsvFileHelper<TestModel1>(testFile))
            {
                var records = csvFileHelper.ReadAll();
                count = records.Count();
                Console.WriteLine("ReadAll = " + count);
                Debug.WriteLine("-------------- Dispose ------------");
            }

            Assert.AreEqual(MaxRecords, count, "Records do not match");
        }

        [DeploymentItem("InfotransitInfoUnitTestV1.csv")]
        [TestMethod]
        public void ReadInfotransitInfoUnitTestCsvFileV1()
        {
            const string TestFile = "InfotransitInfoUnitTestV1.csv";
            Assert.IsTrue(File.Exists(TestFile), "Test file not found");

            Console.WriteLine("ReadInfotransitInfoUnitTestCsvFileV1 Start testFile = " + TestFile);
            using (var log = new CsvFileHelper<InfotransitPresentationInfo>(TestFile))
            {
                Assert.IsTrue(log.IsOpen, "File Not opened");
                var records = log.ReadAll();
                Assert.IsNotNull(records);
                Assert.IsTrue(records.Count > 0);
                Debug.WriteLine("-------------- Dispose ------------");
            }
        }

        [TestMethod]
        public void ReadWriteInfotransitPresentationInfoFile()
        {
            var testFile = Path.Combine(TestFolder, "InfotransitInfoUnitTest.csv");
            File.Delete(testFile);

            Console.WriteLine("ReadWriteInforantistInfoFile Start testFile = " + testFile);

            using (var log = new CsvFileHelper<InfotransitPresentationInfo>(testFile))
            {
                Assert.IsTrue(log.IsOpen, "File Not opened");
                var model = new InfotransitPresentationInfo
                                {
                                    FileName = "test.png",
                                    VehicleId = "BUS1",
                                    Route = "R1",
                                    PlayStarted = DateTime.Now,
                                    PlayStopped = DateTime.Now.AddSeconds(10),
                                    IsPlayInterrupted = false,
                                    PlayedDuration = 10,
                                    Duration = 10,
                                    ResourceId = "RES123",
                                    StartedLatitude = "33.019844",
                                    StartedLongitude = "-96.698883",
                                    StoppedLatitude = "33.019847",
                                    StoppedLongitude = "-96.698887",
                                    Trip = "T1"
                                };
                log.WriteAsync(model);
                Debug.WriteLine("-------------- Dispose ------------");
            }

            Assert.IsTrue(File.Exists(testFile), "Test File Missing");

            using (var log = new CsvFileHelper<InfotransitPresentationInfo>(testFile))
            {
                var records = log.ReadAll();
                Assert.AreEqual(1, records.Count);
            }
        }

        /// <summary>The test model 3 test_ expected io exception.</summary>
        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void TestModel3TestExpectedIoException()
        {
            using (var log = new CsvFileHelper<TestModel3>())
            {
                Assert.IsNotNull(log);
                Assert.IsFalse(log.IsOpen);
                log.Write(new TestModel3());
            }
        }

        /// <summary>The write all test.</summary>
        [TestMethod]
        public void WriteAllTest()
        {
            var testFile = Path.Combine(TestFolder, "WriteAllTest.csv");
            Console.WriteLine("WriteAllTest Start testFile = " + testFile);
            File.Delete(testFile);
            const int MaxRecords = 1000;
            using (var csvFile = new CsvFileHelper<TestModel1>(testFile))
            {
                var records = new List<TestModel1>();
                for (var i = 0; i < MaxRecords; i++)
                {
                    var model = new TestModel1 { Id = i };
                    records.Add(model);
                }

                csvFile.WriteAll(records);
                Console.WriteLine("records = " + records);
                Assert.AreEqual(records.Count, csvFile.Records, "Record count incorrect");
                Debug.WriteLine("-------------- Dispose ------------");
            }

            Assert.IsTrue(File.Exists(testFile));
            var lines = File.ReadLines(testFile).ToList();
            var headerString = lines.First();
            Assert.IsTrue(
                headerString.Contains("IgnoreMeValue") == false); // excluded read only field expect to be missing
        }

        /// <summary>The write infotransit presentation info to csv file.</summary>
        [TestMethod]
        public void WriteBigInfotransitPresentationInfoToCsvFile()
        {
            var testFile = Path.Combine(TestFolder, "InfotransitPresentationInfo.csv");
            DeleteTestFiles(testFile);

            const int MaxRecords = 8640; // full day at ten second interval
            const int MaxThreads = 1;
            var countdownEvent = new CountdownEvent(MaxThreads);

            using (var csvFile = new CsvFileHelper<InfotransitPresentationInfo>(testFile))
            {
                Assert.IsTrue(csvFile.IsOpen);

                csvFile.OnFileMoved += (sender, s) =>
                    {
                        Debug.WriteLine("File Created " + s);
                    };

                for (var i = 0; i < MaxThreads; i++)
                {
                    Task.Run(
                        () =>
                            {
                                var items = MaxRecords;
                                for (var count = 0; count < items; count++)
                                {
                                    var model = new InfotransitPresentationInfo
                                                    {
                                                        PlayStarted = DateTime.Now,
                                                        PlayStopped =
                                                            DateTime.Now.AddSeconds(
                                                                RandomNumber.Next(1, 30)),
                                                        Route = "TESTROUTE",
                                                        ResourceId =
                                                            "ID_" + RandomNumber.Next(
                                                                1,
                                                                100),
                                                        FileName = Name.First() + ".jpg",
                                                        VehicleId = "VEHICLE123",
                                                        IsPlayInterrupted = false,
                                                        Duration = 30,
                                                        StartedLatitude = "33.019844",
                                                        StartedLongitude = "-96.698883",
                                                        StoppedLatitude = "33.019847",
                                                        StoppedLongitude = "-96.698887"
                                                    };
                                    csvFile.WriteAsync(model);
                                }

                                countdownEvent.Signal(1);
                            });
                }

                var signaled = countdownEvent.Wait(TimeSpan.FromMinutes(1));
                Assert.IsTrue(signaled);
            }
        }

        /// <summary>The write csv wtih auto class mapping.</summary>
        [TestMethod]
        public void WriteCsvWtihAutoClassMapping()
        {
            /*
            Output with Auto Mapping which has the header and data correct.
          Description,Created,First,Last,Comment,Enabled,Started,Stopped,PlayStopped
            Description=Dolor quod qui ut tempora minima tempore.,9/11/2017 10:45:02 AM,First=Helene,Last=Hamill,Comment=Et tempora quos voluptatibus.,False,3/4/2016 10:45:02 AM,9/3/2023 11:46:31 PM,
            Description=Commodi eveniet nihil quo excepturi earum deserunt facere voluptate.,9/11/2017 10:45:02 AM,First=Elian,Last=Witting,Comment=Rerum sed omnis dolor odit nulla dolores et autem.,False,8/10/2017 10:45:02 AM,3/25/2019 6:35:51 AM,
            Description=Dolorem alias numquam non.,9/11/2017 10:45:02 AM,First=Deondre,Last=Will,Comment=Minima a aspernatur et sed.,False,9/1/2015 10:45:02 AM,10/14/2021 6:28:36 AM,
            Description=Minima ipsam maxime reprehenderit veniam.,9/11/2017 10:45:02 AM,First=Geo,Last=McKenzie,Comment=Facilis fugit rerum numquam voluptatem consequatur mollitia sed consequatur.,False,11/3/2015 10:45:02 AM,8/12/2081 10:48:10 AM,

            */
            var testFile = Path.Combine(TestFolder, "WriteCsvWtihAutoClassMapping.csv");
            File.Delete(testFile);
            Console.WriteLine("WriteCsvWtihAutoClassMapping Start testFile = " + testFile);

            using (var log = new CsvFileHelper<UnitTestModel2>(testFile, typeof(UnitTestModel2CsvClassMap)))
            {
                var classMap = log.CurrentClassMap;
                Assert.IsNotNull(classMap);

                Assert.IsNotNull(log);
                Assert.IsTrue(log.IsOpen);
                log.Write(new UnitTestModel2());
                log.Write(new UnitTestModel2());
                log.Write(new UnitTestModel2());
                log.Write(new UnitTestModel2());

                log.Close();
                Assert.IsFalse(log.IsOpen);
                Assert.IsTrue(File.Exists(testFile));
                Assert.AreEqual(4, log.Records);
                Debug.WriteLine("-------------- Dispose ------------");
            }

            var allLines = File.ReadAllLines(testFile);
            Assert.AreEqual(5, allLines.Count());

            using (var stream = new StreamReader(File.OpenRead(testFile)))
            {
                var csvReader = new CsvReader(stream);
                var records = csvReader.GetRecords<UnitTestModel2>().ToList();
                Assert.AreEqual(4, records.Count);
            }

            using (var log = new CsvFileHelper<UnitTestModel2>(testFile, typeof(UnitTestModel2CsvClassMap)))
            {
                try
                {
                    var records = log.ReadAll();
                    Assert.AreEqual(4, records.Count);
                    foreach (var r in records)
                    {
                        Assert.IsTrue(r.Comment.StartsWith("Comment"));
                        Assert.IsTrue(r.First.StartsWith("First"));
                        Assert.IsTrue(r.Last.StartsWith("Last"));
                        Assert.IsTrue(r.Description.StartsWith("Description"));
                        Assert.AreEqual(false, r.Enabled);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Assert.Fail(ex.Message);
                }
            }
        }

        /// <summary>The write csv wtih class mapping.</summary>
        [TestMethod]
        public void WriteCsvWtihClassMapping()
        {
            /*
              * The Auto mapping works as expected. See test WriteCsvWtihAutoClassMapping
                Description,Created,First,Last,Comment,Enabled,Started,Stopped,PlayStopped
                Description=Dolor quod qui ut tempora minima tempore.,9/11/2017 10:45:02 AM,First=Helene,Last=Hamill,Comment=Et tempora quos voluptatibus.,False,3/4/2016 10:45:02 AM,9/3/2023 11:46:31 PM,
                Description=Commodi eveniet nihil quo excepturi earum deserunt facere voluptate.,9/11/2017 10:45:02 AM,First=Elian,Last=Witting,Comment=Rerum sed omnis dolor odit nulla dolores et autem.,False,8/10/2017 10:45:02 AM,3/25/2019 6:35:51 AM,
                Description=Dolorem alias numquam non.,9/11/2017 10:45:02 AM,First=Deondre,Last=Will,Comment=Minima a aspernatur et sed.,False,9/1/2015 10:45:02 AM,10/14/2021 6:28:36 AM,
                Description=Minima ipsam maxime reprehenderit veniam.,9/11/2017 10:45:02 AM,First=Geo,Last=McKenzie,Comment=Facilis fugit rerum numquam voluptatem consequatur mollitia sed consequatur.,False,11/3/2015 10:45:02 AM,8/12/2081 10:48:10 AM,
            */
            var testFile = Path.Combine(TestFolder, "WriteCsvWtihClassMapping.csv");
            File.Delete(testFile);
            Console.WriteLine("WriteCsvWtihClassMapping Start testFile = " + testFile);
            using (var log = new CsvFileHelper<UnitTestModel2>(testFile))
            {
                log.RegisterClassMap(typeof(UnitTestModel2CsvClassMap));
                var classMap = log.CurrentClassMap;
                Assert.IsNotNull(classMap);

                Assert.IsNotNull(log);
                Assert.IsTrue(log.IsOpen);
                log.Write(new UnitTestModel2());
                log.Write(new UnitTestModel2());
                log.Write(new UnitTestModel2());
                log.Write(new UnitTestModel2());
                log.Close();
                Assert.IsFalse(log.IsOpen);
                Assert.IsTrue(File.Exists(testFile));
                Assert.AreEqual(4, log.Records);
                Debug.WriteLine("-------------- Dispose ------------");
            }

            using (var log = new CsvFileHelper<UnitTestModel2>(testFile))
            {
                try
                {
                    log.RegisterClassMap(typeof(UnitTestModel2CsvClassMap));
                    var records = log.ReadAll();
                    Assert.AreEqual(4, records.Count);

                    foreach (var r in records)
                    {
                        Assert.IsTrue(r.Comment.StartsWith("Comment"));
                        Assert.IsTrue(r.First.StartsWith("First"));
                        Assert.IsTrue(r.Last.StartsWith("Last"));
                        Assert.IsTrue(r.Description.StartsWith("Description"));
                        Assert.AreEqual(false, r.Enabled);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Assert.Fail(ex.Message);
                }

                Debug.WriteLine("-------------- Dispose ------------");
            }
        }

        /// <summary>The write multiple on threads.</summary>
        [TestMethod]
        public void WriteMultipleOnThreads1000Records()
        {
            var testFile = Path.Combine(TestFolder, "WriteMultipleOnThreads1000Records.csv");
            Console.WriteLine("WriteMultipleOnThreads Start testFile = " + testFile);
            File.Delete(testFile);
            const int MaxThreads = 100;

            var countdownEvent = new CountdownEvent(MaxThreads);
            var task = new List<Task>();

            using (var csvFileHelper = new CsvFileHelper<TestModel1>(testFile))
            {
                Assert.AreEqual(FileNameRolloverType.None, csvFileHelper.FileNameRolloverType);
                Assert.IsTrue(csvFileHelper.IsOpen, "File Not Opened");

                for (var i = 0; i < MaxThreads; i++)
                {
                    var t = new Task(
                        () =>
                            {
                                var items = 10;
                                for (var count = 0; count < items; count++)
                                {
                                    var model = new TestModel1 { Id = count, Started = DateTime.Now };
                                    if (csvFileHelper != null)
                                    {
                                        csvFileHelper.WriteAsync(model);
                                    }
                                }

                                countdownEvent.Signal(1);
                            });
                    task.Add(t);
                }

                task.ForEach(m => m.Start());
                var signaled = countdownEvent.Wait(Debugger.IsAttached ? -1 : 30000);
                Assert.IsTrue(signaled, "Not Signaled");
            }

            Assert.IsTrue(File.Exists(testFile));
            Assert.IsTrue(FileSizeNonZero(testFile));
            var recordCount = GetRecordCount<TestModel1>(testFile);
            Debug.WriteLine("Final file Records Cont = " + recordCount);
            Assert.AreNotEqual(0, recordCount, "Record Count in file != 0");
            Assert.AreEqual(1000, recordCount);
        }

        /// <summary>The write with max file size.</summary>
        [TestMethod]
        public void WriteMultipleThreads_MaxFileSize()
        {
            var testFile = Path.Combine(TestFolder, "WriteWithMaxFileSize.csv");
            DeleteTestFiles(testFile);

            const int MaxThreads = 10;
            var countdownEvent = new CountdownEvent(MaxThreads);

            using (var csvFile = new CsvFileHelper<TestModel1>(testFile))
            {
                Assert.IsTrue(csvFile.IsOpen);

                csvFile.OnFileMoved += (sender, s) => { Debug.WriteLine("File Created " + s); };

                csvFile.FileNameRolloverType = FileNameRolloverType.None;
                for (var i = 0; i < MaxThreads; i++)
                {
                    Task.Run(
                        () =>
                            {
                                var items = 10;
                                for (var count = 0; count < items; count++)
                                {
                                    var model = new TestModel1 { Id = count };
                                    csvFile.WriteAsync(model);
                                }

                                countdownEvent.Signal(1);
                            });
                }

                var signaled = countdownEvent.Wait(TimeSpan.FromMinutes(5));
                Assert.IsTrue(signaled);
            }

            Assert.IsTrue(File.Exists(testFile), "File Not Found " + testFile);

            var directoryInfo = new DirectoryInfo(TestFolder);
            foreach (var file in directoryInfo.GetFiles("WriteMultipleOnThreadsMaxRecords*.csv"))
            {
                var fullFileName = Path.Combine(TestFolder, file.Name);
                var recordCount = GetRecordCount<TestModel1>(fullFileName);
                Assert.IsTrue(FileSizeNonZero(fullFileName));

                Debug.WriteLine("CSV File: " + file.Name + " Size = " + file.Length + " Records = " + recordCount);
            }
        }

        [TestMethod]
        public void WriteLogsDateTimeStampRollOver()
        {
            var testFile = Path.Combine(TestFolder, "WriteLogsDateTimeStampRollOver.csv");
            using (var csvFileHelper = new CsvFileHelper<TestModel1>(testFile))
            {
                Assert.IsTrue(csvFileHelper.IsOpen);
                csvFileHelper.OnFileMoved += (sender, s) => { Debug.WriteLine("File Moved  " + s); };
                csvFileHelper.OnFileOpened += (sender, s) => { Debug.WriteLine("File Opened  " + s); };

                csvFileHelper.Close();
            }
        }

        [TestMethod]
        public void TestFileOpenCloseEvents()
        {
            var fileOpenedEvent = false;
            var fileClosedEvent = false;
            var testFile = Path.Combine(TestFolder, "TestFileOpenEvent.csv");
            var csvFileHelper = new CsvFileHelper<TestModel1>();
            
            csvFileHelper.OnFileOpened += (sender, s) =>
                {
                    Debug.WriteLine("File Opened  " + s);
                    fileOpenedEvent = true;
                };

            csvFileHelper.OnFileClosed += (sender, s) =>
                {
                    Debug.WriteLine("File Closed  " + s);
                    fileClosedEvent = true;
                };

            Assert.IsFalse(csvFileHelper.IsOpen);
            csvFileHelper.Open(testFile);
            Assert.IsTrue(csvFileHelper.IsOpen);
            Assert.IsTrue(fileOpenedEvent);

            csvFileHelper.Close();
            Assert.IsFalse(csvFileHelper.IsOpen);
            Assert.IsTrue(fileClosedEvent);

        }

        /// <summary>The write multiple on threads.</summary>
        [TestMethod]
        public void WriteOnMultipleThreadsMaxRecordsNumericalRollOver()
        {
            var testFile = Path.Combine(TestFolder, "WriteOnMultipleThreadsMaxRecordsNumericalRollOver.csv");
            DeleteTestFiles(testFile);

            const int MaxRecords = 100;
            const int MaxThreads = 10;
            var recordCount = 0;

            int fileMovedCount = 0;

            using (var csvFileHelper = new CsvFileHelper<TestModel1>(testFile))
            {                
                Assert.IsTrue(csvFileHelper.IsOpen);
                csvFileHelper.OnFileMoved += (sender, s) =>
                    {
                        Debug.WriteLine("File Moved  " + s);
                        fileMovedCount++;
                    };

                var countdownEvent = new CountdownEvent(MaxThreads);
                var task = new List<Task>();
                Assert.AreEqual(0, csvFileHelper.MaxFileSize);

                // setup to roll over of file output on max records per csv file (100)
                csvFileHelper.MaxRecords = MaxRecords;
                csvFileHelper.FileNameRolloverType = FileNameRolloverType.Numerical;

                for (var i = 0; i < MaxThreads; i++)
                {
                    var t = new Task(
                        () =>
                            {
                                var fileHelper = csvFileHelper;
                                var totalRecords = 100;

                                // generate random records
                                for (var count = 0; count < totalRecords; count++)
                                {
                                    var model = new TestModel1 { Id = recordCount };
                                    recordCount++;
                                    fileHelper?.WriteAsync(model);
                                }

                                countdownEvent.Signal(1);
                            });
                    task.Add(t);
                }

                task.ForEach(m => m.Start());

                var signaled = countdownEvent.Wait(Debugger.IsAttached ? -1 : 10000);
                Assert.IsTrue(signaled);
                Debug.WriteLine("-------------- Dispose ------------");
            }

            Assert.AreEqual(10, fileMovedCount);
            
            var directoryInfo = new DirectoryInfo(TestFolder);
            var totalFiles = 0;
            foreach (var file in directoryInfo.GetFiles("WriteOnMultipleThreadsMaxRecordsNumericalRollOver*.csv"))
            {
                var fullFileName = Path.Combine(TestFolder, file.Name);
                var recordsInFile = GetRecordCount<TestModel1>(fullFileName);

                Debug.WriteLine("CSV File: " + file.Name + " Size = " + file.Length + " Records = " + recordCount);
                Assert.IsTrue(FileSizeNonZero(fullFileName));
                Assert.AreEqual(MaxRecords, recordsInFile);
                totalFiles++;
            }

            Assert.AreEqual(10, totalFiles);
        }

        /// <summary>The write test and dispose.</summary>
        [TestMethod]
        public void WriteTestAndDispose()
        {
            var testFile = Path.Combine(TestFolder, "WriteTestAndDispose.csv");
            File.Delete(testFile);

            Console.WriteLine("WriteTestAndDispose Start testFile = " + testFile);
            using (var log = new CsvFileHelper<UnitTestModel>(testFile))
            {
                Assert.IsTrue(log.IsOpen, "File Not opened");
                log.WriteAsync(new UnitTestModel());
                Debug.WriteLine("-------------- Dispose ------------");
            }

            Assert.IsTrue(File.Exists(testFile));
            Assert.IsTrue(FileSizeNonZero(testFile));
            Debug.Write("Test Completed");
        }

        /// <summary>The write test not opened exception.</summary>
        [TestMethod]
        public void WriteTestNotOpenException()
        {
            var opened = false;
            try
            {
                var log = new CsvFileHelper<UnitTestModel>();
                if (log.IsOpen)
                {
                    opened = true;
                    Console.WriteLine(log.FileName);
                }

                // throw IOException since not opened
                log.Write(new UnitTestModel());
            }
            catch (IOException)
            {
                if (!opened)
                {
                    Debug.WriteLine("Expected IOException occurred");
                }
            }
        }

        [TestMethod]
        public void WriteToZipArchiveMax100RecordsPerCsv()
        {
            var testFile = Path.Combine(TestFolder, "WriteToZipArchiveMax100RecordsPerCsv.csv");
            Console.WriteLine("WriteToZipArchive Start testFile = " + testFile);
            File.Delete(testFile);
            const int MaxRecords = 1000;

            using (var csvFileHelper = new CsvFileHelper<InfotransitPresentationInfo>(testFile))
            {
                Assert.IsTrue(csvFileHelper.IsOpen, "File Not opened");
                csvFileHelper.MaxRecords = 100;
                csvFileHelper.FileNameRolloverType = FileNameRolloverType.Numerical | FileNameRolloverType.ZipArchive;

                for (var i = 0; i < MaxRecords; i++)
                {
                    var model = new InfotransitPresentationInfo
                                    {
                                        FileName = "test.png",
                                        VehicleId = "BUS1",
                                        Route = "R1",
                                        PlayStarted = DateTime.Now,
                                        PlayStopped = DateTime.Now.AddSeconds(10),
                                        IsPlayInterrupted = false,
                                        PlayedDuration = 10,
                                        Duration = 10,
                                        ResourceId = "RES123",
                                        StartedLatitude = "33.019844",
                                        StartedLongitude = "-96.698883",
                                        StoppedLatitude = "33.019847",
                                        StoppedLongitude = "-96.698887",
                                        Trip = "T1"
                                    };
                    csvFileHelper.WriteAsync(model);
                }

                Debug.WriteLine("-------------- Dispose ------------");
            }

            Assert.IsFalse(File.Exists(testFile), "Test file not deleted and should be, file=" + testFile);
            var zipFile = Path.Combine(
                Path.GetDirectoryName(testFile),
                Path.GetFileNameWithoutExtension(testFile) + ".zip");
            Assert.IsTrue(File.Exists(zipFile), "zip file missing " + zipFile);
        }

        [TestMethod]
        public void WriteToZipOnClose()
        {
            var testFile = Path.Combine(TestFolder, "WriteToZipOnClose.csv");
            Console.WriteLine("WriteToZipArchive Start testFile = " + testFile);
            File.Delete(testFile);
            const int MaxRecords = 100;
            var zipFile = Path.Combine(
                Path.GetDirectoryName(testFile),
                Path.GetFileNameWithoutExtension(testFile) + ".zip");

            // Append new csv files to the existing Zip if file is present
            var zipFileExists = File.Exists(zipFile);
            List<string> entries = null;
            if (zipFileExists)
            {
                entries = ZipFileHelper.GetFileEntries(zipFile);
                Assert.IsNotNull(entries);
            }

            using (var csvFileHelper = new CsvFileHelper<InfotransitPresentationInfo>(testFile))
            {
                Assert.IsTrue(csvFileHelper.IsOpen, "File Not opened");
                csvFileHelper.FileNameRolloverType = FileNameRolloverType.ZipArchive;

                var expectedZipFile = csvFileHelper.GetZipFileName();
                Assert.AreEqual(zipFile, expectedZipFile);

                expectedZipFile = csvFileHelper.GetZipFileName(testFile);
                Assert.AreEqual(zipFile, expectedZipFile);

                csvFileHelper.GetZipFileName(testFile);

                for (var i = 0; i < MaxRecords; i++)
                {
                    var model = new InfotransitPresentationInfo
                                    {
                                        FileName = "test.png",
                                        VehicleId = "BUS1",
                                        Route = "R1",
                                        PlayStarted = DateTime.Now,
                                        PlayStopped = DateTime.Now.AddSeconds(10),
                                        IsPlayInterrupted = false,
                                        PlayedDuration = 10,
                                        Duration = 10,
                                        ResourceId = "RES123",
                                        StartedLatitude = "33.019844",
                                        StartedLongitude = "-96.698883",
                                        StoppedLatitude = "33.019847",
                                        StoppedLongitude = "-96.698887",
                                        Trip = "T1"
                                    };
                    csvFileHelper.WriteAsync(model);
                }

                Debug.WriteLine("-------------- Dispose ------------");
            }

            Assert.IsFalse(File.Exists(testFile), "Test file not deleted and should be, file=" + testFile);

            Assert.IsTrue(File.Exists(zipFile), "zip file missing " + zipFile);

            // Test if Zip existed prior to test total entries before and after
            if (zipFileExists && entries.Any())
            {
                var newZipFileEntries = ZipFileHelper.GetFileEntries(zipFile);
                Assert.AreEqual(entries.Count + 1, newZipFileEntries.Count);
            }
        }

        [TestMethod]
        public void ZipFileNameTest()
        {
            var testFile = Path.Combine(TestFolder, "ZipFileNameTest.csv");
            var zipFile = Path.Combine(
                Path.GetDirectoryName(testFile),
                Path.GetFileNameWithoutExtension(testFile) + ".zip");

            using (var csvFileHelper = new CsvFileHelper<InfotransitPresentationInfo>(testFile))
            {
                Assert.AreEqual(FileNameRolloverType.None, csvFileHelper.FileNameRolloverType);

                var expectedZipFile = csvFileHelper.GetZipFileName();
                Assert.AreEqual(zipFile, expectedZipFile);

                csvFileHelper.FileNameRolloverType = FileNameRolloverType.ZipArchive;

                expectedZipFile = csvFileHelper.GetZipFileName();
                Assert.AreEqual(zipFile, expectedZipFile);

                expectedZipFile = csvFileHelper.GetZipFileName(testFile);
                Assert.AreEqual(zipFile, expectedZipFile);
            }
        }

        [TestMethod]
        public void TestFileRolloverAndMove()
        {
            var testFile = Path.Combine(TestFolder, "TestFileRolloverAndMove.csv");
            File.Delete(testFile);
            const string RolloverTestFolder = @"C:\Temp\TestRolloverFolder";
            if (Directory.Exists(RolloverTestFolder))
            {
                // remove to test it being created
                Directory.Delete(RolloverTestFolder, true);
            }
            
            using (var csvFileHelper = new CsvFileHelper<InfotransitPresentationInfo>(testFile))
            {
                Assert.AreEqual(FileNameRolloverType.None, csvFileHelper.FileNameRolloverType);

                csvFileHelper.MaxRecords = 50;
                csvFileHelper.FileNameRolloverType = FileNameRolloverType.TimeStampTicks;
                csvFileHelper.RollOverFilePath = RolloverTestFolder;
                csvFileHelper.OnFileOpened += (sender, s) => { Debug.WriteLine("*File Opened " + s); };
                csvFileHelper.OnFileMoved += (sender, s) => { Debug.WriteLine("*File Moved " + s); };
                csvFileHelper.OnFileClosed += (sender, s) => { Debug.WriteLine("*File Closed " + s); };
                const int MaxRecords = 100;

                for (var i = 0; i < MaxRecords; i++)
                {
                    var model = new InfotransitPresentationInfo
                                    {
                                        FileName = "test.png",
                                        VehicleId = "BUS1",
                                        Route = "R1",
                                        PlayStarted = DateTime.Now,
                                        PlayStopped = DateTime.Now.AddSeconds(10),
                                        IsPlayInterrupted = false,
                                        PlayedDuration = 10,
                                        Duration = 10,
                                        ResourceId = "RES123",
                                        StartedLatitude = "33.019844",
                                        StartedLongitude = "-96.698883",
                                        StoppedLatitude = "33.019847",
                                        StoppedLongitude = "-96.698887",
                                        Trip = "T1"
                                    };
                    csvFileHelper.WriteAsync(model);
                }   

                Debug.WriteLine("Disposing...");
            }
            
            Assert.IsTrue(Directory.Exists(RolloverTestFolder));
            var directoryInfo = new DirectoryInfo(RolloverTestFolder);
            Assert.IsTrue(directoryInfo.Exists);
            Assert.AreEqual(2, directoryInfo.GetFiles().Length);
        }

        private static void DeleteTestFiles(string fileName)
        {
            var testFile = Path.Combine(TestFolder, fileName);
            Console.WriteLine("Deleting testFile = " + testFile);
            File.Delete(testFile);
            var directoryInfo = new DirectoryInfo(TestFolder);
            var t = Path.GetFileNameWithoutExtension(fileName) + "*.csv";
            var files = directoryInfo.GetFiles(t, SearchOption.TopDirectoryOnly);
            files.ToList().ForEach(
                m =>
                    {
                        Debug.WriteLine("Delete " + m.Name);
                        m.Delete();
                    });
        }

        /// <summary>The file not empty.</summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="bool" />.</returns>
        private static bool FileNotEmpty(string fileName)
        {
            using (var f = File.Open(fileName, FileMode.Open))
            {
                return f.Length > 0;
            }
        }

        private static bool FileSizeNonZero(string fileName)
        {
            var f = new FileInfo(fileName);
            Console.WriteLine("File " + fileName + " Size = " + f.Length);
            return f.Exists && f.Length > 0;
        }

        /// <summary>Get the csv file record count.</summary>
        /// <param name="csvFileName">The file name.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="int" />.</returns>
        private static int GetRecordCount<T>(string csvFileName)
            where T : class
        {
            return CsvFileHelper<T>.GetCsvFileRecordCount(csvFileName);
        }
    }
}
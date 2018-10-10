namespace Luminator.PresentationLogging.UnitTest.PresentationPlayDataTracking
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Ximple.Generic;

    using Luminator.PresentationPlayLogging.Config;
    using Luminator.PresentationPlayLogging.Core;
    using Luminator.PresentationPlayLogging.Core.Models;
    using Luminator.PresentationPlayLogging.Core.PresentationPlayDataTracking;
    using Luminator.Utility.CsvFileHelper;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CSVLoggerTests
    {
        public const string TestFolder = @"C:\temp";

        private const string TestRolloverFolder = @"C:\temp\Rollover";

        private static bool foundFixedDriveD;

        private static string outFolder;

        private static TestContext testContext;

        /// <summary>
        ///     Gets the dictionary.
        /// </summary>
        public static Dictionary Dictionary { get; private set; }

        /// <summary>The class init.</summary>
        /// <param name="context">The context.</param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            testContext = context;
            outFolder = testContext.TestDeploymentDir;
            Directory.CreateDirectory(TestFolder);
            foundFixedDriveD = DriveInfo.GetDrives().Any(m => m.Name == @"D:\" && m.DriveType == DriveType.Fixed);

            Dictionary = PresentationInfotransitCsvLogging.ReadDictionaryFile();
        }

        /// <summary>The write multiple on threads.</summary>
        [TestMethod]
        public void Write1000InfotransitPresentationInfoRecords()
        {
            const int MaxRecords = 500;
            var testFile = Path.Combine(PresentationLoggingUnitTest.TestFolder, "Write1000InfotransitePresentationInfoRecords.csv");
            Console.WriteLine("WriteMultipleOnThreads Start testFile = " + testFile);
            File.Delete(testFile);
            var config = PresentationInfotransitCsvLoggingTests.CreateConfig(MaxRecords).ClientConfig;

            using (var logging = new CSVLogger<InfotransitPresentationInfo>())
            {
                logging.Start(testFile, config.RollOverLogOutputFolder, config.FileNameRolloverType, config.MaxFileSize, config.MaxRecords);
                var records = new List<InfotransitPresentationInfo>();
                var r = new Random();
                var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".jpg";
                var resourceId = Guid.NewGuid().ToString();

                for (var i = 0; i < config.MaxRecords; i++)
                {
                    // create fake data
                    var model = PresentationLoggingUnitTest.CreateMockInfotransitPresentationInfo(r, i, ref fileName, ref resourceId);
                    records.Add(model);
                }

                logging.WriteAll(records);
            }

            Assert.IsTrue(File.Exists(testFile));
            Assert.IsTrue(PresentationLoggingUnitTest.IsFileSizeNonZero(testFile));
            var recordCount = PresentationLoggingUnitTest.GetCsvFileRecordCount<InfotransitPresentationInfo>(testFile);
            Console.WriteLine("Final file Records Cont = " + recordCount);
            Assert.AreNotEqual(0, recordCount, "Record Count in file != 0");
            Assert.AreEqual(MaxRecords, recordCount);

            var allRecords = PresentationInfotransitCsvLogging.ReadAll(testFile);
            Assert.AreEqual(MaxRecords, allRecords.Count);
        }

        [TestMethod]
        public void WriteAndTestNumericalRollover()
        {
            this.WriteAndTestTimestampRollover(FileNameRolloverType.Numerical);
        }

        [TestMethod]
        public void WriteAndTestTimestampRollover()
        {
            this.WriteAndTestTimestampRollover(FileNameRolloverType.TimeStampTicks);
        }

        public void WriteAndTestTimestampRollover(FileNameRolloverType fileNameRolloverType)
        {
            var testFile = Path.Combine(PresentationLoggingUnitTest.TestFolder, fileNameRolloverType + "-UnitTestRollover.csv");
            Console.WriteLine("WriteMultipleOnThreads Start testFile = " + testFile);
            File.Delete(testFile);

            DirectoryInfo dirInfo = null;
            var searchPattern = fileNameRolloverType + "*.csv";
            const int MaxRolloverRecords = 100;
            const int RecordsToWrite = 1000;
            var config = PresentationInfotransitCsvLoggingTests.CreateConfig(MaxRolloverRecords).ClientConfig;
            using (var logging = new CSVLogger<InfotransitPresentationInfo>())
            {

                Console.WriteLine("RollOverLogOutputFolder=" + config.RollOverLogOutputFolder);

                // delete any existing files for testing
                dirInfo = new DirectoryInfo(config.RollOverLogOutputFolder);
                var files = dirInfo.GetFiles(searchPattern).ToList();
                foreach (var f in files)
                {
                    File.Delete(f.FullName);
                }

                logging.Start(testFile, config.RollOverLogOutputFolder, fileNameRolloverType, config.MaxFileSize, MaxRolloverRecords);

                var resourceId = Guid.NewGuid().ToString();
                var r = new Random();
                var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".jpg";
                for (var i = 0; i < RecordsToWrite; i++)
                {
                    // create fake data
                    var model = PresentationLoggingUnitTest.CreateMockInfotransitPresentationInfo(r, i, ref fileName, ref resourceId);
                    logging.WriteAsync(model);
                }
            }

            var newFiles = dirInfo.GetFiles(searchPattern).ToList();
            Assert.AreEqual(10, newFiles.Count());
        }
    }
}
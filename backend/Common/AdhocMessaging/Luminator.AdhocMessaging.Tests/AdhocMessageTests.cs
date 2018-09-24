namespace Luminator.AdhocMessaging.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Luminator.AdhocMessaging.Helpers;
    using Luminator.AdhocMessaging.Interfaces;
    using Luminator.AdhocMessaging.Models;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using NLog;

    [TestClass]
    public class AdhocMessageTests
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static AdhocConfiguration adhocConfiguration;

        private static TestContext testContext;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            testContext = context;
            var nlogTraceListner = new NLogTraceListener { TraceOutputOptions = TraceOptions.Timestamp | TraceOptions.ThreadId };
            Debug.Listeners.Add(nlogTraceListner);
            adhocConfiguration = new AdhocConfiguration();
        }

        [TestMethod]
        public void AdhocManagerCustomConfigTest()
        {
            var adhocMessageFactory = new AdhocMessageFactory();
            var ac = new AdhocConfiguration("http://swdevicntrapp.luminatorusa.com/", string.Empty, "http://swdevicntrweb.luminatorusa.com/", string.Empty);
            var adhocManager = adhocMessageFactory.CreateAdhocManager(adhocConfiguration);
            Assert.IsNotNull(adhocManager);
        }

        [TestMethod]
        public void AdhocManagerTest()
        {
            var adhocMessageFactory = new AdhocMessageFactory();
            var adhocManager = adhocMessageFactory.CreateAdhocManager(adhocConfiguration);
            Assert.IsNotNull(adhocManager);
        }

        [TestMethod]
        public void TestCreateAdhocManagerUnitTest()
        {
            var mockFactory = new Mock<IAdhocMessageFactory>();
            var mockAdhoc = new Mock<IAdhocManager>();
            mockFactory.Setup(fac => fac.CreateAdhocManager(It.IsAny<string>())).Returns(mockAdhoc.Object);
            Assert.IsNotNull(mockAdhoc.Object);
        }

        [TestMethod]
        public void RegisterUnitTest()
        {
            var adhocMessageFactory = new AdhocMessageFactory();
            var ac = new AdhocConfiguration("http://swdevicntrapp.luminatorusa.com/", string.Empty, "http://swdevicntrweb.luminatorusa.com/", string.Empty);
            var adhocManager = adhocMessageFactory.CreateAdhocManager(adhocConfiguration);
            var result = adhocManager.RegisterUnitAsync("Test Unit");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RegisterUnitUsingLocalhostandLocalDbTest()
        {
            var adhocMessageFactory = new AdhocMessageFactory();
            var ac = new AdhocConfiguration("http://localhost", "63093", "http://localhost/", string.Empty);
            var adhocManager = adhocMessageFactory.CreateAdhocManager(ac);
            var rnd = new Random((int)DateTime.Now.Ticks);
            var next = rnd.Next(rnd.Next(100, 1000), 1500);
            var tftName = $"Test TFT-{next}";
            var result = adhocManager.RegisterUnitAsync(tftName).Result;
            if (result == HttpStatusCode.OK)
                try
                {
                    var connectionString = AdhocConstants.DefaultDbConnectionStringsList[2];

                    using (var connection = new SqlConnection(connectionString))
                    {
                        if (connection.IsAvailable())
                        {
                            connection.Open();
                            Console.WriteLine($"Connection State to DB : {connection.State}");
                            var command = new SqlCommand($"SELECT * FROM Units where Name = '{tftName}'", connection);
                            var reader = command.ExecuteReader();
                            Assert.IsTrue(reader.FieldCount > 0);
                            Assert.IsTrue(reader.HasRows);
                            reader.Print();
                            var deleteCommand = $"DELETE FROM [dbo].[Units] WHERE Name = '{tftName}'";
                            command = new SqlCommand(deleteCommand, connection);
                            command.ExecuteNonQuery();
                            command = new SqlCommand($"SELECT * FROM Units where Name = '{tftName}'", connection);
                            reader = command.ExecuteReader();
                            Assert.IsFalse(reader.HasRows);
                            reader.Print();
                            connection.Close();
                            Console.WriteLine($"Connection State to DB : {connection.State}");
                        }
                    }
                }
                catch (DbException ex)
                {
                    Console.WriteLine($"Exception on Connection to DB {ex}");
                }
                catch (SystemException ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            else
            {
                Console.WriteLine($"Registration of Unit Failed...");
            }
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RegisterVehicleWithUnitNoUnitNameTest()
        {
            var adhocMessageFactory = new AdhocMessageFactory();
            var ac = new AdhocConfiguration("http://localhost", "63093", "http://localhost/", string.Empty);
            var adhocManager = adhocMessageFactory.CreateAdhocManager(ac);
            var rnd = new Random((int)DateTime.Now.Ticks);
            var next = rnd.Next(rnd.Next(100, 1000), 1500);
            var tftName = $"Test TFT-{next}";
            var busName = $"Test Bus # {next}";
            HttpStatusCode result = HttpStatusCode.NotFound;
            try
            {
                var t = Task.Run(() => adhocManager.RegisterVehicleAndUnitAsync($"{busName}", String.Empty))
                    .ContinueWith(
                        prev =>
                            {
                                Console.WriteLine((result = prev.Result) == HttpStatusCode.OK ?
                                                      " Sucessfully Registered Vechile with unit" :
                                                      $"Failed to Register Vechile with Unit {prev.Result}");
                            },
                    TaskContinuationOptions.OnlyOnRanToCompletion);
                t.Wait(15000);
                Assert.IsFalse(result != HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

        }

        [TestMethod]
        public void RegisterVehicleWithUnitNoVehicleNameNoUnitNameTest()
        {
            var adhocMessageFactory = new AdhocMessageFactory();
            var ac = new AdhocConfiguration("http://localhost", "63093", "http://localhost/", string.Empty);
            var adhocManager = adhocMessageFactory.CreateAdhocManager(ac);
            var rnd = new Random((int)DateTime.Now.Ticks);
            var next = rnd.Next(rnd.Next(100, 1000), 1500);
            var tftName = $"Test TFT-{next}";
            var busName = $"Test Bus # {next}";
            var result = HttpStatusCode.NotFound;
            try
            {
                var t = Task.Run(() => adhocManager.RegisterVehicleAndUnitAsync(string.Empty, string.Empty))
                    .ContinueWith(
                        prev =>
                            {
                                Console.WriteLine((result = prev.Result) == HttpStatusCode.OK ?
                                                      " Sucessfully Registered Vechile with unit" :
                                                      $"Failed to Register Vechile with Unit {prev.Result}");
                            },
                        TaskContinuationOptions.OnlyOnRanToCompletion);
                t.Wait(15000);
                Assert.IsFalse(result != HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

        }

        [TestMethod]
        public void RegisterVehicleWithUnitUsingLocalhostandLocalDbTest()
        {
            var adhocMessageFactory = new AdhocMessageFactory();
            var ac = new AdhocConfiguration("http://localhost", "63093", "http://localhost/", string.Empty);
            var adhocManager = adhocMessageFactory.CreateAdhocManager(ac);
            var rnd = new Random((int)DateTime.Now.Ticks);
            var next = rnd.Next(rnd.Next(100, 1000), 1500);
            var tftName = $"Test TFT-{next}";
            var busName = $"Test Bus # {next}";
            HttpStatusCode result = HttpStatusCode.NotFound;
            var t = Task.Run(() => adhocManager.RegisterVehicleAndUnitAsync($"{busName}", $"{tftName}")).ContinueWith(
                prev => { Console.WriteLine((result = prev.Result) == HttpStatusCode.OK ? " Sucessfully Registered Vechile with unit" : $"Failed to Register Vechile with Unit {prev.Result}"); },
                TaskContinuationOptions.OnlyOnRanToCompletion);
            t.Wait(15000);

            if (result == HttpStatusCode.OK)
                try
                {
                    var connectionString = AdhocConstants.DefaultDbConnectionStringsList[2];

                    using (var connection = new SqlConnection(connectionString))
                    {
                        if (connection.IsAvailable())
                        {
                            connection.Open();
                            Console.WriteLine($"Connection State to DB : {connection.State}");
                            var command = new SqlCommand($"SELECT * FROM Units where Name = '{tftName}'", connection);
                            var reader = command.ExecuteReader();
                            Assert.IsTrue(reader.FieldCount > 0);
                            Assert.IsTrue(reader.HasRows);
                            reader.Print();
                            command = new SqlCommand($"SELECT * FROM Vehicles where Name = '{busName}'", connection);
                            reader = command.ExecuteReader();
                            Assert.IsTrue(reader.FieldCount > 0);
                            Assert.IsTrue(reader.HasRows);
                            reader.Print();
                            var deleteCommand = $"DELETE FROM [dbo].[Units] WHERE Name = '{tftName}'";
                            command = new SqlCommand(deleteCommand, connection);
                            command.ExecuteNonQuery();
                            deleteCommand = $"DELETE FROM Vehicles WHERE Name = '{tftName}'";
                            command = new SqlCommand(deleteCommand, connection);
                            command.ExecuteNonQuery();
                            command = new SqlCommand($"SELECT * FROM Units where Name = '{tftName}'", connection);
                            reader = command.ExecuteReader();
                            Assert.IsFalse(reader.HasRows);
                            reader.Print();
                            command = new SqlCommand($"SELECT * FROM Vehicles where Name = '{tftName}'", connection);
                            reader = command.ExecuteReader();
                            Assert.IsFalse(reader.HasRows);
                            reader.Print();
                            connection.Close();
                            Console.WriteLine($"Connection State to DB : {connection.State}");
                        }
                    }
                }
                catch (DbException ex)
                {
                    Console.WriteLine($"Exception on Connection to DB {ex}");
                }
                catch (SystemException ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            else
            {
                Console.WriteLine($"Registration of Unit Failed...");
            }
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RegisterVehicleWithUnitUsingSwdevIcenterandSwdevIcntrSqlDbLoadTest()
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < 500; i++)
            {
                Console.WriteLine($"Load # {i}");
                var next = rnd.Next(rnd.Next(100, 1000), 1500);
                VechicleWithUnitRegistrationTest($"{next}");
                Thread.Sleep(1000);
            }
        }

        [TestMethod]
        public void RegisterVehicleWithUnitUsingSwdevIcenterandSwdevIcntrSqlDbTest()
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            var next = rnd.Next(rnd.Next(100, 1000), 1500);
            VechicleWithUnitRegistrationTest($"{next}");
        }

        [TestMethod]
        public void RegisterVehicleWithUnitUsingSwdevIcenterandSwdevIcntrSqlDbTest001()
        {
           
            VechicleWithUnitRegistrationTest($"001");
        }

        private static void VechicleWithUnitRegistrationTest(string next)
        {
            var adhocMessageFactory = new AdhocMessageFactory();
            var ac = new AdhocConfiguration(AdhocConstants.DefaultDestinationsApiUrl, string.Empty, AdhocConstants.DefaultMessageApiUrl, string.Empty);
            var adhocManager = adhocMessageFactory.CreateAdhocManager(ac);
            var tftName = $"Test TFT-{next}";
            var busName = $"Test Bus # {next}";
            HttpStatusCode result = HttpStatusCode.NotFound;
            var t = Task.Run(() => adhocManager.RegisterVehicleAndUnitAsync($"{busName}", $"{tftName}")).ContinueWith(
                prev => { Console.WriteLine((result = prev.Result) == HttpStatusCode.OK ? " Sucessfully Registered Vechile with unit" : $"Failed to Register Vechile with Unit {prev.Result}"); },
                TaskContinuationOptions.OnlyOnRanToCompletion);
            t.Wait(15000);

            if (result == HttpStatusCode.OK)
                try
                {
                    var connectionString = AdhocConstants.DefaultDbConnectionStringsList[4];

                    using (var connection = new SqlConnection(connectionString))
                    {
                        if (connection.IsAvailable())
                        {
                            connection.Open();
                            Console.WriteLine($"Connection State to DB : {connection.State}");
                            var command = new SqlCommand($"SELECT * FROM Units where Name = '{tftName}'", connection);
                            var reader = command.ExecuteReader();
                            Assert.IsTrue(reader.FieldCount > 0);
                            Assert.IsTrue(reader.HasRows);
                            reader.Print();
                            command = new SqlCommand($"SELECT * FROM Vehicles where Name = '{busName}'", connection);
                            reader = command.ExecuteReader();
                            Assert.IsTrue(reader.FieldCount > 0);
                            Assert.IsTrue(reader.HasRows);
                            reader.Print();
                            var deleteCommand = $"DELETE FROM [dbo].[Units] WHERE Name = '{tftName}'";
                            command = new SqlCommand(deleteCommand, connection);
                            command.ExecuteNonQuery();
                            deleteCommand = $"DELETE FROM Vehicles WHERE Name = '{busName}'";
                            command = new SqlCommand(deleteCommand, connection);
                            command.ExecuteNonQuery();
                            command = new SqlCommand($"SELECT * FROM Units where Name = '{tftName}'", connection);
                            reader = command.ExecuteReader();
                            Assert.IsFalse(reader.HasRows);
                            reader.Print();
                            command = new SqlCommand($"SELECT * FROM Vehicles where Name = '{busName}'", connection);
                            reader = command.ExecuteReader();
                            Assert.IsFalse(reader.HasRows);
                            reader.Print();
                            connection.Close();
                            Console.WriteLine($"Connection State to DB : {connection.State}");
                        }
                    }
                }
                catch (DbException ex)
                {
                    Console.WriteLine($"Exception on Connection to DB {ex}");
                }
                catch (SystemException ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            else
            {
                Console.WriteLine($"Registration of Unit Failed...");
            }
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RegisterVehicleWithUnitTest()
        {
            var adhocMessageFactory = new AdhocMessageFactory();
            var ac = new AdhocConfiguration("http://nyctdestinations.luminatorusa.com", string.Empty, "http://nyctadhoc.luminatorusa.com", string.Empty);
            var adhocManager = adhocMessageFactory.CreateAdhocManager(ac);
            var rnd = new Random((int)DateTime.Now.Ticks);
            var next = rnd.Next(rnd.Next(100, 1000), 1500);
            Console.WriteLine("Registering Vehicle with Unit Async");
            var toRegisterTft = $"{Environment.MachineName} {next}";
            var t = Task.Run(() => adhocManager.RegisterVehicleAndUnitAsync($"Vehicle {next} {DateTime.Now.ToShortDateString()}", $"{toRegisterTft}")).ContinueWith(
                prev => { Console.WriteLine(prev.Result == HttpStatusCode.OK ? " Sucessfully Registered Vechile with unit" : $"Failed to Register Vechile with Unit {prev.Result}"); },
                TaskContinuationOptions.OnlyOnRanToCompletion);
            t.Wait(15000);
        }

        [TestMethod]
        public void RegisterVehicleWithUnitFlowTest()
        {
            var sleepThreadForInMillSeconds = 5000;
            var waitForThreadCompletionInSeconds = 7000;
            Console.WriteLine("Registering Vehicle with Unit Async Flow Test");
            var success = false;
            Assert.IsTrue(sleepThreadForInMillSeconds + 1000 < waitForThreadCompletionInSeconds);
            var t = Task.Run(() => { Thread.Sleep(sleepThreadForInMillSeconds); }).ContinueWith(
                prev =>
                    {
                        Console.WriteLine(" Sucessfully Completed wait of 5 seconds");
                        success = true;
                    },
                TaskContinuationOptions.OnlyOnRanToCompletion);
            t.Wait(waitForThreadCompletionInSeconds);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void RegisterVehicleTest()
        {
            var mockFactory = new Mock<IAdhocMessageFactory>();
            var mockAdhoc = new Mock<IAdhocManager>();
            mockFactory.Setup(fac => fac.CreateAdhocManager(It.IsAny<string>())).Returns(mockAdhoc.Object);
            var vehicle = new Vehicle { Description = string.Empty, Name = string.Empty };
            var result = mockAdhoc.Object.RegisterVehicleAsync(vehicle);
            Assert.IsTrue(result.Result == HttpStatusCode.OK);
            var vehicleExistsResult = mockAdhoc.Object.UnitExistsAsync(vehicle.Name);
            Assert.IsTrue(vehicleExistsResult.Result == HttpStatusCode.OK);
        }

        [TestMethod]
        public void RegisterVehicleWithMissingUnitName()
        {
            var vehicle = new Vehicle { Description = "New Registration", Name = string.Empty };
            var mgr = new AdhocManager(new AdhocConfiguration());
            var result = mgr.RegisterVehicleAsync(vehicle);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.Result);
        }

        [TestMethod]
        public void VehicleExistsTest()
        {
            var mockFactory = new Mock<IAdhocMessageFactory>();
            var mockAdhoc = new Mock<IAdhocManager>();
            mockFactory.Setup(fac => fac.CreateAdhocManager(It.IsAny<string>())).Returns(mockAdhoc.Object);
            var vehicle = new Vehicle { Description = string.Empty, Name = string.Empty };
            var result = mockAdhoc.Object.RegisterVehicleAsync(vehicle);
            Assert.IsTrue(result.Result == HttpStatusCode.OK);
            var vehicleExistsResult = mockAdhoc.Object.UnitExistsAsync(vehicle.Name);
            Assert.IsTrue(vehicleExistsResult.Result == HttpStatusCode.OK);
        }
    }
}
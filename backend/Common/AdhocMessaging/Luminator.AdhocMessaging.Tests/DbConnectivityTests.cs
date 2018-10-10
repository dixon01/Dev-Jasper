using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminator.AdhocMessaging.Tests
{
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Threading;

    using Luminator.AdhocMessaging.Helpers;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass]
    public class DbConnectivityTests
    {
        [TestMethod]
        public void ConnectToLocalDbTest()
        {
            string sql2 = $@"Data Source=.\SQLEXPRESS;Integrated Security=True;Database = Infotransit.Destinations;MultipleActiveResultSets=True";
            try
            {
                using (var connection = new SqlConnection(sql2))
                {
                    connection.Open();
                    Console.WriteLine($"Opened Connection to DB State : {connection.State}, {connection.Database}");
                    SqlCommand command = new SqlCommand($"SELECT * FROM units", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader[0]} , {reader[1]} , {reader[2]}");
                    }
                    connection.Close();
                    Console.WriteLine($"Closing Connection to DB State : {connection.State}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [TestMethod]
        public void ConnectToDbTest()
        {
            // var connectionString = "Connect Timeout=10;Pooling=false;Server = swnycticntr; Database = Infotransit.Destinations; user = lumuser; password = Lum2014;";
            //ConnectionString": "Server = swnycticntr; Database = Infotransit.Web; user = lumuser; password = Lum2014; "
            //"ConnectionString": "Server=swdevsql;Database=Infotransit.Destinations;user=lumuser;password=Lum2014;",

            var dbConnectionStringsList = new List<string> { @"Connect Timeout=10;Pooling=false;Server = swnycticntr; Database = Infotransit.Destinations; user = lumuser; password = Lum2014;",
                                                               @"Data Source=.\SQLEXPRESS;Integrated Security=True;Database = Infotransit.Destinations;MultipleActiveResultSets=True" };

            var connectionString = SqlExtensions.GetFirstActiveDbConnectionString(dbConnectionStringsList);
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    if (connection.IsAvailable())
                    {
                        connection.Open();
                        Console.WriteLine($"Opened Connection to DB State : {connection.State}, {connection.Database}");
                        SqlCommand command = new SqlCommand($"SELECT * FROM Units", connection);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader[0]} , {reader[1]} , {reader[2]}");
                        }
                        connection.Close();
                        Console.WriteLine($"Closing Connection to DB State : {connection.State}");
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




        }

        [TestMethod]
        public void ConnectToDbAndCreateTftTest()
        {

            try
            {
                string connectionString = $@"Data Source=.\SQLEXPRESS;Integrated Security=True;Database = Infotransit.Destinations;MultipleActiveResultSets=True";

                // var connectionString = "Connect Timeout=10;Pooling=false;Server = swnycticntr; Database = Infotransit.Destinations; user = lumuser; password = Lum2014;";
                //ConnectionString": "Server = swnycticntr; Database = Infotransit.Web; user = lumuser; password = Lum2014; "
                //"ConnectionString": "Server=swdevsql;Database=Infotransit.Destinations;user=lumuser;password=Lum2014;",
                Random rnd = new Random((int)DateTime.Now.Ticks);
                var next = rnd.Next(rnd.Next(100, 1000), 1500);
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine($"Opened Connection to DB State : {connection.State}, {connection.Database}");
                    var tftName = $"Test Name {next}";
                    var insertCommand = $"INSERT INTO Units (Id,Description, Name, TenantId,Version,ProductType_Id,UpdateGroup_Id)\r\nVALUES (NEWID(),\'Test Desc {next}\', \'{tftName}\', \'7141d8e5-b8e1-425a-21e5-08d541a9fc53\',1,0,0);";
                    SqlCommand command = new SqlCommand(insertCommand, connection);
                    command.ExecuteNonQuery();
                    SqlCommand commandSelect = new SqlCommand($"SELECT * FROM Units where Name = '{tftName}'", connection);
                    SqlDataReader reader = commandSelect.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader[0]} , {reader[1]} , {reader[2]}");
                    }
                    reader.Close();

                    var deleteCommand = $"DELETE FROM [dbo].[Units] WHERE Name = '{tftName}'";
                    command = new SqlCommand(deleteCommand, connection);
                    command.ExecuteNonQuery();
                    SqlCommand commandSelect2 = new SqlCommand($"SELECT * FROM Units where Name = '{tftName}'", connection);
                    Thread.Sleep(2000);
                    reader = commandSelect2.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader[0]} , {reader[1]} , {reader[2]}");
                    }

                    connection.Close();
                    Console.WriteLine($"Closing Connection to DB State : {connection.State}");
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


        }


    }
}

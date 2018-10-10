namespace Luminator.AdhocMessaging.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;

    public static class SqlExtensions
    {
        public static bool IsAvailable(this SqlConnection connection)
        {
            try
            {
                connection.Open();
                connection.Close();
            }
            catch (SqlException)
            {
                return false;
            }

            return true;
        }

        public static bool Print(this SqlDataReader reader)
        {
            if (!reader.HasRows)
            {
                Console.WriteLine("No Rows in Table");
                return false;
            }

            try
            {
                var table = new DataTable();
                table.Load(reader);
                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        Console.Write($"{column.ColumnName} : {row[column]} , ");
                    }

                    Console.WriteLine();
                }
            }
            catch (SqlException)
            {
                return false;
            }
            return true;
        }

        public static string GetFirstActiveDbConnectionString(List<string> connectionStrings)
        {
            foreach (var connectionString in connectionStrings)
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        if (connection.IsAvailable())
                        {
                            connection.Open();

                            connection.Close();
                            return connectionString;
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

            return string.Empty;
        }
    }
}
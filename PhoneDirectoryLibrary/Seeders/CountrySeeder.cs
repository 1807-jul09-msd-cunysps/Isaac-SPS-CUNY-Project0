using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary.Seeders
{
    public static class CountrySeeder
    {
        public static void Seed()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            SqlConnection connection = null;
            string connectionString = "Data Source=robodex.database.windows.net;Initial Catalog=RoboDex;Persist Security Info=True;User ID=isaac;Password=qe%8KQ^mrjJe^zq75JmPe$xa2tWFxH";

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                foreach (var country in Lookups.CountryKeys())
                {
                    InsertCountry(country.Key, country.Value, connection);
                }
            }
            catch(SeederException e)
            {
                logger.Error(e.Message);
            }
            catch(SqlException e)
            {
                logger.Error(e.Message);
            }
            catch(Exception e)
            {
                logger.Error(e.Message);
            }
        }

        private static void InsertCountry(int countryId, string countryName, SqlConnection connection)
        {
            string commandString = "INSERT INTO Country values(@p1, @p2)";
            SqlCommand insertCommand = new SqlCommand(commandString, connection);
            insertCommand.Parameters.AddWithValue("@p1", countryId);
            insertCommand.Parameters.AddWithValue("@p2", countryName);

            if(insertCommand.ExecuteNonQuery() != 1)
            {
                throw new SeederException($"Failed to insert country '{countryName}'");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary.Seeders
{
    public static class StateSeeder
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

                foreach (var state in Lookups.StateNames)
                {
                    InsertState(state.Key.ToString(), state.Value, connection);
                }
            }
            catch (SeederException e)
            {
                logger.Error(e.Message);
            }
            catch (SqlException e)
            {
                logger.Error(e.Message);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        private static void InsertState(string stateCode, string stateName, SqlConnection connection)
        {
            string commandString = "INSERT INTO StateLookup values(@p1, @p2)";
            SqlCommand insertCommand = new SqlCommand(commandString, connection);
            insertCommand.Parameters.AddWithValue("@p1", stateCode);
            insertCommand.Parameters.AddWithValue("@p2", stateName);

            if (insertCommand.ExecuteNonQuery() != 1)
            {
                throw new SeederException($"Failed to insert state '{stateName}'");
            }
        }
    }
}

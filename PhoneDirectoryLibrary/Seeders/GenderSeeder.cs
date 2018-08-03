using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary.Seeders
{
    class GenderSeeder
    {
        public static void Seed(ref PhoneDirectory phoneDirectory, int count = 1)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            string connectionString = "Data Source=robodex.database.windows.net;Initial Catalog=RoboDex;Persist Security Info=True;User ID=isaac;Password=qe%8KQ^mrjJe^zq75JmPe$xa2tWFxH";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var gender in Lookups.Genders)
                    {
                        string genderCommandString = "INSERT INTO Gender VALUES(@id, @DisplayName)";
                        SqlCommand genderCommand = new SqlCommand(genderCommandString, connection);
                        genderCommand.Parameters.AddWithValue("@id", gender.Key);
                        genderCommand.Parameters.AddWithValue("@DisplayName", gender.Value);

                        if(genderCommand.ExecuteNonQuery() == 0)
                        {
                            throw new SeederException($"Couldn't insert gender. ID: {gender.Key}, DisplayName: {gender.Value}");
                        }
                    }
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
    }
}

using Microsoft.Data.SqlClient;

namespace API;

public class DeviceService
{
    string _connectionString = "UniversityDatabase";
    public IEnumerable<Device> GetDevices()
    {
        List<Device> devices = [];
        const string queryString = "SELECT * FROM Device";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows))
                {
                    while (reader.Read())
                    {
                        var device = new Device()
                        {
                            
                        }
                    }
                }
            }
            finally
            {
                
            }
        }
    }
}
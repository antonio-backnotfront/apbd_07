using Microsoft.Data.SqlClient;
using Models;
using System.Data;

namespace Logic;

public class DeviceRepository : IDeviceRepository
{
    private readonly string _connectionString;

    public DeviceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<Device> GetAllDevices()
    {
        var devices = new List<Device>();

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var command = new SqlCommand(@"
            SELECT d.Id, d.Name, d.IsTurnedOn, 
                   ed.IpAddress, ed.NetworkName,
                   pc.OperatingSystem,
                   sw.BatteryPercentage
            FROM Device d
            LEFT JOIN EmbeddedDevice ed ON ed.Id = d.Id
            LEFT JOIN PersonalComputer pc ON pc.Id = d.Id
            LEFT JOIN Smartwatch sw ON sw.Id = d.Id", connection);

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            devices.Add(MapDevice(reader));
        }

        return devices;
    }

    public Device? GetDeviceById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var command = new SqlCommand(@"
            SELECT d.Id, d.Name, d.IsTurnedOn, 
                   ed.IpAddress, ed.NetworkName,
                   pc.OperatingSystem,
                   sw.BatteryPercentage
            FROM Device d
            LEFT JOIN EmbeddedDevice ed ON ed.Id = d.Id
            LEFT JOIN PersonalComputer pc ON pc.Id = d.Id
            LEFT JOIN Smartwatch sw ON sw.Id = d.Id
            WHERE d.Id = @Id", connection);

        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return MapDevice(reader);
        }

        return null;
    }

    public int AddDevice(Device device)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var insertDeviceCommand = new SqlCommand(@"
                INSERT INTO Device (Name, IsTurnedOn)
                OUTPUT INSERTED.Id
                VALUES (@Name, @IsTurnedOn)", connection, transaction);

            insertDeviceCommand.Parameters.AddWithValue("@Name", device.Name);
            insertDeviceCommand.Parameters.AddWithValue("@IsTurnedOn", device.IsTurnedOn);

            var newId = (int)insertDeviceCommand.ExecuteScalar()!;

            switch (device)
            {
                case EmbeddedDevice ed:
                    var insertEmbedded = new SqlCommand(@"
                        INSERT INTO EmbeddedDevice (Id, IpAddress, NetworkName)
                        VALUES (@Id, @IpAddress, @NetworkName)", connection, transaction);
                    insertEmbedded.Parameters.AddWithValue("@Id", newId);
                    insertEmbedded.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
                    insertEmbedded.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                    insertEmbedded.ExecuteNonQuery();
                    break;

                case PersonalComputer pc:
                    var insertPC = new SqlCommand(@"
                        INSERT INTO PersonalComputer (Id, OperatingSystem)
                        VALUES (@Id, @OperatingSystem)", connection, transaction);
                    insertPC.Parameters.AddWithValue("@Id", newId);
                    insertPC.Parameters.AddWithValue("@OperatingSystem", pc.OperatingSystem ?? (object)DBNull.Value);
                    insertPC.ExecuteNonQuery();
                    break;

                case Smartwatch sw:
                    var insertSW = new SqlCommand(@"
                        INSERT INTO Smartwatch (Id, BatteryPercentage)
                        VALUES (@Id, @BatteryPercentage)", connection, transaction);
                    insertSW.Parameters.AddWithValue("@Id", newId);
                    insertSW.Parameters.AddWithValue("@BatteryPercentage", sw.BatteryPercentage);
                    insertSW.ExecuteNonQuery();
                    break;
            }

            transaction.Commit();
            return newId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void UpdateDevice(Device device)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var updateDeviceCommand = new SqlCommand(@"
                UPDATE Device
                SET Name = @Name, IsTurnedOn = @IsTurnedOn
                WHERE Id = @Id", connection, transaction);

            updateDeviceCommand.Parameters.AddWithValue("@Id", device.Id);
            updateDeviceCommand.Parameters.AddWithValue("@Name", device.Name);
            updateDeviceCommand.Parameters.AddWithValue("@IsTurnedOn", device.IsTurnedOn);

            updateDeviceCommand.ExecuteNonQuery();

            switch (device)
            {
                case EmbeddedDevice ed:
                    var updateEmbedded = new SqlCommand(@"
                        UPDATE EmbeddedDevice
                        SET IpAddress = @IpAddress, NetworkName = @NetworkName
                        WHERE Id = @Id", connection, transaction);
                    updateEmbedded.Parameters.AddWithValue("@Id", ed.Id);
                    updateEmbedded.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
                    updateEmbedded.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                    updateEmbedded.ExecuteNonQuery();
                    break;

                case PersonalComputer pc:
                    var updatePC = new SqlCommand(@"
                        UPDATE PersonalComputer
                        SET OperatingSystem = @OperatingSystem
                        WHERE Id = @Id", connection, transaction);
                    updatePC.Parameters.AddWithValue("@Id", pc.Id);
                    updatePC.Parameters.AddWithValue("@OperatingSystem", pc.OperatingSystem ?? (object)DBNull.Value);
                    updatePC.ExecuteNonQuery();
                    break;

                case Smartwatch sw:
                    var updateSW = new SqlCommand(@"
                        UPDATE Smartwatch
                        SET BatteryPercentage = @BatteryPercentage
                        WHERE Id = @Id", connection, transaction);
                    updateSW.Parameters.AddWithValue("@Id", sw.Id);
                    updateSW.Parameters.AddWithValue("@BatteryPercentage", sw.BatteryPercentage);
                    updateSW.ExecuteNonQuery();
                    break;
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void DeleteDevice(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var deleteEmbedded = new SqlCommand("DELETE FROM EmbeddedDevice WHERE Id = @Id", connection, transaction);
            deleteEmbedded.Parameters.AddWithValue("@Id", id);
            deleteEmbedded.ExecuteNonQuery();

            var deletePC = new SqlCommand("DELETE FROM PersonalComputer WHERE Id = @Id", connection, transaction);
            deletePC.Parameters.AddWithValue("@Id", id);
            deletePC.ExecuteNonQuery();

            var deleteSW = new SqlCommand("DELETE FROM Smartwatch WHERE Id = @Id", connection, transaction);
            deleteSW.Parameters.AddWithValue("@Id", id);
            deleteSW.ExecuteNonQuery();

            var deleteDevice = new SqlCommand("DELETE FROM Device WHERE Id = @Id", connection, transaction);
            deleteDevice.Parameters.AddWithValue("@Id", id);
            deleteDevice.ExecuteNonQuery();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private Device MapDevice(SqlDataReader reader)
    {
        var id = reader.GetInt32(reader.GetOrdinal("Id"));
        var name = reader.GetString(reader.GetOrdinal("Name"));
        var isTurnedOn = reader.GetBoolean(reader.GetOrdinal("IsTurnedOn"));

        if (!reader.IsDBNull(reader.GetOrdinal("IpAddress")))
        {
            return new EmbeddedDevice
            {
                Id = id,
                Name = name,
                IsTurnedOn = isTurnedOn,
                IpAddress = reader.GetString(reader.GetOrdinal("IpAddress")),
                NetworkName = reader.GetString(reader.GetOrdinal("NetworkName"))
            };
        }
        else if (!reader.IsDBNull(reader.GetOrdinal("OperatingSystem")))
        {
            return new PersonalComputer
            {
                Id = id,
                Name = name,
                IsTurnedOn = isTurnedOn,
                OperatingSystem = reader.GetString(reader.GetOrdinal("OperatingSystem"))
            };
        }
        else if (!reader.IsDBNull(reader.GetOrdinal("BatteryPercentage")))
        {
            return new Smartwatch
            {
                Id = id,
                Name = name,
                IsTurnedOn = isTurnedOn,
                BatteryPercentage = reader.GetInt32(reader.GetOrdinal("BatteryPercentage"))
            };
        }
        else
        {
            throw new Exception("Unknown device type in database.");
        }
    }
}

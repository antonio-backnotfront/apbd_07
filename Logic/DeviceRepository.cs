using Microsoft.Data.SqlClient;
using Models;


namespace Logic;

public class DeviceRepository : IDeviceRepository
{
    private readonly string _connectionString;

    public DeviceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Device>> GetAllDevicesAsync()
    {
        var devices = new List<Device>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new SqlCommand(@"
            SELECT d.Id, d.Name, d.IsTurnedOn, 
                   ed.IpAddress, ed.NetworkName,
                   pc.OperatingSystem,
                   sw.BatteryPercentage
            FROM Device d
            LEFT JOIN EmbeddedDevice ed ON ed.Id = d.Id
            LEFT JOIN PersonalComputer pc ON pc.Id = d.Id
            LEFT JOIN Smartwatch sw ON sw.Id = d.Id", connection);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            devices.Add(MapDevice(reader));
        }

        return devices;
    }

    public async Task<Device?> GetDeviceByIdAsync(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

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

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapDevice(reader);
        }

        return null;
    }

    public async Task<int> AddDeviceAsync(Device device)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var insertDeviceCommand = new SqlCommand(@"
                INSERT INTO Device (Name, IsTurnedOn)
                OUTPUT INSERTED.Id
                VALUES (@Name, @IsTurnedOn)", connection, (SqlTransaction)transaction);

            insertDeviceCommand.Parameters.AddWithValue("@Name", device.Name);
            insertDeviceCommand.Parameters.AddWithValue("@IsTurnedOn", device.IsTurnedOn);

            var newId = (int)(await insertDeviceCommand.ExecuteScalarAsync())!;

            switch (device)
            {
                case EmbeddedDevice ed:
                    var insertEmbedded = new SqlCommand(@"
                        INSERT INTO EmbeddedDevice (Id, IpAddress, NetworkName)
                        VALUES (@Id, @IpAddress, @NetworkName)", connection, (SqlTransaction)transaction);
                    insertEmbedded.Parameters.AddWithValue("@Id", newId);
                    insertEmbedded.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
                    insertEmbedded.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                    await insertEmbedded.ExecuteNonQueryAsync();
                    break;

                case PersonalComputer pc:
                    var insertPC = new SqlCommand(@"
                        INSERT INTO PersonalComputer (Id, OperatingSystem)
                        VALUES (@Id, @OperatingSystem)", connection, (SqlTransaction)transaction);
                    insertPC.Parameters.AddWithValue("@Id", newId);
                    insertPC.Parameters.AddWithValue("@OperatingSystem", pc.OperatingSystem ?? (object)DBNull.Value);
                    await insertPC.ExecuteNonQueryAsync();
                    break;

                case Smartwatch sw:
                    var insertSW = new SqlCommand(@"
                        INSERT INTO Smartwatch (Id, BatteryPercentage)
                        VALUES (@Id, @BatteryPercentage)", connection, (SqlTransaction)transaction);
                    insertSW.Parameters.AddWithValue("@Id", newId);
                    insertSW.Parameters.AddWithValue("@BatteryPercentage", sw.BatteryPercentage);
                    await insertSW.ExecuteNonQueryAsync();
                    break;
            }

            await transaction.CommitAsync();
            return newId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateDeviceAsync(Device device)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var updateDeviceCommand = new SqlCommand(@"
                UPDATE Device
                SET Name = @Name, IsTurnedOn = @IsTurnedOn
                WHERE Id = @Id", connection, (SqlTransaction)transaction);

            updateDeviceCommand.Parameters.AddWithValue("@Id", device.Id);
            updateDeviceCommand.Parameters.AddWithValue("@Name", device.Name);
            updateDeviceCommand.Parameters.AddWithValue("@IsTurnedOn", device.IsTurnedOn);

            await updateDeviceCommand.ExecuteNonQueryAsync();

            switch (device)
            {
                case EmbeddedDevice ed:
                    var updateEmbedded = new SqlCommand(@"
                        UPDATE EmbeddedDevice
                        SET IpAddress = @IpAddress, NetworkName = @NetworkName
                        WHERE Id = @Id", connection, (SqlTransaction)transaction);
                    updateEmbedded.Parameters.AddWithValue("@Id", ed.Id);
                    updateEmbedded.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
                    updateEmbedded.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                    await updateEmbedded.ExecuteNonQueryAsync();
                    break;

                case PersonalComputer pc:
                    var updatePC = new SqlCommand(@"
                        UPDATE PersonalComputer
                        SET OperatingSystem = @OperatingSystem
                        WHERE Id = @Id", connection, (SqlTransaction)transaction);
                    updatePC.Parameters.AddWithValue("@Id", pc.Id);
                    updatePC.Parameters.AddWithValue("@OperatingSystem", pc.OperatingSystem ?? (object)DBNull.Value);
                    await updatePC.ExecuteNonQueryAsync();
                    break;

                case Smartwatch sw:
                    var updateSW = new SqlCommand(@"
                        UPDATE Smartwatch
                        SET BatteryPercentage = @BatteryPercentage
                        WHERE Id = @Id", connection, (SqlTransaction)transaction);
                    updateSW.Parameters.AddWithValue("@Id", sw.Id);
                    updateSW.Parameters.AddWithValue("@BatteryPercentage", sw.BatteryPercentage);
                    await updateSW.ExecuteNonQueryAsync();
                    break;
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteDeviceAsync(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // Delete from specific tables first
            var deleteEmbedded = new SqlCommand("DELETE FROM EmbeddedDevice WHERE Id = @Id", connection, (SqlTransaction)transaction);
            deleteEmbedded.Parameters.AddWithValue("@Id", id);
            await deleteEmbedded.ExecuteNonQueryAsync();

            var deletePC = new SqlCommand("DELETE FROM PersonalComputer WHERE Id = @Id", connection, (SqlTransaction)transaction);
            deletePC.Parameters.AddWithValue("@Id", id);
            await deletePC.ExecuteNonQueryAsync();

            var deleteSW = new SqlCommand("DELETE FROM Smartwatch WHERE Id = @Id", connection, (SqlTransaction)transaction);
            deleteSW.Parameters.AddWithValue("@Id", id);
            await deleteSW.ExecuteNonQueryAsync();

            // Finally delete from Device
            var deleteDevice = new SqlCommand("DELETE FROM Device WHERE Id = @Id", connection, (SqlTransaction)transaction);
            deleteDevice.Parameters.AddWithValue("@Id", id);
            await deleteDevice.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private Device MapDevice(SqlDataReader reader)
    {
        var id = reader.GetInt32(reader.GetOrdinal("Id"));
        var name = reader.

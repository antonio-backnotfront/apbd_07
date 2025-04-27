-- Insert sample data into Device
INSERT INTO Device (Id, Name, IsEnabled) VALUES
                                             ('dev-001', N'Embedded Sensor', 1),
                                             ('dev-002', N'Work Laptop', 1),
                                             ('dev-003', N'Fitness Smartwatch', 1);
GO

-- Insert sample data into Embedded
INSERT INTO Embedded (IpAddress, NetworkName, DeviceId) VALUES
    ('192.168.1.10', 'HomeNetwork', 'dev-001');
GO

-- Insert sample data into PersonalComputer
INSERT INTO PersonalComputer (OperationSystem, DeviceId) VALUES
    ('Windows 11', 'dev-002');
GO

-- Insert sample data into Smartwatch
INSERT INTO Smartwatch (BatteryPercentage, DeviceId) VALUES
    (85, 'dev-003');
GO
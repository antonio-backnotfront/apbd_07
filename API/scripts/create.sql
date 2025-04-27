USE s30252;

-- Create the Device table
CREATE TABLE Device (
                        Id VARCHAR(255) PRIMARY KEY,
                        Name NVARCHAR(255) NOT NULL,
                        IsEnabled BIT NOT NULL
);
GO

-- Create the Embedded table
CREATE TABLE Embedded (
                          Id INT PRIMARY KEY IDENTITY(1,1),
                          IpAddress VARCHAR(255) NOT NULL,
                          NetworkName VARCHAR(255) NOT NULL,
                          DeviceId VARCHAR(255) NOT NULL,
                          FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);
GO

-- Create the PersonalComputer table
CREATE TABLE PersonalComputer (
                                  Id INT PRIMARY KEY IDENTITY(1,1),
                                  OperationSystem VARCHAR(255),
                                  DeviceId VARCHAR(255) NOT NULL,
                                  FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);
GO

-- Create the Smartwatch table
CREATE TABLE Smartwatch (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            BatteryPercentage INT NOT NULL,
                            DeviceId VARCHAR(255) NOT NULL,
                            FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);
GO




-- select * from PersonalComputer
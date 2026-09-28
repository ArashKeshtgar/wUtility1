-- SchemaSyncDemo_Source: the reference ("desired") schema for the schema-sync demo.
-- Idempotent: drops and recreates its own tables. Runs on SQL Server and Azure SQL.
-- $(AppIdentity) = the web app's managed identity name on Azure, empty locally
-- (sqlcmd -v AppIdentity="" or web/tools/DbSetup).

IF OBJECT_ID('dbo.Appointments') IS NOT NULL DROP TABLE dbo.Appointments;
IF OBJECT_ID('dbo.Patients') IS NOT NULL DROP TABLE dbo.Patients;
GO

CREATE TABLE dbo.Patients (
    PatientId    int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FullName     nvarchar(150) NOT NULL,
    NationalCode nvarchar(20)  NOT NULL,
    BirthDate    date          NULL,
    Phone        nvarchar(20)  NULL
);

CREATE TABLE dbo.Appointments (
    AppointmentId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PatientId     int           NOT NULL,
    DoctorName    nvarchar(100) NOT NULL,
    ScheduledAt   datetime2     NOT NULL
);
GO

-- Read-only: the app only reads the reference schema.
DECLARE @app sysname = N'$(AppIdentity)', @q nvarchar(300);
IF @app <> N''
BEGIN
    SET @q = QUOTENAME(@app);
    IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @app)
        EXEC (N'CREATE USER ' + @q + N' FROM EXTERNAL PROVIDER');
    EXEC (N'ALTER ROLE db_datareader ADD MEMBER ' + @q);
END
GO

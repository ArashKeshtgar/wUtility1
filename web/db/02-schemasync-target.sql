-- SchemaSyncDemo_Target: a "site" database that has drifted from the source.
-- One of each difference SchemaDiffer reports: a missing table (Appointments),
-- a missing column (Patients.Phone), a type mismatch (NationalCode 10 vs 20)
-- and a nullability mismatch (BirthDate). The API's /api/demo/reset runs the
-- same statements (api/DemoMode.cs), so keep the two in step.

IF OBJECT_ID('dbo.Appointments') IS NOT NULL DROP TABLE dbo.Appointments;
IF OBJECT_ID('dbo.Patients') IS NOT NULL DROP TABLE dbo.Patients;
GO

CREATE TABLE dbo.Patients (
    PatientId    int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FullName     nvarchar(150) NOT NULL,
    NationalCode nvarchar(10)  NOT NULL,
    BirthDate    date          NOT NULL
);
GO

-- The app applies generated scripts and resets the demo here, so it needs
-- DDL rights, but only in this database.
DECLARE @app sysname = N'$(AppIdentity)', @q nvarchar(300);
IF @app <> N''
BEGIN
    SET @q = QUOTENAME(@app);
    IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @app)
        EXEC (N'CREATE USER ' + @q + N' FROM EXTERNAL PROVIDER');
    EXEC (N'ALTER ROLE db_datareader ADD MEMBER ' + @q);
    EXEC (N'ALTER ROLE db_ddladmin ADD MEMBER ' + @q);
END
GO

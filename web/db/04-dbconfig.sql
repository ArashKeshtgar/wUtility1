-- dbConfigDataBasesDemo: DatabasesForm's site registry (Tbl_dbConfigSetting)
-- and the web port's encrypted connection vault (SiteConnections). The vault
-- starts empty: its rows are encrypted with CONNECTION_VAULT_KEY, which a
-- plain SQL script doesn't have. Site names below are fictional.

IF OBJECT_ID('dbo.SiteConnections') IS NOT NULL DROP TABLE dbo.SiteConnections;
IF OBJECT_ID('dbo.Tbl_dbConfigSetting') IS NOT NULL DROP TABLE dbo.Tbl_dbConfigSetting;
GO

CREATE TABLE dbo.Tbl_dbConfigSetting (
    fldID                   int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    fldMainDataBase         nvarchar(50)  NOT NULL,
    fldDataBaseName         nvarchar(100) NOT NULL,
    fldDelphiConnectionName nvarchar(100) NULL,
    fldPersianDescDataBase  nvarchar(200) NULL,
    fldIsActive             bit           NOT NULL DEFAULT (1),
    fldInstanceName         nvarchar(100) NULL,
    fldSecondInstance       nvarchar(100) NULL
);

CREATE TABLE dbo.SiteConnections (
    Id                        int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ModuleName                nvarchar(50)  NOT NULL,
    DisplayName               nvarchar(150) NOT NULL,
    EncryptedConnectionString nvarchar(max) NOT NULL,
    CreatedAt                 datetime2     NOT NULL DEFAULT (sysutcdatetime())
);
GO

INSERT INTO dbo.Tbl_dbConfigSetting
    (fldMainDataBase, fldDataBaseName, fldDelphiConnectionName, fldPersianDescDataBase, fldIsActive, fldInstanceName, fldSecondInstance)
VALUES
    (N'Totalsystem', N'Totalsystem_ShafaHospital', N'ConnTotal1', N'بیمارستان شفا',             1, N'SHAFA-SQL01', NULL),
    (N'OpRoom',      N'OpRoom_DenaHospital',       N'ConnOR1',    N'اتاق عمل - بیمارستان دنا',  1, N'DENA-SQL01',  N'DENA-SQL02'),
    (N'Lab',         N'Lab_DenaHospital',          N'ConnLab1',   N'آزمایشگاه - بیمارستان دنا', 0, N'DENA-SQL01',  NULL);
GO

DECLARE @app sysname = N'$(AppIdentity)', @q nvarchar(300);
IF @app <> N''
BEGIN
    SET @q = QUOTENAME(@app);
    IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @app)
        EXEC (N'CREATE USER ' + @q + N' FROM EXTERNAL PROVIDER');
    EXEC (N'ALTER ROLE db_datareader ADD MEMBER ' + @q);
    EXEC (N'ALTER ROLE db_datawriter ADD MEMBER ' + @q);
END
GO

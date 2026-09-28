-- TotalsystemDemo: the tables DoctorsForm / DoctorsPriceGroupsForm use, with
-- made-up demo rows (no real doctors). Same shapes as the hospital Totalsystem DB.

IF OBJECT_ID('dbo.DoctorsPriceGroups') IS NOT NULL DROP TABLE dbo.DoctorsPriceGroups;
IF OBJECT_ID('dbo.Doctors') IS NOT NULL DROP TABLE dbo.Doctors;
IF OBJECT_ID('dbo.Works') IS NOT NULL DROP TABLE dbo.Works;
GO

CREATE TABLE dbo.Doctors (
    Code   int           NOT NULL PRIMARY KEY,
    fName  nvarchar(50)  NOT NULL,
    lName  nvarchar(50)  NOT NULL,
    Name   nvarchar(100) NOT NULL,
    PostNo nvarchar(20)  NULL
);

CREATE TABLE dbo.Works (
    Code int           NOT NULL PRIMARY KEY,
    Name nvarchar(100) NOT NULL
);

CREATE TABLE dbo.DoctorsPriceGroups (
    RowGD            int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    DrCode           int            NOT NULL REFERENCES dbo.Doctors (Code),
    WCode            int            NULL     REFERENCES dbo.Works (Code),
    CenterSharePrice decimal(12,2)  NULL,
    Title            nvarchar(150)  NULL
);
GO

INSERT INTO dbo.Doctors (Code, fName, lName, Name, PostNo) VALUES
    (101, N'سارا',  N'احمدی', N'دکتر سارا احمدی',  N'P-101'),
    (102, N'رضا',   N'کریمی', N'دکتر رضا کریمی',   N'P-102'),
    (103, N'مریم',  N'رحیمی', N'دکتر مریم رحیمی',  N'P-103');

INSERT INTO dbo.Works (Code, Name) VALUES
    (1, N'ویزیت'),
    (2, N'جراحی سرپایی'),
    (3, N'سونوگرافی');

-- Same rows as DemoResetService.ResetPriceGroupsSql (api/DemoMode.cs).
INSERT INTO dbo.DoctorsPriceGroups (DrCode, WCode, CenterSharePrice, Title) VALUES
    (101, 1, 350000.00,  N'ویزیت عمومی'),
    (101, 2, 1200000.00, N'جراحی سرپایی'),
    (102, 3, 500000.00,  N'سونوگرافی شکم'),
    (101, 3, 750000.00,  N'سونوگرافی تخصصی');
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

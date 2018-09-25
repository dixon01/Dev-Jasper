/*
	This script creates the PerformanceLogger database needed for the Center application to save information about performance.
	
	*** WARNING *************************************************************************
	*
	* The script drops the current database and creates a new one. All data will be lost!
	*
	*************************************************************************************
	
	Before running, ensure that you have right permissions on the 'master' database (CREATE LOGIN, DROP AND CREATE DATABASE).
*/
USE [master]
GO

/* Dropping the database, if it already exists */
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'PerformanceLogger')
DROP DATABASE [PerformanceLogger]
GO

/* Variable containing the base data path */
DECLARE @dataPath varchar(255) = 'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\'


DECLARE @databasePath nvarchar(255) = @dataPath + 'PerformanceLogger.mdf'
DECLARE @databaseLogPath nvarchar(255) = @dataPath + 'PerformanceLogger_log.ldf'
DECLARE @sql nvarchar(MAX)

/****** Object:  Database [PerformanceLogger]    Script Date: 01/29/2013 09:48:18 ******/
SET @sql = 'CREATE DATABASE [PerformanceLogger] ON  PRIMARY
( NAME = N''PerformanceLogger'', FILENAME = ''' + @databasePath + ''', SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N''PerformanceLogger_log'', FILENAME = ''' + @databaseLogPath + ''', SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)'
EXEC(@sql)
GO

ALTER DATABASE [PerformanceLogger] SET COMPATIBILITY_LEVEL = 100
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PerformanceLogger].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [PerformanceLogger] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [PerformanceLogger] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [PerformanceLogger] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [PerformanceLogger] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [PerformanceLogger] SET ARITHABORT OFF 
GO

ALTER DATABASE [PerformanceLogger] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [PerformanceLogger] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [PerformanceLogger] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [PerformanceLogger] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [PerformanceLogger] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [PerformanceLogger] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [PerformanceLogger] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [PerformanceLogger] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [PerformanceLogger] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [PerformanceLogger] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [PerformanceLogger] SET  DISABLE_BROKER 
GO

ALTER DATABASE [PerformanceLogger] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [PerformanceLogger] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [PerformanceLogger] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [PerformanceLogger] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [PerformanceLogger] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [PerformanceLogger] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [PerformanceLogger] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [PerformanceLogger] SET  READ_WRITE 
GO

ALTER DATABASE [PerformanceLogger] SET RECOVERY FULL 
GO

ALTER DATABASE [PerformanceLogger] SET  MULTI_USER 
GO

ALTER DATABASE [PerformanceLogger] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [PerformanceLogger] SET DB_CHAINING OFF 
GO

USE [master]
GO

/*
	Verifying login for the AppPool BackgroundSystem. Creating it if it doesn't exist.
*/
IF NOT EXISTS (SELECT loginname FROM master.dbo.syslogins WHERE name = 'IIS APPPool\BackgroundSystem' AND dbname = 'master')
	BEGIN
		CREATE LOGIN [IIS APPPOOL\BackgroundSystem] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
	END
GO

USE [PerformanceLogger]
GO

/* Verifying user for the AppPool BackgroundSystem */
IF NOT EXISTS(SELECT * FROM sys.database_principals WHERE name = 'BackgroundSystem')
	BEGIN
		CREATE USER [BackgroundSystem] FOR LOGIN [IIS APPPOOL\BackgroundSystem]
	END
GO

/* Creating tables */
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sessions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[Machine] [varchar](160) COLLATE Latin1_General_CI_AS NOT NULL,
	[Description] [varchar](500) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
USE [PerformanceLogger]
GO
/****** Object:  Table [dbo].[Markers]    Script Date: 01/29/2013 09:32:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Markers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SessionId] [int] NOT NULL,
	[Category] [varchar](160) COLLATE Latin1_General_CI_AS NOT NULL,
	[Tag] [varchar](160) COLLATE Latin1_General_CI_AS NULL,
	[MarkerId] [int] NOT NULL,
	[TickCount] [bigint] NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[Unit] [varchar](160) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK_Markers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
/****** Object:  StoredProcedure [dbo].[ClearSession]    Script Date: 01/29/2013 09:32:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 29.01.13
-- Description:	Clears the markers of a specified session and optionally deletes the session itself.
-- =============================================
CREATE PROCEDURE [dbo].[ClearSession]
	@SessionId int,
	@DeleteSession bit = 0
AS
BEGIN
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM [Markers] WHERE [SessionId] = @SessionId
	IF @DeleteSession = 1
		BEGIN
			DELETE FROM [Sessions] WHERE [Id] = @SessionId
		END
END

GO
/****** Object:  StoredProcedure [dbo].[CreateSession]    Script Date: 01/29/2013 09:32:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 29.01.13
-- Description:	Creates a new session returning (and selecting) the identifier assigned to the object created.
-- =============================================
CREATE PROCEDURE [dbo].[CreateSession] 
	-- Add the parameters for the stored procedure here
	@MachineName varchar(160),
	@Description varchar(500) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [PerformanceLogger].[dbo].[Sessions]
           ([Machine]
           ,[Description])
     VALUES
           (@MachineName
           , @Description)
     DECLARE @id int = SCOPE_IDENTITY()
     SELECT @id
     
     PRINT 'Created session with id ' + convert(varchar(10), @id)
     
     RETURN @id
END

GO
/****** Object:  StoredProcedure [dbo].[AddMarker]    Script Date: 01/29/2013 09:32:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 29.01.13
-- Description:	Adds a marker.
-- =============================================
CREATE PROCEDURE [dbo].[AddMarker]
	@SessionId int,
	@Category varchar(160) = NULL,
	@Tag varchar(160) = NULL,
	@MarkerId int,
	@TickCount bigint,
	@Unit varchar(160) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [PerformanceLogger].[dbo].[Markers]
           ([SessionId]
           , [Category]
           , [Tag]
           ,[MarkerId]
           ,[TickCount]
           , [Unit])
     VALUES
           (@SessionId
           , @Category
           , @Tag
           , @MarkerId
           , @TickCount
           , @Unit)

END

GO
GRANT EXECUTE ON [dbo].[AddMarker] TO [BackgroundSystem] AS [dbo]
GO
ALTER TABLE [dbo].[Sessions] ADD  CONSTRAINT [DF_Sessions_StartDate]  DEFAULT (getutcdate()) FOR [StartDate]
GO
ALTER TABLE [dbo].[Markers]  WITH CHECK ADD  CONSTRAINT [FK_Markers_Sessions] FOREIGN KEY([SessionId])
REFERENCES [Sessions] ([Id])
GO
ALTER TABLE [dbo].[Markers] CHECK CONSTRAINT [FK_Markers_Sessions]
GO

ALTER TABLE [dbo].[Markers] ADD  CONSTRAINT [DF_Markers_Date]  DEFAULT (getutcdate()) FOR [Date]
GO
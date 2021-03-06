USE [master]
GO

USE [master]

IF NOT EXISTS(SELECT name FROM sys.sql_logins WHERE name = 'gorba_center_controllers_metabase')
BEGIN
    CREATE LOGIN [gorba_center_controllers_metabase] WITH PASSWORD=N'gorba', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
END



USE [master]
GO
/****** Object:  Database [Gorba_CenterControllersMetabase]    Script Date: 09/01/2012 21:51:48 ******/
CREATE DATABASE [Gorba_CenterControllersMetabase]
 COLLATE Latin1_General_CI_AS
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Gorba_CenterControllersMetabase].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET ARITHABORT OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET  READ_WRITE 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET RECOVERY FULL 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET  MULTI_USER 
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET DB_CHAINING OFF 
GO
USE [Gorba_CenterControllersMetabase]
GO
/****** Object:  DatabaseRole [db_executor]    Script Date: 09/01/2012 21:51:48 ******/
CREATE ROLE [db_executor] AUTHORIZATION [dbo]
GO
/****** Object:  User [gorba_center_controllers_metabase]    Script Date: 09/01/2012 21:51:48 ******/
GO
CREATE USER [gorba_center_controllers_metabase] FOR LOGIN [gorba_center_controllers_metabase] WITH DEFAULT_SCHEMA=[dbo]
GO
sys.sp_addrolemember @rolename = N'db_executor', @membername = N'gorba_center_controllers_metabase'
GO
sys.sp_addrolemember @rolename = N'db_datareader', @membername = N'gorba_center_controllers_metabase'
GO
sys.sp_addrolemember @rolename = N'db_datawriter', @membername = N'gorba_center_controllers_metabase'
GO
GRANT EXECUTE TO [db_executor] AS [dbo]
GO
GRANT CONNECT TO [gorba_center_controllers_metabase] AS [dbo]
GO
USE [Gorba_CenterControllersMetabase]
GO
/****** Object:  Table [dbo].[ActivityInstanceControllerSet]    Script Date: 09/01/2012 21:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActivityInstanceControllerSet](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [InstanceId] [uniqueidentifier] NOT NULL,
    [ActivityInstanceId] int NOT NULL,
    [UnitId] [int] NOT NULL,
    [ActivityId] [int] NOT NULL,
    [CurrentTaskIdentifier] [uniqueidentifier] NULL,
    [CreationTime] [datetime2](7) NOT NULL,
    [LastUpdated] [datetime2](7) NULL,
    [IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ActivityInstanceControllerSet] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO

USE [Gorba_CenterControllersMetabase]
/****** Object:  Index [IX_ActivityInstanceController_InstanceId]    Script Date: 09/01/2012 21:51:55 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ActivityInstanceController_InstanceId] ON [dbo].[ActivityInstanceControllerSet] 
(
    [InstanceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
GO

USE [Gorba_CenterControllersMetabase]
/****** Object:  Index [IX_ActivityInstanceController_InstanceId_IsDeleted]    Script Date: 09/01/2012 21:51:55 ******/
CREATE NONCLUSTERED INDEX [IX_ActivityInstanceController_InstanceId_IsDeleted] ON [dbo].[ActivityInstanceControllerSet] 
(
    [InstanceId] ASC,
    [IsDeleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
GO

USE [Gorba_CenterControllersMetabase]
GO
/****** Object:  Table [dbo].[ActivityTaskLifecycleSet]    Script Date: 09/01/2012 21:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActivityTaskLifecycleSet](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [InstanceId] [uniqueidentifier] NOT NULL,
    [ActivityInstanceId] int NOT NULL,
    [ActivityTaskId] [bigint] NOT NULL,
    [UnitId] [int] NOT NULL,
    [ActivityId] [int] NOT NULL,
    [CreationTime] [datetime2](7) NOT NULL,
    [LastUpdated] [datetime2](7) NULL,
    [IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ActivityTaskLifecycleSet] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO

USE [Gorba_CenterControllersMetabase]
/****** Object:  Index [IX_ActivityTaskLifecycle_InstanceId]    Script Date: 09/01/2012 21:51:55 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ActivityTaskLifecycle_InstanceId] ON [dbo].[ActivityTaskLifecycleSet] 
(
    [InstanceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
GO

USE [Gorba_CenterControllersMetabase]
/****** Object:  Index [IX_ActivityTaskLifecycle_InstanceId_IsDeleted]    Script Date: 09/01/2012 21:51:55 ******/
CREATE NONCLUSTERED INDEX [IX_ActivityTaskLifecycle_InstanceId_IsDeleted] ON [dbo].[ActivityTaskLifecycleSet] 
(
    [InstanceId] ASC,
    [IsDeleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
GO
/****** Object:  View [dbo].[ActivityInstanceControllers]    Script Date: 09/01/2012 21:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ActivityInstanceControllers]
AS
SELECT [Id]
      ,[InstanceId]
      ,[UnitId]
      ,[ActivityId]
      ,[CurrentTaskIdentifier]
      ,[CreationTime]
      ,[LastUpdated]
  FROM [Gorba_CenterControllersMetabase].[dbo].[ActivityInstanceControllerSet]
  WHERE [IsDeleted] = 0




GO
/****** Object:  View [dbo].[ActivityTaskLifecycles]    Script Date: 09/01/2012 21:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ActivityTaskLifecycles]
AS
SELECT [Id]
      ,[InstanceId]
      ,[ActivityTaskId]
      ,[UnitId]
      ,[ActivityId]
      ,[CreationTime]
      ,[LastUpdated]
  FROM [Gorba_CenterControllersMetabase].[dbo].[ActivityTaskLifecycleSet]
  WHERE [IsDeleted] = 0


GO
/****** Object:  StoredProcedure [dbo].[AddOrUpdateActivityInstanceController]    Script Date: 09/01/2012 21:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[AddOrUpdateActivityInstanceController] 
    -- Add the parameters for the stored procedure here
    (
        @instanceId uniqueidentifier,
        @currentTaskIdentifier uniqueidentifier,
        @unitId int,
        @activityId int
    )
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    BEGIN TRY
        BEGIN TRANSACTION [Tx_AddOrUpdateAIC]
        
        DECLARE @id TABLE ([Id] int)
        UPDATE [Gorba_CenterControllersMetabase].[dbo].[ActivityInstanceControllerSet]
        SET [CurrentTaskIdentifier] = @currentTaskIdentifier
            ,[LastUpdated] = GETUTCDATE()
        OUTPUT inserted.Id INTO @id
        WHERE [InstanceId] = @instanceId
        
        IF @@ROWCOUNT = 0
            BEGIN
                INSERT INTO [Gorba_CenterControllersMetabase].[dbo].[ActivityInstanceControllerSet]
                ([InstanceId], [UnitId], [ActivityId], [CurrentTaskIdentifier])
                OUTPUT inserted.Id INTO @id
                VALUES
                (@instanceId, @unitId, @activityId, @currentTaskIdentifier)
            END
        
        SELECT [Id]
        FROM @id
         
        COMMIT TRANSACTION [Tx_AddOrUpdateAIC]
    END TRY
     
    BEGIN CATCH
        IF @@TRANCOUNT < 0
            BEGIN
                ROLLBACK TRANSACTION [Tx_AddOrUpdateAIC] --RollBack in case of Error
            END
     
        -- Raise an error with the details of the exception
        DECLARE @ErrMsg nvarchar(4000), @ErrSeverity INT
        SELECT @ErrMsg = ERROR_MESSAGE(),
        @ErrSeverity = ERROR_SEVERITY()
     
        RAISERROR(@ErrMsg, @ErrSeverity, 1)
     
    END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[AddOrUpdateActivityTaskLifecycle]    Script Date: 09/01/2012 21:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[AddOrUpdateActivityTaskLifecycle] 
    -- Add the parameters for the stored procedure here
    (
        @instanceId uniqueidentifier,
        @activityTaskId bigint,
        @unitId int,
        @activityId int
    )
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    BEGIN TRY
        BEGIN TRANSACTION [Tx_AddOrUpdateATL]
        
        DECLARE @id TABLE ([Id] int, [ActivityTaskId] bigint)
        
        UPDATE [Gorba_CenterControllersMetabase].[dbo].[ActivityTaskLifecycleSet]
        SET [ActivityTaskId] = @activityTaskId
            ,[LastUpdated] = GETUTCDATE()
            OUTPUT inserted.[Id], @activityTaskId INTO @id
        WHERE [InstanceId] = @instanceId
        
        IF @@ROWCOUNT = 0
            BEGIN
                INSERT INTO [Gorba_CenterControllersMetabase].[dbo].[ActivityTaskLifecycleSet]
                ([InstanceId], [ActivityTaskId], [UnitId], [ActivityId])
                OUTPUT inserted.[Id], inserted.[Id] + @activityTaskId INTO @id
                VALUES
                (@instanceId, 0, @unitId, @activityId)
                
                
                SELECT @activityTaskId = [ActivityTaskId]
                FROM @id
                
                UPDATE [Gorba_CenterControllersMetabase].[dbo].[ActivityTaskLifecycleSet]
                SET [ActivityTaskId] = @activityTaskId
                WHERE [InstanceId] = @instanceId
            END
        
        
        UPDATE @id
        SET [ActivityTaskId] = @activityTaskId
        
        SELECT [Id], [ActivityTaskId]
        FROM @id
         
        COMMIT TRANSACTION [Tx_AddOrUpdateATL]
    END TRY
     
    BEGIN CATCH
        IF @@TRANCOUNT < 0
            BEGIN
                ROLLBACK TRANSACTION [Tx_AddOrUpdateATL] --RollBack in case of Error
            END
     
        -- Raise an error with the details of the exception
        DECLARE @ErrMsg nvarchar(4000), @ErrSeverity INT
        SELECT @ErrMsg = ERROR_MESSAGE(),
        @ErrSeverity = ERROR_SEVERITY()
     
        RAISERROR(@ErrMsg, @ErrSeverity, 1)
     
    END CATCH
END


GO
/****** Object:  StoredProcedure [dbo].[ClearControllers]    Script Date: 09/01/2012 21:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE ClearControllers
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    DELETE FROM [ActivityTaskLifecycleSet]
    DELETE FROM [ActivityInstanceControllerSet]
END


GO
/****** Object:  StoredProcedure [dbo].[FindCurrentActivityTaskIdentifier]    Script Date: 09/01/2012 21:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindCurrentActivityTaskIdentifier]
(
    @unitId int,
    @activityId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    DECLARE @currentTaskIdentifier uniqueidentifier
    
    SELECT @currentTaskIdentifier = [CurrentTaskIdentifier]
    FROM [Gorba_CenterControllersMetabase].[dbo].[ActivityInstanceControllerSet]
    WHERE [UnitId] = @unitId AND [ActivityId] = @activityId
    
    SELECT @currentTaskIdentifier

END

GO
/****** Object:  StoredProcedure [dbo].[GetActivityInstanceController]    Script Date: 09/01/2012 21:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE GetActivityInstanceController
(
    @instanceId uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT [C].[Id], [C].[InstanceId], [C].[UnitId], [C].[ActivityId], [C].[CurrentTaskIdentifier], [C].[CreationTime], [C].[LastUpdated]
  FROM [Gorba_CenterControllersMetabase].[dbo].[ActivityInstanceControllerSet] [C]
  WHERE [C].[InstanceId] = @instanceId
  
END

GO
/****** Object:  StoredProcedure [dbo].[GetActivityTaskLifecycleInstanceId]    Script Date: 09/01/2012 21:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetActivityTaskLifecycleInstanceId] 
    -- Add the parameters for the stored procedure here
    (
        @activityTaskId bigint
    )
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    DECLARE @instanceId uniqueidentifier
    
    SELECT @instanceId = [InstanceId]
    FROM [Gorba_CenterControllersMetabase].[dbo].[ActivityTaskLifecycleSet]
    WHERE [ActivityTaskId] = @activityTaskId
    
    SELECT @instanceId


END

GO
/****** Object:  StoredProcedure [dbo].[GetActivityTaskLifecycleInstanceId]    Script Date: 09/01/2012 21:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetActivityTaskLifecycleInstanceIds] 
    -- Add the parameters for the stored procedure here
    (
        @unitId int
    )
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT [InstanceId]
    FROM [Gorba_CenterControllersMetabase].[dbo].[ActivityTaskLifecycleSet]
    WHERE [UnitId] = @unitId AND [IsDeleted] = 0
    
END

GO
/****** Object:  StoredProcedure [dbo].[RemoveActivityInstanceController]    Script Date: 09/01/2012 21:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[RemoveActivityInstanceController]
(
    @instanceId uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    BEGIN TRY
    BEGIN TRANSACTION [TX_RemoveAIC]
     
        /* logic here */
        UPDATE [Gorba_CenterControllersMetabase].[dbo].[ActivityInstanceControllerSet]
        SET [IsDeleted] = 1
        WHERE [InstanceId] = @instanceId
     
    COMMIT TRANSACTION [TX_RemoveAIC]
END TRY
 
    BEGIN CATCH
        IF @@TRANCOUNT < 0
            BEGIN
                ROLLBACK TRANSACTION [TX_RemoveAIC] --RollBack in case of Error
            END
     
        -- Raise an error with the details of the exception
        DECLARE @ErrMsg nvarchar(4000), @ErrSeverity INT
        SELECT @ErrMsg = ERROR_MESSAGE(),
        @ErrSeverity = ERROR_SEVERITY()
     
        RAISERROR(@ErrMsg, @ErrSeverity, 1)
 
    END CATCH
    
END

GO
/****** Object:  StoredProcedure [dbo].[RemoveActivityTaskLifecycle]    Script Date: 09/01/2012 21:51:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[RemoveActivityTaskLifecycle]
(
    @instanceId uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    BEGIN TRY
    BEGIN TRANSACTION [TX_RemoveATL]
     
        /* logic here */
        UPDATE [Gorba_CenterControllersMetabase].[dbo].[ActivityTaskLifecycleSet]
        SET [IsDeleted] = 1
        WHERE [InstanceId] = @instanceId
     
    COMMIT TRANSACTION [TX_RemoveATL]
END TRY
 
    BEGIN CATCH
        IF @@TRANCOUNT < 0
            BEGIN
                ROLLBACK TRANSACTION [TX_RemoveATL] --RollBack in case of Error
            END
     
        -- Raise an error with the details of the exception
        DECLARE @ErrMsg nvarchar(4000), @ErrSeverity INT
        SELECT @ErrMsg = ERROR_MESSAGE(),
        @ErrSeverity = ERROR_SEVERITY()
     
        RAISERROR(@ErrMsg, @ErrSeverity, 1)
 
    END CATCH
    
END

GO
ALTER TABLE [dbo].[ActivityInstanceControllerSet] ADD  CONSTRAINT [DF_ActivityInstanceControllerSet_CreationTime]  DEFAULT (getutcdate()) FOR [CreationTime]
GO
ALTER TABLE [dbo].[ActivityInstanceControllerSet] ADD  CONSTRAINT [DF_ActivityInstanceControllerSet_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ActivityTaskLifecycleSet]  WITH CHECK ADD  CONSTRAINT [FK_ActivityTaskLifecycleSet_ActivityTaskLifecycleSet] FOREIGN KEY([Id])
REFERENCES [ActivityTaskLifecycleSet] ([Id])
GO
ALTER TABLE [dbo].[ActivityTaskLifecycleSet] CHECK CONSTRAINT [FK_ActivityTaskLifecycleSet_ActivityTaskLifecycleSet]
GO
ALTER TABLE [dbo].[ActivityTaskLifecycleSet] ADD  CONSTRAINT [DF_ActivityTaskLifecycleSet_CreationTime]  DEFAULT (getutcdate()) FOR [CreationTime]
GO
ALTER TABLE [dbo].[ActivityTaskLifecycleSet] ADD  CONSTRAINT [DF_ActivityTaskLifecycleSet_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

/****** Object:  Table [dbo].[DatabaseVersionSet]    Script Date: 11/28/2012 07:24:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[DatabaseVersionSet](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [varchar](100) NOT NULL,
    [Description] [varchar](500) NULL,
    [VersionMajor] [int] NOT NULL,
    [VersionMinor] [int] NOT NULL,
    [VersionBuild] [int] NOT NULL,
    [VersionRevision] [int] NOT NULL,
    [DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_DatabaseVersionSet] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  View [dbo].[DatabaseVersions]    Script Date: 11/28/2012 07:25:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[DatabaseVersions]
AS
SELECT     Id, Name, VersionMajor, VersionMinor, VersionBuild, VersionRevision, Description, DateCreated
FROM         dbo.DatabaseVersionSet

GO

USE [Gorba_CenterControllersMetabase] 
GO

/****** Object:  StoredProcedure [dbo].[DatabaseVersion_Insert]    Script Date: 11/28/2012 07:32:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 02.12.11 10:47
-- Description:	Inserts a new version of the database in the DatabaseVersionSet table.
--				Everything is done within a transaction.
--				If the @dateCreated parameter is NULL, then the actual UTC time is used.
--				If the operations succeeds, the identifier assigned to the created row is selected
-- =============================================
CREATE PROCEDURE [dbo].[DatabaseVersion_Insert]
(
    @name varchar(100),
    @description varchar(500) = NULL,
    @versionMajor int,
    @versionMinor int,
    @versionBuild int,
    @versionRevision int,
    @dateCreated datetime2 = NULL
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    BEGIN TRY --Start the Try Block..
        BEGIN TRANSACTION DatabaseVersion_InsertTx -- Start the transaction..
            IF @dateCreated IS NULL
                BEGIN
                    SET @dateCreated = GETUTCDATE()
                END
            INSERT INTO [DatabaseVersionSet]
            ([Name], [Description], [VersionMajor], [VersionMinor], [VersionBuild], [VersionRevision], [DateCreated])
            VALUES
            (@name, @description, @versionMajor, @versionMinor, @versionBuild, @versionRevision, @dateCreated)
            
            DECLARE @id int = SCOPE_IDENTITY()
            SELECT @id
            
            COMMIT TRAN -- Transaction Success!

    END TRY

    BEGIN CATCH


        IF @@TRANCOUNT > 0
            BEGIN
                ROLLBACK TRAN --RollBack in case of Error
            END
            
            DECLARE @message varchar(500) = ERROR_MESSAGE()
            DECLARE @severity int = ERROR_SEVERITY()
            RAISERROR(@message, @severity, 1)

    END CATCH

END

GO

ALTER DATABASE [Gorba_CenterControllersMetabase] SET allow_snapshot_isolation ON
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET READ_COMMITTED_SNAPSHOT ON;
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterControllersMetabase].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 0.99.26.1'
  ,@description = 'Added database version management. Added ActivityInstanceId columns to ActivityInstanceControllerSet and ActivityTaskLifecycleSet. Removed unique index IX_ActivityInstanceController_UnitId_ActivityId_IsDeleted.'
  ,@versionMajor = 0
  ,@versionMinor = 99
  ,@versionBuild = 26
  ,@versionRevision = 1
  ,@dateCreated = '2012-11-28 05:30:00'
GO

EXECUTE @RC = [Gorba_CenterControllersMetabase].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.1.29.2'
  ,@description = 'Refactoring for the code quality. Removed unused stored procedures.'
  ,@versionMajor = 1
  ,@versionMinor = 1
  ,@versionBuild = 29
  ,@versionRevision = 2
  ,@dateCreated = '2013-01-28 14:30:00'
GO

EXECUTE @RC = [Gorba_CenterControllersMetabase].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.5.201410.3'
  ,@description = 'Updated snapshot isolation settings.'
  ,@versionMajor = 1
  ,@versionMinor = 5
  ,@versionBuild = 201410
  ,@versionRevision = 3
  ,@dateCreated = '2014-05-19T09:00:00.000'
GO


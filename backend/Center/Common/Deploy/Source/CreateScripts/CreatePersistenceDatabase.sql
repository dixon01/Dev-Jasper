USE [master]
GO

USE [master]

IF NOT EXISTS(SELECT name FROM sys.sql_logins WHERE name = 'gorba_center_controllers')
BEGIN
    CREATE LOGIN [gorba_center_controllers] WITH PASSWORD=N'gorba', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
END



USE [master]
GO
/****** Object:  Database [Gorba_CenterControllers]    Script Date: 08/23/2012 08:19:42 ******/
CREATE DATABASE [Gorba_CenterControllers]
 COLLATE SQL_Latin1_General_CP1_CI_AS
GO
ALTER DATABASE [Gorba_CenterControllers] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Gorba_CenterControllers].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Gorba_CenterControllers] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET ARITHABORT OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Gorba_CenterControllers] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Gorba_CenterControllers] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Gorba_CenterControllers] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Gorba_CenterControllers] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Gorba_CenterControllers] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Gorba_CenterControllers] SET  READ_WRITE 
GO
ALTER DATABASE [Gorba_CenterControllers] SET RECOVERY FULL 
GO
ALTER DATABASE [Gorba_CenterControllers] SET  MULTI_USER 
GO
ALTER DATABASE [Gorba_CenterControllers] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Gorba_CenterControllers] SET DB_CHAINING OFF 
GO
USE [Gorba_CenterControllers]
GO
/****** Object:  DatabaseRole [System.Activities.DurableInstancing.InstanceStoreObservers]    Script Date: 08/23/2012 08:19:42 ******/
CREATE ROLE [System.Activities.DurableInstancing.InstanceStoreObservers] AUTHORIZATION [dbo]
GO
/****** Object:  DatabaseRole [System.Activities.DurableInstancing.InstanceStoreUsers]    Script Date: 08/23/2012 08:19:42 ******/
CREATE ROLE [System.Activities.DurableInstancing.InstanceStoreUsers] AUTHORIZATION [dbo]
GO
/****** Object:  DatabaseRole [System.Activities.DurableInstancing.WorkflowActivationUsers]    Script Date: 08/23/2012 08:19:42 ******/
CREATE ROLE [System.Activities.DurableInstancing.WorkflowActivationUsers] AUTHORIZATION [dbo]
GO
/****** Object:  Schema [System.Activities.DurableInstancing]    Script Date: 08/23/2012 08:19:42 ******/
CREATE SCHEMA [System.Activities.DurableInstancing] AUTHORIZATION [dbo]
GO
/****** Object:  DatabaseRole [db_executor]    Script Date: 08/23/2012 08:19:42 ******/
CREATE ROLE [db_executor] AUTHORIZATION [dbo]
GO
/****** Object:  User [gorba_center_controllers]    Script Date: 08/23/2012 08:19:42 ******/
GO
CREATE USER [gorba_center_controllers] FOR LOGIN [gorba_center_controllers] WITH DEFAULT_SCHEMA=[dbo]
GO
sys.sp_addrolemember @rolename = N'db_executor', @membername = N'gorba_center_controllers'
GO
sys.sp_addrolemember @rolename = N'db_datareader', @membername = N'gorba_center_controllers'
GO
sys.sp_addrolemember @rolename = N'db_datawriter', @membername = N'gorba_center_controllers'
GO
GRANT EXECUTE TO [db_executor] AS [dbo]
GO
GRANT CONNECT TO [gorba_center_controllers] AS [dbo]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[InstanceMetadataChangesTable]    Script Date: 08/23/2012 08:19:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[InstanceMetadataChangesTable](
	[SurrogateInstanceId] [bigint] NOT NULL,
	[ChangeTime] [bigint] IDENTITY(1,1) NOT NULL,
	[EncodingOption] [tinyint] NOT NULL,
	[Change] [varbinary](max) NOT NULL
)

GO

USE [Gorba_CenterControllers]
/****** Object:  Index [CIX_InstanceMetadataChangesTable]    Script Date: 08/23/2012 08:19:51 ******/
CREATE UNIQUE CLUSTERED INDEX [CIX_InstanceMetadataChangesTable] ON [System.Activities.DurableInstancing].[InstanceMetadataChangesTable] 
(
	[SurrogateInstanceId] ASC,
	[ChangeTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
GO
USE [Gorba_CenterControllers]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable]    Script Date: 08/23/2012 08:19:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable](
	[SurrogateInstanceId] [bigint] NOT NULL,
	[PromotionName] [nvarchar](400) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Value1] [sql_variant] NULL,
	[Value2] [sql_variant] NULL,
	[Value3] [sql_variant] NULL,
	[Value4] [sql_variant] NULL,
	[Value5] [sql_variant] NULL,
	[Value6] [sql_variant] NULL,
	[Value7] [sql_variant] NULL,
	[Value8] [sql_variant] NULL,
	[Value9] [sql_variant] NULL,
	[Value10] [sql_variant] NULL,
	[Value11] [sql_variant] NULL,
	[Value12] [sql_variant] NULL,
	[Value13] [sql_variant] NULL,
	[Value14] [sql_variant] NULL,
	[Value15] [sql_variant] NULL,
	[Value16] [sql_variant] NULL,
	[Value17] [sql_variant] NULL,
	[Value18] [sql_variant] NULL,
	[Value19] [sql_variant] NULL,
	[Value20] [sql_variant] NULL,
	[Value21] [sql_variant] NULL,
	[Value22] [sql_variant] NULL,
	[Value23] [sql_variant] NULL,
	[Value24] [sql_variant] NULL,
	[Value25] [sql_variant] NULL,
	[Value26] [sql_variant] NULL,
	[Value27] [sql_variant] NULL,
	[Value28] [sql_variant] NULL,
	[Value29] [sql_variant] NULL,
	[Value30] [sql_variant] NULL,
	[Value31] [sql_variant] NULL,
	[Value32] [sql_variant] NULL,
	[Value33] [varbinary](max) NULL,
	[Value34] [varbinary](max) NULL,
	[Value35] [varbinary](max) NULL,
	[Value36] [varbinary](max) NULL,
	[Value37] [varbinary](max) NULL,
	[Value38] [varbinary](max) NULL,
	[Value39] [varbinary](max) NULL,
	[Value40] [varbinary](max) NULL,
	[Value41] [varbinary](max) NULL,
	[Value42] [varbinary](max) NULL,
	[Value43] [varbinary](max) NULL,
	[Value44] [varbinary](max) NULL,
	[Value45] [varbinary](max) NULL,
	[Value46] [varbinary](max) NULL,
	[Value47] [varbinary](max) NULL,
	[Value48] [varbinary](max) NULL,
	[Value49] [varbinary](max) NULL,
	[Value50] [varbinary](max) NULL,
	[Value51] [varbinary](max) NULL,
	[Value52] [varbinary](max) NULL,
	[Value53] [varbinary](max) NULL,
	[Value54] [varbinary](max) NULL,
	[Value55] [varbinary](max) NULL,
	[Value56] [varbinary](max) NULL,
	[Value57] [varbinary](max) NULL,
	[Value58] [varbinary](max) NULL,
	[Value59] [varbinary](max) NULL,
	[Value60] [varbinary](max) NULL,
	[Value61] [varbinary](max) NULL,
	[Value62] [varbinary](max) NULL,
	[Value63] [varbinary](max) NULL,
	[Value64] [varbinary](max) NULL
)

GO

USE [Gorba_CenterControllers]
/****** Object:  Index [CIX_InstancePromotedPropertiesTable]    Script Date: 08/23/2012 08:19:51 ******/
CREATE UNIQUE CLUSTERED INDEX [CIX_InstancePromotedPropertiesTable] ON [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable] 
(
	[SurrogateInstanceId] ASC,
	[PromotionName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO
USE [Gorba_CenterControllers]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[InstancesTable]    Script Date: 08/23/2012 08:19:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[InstancesTable](
	[Id] [uniqueidentifier] NOT NULL,
	[SurrogateInstanceId] [bigint] IDENTITY(1,1) NOT NULL,
	[SurrogateLockOwnerId] [bigint] NULL,
	[PrimitiveDataProperties] [varbinary](max) NULL,
	[ComplexDataProperties] [varbinary](max) NULL,
	[WriteOnlyPrimitiveDataProperties] [varbinary](max) NULL,
	[WriteOnlyComplexDataProperties] [varbinary](max) NULL,
	[MetadataProperties] [varbinary](max) NULL,
	[DataEncodingOption] [tinyint] NULL,
	[MetadataEncodingOption] [tinyint] NULL,
	[Version] [bigint] NOT NULL,
	[PendingTimer] [datetime] NULL,
	[CreationTime] [datetime] NOT NULL,
	[LastUpdated] [datetime] NULL,
	[WorkflowHostType] [uniqueidentifier] NULL,
	[ServiceDeploymentId] [bigint] NULL,
	[SuspensionExceptionName] [nvarchar](450) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SuspensionReason] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[BlockingBookmarks] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LastMachineRunOn] [nvarchar](450) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExecutionStatus] [nvarchar](450) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsInitialized] [bit] NULL,
	[IsSuspended] [bit] NULL,
	[IsReadyToRun] [bit] NULL,
	[IsCompleted] [bit] NULL
)

GO

USE [Gorba_CenterControllers]
/****** Object:  Index [CIX_InstancesTable]    Script Date: 08/23/2012 08:19:51 ******/
CREATE UNIQUE CLUSTERED INDEX [CIX_InstancesTable] ON [System.Activities.DurableInstancing].[InstancesTable] 
(
	[SurrogateInstanceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO

USE [Gorba_CenterControllers]
/****** Object:  Index [NCIX_InstancesTable_CreationTime]    Script Date: 08/23/2012 08:19:51 ******/
CREATE NONCLUSTERED INDEX [NCIX_InstancesTable_CreationTime] ON [System.Activities.DurableInstancing].[InstancesTable] 
(
	[CreationTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO

USE [Gorba_CenterControllers]
/****** Object:  Index [NCIX_InstancesTable_Id]    Script Date: 08/23/2012 08:19:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [NCIX_InstancesTable_Id] ON [System.Activities.DurableInstancing].[InstancesTable] 
(
	[Id] ASC
)
INCLUDE ( [Version],
[SurrogateLockOwnerId],
[IsCompleted]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO

USE [Gorba_CenterControllers]
/****** Object:  Index [NCIX_InstancesTable_LastUpdated]    Script Date: 08/23/2012 08:19:51 ******/
CREATE NONCLUSTERED INDEX [NCIX_InstancesTable_LastUpdated] ON [System.Activities.DurableInstancing].[InstancesTable] 
(
	[LastUpdated] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO

USE [Gorba_CenterControllers]
/****** Object:  Index [NCIX_InstancesTable_ServiceDeploymentId]    Script Date: 08/23/2012 08:19:51 ******/
CREATE NONCLUSTERED INDEX [NCIX_InstancesTable_ServiceDeploymentId] ON [System.Activities.DurableInstancing].[InstancesTable] 
(
	[ServiceDeploymentId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO

USE [Gorba_CenterControllers]
/****** Object:  Index [NCIX_InstancesTable_SurrogateLockOwnerId]    Script Date: 08/23/2012 08:19:51 ******/
CREATE NONCLUSTERED INDEX [NCIX_InstancesTable_SurrogateLockOwnerId] ON [System.Activities.DurableInstancing].[InstancesTable] 
(
	[SurrogateLockOwnerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO

USE [Gorba_CenterControllers]
/****** Object:  Index [NCIX_InstancesTable_SuspensionExceptionName]    Script Date: 08/23/2012 08:19:51 ******/
CREATE NONCLUSTERED INDEX [NCIX_InstancesTable_SuspensionExceptionName] ON [System.Activities.DurableInstancing].[InstancesTable] 
(
	[SuspensionExceptionName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO

USE [Gorba_CenterControllers]
/****** Object:  Index [NCIX_InstancesTable_WorkflowHostType]    Script Date: 08/23/2012 08:19:51 ******/
CREATE NONCLUSTERED INDEX [NCIX_InstancesTable_WorkflowHostType] ON [System.Activities.DurableInstancing].[InstancesTable] 
(
	[WorkflowHostType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO
USE [Gorba_CenterControllers]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[KeysTable]    Script Date: 08/23/2012 08:19:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[KeysTable](
	[Id] [uniqueidentifier] NOT NULL,
	[SurrogateKeyId] [bigint] IDENTITY(1,1) NOT NULL,
	[SurrogateInstanceId] [bigint] NULL,
	[EncodingOption] [tinyint] NULL,
	[Properties] [varbinary](max) NULL,
	[IsAssociated] [bit] NULL
)

GO

USE [Gorba_CenterControllers]
/****** Object:  Index [CIX_KeysTable]    Script Date: 08/23/2012 08:19:51 ******/
CREATE UNIQUE CLUSTERED INDEX [CIX_KeysTable] ON [System.Activities.DurableInstancing].[KeysTable] 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = ON, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO

USE [Gorba_CenterControllers]
/****** Object:  Index [NCIX_KeysTable_SurrogateInstanceId]    Script Date: 08/23/2012 08:19:51 ******/
CREATE NONCLUSTERED INDEX [NCIX_KeysTable_SurrogateInstanceId] ON [System.Activities.DurableInstancing].[KeysTable] 
(
	[SurrogateInstanceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO
USE [Gorba_CenterControllers]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[LockOwnersTable]    Script Date: 08/23/2012 08:19:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[LockOwnersTable](
	[Id] [uniqueidentifier] NOT NULL,
	[SurrogateLockOwnerId] [bigint] IDENTITY(1,1) NOT NULL,
	[LockExpiration] [datetime] NOT NULL,
	[WorkflowHostType] [uniqueidentifier] NULL,
	[MachineName] [nvarchar](128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[EnqueueCommand] [bit] NOT NULL,
	[DeletesInstanceOnCompletion] [bit] NOT NULL,
	[PrimitiveLockOwnerData] [varbinary](max) NULL,
	[ComplexLockOwnerData] [varbinary](max) NULL,
	[WriteOnlyPrimitiveLockOwnerData] [varbinary](max) NULL,
	[WriteOnlyComplexLockOwnerData] [varbinary](max) NULL,
	[EncodingOption] [tinyint] NULL
)

GO

USE [Gorba_CenterControllers]
/****** Object:  Index [CIX_LockOwnersTable]    Script Date: 08/23/2012 08:19:51 ******/
CREATE UNIQUE CLUSTERED INDEX [CIX_LockOwnersTable] ON [System.Activities.DurableInstancing].[LockOwnersTable] 
(
	[SurrogateLockOwnerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO

USE [Gorba_CenterControllers]
/****** Object:  Index [NCIX_LockOwnersTable_Id]    Script Date: 08/23/2012 08:19:52 ******/
CREATE UNIQUE NONCLUSTERED INDEX [NCIX_LockOwnersTable_Id] ON [System.Activities.DurableInstancing].[LockOwnersTable] 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = ON, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO

USE [Gorba_CenterControllers]
/****** Object:  Index [NCIX_LockOwnersTable_LockExpiration]    Script Date: 08/23/2012 08:19:52 ******/
CREATE NONCLUSTERED INDEX [NCIX_LockOwnersTable_LockExpiration] ON [System.Activities.DurableInstancing].[LockOwnersTable] 
(
	[LockExpiration] ASC
)
INCLUDE ( [WorkflowHostType],
[MachineName]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO

USE [Gorba_CenterControllers]
/****** Object:  Index [NCIX_LockOwnersTable_WorkflowHostType]    Script Date: 08/23/2012 08:19:52 ******/
CREATE NONCLUSTERED INDEX [NCIX_LockOwnersTable_WorkflowHostType] ON [System.Activities.DurableInstancing].[LockOwnersTable] 
(
	[WorkflowHostType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO
USE [Gorba_CenterControllers]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[RunnableInstancesTable]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[RunnableInstancesTable](
	[SurrogateInstanceId] [bigint] NOT NULL,
	[WorkflowHostType] [uniqueidentifier] NULL,
	[ServiceDeploymentId] [bigint] NULL,
	[RunnableTime] [datetime] NOT NULL
)

GO

USE [Gorba_CenterControllers]
/****** Object:  Index [CIX_RunnableInstancesTable_SurrogateInstanceId]    Script Date: 08/23/2012 08:19:52 ******/
CREATE UNIQUE CLUSTERED INDEX [CIX_RunnableInstancesTable_SurrogateInstanceId] ON [System.Activities.DurableInstancing].[RunnableInstancesTable] 
(
	[SurrogateInstanceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = ON, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO

USE [Gorba_CenterControllers]
/****** Object:  Index [NCIX_RunnableInstancesTable]    Script Date: 08/23/2012 08:19:52 ******/
CREATE NONCLUSTERED INDEX [NCIX_RunnableInstancesTable] ON [System.Activities.DurableInstancing].[RunnableInstancesTable] 
(
	[WorkflowHostType] ASC,
	[RunnableTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO

USE [Gorba_CenterControllers]
/****** Object:  Index [NCIX_RunnableInstancesTable_RunnableTime]    Script Date: 08/23/2012 08:19:52 ******/
CREATE NONCLUSTERED INDEX [NCIX_RunnableInstancesTable_RunnableTime] ON [System.Activities.DurableInstancing].[RunnableInstancesTable] 
(
	[RunnableTime] ASC
)
INCLUDE ( [WorkflowHostType],
[ServiceDeploymentId]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO
USE [Gorba_CenterControllers]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[ServiceDeploymentsTable]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[ServiceDeploymentsTable](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ServiceDeploymentHash] [uniqueidentifier] NOT NULL,
	[SiteName] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RelativeServicePath] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RelativeApplicationPath] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ServiceName] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ServiceNamespace] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
)

GO

USE [Gorba_CenterControllers]
/****** Object:  Index [CIX_ServiceDeploymentsTable]    Script Date: 08/23/2012 08:19:52 ******/
CREATE UNIQUE CLUSTERED INDEX [CIX_ServiceDeploymentsTable] ON [System.Activities.DurableInstancing].[ServiceDeploymentsTable] 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO

USE [Gorba_CenterControllers]
/****** Object:  Index [NCIX_ServiceDeploymentsTable_ServiceDeploymentHash]    Script Date: 08/23/2012 08:19:52 ******/
CREATE UNIQUE NONCLUSTERED INDEX [NCIX_ServiceDeploymentsTable_ServiceDeploymentHash] ON [System.Activities.DurableInstancing].[ServiceDeploymentsTable] 
(
	[ServiceDeploymentHash] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = ON, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF)
GO
USE [Gorba_CenterControllers]
GO
/****** Object:  Table [System.Activities.DurableInstancing].[SqlWorkflowInstanceStoreVersionTable]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System.Activities.DurableInstancing].[SqlWorkflowInstanceStoreVersionTable](
	[Major] [bigint] NULL,
	[Minor] [bigint] NULL,
	[Build] [bigint] NULL,
	[Revision] [bigint] NULL,
	[LastUpdated] [datetime] NULL
)

GO

USE [Gorba_CenterControllers]
/****** Object:  Index [CIX_SqlWorkflowInstanceStoreVersionTable]    Script Date: 08/23/2012 08:19:52 ******/
CREATE UNIQUE CLUSTERED INDEX [CIX_SqlWorkflowInstanceStoreVersionTable] ON [System.Activities.DurableInstancing].[SqlWorkflowInstanceStoreVersionTable] 
(
	[Major] ASC,
	[Minor] ASC,
	[Build] ASC,
	[Revision] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
GO
/****** Object:  View [System.Activities.DurableInstancing].[InstancePromotedProperties]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create view [System.Activities.DurableInstancing].[InstancePromotedProperties] with schemabinding as
      select [InstancesTable].[Id] as [InstanceId],
			 [InstancesTable].[DataEncodingOption] as [EncodingOption],
			 [PromotionName],
			 [Value1],
			 [Value2],
			 [Value3],
			 [Value4],
			 [Value5],
			 [Value6],
			 [Value7],
			 [Value8],
			 [Value9],
			 [Value10],
			 [Value11],
			 [Value12],
			 [Value13],
			 [Value14],
			 [Value15],
			 [Value16],
			 [Value17],
			 [Value18],
			 [Value19],
			 [Value20],
			 [Value21],
			 [Value22],
			 [Value23],
			 [Value24],
			 [Value25],
			 [Value26],
			 [Value27],
			 [Value28],
			 [Value29],
			 [Value30],
			 [Value31],
			 [Value32],
			 [Value33],
			 [Value34],
			 [Value35],
			 [Value36],
			 [Value37],
			 [Value38],
			 [Value39],
			 [Value40],
			 [Value41],
			 [Value42],
			 [Value43],
			 [Value44],
			 [Value45],
			 [Value46],
			 [Value47],
			 [Value48],
			 [Value49],
			 [Value50],
			 [Value51],
			 [Value52],
			 [Value53],
			 [Value54],
			 [Value55],
			 [Value56],
			 [Value57],
			 [Value58],
			 [Value59],
			 [Value60],
			 [Value61],
			 [Value62],
			 [Value63],
			 [Value64]
    from [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable]
    join [System.Activities.DurableInstancing].[InstancesTable]
    on [InstancePromotedPropertiesTable].[SurrogateInstanceId] = [InstancesTable].[SurrogateInstanceId]



GO
USE [Gorba_CenterControllers]
GO
SET ARITHABORT ON
GO
SET CONCAT_NULL_YIELDS_NULL ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
SET ANSI_PADDING ON
GO
SET ANSI_WARNINGS ON
GO
SET NUMERIC_ROUNDABORT OFF
GO
/****** Object:  Index [CIX_InstancePromotedProperties]    Script Date: 08/23/2012 08:19:52 ******/
CREATE UNIQUE CLUSTERED INDEX [CIX_InstancePromotedProperties] ON [System.Activities.DurableInstancing].[InstancePromotedProperties] 
(
	[InstanceId] ASC,
	[PromotionName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
GO
USE [Gorba_CenterControllers]
GO
SET ARITHABORT ON
GO
SET CONCAT_NULL_YIELDS_NULL ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
SET ANSI_PADDING ON
GO
SET ANSI_WARNINGS ON
GO
SET NUMERIC_ROUNDABORT OFF
GO

/****** Object:  View [System.Activities.DurableInstancing].[Instances]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create view [System.Activities.DurableInstancing].[Instances] as
      select [InstancesTable].[Id] as [InstanceId],
             [PendingTimer],
             [CreationTime],
             [LastUpdated] as [LastUpdatedTime],
             [InstancesTable].[ServiceDeploymentId],
             [SuspensionExceptionName],
             [SuspensionReason],
             [BlockingBookmarks] as [ActiveBookmarks],
             case when [LockOwnersTable].[LockExpiration] > getutcdate()
				then [LockOwnersTable].[MachineName]
				else null
				end as [CurrentMachine],
             [LastMachineRunOn] as [LastMachine],
             [ExecutionStatus],
             [IsInitialized],
             [IsSuspended],
             [IsCompleted],
             [InstancesTable].[DataEncodingOption] as [EncodingOption],
             [PrimitiveDataProperties] as [ReadWritePrimitiveDataProperties],
             [WriteOnlyPrimitiveDataProperties],
             [ComplexDataProperties] as [ReadWriteComplexDataProperties],
             [WriteOnlyComplexDataProperties]
      from [System.Activities.DurableInstancing].[InstancesTable]
      left outer join [System.Activities.DurableInstancing].[LockOwnersTable]
      on [InstancesTable].[SurrogateLockOwnerId] = [LockOwnersTable].[SurrogateLockOwnerId]



GO
/****** Object:  View [System.Activities.DurableInstancing].[ServiceDeployments]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create view [System.Activities.DurableInstancing].[ServiceDeployments] as
      select [Id] as [ServiceDeploymentId],
             [SiteName],
             [RelativeServicePath],
             [RelativeApplicationPath],
             [ServiceName],
             [ServiceNamespace]
      from [System.Activities.DurableInstancing].[ServiceDeploymentsTable]



GO
/****** Object:  UserDefinedFunction [System.Activities.DurableInstancing].[GetExpirationTime]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create function [System.Activities.DurableInstancing].[GetExpirationTime] (@offsetInMilliseconds bigint)
returns datetime
as
begin

	if (@offsetInMilliseconds is null)
	begin
		return null
	end

	declare @hourInMillisecond bigint
	declare @offsetInHours bigint
	declare @remainingOffsetInMilliseconds bigint
	declare @expirationTimer datetime

	set @hourInMillisecond = 60*60*1000
	set @offsetInHours = @offsetInMilliseconds / @hourInMillisecond
	set @remainingOffsetInMilliseconds = @offsetInMilliseconds % @hourInMillisecond

	set @expirationTimer = getutcdate()
	set @expirationTimer = dateadd (hour, @offsetInHours, @expirationTimer)
	set @expirationTimer = dateadd (millisecond,@remainingOffsetInMilliseconds, @expirationTimer)

	return @expirationTimer

end



GO
/****** Object:  UserDefinedFunction [System.Activities.DurableInstancing].[ParseBinaryPropertyValue]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create function [System.Activities.DurableInstancing].[ParseBinaryPropertyValue] (@startPosition int, @length int, @concatenatedKeyProperties varbinary(max))
returns varbinary(max)
as
begin
	if (@length > 0)
		return substring(@concatenatedKeyProperties, @startPosition + 1, @length)
	return null
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[DeleteInstance]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[DeleteInstance]
	@surrogateInstanceId bigint = null
as
begin	
	set nocount on
	set transaction isolation level read committed		
	set xact_abort on;	
	
	delete [InstancePromotedPropertiesTable]
	where [SurrogateInstanceId] = @surrogateInstanceId
		
	delete [KeysTable]
	where [SurrogateInstanceId] = @surrogateInstanceId
		
	delete [InstanceMetadataChangesTable]
	where [SurrogateInstanceId] = @surrogateInstanceId

	delete [RunnableInstancesTable] 
	where [SurrogateInstanceId] = @surrogateInstanceId

	delete [InstancesTable] 
	where [SurrogateInstanceId] = @surrogateInstanceId

end



GO

/****** Object:  StoredProcedure [dbo].[ClearInstances]    Script Date: 01/14/2013 12:47:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 17.01.12
-- Description:	Clears instances from the database
-- =============================================
CREATE PROCEDURE [dbo].[ClearInstances]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @id bigint
	DECLARE @RC int
	DECLARE @text varchar(1000)
	
	DECLARE instanceCursor CURSOR READ_ONLY
	FOR
	SELECT [SurrogateInstanceId]
	FROM [System.Activities.DurableInstancing].[InstancesTable]


	OPEN instanceCursor

	FETCH NEXT FROM instanceCursor
	INTO @id

	WHILE @@FETCH_STATUS = 0
	BEGIN
	

		EXECUTE @RC = [Gorba_CenterControllers].[System.Activities.DurableInstancing].[DeleteInstance] 
			@surrogateInstanceId = @id

		SET @text = 'Deleted instance ' + CONVERT(varchar(100), @id)
		
		print @text

		FETCH NEXT FROM instanceCursor
		INTO @id

	END

	CLOSE instanceCursor
	DEALLOCATE instanceCursor
	
END

GO




GO
/****** Cannot script Unresolved Entities : Server[@Name='PC1234']/Database[@Name='T']/UnresolvedEntity[@Name='value' and @Schema='Item'] ******/

GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[AssociateKeys]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[AssociateKeys]
	@surrogateInstanceId bigint,
	@keysToAssociate xml = null,
	@concatenatedKeyProperties varbinary(max) = null,
	@encodingOption tinyint,
	@singleKeyId uniqueidentifier
as
begin	
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	declare @badKeyId uniqueidentifier
	declare @numberOfKeys int
	declare @result int
	declare @keys table([KeyId] uniqueidentifier, [Properties] varbinary(max))
	
	set @result = 0
	
	if (@keysToAssociate is not null)
	begin
		insert into @keys
		select T.Item.value('@KeyId', 'uniqueidentifier') as [KeyId],
			   [System.Activities.DurableInstancing].[ParseBinaryPropertyValue](T.Item.value('@StartPosition', 'int'), T.Item.value('@BinaryLength', 'int'), @concatenatedKeyProperties) as [Properties]
	    from @keysToAssociate.nodes('/CorrelationKeys/CorrelationKey') as T(Item)
		option (maxdop 1)

		select @numberOfKeys = count(1) from @keys
		
		insert into [KeysTable] ([Id], [SurrogateInstanceId], [IsAssociated])
		select [KeyId], @surrogateInstanceId, 1
		from @keys as [Keys]
		
		if (@@rowcount != @numberOfKeys)
		begin
			select top 1 @badKeyId = [Keys].[KeyId] 
			from @keys as [Keys]
			join [KeysTable] on [Keys].[KeyId] = [KeysTable].[Id]
			where [KeysTable].[SurrogateInstanceId] != @surrogateInstanceId
			
			if (@@rowcount != 0)
			begin
				select 3 as 'Result', @badKeyId
				return 3
			end
		end
		
		update [KeysTable]
		set [Properties] = [Keys].[Properties],
			[EncodingOption] = @encodingOption
		from @keys as [Keys]
		join [KeysTable] on [Keys].[KeyId] = [KeysTable].[Id]
		where [KeysTable].[EncodingOption] is null
	end
	
	if (@singleKeyId is not null)
	begin
InsertSingleKey:
		update [KeysTable]
		set [Properties] = @concatenatedKeyProperties,
			[EncodingOption] = @encodingOption
		where ([Id] = @singleKeyId) and ([SurrogateInstanceId] = @surrogateInstanceId)
			  
		if (@@rowcount != 1)
		begin
			if exists (select [Id] from [KeysTable] where [Id] = @singleKeyId)
			begin
				select 3 as 'Result', @singleKeyId
				return 3
			end
			
			insert into [KeysTable] ([Id], [SurrogateInstanceId], [IsAssociated], [Properties], [EncodingOption])
			values (@singleKeyId, @surrogateInstanceId, 1, @concatenatedKeyProperties, @encodingOption)
			
			if (@@rowcount = 0)
				goto InsertSingleKey
		end
	end
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[CompleteKeys]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[CompleteKeys]
	@surrogateInstanceId bigint,
	@keysToComplete xml = null
as
begin	
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	declare @badKeyId uniqueidentifier
	declare @numberOfKeys int
	declare @result int
	declare @keyIds table([KeyId] uniqueidentifier)
	
	set @result = 0
	
	if (@keysToComplete is not null)
	begin
		insert into @keyIds
		select T.Item.value('@KeyId', 'uniqueidentifier')
		from @keysToComplete.nodes('//CorrelationKey') as T(Item)
		option(maxdop 1)
		
		select @numberOfKeys = count(1) from @keyIds
		
		update [KeysTable]
		set [IsAssociated] = 0
		from @keyIds as [KeyIds]
		join [KeysTable] on [KeyIds].[KeyId] = [KeysTable].[Id]
		where [SurrogateInstanceId] = @surrogateInstanceId
		
		if (@@rowcount != @numberOfKeys)
		begin
			select top 1 @badKeyId = [MissingKeys].[MissingKeyId]
			from
				(select [KeyIds].[KeyId] as [MissingKeyId] 
				 from @keyIds as [KeyIds]
				 except
				 select [Id] from [KeysTable] where [SurrogateInstanceId] = @surrogateInstanceId) as MissingKeys
		
			select 4 as 'Result', @badKeyId
			return 4
		end
	end
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[CreateInstance]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[CreateInstance]
	@instanceId uniqueidentifier,
	@surrogateLockOwnerId bigint,
	@workflowHostType uniqueidentifier,
	@serviceDeploymentId bigint,
	@surrogateInstanceId bigint output,
	@result int output
as
begin
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	set @surrogateInstanceId = 0
	set @result = 0
	
	begin try
		insert into [InstancesTable] ([Id], [SurrogateLockOwnerId], [CreationTime], [WorkflowHostType], [ServiceDeploymentId], [Version])
		values (@instanceId, @surrogateLockOwnerId, getutcdate(), @workflowHostType, @serviceDeploymentId, 1)
		
		set @surrogateInstanceId = scope_identity()		
	end try
	begin catch
		if (error_number() != 2601)
		begin
			set @result = 99
			select @result as 'Result'
		end
	end catch
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[CreateLockOwner]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [System.Activities.DurableInstancing].[CreateLockOwner]
	@lockOwnerId uniqueidentifier,
	@lockTimeout int,
	@workflowHostType uniqueidentifier,
	@enqueueCommand bit,
	@deleteInstanceOnCompletion bit,	
	@primitiveLockOwnerData varbinary(max),
	@complexLockOwnerData varbinary(max),
	@writeOnlyPrimitiveLockOwnerData varbinary(max),
	@writeOnlyComplexLockOwnerData varbinary(max),
	@encodingOption tinyint,
	@machineName nvarchar(128)
as
begin
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	begin transaction
	
	declare @lockAcquired bigint
	declare @lockExpiration datetime
	declare @now datetime
	declare @result int
	declare @surrogateLockOwnerId bigint
	
	set @result = 0
	
	exec @lockAcquired = sp_getapplock @Resource = 'InstanceStoreLock', @LockMode = 'Shared', @LockTimeout = 10000
		
	if (@lockAcquired < 0)
	begin
		select @result as 'Result'
		set @result = 13
	end
	
	if (@result = 0)
	begin
		set @now = getutcdate()
		
		if (@lockTimeout = 0)
			set @lockExpiration = '9999-12-31T23:59:59';
		else 
			set @lockExpiration = dateadd(second, @lockTimeout, getutcdate());
		
		insert into [LockOwnersTable] ([Id], [LockExpiration], [MachineName], [WorkflowHostType], [EnqueueCommand], [DeletesInstanceOnCompletion], [PrimitiveLockOwnerData], [ComplexLockOwnerData], [WriteOnlyPrimitiveLockOwnerData], [WriteOnlyComplexLockOwnerData], [EncodingOption])
		values (@lockOwnerId, @lockExpiration, @machineName, @workflowHostType, @enqueueCommand, @deleteInstanceOnCompletion, @primitiveLockOwnerData, @complexLockOwnerData, @writeOnlyPrimitiveLockOwnerData, @writeOnlyComplexLockOwnerData, @encodingOption)
		
		set @surrogateLockOwnerId = scope_identity()
	end
	
	if (@result != 13)
		exec sp_releaseapplock @Resource = 'InstanceStoreLock'
	
	if (@result = 0)
	begin
		commit transaction
		select 0 as 'Result', @surrogateLockOwnerId
	end
	else
		rollback transaction
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[CreateServiceDeployment]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[CreateServiceDeployment]	
	@serviceDeploymentHash uniqueIdentifier,
	@siteName nvarchar(max),
	@relativeServicePath nvarchar(max),
	@relativeApplicationPath nvarchar(max),
	@serviceName nvarchar(max),
    @serviceNamespace nvarchar(max),
    @serviceDeploymentId bigint output
as
begin
	set nocount on
	set transaction isolation level read committed		
	set xact_abort on;	
	
		--Create or select the service deployment id
		insert into [ServiceDeploymentsTable]
			([ServiceDeploymentHash], [SiteName], [RelativeServicePath], [RelativeApplicationPath], [ServiceName], [ServiceNamespace])
			values (@serviceDeploymentHash, @siteName, @relativeServicePath, @relativeApplicationPath, @serviceName, @serviceNamespace)
			
		if (@@rowcount = 0)
		begin		
			select @serviceDeploymentId = [Id]
			from [ServiceDeploymentsTable]
			where [ServiceDeploymentHash] = @serviceDeploymentHash		
		end
		else			
		begin
			set @serviceDeploymentId = scope_identity()		
		end	
		
		select 0 as 'Result', @serviceDeploymentId		
end	



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[DeleteLockOwner]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [System.Activities.DurableInstancing].[DeleteLockOwner]
	@surrogateLockOwnerId bigint
as
begin
	set nocount on
	set transaction isolation level read committed
	set deadlock_priority low
	set xact_abort on;	
	
	begin transaction
	
	declare @lockAcquired bigint
	declare @result int
	set @result = 0
	
	exec @lockAcquired = sp_getapplock @Resource = 'InstanceStoreLock', @LockMode = 'Shared', @LockTimeout = 10000
		
	if (@lockAcquired < 0)
	begin
		select @result as 'Result'
		set @result = 13
	end
	
	if (@result = 0)
	begin
		update [LockOwnersTable]
		set [LockExpiration] = '2000-01-01T00:00:00'
		where [SurrogateLockOwnerId] = @surrogateLockOwnerId
	end
	
	if (@result != 13)
		exec sp_releaseapplock @Resource = 'InstanceStoreLock' 
	
	if (@result = 0)
	begin
		commit transaction
		select 0 as 'Result'
	end
	else
		rollback transaction
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[DetectRunnableInstances]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [System.Activities.DurableInstancing].[DetectRunnableInstances]
	@workflowHostType uniqueidentifier
as
begin
	set nocount on
	set transaction isolation level read committed	
	set xact_abort on;	
	set deadlock_priority low
	
	declare @nextRunnableTime datetime

	select top 1 @nextRunnableTime = [RunnableInstancesTable].[RunnableTime]
			  from [RunnableInstancesTable] with (readpast)
			  where [WorkflowHostType] = @workflowHostType
			  order by [WorkflowHostType], [RunnableTime]
			  
	select 0 as 'Result', @nextRunnableTime, getutcdate()
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[ExtendLock]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[ExtendLock]
	@surrogateLockOwnerId bigint,
	@lockTimeout int	
as
begin
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	begin transaction	
	
	declare @now datetime
	declare @newLockExpiration datetime
	declare @result int
	
	set @now = getutcdate()
	set @result = 0
	
	if (@lockTimeout = 0)
		set @newLockExpiration = '9999-12-31T23:59:59'
	else
		set @newLockExpiration = dateadd(second, @lockTimeout, @now)
	
	update [LockOwnersTable]
	set [LockExpiration] = @newLockExpiration
	where ([SurrogateLockOwnerId] = @surrogateLockOwnerId) and
		  ([LockExpiration] > @now)
	
	if (@@rowcount = 0) 
	begin
		if exists (select * from [LockOwnersTable] where ([SurrogateLockOwnerId] = @surrogateLockOwnerId))
		begin
			exec [System.Activities.DurableInstancing].[DeleteLockOwner] @surrogateLockOwnerId
			set @result = 11
		end
		else
			set @result = 12
	end
	
	select @result as 'Result'
	commit transaction
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[FreeKeys]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[FreeKeys]
	@surrogateInstanceId bigint,
	@keysToFree xml = null
as
begin	
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	declare @badKeyId uniqueidentifier
	declare @numberOfKeys int
	declare @result int
	declare @keyIds table([KeyId] uniqueidentifier)
	
	set @result = 0
	
	if (@keysToFree is not null)
	begin
		insert into @keyIds
		select T.Item.value('@KeyId', 'uniqueidentifier')
		from @keysToFree.nodes('//CorrelationKey') as T(Item)
		option(maxdop 1)
		
		select @numberOfKeys = count(1) from @keyIds
		
		delete [KeysTable]
		from @keyIds as [KeyIds]
		join [KeysTable] on [KeyIds].[KeyId] = [KeysTable].[Id]
		where [SurrogateInstanceId] = @surrogateInstanceId

		if (@@rowcount != @numberOfKeys)
		begin
			select top 1 @badKeyId = [MissingKeys].[MissingKeyId] from
				(select [KeyIds].[KeyId] as [MissingKeyId]
				 from @keyIds as [KeyIds]
				 except
				 select [Id] from [KeysTable] where [SurrogateInstanceId] = @surrogateInstanceId) as MissingKeys
		
			select 4 as 'Result', @badKeyId
			return 4
		end
	end
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[GetActivatableWorkflowsActivationParameters]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [System.Activities.DurableInstancing].[GetActivatableWorkflowsActivationParameters]
	@machineName nvarchar(128)
as
begin
	set nocount on
	set transaction isolation level read committed	
	set xact_abort on;	
	set deadlock_priority low
	
	declare @now datetime
	set @now = getutcdate()
	
	select 0 as 'Result'
	
	select top(1000) serviceDeployments.[SiteName], serviceDeployments.[RelativeApplicationPath], serviceDeployments.[RelativeServicePath]
	from (
		select distinct [ServiceDeploymentId], [WorkflowHostType]
		from [RunnableInstancesTable] with (readpast)
		where [RunnableTime] <= @now
		) runnableWorkflows inner join [ServiceDeploymentsTable] serviceDeployments
		on runnableWorkflows.[ServiceDeploymentId] = serviceDeployments.[Id]
	where not exists (
						select top (1) 1
						from [LockOwnersTable] lockOwners
						where lockOwners.[LockExpiration] > @now
						and lockOwners.[MachineName] = @machineName
						and lockOwners.[WorkflowHostType] = runnableWorkflows.[WorkflowHostType]
					  )
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[InsertPromotedProperties]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [System.Activities.DurableInstancing].[InsertPromotedProperties]
	@instanceId uniqueidentifier,
	@promotionName nvarchar(400),
	@value1 sql_variant = null,
	@value2 sql_variant = null,
	@value3 sql_variant = null,
	@value4 sql_variant = null,
	@value5 sql_variant = null,
	@value6 sql_variant = null,
	@value7 sql_variant = null,
	@value8 sql_variant = null,
	@value9 sql_variant = null,
	@value10 sql_variant = null,
	@value11 sql_variant = null,
	@value12 sql_variant = null,
	@value13 sql_variant = null,
	@value14 sql_variant = null,
	@value15 sql_variant = null,
	@value16 sql_variant = null,
	@value17 sql_variant = null,
	@value18 sql_variant = null,
	@value19 sql_variant = null,
	@value20 sql_variant = null,
	@value21 sql_variant = null,
	@value22 sql_variant = null,
	@value23 sql_variant = null,
	@value24 sql_variant = null,
	@value25 sql_variant = null,
	@value26 sql_variant = null,
	@value27 sql_variant = null,
	@value28 sql_variant = null,
	@value29 sql_variant = null,
	@value30 sql_variant = null,
	@value31 sql_variant = null,
	@value32 sql_variant = null,
	@value33 varbinary(max) = null,
	@value34 varbinary(max) = null,
	@value35 varbinary(max) = null,
	@value36 varbinary(max) = null,
	@value37 varbinary(max) = null,
	@value38 varbinary(max) = null,
	@value39 varbinary(max) = null,
	@value40 varbinary(max) = null,
	@value41 varbinary(max) = null,
	@value42 varbinary(max) = null,
	@value43 varbinary(max) = null,
	@value44 varbinary(max) = null,
	@value45 varbinary(max) = null,
	@value46 varbinary(max) = null,
	@value47 varbinary(max) = null,
	@value48 varbinary(max) = null,
	@value49 varbinary(max) = null,
	@value50 varbinary(max) = null,
	@value51 varbinary(max) = null,
	@value52 varbinary(max) = null,
	@value53 varbinary(max) = null,
	@value54 varbinary(max) = null,
	@value55 varbinary(max) = null,
	@value56 varbinary(max) = null,
	@value57 varbinary(max) = null,
	@value58 varbinary(max) = null,
	@value59 varbinary(max) = null,
	@value60 varbinary(max) = null,
	@value61 varbinary(max) = null,
	@value62 varbinary(max) = null,
	@value63 varbinary(max) = null,
	@value64 varbinary(max) = null
as
begin
	set nocount on
	set transaction isolation level read committed		
	set xact_abort on;	

	declare @surrogateInstanceId bigint

	select @surrogateInstanceId = [SurrogateInstanceId]
	from [InstancesTable]
	where [Id] = @instanceId

	insert into [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable]
	values (@surrogateInstanceId, @promotionName, @value1, @value2, @value3, @value4, @value5, @value6, @value7, @value8,
			@value9, @value10, @value11, @value12, @value13, @value14, @value15, @value16, @value17, @value18, @value19,
			@value20, @value21, @value22, @value23, @value24, @value25, @value26, @value27, @value28, @value29, @value30,
			@value31, @value32, @value33, @value34, @value35, @value36, @value37, @value38, @value39, @value40, @value41,
			@value42, @value43, @value44, @value45, @value46, @value47, @value48, @value49, @value50, @value51, @value52,
			@value53, @value54, @value55, @value56, @value57, @value58, @value59, @value60, @value61, @value62, @value63,
			@value64)
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[InsertRunnableInstanceEntry]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [System.Activities.DurableInstancing].[InsertRunnableInstanceEntry]
	@surrogateInstanceId bigint,
	@workflowHostType uniqueidentifier,
	@serviceDeploymentId bigint, 
	@isSuspended bit,
	@isReadyToRun bit,
	@pendingTimer datetime
AS
begin    
	set nocount on;
	set transaction isolation level read committed;
	set xact_abort on;	
	
	declare @runnableTime datetime
	
	if (@isSuspended  = 0)
	begin
		if (@isReadyToRun = 1)
		begin
			set @runnableTime = getutcdate()					
		end
		else if (@pendingTimer is not null)
		begin
			set @runnableTime = @pendingTimer
		end
	end
		
	if (@runnableTime is not null and @workflowHostType is not null)
	begin	
		insert into [RunnableInstancesTable]
			([SurrogateInstanceId], [WorkflowHostType], [ServiceDeploymentId], [RunnableTime])
			values( @surrogateInstanceId, @workflowHostType, @serviceDeploymentId, @runnableTime)
	end
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[LockInstance]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[LockInstance]
	@instanceId uniqueidentifier,
	@surrogateLockOwnerId bigint,
	@handleInstanceVersion bigint,
	@handleIsBoundToLock bit,
	@surrogateInstanceId bigint output,
	@lockVersion bigint output,
	@result int output
as
begin
	set nocount on
	set transaction isolation level read committed
	set xact_abort on;	
	
	declare @isCompleted bit
	declare @currentLockOwnerId bigint
	declare @currentVersion bigint

TryLockInstance:
	set @currentLockOwnerId = 0
	set @surrogateInstanceId = 0
	set @result = 0
	
	update [InstancesTable]
	set [SurrogateLockOwnerId] = @surrogateLockOwnerId,
		@lockVersion = [Version] = case when ([InstancesTable].[SurrogateLockOwnerId] is null or 
											  [InstancesTable].[SurrogateLockOwnerId] != @surrogateLockOwnerId)
									then [Version] + 1
									else [Version]
								  end,
		@surrogateInstanceId = [SurrogateInstanceId]
	from [InstancesTable]
	left outer join [LockOwnersTable] on [InstancesTable].[SurrogateLockOwnerId] = [LockOwnersTable].[SurrogateLockOwnerId]
	where ([InstancesTable].[Id] = @instanceId) and
		  ([InstancesTable].[IsCompleted] = 0) and
		  (
		   (@handleIsBoundToLock = 0 and
		    (
		     ([InstancesTable].[SurrogateLockOwnerId] is null) or
		     ([LockOwnersTable].[SurrogateLockOwnerId] is null) or
			  (
		       ([LockOwnersTable].[LockExpiration] < getutcdate()) and
               ([LockOwnersTable].[SurrogateLockOwnerId] != @surrogateLockOwnerId)
			  )
		    )
		   ) or 
		   (
			(@handleIsBoundToLock = 1) and
		    ([LockOwnersTable].[SurrogateLockOwnerId] = @surrogateLockOwnerId) and
		    ([LockOwnersTable].[LockExpiration] > getutcdate()) and
		    ([InstancesTable].[Version] = @handleInstanceVersion)
		   )
		  )
	
	if (@@rowcount = 0)
	begin
		if not exists (select * from [LockOwnersTable] where ([SurrogateLockOwnerId] = @surrogateLockOwnerId) and ([LockExpiration] > getutcdate()))
		begin
			if exists (select * from [LockOwnersTable] where [SurrogateLockOwnerId] = @surrogateLockOwnerId)
				set @result = 11
			else
				set @result = 12
			
			select @result as 'Result'
			return 0
		end
		
		select @currentLockOwnerId = [SurrogateLockOwnerId],
			   @isCompleted = [IsCompleted],
			   @currentVersion = [Version]
		from [InstancesTable]
		where [Id] = @instanceId
	
		if (@@rowcount = 1)
		begin
			if (@isCompleted = 1)
				set @result = 7
			else if (@currentLockOwnerId = @surrogateLockOwnerId)
			begin
				if (@handleIsBoundToLock = 1)
					set @result = 10
				else
					set @result = 14
			end
			else if (@handleIsBoundToLock = 0)
				set @result = 2
			else
				set @result = 6
		end
		else if (@handleIsBoundToLock = 1)
			set @result = 6
	end

	if (@result != 0 and @result != 2)
		select @result as 'Result', @instanceId, @currentVersion
	else if (@result = 2)
	begin
		select @result as 'Result', @instanceId, [LockOwnersTable].[Id], [LockOwnersTable].[EncodingOption], [PrimitiveLockOwnerData], [ComplexLockOwnerData]
		from [LockOwnersTable]
		join [InstancesTable] on [InstancesTable].[SurrogateLockOwnerId] = [LockOwnersTable].[SurrogateLockOwnerId]
		where [InstancesTable].[SurrogateLockOwnerId] = @currentLockOwnerId and
			  [InstancesTable].[Id] = @instanceId
		
		if (@@rowcount = 0)
			goto TryLockInstance
	end
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[LoadInstance]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[LoadInstance]
	@surrogateLockOwnerId bigint,
	@operationType tinyint,
	@handleInstanceVersion bigint,
	@handleIsBoundToLock bit,
	@keyToLoadBy uniqueidentifier = null,
	@instanceId uniqueidentifier = null,
	@keysToAssociate xml = null,
	@encodingOption tinyint,
	@concatenatedKeyProperties varbinary(max) = null,
	@singleKeyId uniqueidentifier,
	@operationTimeout int
as
begin
	set nocount on
	set transaction isolation level read committed	
	set xact_abort on;		
	set deadlock_priority low
	begin transaction
	
	declare @result int
	declare @lockAcquired bigint
	declare @isInitialized bit
	declare @createKey bit
	declare @createdInstance bit
	declare @keyIsAssociated bit
	declare @loadedByKey bit
	declare @now datetime
	declare @surrogateInstanceId bigint

	set @createdInstance = 0
	set @isInitialized = 0
	set @keyIsAssociated = 0
	set @result = 0
	set @surrogateInstanceId = null
	
	exec @lockAcquired = sp_getapplock @Resource = 'InstanceStoreLock', @LockMode = 'Shared', @LockTimeout = @operationTimeout
	
	if (@lockAcquired < 0)
	begin
		set @result = 13
		select @result as 'Result'
	end
	
	if (@result = 0)
	begin
		set @now = getutcdate()

		if (@operationType = 0) or (@operationType = 2)
		begin
MapKeyToInstanceId:
			set @loadedByKey = 0
			set @createKey = 0
			
			select @surrogateInstanceId = [SurrogateInstanceId],
				   @keyIsAssociated = [IsAssociated]
			from [KeysTable]
			where [Id] = @keyToLoadBy
			
			if (@@rowcount = 0)
			begin
				if (@operationType = 2)
				begin
					set @result = 4
					select @result as 'Result', @keyToLoadBy 
				end
					set @createKey = 1
			end
			else if (@keyIsAssociated = 0)
			begin
				set @result = 8
				select @result as 'Result', @keyToLoadBy
			end
			else
			begin
				select @instanceId = [Id]
				from [InstancesTable]
				where [SurrogateInstanceId] = @surrogateInstanceId

				if (@@rowcount = 0)
					goto MapKeyToInstanceId

				set @loadedByKey = 1
			end
		end
	end

	if (@result = 0)
	begin
LockOrCreateInstance:
		exec [System.Activities.DurableInstancing].[LockInstance] @instanceId, @surrogateLockOwnerId, @handleInstanceVersion, @handleIsBoundToLock, @surrogateInstanceId output, null, @result output
														  
		if (@result = 0 and @surrogateInstanceId = 0)
		begin
			if (@loadedByKey = 1)
				goto MapKeyToInstanceId
			
			if (@operationType > 1)
			begin
				set @result = 1
				select @result as 'Result', @instanceId as 'InstanceId'
			end
			else
			begin				
				exec [System.Activities.DurableInstancing].[CreateInstance] @instanceId, @surrogateLockOwnerId, null, null, @surrogateInstanceId output, @result output
			
				if (@result = 0 and @surrogateInstanceId = 0)
					goto LockOrCreateInstance
				else if (@surrogateInstanceId > 0)
					set @createdInstance = 1
			end
		end
		else if (@result = 0)
		begin
			delete from [RunnableInstancesTable]
			where [SurrogateInstanceId] = @surrogateInstanceId
		end
	end
		
	if (@result = 0)
	begin
		if (@createKey = 1) 
		begin
			select @isInitialized = [IsInitialized]
			from [InstancesTable]
			where [SurrogateInstanceId] = @surrogateInstanceId
			
			if (@isInitialized = 1)
			begin
				set @result = 5
				select @result as 'Result', @instanceId
			end
			else
			begin													
				insert into [KeysTable] ([Id], [SurrogateInstanceId], [IsAssociated])
				values (@keyToLoadBy, @surrogateInstanceId, 1)
				
				if (@@rowcount = 0)
				begin
					if (@createdInstance = 1)
					begin
						delete [InstancesTable]
						where [SurrogateInstanceId] = @surrogateInstanceId
					end
					else
					begin
						update [InstancesTable]
						set [SurrogateLockOwnerId] = null
						where [SurrogateInstanceId] = @surrogateInstanceId
					end
					
					goto MapKeyToInstanceId
				end
			end
		end
		else if (@loadedByKey = 1 and not exists(select [Id] from [KeysTable] where ([Id] = @keyToLoadBy) and ([IsAssociated] = 1)))
		begin
			set @result = 8
			select @result as 'Result', @keyToLoadBy
		end
		
		if (@operationType > 1 and not exists(select [Id] from [InstancesTable] where ([Id] = @instanceId) and ([IsInitialized] = 1)))
		begin
			set @result = 1
			select @result as 'Result', @instanceId as 'InstanceId'
		end
		
		if (@result = 0)
			exec @result = [System.Activities.DurableInstancing].[AssociateKeys] @surrogateInstanceId, @keysToAssociate, @concatenatedKeyProperties, @encodingOption, @singleKeyId
		
		-- Ensure that this key's data will never be overwritten.
		if (@result = 0 and @createKey = 1)
		begin
			update [KeysTable]
			set [EncodingOption] = @encodingOption
			where [Id] = @keyToLoadBy
		end
	end
	
	if (@result != 13)
		exec sp_releaseapplock @Resource = 'InstanceStoreLock'
		
	if (@result = 0)
	begin
		select @result as 'Result',
			   [Id],
			   [SurrogateInstanceId],
			   [PrimitiveDataProperties],
			   [ComplexDataProperties],
			   [MetadataProperties],
			   [DataEncodingOption],
			   [MetadataEncodingOption],
			   [Version],
			   [IsInitialized],
			   @createdInstance
		from [InstancesTable]
		where [SurrogateInstanceId] = @surrogateInstanceId
		
		if (@createdInstance = 0)
		begin
			select @result as 'Result',
				   [EncodingOption],
				   [Change]
			from [InstanceMetadataChangesTable]
			where [SurrogateInstanceId] = @surrogateInstanceId
			order by([ChangeTime])
			
			if (@@rowcount = 0)
			select @result as 'Result', null, null
				
			select @result as 'Result',
				   [Id],
				   [IsAssociated],
				   [EncodingOption],
				   [Properties]
			from [KeysTable] with (index(NCIX_KeysTable_SurrogateInstanceId))
			where ([KeysTable].[SurrogateInstanceId] = @surrogateInstanceId)
			
			if (@@rowcount = 0)
				select @result as 'Result', null, null, null, null
		end

		commit transaction
	end
	else if (@result = 2 or @result = 14)
		commit transaction
	else
		rollback transaction
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[RecoverInstanceLocks]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [System.Activities.DurableInstancing].[RecoverInstanceLocks]
as
begin
	set nocount on;
	set transaction isolation level read committed;
	set xact_abort on;
	set deadlock_priority low;
    
	declare @now as datetime
	set @now = getutcdate()	
	
	insert into [RunnableInstancesTable] ([SurrogateInstanceId], [WorkflowHostType], [ServiceDeploymentId], [RunnableTime])
		select top (1000) instances.[SurrogateInstanceId], instances.[WorkflowHostType], instances.[ServiceDeploymentId], @now
		from [LockOwnersTable] lockOwners with (readpast) inner loop join
			 [InstancesTable] instances with (readpast)
				on instances.[SurrogateLockOwnerId] = lockOwners.[SurrogateLockOwnerId]
			where 
				lockOwners.[LockExpiration] <= @now and
				instances.[IsInitialized] = 1 and
				instances.[IsSuspended] = 0

	delete from [LockOwnersTable] with (readpast)
	from [LockOwnersTable] lockOwners
	where [LockExpiration] <= @now
	and not exists
	(
		select top (1) 1
		from [InstancesTable] instances with (nolock)
		where instances.[SurrogateLockOwnerId] = lockOwners.[SurrogateLockOwnerId]
	)
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[SaveInstance]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[SaveInstance]
	@instanceId uniqueidentifier,
	@surrogateLockOwnerId bigint,
	@handleInstanceVersion bigint,
	@handleIsBoundToLock bit,
	@primitiveDataProperties varbinary(max),
	@complexDataProperties varbinary(max),
	@writeOnlyPrimitiveDataProperties varbinary(max),
	@writeOnlyComplexDataProperties varbinary(max),
	@metadataProperties varbinary(max),
	@metadataIsConsistent bit,
	@encodingOption tinyint,
	@timerDurationMilliseconds bigint,
	@suspensionStateChange tinyint,
	@suspensionReason nvarchar(max),
	@suspensionExceptionName nvarchar(450),
	@keysToAssociate xml,
	@keysToComplete xml,
	@keysToFree xml,
	@concatenatedKeyProperties varbinary(max),
	@unlockInstance bit,
	@isReadyToRun bit,
	@isCompleted bit,
	@singleKeyId uniqueidentifier,
	@lastMachineRunOn nvarchar(450),
	@executionStatus nvarchar(450),
	@blockingBookmarks nvarchar(max),
	@workflowHostType uniqueidentifier,
	@serviceDeploymentId bigint,
	@operationTimeout int
as
begin
	set nocount on
	set transaction isolation level read committed		
	set xact_abort on;	

	declare @currentInstanceVersion bigint
	declare @deleteInstanceOnCompletion bit
	declare @enqueueCommand bit
	declare @isSuspended bit
	declare @lockAcquired bigint
	declare @metadataUpdateOnly bit
	declare @now datetime
	declare @result int
	declare @surrogateInstanceId bigint
	declare @pendingTimer datetime
	
	set @result = 0
	set @metadataUpdateOnly = 0
	
	exec @lockAcquired = sp_getapplock @Resource = 'InstanceStoreLock', @LockMode = 'Shared', @LockTimeout = @operationTimeout
		
	if (@lockAcquired < 0)
	begin
		select @result as 'Result'
		set @result = 13
	end
	
	set @now = getutcdate()
	
	if (@primitiveDataProperties is null and @complexDataProperties is null and @writeOnlyPrimitiveDataProperties is null and @writeOnlyComplexDataProperties is null)
		set @metadataUpdateOnly = 1

LockOrCreateInstance:
	if (@result = 0)
	begin
		exec [System.Activities.DurableInstancing].[LockInstance] @instanceId, @surrogateLockOwnerId, @handleInstanceVersion, @handleIsBoundToLock, @surrogateInstanceId output, @currentInstanceVersion output, @result output
															  
		if (@result = 0 and @surrogateInstanceId = 0)
		begin
			exec [System.Activities.DurableInstancing].[CreateInstance] @instanceId, @surrogateLockOwnerId, @workflowHostType, @serviceDeploymentId, @surrogateInstanceId output, @result output
			
			if (@result = 0 and @surrogateInstanceId = 0)
				goto LockOrCreateInstance
			
			set @currentInstanceVersion = 1
		end
	end
	
	if (@result = 0)
	begin
		select @enqueueCommand = [EnqueueCommand],
			   @deleteInstanceOnCompletion = [DeletesInstanceOnCompletion]
		from [LockOwnersTable]
		where ([SurrogateLockOwnerId] = @surrogateLockOwnerId)
		
		if (@isCompleted = 1 and @deleteInstanceOnCompletion = 1)
		begin
			exec [System.Activities.DurableInstancing].[DeleteInstance] @surrogateInstanceId
			goto Finally
		end
		
		update [InstancesTable] 
		set @instanceId = [InstancesTable].[Id],
			@workflowHostType = [WorkflowHostType] = 
					case when (@workflowHostType is null)
						then [WorkflowHostType]
						else @workflowHostType 
					end,
			@serviceDeploymentId = [ServiceDeploymentId] = 
					case when (@serviceDeploymentId is null)
						then [ServiceDeploymentId]
						else @serviceDeploymentId 
					end,
			@pendingTimer = [PendingTimer] = 
					case when (@metadataUpdateOnly = 1)
						then [PendingTimer]
						else [System.Activities.DurableInstancing].[GetExpirationTime](@timerDurationMilliseconds)
					end,
			@isReadyToRun = [IsReadyToRun] = 
					case when (@metadataUpdateOnly = 1)
						then [IsReadyToRun]
						else @isReadyToRun
					end,
			@isSuspended = [IsSuspended] = 
					case when (@suspensionStateChange = 0) then [IsSuspended]
						 when (@suspensionStateChange = 1) then 1
						 else 0
					end,
			[SurrogateLockOwnerId] = case when (@unlockInstance = 1 or @isCompleted = 1)
										then null
										else @surrogateLockOwnerId
									 end,
			[PrimitiveDataProperties] = case when (@metadataUpdateOnly = 1)
										then [PrimitiveDataProperties]
										else @primitiveDataProperties
									   end,
			[ComplexDataProperties] = case when (@metadataUpdateOnly = 1)
										then [ComplexDataProperties]
										else @complexDataProperties
									   end,
			[WriteOnlyPrimitiveDataProperties] = case when (@metadataUpdateOnly = 1)
										then [WriteOnlyPrimitiveDataProperties]
										else @writeOnlyPrimitiveDataProperties
									   end,
			[WriteOnlyComplexDataProperties] = case when (@metadataUpdateOnly = 1)
										then [WriteOnlyComplexDataProperties]
										else @writeOnlyComplexDataProperties
									   end,
			[MetadataProperties] = case
									 when (@metadataIsConsistent = 1) then @metadataProperties
									 else [MetadataProperties]
								   end,
			[SuspensionReason] = case
									when (@suspensionStateChange = 0) then [SuspensionReason]
									when (@suspensionStateChange = 1) then @suspensionReason
									else null
								 end,
			[SuspensionExceptionName] = case
									when (@suspensionStateChange = 0) then [SuspensionExceptionName]
									when (@suspensionStateChange = 1) then @suspensionExceptionName
									else null
								 end,
			[IsCompleted] = @isCompleted,
			[IsInitialized] = case
								when (@metadataUpdateOnly = 0) then 1
								else [IsInitialized]
							  end,
			[DataEncodingOption] = case
									when (@metadataUpdateOnly = 0) then @encodingOption
									else [DataEncodingOption]
								   end,
			[MetadataEncodingOption] = case
									when (@metadataIsConsistent = 1) then @encodingOption
									else [MetadataEncodingOption]
								   end,
			[BlockingBookmarks] = case
									when (@metadataUpdateOnly = 0) then @blockingBookmarks
									else [BlockingBookmarks]
								  end,
			[LastUpdated] = @now,
			[LastMachineRunOn] = case
									when (@metadataUpdateOnly = 0) then @lastMachineRunOn
									else [LastMachineRunOn]
								 end,
			[ExecutionStatus] = case
									when (@metadataUpdateOnly = 0) then @executionStatus
									else [ExecutionStatus]
								end
		from [InstancesTable]		
		where ([InstancesTable].[SurrogateInstanceId] = @surrogateInstanceId)
	
		if (@@rowcount = 0)
		begin
			set @result = 99
			select @result as 'Result' 
		end
		else
		begin
			if (@keysToAssociate is not null or @singleKeyId is not null)
				exec @result = [System.Activities.DurableInstancing].[AssociateKeys] @surrogateInstanceId, @keysToAssociate, @concatenatedKeyProperties, @encodingOption, @singleKeyId
			
			if (@result = 0 and @keysToComplete is not null)
				exec @result = [System.Activities.DurableInstancing].[CompleteKeys] @surrogateInstanceId, @keysToComplete
			
			if (@result = 0 and @keysToFree is not null)
				exec @result = [System.Activities.DurableInstancing].[FreeKeys] @surrogateInstanceId, @keysToFree
			
			if (@result = 0) and (@metadataUpdateOnly = 0)
			begin
				delete from [InstancePromotedPropertiesTable]
				where [SurrogateInstanceId] = @surrogateInstanceId
			end
			
			if (@result = 0)
			begin
				if (@metadataIsConsistent = 1)
				begin
					delete from [InstanceMetadataChangesTable]
					where [SurrogateInstanceId] = @surrogateInstanceId
				end
				else if (@metadataProperties is not null)
				begin
					insert into [InstanceMetadataChangesTable] ([SurrogateInstanceId], [EncodingOption], [Change])
					values (@surrogateInstanceId, @encodingOption, @metadataProperties)
				end
			end
			
			if (@result = 0 and @unlockInstance = 1 and @isCompleted = 0)
				exec [System.Activities.DurableInstancing].[InsertRunnableInstanceEntry] @surrogateInstanceId, @workflowHostType, @serviceDeploymentId, @isSuspended, @isReadyToRun, @pendingTimer				
		end
	end

Finally:
	if (@result != 13)
		exec sp_releaseapplock @Resource = 'InstanceStoreLock'
	
	if (@result = 0)
		select @result as 'Result', @currentInstanceVersion

	return @result
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[TryLoadRunnableInstance]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [System.Activities.DurableInstancing].[TryLoadRunnableInstance]
	@surrogateLockOwnerId bigint,
	@workflowHostType uniqueidentifier,
	@operationType tinyint,
	@handleInstanceVersion bigint,
	@handleIsBoundToLock bit,
	@encodingOption tinyint,	
	@operationTimeout int
as
begin
	set nocount on
	set transaction isolation level read committed	
	set xact_abort on;	
	set deadlock_priority low
	begin tran
	
	declare @instanceId uniqueIdentifier
	declare @now datetime
	set @now = getutcdate()
	
	select top (1) @instanceId = instances.[Id]
	from [RunnableInstancesTable] runnableInstances with (readpast, updlock)
		inner loop join [InstancesTable] instances with (readpast, updlock)
		on runnableInstances.[SurrogateInstanceId] = instances.[SurrogateInstanceId]
	where runnableInstances.[WorkflowHostType] = @workflowHostType
		  and 
	      runnableInstances.[RunnableTime] <= @now
    
    if (@@rowcount = 1)
    begin
		select 0 as 'Result', cast(1 as bit)				
		exec [System.Activities.DurableInstancing].[LoadInstance] @surrogateLockOwnerId, @operationType, @handleInstanceVersion, @handleIsBoundToLock, null, @instanceId, null, @encodingOption, null, null, @operationTimeout
    end	
    else
    begin
		select 0 as 'Result', cast(0 as bit)
    end
    
    if (@@trancount > 0)
    begin
		commit tran
    end
end



GO
/****** Object:  StoredProcedure [System.Activities.DurableInstancing].[UnlockInstance]    Script Date: 08/23/2012 08:19:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [System.Activities.DurableInstancing].[UnlockInstance]
	@surrogateLockOwnerId bigint,
	@instanceId uniqueidentifier,
	@handleInstanceVersion bigint
as
begin
	set nocount on
	set transaction isolation level read committed		
	set xact_abort on;	
	begin transaction
	
	declare @pendingTimer datetime
	declare @surrogateInstanceId bigint
	declare @workflowHostType uniqueidentifier
	declare @serviceDeploymentId bigint
	declare @enqueueCommand bit	
	declare @isReadyToRun bit	
	declare @isSuspended bit
	declare @now datetime
	
	set @now = getutcdate()
		
	update [InstancesTable]
	set [SurrogateLockOwnerId] = null,
	    @surrogateInstanceId = [SurrogateInstanceId],
	    @workflowHostType = [WorkflowHostType],
   	    @serviceDeploymentId = [ServiceDeploymentId],
	    @pendingTimer = [PendingTimer],
	    @isReadyToRun =  [IsReadyToRun],
	    @isSuspended = [IsSuspended]
	where [Id] = @instanceId and
		  [SurrogateLockOwnerId] = @surrogateLockOwnerId and
		  [Version] = @handleInstanceVersion
	
	exec [System.Activities.DurableInstancing].[InsertRunnableInstanceEntry] @surrogateInstanceId, @workflowHostType, @serviceDeploymentId, @isSuspended, @isReadyToRun, @pendingTimer    
	
	commit transaction
end



GO
GRANT SELECT ON [System.Activities.DurableInstancing].[InstancePromotedProperties] TO [System.Activities.DurableInstancing.InstanceStoreObservers] AS [dbo]
GO
GRANT SELECT ON [System.Activities.DurableInstancing].[Instances] TO [System.Activities.DurableInstancing.InstanceStoreObservers] AS [dbo]
GO
GRANT DELETE ON [System.Activities.DurableInstancing].[Instances] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
GRANT SELECT ON [System.Activities.DurableInstancing].[ServiceDeployments] TO [System.Activities.DurableInstancing.InstanceStoreObservers] AS [dbo]
GO
GRANT DELETE ON [System.Activities.DurableInstancing].[ServiceDeployments] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[DeleteInstance] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[CreateLockOwner] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[CreateLockOwner] TO [System.Activities.DurableInstancing.WorkflowActivationUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[CreateServiceDeployment] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[DeleteLockOwner] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[DeleteLockOwner] TO [System.Activities.DurableInstancing.WorkflowActivationUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[DetectRunnableInstances] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[ExtendLock] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[ExtendLock] TO [System.Activities.DurableInstancing.WorkflowActivationUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[GetActivatableWorkflowsActivationParameters] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[GetActivatableWorkflowsActivationParameters] TO [System.Activities.DurableInstancing.WorkflowActivationUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[InsertPromotedProperties] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[LoadInstance] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[RecoverInstanceLocks] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[RecoverInstanceLocks] TO [System.Activities.DurableInstancing.WorkflowActivationUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[SaveInstance] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[TryLoadRunnableInstance] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
GRANT EXECUTE ON [System.Activities.DurableInstancing].[UnlockInstance] TO [System.Activities.DurableInstancing.InstanceStoreUsers] AS [dbo]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [PrimitiveDataProperties]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [ComplexDataProperties]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [WriteOnlyPrimitiveDataProperties]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [WriteOnlyComplexDataProperties]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [MetadataProperties]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT ((0)) FOR [DataEncodingOption]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT ((0)) FOR [MetadataEncodingOption]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [LastUpdated]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [SuspensionExceptionName]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [SuspensionReason]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [BlockingBookmarks]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [LastMachineRunOn]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT (NULL) FOR [ExecutionStatus]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT ((0)) FOR [IsInitialized]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT ((0)) FOR [IsSuspended]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT ((0)) FOR [IsReadyToRun]
GO
ALTER TABLE [System.Activities.DurableInstancing].[InstancesTable] ADD  DEFAULT ((0)) FOR [IsCompleted]
GO
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] ADD  DEFAULT (NULL) FOR [PrimitiveLockOwnerData]
GO
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] ADD  DEFAULT (NULL) FOR [ComplexLockOwnerData]
GO
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] ADD  DEFAULT (NULL) FOR [WriteOnlyPrimitiveLockOwnerData]
GO
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] ADD  DEFAULT (NULL) FOR [WriteOnlyComplexLockOwnerData]
GO
ALTER TABLE [System.Activities.DurableInstancing].[LockOwnersTable] ADD  DEFAULT ((0)) FOR [EncodingOption]
GO

/****** Object:  Table [dbo].[DatabaseVersionSet]    Script Date: 01/28/2013 16:04:51 ******/
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

/****** Object:  StoredProcedure [dbo].[DatabaseVersion_Insert]    Script Date: 01/28/2013 16:05:37 ******/
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

ALTER DATABASE [Gorba_CenterControllers] SET allow_snapshot_isolation ON
GO
ALTER DATABASE [Gorba_CenterControllers] SET READ_COMMITTED_SNAPSHOT ON;
GO




--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterControllers].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.1.29.1'
  ,@description = 'Refactoring for the code quality. Removed unused stored procedures. Added versioning'
  ,@versionMajor = 1
  ,@versionMinor = 1
  ,@versionBuild = 29
  ,@versionRevision = 1
  ,@dateCreated = '2013-01-28 14:30:00'
GO

EXECUTE @RC = [Gorba_CenterControllers].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.5.201410.2'
  ,@description = 'Updated snapshot isolation settings.'
  ,@versionMajor = 1
  ,@versionMinor = 5
  ,@versionBuild = 201410
  ,@versionRevision = 2
  ,@dateCreated = '2014-05-19T09:00:00.000'
GO
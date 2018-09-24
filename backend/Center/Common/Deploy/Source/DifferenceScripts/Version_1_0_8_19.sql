 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[AssociationUnitStationSet] DROP CONSTRAINT [FK_AssociationUnitStationSet_UnitSet]
GO
ALTER TABLE [dbo].[AssociationUnitOperationSet] DROP CONSTRAINT [FK_AssociationUnitOperationSet_UnitSet]
GO
ALTER TABLE [dbo].[WorkflowInstanceSet] DROP CONSTRAINT [FK_WorkflowInstanceSet_UnitSet]
GO
ALTER TABLE [dbo].[TimeTableEntrySet] DROP CONSTRAINT [FK_TimeTableEntrySet_UnitSet]
GO
ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [FK_AlarmSet_UnitSet]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [FK_UnitSet_ProductTypeSet1]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [FK_UnitSet_ProductTypeSet]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [FK_UnitSet_UnitGroupSet]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [FK_UnitSet_TenanttSet]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [FK_UnitSet_LayoutSet]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [DF_UnitSet_IsDeleted]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [DF_UnitSet_IsOnline]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [PK_UnitSet]
GO
DROP INDEX [IX_UnitSetUniqueName] ON [dbo].[UnitSet]
GO
CREATE TABLE [dbo].[TempUnitSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[TenantId] [int] NOT NULL,
	[ProductTypeId] [int] NOT NULL,
	[GroupId] [int] NULL,
	[LayoutId] [int] NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ShortName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SystemName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SerialNumber] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_UnitSet_IsDeleted] DEFAULT ((0)),
	[NetworkAddress] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsOnline] [bit] NOT NULL CONSTRAINT [DF_UnitSet_IsOnline] DEFAULT ((0)),
	[LastSeenOnline] [datetime] NULL,
	[LocationName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AlarmStatus] [int] NOT NULL,
	[CommunicationStatus] [int] NOT NULL,
	[OperationStatus] [int] NOT NULL,
	[LastRestartRequestDate] [datetime] NULL,
	[LastTimeSyncRequestDate] [datetime] NULL,
	[LastTimeSyncValue] [datetime] NULL,
	[TimeZoneInfoId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempUnitSet] ON
INSERT INTO [dbo].[TempUnitSet] ([Id],[TenantId],[ProductTypeId],[GroupId],[LayoutId],[Name],[ShortName],[SystemName],[SerialNumber],[Description],[DateCreated],[DateModified],[IsDeleted],[NetworkAddress],[IsOnline],[LastSeenOnline],[AlarmStatus],[CommunicationStatus],[OperationStatus]) SELECT [Id],[TenantId],[ProductTypeId],[GroupId],[LayoutId],[Name],[ShortName],[SystemName],[SerialNumber],[Description],[DateCreated],[DateModified],[IsDeleted],[NetworkAddress],[IsOnline],[LastSeenOnline],0,0,0 FROM [dbo].[UnitSet]
SET IDENTITY_INSERT [dbo].[TempUnitSet] OFF
GO

DROP TABLE [dbo].[UnitSet]
GO
EXEC sp_rename N'[dbo].[TempUnitSet]',N'UnitSet', 'OBJECT'
GO


ALTER VIEW [dbo].[Units]
AS
SELECT [Id]
      ,[TenantId]
      ,[ProductTypeId]
      ,[GroupId]
      ,[LayoutId]
      ,[Name]
      ,[ShortName]
      ,[SystemName]
      ,[SerialNumber]
      ,[Description]
      ,[DateCreated]
      ,[DateModified]
      ,[NetworkAddress]
      ,[IsOnline]
      ,[LastSeenOnline]
      ,[LocationName]
      ,[AlarmStatus]
      ,[CommunicationStatus]
      ,[OperationStatus]
      ,[LastRestartRequestDate]
      ,[LastTimeSyncRequestDate]
      ,[LastTimeSyncValue]
      , [TimeZoneInfoId]

FROM         dbo.UnitSet
WHERE     (IsDeleted = 0)
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 14.02.2012
-- Description:	Updates a Unit
-- =============================================
ALTER PROCEDURE [dbo].[Unit_Update]
	(@id int,
	@tenantId int,
	@productTypeId int,
	@groupId int = NULL,
	@layoutId int = NULL,
	@name varchar(100),
	@shortName varchar(50) = NULL,
	@systemName varchar(100) = NULL,
	@serialNumber varchar(50) = NULL,
	@description varchar(500) = NULL,
	@networkAddress varchar(256) = NULL,
	@isOnline bit = 0,
	@lastSeenOnline datetime = NULL,
	@dateModified datetime = NULL
	, @locationName varchar(100)
	, @alarmStatus int
	, @communicationStatus int
	, @operationStatus int
	, @lastRestartRequestDate datetime
	, @lastTimeSyncRequestDate datetime
	, @lastTimeSyncValue datetime
	, @timeZoneInfoId varchar(100))
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Unit_EditTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateModified IS NULL
			BEGIN
				SET @dateModified = GETUTCDATE()
			END
			
			UPDATE [UnitSet] 
			SET [TenantId] = @tenantId
			, [ProductTypeId] = @productTypeId
			, [GroupId] = @groupId
			, [LayoutId] = @layoutId
			, [Name] = @name
			, [ShortName] = @shortName
			, [SystemName] = @systemName
			, [SerialNumber] = @serialNumber
			, [NetworkAddress] = @networkAddress
			, [Description] = @description
			, [IsOnline] = @isOnline
			, [LastSeenOnline] = @lastSeenOnline
			, [DateModified] = @dateModified
			, [LocationName] = @locationName
			, [AlarmStatus] = @alarmStatus
			, [CommunicationStatus] = @communicationStatus
			, [OperationStatus] = @operationStatus
			, [LastRestartRequestDate] = @lastRestartRequestDate
			, [LastTimeSyncRequestDate] = @lastTimeSyncRequestDate
			, [LastTimeSyncValue] = @lastTimeSyncValue
			, [TimeZoneInfoId] = @timeZoneInfoId
			WHERE [Id] = @id
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
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 05.12.11 15:40
-- Description:	Adds a unit to the system
-- =============================================
ALTER PROCEDURE [dbo].[Unit_Insert]
(
	@tenantId int,
	@productTypeId int,
	@groupId int = NULL,
	@layoutId int = NULL,
	@name varchar(100),
	@shortName varchar(50) = NULL,
	@systemName varchar(100) = NULL,
	@serialNumber varchar(50) = NULL,
	@networkAddress varchar(256) = NULL,
	@description varchar(500) = NULL,
	@dateCreated datetime = NULL
	, @locationName varchar(100)
	, @alarmStatus int
	, @communicationStatus int
	, @operationStatus int
	, @lastRestartRequestDate datetime
	, @lastTimeSyncRequestDate datetime
	, @lastTimeSyncValue datetime
	, @timeZoneInfoId varchar(100)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Unit_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [UnitSet]
			([TenantId]
			, [ProductTypeId]
			, [GroupId]
			, [LayoutId]
			, [Name]
			, [ShortName]
			, [SystemName]
			, [SerialNumber]
			, [NetworkAddress]
			, [Description]
			, [DateCreated]
			, [LocationName]
			, [AlarmStatus]
			, [CommunicationStatus]
			, [OperationStatus]
			, [LastRestartRequestDate]
			, [LastTimeSyncRequestDate]
			, [LastTimeSyncValue]
			, [TimeZoneInfoId])
			VALUES
			(@tenantId
			, @productTypeId
			, @groupId
			, @layoutId
			, @name
			, @shortName
			, @systemName
			, @serialNumber
			, @networkAddress
			, @description
			, @dateCreated
			, @locationName
			, @alarmStatus
			, @communicationStatus
			, @operationStatus
			, @lastRestartRequestDate
			, @lastTimeSyncRequestDate
			, @lastTimeSyncValue
			, @timeZoneInfoId)
			
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
CREATE UNIQUE NONCLUSTERED INDEX [IX_UnitSetUniqueName] ON [dbo].[UnitSet]
(
	[TenantId] ASC,
	[Name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [PK_UnitSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AssociationUnitStationSet] ADD CONSTRAINT [FK_AssociationUnitStationSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[AlarmSet] ADD CONSTRAINT [FK_AlarmSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[AssociationUnitOperationSet] ADD CONSTRAINT [FK_AssociationUnitOperationSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[WorkflowInstanceSet] ADD CONSTRAINT [FK_WorkflowInstanceSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[TimeTableEntrySet] ADD CONSTRAINT [FK_TimeTableEntrySet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_TenanttSet] FOREIGN KEY
	(
		[TenantId]
	)
	REFERENCES [dbo].[TenantSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_UnitGroupSet] FOREIGN KEY
	(
		[GroupId]
	)
	REFERENCES [dbo].[UnitGroupSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_LayoutSet] FOREIGN KEY
	(
		[LayoutId]
	)
	REFERENCES [dbo].[LayoutSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_ProductTypeSet] FOREIGN KEY
	(
		[ProductTypeId]
	)
	REFERENCES [dbo].[ProductTypeSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_ProductTypeSet1] FOREIGN KEY
	(
		[ProductTypeId]
	)
	REFERENCES [dbo].[ProductTypeSet]
	(
		[Id]
	)
GO

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.8.19'
  ,@description = 'Added fields to the unit table.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 8
  ,@versionRevision = 19
  ,@dateCreated = '2012-03-28T10:10:00.000'
GO


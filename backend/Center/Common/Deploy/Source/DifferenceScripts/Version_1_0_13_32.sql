 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [FK_AlarmSet_UserSet]
GO
ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [FK_AlarmSet_UnitSet]
GO
ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [DF_AlarmSet_IsDeleted]
GO
ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [PK_AlarmSet]
GO
ALTER TABLE [dbo].[UnitSet] DROP COLUMN [AlarmStatus]
GO
CREATE TABLE [dbo].[TempAlarmSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[UnitId] [int] NOT NULL,
	[UserId] [int] NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Severity] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[ConfirmationText] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[DateConfirmed] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_AlarmSet_IsDeleted] DEFAULT ((0)),
	[DateModified] [datetime] NULL,
	[DateReceived] [datetime] NOT NULL,
	[UnconfirmedSince] AS (case when [DateConfirmed] IS NULL then [DateReceived] else '9999-12-31 23:59:59.997' end)

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempAlarmSet] ON
INSERT INTO [dbo].[TempAlarmSet] ([Id],[Name],[UnitId],[UserId],[Description],[Severity],[Type],[ConfirmationText],[DateCreated],[EndDate],[DateConfirmed],[IsDeleted],[DateModified],[DateReceived]) SELECT [Id],[Name],[UnitId],[UserId],[Description],[Severity],[Type],[ConfirmationText],[DateCreated],[EndDate],[DateConfirmed],[IsDeleted],[DateModified],[DateReceived] FROM [dbo].[AlarmSet]
SET IDENTITY_INSERT [dbo].[TempAlarmSet] OFF
GO

DROP TABLE [dbo].[AlarmSet]
GO
EXEC sp_rename N'[dbo].[TempAlarmSet]',N'AlarmSet', 'OBJECT'
GO


ALTER VIEW [dbo].[Units]
AS
WITH AlarmStatuses([Status], [UnitId])
AS
(
	SELECT  SUM(DISTINCT [A].[Type]) [Status], [A].[UnitId]
	FROM [AlarmSet] [A]
	GROUP BY [A].[UnitId]
)
SELECT [U].[Id]
      ,[U].[TenantId]
      ,[U].[ProductTypeId]
      ,[U].[GroupId]
      ,[U].[LayoutId]
      ,[U].[StationId]
      ,[U].[Name]
      ,[U].[ShortName]
      ,[U].[SystemName]
      ,[U].[SerialNumber]
      ,[U].[Description]
      ,[U].[DateCreated]
      ,[U].[DateModified]
      ,[U].[NetworkAddress]
      ,[U].[IsOnline]
      ,[U].[LastSeenOnline]
      ,[U].[LocationName]
      ,[U].[CommunicationStatus]
      ,[U].[OperationStatus]
      ,[U].[LastRestartRequestDate]
      ,[U].[LastTimeSyncRequestDate]
      ,[U].[LastTimeSyncValue]
      ,[U].[TimeZoneInfoId]
      , [U].[GatewayAddress]
      , ISNULL(CAST([A].[Status] AS int), 0) AS [AlarmStatus]

FROM         [dbo].[UnitSet] [U]
LEFT OUTER JOIN [AlarmStatuses] [A] ON [A].[UnitId] = [U].[Id]

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
	@layoutId int = NULL
	, @stationId int
	, @name varchar(100),
	@shortName varchar(50) = NULL,
	@systemName varchar(100) = NULL,
	@serialNumber varchar(50) = NULL,
	@description varchar(500) = NULL,
	@networkAddress varchar(256) = NULL,
	@isOnline bit = 0,
	@lastSeenOnline datetime = NULL,
	@dateModified datetime = NULL
	, @locationName varchar(100)
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
			, [StationId] = @stationId
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
	@layoutId int = NULL
	, @stationId int
	, @name varchar(100),
	@shortName varchar(50) = NULL,
	@systemName varchar(100) = NULL,
	@serialNumber varchar(50) = NULL,
	@networkAddress varchar(256) = NULL,
	@description varchar(500) = NULL,
	@dateCreated datetime = NULL
	, @locationName varchar(100)
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
			, [StationId]
			, [Name]
			, [ShortName]
			, [SystemName]
			, [SerialNumber]
			, [NetworkAddress]
			, [Description]
			, [DateCreated]
			, [LocationName]
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
			, @stationId
			, @name
			, @shortName
			, @systemName
			, @serialNumber
			, @networkAddress
			, @description
			, @dateCreated
			, @locationName
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
			
			DECLARE @line varchar(5) = ERROR_LINE()
			DECLARE @message varchar(500) = ERROR_MESSAGE()
			DECLARE @severity int = ERROR_SEVERITY()
			print 'Error on line ' + @line + ': ' + @message
			--RAISERROR(@message, @severity, 1)

	END CATCH
END
GO
ALTER TABLE [dbo].[AlarmSet] ADD CONSTRAINT [PK_AlarmSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
ALTER TABLE [dbo].[AlarmSet] ADD CONSTRAINT [FK_AlarmSet_UserSet] FOREIGN KEY
	(
		[UserId]
	)
	REFERENCES [dbo].[UserSet]
	(
		[Id]
	)
GO

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.13.32'
  ,@description = 'Updated Units view to compute the AlarmStatus column. Changed the logic of the UnconfirmedSince column.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 13
  ,@versionRevision = 32
  ,@dateCreated = '2012-06-05T07:10:00.000'
GO
 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [FK_AlarmSet_UnitSet]
GO
ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [FK_AlarmSet_UserSet]
GO
ALTER TABLE [dbo].[ItcsFilterSet] DROP CONSTRAINT [FK_ItcsFilterSet_ItcsConfigurationSet]
GO
ALTER TABLE [dbo].[ItcsFilterSet] DROP CONSTRAINT [FK_ItcsFilterSet_StationSet]
GO
ALTER TABLE [dbo].[ItcsFilterSet] DROP CONSTRAINT [FK_ItcsFilterSet_ItcsStationReferenceSet]
GO
ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [DF_AlarmSet_IsDeleted]
GO
ALTER TABLE [dbo].[ItcsFilterSet] DROP CONSTRAINT [PK_ItcsFilterSet]
GO
ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [PK_AlarmSet]
GO
ALTER TABLE [dbo].[OperationSet] ADD 
[RevokedOn] [datetime] NULL,
[RevokedBy] [int] NULL,
[IsRevoked] AS (case when [RevokedOn] IS NULL then (0) else (1) end) PERSISTED NOT NULL
GO
ALTER TABLE [dbo].[OperationSet] DROP COLUMN
[IsFavorite]
GO
CREATE TABLE [dbo].[SwitchDisplayStateActivitySet]
(
	[Id] [int] NOT NULL,
	[NewState] [int] NOT NULL,
	CONSTRAINT [PK_SwitchDisplayStateActivitySet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AnnouncementTextSet]
(
	[AnnouncementId] [int] NOT NULL,
	[Language] [char] (2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Text] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT [PK_AnnouncementTextSet] PRIMARY KEY CLUSTERED
	(
		[AnnouncementId] ASC,
		[Language] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[OperationFavoriteSet]
(
	[TenantId] [int] NOT NULL,
	[OperationId] [int] NOT NULL,
	CONSTRAINT [PK_OperationFavoriteSet] PRIMARY KEY CLUSTERED
	(
		[TenantId] ASC,
		[OperationId] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[StopPointSet]
(
	[Id] [int] NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT [PK_StopPointSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AnnouncementActivitySet]
(
	[Id] [int] NOT NULL,
	[Interval] [time](7) NOT NULL,
	CONSTRAINT [PK_AnnouncementActivitySet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TempItcsFilterSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[StopPointId] [int] NOT NULL,
	[ItcsConfigurationId] [int] NOT NULL,
	[ItcsStationReferenceId] [int] NOT NULL,
	[LineName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LineReferenceName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Direction] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempItcsFilterSet] ON
INSERT INTO [dbo].[TempItcsFilterSet] ([Id],[ItcsConfigurationId],[ItcsStationReferenceId],[LineName],[LineReferenceName],[Direction],[StopPointId]) SELECT [Id],[ItcsConfigurationId],[ItcsStationReferenceId],[LineName],[LineReferenceName],[Direction],0 FROM [dbo].[ItcsFilterSet]
SET IDENTITY_INSERT [dbo].[TempItcsFilterSet] OFF
GO

DROP TABLE [dbo].[ItcsFilterSet]
GO
EXEC sp_rename N'[dbo].[TempItcsFilterSet]',N'ItcsFilterSet', 'OBJECT'
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
	[UnconfirmedSince] AS (case when [DateConfirmed] IS NULL then [DateReceived]  end) PERSISTED

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempAlarmSet] ON
INSERT INTO [dbo].[TempAlarmSet] ([Id],[Name],[UnitId],[UserId],[Description],[Severity],[Type],[ConfirmationText],[DateCreated],[EndDate],[DateConfirmed],[IsDeleted],[DateModified],[DateReceived]) SELECT [Id],[Name],[UnitId],[UserId],[Description],[Severity],[Type],[ConfirmationText],[DateCreated],[EndDate],[DateConfirmed],[IsDeleted],[DateModified],getdate() FROM [dbo].[AlarmSet]
SET IDENTITY_INSERT [dbo].[TempAlarmSet] OFF
GO

DROP TABLE [dbo].[AlarmSet]
GO
EXEC sp_rename N'[dbo].[TempAlarmSet]',N'AlarmSet', 'OBJECT'
GO


-- =============================================
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 04.01.12
-- Description:	Gets the name for a deleted entity
-- =============================================
ALTER FUNCTION [dbo].[GetDeletedName] 
(
	@id int,
	@entityName varchar(100)
)
RETURNS varchar(100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @len int = LEN(@entityName)
	DECLARE @size int = 8
	DECLARE @idString varchar(8000) = CONVERT(varchar(8000), @id)
	
	DECLARE @normalizedIdString char(8) = REPLICATE('0',@size-LEN(RTRIM(@idString))) + @idString

	DECLARE @finalLen int = 100 - @size - 1 -- I remove the '~' char too
	
	IF @finalLen > @len
		BEGIN
			SET @finalLen = @len
		END
	
	DECLARE @final varchar(100) = '~' + SUBSTRING(@entityName, 1, @finalLen) + @normalizedIdString

	-- Return the result of the function
	RETURN @final

END

;

GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[MD5] 
(
	-- Add the parameters for the function here
	@input varchar(100)
)
RETURNS char(32)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @md5 char(32) = CONVERT(char(32), HashBytes('MD5', @input), 2)

	-- Return the result of the function
	RETURN @md5

END

;

GO
ALTER VIEW [dbo].[InfoLineTextActivities]
AS
SELECT     Base.Id, Base.OperationId, Base.DateCreated, Base.DateModified, Derived.LineNumber, Derived.DestinationId, Derived.DisplayText, 
                      Derived.ExpirationDate, Derived.InfoRowId, Derived.Blink, Derived.DisplayedScreenSide, Derived.Alignment, Derived.Font
FROM         dbo.InfoLineTextActivitySet AS Derived INNER JOIN
                      dbo.ActivitySet AS Base ON Derived.Id = Base.Id
WHERE     (Base.IsDeleted = 0)


;

GO
ALTER VIEW [dbo].[ItcsConfigurations]
AS
SELECT     Id, Name, Description, CollectSystemData, OperationDayStartUtc, OperationDayDuration, UtcOffset, DayLightSaving, ProtocolConfigurationId
FROM         dbo.ItcsConfigurationSet
WHERE     (IsDeleted = 0)


;

GO
ALTER VIEW [dbo].[DatabaseVersions]
AS
SELECT     Id, Name, VersionMajor, VersionMinor, VersionBuild, VersionRevision, Description, DateCreated
FROM         dbo.DatabaseVersionSet

;

GO
ALTER VIEW [dbo].[DataScopes]
AS
SELECT     Id, Name, Description, DateCreated, DateModified
FROM         dbo.DataScopeSet



;

GO
ALTER VIEW [dbo].[Permissions]
AS
SELECT     Id, Name, Description, DateCreated, DateModified
FROM         dbo.PermissionSet



;

GO
ALTER VIEW [dbo].[ProductTypes]
AS
SELECT     Id, UnitTypeId, Name, Revision, Description, IsDefault, DateCreated, DateModified
FROM         dbo.ProductTypeSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[Layouts]
AS
SELECT     Id, Name, Definition, [Description], DateCreated, DateModified
FROM         dbo.LayoutSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[Operations]
AS
SELECT [Id]
      ,[UserId]
      ,[StartDate]
      ,[Name]
      ,[StopDate]
      ,[StartExecutionDayMon]
      ,[StartExecutionDayTue]
      ,[StartExecutionDayWed]
      ,[StartExecutionDayThu]
      ,[StartExecutionDayFri]
      ,[StartExecutionDaySat]
      ,[StartExecutionDaySun]
      ,[Repetition]
      ,[DateCreated]
      ,[DateModified]
      ,[OperationState]
      ,[ExecutionOnceStartDateKind]
      ,[ExecutionOnceStopDateKind]
      ,[ExecutionWeeklyStartDate]
      ,[ExecutionWeeklyStopDate]
      ,[ExecutionWeeklyBeginTime]
      ,[ExecutionWeeklyEndTime]
      ,[ExecutionWeeklyStopDateKind]
      ,[WeekRepetition]
      ,[ActivityStatus]
  FROM [dbo].[OperationSet]
WHERE     (IsDeleted = 0)

;

GO
ALTER VIEW [dbo].[AssociationsUnitStation]
AS
SELECT     Id, UnitId, StationId
FROM         dbo.AssociationUnitStationSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[Alarms]
AS
SELECT [Id]
      ,[Name]
      ,[UnitId]
      ,[UserId]
      ,[Description]
      ,[Severity]
      ,[Type]
      ,[ConfirmationText]
      ,[DateCreated]
      ,[EndDate]
      ,[DateConfirmed]
      ,[DateModified]
      , [DateReceived]
  FROM [dbo].[AlarmSet]
WHERE     (IsDeleted = 0)

;
GO
ALTER VIEW [dbo].[AlarmStatusTypes]
AS
SELECT     Id, Name, Description, DateCreated, DateModified
FROM         dbo.AlarmStatusTypeSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[Activities]
AS
SELECT     Id, OperationId, DateCreated, DateModified
FROM         dbo.ActivitySet
WHERE     (IsDeleted = 0)


;

GO
ALTER VIEW [dbo].[AlarmCategories]
AS
SELECT     Id, Name, Description, DateCreated, DateModified
FROM         dbo.AlarmCategorySet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[AssociationsTenantUserUserRole]
AS
SELECT     Id, TenantId, UserId, UserRoleId
FROM         dbo.AssociationTenantUserUserRoleSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[AssociationsUnitOperation]
AS
SELECT     Id, UnitId, OperationId
FROM         dbo.AssociationUnitOperationSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[AlarmTypes]
AS
SELECT     Id, AlarmCategoryId, SeverityId, Name, Description, DateCreated, DateModified
FROM         dbo.AlarmTypeSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[AssociationsPermissionDataScopeUserRole]
AS
SELECT     Id, PermissionId, DataScopeId, UserRoleId, Name
FROM         dbo.AssociationPermissionDataScopeUserRoleSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[ProtocolConfigurations]
AS
SELECT     Id, ProtocolTypeId, RealTimeDataOnly, HttpListenerHost, HttpListenerPort, HttpServerHost, HttpServerport, HttpWebProxyHost, HttpWebProxyPort, 
                      HttpClientIdentification, HttpServerIdentification, HttpResponseTimeOut, XmlClientRequestSenderId, XmlServerRequestSenderId, XmlNameSpaceRequest, 
                      XmlNameSpaceResponse, OmitXmlDeclaration, EvaluateDataReadyInStatusResponse, StatusRequestIntervalInSec, SubscriptionRetryIntervalInSec
FROM         dbo.ProtocolConfigurationSet


;

GO
ALTER VIEW [dbo].[UserRoles]
AS
SELECT     Id, Name, Description, DateCreated, DateModified
FROM         dbo.UserRoleSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[Users]
AS
SELECT [Id]
      ,[Username]
      , [HashedPassword]
      ,[Name]
      ,[LastName]
      ,[Email]
      ,[DateCreated]
      ,[DateModified]
      ,[Culture]
FROM         dbo.UserSet
WHERE     (IsDeleted = 0)

;

GO
ALTER VIEW [dbo].[Units]
AS
SELECT [Id]
      ,[TenantId]
      ,[ProductTypeId]
      ,[GroupId]
      ,[LayoutId]
      , [StationId]
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

;

GO
ALTER VIEW [dbo].[UnitTypes]
AS
SELECT     Id, Name, Description, IsDefault, DateCreated, DateModified
FROM         dbo.UnitTypeSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[AssociationsUnitLine]
AS
SELECT [Id]
      ,[UnitId]
      ,[LineId]
      ,[LaneReferenceName]
      ,[DirectionReferenceName]
  FROM [dbo].[AssociationUnitLineSet]

;

GO
ALTER VIEW [dbo].[Lines]
AS
SELECT [Id]
      ,[Name]
      ,[ShortName]
      ,[Description]
      ,[ReferenceName]
  FROM [dbo].[LineSet]
  
WHERE     (IsDeleted = 0)

;

GO
ALTER VIEW [dbo].[WorkflowInstances]
AS
SELECT     Id, WorkflowId, UnitId, Definition, DateCreated, DateModified
FROM         dbo.WorkflowInstanceSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[Workflows]
AS
SELECT     Id, Definition, DateCreated, DateModified
FROM         dbo.WorkflowSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[UnitGroupTypes]
AS
SELECT     Id, Name, Description, IsDefault, DateCreated, DateModified
FROM         dbo.UnitGroupTypeSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[Scheduleds]
AS
SELECT     Id, LineNumber, LineText, DirectionNumber, DirectionText, LaneText, ScheduledArrival, ScheduledDeparture, DateCreated, DateModified
FROM         dbo.ScheduledSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[Severities]
AS
SELECT     Id, Name, Description, DateCreated, DateModified
FROM         dbo.SeveritySet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[ProtocolTypes]
AS
SELECT     Id, Description, Name
FROM         dbo.ProtocolTypeSet


;

GO
ALTER VIEW [dbo].[Realtimes]
AS
SELECT     Id, EstimatedArrival, EstimatedDeparture, RealArrival, RealDeparture, VehicleAtStation, CleardownRef, RealTimeData, CongestionIndicator, DateCreated, 
                      DateModified
FROM         dbo.RealtimeSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[TimeTableEntries]
AS
SELECT     Id, UnitId, ScheduledDataId, RealtimeDataId, ItcsRef, ItcsStationId, ItcsLineRef, DirectionRef, LaneRef, TripInfo, ValidUntil, DriveId, VisitNumber, OperationalDay, 
                      DateCreated, DateModified
FROM         dbo.TimeTableEntrySet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[UnitGroups]
AS
SELECT     Id, TenantId, GroupTypeId, ParentId, Name, SystemName, Description, DateCreated, DateModified
FROM         dbo.UnitGroupSet
WHERE     (IsDeleted = 0)



;

GO
ALTER VIEW [dbo].[Stations]
AS
SELECT [Id]
      ,[Name]
      ,[Number]
      ,[Latitude]
      ,[Longitude]
      ,[UtcOffset]
      ,[DayLightSaving]
      ,[DateCreated]
      ,[DateModified]
      ,[ReferenceName]
      ,[LocationName]
      ,[Description]
      ,[TimeZoneId]

FROM         dbo.StationSet
WHERE     (IsDeleted = 0)

;

GO
ALTER VIEW [dbo].[Tenants]
AS
SELECT     Id, Name, Description, IsDefault, DateCreated, DateModified
FROM         dbo.TenantSet
WHERE     (IsDeleted = 0)



;

GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: 2011-12-07
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Operation_Select]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT *
    FROM [Operations]
END



;

GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: 04.0.12
-- Description:	Finds an operation by name. The search is case insensitive and finds the given string even if it is in the middle of the name.
-- =============================================
ALTER PROCEDURE [dbo].[Operation_ListByUnit]
(
	@unitId int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT *
	FROM [Operations] [O]
	WHERE EXISTS(
	SELECT [A].[OperationId]
	FROM [AssociationsUnitOperation] [A] 
	WHERE [A].[UnitId]=@unitId AND [A].[OperationId]=[O].[Id]
	)
END

;

GO
-- =============================================
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 2012-01-04
-- Description:	Adds a permission
-- =============================================
ALTER PROCEDURE [dbo].[Permission_Insert]
(
	@name varchar(100)
	, @description varchar(500) = NULL
	, @dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Permission_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [PermissionSet]
			([Name]
			, [Description]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @dateCreated)
			
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



;

GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 22.12.11
-- Description:	Updates an operation
-- =============================================
ALTER PROCEDURE [dbo].[Operation_Update] 
	-- Add the parameters for the stored procedure her
	@id int,
	@startDate datetime,
	@name varchar(100),
	@stopDate datetime,
	@operationState int,
	@startExecutionDayMon bit = 0,
	@startExecutionDayTue bit = 0,
	@startExecutionDayWed bit = 0,
	@startExecutionDayThu bit = 0,
	@startExecutionDayFri bit = 0,
	@startExecutionDaySat bit = 0,
	@startExecutionDaySun bit = 0,
	@repetition int
	, @executionOnceStartDateKind int
	, @executionOnceStopDateKind int
	, @executionWeeklyStartDate datetime = NULL
	, @executionWeeklyStopDate datetime = NULL
	, @executionWeeklyBeginTime datetime = NULL
	, @executionWeeklyEndTime datetime = NULL
	, @executionWeeklyStopDateKind int = 0
	, @weekRepetition int = 0
	, @dateModified datetime = NULL
	, @activityStatus int
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
			
			UPDATE [OperationSet] 
			SET [StartDate] = @startDate
			, [Name] = @name
			, [StopDate] = @stopDate
			, [OperationState] = @operationState
			, [StartExecutionDayMon] = @startExecutionDayMon
			, [StartExecutionDayTue] = @startExecutionDayTue
			, [StartExecutionDayWed] = @startExecutionDayWed
			, [StartExecutionDayThu] = @startExecutionDayThu
			, [StartExecutionDayFri] = @startExecutionDayFri
			, [StartExecutionDaySat] = @startExecutionDaySat
			, [StartExecutionDaySun] = @startExecutionDaySun
			, [Repetition] = @repetition
			, [ExecutionOnceStartDateKind] = @executionOnceStartDateKind
			, [ExecutionOnceStopDateKind] = @executionOnceStopDateKind
			, [ExecutionWeeklyStartDate] = @executionWeeklyStartDate
			, [ExecutionWeeklyStopDate] = @executionWeeklyStopDate
			, [ExecutionWeeklyBeginTime] = @executionWeeklyBeginTime
			, [ExecutionWeeklyEndTime] = @executionWeeklyEndTime
			, [ExecutionWeeklyStopDateKind] = @executionWeeklyStopDateKind
			, [WeekRepetition] = @weekRepetition
			, [DateModified] = @dateModified
			, [ActivityStatus] = @activityStatus
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

;

GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 22.12.11
-- Description:	Deletes an operation and all its related activities
-- =============================================
ALTER PROCEDURE [dbo].[Operation_Delete]
(
	@id int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Operation_DeleteTx -- Start the transaction..
	
	-- If needed, it can be replaced with the usage of a cursor and the Activity_Delete operation.
	UPDATE [ActivitySet]
	SET [IsDeleted]=1
	WHERE [OperationId]=@id

	UPDATE [OperationSet]
	SET [Name] = [dbo].GetDeletedName([Id], [Name]), [IsDeleted]=1
	WHERE [Id]=@id
			
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



;

GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 13.12.11
-- Description:	Adds the association between a unit and an operation if does not exist.
-- =============================================
ALTER PROCEDURE [dbo].[AssociationUnitOperation_Insert]
(
	@unitId int,
	@operationId int
)
AS
BEGIN
		
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AssociationUnitOperation_Ins_Tx -- Start the transaction..
		-- Insert statements for procedure here
		
			DECLARE @id int = NULL
			SELECT @id = [A].[Id]
			FROM [AssociationUnitOperationSet] [A]
			WHERE [A].[UnitId]=@unitId AND [A].[OperationId]=@operationId
			
			IF @id IS NULL
				BEGIN
			
					INSERT INTO [AssociationUnitOperationSet]
					([UnitId]
					, [OperationId])
					VALUES
					(@unitId
					, @operationId)
					
					SET @id = SCOPE_IDENTITY()
				
				END
			
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



;

GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: 2011-12-07
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Operation_Insert]
(
	@userId int
	, @name varchar(100)
	, @dateCreated datetime
	, @startDate datetime
	, @stopDate datetime
	, @operationState int
	, @startExecutionDayMon bit = 0
	, @startExecutionDayTue bit = 0
	, @startExecutionDayWed bit = 0
	, @startExecutionDayThu bit = 0
	, @startExecutionDayFri bit = 0
	, @startExecutionDaySat bit = 0
	, @startExecutionDaySun bit = 0
	, @repetition int
	, @executionOnceStartDateKind int
	, @executionOnceStopDateKind int
	, @executionWeeklyStartDate datetime = NULL
	, @executionWeeklyStopDate datetime = NULL
	, @executionWeeklyBeginTime datetime = NULL
	, @executionWeeklyEndTime datetime = NULL
	, @executionWeeklyStopDateKind int = 0
	, @weekRepetition int = 0
	, @activityStatus int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Operation_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [OperationSet]
			([UserId]
			, [Name]
			, [DateCreated]
			, [StartDate]
			, [StopDate]
			, [OperationState]
			, [StartExecutionDayMon]
			, [StartExecutionDayTue]
			, [StartExecutionDayWed]
			, [StartExecutionDayThu]
			, [StartExecutionDayFri]
			, [StartExecutionDaySat]
			, [StartExecutionDaySun]
			, [Repetition]
			, [ExecutionOnceStartDateKind]
			, [ExecutionOnceStopDateKind]
			, [ExecutionWeeklyStartDate]
			, [ExecutionWeeklyStopDate]
			, [ExecutionWeeklyBeginTime]
			, [ExecutionWeeklyEndTime]
			, [ExecutionWeeklyStopDateKind]
			, [WeekRepetition]
			, [ActivityStatus])
			VALUES
			(@userId
			, @name
			, @dateCreated
			, @startDate
			, @stopDate
			, @operationState
			, @startExecutionDayMon
			, @startExecutionDayTue
			, @startExecutionDayWed
			, @startExecutionDayThu
			, @startExecutionDayFri
			, @startExecutionDaySat
			, @startExecutionDaySun
			, @repetition
			, @executionOnceStartDateKind
			, @executionOnceStopDateKind
			, @executionWeeklyStartDate
			, @executionWeeklyStopDate
			, @executionWeeklyBeginTime
			, @executionWeeklyEndTime
			, @executionWeeklyStopDateKind
			, @weekRepetition
			, @activityStatus)
			
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

;

GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: 04.0.12
-- Description:	Finds an operation by name. The search is case insensitive and finds the given string even if it is in the middle of the name.
-- =============================================
ALTER PROCEDURE [dbo].[Operation_FindByName]
(
	@name varchar(100)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT *
	FROM [Operations] [O]
	WHERE [O].[Name] = @name
END



;

GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 05.12.11 15:40
-- Description:	Adds a product type to the system
-- =============================================
ALTER PROCEDURE [dbo].[ProductType_Insert]
(
	@unitTypeId int,
	@name varchar(100),
	@revision varchar(100),
	@description varchar(500) = NULL,
	@isDefault bit = 0,
	@dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION ProductType_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [ProductTypeSet]
			([UnitTypeId]
			, [Name]
			, [Revision]
			, [Description]
			, [IsDefault]
			, [DateCreated])
			VALUES
			(@unitTypeId
			, @name
			, @revision
			, @description
			, @isDefault
			, @dateCreated)
			
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



;

GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 05.12.11 15:40
-- Description:	Adds a tenant to the system
-- =============================================
ALTER PROCEDURE [dbo].[Tenant_Insert]
(
	@name varchar(100),
	@description varchar(500) = NULL,
	@isDefault bit = 0,
	@dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Tenant_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [TenantSet]
			([Name]
			, [Description]
			, [IsDefault]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @isDefault
			, @dateCreated)
			
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



;

GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 7.12.11 09:20
-- Description:	Adds a station to the system
-- =============================================
ALTER PROCEDURE [dbo].[Station_Insert] 
	(@name varchar(100),
	@number varchar(100),
	@latitude float,
	@longitude float,
	@utcOffset datetime,
	@daylightSaving bit = 0,
	@dateCreated datetime = NULL
	, @referenceName varchar(100)
	, @timeZoneId varchar(100)
	, @locationName varchar(100)
	, @description varchar(500)
	)
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Station_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [StationSet]
			([Name]
			, [Number]
			, [Latitude]
			, [Longitude]
			, [UtcOffset]
			, [DayLightSaving]
			, [DateCreated]
			, [ReferenceName]
			, [TimeZoneId]
			, [LocationName]
			, [Description]
			)
			VALUES
			(@name ,
	@number ,
	@latitude ,
	@longitude ,
	@utcOffset ,
	@daylightSaving ,
	@dateCreated
	, @referenceName
	, @timeZoneId
	, @locationName
	, @description)
			
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

;

;

;
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
			, [StationId]
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
			, @stationId
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
			
			DECLARE @line varchar(5) = ERROR_LINE()
			DECLARE @message varchar(500) = ERROR_MESSAGE()
			DECLARE @severity int = ERROR_SEVERITY()
			print 'Error on line ' + @line + ': ' + @message
			--RAISERROR(@message, @severity, 1)

	END CATCH
END

;

;

;
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 7.12.11 10:10
-- Description:	Adds a TimeTableEntry to the system
-- =============================================
ALTER PROCEDURE [dbo].[TimeTableEntry_Insert] 
	(
	@unitId int,
	@scheduledDataId int,
	@realtimeDataId int,
	@itcsRef int,
	@itcsStationId int,
	@itcsLineRef int,
	@directionRef int = NULL,
	@laneRef int = NULL,
	@tripInfo varchar(100) = NULL,
	@validUntil datetime = NULL,
	@driveId varchar(100),
	@visitNumber varchar(100),
	@operationalDay datetime,
	@dateCreated datetime = NULL
	)
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION TimeTableEntry_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [TimeTableEntrySet]
			([UnitId]
			, [ScheduledDataId]
			, [RealtimeDataId]
			, [ItcsRef]
			, [ItcsStationId]
			, [ItcsLineRef]
			, [DirectionRef]
			, [LaneRef]
			, [TripInfo]
			, [ValidUntil]
			, [DriveId]
			, [VisitNumber]
			, [OperationalDay]
			, [DateCreated]
			)
			VALUES
			(@unitId ,
	@scheduledDataId ,
	@realtimeDataId ,
	@itcsRef ,
	@itcsStationId ,
	@itcsLineRef ,
	@directionRef ,
	@laneRef ,
	@tripInfo ,
	@validUntil ,
	@driveId ,
	@visitNumber ,
	@operationalDay ,
	@dateCreated )
			
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



;

;

;
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 7.12.11 08:55
-- Description:	Adds a realtime entry to the system
-- =============================================
ALTER PROCEDURE [dbo].[Realtime_Insert] 
	(@estimatedArrival datetime,
	@estimatedDeparture datetime,
	@realArrival datetime,
	@realDeparture datetime,
	@vehicleAtStation bit = 0,
	@cleardownRef int,
	@realTimeData bit = 0,
	@congestionIndicator bit = 0,
	@dateCreated datetime = NULL
	)
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Realtime_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [RealtimeSet]
			([EstimatedArrival]
			, [EstimatedDeparture]
			, [RealArrival]
			, [RealDeparture]
			, [VehicleAtStation]
			, [CleardownRef]
			, [RealTimeData]
			, [CongestionIndicator]
			, [DateCreated]
			)
			VALUES
			(@estimatedArrival ,
	@estimatedDeparture ,
	@realArrival ,
	@realDeparture ,
	@vehicleAtStation ,
	@cleardownRef ,
	@realTimeData ,
	@congestionIndicator ,
	@dateCreated )
			
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



;

;

;
GO
-- =============================================
-- Author:		Jerome Coutant
-- Create date: 09-12-2011
-- Description:	
-- =============================================
ALTER PROCEDURE [dbo].[ProtocolType_Insert] 
	@name varchar(50),
	@description varchar(500) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION ProtocolType_InsertTx -- Start the transaction..
		-- Insert statements for procedure here		
			INSERT INTO [ProtocolTypeSet]
			([Name]
			, [Description])
			VALUES
			(@name
			, @description)
			
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



;

;

;
GO
-- =============================================
-- Author:		Thomas Epple (ept@gorba.com)
-- Create date: 12-01-09
-- Description:	Adds a Severity
-- =============================================
ALTER PROCEDURE [dbo].[Severity_Insert]
(
	@name varchar(100),
	@description varchar(500),
	@dateCreated datetime = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Severity_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [SeveritySet]
			([Name]
			, [Description]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @dateCreated)
			
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



;

;

;
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 7.12.11 09:20
-- Description:	Adds a scheduled entry to the system
-- =============================================
ALTER PROCEDURE [dbo].[Scheduled_Insert] 
	(@lineNumber varchar(100),
	@lineText varchar(100),
	@directionNumber varchar(100),
	@directionText varchar(100),
	@laneText varchar(100),
	@scheduledArrival datetime,
	@scheduledDeparture datetime,
	@dateCreated datetime = NULL
	)
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Scheduled_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [ScheduledSet]
			([LineNumber]
			, [LineText]
			, [DirectionNumber]
			, [DirectionText]
			, [LaneText]
			, [ScheduledArrival]
			, [ScheduledDeparture]
			, [DateCreated]
			)
			VALUES
			(@lineNumber ,
	@lineText ,
	@directionNumber ,
	@directionText ,
	@laneText ,
	@scheduledArrival ,
	@scheduledDeparture ,
	@dateCreated )
			
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



;

;

;
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Layout_Delete]
(
	@id int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Layout_DeleteTx -- Start the transaction..

	UPDATE [LayoutSet]
	SET [IsDeleted]=1
	WHERE [Id]=@id
			
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



;

;

;
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: 2011-12-07
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Workflow_Insert]
(
	@definition xml,
	@dateCreated datetime = NULL	
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Workflow_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [WorkflowSet]
			([Definition]
			, [DateCreated])
			VALUES
			(@definition
			, @dateCreated)
			
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



;

;

;
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: 2011-12-07
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[WorkflowInstance_Insert]
(
	@workflowId int,
	@unitId int,
	@definition xml,
	@dateCreated datetime = NULL	
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION WorkflowInstance_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [WorkflowInstanceSet]
			([WorkflowId]
			, [UnitId]
			, [Definition]
			, [DateCreated])
			VALUES
			(@workflowId
			, @unitId
			, @definition
			, @dateCreated)
			
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



;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 2012-01-04
-- Description:	Adds a UserRole
-- =============================================
ALTER PROCEDURE [dbo].[UserRole_Insert]
(
	@name varchar(100)
	, @description varchar(500) = NULL
	, @dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION UserRole_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [UserRoleSet]
			([Name]
			, [Description]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @dateCreated)
			
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



;

;

;
GO
-- =============================================
-- Author:		Jerome Coutant
-- Create date: 09-12-2011
-- Description:	Insert new ProtocolConfiguration row with only needed vdv fields. 
--				Note : The different fieds are set or not according to the underlying used protocol.
-- =============================================
ALTER PROCEDURE [dbo].[VdvProtocolConfiguration_Insert] 
	-- Add the parameters for the stored procedure here
	@ProtocolTypeId int,
	@realTimeDataOnly bit = 0,
	@httpListenerHost varchar(50),
	@httpListenerPort int,
	@httpServerHost varchar(50),
	@httpServerport int,
	@httpWebProxyHost varchar(50) = null,
	@httpWebProxyPort int = 0,
	@httpClientIdentification varchar(50),
	@httpServerIdentification varchar(50),
	@httpResponseTimeOut int = 10,
	@xmlClientRequestSenderId varchar(50) = "client",
	@xmlServerRequestSenderId varchar(50) = "server",
	@xmlNameSpaceRequest varchar(50) = null,
	@xmlNameSpaceResponse varchar(50) = null,
	@omitXmlDeclaration bit = 1,
	@evaluateDataReadyInStatusResponse bit = 1,
	@statusRequestIntervalInSec int = 60,
	@subscriptionRetryIntervalInSec int = 60
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION VdvProtocolConfig_InsertTx -- Start the transaction.				
					
    -- Insert statements for procedure here
		INSERT INTO [Gorba_CenterOnline].[dbo].[ProtocolConfigurationSet]
			   ( [ProtocolTypeId]
			   , [realTimeDataOnly]
			   , [httpListenerHost]
			   , [httpListenerPort]
			   , [httpServerHost]
			   , [httpServerport]
			   , [httpWebProxyHost]
			   , [httpWebProxyPort]
			   , [httpClientIdentification]
			   , [httpServerIdentification]
			   , [httpResponseTimeOut]
			   , [xmlClientRequestSenderId]
			   , [xmlServerRequestSenderId]
			   , [xmlNameSpaceRequest]
			   , [xmlNameSpaceResponse]
			   , [omitXmlDeclaration]
			   , [evaluateDataReadyInStatusResponse]
			   , [statusRequestIntervalInSec]
			   , [subscriptionRetryIntervalInSec])
		 VALUES
			   ( @ProtocolTypeId,
				 @realTimeDataOnly,
				 @httpListenerHost,
				 @httpListenerPort,
				 @httpServerHost,
				 @httpServerport,
				 @httpWebProxyHost,
				 @httpWebProxyPort,
				 @httpClientIdentification,
				 @httpServerIdentification,
				 @httpResponseTimeOut,
				 @xmlClientRequestSenderId,
				 @xmlServerRequestSenderId,
				 @xmlNameSpaceRequest,
				 @xmlNameSpaceResponse,
				 @omitXmlDeclaration,
				 @evaluateDataReadyInStatusResponse,
				 @statusRequestIntervalInSec,
				 @subscriptionRetryIntervalInSec)
			
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



;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 05.01.12
-- Description:	Deletes an activity
-- =============================================
ALTER PROCEDURE [dbo].[Activity_Delete]
(
	@id int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Activity_DeleteTx -- Start the transaction..
	
	UPDATE [ActivitySet]
	SET [IsDeleted]=1
	WHERE [Id]=@id
			
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



;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 13.12.11
-- Description:	Selects the activities bound to an operation
-- =============================================
ALTER PROCEDURE [dbo].[Activity_SelectByOperation]
(
	@operationId int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT [A].*
    FROM [Activities] [A]
    WHERE [A].[OperationId]=@operationId
END



;

;

;
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 14.02.2012
-- Description:	Selects all Units belonging to a tenantId
-- =============================================
ALTER PROCEDURE [dbo].[Unit_SelectByTenant]
	@tenantId int = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    IF @tenantId IS NULL
    BEGIN
		SELECT *
		FROM [Units]
	END
	ELSE
	BEGIN
		SELECT [U].*
		FROM [Units] [U]
		WHERE [U].[TenantId] = @tenantId
	END
END

;

;

;
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: 2011-12-07
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Activity_Insert]
(
	@operationId int
	, @dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Activity_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [ActivitySet]
			( [OperationId]
			, [DateCreated]
			)
			VALUES
			( @operationId
			, @dateCreated
			)
			
			DECLARE @id int = SCOPE_IDENTITY()
			SELECT @id			
			COMMIT TRAN -- Transaction Success!

			RETURN @id
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


;

;

;
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 14.02.2012
-- Description:	Selects all Units belonging to the tenants of the current user
-- =============================================
ALTER PROCEDURE [dbo].[Unit_SelectByTenantOfUser]
	@userId int = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF @userId IS NULL
	BEGIN
		SELECT *
		FROM [Units]
	END
	ELSE
	BEGIN
		SELECT U.* from UnitSet U 
		JOIN AssociationTenantUserUserRoleSet TU ON U.TenantId = TU.TenantId 
		WHERE TU.UserId = @userId
	END
END

;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: Friday, March 30th
-- Description:	Sets the request restart date for the specified unit.
-- =============================================
ALTER PROCEDURE [dbo].[Unit_Restart]
(
	@unitId int,
	@requestDate datetime
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    BEGIN TRY
		BEGIN TRANSACTION UnitRestart_Tx
		
			UPDATE [UnitSet]
			SET [LastRestartRequestDate] = @requestDate
			WHERE [Id] = @unitId
		
		COMMIT TRANSACTION
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

;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 22.12.11
-- Description:	Select stations associated with a specified unit
-- =============================================
ALTER PROCEDURE [dbo].[Unit_SelectAssociatedStations]
(
	@unitId int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [S].*
	FROM [Stations] [S]
	INNER JOIN [AssociationsUnitStation] [A] ON [A].[UnitId]=@unitId AND [A].[StationId]=[S].[Id]
	
END



;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 05.12.11 15:40
-- Description:	Adds a layout to the system
-- =============================================
ALTER PROCEDURE [dbo].[Layout_Insert]
(
	@name varchar(100),
	@definition xml,
	@description varchar(500) = NULL,
	@dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Layout_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [LayoutSet]
			([Name]
			, [Definition]
			, [Description]
			, [DateCreated])
			VALUES
			(@name
			, @definition
			, @description
			, @dateCreated)
			
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



;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 22.12.11
-- Description:	Select Operations associated with a specified unit
-- =============================================
ALTER PROCEDURE [dbo].[Unit_SelectAssociatedOperations]
(
	@unitId int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [O].*
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[UnitId]=@unitId AND [A].[OperationId]=[O].[Id]
	
END



;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: Friday, March 30th
-- Description:	Sets the request time sync date for the specified unit.
-- =============================================
ALTER PROCEDURE [dbo].[Unit_TimeSync]
(
	@unitId int,
	@requestDate datetime,
	@value datetime
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    BEGIN TRY
		BEGIN TRANSACTION UnitTimeSync_Tx
		
			UPDATE [UnitSet]
			SET [LastTimeSyncRequestDate] = @requestDate
			, [LastTimeSyncValue] = @value
			WHERE [Id] = @unitId
		
		COMMIT TRANSACTION
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

;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 05.12.11 15:40
-- Description:	Adds a unit type to the system
-- =============================================
ALTER PROCEDURE [dbo].[UnitType_Insert]
(
	@name varchar(100),
	@description varchar(500) = NULL,
	@isDefault bit = 0,
	@dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION UnitType_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [UnitTypeSet]
			([Name]
			, [Description]
			, [IsDefault]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @isDefault
			, @dateCreated)
			
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



;

;

;
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: 2011-12-07
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[User_Insert]
(
	@username varchar(100),
	@hashedPassword char(32),
	@name varchar(100),
	@lastName varchar(100),
	@email varchar(256),
	@culture varchar(5) = NULL,
	@dateCreated datetime = NULL	
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION User_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [UserSet]
			([Username]
			, [HashedPassword]
			, [Name]
			, [LastName]
			, [Email]
			, [Culture]
			, [DateCreated])
			VALUES
			(@username
			, @hashedPassword
			, @name
			, @lastName
			, @email
			, @culture
			, @dateCreated)
			
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

;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 05.12.11 15:40
-- Description:	Adds a unit group to the system
-- =============================================
ALTER PROCEDURE [dbo].[UnitGroup_Insert]
(
	@tenantId int,
	@groupTypeId int,
	@parentId int = NULL,
	@name varchar(100),
	@systemName varchar(100) = NULL,
	@description varchar(500) = NULL,
	@dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION UnitGroup_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [UnitGroupSet]
			([TenantId]
			, [GroupTypeId]
			, [ParentId]
			, [Name]
			, [SystemName]
			, [Description]
			, [DateCreated])
			VALUES
			(@tenantId
			, @groupTypeId
			, @parentId
			, @name
			, @systemName
			, @description
			, @dateCreated)
			
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



;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 05.12.11 15:40
-- Description:	Adds a unit group type to the system
-- =============================================
ALTER PROCEDURE [dbo].[UnitGroupType_Insert]
(
	@name varchar(100),
	@description varchar(500) = NULL,
	@isDefault bit = 0,
	@dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION ProductType_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [ProductTypeSet]
			([Name]
			, [Description]
			, [IsDefault]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @isDefault
			, @dateCreated)
			
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



;

;

;
GO
-- =============================================
-- Author:		Thomas Epple (ept@gorba.com)
-- Create date: 2012-01-09
-- Description:	Adds an Alarm
-- =============================================
ALTER PROCEDURE [dbo].[Alarm_Insert]
(
	@unitId int,
	@userId int = NULL,
	@name varchar(100),
	@description varchar(500),
	@endDate datetime,
	@dateCreated datetime = NULL,
	@dateReceived datetime
)
AS
BEGIN
	SET NOCOUNT ON;
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Alarm_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [AlarmSet]
			([UnitId]
			, [UserId]
			, [Name]
			, [Description]
			, [EndDate]
			, [DateCreated]
			, [DateReceived])
			VALUES
			(@unitId
			, @userId
			, @name
			, @description
			, @endDate
			, @dateCreated
			, @dateReceived)
			
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



;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 02.12.11 10:47
-- Description:	Inserts a new version of the database in the DatabaseVersionSet table.
--				Everything is done within a transaction.
--				If the @dateCreated parameter is NULL, then the actual UTC time is used.
--				If the operations succeeds, the identifier assigned to the created row is selected
-- =============================================
ALTER PROCEDURE [dbo].[DatabaseVersion_Insert]
(
	@name varchar(100),
	@description varchar(500) = NULL,
	@versionMajor int,
	@versionMinor int,
	@versionBuild int,
	@versionRevision int,
	@dateCreated datetime = NULL
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



;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 02.12.11
-- Description:	This SP selects the database version entries ordering them by version.
-- =============================================
ALTER PROCEDURE [dbo].[DatabaseVersion_Select]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [DV].*
	FROM [DatabaseVersions] [DV]
	ORDER BY [VersionMajor], [VersionMinor], [VersionBuild], [VersionRevision]
END



;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 13.12.11
-- Description:	Deleted the association between a unit and an operation (if exists).
-- =============================================
ALTER PROCEDURE [dbo].[AssociationUnitStation_Delete]
(
	@id int = NULL,
	@unitId int = NULL,
	@stationId int = NULL
)
AS
BEGIN
		
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AssociationUnitStation_Del_Tx -- Start the transaction..
		-- Insert statements for procedure here
		
			UPDATE [AssociationUnitStationSet]
			SET [IsDeleted]=1
			WHERE
			(@id IS NOT NULL AND [Id]=@id)
			OR
			([UnitId]=@unitId AND [StationId]=@stationId)
			
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



;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 13.12.11
-- Description:	Adds the association between a unit and an operation if does not exist.
-- =============================================
ALTER PROCEDURE [dbo].[AssociationUnitStation_Insert]
(
	@unitId int,
	@stationId int
)
AS
BEGIN
		
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AssociationUnitStation_Ins_Tx -- Start the transaction..
		-- Insert statements for procedure here
		
			DECLARE @id int = NULL
			SELECT @id = [A].[Id]
			FROM [AssociationUnitStationSet] [A]
			WHERE [A].[UnitId]=@unitId AND [A].[StationId]=@stationId
			
			IF @id IS NULL
				BEGIN
			
					INSERT INTO [AssociationUnitStationSet]
					([UnitId]
					, [StationId])
					VALUES
					(@unitId
					, @stationId)
					
					SET @id = SCOPE_IDENTITY()
				
				END
			
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



;

;

;
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 7.12.11 08:50
-- Description:	Adds an info line text to the system
-- =============================================
ALTER PROCEDURE [dbo].[InfoLineTextActivity_Insert] 
	( @operationId int
	, @dateCreated datetime = NULL
	, @lineNumber int
	, @destinationId int
	, @displayText varchar(100)
	, @expirationDate datetime
	, @infoRowId smallint
	, @blink bit
	, @displayedScreenSide smallint
	, @alignment smallint
	, @font smallint
	)
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @id int;
	
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION InfoLineTextActivity_InsertTx -- Start the transaction..
		-- Call base stored procedure Activity_Insert to insert first the fields of
		-- base entity
		EXEC @id = dbo.Activity_Insert @operationId = @operationId, @dateCreated = @dateCreated;
		
		-- insert the fields of the derived entity		
			INSERT INTO [InfoLineTextActivitySet]
			( [Id]
			, [LineNumber]
			, [DestinationId]
			, [DisplayText]
			, [ExpirationDate]
			, [InfoRowId]
			, [Blink]
			, [DisplayedScreenSide]
			, [Alignment]
			, [Font]
			)
			VALUES
			( @id
			, @lineNumber
			, @destinationId
			, @displayText 
			, @expirationDate
			, @infoRowId
			, @blink
			, @displayedScreenSide
			, @alignment
			, @font
			)
			
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


;

;

;
GO
-- =============================================
-- Author:		Jerome Coutant
-- Create date: 09-12-2011
-- Description:	
-- =============================================
ALTER PROCEDURE [dbo].[ItcsConfiguration_Insert] 
	@ProtocolConfigurationId int,
	@name varchar(50),
	@description varchar(500) = null,
	@collectSystemData bit = 1,
	@operationDayStartUtc datetime,
	@operationDayDuration datetime,
	@utcOffset datetime,
	@dayLightSaving bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
			BEGIN TRANSACTION ItcsConfiguration_InsertTx -- Start the transaction..
			-- Insert statements for procedure here
			INSERT INTO [Gorba_CenterOnline].[dbo].[ItcsConfigurationSet]
				   ([ProtocolConfigurationId]
				   ,[name]
				   ,[description]
				   ,[collectSystemData]
				   ,[operationDayStartUtc]
				   ,[operationDayDuration]
				   ,[utcOffset]
				   ,[dayLightSaving])
			 VALUES
				   (@ProtocolConfigurationId,
					@name,
					@description,
					@collectSystemData,
					@operationDayStartUtc,
					@operationDayDuration,
					@utcOffset,
					@dayLightSaving)
			
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


;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 2012-01-04
-- Description:	Adds a DataScope
-- =============================================
ALTER PROCEDURE [dbo].[DataScope_Insert]
(
	@name varchar(100)
	, @description varchar(500)
	, @dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION DataScope_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [DataScopeSet]
			([Name]
			, [Description]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @dateCreated)
			
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



;

;

;
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[InfoLineTextActivity_Delete] 
(
	@id int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		EXEC dbo.Activity_Delete @id = @id;

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


;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 13.12.11
-- Description:	Deleted the association between a unit and an operation (if exists).
-- =============================================
ALTER PROCEDURE [dbo].[AssociationUnitOperation_Delete]
(
	@id int = NULL,
	@unitId int = NULL,
	@operationId int = NULL
)
AS
BEGIN
		
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AssociationUnitOperation_Del_Tx -- Start the transaction..
		-- Insert statements for procedure here
		
			UPDATE [AssociationUnitOperationSet]
			SET [IsDeleted]=1
			WHERE
			(@id IS NOT NULL AND [Id]=@id)
			OR
			([UnitId]=@unitId AND [OperationId]=@operationId)
			
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



;

;

;
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

;

;

;
GO
-- =============================================
-- Author:		Thomas Epple (ept@gorba.com)
-- Create date: 12-01-09
-- Description:	Adds an AlarmType
-- =============================================
ALTER PROCEDURE [dbo].[AlarmType_Insert]
(
	@alarmCategoryId int,
	@severityId int,
	@description varchar(500),
	@name varchar(100),
	@dateCreated datetime = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AlarmType_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [AlarmTypeSet]
			([AlarmCategoryId]
			, [SeverityId]
			, [Description]
			, [Name]
			, [DateCreated])
			VALUES
			(@alarmCategoryId
			, @severityId
			, @description
			, @name
			, @dateCreated)
			
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



;

;

;
GO
-- =============================================
-- Author:		Thomas Epple (ept@gorba.com)
-- Create date: 12-01-09
-- Description:	Adds an AlarmCategory
-- =============================================
ALTER PROCEDURE [dbo].[AlarmCategory_Insert]
(
	@name varchar(100),
	@description varchar(500),
	@dateCreated datetime = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AlarmCategory_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [AlarmCategorySet]
			([Name]
			, [Description]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @dateCreated)
			
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



;

;

;
GO
-- =============================================
-- Author:		Thomas Epple (ept@gorba.com)
-- Create date: 12-01-09
-- Description:	Adds an AlarmStatusType
-- =============================================
ALTER PROCEDURE [dbo].[AlarmStatusType_Insert]
(
	@name varchar(100),
	@description varchar(500),
	@dateCreated datetime = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AlarmStatusType_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [AlarmStatusTypeSet]
			([Name]
			, [Description]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @dateCreated)
			
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



;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 04.01.12
-- Description:	Adds the association.
-- =============================================
ALTER PROCEDURE [dbo].[AssociationTenantUserUserRole_Insert]
(
	@tenantId int,
	@userId int,
	@userRoleId int
)
AS
BEGIN
		
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION TenantUserURole_Ins_Tx -- Start the transaction..
		-- Insert statements for procedure here
		
			DECLARE @id int = NULL
			SELECT @id = [A].[Id]
			FROM [AssociationsTenantUserUserRole] [A]
			WHERE [A].[TenantId]=@tenantId AND [A].[UserId]=@userId AND [A]
			.[UserRoleId]=@userRoleId
			
			IF @id IS NULL
				BEGIN
			
					INSERT INTO [AssociationTenantUserUserRoleSet]
					([TenantId]
					, [UserId]
					, [UserRoleId])
					VALUES
					(@tenantId
					, @userId
					, @userRoleId)
					
					SET @id = SCOPE_IDENTITY()
				
				END
			
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

;

;

;
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 14.02.2012
-- Description:	Selects Operations filterd by userId and/or tenantId
-- =============================================
ALTER PROCEDURE [dbo].[Operation_SelectByUser] 
	(@userId int = NULL,
	@tenantId int = NULL
	)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF @userId IS NULL AND @tenantId IS NULL
	BEGIN
		SELECT *
		FROM [Operations]
	END
	ELSE
	BEGIN
		IF @userId IS NULL
		BEGIN
			SELECT DISTINCT O.* from OperationSet O 
			JOIN AssociationUnitOperationSet UO ON O.Id = UO.OperationId 
			JOIN UnitSet U on UO.UnitId = U.Id
			WHERE U.TenantId = @tenantId		
		END
		ELSE
		BEGIN
			IF @tenantId IS NULL
			BEGIN
				SELECT *
				FROM [Operations] [O]
				WHERE O.UserId = @userID
			END
			ELSE
			BEGIN
				SELECT DISTINCT O.* from OperationSet O 
				JOIN AssociationUnitOperationSet UO ON O.Id = UO.OperationId 
				JOIN UnitSet U on UO.UnitId = U.Id
				WHERE U.TenantId = @tenantId AND O.UserId = @userId
			END
		END	
	END	END

;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: February 16th, 2012
-- Description:	Retrieves the submitted activity for an operation on a specified unit.
-- ACHTUNG! In this version, all activities are returned, and not only active ones.
-- The management of the state of an activity should be implemented.
-- =============================================
ALTER PROCEDURE [dbo].[Operation_GetActivitySubmissions] 
	-- Add the parameters for the stored procedure here
	@operationId int = NULL,
	@unitId int = NULL,
	@onlyActive bit = 0,
	@userId int = NULL,
	@tenantId int = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

DECLARE @operations TABLE
		([Id] int
      ,[UserId] int
      ,[StartDate] datetime
      ,[Name] varchar(100)
      ,[StopDate] datetime NULL
      ,[OperationState] int
      ,[StartExecutionDayMon] bit
      ,[StartExecutionDayTue] bit
      ,[StartExecutionDayWed] bit
      ,[StartExecutionDayThu] bit
      ,[StartExecutionDayFri] bit
      ,[StartExecutionDaySat] bit
      ,[StartExecutionDaySun] bit
      ,[Repetition] int
      ,[DateCreated] datetime
      ,[DateModified] datetime NULL
      ,[ExecutionOnceStartDateKind] int
      ,[ExecutionOnceStopDateKind] int
      ,[ExecutionWeeklyStartDate] datetime
      ,[ExecutionWeeklyStopDate] datetime
      ,[ExecutionWeeklyBeginTime] datetime
      ,[ExecutionWeeklyEndTime] datetime
      ,[ExecutionWeeklyStopDateKind] int
      ,[WeekRepetition] int
      , [ActivityStatus] int)


    -- Insert statements for procedure here
    INSERT INTO @operations
	EXEC [Operation_SelectByUser] @userId = @userId, @tenantId = @tenantId
	
	DECLARE @unit_operation TABLE([UnitId] int, [OperationId] int)
	
	INSERT INTO @unit_operation
	SELECT [UO].[UnitId], [UO].[OperationId]
	FROM [AssociationsUnitOperation] [UO]
	WHERE [UO].[OperationId] IN
	(
		SELECT [O].[Id]
		FROM @operations [O]
		WHERE (@operationId IS NULL OR [O].[Id]=@operationId)
	)
	AND
	(@unitId IS NULL OR [UO].[UnitId]=@unitId)
	
	SELECT [A].[Id] [ActivityId], [UO].[OperationId], [UO].[UnitId]
	FROM @unit_operation [UO]
	INNER JOIN [Activities] [A] ON [A].[OperationId]=[UO].[OperationId]
END

;

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 04.01.12
-- Description:	Adds the association.
-- =============================================
ALTER PROCEDURE [dbo].[AssociationPermissionDataScopeUserRole_Insert]
(
	@permissionId int,
	@dataScopeId int,
	@userRoleId int,
	@name varchar(100) = NULL
)
AS
BEGIN
		
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION PermissionDScopeURole_Ins_Tx -- Start the transaction..
		-- Insert statements for procedure here
		
			DECLARE @id int = NULL
			SELECT @id = [A].[Id]
			FROM [AssociationPermissionDataScopeUserRoles] [A]
			WHERE [A].[PermissionId]=@permissionId AND [A].[DataScopeId]=@dataScopeId AND [A]
			.[UserRoleId]=@userRoleId
			
			IF @id IS NULL
				BEGIN
			
					INSERT INTO [AssociationPermissionDataScopeUserRoleSet]
					([PermissionId]
					, [DataScopeId]
					, [UserRoleId]
					, [Name])
					VALUES
					(@permissionId
					, @dataScopeId
					, @userRoleId
					, @name)
					
					SET @id = SCOPE_IDENTITY()
				
				END
			
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



;

;

;
GO
ALTER TABLE [dbo].[AlarmSet] ADD CONSTRAINT [PK_AlarmSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ItcsFilterSet] ADD CONSTRAINT [PK_ItcsFilterSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ItcsFilterSet] ADD CONSTRAINT [FK_ItcsFilterSet_ItcsConfigurationSet] FOREIGN KEY
	(
		[ItcsConfigurationId]
	)
	REFERENCES [dbo].[ItcsConfigurationSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ItcsFilterSet] ADD CONSTRAINT [FK_ItcsFilterSet_StopPointSet] FOREIGN KEY
	(
		[StopPointId]
	)
	REFERENCES [dbo].[StopPointSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ItcsFilterSet] ADD CONSTRAINT [FK_ItcsFilterSet_ItcsStationReferenceSet] FOREIGN KEY
	(
		[ItcsStationReferenceId]
	)
	REFERENCES [dbo].[ItcsStationReferenceSet]
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
ALTER TABLE [dbo].[AlarmSet] ADD CONSTRAINT [FK_AlarmSet_UserSet] FOREIGN KEY
	(
		[UserId]
	)
	REFERENCES [dbo].[UserSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[SwitchDisplayStateActivitySet] ADD CONSTRAINT [FK_SwitchDisplayStateActivitySet_ActivitySet] FOREIGN KEY
	(
		[Id]
	)
	REFERENCES [dbo].[ActivitySet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[AnnouncementTextSet] ADD CONSTRAINT [FK_AnnouncementTextSet_AnnouncementActivitySet] FOREIGN KEY
	(
		[AnnouncementId]
	)
	REFERENCES [dbo].[AnnouncementActivitySet]
	(
		[Id]
	)
GO

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.11.27'
  ,@description = 'Added computed fields to alarms and operations.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 11
  ,@versionRevision = 27
  ,@dateCreated = '2012-05-07T10:10:00.000'
GO
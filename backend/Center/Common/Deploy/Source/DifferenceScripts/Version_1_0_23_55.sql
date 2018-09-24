 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[WorkflowInstanceSet] DROP CONSTRAINT [FK_WorkflowInstanceSet_WorkflowSet]
GO
ALTER TABLE [dbo].[WorkflowInstanceSet] DROP CONSTRAINT [FK_WorkflowInstanceSet_UnitSet]
GO
ALTER TABLE [dbo].[AssociationUnitStationSet] DROP CONSTRAINT [FK_AssociationUnitStationSet_UnitSet]
GO
ALTER TABLE [dbo].[AssociationUnitStationSet] DROP CONSTRAINT [FK_AssociationUnitStationSet_StationSet]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [FK_UnitSet_StationSet]
GO
ALTER TABLE [dbo].[VdvMessageSet] DROP CONSTRAINT [FK_VdvMessageSet_ItcsProviders]
GO
ALTER TABLE [dbo].[VdvMessageSet] DROP CONSTRAINT [FK_VdvMessageSet_UnitSet]
GO
DROP PROCEDURE [dbo].[AssociationUnitStation_Delete]
GO
DROP PROCEDURE [dbo].[AssociationUnitStation_Insert]
GO
DROP PROCEDURE [dbo].[Unit_SelectAssociatedStations]
GO
DROP PROCEDURE [dbo].[Layout_Delete]
GO
DROP PROCEDURE [dbo].[Layout_Insert]
GO
DROP VIEW [dbo].[WorkflowInstances]
GO
DROP VIEW [dbo].[Workflows]
GO
DROP VIEW [dbo].[AssociationsUnitStation]
GO
DROP VIEW [dbo].[Stations]
GO
DROP VIEW [dbo].[VdvMessages]
GO
DROP TABLE [dbo].[WorkflowSet]
GO
DROP TABLE [dbo].[WorkflowInstanceSet]
GO
DROP TABLE [dbo].[VdvMessageSet]
GO
DROP TABLE [dbo].[AssociationUnitStationSet]
GO
DROP TABLE [dbo].[StationSet]
GO
ALTER TABLE [dbo].[UnitSet] DROP COLUMN [StationId]
GO
ALTER TABLE [dbo].[ProductTypeSet] ADD 
[Properties] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[ProtocolConfigurationSet] ALTER COLUMN [Properties] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[ItcsFilterSet] ALTER COLUMN [Properties] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[ItcsDisplayAreas] ALTER COLUMN [Properties] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[LayoutSet] ALTER COLUMN [Definition] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
CREATE TABLE [dbo].[ItcsRealtimeDataSet]
(
	[ItcsTimeTableEntryId] [int] NOT NULL,
	[EstimatedArrival] [datetime2](7) NOT NULL,
	[EstimatedDeparture] [datetime2](7) NOT NULL,
	[RealArrival] [datetime2](7) NOT NULL,
	[RealDeparture] [datetime2](7) NOT NULL,
	[IsAtStation] [bit] NOT NULL,
	[CleardownReference] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ContainsRealtime] [bit] NOT NULL,
	[InCongestion] [bit] NOT NULL,
	[PlatformText] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PlatformId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsDeleted] [bit] NOT NULL,
	CONSTRAINT [PK_ItcsRealtimeDataSet] PRIMARY KEY CLUSTERED
	(
		[ItcsTimeTableEntryId] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ItcsTimeTableEntrySet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ItcsDisplayAreaId] [int] NOT NULL,
	[OperationalDay] [datetime2](7) NOT NULL,
	[LineId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DirectionId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TripId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[StopSequenceCounter] [int] NOT NULL,
	[LastUpdated] [datetime2](7) NOT NULL,
	[ValidUntil] [datetime2](7) NOT NULL,
	[LineText] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DirectionText] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ScheduledArrival] [datetime2](7) NULL,
	[ScheduledDeparture] [datetime2](7) NULL,
	CONSTRAINT [PK_ItcsTimeTableEntrySet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER VIEW [dbo].[Units]
AS
WITH Map([Type], [Status])
AS
(
	SELECT 0,1
	UNION ALL
	SELECT 1,2
	UNION ALL
	SELECT 2,4
	UNION ALL
	SELECT 3,8
)
, AlarmStatuses([Status], [UnitId])
AS
(
	SELECT  SUM(DISTINCT ISNULL([M].[Status], 1)) AS [Status], [A].[UnitId]
	FROM [AlarmSet] [A]
	LEFT OUTER JOIN [Map] [M] ON [M].[Type] = [A].[Type]
	WHERE [A].[DateConfirmed] IS NULL
	GROUP BY [A].[UnitId]
)
, [Operations_Aggregated] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	GROUP BY [A].[UnitId]
)
, [Operations_Error] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState]=1
	GROUP BY [A].[UnitId]
)
, [Operations_Revoking] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState]=2
	GROUP BY [A].[UnitId]
)
, [Operations_Active] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 3
	GROUP BY [A].[UnitId]
)
, [Operations_Transmitting] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 4
	GROUP BY [A].[UnitId]
)
, [Operations_Transmitted] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 5
	GROUP BY [A].[UnitId]
)
, [Operations_Scheduled] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 6
	GROUP BY [A].[UnitId]
)
, [Operations_Ended] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 7
	GROUP BY [A].[UnitId]
)
, [Operations_Revoked] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 8
	GROUP BY [A].[UnitId]
)
SELECT [U].[Id]
      ,[U].[TenantId]
      ,[U].[ProductTypeId]
      ,[U].[LayoutId]
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
      ,[U].[GatewayAddress]
      ,ISNULL(CAST([A].[Status] AS int), 0) AS [AlarmStatus]
      ,[U].[ErrorOperationsCount]
      ,[U].[RevokingOperationsCount]
      ,[U].[ActiveOperationsCount]
      ,[U].[TransmittingOperationsCount]
      ,[U].[TransmittedOperationsCount]
      ,[U].[ScheduledOperationsCount]
      ,[U].[EndedOperationsCount]
      ,[U].[RevokedOperationsCount]
      ,[U].[TotalOperationsCount]

FROM         [dbo].[UnitSet] [U]
LEFT OUTER JOIN [AlarmStatuses] [A] ON [A].[UnitId] = [U].[Id]

WHERE     [U].[IsDeleted] = 0
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[ClearItcsData]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    DECLARE @now datetime2(7) = GETUTCDATE()
    
    DELETE [ItcsRealtimeDataSet]
    FROM [ItcsRealtimeDataSet] [R]
    INNER JOIN [ItcsTimeTableEntrySet] [E] ON [E].[Id] = [R].[ItcsTimeTableEntryId]
    WHERE [ValidUntil] IS NOT NULL AND [ValidUntil] < @now
    
    DELETE FROM [ItcsTimeTableEntrySet]
    WHERE [ValidUntil] IS NOT NULL AND [ValidUntil] < @now
    
    DELETE FROM [ReferenceTextSet]
    WHERE [ValidUntil] IS NOT NULL AND [ValidUntil] < @now
END
GO
CREATE VIEW [dbo].[ItcsRealtimeData]
AS
SELECT [ItcsTimeTableEntryId]
      ,[EstimatedArrival]
      ,[EstimatedDeparture]
      ,[RealArrival]
      ,[RealDeparture]
      ,[IsAtStation]
      ,[CleardownReference]
      ,[ContainsRealtime]
      ,[InCongestion]
      ,[PlatformText]
      ,[PlatformId]
      ,[IsDeleted]
  FROM [Gorba_CenterOnline].[dbo].[ItcsRealtimeDataSet]
GO
CREATE VIEW [dbo].[ItcsTimeTableEntries]
AS
SELECT [Id]
      ,[ItcsDisplayAreaId]
      ,[OperationalDay]
      ,[LineId]
      ,[DirectionId]
      ,[TripId]
      ,[StopSequenceCounter]
      ,[LastUpdated]
      ,[ValidUntil]
      ,[LineText]
      ,[DirectionText]
      ,[ScheduledArrival]
      ,[ScheduledDeparture]
  FROM [Gorba_CenterOnline].[dbo].[ItcsTimeTableEntrySet]
GO
ALTER TABLE [dbo].[ItcsRealtimeDataSet] ADD CONSTRAINT [FK_ItcsRealtimeDataSet_ItcsTimeTableEntrySet] FOREIGN KEY
	(
		[ItcsTimeTableEntryId]
	)
	REFERENCES [dbo].[ItcsTimeTableEntrySet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ItcsTimeTableEntrySet] ADD CONSTRAINT [FK_ItcsTimeTableEntrySet_ItcsDisplayAreas] FOREIGN KEY
	(
		[ItcsDisplayAreaId]
	)
	REFERENCES [dbo].[ItcsDisplayAreas]
	(
		[Id]
	)
GO

ALTER VIEW [dbo].[ProductTypes]
AS
SELECT [Id]
      ,[UnitTypeId]
      ,[Name]
      ,[Revision]
      ,[Description]
      ,[IsDefault]
      ,[DateCreated]
      ,[DateModified]
      ,[Properties]
  FROM [Gorba_CenterOnline].[dbo].[ProductTypeSet]
WHERE     (IsDeleted = 0)

GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.23.55'
  ,@description = 'Added ItcsTimeTableEntrySet and ItcsRealtimeDataSet tables (and relative views). Removed VdvMessageSet, StationSet, WorkflowSet and WorkflowInstanceSet (and all relative views and SPs). Changed xml fields to varchar(MAX).'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 23
  ,@versionRevision = 55
  ,@dateCreated = '2012-10-24T00:00:00.000'
GO
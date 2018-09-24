 

USE Gorba_CenterOnline
GO

CREATE TABLE [dbo].[ActivityStateSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ActivityId] [int] NOT NULL,
	[EmittedDateTime] [datetime] NOT NULL,
	CONSTRAINT [PK_ActivityStateSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER VIEW [dbo].[Units]
AS
WITH AlarmStatuses([Status], [UnitId])
AS
(
	SELECT  SUM(DISTINCT [A].[Type]) [Status], [A].[UnitId]
	FROM [AlarmSet] [A]
	WHERE [A].[DateConfirmed] IS NULL
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

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.13.33'
  ,@description = 'Fixed error in the Units view.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 13
  ,@versionRevision = 33
  ,@dateCreated = '2012-06-06T07:10:00.000'
GO
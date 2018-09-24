

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[ItcsFilterSet] DROP CONSTRAINT [FK_ItcsFilters_StopPoints]
GO
ALTER TABLE [dbo].[ItcsFilterSet] DROP CONSTRAINT [FK_ItcsFilters_ItcsDisplayAreas]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [FK_UnitSet_ProductTypeSet1]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [FK_UnitSet_UnitGroupSet]
GO
ALTER TABLE [dbo].[UnitSet] DROP COLUMN [GroupId]
GO

-- =============================================
-- Author:		Jerome Coutant (jerome.coutant@gorba.com)
-- Create date: 10.10.2012
-- Description:	Create AssociationUnitUnitGroupSet table to link units and unit groups
-- =============================================
CREATE TABLE [dbo].[AssociationUnitUnitGroupSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[UnitId] [int] NOT NULL,
	[UnitGroupId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_AssociationUnitUnitGroupSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_AssociationUnitUnitGroupSet] PRIMARY KEY CLUSTERED
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
-- Create new view on [ItcsFiltersSet] called ItcsFilters
-- =============================================
ALTER VIEW [dbo].[ItcsFilters]
AS
SELECT     Id, StopPointId, ItcsDisplayAreaId, Properties, LineReference, DirectionText, DirectionReference, LineText, IsActive
FROM         dbo.ItcsFilterSet
WHERE     (IsActive = 1)
GO
CREATE VIEW [dbo].[AssociationUnitUnitGroup]
AS
SELECT     Id, UnitId, UnitGroupId
FROM         dbo.AssociationUnitUnitGroupSet
WHERE     (IsDeleted = 0)


;
GO
ALTER TABLE [dbo].[ItcsFilterSet] WITH NOCHECK ADD CONSTRAINT [FK_ItcsFilters_StopPoints] FOREIGN KEY
	(
		[StopPointId]
	)
	REFERENCES [dbo].[StopPointSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[AssociationUnitUnitGroupSet] ADD CONSTRAINT [FK_AssociationUnitUnitGroupSet_UnitGroupSet] FOREIGN KEY
	(
		[UnitGroupId]
	)
	REFERENCES [dbo].[UnitGroupSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[AssociationUnitUnitGroupSet] ADD CONSTRAINT [FK_AssociationUnitUnitGroupSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ItcsFilterSet] WITH NOCHECK ADD CONSTRAINT [FK_ItcsFilters_ItcsDisplayAreas] FOREIGN KEY
	(
		[ItcsDisplayAreaId]
	)
	REFERENCES [dbo].[ItcsDisplayAreas]
	(
		[Id]
	)
GO
-- =============================================
-- Author:		Jerome Coutant (jerome.coutant@gorba.com)
-- Create date: 04.10.2012
-- Description:	Select units associated with a specified unit group
-- =============================================
CREATE PROCEDURE [dbo].[UnitGroup_SelectAssociatedUnits]
(
	@unitId int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT [G].*
	FROM [UnitGroups] [G]
	INNER JOIN [AssociationUnitUnitGroup] [A] ON [A].[UnitId]=12 AND [A].[UnitGroupId]=[G].[Id]
	
	
END



;

;

;
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.22.53'
  ,@description = 'Add table, view and stored procedure to handle the relation 0..n between units and unit groups.Remove GroupId from UnitSet table and relatives database objects.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 22
  ,@versionRevision = 53
  ,@dateCreated = '2012-10-10T00:00:00.000'
GO
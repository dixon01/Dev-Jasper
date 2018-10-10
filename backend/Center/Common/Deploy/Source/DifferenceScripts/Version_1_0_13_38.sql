 

USE Gorba_CenterOnline
GO

ALTER VIEW [dbo].[Operations]
AS
WITH Mapping([Source], [Destination])
AS
(
	SELECT 4,1
	UNION ALL
	SELECT 6,2
)
, ActivityInstanceStatesView([State], [OperationId])
AS
(
	SELECT MAX(DISTINCT [S].[State]), [A].[OperationId]
	FROM [ActivityInstanceStateSet] [S]
	LEFT OUTER JOIN [ActivitySet] [A] ON [A].[Id]=[S].[ActivityId] AND [A].[IsDeleted]=0
	GROUP BY [A].[OperationId]
)
SELECT [O].[Id]
      ,[O].[UserId]
      ,[O].[StartDate]
      ,[O].[Name]
      ,[O].[StopDate]
      ,[O].[StartExecutionDayMon]
      ,[O].[StartExecutionDayTue]
      ,[O].[StartExecutionDayWed]
      ,[O].[StartExecutionDayThu]
      ,[O].[StartExecutionDayFri]
      ,[O].[StartExecutionDaySat]
      ,[O].[StartExecutionDaySun]
      ,[O].[Repetition]
      ,[O].[DateCreated]
      ,[O].[DateModified]
      ,ISNULL(CAST([S].[State] AS int), 0) AS [OperationState]
      ,[O].[ExecutionOnceStartDateKind]
      ,[O].[ExecutionOnceStopDateKind]
      ,[O].[ExecutionWeeklyStartDate]
      ,[O].[ExecutionWeeklyStopDate]
      ,[O].[ExecutionWeeklyBeginTime]
      ,[O].[ExecutionWeeklyEndTime]
      ,[O].[ExecutionWeeklyStopDateKind]
      ,[O].[WeekRepetition]
      ,[O].[ActivityStatus]
      , [O].[RevokedOn]
      , [O].[IsRevoked]
      , [O].[RevokedBy]
      , [O].[FavoriteFlag]
  FROM [dbo].[OperationSet] [O]
  LEFT OUTER JOIN [ActivityInstanceStatesView] [S] ON [S].[OperationId]=[O].[Id]
WHERE     ([O].IsDeleted = 0)

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
      , [IsDeleted] bit
      ,[OperationState] int
      ,[ExecutionOnceStartDateKind] int
      ,[ExecutionOnceStopDateKind] int
      ,[ExecutionWeeklyStartDate] datetime NULL
      ,[ExecutionWeeklyStopDate] datetime NULL
      ,[ExecutionWeeklyBeginTime] datetime NULL
      ,[ExecutionWeeklyEndTime] datetime NULL
      ,[ExecutionWeeklyStopDateKind] int
      ,[WeekRepetition] int
      , [ActivityStatus] int
      , [RevokedOn] datetime NULL
      , [IsRevoked] int
      , [RevokedBy] int NULL
      , [FavoriteFlag] int)

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
		FROM [OperationSet]
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
				FROM [OperationSet] [O]
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

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.13.38'
  ,@description = 'Edited the Operations view to get the state from the ActivityInfoStates.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 13
  ,@versionRevision = 38
  ,@dateCreated = '2012-06-11T08:10:00.000'
GO

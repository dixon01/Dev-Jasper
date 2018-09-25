 

USE Gorba_CenterOnline
GO

-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: February 16th, 2012
-- Description:	Retrieves the submitted activity for an operation on a specified unit.
-- ACHTUNG! In this version, all activities are returned, and not only active ones.
-- The management of the state of an activity should be implemented.
-- =============================================
CREATE PROCEDURE [dbo].[Operation_GetActivitySubmissions] 
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
      ,[WeekRepetition] int)


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
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.5.17'
  ,@description = 'Added SP GetActivitySubmissions'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 5
  ,@versionRevision = 17
  ,@dateCreated = '2012-02-16T11:20:00.000'
GO


/*
 * EPT 24.10.13
 * WARNING!!! always backup the database before executing the script
 * Extends the Operation table adding a not null column named Priority.
 * WARNING!!! For existing operations the priority 2 (icenter.online) will be set.
*/

USE [Gorba_CenterOnline]

-- Add the SuspendingActivitiesCount and SuspendActivitiesCount columns to OperationSet as nullable
ALTER TABLE [Gorba_CenterOnline].[dbo].[OperationSet]
ADD [SuspendingActivitiesCount] int NULL, [SuspendedActivitiesCount] int NULL
GO

-- Add the QueuingOperationsCount and SuspendedOperationsCount columns to UnitSet as nullable
ALTER TABLE [Gorba_CenterOnline].[dbo].[UnitSet]
ADD [SuspendingOperationsCount] int NULL, [SuspendedOperationsCount] int NULL
GO

-- Add the SuspendingActivityInstancesCount, SuspendedActivityInstancesCount and IsUpdating columns to ActivitySet as nullable
ALTER TABLE [Gorba_CenterOnline].[dbo].[ActivitySet]
ADD [SuspendingActivityInstancesCount] int NULL, [SuspendedActivityInstancesCount] int NULL, [IsUpdating] bit NULL
GO

BEGIN TRY --Start the Try Block..
	BEGIN TRANSACTION [ClearDatabase_Tx] -- Start the transaction..
	DECLARE @operationsCount int
	
	SELECT @operationsCount = COUNT(*)
	FROM [Gorba_CenterOnline].[dbo].[OperationSet]
	
	IF @operationsCount > 0
		BEGIN	 	
			UPDATE [Gorba_CenterOnline].[dbo].[OperationSet]
			SET [SuspendedActivitiesCount] = 0
				,[SuspendingActivitiesCount] = 0
		END
	-- ENDIF
	
	DECLARE @unitsCount int
	SELECT @unitsCount = COUNT(*)
	FROM [Gorba_CenterOnline].[dbo].[UnitSet]
	
	IF @unitsCount > 0
		BEGIN
			UPDATE [Gorba_CenterOnline].[dbo].[UnitSet]
			SET [SuspendedOperationsCount] = 0
				,[SuspendingOperationsCount] = 0
		END
	
	DECLARE @activitiesCount int
	SELECT @activitiesCount = COUNT(*)
	FROM [Gorba_CenterOnline].[dbo].[ActivitySet]
	
	IF @activitiesCount > 0
		BEGIN
			UPDATE [Gorba_CenterOnline].[dbo].[ActivitySet]
			SET [SuspendedActivityInstancesCount] = 0
				,[SuspendingActivityInstancesCount] = 0
				,[IsUpdating] = 0
		END
	
	--=============================================================
	-- Versioning
	--=============================================================
	DECLARE @RC int

	EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
	   @name = 'Version 1.4.48.66'
	  ,@description = 'Added columns SuspendedActivityInstancesCount and IsUpdating to table ActivitySet, column SuspendedOperationsCount to UnitSet, column SuspendedActivitiesCount to OperationSet and updated the views.'
	  ,@versionMajor = 1
	  ,@versionMinor = 4
	  ,@versionBuild = 48
	  ,@versionRevision = 66
	  ,@dateCreated = '2013-10-24T09:00:00.000'

	COMMIT TRAN -- Transaction Success!

END TRY

	BEGIN CATCH


	IF @@TRANCOUNT > 0


		ROLLBACK TRAN --RollBack in case of Error



-- you can Raise ERROR with RAISEERROR() Statement including the details of the exception

	DECLARE @errorMessage varchar(5000) = ERROR_MESSAGE()


	RAISERROR(@errorMessage, 11, 1)

END CATCH

GO

-- Remove the nullability from added columns
ALTER TABLE [Gorba_CenterOnline].[dbo].[OperationSet]
ALTER COLUMN [SuspendedActivitiesCount] int NOT NULL
GO

ALTER TABLE [Gorba_CenterOnline].[dbo].[OperationSet]
ALTER COLUMN [SuspendingActivitiesCount] int NOT NULL
GO

ALTER TABLE [Gorba_CenterOnline].[dbo].[UnitSet]
ALTER COLUMN [SuspendedOperationsCount] int NOT NULL
GO

ALTER TABLE [Gorba_CenterOnline].[dbo].[UnitSet]
ALTER COLUMN [SuspendingOperationsCount] int NOT NULL
GO

ALTER TABLE [Gorba_CenterOnline].[dbo].[ActivitySet]
ALTER COLUMN [SuspendedActivityInstancesCount] int NOT NULL
GO

ALTER TABLE [Gorba_CenterOnline].[dbo].[ActivitySet]
ALTER COLUMN [SuspendingActivityInstancesCount] int NOT NULL
GO

ALTER TABLE [Gorba_CenterOnline].[dbo].[ActivitySet]
ALTER COLUMN [IsUpdating] BIT NOT NULL
GO

ALTER VIEW [dbo].[Operations]
AS
SELECT     [Id], [UserId], [StartDate], [Name], [StopDate], [StartExecutionDayMon], [StartExecutionDayTue], [StartExecutionDayWed], [StartExecutionDayThu], [StartExecutionDayFri], 
                      [StartExecutionDaySat], [StartExecutionDaySun], [Repetition], [DateCreated], [DateModified], 0 AS OperationState, [ExecutionOnceStartDateKind], [ExecutionOnceStopDateKind], 
                      [ExecutionWeeklyStartDate], [ExecutionWeeklyStopDate], [ExecutionWeeklyBeginTime], [ExecutionWeeklyEndTime], [ExecutionWeeklyStopDateKind], [WeekRepetition], 
                      [ActivityStatus], [RevokedOn], [IsRevoked], [RevokedBy], [FavoriteFlag], [Priority], TotalActivitiesCount AS ActivitiesCount, [TotalActivitiesCount], [ErrorActivitiesCount], 
                      [RevokingActivitiesCount], [ActiveActivitiesCount], [TransmittingActivitiesCount], [TransmittedActivitiesCount], [ScheduledActivitiesCount], [EndedActivitiesCount], 
                      [RevokedActivitiesCount], [SuspendingActivitiesCount], [SuspendedActivitiesCount]
FROM         [Gorba_CenterOnline].[dbo].[OperationSet]
WHERE     (IsDeleted = 0)
GO

ALTER VIEW [dbo].[Activities]
AS
SELECT     Id, OperationId, DateCreated, DateModified, IsDeleted, RealTaskId, LastRealTaskCreationDateTime, CurrentState, ErrorActivityInstancesCount, 
                      RevokingActivityInstancesCount, ActiveActivityInstancesCount, TransmittingActivityInstancesCount, TransmittedActivityInstancesCount, 
                      ScheduledActivityInstancesCount, EndedActivityInstancesCount, RevokedActivityInstancesCount, CreatedActivityInstancesCount, SchedulingActivityInstancesCount, 
					  SuspendingActivityInstancesCount, SuspendedActivityInstancesCount, TotalActivityInstancesCount, RemovedOn, IsRemoved, IsUpdating
FROM         [Gorba_CenterOnline].[dbo].[ActivitySet]
WHERE     (IsDeleted = 0)
GO

ALTER VIEW [dbo].[Units]
AS
WITH Map([Type], [Status]) AS (SELECT     0 AS Expr1, 1 AS Expr2
                                                                   UNION ALL
                                                                   SELECT     1 AS Expr1, 2 AS Expr2
                                                                   UNION ALL
                                                                   SELECT     2 AS Expr1, 4 AS Expr2
                                                                   UNION ALL
                                                                   SELECT     3 AS Expr1, 8 AS Expr2), AlarmStatuses([Status], [UnitId]) AS
    (SELECT     SUM(DISTINCT ISNULL(M.[Status], 1)) AS Status, A.UnitId
      FROM          dbo.AlarmSet AS A LEFT OUTER JOIN
                             Map AS M ON M.[Type] = A.Type
      WHERE      (A.DateConfirmed IS NULL)
      GROUP BY A.UnitId), Operations_Aggregated AS
    (SELECT     COUNT(*) AS Count, A.UnitId
      FROM          dbo.Operations AS O INNER JOIN
                             dbo.AssociationsUnitOperation AS A ON A.OperationId = O.Id
      GROUP BY A.UnitId), Operations_Error AS
    (SELECT     COUNT(*) AS Count, A.UnitId
      FROM          dbo.Operations AS O INNER JOIN
                             dbo.AssociationsUnitOperation AS A ON A.OperationId = O.Id
      WHERE      (O.OperationState = 1)
      GROUP BY A.UnitId), Operations_Revoking AS
    (SELECT     COUNT(*) AS Count, A.UnitId
      FROM          dbo.Operations AS O INNER JOIN
                             dbo.AssociationsUnitOperation AS A ON A.OperationId = O.Id
      WHERE      (O.OperationState = 2)
      GROUP BY A.UnitId), Operations_Active AS
    (SELECT     COUNT(*) AS Count, A.UnitId
      FROM          dbo.Operations AS O INNER JOIN
                             dbo.AssociationsUnitOperation AS A ON A.OperationId = O.Id
      WHERE      (O.OperationState = 3)
      GROUP BY A.UnitId), Operations_Transmitting AS
    (SELECT     COUNT(*) AS Count, A.UnitId
      FROM          dbo.Operations AS O INNER JOIN
                             dbo.AssociationsUnitOperation AS A ON A.OperationId = O.Id
      WHERE      (O.OperationState = 4)
      GROUP BY A.UnitId), Operations_Transmitted AS
    (SELECT     COUNT(*) AS Count, A.UnitId
      FROM          dbo.Operations AS O INNER JOIN
                             dbo.AssociationsUnitOperation AS A ON A.OperationId = O.Id
      WHERE      (O.OperationState = 5)
      GROUP BY A.UnitId), Operations_Scheduled AS
    (SELECT     COUNT(*) AS Count, A.UnitId
      FROM          dbo.Operations AS O INNER JOIN
                             dbo.AssociationsUnitOperation AS A ON A.OperationId = O.Id
      WHERE      (O.OperationState = 6)
      GROUP BY A.UnitId), Operations_Ended AS
    (SELECT     COUNT(*) AS Count, A.UnitId
      FROM          dbo.Operations AS O INNER JOIN
                             dbo.AssociationsUnitOperation AS A ON A.OperationId = O.Id
      WHERE      (O.OperationState = 7)
      GROUP BY A.UnitId), Operations_Revoked AS
    (SELECT     COUNT(*) AS Count, A.UnitId
      FROM          dbo.Operations AS O INNER JOIN
                             dbo.AssociationsUnitOperation AS A ON A.OperationId = O.Id
      WHERE      (O.OperationState = 8)
      GROUP BY A.UnitId), Operations_Suspended AS
	(SELECT     COUNT(*) AS Count, A.UnitId
      FROM          dbo.Operations AS O INNER JOIN
                             dbo.AssociationsUnitOperation AS A ON A.OperationId = O.Id
      WHERE      (O.OperationState = 9)
      GROUP BY A.UnitId), Operations_Suspending AS
	(SELECT     COUNT(*) AS Count, A.UnitId
      FROM          dbo.Operations AS O INNER JOIN
                             dbo.AssociationsUnitOperation AS A ON A.OperationId = O.Id
      WHERE      (O.OperationState = 10)
      GROUP BY A.UnitId)
    SELECT     U.Id, U.TenantId, U.ProductTypeId, U.LayoutId, U.Name, U.ShortName, U.SystemName, U.SerialNumber, U.Description, U.DateCreated, U.DateModified, 
                            U.NetworkAddress, U.IsOnline, U.LastSeenOnline, U.LocationName, U.CommunicationStatus, U.OperationStatus, U.LastRestartRequestDate, 
                            U.LastTimeSyncRequestDate, U.LastTimeSyncValue, U.TimeZoneInfoId, U.GatewayAddress, ISNULL(CAST(A.[Status] AS int), 0) AS AlarmStatus, 
                            U.ErrorOperationsCount, U.RevokingOperationsCount, U.ActiveOperationsCount, U.TransmittingOperationsCount, U.TransmittedOperationsCount, 
                            U.ScheduledOperationsCount, U.EndedOperationsCount, U.RevokedOperationsCount, U.SuspendingOperationsCount, U.SuspendedOperationsCount, U.TotalOperationsCount
     FROM         dbo.UnitSet AS U LEFT OUTER JOIN
                            AlarmStatuses AS A ON A.[UnitId] = U.Id
     WHERE     (U.IsDeleted = 0)

GO


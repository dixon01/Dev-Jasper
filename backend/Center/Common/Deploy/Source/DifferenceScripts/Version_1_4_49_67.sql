/*
 * LEF 11.07.13
 * WARNING!!! always backup the database before executing the script
 * Moving the IsUpdating property from Activity to ActivityInstance.
 * We assume that the IsUpdating flag on Activity has not been used yet.
 * In any case, this update should be run without operations currently being updated.
*/

USE [Gorba_CenterOnline]

-- Add the IsUpdating columns to ActivitySet with default value of 0 (false).
ALTER TABLE [Gorba_CenterOnline].[dbo].[ActivityInstanceSet]
ADD [IsUpdating] bit NOT NULL
CONSTRAINT [ActivityInstance_IsUpdating_Default] DEFAULT(0)
GO

-- Add the IsUpdating columns to ActivitySet with default value of 0 (false).
ALTER TABLE [Gorba_CenterOnline].[dbo].[ActivitySet]
DROP COLUMN [IsUpdating]
GO

ALTER VIEW [dbo].[Activities]
AS
SELECT     Id, OperationId, DateCreated, DateModified, IsDeleted, RealTaskId, LastRealTaskCreationDateTime, CurrentState, ErrorActivityInstancesCount, 
                      RevokingActivityInstancesCount, ActiveActivityInstancesCount, TransmittingActivityInstancesCount, TransmittedActivityInstancesCount, 
                      ScheduledActivityInstancesCount, EndedActivityInstancesCount, RevokedActivityInstancesCount, CreatedActivityInstancesCount, SchedulingActivityInstancesCount, 
					  SuspendingActivityInstancesCount, SuspendedActivityInstancesCount, TotalActivityInstancesCount, RemovedOn, IsRemoved
FROM         [Gorba_CenterOnline].[dbo].[ActivitySet]
WHERE     (IsDeleted = 0)

GO
	
	--=============================================================
	-- Versioning
	--=============================================================
	DECLARE @RC int

	EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
	   @name = 'Version 1.4.49.67'
	  ,@description = 'Moved column IsUpdating from ActivitySet to ActivityInstanceSet and updated the views.'
	  ,@versionMajor = 1
	  ,@versionMinor = 4
	  ,@versionBuild = 49
	  ,@versionRevision = 67
	  ,@dateCreated = '2013-11-07T09:00:00.000'
GO


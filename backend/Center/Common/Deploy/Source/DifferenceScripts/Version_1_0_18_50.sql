 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[ActivityBusLineOnOffSet] DROP CONSTRAINT [FK_ActivityBusLineOnOffSet_ActivitySet]
GO
ALTER TABLE [dbo].[ActivityBusLineOnOffSet] DROP CONSTRAINT [PK_ActivityBusLineOnOffSet]
GO
ALTER TABLE [dbo].[AlarmSet] ADD 
[IsConfirmed] AS (CONVERT([bit],case when [UserId] IS NOT NULL then (1) else (0) end,0)) PERSISTED
GO
CREATE TABLE [dbo].[TempActivityBusLineOnOffSet]
(
	[Id] [int] NOT NULL,
	[Activate] [bit] NOT NULL,
	[Line] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ActivationMode] [int] NOT NULL,
	[SpecialText] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

) ON [PRIMARY]
GO

INSERT INTO [dbo].[TempActivityBusLineOnOffSet] ([Id],[Activate],[Line],[SpecialText],[ActivationMode]) SELECT [Id],[Activate],[Line],[SpecialText],0 FROM [dbo].[ActivityBusLineOnOffSet]
DROP TABLE [dbo].[ActivityBusLineOnOffSet]
GO
EXEC sp_rename N'[dbo].[TempActivityBusLineOnOffSet]',N'ActivityBusLineOnOffSet', 'OBJECT'
GO


ALTER VIEW [dbo].[Activities]
AS
SELECT [Id]
      ,[OperationId]
      ,[DateCreated]
      ,[DateModified]
      ,[IsDeleted]
      ,[RealTaskId]
      ,[LastRealTaskCreationDateTime]
      ,[CurrentState]
      ,[ErrorActivityInstancesCount]
      ,[RevokingActivityInstancesCount]
      ,[ActiveActivityInstancesCount]
      ,[TransmittingActivityInstancesCount]
      ,[TransmittedActivityInstancesCount]
      ,[ScheduledActivityInstancesCount]
      ,[EndedActivityInstancesCount]
      ,[RevokedActivityInstancesCount]
      ,[CreatedActivityInstancesCount]
      , [SchedulingActivityInstancesCount]
      , [TotalActivityInstancesCount]
  FROM [Gorba_CenterOnline].[dbo].[ActivitySet]
WHERE     (IsDeleted = 0)
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[ClearOperations]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    
    DELETE FROM ActivityTaskSet
    DELETE FROM ActivityInstanceSet
    
    DELETE FROM ActivityBusLineOnOffSet
    DELETE FROM ActivityDeleteTripsSet
    DELETE FROM ActivityDisplayOnOffSet
    DELETE FROM AnnouncementTextSet
    DELETE FROM ActivityAnnouncementTextSet
    DELETE FROM ActivityInfoTextSet
    DELETE FROM ActivitySet
    
    DELETE FROM AssociationUnitOperationSet
    
    DELETE FROM OperationSet
    
    UPDATE [UnitSet]
    SET [OperationStatus] = 0,
     [TotalOperationsCount] = 0,
    [ErrorOperationsCount] = 0,
    [RevokingOperationsCount] = 0,
    [ActiveOperationsCount] = 0,
    [TransmittingOperationsCount] = 0,
    [TransmittedOperationsCount] = 0,
    [ScheduledOperationsCount] = 0,
    [EndedOperationsCount] = 0,
    [RevokedOperationsCount] = 0
END
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UnitSet_NetworkAddress] ON [dbo].[UnitSet]
(
	[NetworkAddress] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ActivityBusLineOnOffSet] ADD CONSTRAINT [PK_ActivityBusLineOnOffSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ActivityBusLineOnOffSet] ADD CONSTRAINT [FK_ActivityBusLineOnOffSet_ActivitySet] FOREIGN KEY
	(
		[Id]
	)
	REFERENCES [dbo].[ActivitySet]
	(
		[Id]
	)
GO





--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.18.50'
  ,@description = 'Added computed IsConfirmed column, updated ActivityBusLineOnOff.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 18
  ,@versionRevision = 50
  ,@dateCreated = '2012-08-16T08:47:00.000'
GO
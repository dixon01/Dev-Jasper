SELECT *
  FROM [Gorba_CenterControllers].[System.Activities.DurableInstancing].[Instances]
GO


SELECT [InstanceId]
      ,[CurrentTaskIdentifier]
      ,[UnitId]
      ,[ActivityId]
      , [IsDeleted]
  FROM [Gorba_CenterControllersMetabase].[dbo].[ActivityInstanceControllerSet]
GO

SELECT [InstanceId]
      ,[ActivityTaskId]
      ,[UnitId]
      ,[ActivityId]
      , [IsDeleted]
  FROM [Gorba_CenterControllersMetabase].[dbo].[ActivityTaskLifecycleSet]
GO


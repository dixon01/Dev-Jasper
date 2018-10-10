 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[ItcsRealtimeDataSet] ALTER COLUMN [RealArrival] [datetime2](7) NULL
GO
ALTER TABLE [dbo].[ItcsRealtimeDataSet] ALTER COLUMN [RealDeparture] [datetime2](7) NULL
GO
ALTER TABLE [dbo].[ItcsRealtimeDataSet] ALTER COLUMN [EstimatedArrival] [datetime2](7) NULL
GO
ALTER TABLE [dbo].[ItcsRealtimeDataSet] ALTER COLUMN [EstimatedDeparture] [datetime2](7) NULL
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.24.57'
  ,@description = 'Change Real and Estimated datetime columns of ItcsRealtimeDataSet to allow NULL.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 24
  ,@versionRevision = 57
  ,@dateCreated = '2012-11-07T11:30:00.000'
GO

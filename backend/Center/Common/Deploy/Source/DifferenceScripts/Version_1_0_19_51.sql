 

USE Gorba_CenterOnline
GO

ALTER VIEW [dbo].[Alarms]
AS
SELECT     Id, Name, UnitId, UserId, Description, Severity, Type, ConfirmationText, DateCreated, EndDate, DateConfirmed, DateModified, DateReceived, UnconfirmedSince, 
                      AlarmMessageId, IsConfirmed
FROM         dbo.AlarmSet
WHERE     (IsDeleted = 0)
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.19.51'
  ,@description = 'Added IsConfirmed to Alarms view.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 19
  ,@versionRevision = 51
  ,@dateCreated = '2012-08-24T14:44:00.000'
GO
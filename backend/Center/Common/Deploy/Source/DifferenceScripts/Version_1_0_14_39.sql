 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[UserSet] ADD 
[TimeZoneInfoId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[AlarmSet] ADD 
[AlarmMessageId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[ActivitySet] ADD 
[RealTaskId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER VIEW [dbo].[Users]
AS
SELECT     Id, Username, HashedPassword, Name, LastName, Email, DateCreated, DateModified, IsDeleted, Culture, ShortName, TimeZoneInfoId
FROM         dbo.UserSet
GO
ALTER VIEW [dbo].[Alarms]
AS
SELECT     Id, Name, UnitId, UserId, Description, Severity, Type, ConfirmationText, DateCreated, EndDate, DateConfirmed, DateModified, DateReceived, UnconfirmedSince, 
                      AlarmMessageId
FROM         dbo.AlarmSet
WHERE     (IsDeleted = 0)
GO
ALTER VIEW [dbo].[Activities]
AS
SELECT     Id, OperationId, DateCreated, DateModified, RealTaskId
FROM         dbo.ActivitySet
WHERE     (IsDeleted = 0)
GO

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.14.39'
  ,@description = 'Added TimeZoneInfoId to UserSet, RealTaskId to ActivitySet, AlarmMessageId to AlarmSet and updated the views.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 14
  ,@versionRevision = 39
  ,@dateCreated = '2012-06-20T10:41:00.000'
GO
 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[ActivityInstanceStateSet] DROP CONSTRAINT [PK_ActivityInstanceStateSet]
GO
CREATE TABLE [dbo].[TempActivityInstanceStateSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ActivityId] [int] NOT NULL,
	[State] [int] NOT NULL,
	[EmittedDateTime] [datetime] NOT NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempActivityInstanceStateSet] ON
INSERT INTO [dbo].[TempActivityInstanceStateSet] ([Id],[ActivityId],[EmittedDateTime],[State]) SELECT [Id],[ActivityId],[EmittedDateTime],0 FROM [dbo].[ActivityInstanceStateSet]
SET IDENTITY_INSERT [dbo].[TempActivityInstanceStateSet] OFF
GO

DROP TABLE [dbo].[ActivityInstanceStateSet]
GO
EXEC sp_rename N'[dbo].[TempActivityInstanceStateSet]',N'ActivityInstanceStateSet', 'OBJECT'
GO


ALTER TABLE [dbo].[ActivityInstanceStateSet] ADD CONSTRAINT [PK_ActivityInstanceStateSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ActivityInstanceStateSet] ADD CONSTRAINT [FK_ActivityInstanceStateSet_ActivitySet] FOREIGN KEY
	(
		[ActivityId]
	)
	REFERENCES [dbo].[ActivitySet]
	(
		[Id]
	)
GO

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.13.37'
  ,@description = 'Added missing State column to the ActivityInstanceState table.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 13
  ,@versionRevision = 37
  ,@dateCreated = '2012-06-08T11:10:00.000'
GO


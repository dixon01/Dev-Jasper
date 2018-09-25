 

USE Gorba_CenterOnline
GO

CREATE TABLE [dbo].[ActivityDeleteTripsSet]
(
	[Id] [int] NOT NULL,
	[Trips] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT [PK_ActivityDeleteTripsSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ActivityDisplayOnOffSet]
(
	[Id] [int] NOT NULL,
	[NewState] [int] NOT NULL,
	CONSTRAINT [PK_ActivityDisplayOnOffSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ActivityDisplayOnOffSet] ADD CONSTRAINT [FK_ActivityDisplayOnOffSet_ActivitySet] FOREIGN KEY
	(
		[Id]
	)
	REFERENCES [dbo].[ActivitySet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ActivityDeleteTripsSet] ADD CONSTRAINT [FK_ActivityDeleteTripsSet_ActivitySet] FOREIGN KEY
	(
		[Id]
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
   @name = 'Version 1.0.13.36'
  ,@description = 'Added ActivityDisplayOnOff and ActivityDeleteTrips.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 13
  ,@versionRevision = 36
  ,@dateCreated = '2012-06-07T11:10:00.000'
GO


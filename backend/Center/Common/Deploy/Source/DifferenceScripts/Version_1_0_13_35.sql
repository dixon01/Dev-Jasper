 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[DirectionReferences] DROP CONSTRAINT [FK_DirectionReferences_ItcsProviders]
GO
ALTER TABLE [dbo].[LineReferences] DROP CONSTRAINT [FK_LineReferences_ItcsProviders]
GO
ALTER TABLE [dbo].[ItcsDisplayAreas] DROP CONSTRAINT [FK_ItcsDisplayAreas_ItcsProviders]
GO
ALTER TABLE [dbo].[ItcsProviders] DROP CONSTRAINT [PK_ItcsProviders]
GO
DROP PROCEDURE [dbo].[VdvProtocolConfiguration_Insert]
GO
DROP VIEW [dbo].[ProtocolConfigurations]
GO
CREATE TABLE [dbo].[TempItcsProviders]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsDeleted] [bit] NOT NULL,
	[Properties] [xml] NULL,
	[ProtocolTypeId] [int] NOT NULL

) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempItcsProviders] ON
INSERT INTO [dbo].[TempItcsProviders] ([Id],[Name],[Description],[IsDeleted],[Properties],[ProtocolTypeId]) SELECT [Id],[Name],[Description],[IsDeleted],[Properties],0 FROM [dbo].[ItcsProviders]
SET IDENTITY_INSERT [dbo].[TempItcsProviders] OFF
GO

DROP TABLE [dbo].[ItcsProviders]
GO
EXEC sp_rename N'[dbo].[TempItcsProviders]',N'ItcsProviders', 'OBJECT'
GO


ALTER TABLE [dbo].[ItcsProviders] ADD CONSTRAINT [PK_ItcsProviders] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DirectionReferences] ADD CONSTRAINT [FK_DirectionReferences_ItcsProviders] FOREIGN KEY
	(
		[ProviderId]
	)
	REFERENCES [dbo].[ItcsProviders]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[LineReferences] ADD CONSTRAINT [FK_LineReferences_ItcsProviders] FOREIGN KEY
	(
		[ProviderId]
	)
	REFERENCES [dbo].[ItcsProviders]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ItcsDisplayAreas] ADD CONSTRAINT [FK_ItcsDisplayAreas_ItcsProviders] FOREIGN KEY
	(
		[ProviderId]
	)
	REFERENCES [dbo].[ItcsProviders]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ItcsProviders] ADD CONSTRAINT [FK_ItcsProviders_ProtocolTypeSet] FOREIGN KEY
	(
		[ProtocolTypeId]
	)
	REFERENCES [dbo].[ProtocolTypeSet]
	(
		[Id]
	)
GO

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.13.35'
  ,@description = 'Removed ItcsConfiguration and ProtocolConfiguration.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 13
  ,@versionRevision = 35
  ,@dateCreated = '2012-06-07T09:10:00.000'
GO
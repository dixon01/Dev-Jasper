 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[InfoLineTextActivitySet] DROP CONSTRAINT [FK_InfoLineTextSet_StationSet]
GO
ALTER TABLE [dbo].[InfoLineTextActivitySet] DROP CONSTRAINT [FK_InfoLineTextActivitySet_ActivitySet]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] DROP CONSTRAINT [FK_ItcsConfiguration_ProtocolConfiguration]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] DROP CONSTRAINT [DF_ItcsConfiguration_collectSystemData]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] DROP CONSTRAINT [DF_ItcsConfiguration_IsDeleted]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] DROP CONSTRAINT [DF_ItcsConfiguration_dayLightSaving]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] DROP CONSTRAINT [PK_ItcsConfiguration]
GO
ALTER TABLE [dbo].[InfoLineTextActivitySet] DROP CONSTRAINT [PK_InfoLineTextActivitySet]
GO
DROP PROCEDURE [dbo].[InfoLineText_Insert]
GO
DROP INDEX [IX_ItcsConfigurationName] ON [dbo].[ItcsConfigurationSet]
GO
CREATE TABLE [dbo].[TempInfoLineTextActivitySet]
(
	[Id] [int] NOT NULL,
	[UnitId] [int] NOT NULL,
	[LineNumber] [int] NOT NULL,
	[DestinationId] [int] NOT NULL,
	[DisplayText] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExpirationTime] [datetime] NOT NULL,
	[InfoRowId] [smallint] NOT NULL,
	[Blink] [bit] NOT NULL,
	[DisplayedScreenSide] [smallint] NOT NULL,
	[Alignment] [smallint] NOT NULL,
	[Font] [smallint] NULL

) ON [PRIMARY]
GO

INSERT INTO [dbo].[TempInfoLineTextActivitySet] ([Id],[LineNumber],[DestinationId],[DisplayText],[ExpirationTime],[InfoRowId],[Blink],[DisplayedScreenSide],[Alignment],[Font],[UnitId]) SELECT [Id],[LineNumber],[DestinationId],[DisplayText],[ExpirationTime],[InfoRowId],[Blink],[DisplayedScreenSide],[Alignment],[Font],0 FROM [dbo].[InfoLineTextActivitySet]
DROP TABLE [dbo].[InfoLineTextActivitySet]
GO
EXEC sp_rename N'[dbo].[TempInfoLineTextActivitySet]',N'InfoLineTextActivitySet', 'OBJECT'
GO


CREATE TABLE [dbo].[TempItcsConfigurationSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ProtocolConfigurationId] [int] NULL,
	[Name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CollectSystemData] [bit] NOT NULL CONSTRAINT [DF_ItcsConfiguration_collectSystemData] DEFAULT ((1)),
	[OperationDayStartUtc] [datetime] NOT NULL,
	[OperationDayDuration] [datetime] NOT NULL,
	[UtcOffset] [datetime] NOT NULL,
	[DayLightSaving] [bit] NOT NULL CONSTRAINT [DF_ItcsConfiguration_dayLightSaving] DEFAULT ((0)),
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_ItcsConfiguration_IsDeleted] DEFAULT ((0))

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempItcsConfigurationSet] ON
INSERT INTO [dbo].[TempItcsConfigurationSet] ([Id],[Name],[Description],[CollectSystemData],[OperationDayStartUtc],[OperationDayDuration],[UtcOffset],[DayLightSaving],[IsDeleted],[ProtocolConfigurationId]) SELECT [Id],[Name],[Description],[CollectSystemData],[OperationDayStartUtc],[OperationDayDuration],[UtcOffset],[DayLightSaving],[IsDeleted],[ProtocolConfigurationId] FROM [dbo].[ItcsConfigurationSet]
SET IDENTITY_INSERT [dbo].[TempItcsConfigurationSet] OFF
GO

DROP TABLE [dbo].[ItcsConfigurationSet]
GO
EXEC sp_rename N'[dbo].[TempItcsConfigurationSet]',N'ItcsConfigurationSet', 'OBJECT'
GO


ALTER VIEW [dbo].[InfoLineTextActivities]
AS
SELECT     Base.Id, Base.OperationId, Base.Attribute, Base.DateCreated, Base.DateModified, Derived.UnitId, Derived.LineNumber, Derived.DestinationId, Derived.DisplayText, 
                      Derived.ExpirationTime, Derived.InfoRowId, Derived.Blink, Derived.DisplayedScreenSide, Derived.Alignment, Derived.Font
FROM         dbo.InfoLineTextActivitySet AS Derived INNER JOIN
                      dbo.ActivitySet AS Base ON Derived.Id = Base.Id
WHERE     (Base.IsDeleted = 0)
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: 2011-12-07
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Activity_Insert]
(
	@operationId int
	, @attribute varchar(1000)
	, @dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Activity_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [ActivitySet]
			( [OperationId]
			, [Attribute]
			, [DateCreated]
			)
			VALUES
			( @operationId
			, @attribute
			, @dateCreated
			)
			
			DECLARE @id int = SCOPE_IDENTITY()
			SELECT @id			
			COMMIT TRAN -- Transaction Success!

			RETURN @id
	END TRY

	BEGIN CATCH


		IF @@TRANCOUNT > 0
			BEGIN
				ROLLBACK TRAN --RollBack in case of Error
			END
			
			DECLARE @message varchar(500) = ERROR_MESSAGE()
			DECLARE @severity int = ERROR_SEVERITY()
			RAISERROR(@message, @severity, 1)

	END CATCH
END
GO
CREATE NONCLUSTERED INDEX [IX_ItcsConfigurationName] ON [dbo].[ItcsConfigurationSet]
(
	[name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InfoLineTextActivitySet] ADD CONSTRAINT [PK_InfoLineTextActivitySet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ADD CONSTRAINT [PK_ItcsConfiguration] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InfoLineTextActivitySet] ADD CONSTRAINT [FK_InfoLineTextActivitySet_ActivitySet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ADD CONSTRAINT [FK_ItcsConfiguration_ProtocolConfiguration] FOREIGN KEY
	(
		[ProtocolConfigurationId]
	)
	REFERENCES [dbo].[ProtocolConfigurationSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[InfoLineTextActivitySet] ADD CONSTRAINT [FK_InfoLineTextActivitySet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InfoLineTextActivity_Delete] 
(
	@id int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		EXEC dbo.Activity_Delete @id = @id;

	END TRY

	BEGIN CATCH

		IF @@TRANCOUNT > 0
			BEGIN
				ROLLBACK TRAN --RollBack in case of Error
			END
			
			DECLARE @message varchar(500) = ERROR_MESSAGE()
			DECLARE @severity int = ERROR_SEVERITY()
			RAISERROR(@message, @severity, 1)

	END CATCH
END
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 7.12.11 08:50
-- Description:	Adds an info line text to the system
-- =============================================
CREATE PROCEDURE [dbo].[InfoLineTextActivity_Insert] 
	( @operationId int
	, @attribute varchar(1000)
	, @dateCreated datetime = NULL
	
	, @unitId int
	, @lineNumber int
	, @destinationId int
	, @displayText varchar(100)
	, @expirationTime datetime
	, @infoRowId smallint
	, @blink bit
	, @displayedScreenSide smallint
	, @alignment smallint
	, @font smallint
	)
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @id int;
	
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION InfoLineTextActivity_InsertTx -- Start the transaction..
		-- Call base stored procedure Activity_Insert to insert first the fields of
		-- base entity
		EXEC @id = dbo.Activity_Insert @operationId, @attribute, @dateCreated;
		
		-- insert the fields of the derived entity		
			INSERT INTO [InfoLineTextActivitySet]
			( [Id]
			, [UnitId]
			, [LineNumber]
			, [DestinationId]
			, [DisplayText]
			, [ExpirationTime]
			, [InfoRowId]
			, [Blink]
			, [DisplayedScreenSide]
			, [Alignment]
			, [Font]
			)
			VALUES
			( @id
			, @unitId
			, @lineNumber
			, @destinationId
			, @displayText 
			, @expirationTime
			, @infoRowId
			, @blink
			, @displayedScreenSide
			, @alignment
			, @font
			)
			
			SELECT @id
			
			COMMIT TRAN -- Transaction Success!

	END TRY

	BEGIN CATCH


		IF @@TRANCOUNT > 0
			BEGIN
				ROLLBACK TRAN --RollBack in case of Error
			END
			
			DECLARE @message varchar(500) = ERROR_MESSAGE()
			DECLARE @severity int = ERROR_SEVERITY()
			RAISERROR(@message, @severity, 1)

	END CATCH
	END
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.2.11'
  ,@description = 'Renamed SP InfoLineText_Insert into InfoLineTextActivity_Insert and updated it. Added SP InfoLineTextActivity_Delete. Both call the base SP activity_XXX.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 2
  ,@versionRevision = 11
  ,@dateCreated = '2012-01-17T11:20:00.000'
GO


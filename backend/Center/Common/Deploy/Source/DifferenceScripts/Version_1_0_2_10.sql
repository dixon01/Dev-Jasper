 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[ItcsConfigurationSet] DROP CONSTRAINT [FK_ItcsConfiguration_ProtocolConfiguration]
GO
ALTER TABLE [dbo].[InfoLineTextSet] DROP CONSTRAINT [FK_InfoLineTextSet_StationSet]
GO
ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [DF_ActivitySet_TypeId]
GO
DROP VIEW [dbo].[InfoLineTexts]
GO
DROP INDEX [IX_ItcsConfigurationName] ON [dbo].[ItcsConfigurationSet]
GO
DROP TABLE [dbo].[InfoLineTextSet]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ALTER COLUMN [Name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] DROP COLUMN [ItcsConfigurationId]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ADD 
[ProtocolConfigurationId] [int] NULL
GO
ALTER TABLE [dbo].[ActivitySet] DROP COLUMN [TypeId]
GO
CREATE TABLE [dbo].[InfoLineTextActivitySet]
(
	[Id] [int] NOT NULL,
	[ItcsStationId] [int] NOT NULL,
	[LineNumber] [int] NOT NULL,
	[DestinationId] [int] NOT NULL,
	[DisplayText] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExpirationTime] [datetime] NOT NULL,
	[InfoRowId] [smallint] NOT NULL,
	[Blink] [bit] NOT NULL,
	[DisplayedScreenSide] [smallint] NOT NULL,
	[Alignment] [smallint] NOT NULL,
	[Font] [smallint] NULL,
	CONSTRAINT [PK_InfoLineTextActivitySet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER VIEW [dbo].[ProtocolTypes]
AS
SELECT     Id, Description, Name
FROM         dbo.ProtocolTypeSet
GO
ALTER VIEW [dbo].[Activities]
AS
SELECT     Id, OperationId, Attribute, DateCreated, DateModified
FROM         dbo.ActivitySet
WHERE     (IsDeleted = 0)
GO
ALTER VIEW [dbo].[ItcsConfigurations]
AS
SELECT     Id, Name, Description, CollectSystemData, OperationDayStartUtc, OperationDayDuration, UtcOffset, DayLightSaving, ProtocolConfigurationId
FROM         dbo.ItcsConfigurationSet
WHERE     (IsDeleted = 0)
GO
ALTER VIEW [dbo].[ProtocolConfigurations]
AS
SELECT     Id, ProtocolTypeId, RealTimeDataOnly, HttpListenerHost, HttpListenerPort, HttpServerHost, HttpServerport, HttpWebProxyHost, HttpWebProxyPort, 
                      HttpClientIdentification, HttpServerIdentification, HttpResponseTimeOut, XmlClientRequestSenderId, XmlServerRequestSenderId, XmlNameSpaceRequest, 
                      XmlNameSpaceResponse, OmitXmlDeclaration, EvaluateDataReadyInStatusResponse, StatusRequestIntervalInSec, SubscriptionRetryIntervalInSec
FROM         dbo.ProtocolConfigurationSet
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
			([OperationId]
			, [Attribute]
			, [DateCreated])
			VALUES
			(@operationId
			, @attribute
			, @dateCreated)
			
			DECLARE @id int = SCOPE_IDENTITY()
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
CREATE NONCLUSTERED INDEX [IX_ItcsConfigurationName] ON [dbo].[ItcsConfigurationSet]
(
	[name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE VIEW [dbo].[InfoLineTextActivities]
AS
SELECT     Base.Id, Base.OperationId, Base.Attribute, Base.DateCreated, Base.DateModified, Derived.ItcsStationId, Derived.LineNumber, Derived.DestinationId, 
                      Derived.DisplayText, Derived.ExpirationTime, Derived.InfoRowId, Derived.Blink, Derived.DisplayedScreenSide, Derived.Alignment, Derived.Font
FROM         dbo.InfoLineTextActivitySet AS Derived INNER JOIN
                      dbo.ActivitySet AS Base ON Derived.Id = Base.Id
WHERE     (Base.IsDeleted = 0)
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
ALTER TABLE [dbo].[InfoLineTextActivitySet] ADD CONSTRAINT [FK_InfoLineTextActivitySet_ActivitySet] FOREIGN KEY
	(
		[Id]
	)
	REFERENCES [dbo].[ActivitySet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[InfoLineTextActivitySet] ADD CONSTRAINT [FK_InfoLineTextSet_StationSet] FOREIGN KEY
	(
		[ItcsStationId]
	)
	REFERENCES [dbo].[StationSet]
	(
		[Id]
	)
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.2.10'
  ,@description = 'Changes to meet naming conventions; updates activity and InfoLineTextActivity'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 2
  ,@versionRevision = 10
  ,@dateCreated = '2012-01-17T14:30:00.000'
GO

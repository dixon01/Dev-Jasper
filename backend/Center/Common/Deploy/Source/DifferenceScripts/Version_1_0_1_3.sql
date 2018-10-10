 

USE Gorba_CenterOnline
GO

CREATE TABLE [dbo].[ItcsConfiguration]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ItcsConfigurationId] [int] NULL,
	[name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[description] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[collectSystemData] [bit] NOT NULL CONSTRAINT [DF_ItcsConfiguration_collectSystemData] DEFAULT ((1)),
	[operationDayStartUtc] [datetime] NOT NULL,
	[operationDayDuration] [datetime] NOT NULL,
	[utcOffset] [datetime] NOT NULL,
	[dayLightSaving] [bit] NOT NULL CONSTRAINT [DF_ItcsConfiguration_dayLightSaving] DEFAULT ((0)),
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_ItcsConfiguration_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_ItcsConfiguration] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ItcsConfigurationName] ON [dbo].[ItcsConfiguration]
(
	[name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE TABLE [dbo].[ProtocolConfiguration]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ProtocolTypeId] [int] NOT NULL,
	[realTimeDataOnly] [bit] NULL,
	[httpListenerHost] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpListenerPort] [int] NULL,
	[httpServerHost] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpServerport] [int] NULL,
	[httpWebProxyHost] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpWebProxyPort] [int] NULL,
	[httpClientIdentification] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpServerIdentification] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpResponseTimeOut] [int] NULL,
	[xmlClientRequestSenderId] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[xmlServerRequestSenderId] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[xmlNameSpaceRequest] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[xmlNameSpaceResponse] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[omitXmlDeclaration] [bit] NULL,
	[evaluateDataReadyInStatusResponse] [bit] NULL,
	[statusRequestIntervalInSec] [int] NULL,
	[subscriptionRetryIntervalInSec] [int] NULL,
	CONSTRAINT [PK_ProtocolConfiguration] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProtocolConfiguration] ON [dbo].[ProtocolConfiguration]
(
	[Id] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE TABLE [dbo].[ProtocolType]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[description] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT [PK_ProtocolType] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE VIEW [dbo].[ProtocolTypes]
AS
SELECT     dbo.ProtocolType.*
FROM         dbo.ProtocolType
GO
CREATE VIEW [dbo].[ItcsConfigurations]
AS
SELECT     Id, ItcsConfigurationId, name, description, collectSystemData, operationDayStartUtc, operationDayDuration, utcOffset, dayLightSaving
FROM         dbo.ItcsConfiguration
WHERE     (IsDeleted = 0)
GO
ALTER TABLE [dbo].[ItcsConfiguration] ADD CONSTRAINT [FK_ItcsConfiguration_ProtocolConfiguration] FOREIGN KEY
	(
		[ItcsConfigurationId]
	)
	REFERENCES [dbo].[ProtocolConfiguration]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ProtocolConfiguration] ADD CONSTRAINT [FK_ProtocolConfiguration_ProtocolType] FOREIGN KEY
	(
		[ProtocolTypeId]
	)
	REFERENCES [dbo].[ProtocolType]
	(
		[Id]
	)
GO

-- =============================================
-- Author:		Jerome Coutant
-- Create date: 09-12-2011
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[ItcsConfiguration_Insert] 
	@ItcsConfigurationId int,
	@name varchar(50),
	@description varchar(500) = null,
	@collectSystemData bit = 1,
	@operationDayStartUtc datetime,
	@operationDayDuration datetime,
	@utcOffset datetime,
	@dayLightSaving bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
			BEGIN TRANSACTION ItcsConfiguration_InsertTx -- Start the transaction..
			-- Insert statements for procedure here
			INSERT INTO [Gorba_CenterOnline].[dbo].[ItcsConfiguration]
				   ([ItcsConfigurationId]
				   ,[name]
				   ,[description]
				   ,[collectSystemData]
				   ,[operationDayStartUtc]
				   ,[operationDayDuration]
				   ,[utcOffset]
				   ,[dayLightSaving])
			 VALUES
				   (@ItcsConfigurationId,
					@name,
					@description,
					@collectSystemData,
					@operationDayStartUtc,
					@operationDayDuration,
					@utcOffset,
					@dayLightSaving)
			
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
-- =============================================
-- Author:		Jerome Coutant
-- Create date: 09-12-2011
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[ProtocolType_Insert] 
	@name varchar(50),
	@description varchar(500) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION ProtocolType_InsertTx -- Start the transaction..
		-- Insert statements for procedure here		
			INSERT INTO [ProtocolType]
			([Name]
			, [Description])
			VALUES
			(@name
			, @description)
			
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
-- =============================================
-- Author:		Jerome Coutant
-- Create date: 09-12-2011
-- Description:	Insert new ProtocolConfiguration row with only needed vdv fields. 
--				Note : The different fieds are set or not according to the underlying used protocol.
-- =============================================
CREATE PROCEDURE [dbo].[VdvProtocolConfiguration_Insert] 
	-- Add the parameters for the stored procedure here
	@ProtocolTypeId int,
	@realTimeDataOnly bit = 0,
	@httpListenerHost varchar(50),
	@httpListenerPort int,
	@httpServerHost varchar(50),
	@httpServerport int,
	@httpWebProxyHost varchar(50) = null,
	@httpWebProxyPort int = 0,
	@httpClientIdentification varchar(50),
	@httpServerIdentification varchar(50),
	@httpResponseTimeOut int = 10,
	@xmlClientRequestSenderId varchar(50) = "client",
	@xmlServerRequestSenderId varchar(50) = "server",
	@xmlNameSpaceRequest varchar(50) = null,
	@xmlNameSpaceResponse varchar(50) = null,
	@omitXmlDeclaration bit = 1,
	@evaluateDataReadyInStatusResponse bit = 1,
	@statusRequestIntervalInSec int = 60,
	@subscriptionRetryIntervalInSec int = 60
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION VdvProtocolConfig_InsertTx -- Start the transaction.				
					
    -- Insert statements for procedure here
		INSERT INTO [Gorba_CenterOnline].[dbo].[ProtocolConfiguration]
			   ( [ProtocolTypeId]
			   , [realTimeDataOnly]
			   , [httpListenerHost]
			   , [httpListenerPort]
			   , [httpServerHost]
			   , [httpServerport]
			   , [httpWebProxyHost]
			   , [httpWebProxyPort]
			   , [httpClientIdentification]
			   , [httpServerIdentification]
			   , [httpResponseTimeOut]
			   , [xmlClientRequestSenderId]
			   , [xmlServerRequestSenderId]
			   , [xmlNameSpaceRequest]
			   , [xmlNameSpaceResponse]
			   , [omitXmlDeclaration]
			   , [evaluateDataReadyInStatusResponse]
			   , [statusRequestIntervalInSec]
			   , [subscriptionRetryIntervalInSec])
		 VALUES
			   ( @ProtocolTypeId,
				 @realTimeDataOnly,
				 @httpListenerHost,
				 @httpListenerPort,
				 @httpServerHost,
				 @httpServerport,
				 @httpWebProxyHost,
				 @httpWebProxyPort,
				 @httpClientIdentification,
				 @httpServerIdentification,
				 @httpResponseTimeOut,
				 @xmlClientRequestSenderId,
				 @xmlServerRequestSenderId,
				 @xmlNameSpaceRequest,
				 @xmlNameSpaceResponse,
				 @omitXmlDeclaration,
				 @evaluateDataReadyInStatusResponse,
				 @statusRequestIntervalInSec,
				 @subscriptionRetryIntervalInSec)
			
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

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.1.3'
  ,@description = 'Added tables, views and stored procedures for itcs clientµ. Task id 413'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 1
  ,@versionRevision = 3
  ,@dateCreated = '2011-12-12T15:00:00.000'
GO

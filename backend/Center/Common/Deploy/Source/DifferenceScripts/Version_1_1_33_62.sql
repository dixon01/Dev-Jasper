 

USE [Gorba_CenterOnline]
GO

CREATE UNIQUE NONCLUSTERED INDEX [ItcsTextMappingSetUniqueKey] ON [dbo].[ItcsTextMappingSet]
(
	[ItcsProviderId] ASC,
	[ProductTypeId] ASC,
	[SourceText] ASC,
	[Type] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ClearTextMappingData]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM ItcsTextMappingSet
END
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.1.33.62'
  ,@description = 'Add unique index for text mapping set.'
  ,@versionMajor = 1
  ,@versionMinor = 1
  ,@versionBuild = 33
  ,@versionRevision = 62
  ,@dateCreated = '2013-03-25T15:00:00.000'
GO
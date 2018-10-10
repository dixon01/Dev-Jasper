USE [master]
GO
/****** Object:  Database [Gorba_CenterControllers]    Script Date: 12/02/2011 11:11:16 ******/
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'Gorba_CenterControllers')
	BEGIN
		ALTER DATABASE [Gorba_CenterControllers] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	END
GO
/****** Object:  Database [Gorba_CenterControllers]    Script Date: 12/02/2011 11:11:16 ******/
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'Gorba_CenterControllers')
	BEGIN
		DROP DATABASE [Gorba_CenterControllers]
	END
GO



USE [master]
GO
/****** Object:  Database [Gorba_CenterOnline]    Script Date: 12/02/2011 11:11:16 ******/
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'Gorba_CenterOnline')
	BEGIN
		ALTER DATABASE [Gorba_CenterOnline] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	END
GO
/****** Object:  Database [Gorba_CenterOnline]    Script Date: 12/02/2011 11:11:16 ******/
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'Gorba_CenterOnline')
	BEGIN
		DROP DATABASE [Gorba_CenterOnline]
	END
GO



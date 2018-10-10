BackgroundSystem solution requires (following sequence is the only one tested):
- Visual Studio 2013
- Gorba Visual Studio Extension (H:\35_DEVELOPMENT\Internal\Team Software\Infrastructure\Development Toolchain\Gorba Visual Studio Extension\Gorba.Common.VisualStudio.VisualStudioExtension.vsix)
- Install Sql Server 2014 and enable Pipe binding (Windows Start menu / Sql Server 2014 Configuration Manager / Sql server network configuration / Protocols for MSSQLSERVER / Named Pipes -> Enabled)
- install (Azure Pack) ServiceBus from Web Platform Installer (http://www.microsoft.com/web/downloads/platform.aspx). Packages:
    - Windows Azure Pack: Service Bus 1.1
	- Windows Azure Pack: Security Update for Service Bus 1.1
	- Windows Azure SDK 2.1
- Run Service Bus configuration (Windows Start menu / Service Bus Configuration)
- Reconfigure Service Bus to use Center shared key
    - Run Service Bus PowerShell (Windows Start menu / Service Bus PowerShell)
	- Type the following command:
	    New-SBAuthorizationRule -Namespace ServiceBusDefaultNamespace -Name CenterSharedAccessKey -Rights Listen,Manage,Send -PrimaryKey I10aA2qzGGwJn2xfSKlf0gslt7scFRsED5dmHh0hRbk=
- Install Azure Sdk 2.4.1 from Web Platform Installer. Packages:
    - Microsoft Azure SDK for .NET (VS2013) 2.4
	- Microsoft Azure SDK 2.4.1
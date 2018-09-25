namespace Gorba.Center.BackgroundSystem.PowerShell
{
	using System;
	using System.Collections.Generic;
	using System.Management.Automation;
	using System.ServiceModel;
	using System.Xml;

	using Gorba.Center.Common.ServiceModel;
	using Gorba.Center.Common.ServiceModel.AccessControl;
	using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
	using Gorba.Center.Common.ServiceModel.Configurations;
	using Gorba.Center.Common.ServiceModel.Filters.Configurations;
	using Gorba.Center.Common.ServiceModel.Documents;
	using Gorba.Center.Common.ServiceModel.Filters.Documents;
	using Gorba.Center.Common.ServiceModel.Log;
	using Gorba.Center.Common.ServiceModel.Filters.Log;
	using Gorba.Center.Common.ServiceModel.Membership;
	using Gorba.Center.Common.ServiceModel.Filters.Membership;
	using Gorba.Center.Common.ServiceModel.Meta;
	using Gorba.Center.Common.ServiceModel.Filters.Meta;
	using Gorba.Center.Common.ServiceModel.Resources;
	using Gorba.Center.Common.ServiceModel.Filters.Resources;
	using Gorba.Center.Common.ServiceModel.Software;
	using Gorba.Center.Common.ServiceModel.Filters.Software;
	using Gorba.Center.Common.ServiceModel.Units;
	using Gorba.Center.Common.ServiceModel.Filters.Units;
	using Gorba.Center.Common.ServiceModel.Update;
	using Gorba.Center.Common.ServiceModel.Filters.Update;

	internal static partial class CmdletNouns
	{		
		public const string UserRole = "UserRole";
		
		public const string UserRoleFilter = "UserRoleFilter";
		
		public const string UserRoleQuery = "UserRoleQuery";
		
		public const string UserRoleName = "UserRoleName";
		
		public const string UserRoleDescription = "UserRoleDescription";
		
		public const string Authorization = "Authorization";
		
		public const string AuthorizationFilter = "AuthorizationFilter";
		
		public const string AuthorizationQuery = "AuthorizationQuery";
		
		public const string UnitConfiguration = "UnitConfiguration";
		
		public const string UnitConfigurationFilter = "UnitConfigurationFilter";
		
		public const string UnitConfigurationQuery = "UnitConfigurationQuery";
		
		public const string MediaConfiguration = "MediaConfiguration";
		
		public const string MediaConfigurationFilter = "MediaConfigurationFilter";
		
		public const string MediaConfigurationQuery = "MediaConfigurationQuery";
		
		public const string Document = "Document";
		
		public const string DocumentFilter = "DocumentFilter";
		
		public const string DocumentQuery = "DocumentQuery";
		
		public const string DocumentName = "DocumentName";
		
		public const string DocumentDescription = "DocumentDescription";
		
		public const string DocumentVersion = "DocumentVersion";
		
		public const string DocumentVersionFilter = "DocumentVersionFilter";
		
		public const string DocumentVersionQuery = "DocumentVersionQuery";
		
		public const string DocumentVersionMajor = "DocumentVersionMajor";
		
		public const string DocumentVersionMinor = "DocumentVersionMinor";
		
		public const string DocumentVersionDescription = "DocumentVersionDescription";
		
		public const string LogEntry = "LogEntry";
		
		public const string LogEntryFilter = "LogEntryFilter";
		
		public const string LogEntryQuery = "LogEntryQuery";
		
		public const string LogEntryApplication = "LogEntryApplication";
		
		public const string LogEntryTimestamp = "LogEntryTimestamp";
		
		public const string LogEntryLogger = "LogEntryLogger";
		
		public const string LogEntryMessage = "LogEntryMessage";
		
		public const string LogEntryAdditionalData = "LogEntryAdditionalData";
		
		public const string Tenant = "Tenant";
		
		public const string TenantFilter = "TenantFilter";
		
		public const string TenantQuery = "TenantQuery";
		
		public const string TenantName = "TenantName";
		
		public const string TenantDescription = "TenantDescription";
		
		public const string User = "User";
		
		public const string UserFilter = "UserFilter";
		
		public const string UserQuery = "UserQuery";
		
		public const string UserUsername = "UserUsername";
		
		public const string UserDomain = "UserDomain";
		
		public const string UserHashedPassword = "UserHashedPassword";
		
		public const string UserFirstName = "UserFirstName";
		
		public const string UserLastName = "UserLastName";
		
		public const string UserEmail = "UserEmail";
		
		public const string UserCulture = "UserCulture";
		
		public const string UserTimeZone = "UserTimeZone";
		
		public const string UserDescription = "UserDescription";
		
		public const string UserLastLoginAttempt = "UserLastLoginAttempt";
		
		public const string UserLastSuccessfulLogin = "UserLastSuccessfulLogin";
		
		public const string UserConsecutiveLoginFailures = "UserConsecutiveLoginFailures";
		
		public const string UserIsEnabled = "UserIsEnabled";
		
		public const string AssociationTenantUserUserRole = "AssociationTenantUserUserRole";
		
		public const string AssociationTenantUserUserRoleFilter = "AssociationTenantUserUserRoleFilter";
		
		public const string AssociationTenantUserUserRoleQuery = "AssociationTenantUserUserRoleQuery";
		
		public const string UserDefinedProperty = "UserDefinedProperty";
		
		public const string UserDefinedPropertyFilter = "UserDefinedPropertyFilter";
		
		public const string UserDefinedPropertyQuery = "UserDefinedPropertyQuery";
		
		public const string UserDefinedPropertyName = "UserDefinedPropertyName";
		
		public const string SystemConfig = "SystemConfig";
		
		public const string SystemConfigFilter = "SystemConfigFilter";
		
		public const string SystemConfigQuery = "SystemConfigQuery";
		
		public const string SystemConfigSystemId = "SystemConfigSystemId";
		
		public const string Resource = "Resource";
		
		public const string ResourceFilter = "ResourceFilter";
		
		public const string ResourceQuery = "ResourceQuery";
		
		public const string ResourceOriginalFilename = "ResourceOriginalFilename";
		
		public const string ResourceDescription = "ResourceDescription";
		
		public const string ResourceHash = "ResourceHash";
		
		public const string ResourceThumbnailHash = "ResourceThumbnailHash";
		
		public const string ResourceMimeType = "ResourceMimeType";
		
		public const string ResourceLength = "ResourceLength";
		
		public const string ContentResource = "ContentResource";
		
		public const string ContentResourceFilter = "ContentResourceFilter";
		
		public const string ContentResourceQuery = "ContentResourceQuery";
		
		public const string ContentResourceOriginalFilename = "ContentResourceOriginalFilename";
		
		public const string ContentResourceDescription = "ContentResourceDescription";
		
		public const string ContentResourceThumbnailHash = "ContentResourceThumbnailHash";
		
		public const string ContentResourceHash = "ContentResourceHash";
		
		public const string ContentResourceMimeType = "ContentResourceMimeType";
		
		public const string ContentResourceLength = "ContentResourceLength";
		
		public const string Package = "Package";
		
		public const string PackageFilter = "PackageFilter";
		
		public const string PackageQuery = "PackageQuery";
		
		public const string PackagePackageId = "PackagePackageId";
		
		public const string PackageProductName = "PackageProductName";
		
		public const string PackageDescription = "PackageDescription";
		
		public const string PackageVersion = "PackageVersion";
		
		public const string PackageVersionFilter = "PackageVersionFilter";
		
		public const string PackageVersionQuery = "PackageVersionQuery";
		
		public const string PackageVersionSoftwareVersion = "PackageVersionSoftwareVersion";
		
		public const string PackageVersionDescription = "PackageVersionDescription";
		
		public const string ProductType = "ProductType";
		
		public const string ProductTypeFilter = "ProductTypeFilter";
		
		public const string ProductTypeQuery = "ProductTypeQuery";
		
		public const string ProductTypeName = "ProductTypeName";
		
		public const string ProductTypeDescription = "ProductTypeDescription";
		
		public const string Unit = "Unit";
		
		public const string UnitFilter = "UnitFilter";
		
		public const string UnitQuery = "UnitQuery";
		
		public const string UnitName = "UnitName";
		
		public const string UnitNetworkAddress = "UnitNetworkAddress";
		
		public const string UnitDescription = "UnitDescription";
		
		public const string UnitIsConnected = "UnitIsConnected";
		
		public const string UpdateGroup = "UpdateGroup";
		
		public const string UpdateGroupFilter = "UpdateGroupFilter";
		
		public const string UpdateGroupQuery = "UpdateGroupQuery";
		
		public const string UpdateGroupName = "UpdateGroupName";
		
		public const string UpdateGroupDescription = "UpdateGroupDescription";
		
		public const string UpdatePart = "UpdatePart";
		
		public const string UpdatePartFilter = "UpdatePartFilter";
		
		public const string UpdatePartQuery = "UpdatePartQuery";
		
		public const string UpdatePartStart = "UpdatePartStart";
		
		public const string UpdatePartEnd = "UpdatePartEnd";
		
		public const string UpdatePartDescription = "UpdatePartDescription";
		
		public const string UpdateCommand = "UpdateCommand";
		
		public const string UpdateCommandFilter = "UpdateCommandFilter";
		
		public const string UpdateCommandQuery = "UpdateCommandQuery";
		
		public const string UpdateCommandUpdateIndex = "UpdateCommandUpdateIndex";
		
		public const string UpdateCommandWasTransferred = "UpdateCommandWasTransferred";
		
		public const string UpdateCommandWasInstalled = "UpdateCommandWasInstalled";
		
		public const string UpdateFeedback = "UpdateFeedback";
		
		public const string UpdateFeedbackFilter = "UpdateFeedbackFilter";
		
		public const string UpdateFeedbackQuery = "UpdateFeedbackQuery";
		
		public const string UpdateFeedbackTimestamp = "UpdateFeedbackTimestamp";
	}


	public partial class UserRoleCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "UserRole"; } }
	}

	public partial class UserRoleCmdletBaseWithInputObject : UserRoleCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public UserRole InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UserRole)]
	public partial class NewUserRole : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new UserRole());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.UserRole)]
	public partial class AddUserRole : UserRoleCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUserRoleDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.UserRole, DefaultParameterSetName = "Filter")]
	public partial class GetUserRole : UserRoleCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public UserRoleQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IUserRoleDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IUserRoleDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'UserRole' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.UserRole)]
	public partial class RemoveUserRole : UserRoleCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IUserRoleDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.UserRole)]
	public partial class UpdateUserRole : UserRoleCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUserRoleDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UserRoleFilter)]
	public partial class NewUserRoleFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UserRoleFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UserRoleQuery)]
	public partial class NewUserRoleQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UserRoleQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserRoleName)]
	public partial class SelectUserRoleName : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserRoleFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithName(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserRoleDescription)]
	public partial class SelectUserRoleDescription : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserRoleFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithDescription(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class AuthorizationCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "Authorization"; } }
	}

	public partial class AuthorizationCmdletBaseWithInputObject : AuthorizationCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public Authorization InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.Authorization)]
	public partial class NewAuthorization : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new Authorization());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.Authorization)]
	public partial class AddAuthorization : AuthorizationCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IAuthorizationDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.Authorization, DefaultParameterSetName = "Filter")]
	public partial class GetAuthorization : AuthorizationCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public AuthorizationQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IAuthorizationDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IAuthorizationDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'Authorization' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.Authorization)]
	public partial class RemoveAuthorization : AuthorizationCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IAuthorizationDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.Authorization)]
	public partial class UpdateAuthorization : AuthorizationCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IAuthorizationDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.AuthorizationFilter)]
	public partial class NewAuthorizationFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(AuthorizationFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.AuthorizationQuery)]
	public partial class NewAuthorizationQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(AuthorizationQuery.Create());
		}
	}


	public partial class UnitConfigurationCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "UnitConfiguration"; } }
	}

	public partial class UnitConfigurationCmdletBaseWithInputObject : UnitConfigurationCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public UnitConfiguration InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UnitConfiguration)]
	public partial class NewUnitConfiguration : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new UnitConfiguration());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.UnitConfiguration)]
	public partial class AddUnitConfiguration : UnitConfigurationCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUnitConfigurationDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.UnitConfiguration, DefaultParameterSetName = "Filter")]
	public partial class GetUnitConfiguration : UnitConfigurationCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public UnitConfigurationQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IUnitConfigurationDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IUnitConfigurationDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'UnitConfiguration' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.UnitConfiguration)]
	public partial class RemoveUnitConfiguration : UnitConfigurationCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IUnitConfigurationDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.UnitConfiguration)]
	public partial class UpdateUnitConfiguration : UnitConfigurationCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUnitConfigurationDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UnitConfigurationFilter)]
	public partial class NewUnitConfigurationFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UnitConfigurationFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UnitConfigurationQuery)]
	public partial class NewUnitConfigurationQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UnitConfigurationQuery.Create());
		}
	}


	public partial class MediaConfigurationCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "MediaConfiguration"; } }
	}

	public partial class MediaConfigurationCmdletBaseWithInputObject : MediaConfigurationCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public MediaConfiguration InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.MediaConfiguration)]
	public partial class NewMediaConfiguration : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new MediaConfiguration());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.MediaConfiguration)]
	public partial class AddMediaConfiguration : MediaConfigurationCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IMediaConfigurationDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.MediaConfiguration, DefaultParameterSetName = "Filter")]
	public partial class GetMediaConfiguration : MediaConfigurationCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public MediaConfigurationQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IMediaConfigurationDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IMediaConfigurationDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'MediaConfiguration' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.MediaConfiguration)]
	public partial class RemoveMediaConfiguration : MediaConfigurationCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IMediaConfigurationDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.MediaConfiguration)]
	public partial class UpdateMediaConfiguration : MediaConfigurationCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IMediaConfigurationDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.MediaConfigurationFilter)]
	public partial class NewMediaConfigurationFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(MediaConfigurationFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.MediaConfigurationQuery)]
	public partial class NewMediaConfigurationQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(MediaConfigurationQuery.Create());
		}
	}


	public partial class DocumentCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "Document"; } }
	}

	public partial class DocumentCmdletBaseWithInputObject : DocumentCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public Document InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.Document)]
	public partial class NewDocument : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new Document());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.Document)]
	public partial class AddDocument : DocumentCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IDocumentDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.Document, DefaultParameterSetName = "Filter")]
	public partial class GetDocument : DocumentCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public DocumentQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IDocumentDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IDocumentDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'Document' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.Document)]
	public partial class RemoveDocument : DocumentCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IDocumentDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.Document)]
	public partial class UpdateDocument : DocumentCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IDocumentDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.DocumentFilter)]
	public partial class NewDocumentFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(DocumentFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.DocumentQuery)]
	public partial class NewDocumentQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(DocumentQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.DocumentName)]
	public partial class SelectDocumentName : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public DocumentFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithName(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.DocumentDescription)]
	public partial class SelectDocumentDescription : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public DocumentFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithDescription(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class DocumentVersionCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "DocumentVersion"; } }
	}

	public partial class DocumentVersionCmdletBaseWithInputObject : DocumentVersionCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public DocumentVersion InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.DocumentVersion)]
	public partial class NewDocumentVersion : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new DocumentVersion());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.DocumentVersion)]
	public partial class AddDocumentVersion : DocumentVersionCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IDocumentVersionDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.DocumentVersion, DefaultParameterSetName = "Filter")]
	public partial class GetDocumentVersion : DocumentVersionCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public DocumentVersionQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IDocumentVersionDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IDocumentVersionDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'DocumentVersion' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.DocumentVersion)]
	public partial class RemoveDocumentVersion : DocumentVersionCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IDocumentVersionDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.DocumentVersion)]
	public partial class UpdateDocumentVersion : DocumentVersionCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IDocumentVersionDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.DocumentVersionFilter)]
	public partial class NewDocumentVersionFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(DocumentVersionFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.DocumentVersionQuery)]
	public partial class NewDocumentVersionQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(DocumentVersionQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.DocumentVersionMajor)]
	public partial class SelectDocumentVersionMajor : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public DocumentVersionFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public int Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithMajor(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.DocumentVersionMinor)]
	public partial class SelectDocumentVersionMinor : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public DocumentVersionFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public int Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithMinor(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.DocumentVersionDescription)]
	public partial class SelectDocumentVersionDescription : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public DocumentVersionFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithDescription(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class LogEntryCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "LogEntry"; } }
	}

	public partial class LogEntryCmdletBaseWithInputObject : LogEntryCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public LogEntry InputObject { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Multiple", Position = 3, ValueFromPipeline = true)]
		public IEnumerable<LogEntry> InputObjects { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.LogEntry)]
	public partial class NewLogEntry : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new LogEntry());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.LogEntry)]
	public partial class AddLogEntry : LogEntryCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<ILogEntryDataService>())
			{
				if (this.ParameterSetName == "Multiple")
				{
					channelScope.Channel.AddRangeAsync(this.InputObjects).Wait();
					return;
				}

				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.LogEntry, DefaultParameterSetName = "Filter")]
	public partial class GetLogEntry : LogEntryCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public LogEntryQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<ILogEntryDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<ILogEntryDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'LogEntry' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.LogEntry)]
	public partial class RemoveLogEntry : LogEntryCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<ILogEntryDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.LogEntry)]
	public partial class UpdateLogEntry : LogEntryCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<ILogEntryDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.LogEntryFilter)]
	public partial class NewLogEntryFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(LogEntryFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.LogEntryQuery)]
	public partial class NewLogEntryQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(LogEntryQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.LogEntryApplication)]
	public partial class SelectLogEntryApplication : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public LogEntryFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithApplication(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.LogEntryTimestamp)]
	public partial class SelectLogEntryTimestamp : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public LogEntryFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public DateTime Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithTimestamp(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.LogEntryLogger)]
	public partial class SelectLogEntryLogger : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public LogEntryFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithLogger(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.LogEntryMessage)]
	public partial class SelectLogEntryMessage : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public LogEntryFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithMessage(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.LogEntryAdditionalData)]
	public partial class SelectLogEntryAdditionalData : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public LogEntryFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithAdditionalData(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class TenantCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "Tenant"; } }
	}

	public partial class TenantCmdletBaseWithInputObject : TenantCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public Tenant InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.Tenant)]
	public partial class NewTenant : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new Tenant());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.Tenant)]
	public partial class AddTenant : TenantCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<ITenantDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.Tenant, DefaultParameterSetName = "Filter")]
	public partial class GetTenant : TenantCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public TenantQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<ITenantDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<ITenantDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'Tenant' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.Tenant)]
	public partial class RemoveTenant : TenantCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<ITenantDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.Tenant)]
	public partial class UpdateTenant : TenantCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<ITenantDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.TenantFilter)]
	public partial class NewTenantFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(TenantFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.TenantQuery)]
	public partial class NewTenantQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(TenantQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.TenantName)]
	public partial class SelectTenantName : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public TenantFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithName(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.TenantDescription)]
	public partial class SelectTenantDescription : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public TenantFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithDescription(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class UserCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "User"; } }
	}

	public partial class UserCmdletBaseWithInputObject : UserCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public User InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.User)]
	public partial class NewUser : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new User());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.User)]
	public partial class AddUser : UserCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUserDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.User, DefaultParameterSetName = "Filter")]
	public partial class GetUser : UserCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public UserQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IUserDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IUserDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'User' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.User)]
	public partial class RemoveUser : UserCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IUserDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.User)]
	public partial class UpdateUser : UserCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUserDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UserFilter)]
	public partial class NewUserFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UserFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UserQuery)]
	public partial class NewUserQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UserQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserUsername)]
	public partial class SelectUserUsername : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithUsername(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserDomain)]
	public partial class SelectUserDomain : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithDomain(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserHashedPassword)]
	public partial class SelectUserHashedPassword : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithHashedPassword(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserFirstName)]
	public partial class SelectUserFirstName : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithFirstName(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserLastName)]
	public partial class SelectUserLastName : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithLastName(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserEmail)]
	public partial class SelectUserEmail : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithEmail(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserCulture)]
	public partial class SelectUserCulture : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithCulture(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserTimeZone)]
	public partial class SelectUserTimeZone : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithTimeZone(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserDescription)]
	public partial class SelectUserDescription : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithDescription(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserLastLoginAttempt)]
	public partial class SelectUserLastLoginAttempt : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public DateTime? Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithLastLoginAttempt(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserLastSuccessfulLogin)]
	public partial class SelectUserLastSuccessfulLogin : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public DateTime? Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithLastSuccessfulLogin(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserConsecutiveLoginFailures)]
	public partial class SelectUserConsecutiveLoginFailures : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public int Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithConsecutiveLoginFailures(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserIsEnabled)]
	public partial class SelectUserIsEnabled : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public bool Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithIsEnabled(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class AssociationTenantUserUserRoleCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "AssociationTenantUserUserRole"; } }
	}

	public partial class AssociationTenantUserUserRoleCmdletBaseWithInputObject : AssociationTenantUserUserRoleCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public AssociationTenantUserUserRole InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.AssociationTenantUserUserRole)]
	public partial class NewAssociationTenantUserUserRole : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new AssociationTenantUserUserRole());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.AssociationTenantUserUserRole)]
	public partial class AddAssociationTenantUserUserRole : AssociationTenantUserUserRoleCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IAssociationTenantUserUserRoleDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.AssociationTenantUserUserRole, DefaultParameterSetName = "Filter")]
	public partial class GetAssociationTenantUserUserRole : AssociationTenantUserUserRoleCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public AssociationTenantUserUserRoleQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IAssociationTenantUserUserRoleDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IAssociationTenantUserUserRoleDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'AssociationTenantUserUserRole' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.AssociationTenantUserUserRole)]
	public partial class RemoveAssociationTenantUserUserRole : AssociationTenantUserUserRoleCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IAssociationTenantUserUserRoleDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.AssociationTenantUserUserRole)]
	public partial class UpdateAssociationTenantUserUserRole : AssociationTenantUserUserRoleCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IAssociationTenantUserUserRoleDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.AssociationTenantUserUserRoleFilter)]
	public partial class NewAssociationTenantUserUserRoleFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(AssociationTenantUserUserRoleFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.AssociationTenantUserUserRoleQuery)]
	public partial class NewAssociationTenantUserUserRoleQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(AssociationTenantUserUserRoleQuery.Create());
		}
	}


	public partial class UserDefinedPropertyCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "UserDefinedProperty"; } }
	}

	public partial class UserDefinedPropertyCmdletBaseWithInputObject : UserDefinedPropertyCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public UserDefinedProperty InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UserDefinedProperty)]
	public partial class NewUserDefinedProperty : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new UserDefinedProperty());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.UserDefinedProperty)]
	public partial class AddUserDefinedProperty : UserDefinedPropertyCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUserDefinedPropertyDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.UserDefinedProperty, DefaultParameterSetName = "Filter")]
	public partial class GetUserDefinedProperty : UserDefinedPropertyCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public UserDefinedPropertyQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IUserDefinedPropertyDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IUserDefinedPropertyDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'UserDefinedProperty' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.UserDefinedProperty)]
	public partial class RemoveUserDefinedProperty : UserDefinedPropertyCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IUserDefinedPropertyDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.UserDefinedProperty)]
	public partial class UpdateUserDefinedProperty : UserDefinedPropertyCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUserDefinedPropertyDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UserDefinedPropertyFilter)]
	public partial class NewUserDefinedPropertyFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UserDefinedPropertyFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UserDefinedPropertyQuery)]
	public partial class NewUserDefinedPropertyQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UserDefinedPropertyQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.UserDefinedPropertyName)]
	public partial class SelectUserDefinedPropertyName : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UserDefinedPropertyFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithName(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class SystemConfigCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "SystemConfig"; } }
	}

	public partial class SystemConfigCmdletBaseWithInputObject : SystemConfigCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public SystemConfig InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.SystemConfig)]
	public partial class NewSystemConfig : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new SystemConfig());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.SystemConfig)]
	public partial class AddSystemConfig : SystemConfigCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<ISystemConfigDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.SystemConfig, DefaultParameterSetName = "Filter")]
	public partial class GetSystemConfig : SystemConfigCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public SystemConfigQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<ISystemConfigDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<ISystemConfigDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'SystemConfig' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.SystemConfig)]
	public partial class RemoveSystemConfig : SystemConfigCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<ISystemConfigDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.SystemConfig)]
	public partial class UpdateSystemConfig : SystemConfigCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<ISystemConfigDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.SystemConfigFilter)]
	public partial class NewSystemConfigFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(SystemConfigFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.SystemConfigQuery)]
	public partial class NewSystemConfigQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(SystemConfigQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.SystemConfigSystemId)]
	public partial class SelectSystemConfigSystemId : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public SystemConfigFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public Guid Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithSystemId(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class ResourceCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "Resource"; } }
	}

	public partial class ResourceCmdletBaseWithInputObject : ResourceCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public Resource InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.Resource)]
	public partial class NewResource : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new Resource());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.Resource)]
	public partial class AddResource : ResourceCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IResourceDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.Resource, DefaultParameterSetName = "Filter")]
	public partial class GetResource : ResourceCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public ResourceQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IResourceDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IResourceDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'Resource' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.Resource)]
	public partial class RemoveResource : ResourceCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IResourceDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.Resource)]
	public partial class UpdateResource : ResourceCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IResourceDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.ResourceFilter)]
	public partial class NewResourceFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(ResourceFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.ResourceQuery)]
	public partial class NewResourceQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(ResourceQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.ResourceOriginalFilename)]
	public partial class SelectResourceOriginalFilename : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public ResourceFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithOriginalFilename(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.ResourceDescription)]
	public partial class SelectResourceDescription : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public ResourceFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithDescription(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.ResourceHash)]
	public partial class SelectResourceHash : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public ResourceFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithHash(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.ResourceThumbnailHash)]
	public partial class SelectResourceThumbnailHash : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public ResourceFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithThumbnailHash(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.ResourceMimeType)]
	public partial class SelectResourceMimeType : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public ResourceFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithMimeType(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.ResourceLength)]
	public partial class SelectResourceLength : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public ResourceFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public long Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithLength(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class ContentResourceCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "ContentResource"; } }
	}

	public partial class ContentResourceCmdletBaseWithInputObject : ContentResourceCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public ContentResource InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.ContentResource)]
	public partial class NewContentResource : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new ContentResource());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.ContentResource)]
	public partial class AddContentResource : ContentResourceCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IContentResourceDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.ContentResource, DefaultParameterSetName = "Filter")]
	public partial class GetContentResource : ContentResourceCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public ContentResourceQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IContentResourceDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IContentResourceDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'ContentResource' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.ContentResource)]
	public partial class RemoveContentResource : ContentResourceCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IContentResourceDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.ContentResource)]
	public partial class UpdateContentResource : ContentResourceCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IContentResourceDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.ContentResourceFilter)]
	public partial class NewContentResourceFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(ContentResourceFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.ContentResourceQuery)]
	public partial class NewContentResourceQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(ContentResourceQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.ContentResourceOriginalFilename)]
	public partial class SelectContentResourceOriginalFilename : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public ContentResourceFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithOriginalFilename(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.ContentResourceDescription)]
	public partial class SelectContentResourceDescription : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public ContentResourceFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithDescription(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.ContentResourceThumbnailHash)]
	public partial class SelectContentResourceThumbnailHash : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public ContentResourceFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithThumbnailHash(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.ContentResourceHash)]
	public partial class SelectContentResourceHash : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public ContentResourceFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithHash(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.ContentResourceMimeType)]
	public partial class SelectContentResourceMimeType : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public ContentResourceFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithMimeType(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.ContentResourceLength)]
	public partial class SelectContentResourceLength : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public ContentResourceFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public long Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithLength(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class PackageCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "Package"; } }
	}

	public partial class PackageCmdletBaseWithInputObject : PackageCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public Package InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.Package)]
	public partial class NewPackage : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new Package());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.Package)]
	public partial class AddPackage : PackageCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IPackageDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.Package, DefaultParameterSetName = "Filter")]
	public partial class GetPackage : PackageCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public PackageQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IPackageDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IPackageDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'Package' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.Package)]
	public partial class RemovePackage : PackageCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IPackageDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.Package)]
	public partial class UpdatePackage : PackageCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IPackageDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.PackageFilter)]
	public partial class NewPackageFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(PackageFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.PackageQuery)]
	public partial class NewPackageQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(PackageQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.PackagePackageId)]
	public partial class SelectPackagePackageId : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public PackageFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithPackageId(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.PackageProductName)]
	public partial class SelectPackageProductName : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public PackageFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithProductName(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.PackageDescription)]
	public partial class SelectPackageDescription : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public PackageFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithDescription(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class PackageVersionCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "PackageVersion"; } }
	}

	public partial class PackageVersionCmdletBaseWithInputObject : PackageVersionCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public PackageVersion InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.PackageVersion)]
	public partial class NewPackageVersion : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new PackageVersion());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.PackageVersion)]
	public partial class AddPackageVersion : PackageVersionCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IPackageVersionDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.PackageVersion, DefaultParameterSetName = "Filter")]
	public partial class GetPackageVersion : PackageVersionCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public PackageVersionQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IPackageVersionDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IPackageVersionDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'PackageVersion' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.PackageVersion)]
	public partial class RemovePackageVersion : PackageVersionCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IPackageVersionDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.PackageVersion)]
	public partial class UpdatePackageVersion : PackageVersionCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IPackageVersionDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.PackageVersionFilter)]
	public partial class NewPackageVersionFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(PackageVersionFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.PackageVersionQuery)]
	public partial class NewPackageVersionQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(PackageVersionQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.PackageVersionSoftwareVersion)]
	public partial class SelectPackageVersionSoftwareVersion : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public PackageVersionFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithSoftwareVersion(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.PackageVersionDescription)]
	public partial class SelectPackageVersionDescription : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public PackageVersionFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithDescription(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class ProductTypeCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "ProductType"; } }
	}

	public partial class ProductTypeCmdletBaseWithInputObject : ProductTypeCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public ProductType InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.ProductType)]
	public partial class NewProductType : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new ProductType());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.ProductType)]
	public partial class AddProductType : ProductTypeCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IProductTypeDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.ProductType, DefaultParameterSetName = "Filter")]
	public partial class GetProductType : ProductTypeCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public ProductTypeQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IProductTypeDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IProductTypeDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'ProductType' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.ProductType)]
	public partial class RemoveProductType : ProductTypeCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IProductTypeDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.ProductType)]
	public partial class UpdateProductType : ProductTypeCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IProductTypeDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.ProductTypeFilter)]
	public partial class NewProductTypeFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(ProductTypeFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.ProductTypeQuery)]
	public partial class NewProductTypeQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(ProductTypeQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.ProductTypeName)]
	public partial class SelectProductTypeName : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public ProductTypeFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithName(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.ProductTypeDescription)]
	public partial class SelectProductTypeDescription : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public ProductTypeFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithDescription(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class UnitCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "Unit"; } }
	}

	public partial class UnitCmdletBaseWithInputObject : UnitCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public Unit InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.Unit)]
	public partial class NewUnit : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new Unit());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.Unit)]
	public partial class AddUnit : UnitCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUnitDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.Unit, DefaultParameterSetName = "Filter")]
	public partial class GetUnit : UnitCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public UnitQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IUnitDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IUnitDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'Unit' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.Unit)]
	public partial class RemoveUnit : UnitCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IUnitDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.Unit)]
	public partial class UpdateUnit : UnitCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUnitDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UnitFilter)]
	public partial class NewUnitFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UnitFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UnitQuery)]
	public partial class NewUnitQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UnitQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.UnitName)]
	public partial class SelectUnitName : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UnitFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithName(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UnitNetworkAddress)]
	public partial class SelectUnitNetworkAddress : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UnitFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithNetworkAddress(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UnitDescription)]
	public partial class SelectUnitDescription : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UnitFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithDescription(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UnitIsConnected)]
	public partial class SelectUnitIsConnected : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UnitFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public bool Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithIsConnected(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class UpdateGroupCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "UpdateGroup"; } }
	}

	public partial class UpdateGroupCmdletBaseWithInputObject : UpdateGroupCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public UpdateGroup InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UpdateGroup)]
	public partial class NewUpdateGroup : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new UpdateGroup());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.UpdateGroup)]
	public partial class AddUpdateGroup : UpdateGroupCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUpdateGroupDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.UpdateGroup, DefaultParameterSetName = "Filter")]
	public partial class GetUpdateGroup : UpdateGroupCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public UpdateGroupQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IUpdateGroupDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IUpdateGroupDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'UpdateGroup' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.UpdateGroup)]
	public partial class RemoveUpdateGroup : UpdateGroupCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IUpdateGroupDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.UpdateGroup)]
	public partial class UpdateUpdateGroup : UpdateGroupCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUpdateGroupDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UpdateGroupFilter)]
	public partial class NewUpdateGroupFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UpdateGroupFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UpdateGroupQuery)]
	public partial class NewUpdateGroupQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UpdateGroupQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.UpdateGroupName)]
	public partial class SelectUpdateGroupName : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UpdateGroupFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithName(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UpdateGroupDescription)]
	public partial class SelectUpdateGroupDescription : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UpdateGroupFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithDescription(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class UpdatePartCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "UpdatePart"; } }
	}

	public partial class UpdatePartCmdletBaseWithInputObject : UpdatePartCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public UpdatePart InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UpdatePart)]
	public partial class NewUpdatePart : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new UpdatePart());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.UpdatePart)]
	public partial class AddUpdatePart : UpdatePartCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUpdatePartDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.UpdatePart, DefaultParameterSetName = "Filter")]
	public partial class GetUpdatePart : UpdatePartCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public UpdatePartQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IUpdatePartDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IUpdatePartDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'UpdatePart' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.UpdatePart)]
	public partial class RemoveUpdatePart : UpdatePartCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IUpdatePartDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.UpdatePart)]
	public partial class UpdateUpdatePart : UpdatePartCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUpdatePartDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UpdatePartFilter)]
	public partial class NewUpdatePartFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UpdatePartFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UpdatePartQuery)]
	public partial class NewUpdatePartQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UpdatePartQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.UpdatePartStart)]
	public partial class SelectUpdatePartStart : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UpdatePartFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public DateTime Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithStart(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UpdatePartEnd)]
	public partial class SelectUpdatePartEnd : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UpdatePartFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public DateTime Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithEnd(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UpdatePartDescription)]
	public partial class SelectUpdatePartDescription : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UpdatePartFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public string Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithDescription(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class UpdateCommandCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "UpdateCommand"; } }
	}

	public partial class UpdateCommandCmdletBaseWithInputObject : UpdateCommandCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public UpdateCommand InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UpdateCommand)]
	public partial class NewUpdateCommand : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new UpdateCommand());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.UpdateCommand)]
	public partial class AddUpdateCommand : UpdateCommandCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUpdateCommandDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.UpdateCommand, DefaultParameterSetName = "Filter")]
	public partial class GetUpdateCommand : UpdateCommandCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public UpdateCommandQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IUpdateCommandDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IUpdateCommandDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'UpdateCommand' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.UpdateCommand)]
	public partial class RemoveUpdateCommand : UpdateCommandCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IUpdateCommandDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.UpdateCommand)]
	public partial class UpdateUpdateCommand : UpdateCommandCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUpdateCommandDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UpdateCommandFilter)]
	public partial class NewUpdateCommandFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UpdateCommandFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UpdateCommandQuery)]
	public partial class NewUpdateCommandQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UpdateCommandQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.UpdateCommandUpdateIndex)]
	public partial class SelectUpdateCommandUpdateIndex : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UpdateCommandFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public int Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithUpdateIndex(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UpdateCommandWasTransferred)]
	public partial class SelectUpdateCommandWasTransferred : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UpdateCommandFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public bool Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithWasTransferred(this.Value);
			this.WriteObject(filter);
		}
	}

	[Cmdlet(VerbsCommon.Select, CmdletNouns.UpdateCommandWasInstalled)]
	public partial class SelectUpdateCommandWasInstalled : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UpdateCommandFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public bool Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithWasInstalled(this.Value);
			this.WriteObject(filter);
		}
	}

	public partial class UpdateFeedbackCmdletBase : DataServiceCmdletBase
	{
		public override string EntityName { get { return "UpdateFeedback"; } }
	}

	public partial class UpdateFeedbackCmdletBaseWithInputObject : UpdateFeedbackCmdletBase
	{
        [Parameter(Mandatory = true, ParameterSetName = "Single", Position = 3, ValueFromPipeline = true)]
		public UpdateFeedback InputObject { get; set; }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UpdateFeedback)]
	public partial class NewUpdateFeedback : PSCmdlet
	{
        protected override void ProcessRecord()
        {
            this.WriteObject(new UpdateFeedback());
        }
	}

	[Cmdlet(VerbsCommon.Add, CmdletNouns.UpdateFeedback)]
	public partial class AddUpdateFeedback : UpdateFeedbackCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUpdateFeedbackDataService>())
			{
				var result = channelScope.Channel.AddAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.Get, CmdletNouns.UpdateFeedback, DefaultParameterSetName = "Filter")]
	public partial class GetUpdateFeedback : UpdateFeedbackCmdletBase
	{
        [Parameter(ParameterSetName = "Id", Position = 3, ValueFromPipeline = true)]
		public int Id { get; set; }

        [Parameter(ParameterSetName = "Filter", Position = 3)]
		public UpdateFeedbackQuery Filter { get; set; }

        protected override void ProcessRecord()
        {
			switch (this.ParameterSetName)
			{
				case "Id":
					this.GetById();
					return;
				case "Filter":
					this.GetByFilter();
					return;
				default:
					throw new NotSupportedException();
			}
        }

		private void GetByFilter()
		{
			using (var channelScope = this.CreateDataChannelScope<IUpdateFeedbackDataService>())
			{
				var result = channelScope.Channel.QueryAsync(this.Filter).Result;
				this.WriteObject(result, true);
				return;
			}
		}

		private void GetById()
		{
			using (var channelScope = this.CreateDataChannelScope<IUpdateFeedbackDataService>())
			{
				this.WriteVerbose(string.Format("Getting 'UpdateFeedback' with Id '{0}'", this.Id));
				var result = channelScope.Channel.GetAsync(this.Id).Result;
				this.WriteObject(result);
				return;
			}
		}
	}

	[Cmdlet(VerbsCommon.Remove, CmdletNouns.UpdateFeedback)]
	public partial class RemoveUpdateFeedback : UpdateFeedbackCmdletBaseWithInputObject
	{
        partial void OnRemoving(ref bool shouldStop);

        protected override void ProcessRecord()
        {
            var shouldStop = false;
            this.OnRemoving(ref shouldStop);
            if (shouldStop)
            {
                return;
            }

			using (var channelScope = this.CreateDataChannelScope<IUpdateFeedbackDataService>())
			{
			    channelScope.Channel.DeleteAsync(this.InputObject).Wait();
			}
        }
	}

	[Cmdlet(VerbsData.Update, CmdletNouns.UpdateFeedback)]
	public partial class UpdateUpdateFeedback : UpdateFeedbackCmdletBaseWithInputObject
	{
        protected override void ProcessRecord()
        {
			using (var channelScope = this.CreateDataChannelScope<IUpdateFeedbackDataService>())
			{
				var result = channelScope.Channel.UpdateAsync(this.InputObject).Result;
				this.WriteObject(result);
			}
        }
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UpdateFeedbackFilter)]
	public partial class NewUpdateFeedbackFilter : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UpdateFeedbackFilter.Create());
		}
	}

	[Cmdlet(VerbsCommon.New, CmdletNouns.UpdateFeedbackQuery)]
	public partial class NewUpdateFeedbackQuery : PSCmdlet
	{
		protected override void ProcessRecord()
		{
			this.WriteObject(UpdateFeedbackQuery.Create());
		}
	}


	[Cmdlet(VerbsCommon.Select, CmdletNouns.UpdateFeedbackTimestamp)]
	public partial class SelectUpdateFeedbackTimestamp : PSCmdlet
	{
		[Parameter(Mandatory = true)]
		public UpdateFeedbackFilter Filter { get; set; }

		[Parameter(Mandatory = true)]
		public DateTime Value { get; set; }

		protected override void ProcessRecord()
		{
			var filter = this.Filter.WithTimestamp(this.Value);
			this.WriteObject(filter);
		}
	}
}
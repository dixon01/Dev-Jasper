namespace Gorba.Center.BackgroundSystem.PowerShell
{
    using System;
	using System.Linq;
    using System.Management.Automation;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Client.ChangeTracking;

    using Gorba.Center.Common.Client.ChangeTracking.AccessControl;
    using Gorba.Center.Common.Client.ChangeTracking.Membership;
    using Gorba.Center.Common.Client.ChangeTracking.Units;
    using Gorba.Center.Common.Client.ChangeTracking.Resources;
    using Gorba.Center.Common.Client.ChangeTracking.Update;
    using Gorba.Center.Common.Client.ChangeTracking.Documents;
    using Gorba.Center.Common.Client.ChangeTracking.Software;
    using Gorba.Center.Common.Client.ChangeTracking.Configurations;
    using Gorba.Center.Common.Client.ChangeTracking.Meta;

    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Software;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;

    using NLog;

	[Cmdlet(VerbsCommon.Get, "UserRoleReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetUserRoleReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UserRoleChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public UserRoleQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "AuthorizationReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetAuthorizationReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public AuthorizationChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public AuthorizationQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "TenantReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetTenantReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public TenantChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public TenantQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "UserReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetUserReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UserChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public UserQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "AssociationTenantUserUserRoleReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetAssociationTenantUserUserRoleReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public AssociationTenantUserUserRoleChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public AssociationTenantUserUserRoleQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "ProductTypeReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetProductTypeReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public ProductTypeChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public ProductTypeQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "UnitReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetUnitReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UnitChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public UnitQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "ResourceReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetResourceReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public ResourceChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public ResourceQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "UpdateGroupReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetUpdateGroupReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UpdateGroupChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public UpdateGroupQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "UpdatePartReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetUpdatePartReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UpdatePartChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public UpdatePartQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "UpdateCommandReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetUpdateCommandReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UpdateCommandChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public UpdateCommandQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "UpdateFeedbackReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetUpdateFeedbackReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UpdateFeedbackChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public UpdateFeedbackQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "DocumentReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetDocumentReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public DocumentChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public DocumentQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "DocumentVersionReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetDocumentVersionReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public DocumentVersionChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public DocumentVersionQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "PackageReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetPackageReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public PackageChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public PackageQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "PackageVersionReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetPackageVersionReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public PackageVersionChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public PackageVersionQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "UnitConfigurationReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetUnitConfigurationReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UnitConfigurationChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public UnitConfigurationQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "MediaConfigurationReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetMediaConfigurationReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public MediaConfigurationChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public MediaConfigurationQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "UserDefinedPropertyReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetUserDefinedPropertyReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UserDefinedPropertyChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public UserDefinedPropertyQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	[Cmdlet(VerbsCommon.Get, "SystemConfigReadableModel", DefaultParameterSetName = GetReadableModel.QueryParameterSet)]
	public class GetSystemConfigReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public SystemConfigChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.IdParameterSet)]
		public int Id { get; set; }

		[Parameter(ParameterSetName = GetReadableModel.QueryParameterSet)]
		public SystemConfigQuery Query { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case GetReadableModel.IdParameterSet:
                        await this.GetByIdAsync();
                        break;
                    case GetReadableModel.QueryParameterSet:
                        await this.QueryAsync();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }

	    private async Task QueryAsync()
	    {
	        var result = (await this.ChangeTrackingManager.QueryAsync()).ToList();
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            result.ToList().ForEach(async item => await item.LoadNavigationPropertiesAsync());
	        }

	        this.WriteObject(result, true);
	    }

	    private async Task GetByIdAsync()
	    {
	        var model = await this.ChangeTrackingManager.GetAsync(this.Id);
	        if (this.LoadReferenceProperties.ToBool())
	        {
	            await model.LoadReferencePropertiesAsync();
	        }

			this.WriteObject(model);
	    }
	}
	
}

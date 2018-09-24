namespace ConsoleApplication
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Web.Http;
	using Gorba.Center.Common.ServiceModel;

	using Gorba.Center.Common.ServiceModel.AccessControl;
	using Gorba.Center.Common.ServiceModel.Configurations;
	using Gorba.Center.Common.ServiceModel.Documents;
	using Gorba.Center.Common.ServiceModel.Membership;
	using Gorba.Center.Common.ServiceModel.Meta;
	using Gorba.Center.Common.ServiceModel.Resources;
	using Gorba.Center.Common.ServiceModel.Software;
	using Gorba.Center.Common.ServiceModel.Units;
	using Gorba.Center.Common.ServiceModel.Update;

	namespace AccessControl
	{
		public partial class AuthorizationsController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IAuthorizationDataService>();
				await dataService.DeleteAsync(new Authorization { Id = id });
			}

			public async Task<IEnumerable<Authorization>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IAuthorizationDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<Authorization> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IAuthorizationDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<Authorization> PostAsync([FromBody] Authorization entity)
			{
				var dataService = DependencyResolver.Current.Get<IAuthorizationDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<Authorization> PutAsync(int id, [FromBody] Authorization entity)
			{
				var dataService = DependencyResolver.Current.Get<IAuthorizationDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}

		public partial class UserRolesController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUserRoleDataService>();
				await dataService.DeleteAsync(new UserRole { Id = id });
			}

			public async Task<IEnumerable<UserRole>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IUserRoleDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<UserRole> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUserRoleDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<UserRole> PostAsync([FromBody] UserRole entity)
			{
				var dataService = DependencyResolver.Current.Get<IUserRoleDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<UserRole> PutAsync(int id, [FromBody] UserRole entity)
			{
				var dataService = DependencyResolver.Current.Get<IUserRoleDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}
	}

	namespace Configurations
	{
		public partial class UnitConfigurationsController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUnitConfigurationDataService>();
				await dataService.DeleteAsync(new UnitConfiguration { Id = id });
			}

			public async Task<IEnumerable<UnitConfiguration>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IUnitConfigurationDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<UnitConfiguration> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUnitConfigurationDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<UnitConfiguration> PostAsync([FromBody] UnitConfiguration entity)
			{
				var dataService = DependencyResolver.Current.Get<IUnitConfigurationDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<UnitConfiguration> PutAsync(int id, [FromBody] UnitConfiguration entity)
			{
				var dataService = DependencyResolver.Current.Get<IUnitConfigurationDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}

		public partial class MediaConfigurationsController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IMediaConfigurationDataService>();
				await dataService.DeleteAsync(new MediaConfiguration { Id = id });
			}

			public async Task<IEnumerable<MediaConfiguration>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IMediaConfigurationDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<MediaConfiguration> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IMediaConfigurationDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<MediaConfiguration> PostAsync([FromBody] MediaConfiguration entity)
			{
				var dataService = DependencyResolver.Current.Get<IMediaConfigurationDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<MediaConfiguration> PutAsync(int id, [FromBody] MediaConfiguration entity)
			{
				var dataService = DependencyResolver.Current.Get<IMediaConfigurationDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}
	}

	namespace Documents
	{
		public partial class DocumentsController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IDocumentDataService>();
				await dataService.DeleteAsync(new Document { Id = id });
			}

			public async Task<IEnumerable<Document>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IDocumentDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<Document> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IDocumentDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<Document> PostAsync([FromBody] Document entity)
			{
				var dataService = DependencyResolver.Current.Get<IDocumentDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<Document> PutAsync(int id, [FromBody] Document entity)
			{
				var dataService = DependencyResolver.Current.Get<IDocumentDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}

		public partial class DocumentVersionsController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IDocumentVersionDataService>();
				await dataService.DeleteAsync(new DocumentVersion { Id = id });
			}

			public async Task<IEnumerable<DocumentVersion>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IDocumentVersionDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<DocumentVersion> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IDocumentVersionDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<DocumentVersion> PostAsync([FromBody] DocumentVersion entity)
			{
				var dataService = DependencyResolver.Current.Get<IDocumentVersionDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<DocumentVersion> PutAsync(int id, [FromBody] DocumentVersion entity)
			{
				var dataService = DependencyResolver.Current.Get<IDocumentVersionDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}
	}

	namespace Membership
	{
		public partial class TenantsController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<ITenantDataService>();
				await dataService.DeleteAsync(new Tenant { Id = id });
			}

			public async Task<IEnumerable<Tenant>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<ITenantDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<Tenant> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<ITenantDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<Tenant> PostAsync([FromBody] Tenant entity)
			{
				var dataService = DependencyResolver.Current.Get<ITenantDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<Tenant> PutAsync(int id, [FromBody] Tenant entity)
			{
				var dataService = DependencyResolver.Current.Get<ITenantDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}

		public partial class UsersController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUserDataService>();
				await dataService.DeleteAsync(new User { Id = id });
			}

			public async Task<IEnumerable<User>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IUserDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<User> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUserDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<User> PostAsync([FromBody] User entity)
			{
				var dataService = DependencyResolver.Current.Get<IUserDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<User> PutAsync(int id, [FromBody] User entity)
			{
				var dataService = DependencyResolver.Current.Get<IUserDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}

		public partial class AssociationTenantUserUserRolesController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IAssociationTenantUserUserRoleDataService>();
				await dataService.DeleteAsync(new AssociationTenantUserUserRole { Id = id });
			}

			public async Task<IEnumerable<AssociationTenantUserUserRole>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IAssociationTenantUserUserRoleDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<AssociationTenantUserUserRole> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IAssociationTenantUserUserRoleDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<AssociationTenantUserUserRole> PostAsync([FromBody] AssociationTenantUserUserRole entity)
			{
				var dataService = DependencyResolver.Current.Get<IAssociationTenantUserUserRoleDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<AssociationTenantUserUserRole> PutAsync(int id, [FromBody] AssociationTenantUserUserRole entity)
			{
				var dataService = DependencyResolver.Current.Get<IAssociationTenantUserUserRoleDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}
	}

	namespace Meta
	{
		public partial class UserDefinedPropertiesController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUserDefinedPropertyDataService>();
				await dataService.DeleteAsync(new UserDefinedProperty { Id = id });
			}

			public async Task<IEnumerable<UserDefinedProperty>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IUserDefinedPropertyDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<UserDefinedProperty> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUserDefinedPropertyDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<UserDefinedProperty> PostAsync([FromBody] UserDefinedProperty entity)
			{
				var dataService = DependencyResolver.Current.Get<IUserDefinedPropertyDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<UserDefinedProperty> PutAsync(int id, [FromBody] UserDefinedProperty entity)
			{
				var dataService = DependencyResolver.Current.Get<IUserDefinedPropertyDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}
	}

	namespace Resources
	{
		public partial class ResourcesController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IResourceDataService>();
				await dataService.DeleteAsync(new Resource { Id = id });
			}

			public async Task<IEnumerable<Resource>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IResourceDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<Resource> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IResourceDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<Resource> PostAsync([FromBody] Resource entity)
			{
				var dataService = DependencyResolver.Current.Get<IResourceDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<Resource> PutAsync(int id, [FromBody] Resource entity)
			{
				var dataService = DependencyResolver.Current.Get<IResourceDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}
	}

	namespace Software
	{
		public partial class PackagesController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IPackageDataService>();
				await dataService.DeleteAsync(new Package { Id = id });
			}

			public async Task<IEnumerable<Package>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IPackageDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<Package> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IPackageDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<Package> PostAsync([FromBody] Package entity)
			{
				var dataService = DependencyResolver.Current.Get<IPackageDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<Package> PutAsync(int id, [FromBody] Package entity)
			{
				var dataService = DependencyResolver.Current.Get<IPackageDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}

		public partial class PackageVersionsController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IPackageVersionDataService>();
				await dataService.DeleteAsync(new PackageVersion { Id = id });
			}

			public async Task<IEnumerable<PackageVersion>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IPackageVersionDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<PackageVersion> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IPackageVersionDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<PackageVersion> PostAsync([FromBody] PackageVersion entity)
			{
				var dataService = DependencyResolver.Current.Get<IPackageVersionDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<PackageVersion> PutAsync(int id, [FromBody] PackageVersion entity)
			{
				var dataService = DependencyResolver.Current.Get<IPackageVersionDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}
	}

	namespace Units
	{
		public partial class ProductTypesController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IProductTypeDataService>();
				await dataService.DeleteAsync(new ProductType { Id = id });
			}

			public async Task<IEnumerable<ProductType>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IProductTypeDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<ProductType> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IProductTypeDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<ProductType> PostAsync([FromBody] ProductType entity)
			{
				var dataService = DependencyResolver.Current.Get<IProductTypeDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<ProductType> PutAsync(int id, [FromBody] ProductType entity)
			{
				var dataService = DependencyResolver.Current.Get<IProductTypeDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}

		public partial class UnitsController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUnitDataService>();
				await dataService.DeleteAsync(new Unit { Id = id });
			}

			public async Task<IEnumerable<Unit>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IUnitDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<Unit> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUnitDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<Unit> PostAsync([FromBody] Unit entity)
			{
				var dataService = DependencyResolver.Current.Get<IUnitDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<Unit> PutAsync(int id, [FromBody] Unit entity)
			{
				var dataService = DependencyResolver.Current.Get<IUnitDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}
	}

	namespace Update
	{
		public partial class UpdateGroupsController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUpdateGroupDataService>();
				await dataService.DeleteAsync(new UpdateGroup { Id = id });
			}

			public async Task<IEnumerable<UpdateGroup>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IUpdateGroupDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<UpdateGroup> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUpdateGroupDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<UpdateGroup> PostAsync([FromBody] UpdateGroup entity)
			{
				var dataService = DependencyResolver.Current.Get<IUpdateGroupDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<UpdateGroup> PutAsync(int id, [FromBody] UpdateGroup entity)
			{
				var dataService = DependencyResolver.Current.Get<IUpdateGroupDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}

		public partial class UpdatePartsController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUpdatePartDataService>();
				await dataService.DeleteAsync(new UpdatePart { Id = id });
			}

			public async Task<IEnumerable<UpdatePart>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IUpdatePartDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<UpdatePart> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUpdatePartDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<UpdatePart> PostAsync([FromBody] UpdatePart entity)
			{
				var dataService = DependencyResolver.Current.Get<IUpdatePartDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<UpdatePart> PutAsync(int id, [FromBody] UpdatePart entity)
			{
				var dataService = DependencyResolver.Current.Get<IUpdatePartDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}

		public partial class UpdateCommandsController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUpdateCommandDataService>();
				await dataService.DeleteAsync(new UpdateCommand { Id = id });
			}

			public async Task<IEnumerable<UpdateCommand>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IUpdateCommandDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<UpdateCommand> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUpdateCommandDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<UpdateCommand> PostAsync([FromBody] UpdateCommand entity)
			{
				var dataService = DependencyResolver.Current.Get<IUpdateCommandDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<UpdateCommand> PutAsync(int id, [FromBody] UpdateCommand entity)
			{
				var dataService = DependencyResolver.Current.Get<IUpdateCommandDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}

		public partial class UpdateFeedbacksController : ApiController
		{
			public async Task DeleteAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUpdateFeedbackDataService>();
				await dataService.DeleteAsync(new UpdateFeedback { Id = id });
			}

			public async Task<IEnumerable<UpdateFeedback>> GetAsync()
			{
				var dataService = DependencyResolver.Current.Get<IUpdateFeedbackDataService>();
				return await dataService.QueryAsync();
			}

			public async Task<UpdateFeedback> GetAsync(int id)
			{
				var dataService = DependencyResolver.Current.Get<IUpdateFeedbackDataService>();
				return await dataService.GetAsync(id);
			}

			public async Task<UpdateFeedback> PostAsync([FromBody] UpdateFeedback entity)
			{
				var dataService = DependencyResolver.Current.Get<IUpdateFeedbackDataService>();
				return await dataService.AddAsync(entity);
			}

			public async Task<UpdateFeedback> PutAsync(int id, [FromBody] UpdateFeedback entity)
			{
				var dataService = DependencyResolver.Current.Get<IUpdateFeedbackDataService>();
				return await dataService.UpdateAsync(entity);
			}
		}
	}
}

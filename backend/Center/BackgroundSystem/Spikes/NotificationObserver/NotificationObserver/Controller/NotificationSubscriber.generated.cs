namespace Gorba.Center.BackgroundSystem.Spikes.NotificationObserver.Controller
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

	public partial class NotificationSubscriber
	{
		private IEnumerable<string> GetPaths()
		{
			yield return "UserRoles";
			yield return "Authorizations";
			yield return "UnitConfigurations";
			yield return "MediaConfigurations";
			yield return "Documents";
			yield return "DocumentVersions";
			yield return "Tenants";
			yield return "Users";
			yield return "AssociationTenantUserUserRoles";
			yield return "UserDefinedProperties";
			yield return "Resources";
			yield return "Packages";
			yield return "PackageVersions";
			yield return "ProductTypes";
			yield return "Units";
			yield return "UpdateGroups";
			yield return "UpdateParts";
			yield return "UpdateCommands";
			yield return "UpdateFeedbacks";
		}
	}
}
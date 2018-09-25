namespace Gorba.Center.Common.ServiceModel.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;

    using Filters;

		public static class AuthorizationPropertyReferenceFilters
		{
			[DataContract]
			public class UserRolePropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class UserRolePropertyReferenceFilters
		{
		}

		public static class MediaConfigurationPropertyReferenceFilters
		{
			[DataContract]
			public class DocumentPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class UnitConfigurationPropertyReferenceFilters
		{
			[DataContract]
			public class DocumentPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}

			[DataContract]
			public class ProductTypePropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class DocumentPropertyReferenceFilters
		{
			[DataContract]
			public class TenantPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class DocumentVersionPropertyReferenceFilters
		{
			[DataContract]
			public class CreatingUserPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}

			[DataContract]
			public class DocumentPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class LogEntryPropertyReferenceFilters
		{
			[DataContract]
			public class UnitPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class AssociationTenantUserUserRolePropertyReferenceFilters
		{
			[DataContract]
			public class TenantPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}

			[DataContract]
			public class UserPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}

			[DataContract]
			public class UserRolePropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class TenantPropertyReferenceFilters
		{
		}

		public static class UserPropertyReferenceFilters
		{
			[DataContract]
			public class OwnerTenantPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class SystemConfigPropertyReferenceFilters
		{
		}

		public static class UserDefinedPropertyPropertyReferenceFilters
		{
			[DataContract]
			public class TenantPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class ContentResourcePropertyReferenceFilters
		{
			[DataContract]
			public class UploadingUserPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class ResourcePropertyReferenceFilters
		{
			[DataContract]
			public class UploadingUserPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class PackagePropertyReferenceFilters
		{
		}

		public static class PackageVersionPropertyReferenceFilters
		{
			[DataContract]
			public class PackagePropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class ProductTypePropertyReferenceFilters
		{
		}

		public static class UnitPropertyReferenceFilters
		{
			[DataContract]
			public class ProductTypePropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}

			[DataContract]
			public class TenantPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}

			[DataContract]
			public class UpdateGroupPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class UpdateCommandPropertyReferenceFilters
		{
			[DataContract]
			public class UnitPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class UpdateFeedbackPropertyReferenceFilters
		{
			[DataContract]
			public class UpdateCommandPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class UpdateGroupPropertyReferenceFilters
		{
			[DataContract]
			public class MediaConfigurationPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}

			[DataContract]
			public class TenantPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}

			[DataContract]
			public class UnitConfigurationPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}

		public static class UpdatePartPropertyReferenceFilters
		{
			[DataContract]
			public class UpdateGroupPropertyReferenceFilter : PropertyReferenceFilterBase
			{
				[DataMember]
				public int Value { get; set; }
			}
		}
}
namespace Gorba.Center.BackgroundSystem.Data.Access
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using EntityFramework.BulkInsert.Extensions;

    using Gorba.Center.BackgroundSystem.Data.Model;
    using Gorba.Center.BackgroundSystem.Data.Model.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
    using Gorba.Center.BackgroundSystem.Data.Model.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.BackgroundSystem.Data.Model.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.BackgroundSystem.Data.Model.Log;
    using Gorba.Center.Common.ServiceModel.Filters.Log;
    using Gorba.Center.BackgroundSystem.Data.Model.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.BackgroundSystem.Data.Model.Meta;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.BackgroundSystem.Data.Model.Resources;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.BackgroundSystem.Data.Model.Software;
    using Gorba.Center.Common.ServiceModel.Filters.Software;
    using Gorba.Center.BackgroundSystem.Data.Model.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.BackgroundSystem.Data.Model.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Update;


    public partial class AuthorizationQueryableRepository : RepositoryBase, IQueryableRepository<Authorization>
    {
        protected readonly CenterDataContext dataContext;

        public AuthorizationQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(Authorization entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<Authorization> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.Authorizations.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<Authorization> Query()
        {
            return this.dataContext.Authorizations.AsNoTracking();
        }
    }

    public partial class AuthorizationRepository : AuthorizationQueryableRepository, IRepository<Authorization>
    {
        public AuthorizationRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<Authorization> AddAsync(Authorization entity)
        {
            if (entity.UserRole == null)
            {
                throw new ArgumentException("Property 'UserRole' can't be null");
            }

            entity.UserRole = this.dataContext.UserRoles.Find(entity.UserRole.Id);
            this.dataContext.Authorizations.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.Authorizations.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(Authorization entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.Authorizations.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.Authorizations.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<Authorization> UpdateAsync(Authorization entity)
        {
            var original = this.dataContext.Authorizations.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'Authorization' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            if (entity.UserRole == null)
            {
                this.dataContext.LoadReference(original, "UserRole");
            }
            else if (entity.UserRole == UserRole.Null)
            {
                this.dataContext.SetReference(original, "UserRole", null);
            }
            else
            {
                this.dataContext.SetReference(original, "UserRole", this.dataContext.UserRoles.Find(entity.UserRole.Id));
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class AuthorizationQueryableRepositoryFactory
    {
        static AuthorizationQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static AuthorizationQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<Authorization> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultAuthorizationQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(AuthorizationQueryableRepositoryFactory instance)
        {
            AuthorizationQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultAuthorizationQueryableRepositoryFactory : AuthorizationQueryableRepositoryFactory
        {
            static DefaultAuthorizationQueryableRepositoryFactory()
            {
                Instance = new DefaultAuthorizationQueryableRepositoryFactory();
            }

            public static AuthorizationQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<Authorization> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new AuthorizationRepository(dataContext);
            }
        }
    }
        
    public abstract class AuthorizationRepositoryFactory
    {
        static AuthorizationRepositoryFactory()
        {
            ResetInstance();
        }

        public static AuthorizationRepositoryFactory Current { get; private set; }

        public abstract IRepository<Authorization> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultAuthorizationRepositoryFactory.Instance);
        }

        public static void SetInstance(AuthorizationRepositoryFactory instance)
        {
            AuthorizationRepositoryFactory.Current = instance;
        }

        private sealed class DefaultAuthorizationRepositoryFactory : AuthorizationRepositoryFactory
        {
            static DefaultAuthorizationRepositoryFactory()
            {
                Instance = new DefaultAuthorizationRepositoryFactory();
            }

            public static AuthorizationRepositoryFactory Instance { get; private set; }

            public override IRepository<Authorization> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new AuthorizationRepository(dataContext);
            }
        }
    }
    
    public partial class UserRoleQueryableRepository : RepositoryBase, IQueryableRepository<UserRole>
    {
        protected readonly CenterDataContext dataContext;

        public UserRoleQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(UserRole entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<UserRole> FindAsync(params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length != 1 || !(keyValues[0] is int))
            {
                throw new ArgumentException("Key values must contain only the Id value of type 'int'", "keyValues");
            }

            var id = (int)keyValues[0];
            var entity = await ExtendQueryWithUserDefinedProperties(this.dataContext.UserRoles).SingleOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<UserRole> Query()
        {
            return ExtendQueryWithUserDefinedProperties(this.dataContext.UserRoles).AsNoTracking();
        }

        private static IQueryable<UserRole> ExtendQueryWithUserDefinedProperties(IQueryable<UserRole> query)
        {
            return query.Include("UserDefinedProperties.PropertyDefinition");
        }
    }

    public partial class UserRoleRepository : UserRoleQueryableRepository, IRepository<UserRole>
    {
        public UserRoleRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<UserRole> AddAsync(UserRole entity)
        {
            if (entity.Authorizations != null)
            {
                entity.Authorizations = entity.Authorizations.Select(i => this.dataContext.Authorizations.Find(i.Id)).ToList();
            }

            this.dataContext.UserRoles.Add(entity);
            var userDefinedProperties =
                await this.dataContext.UserDefinedProperties.Where(
                    property => property.OwnerEntity == UserDefinedPropertyEnabledEntity.UserRole).ToListAsync();
            var validProperties = userDefinedProperties.Select(property => property.Name);
            if (entity.RawUserDefinedProperties.Keys.Except(validProperties).Any())
            {
                throw new InvalidDataException("Found invalid user defined properties");
            }

            entity.UserDefinedProperties.Clear();
            foreach (var userDefinedProperty in userDefinedProperties)
            {
                var value = entity.RawUserDefinedProperties.ContainsKey(userDefinedProperty.Name)
                                ? entity.RawUserDefinedProperties[userDefinedProperty.Name]
                                : null;
                entity.UserDefinedProperties.Add(new UserRoleUserDefinedProperty(userDefinedProperty, value));
            }

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.UserRoles.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(UserRole entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.UserRoles.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.UserRoles.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<UserRole> UpdateAsync(UserRole entity)
        {
            var original =
                this.dataContext.UserRoles.Include("UserDefinedProperties.PropertyDefinition")
                    .SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'UserRole' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            var userDefinedProperties =
                await this.dataContext.UserDefinedProperties.Where(
                    property => property.OwnerEntity == UserDefinedPropertyEnabledEntity.UserRole).ToListAsync();
            var validProperties = userDefinedProperties.Select(property => property.Name);
            if (entity.RawUserDefinedProperties.Keys.Except(validProperties).Any())
            {
                throw new InvalidDataException("Found invalid user defined properties");
            }

            entity.UserDefinedProperties.Clear();
            foreach (var userDefinedProperty in userDefinedProperties)
            {
                var userDefinedPropertyValueEntity =
                    original.UserDefinedProperties.SingleOrDefault(
                        property => property.PropertyDefinition.Id == userDefinedProperty.Id);
                if (userDefinedPropertyValueEntity == null)
                {
                    userDefinedPropertyValueEntity = new UserRoleUserDefinedProperty(userDefinedProperty, null);
                    original.UserDefinedProperties.Add(userDefinedPropertyValueEntity);
                }

                userDefinedPropertyValueEntity.Value =
                    entity.RawUserDefinedProperties.ContainsKey(userDefinedProperty.Name)
                        ? entity.RawUserDefinedProperties[userDefinedProperty.Name]
                        : null;
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class UserRoleQueryableRepositoryFactory
    {
        static UserRoleQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static UserRoleQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<UserRole> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUserRoleQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(UserRoleQueryableRepositoryFactory instance)
        {
            UserRoleQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUserRoleQueryableRepositoryFactory : UserRoleQueryableRepositoryFactory
        {
            static DefaultUserRoleQueryableRepositoryFactory()
            {
                Instance = new DefaultUserRoleQueryableRepositoryFactory();
            }

            public static UserRoleQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<UserRole> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UserRoleRepository(dataContext);
            }
        }
    }
        
    public abstract class UserRoleRepositoryFactory
    {
        static UserRoleRepositoryFactory()
        {
            ResetInstance();
        }

        public static UserRoleRepositoryFactory Current { get; private set; }

        public abstract IRepository<UserRole> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUserRoleRepositoryFactory.Instance);
        }

        public static void SetInstance(UserRoleRepositoryFactory instance)
        {
            UserRoleRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUserRoleRepositoryFactory : UserRoleRepositoryFactory
        {
            static DefaultUserRoleRepositoryFactory()
            {
                Instance = new DefaultUserRoleRepositoryFactory();
            }

            public static UserRoleRepositoryFactory Instance { get; private set; }

            public override IRepository<UserRole> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UserRoleRepository(dataContext);
            }
        }
    }
    
    public partial class UnitConfigurationQueryableRepository : RepositoryBase, IQueryableRepository<UnitConfiguration>
    {
        protected readonly CenterDataContext dataContext;

        public UnitConfigurationQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(UnitConfiguration entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<UnitConfiguration> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.UnitConfigurations.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<UnitConfiguration> Query()
        {
            return this.dataContext.UnitConfigurations.AsNoTracking();
        }
    }

    public partial class UnitConfigurationRepository : UnitConfigurationQueryableRepository, IRepository<UnitConfiguration>
    {
        public UnitConfigurationRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<UnitConfiguration> AddAsync(UnitConfiguration entity)
        {
            if (entity.Document == null)
            {
                throw new ArgumentException("Property 'Document' can't be null");
            }

            entity.Document = this.dataContext.Documents.Find(entity.Document.Id);
            if (entity.ProductType == null)
            {
                throw new ArgumentException("Property 'ProductType' can't be null");
            }

            entity.ProductType = this.dataContext.ProductTypes.Find(entity.ProductType.Id);
            if (entity.UpdateGroups != null)
            {
                entity.UpdateGroups = entity.UpdateGroups.Select(i => this.dataContext.UpdateGroups.Find(i.Id)).ToList();
            }

            this.dataContext.UnitConfigurations.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.UnitConfigurations.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(UnitConfiguration entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.UnitConfigurations.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.UnitConfigurations.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<UnitConfiguration> UpdateAsync(UnitConfiguration entity)
        {
            var original = this.dataContext.UnitConfigurations.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'UnitConfiguration' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            if (entity.Document == null)
            {
                this.dataContext.LoadReference(original, "Document");
            }
            else if (entity.Document == Document.Null)
            {
                this.dataContext.SetReference(original, "Document", null);
            }
            else
            {
                this.dataContext.SetReference(original, "Document", this.dataContext.Documents.Find(entity.Document.Id));
            }

            if (entity.ProductType == null)
            {
                this.dataContext.LoadReference(original, "ProductType");
            }
            else if (entity.ProductType == ProductType.Null)
            {
                this.dataContext.SetReference(original, "ProductType", null);
            }
            else
            {
                this.dataContext.SetReference(original, "ProductType", this.dataContext.ProductTypes.Find(entity.ProductType.Id));
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class UnitConfigurationQueryableRepositoryFactory
    {
        static UnitConfigurationQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static UnitConfigurationQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<UnitConfiguration> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUnitConfigurationQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(UnitConfigurationQueryableRepositoryFactory instance)
        {
            UnitConfigurationQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUnitConfigurationQueryableRepositoryFactory : UnitConfigurationQueryableRepositoryFactory
        {
            static DefaultUnitConfigurationQueryableRepositoryFactory()
            {
                Instance = new DefaultUnitConfigurationQueryableRepositoryFactory();
            }

            public static UnitConfigurationQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<UnitConfiguration> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UnitConfigurationRepository(dataContext);
            }
        }
    }
        
    public abstract class UnitConfigurationRepositoryFactory
    {
        static UnitConfigurationRepositoryFactory()
        {
            ResetInstance();
        }

        public static UnitConfigurationRepositoryFactory Current { get; private set; }

        public abstract IRepository<UnitConfiguration> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUnitConfigurationRepositoryFactory.Instance);
        }

        public static void SetInstance(UnitConfigurationRepositoryFactory instance)
        {
            UnitConfigurationRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUnitConfigurationRepositoryFactory : UnitConfigurationRepositoryFactory
        {
            static DefaultUnitConfigurationRepositoryFactory()
            {
                Instance = new DefaultUnitConfigurationRepositoryFactory();
            }

            public static UnitConfigurationRepositoryFactory Instance { get; private set; }

            public override IRepository<UnitConfiguration> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UnitConfigurationRepository(dataContext);
            }
        }
    }
    
    public partial class MediaConfigurationQueryableRepository : RepositoryBase, IQueryableRepository<MediaConfiguration>
    {
        protected readonly CenterDataContext dataContext;

        public MediaConfigurationQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(MediaConfiguration entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<MediaConfiguration> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.MediaConfigurations.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<MediaConfiguration> Query()
        {
            return this.dataContext.MediaConfigurations.AsNoTracking();
        }
    }

    public partial class MediaConfigurationRepository : MediaConfigurationQueryableRepository, IRepository<MediaConfiguration>
    {
        public MediaConfigurationRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<MediaConfiguration> AddAsync(MediaConfiguration entity)
        {
            if (entity.Document == null)
            {
                throw new ArgumentException("Property 'Document' can't be null");
            }

            entity.Document = this.dataContext.Documents.Find(entity.Document.Id);
            if (entity.UpdateGroups != null)
            {
                entity.UpdateGroups = entity.UpdateGroups.Select(i => this.dataContext.UpdateGroups.Find(i.Id)).ToList();
            }

            this.dataContext.MediaConfigurations.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.MediaConfigurations.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(MediaConfiguration entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.MediaConfigurations.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.MediaConfigurations.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<MediaConfiguration> UpdateAsync(MediaConfiguration entity)
        {
            var original = this.dataContext.MediaConfigurations.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'MediaConfiguration' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            if (entity.Document == null)
            {
                this.dataContext.LoadReference(original, "Document");
            }
            else if (entity.Document == Document.Null)
            {
                this.dataContext.SetReference(original, "Document", null);
            }
            else
            {
                this.dataContext.SetReference(original, "Document", this.dataContext.Documents.Find(entity.Document.Id));
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class MediaConfigurationQueryableRepositoryFactory
    {
        static MediaConfigurationQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static MediaConfigurationQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<MediaConfiguration> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultMediaConfigurationQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(MediaConfigurationQueryableRepositoryFactory instance)
        {
            MediaConfigurationQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultMediaConfigurationQueryableRepositoryFactory : MediaConfigurationQueryableRepositoryFactory
        {
            static DefaultMediaConfigurationQueryableRepositoryFactory()
            {
                Instance = new DefaultMediaConfigurationQueryableRepositoryFactory();
            }

            public static MediaConfigurationQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<MediaConfiguration> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new MediaConfigurationRepository(dataContext);
            }
        }
    }
        
    public abstract class MediaConfigurationRepositoryFactory
    {
        static MediaConfigurationRepositoryFactory()
        {
            ResetInstance();
        }

        public static MediaConfigurationRepositoryFactory Current { get; private set; }

        public abstract IRepository<MediaConfiguration> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultMediaConfigurationRepositoryFactory.Instance);
        }

        public static void SetInstance(MediaConfigurationRepositoryFactory instance)
        {
            MediaConfigurationRepositoryFactory.Current = instance;
        }

        private sealed class DefaultMediaConfigurationRepositoryFactory : MediaConfigurationRepositoryFactory
        {
            static DefaultMediaConfigurationRepositoryFactory()
            {
                Instance = new DefaultMediaConfigurationRepositoryFactory();
            }

            public static MediaConfigurationRepositoryFactory Instance { get; private set; }

            public override IRepository<MediaConfiguration> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new MediaConfigurationRepository(dataContext);
            }
        }
    }
    
    public partial class DocumentQueryableRepository : RepositoryBase, IQueryableRepository<Document>
    {
        protected readonly CenterDataContext dataContext;

        public DocumentQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(Document entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<Document> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.Documents.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<Document> Query()
        {
            return this.dataContext.Documents.AsNoTracking();
        }
    }

    public partial class DocumentRepository : DocumentQueryableRepository, IRepository<Document>
    {
        public DocumentRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<Document> AddAsync(Document entity)
        {
            if (entity.Tenant == null)
            {
                throw new ArgumentException("Property 'Tenant' can't be null");
            }

            entity.Tenant = this.dataContext.Tenants.Find(entity.Tenant.Id);
            if (entity.Versions != null)
            {
                entity.Versions = entity.Versions.Select(i => this.dataContext.DocumentVersions.Find(i.Id)).ToList();
            }

            this.dataContext.Documents.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.Documents.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(Document entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.Documents.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.Documents.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<Document> UpdateAsync(Document entity)
        {
            var original = this.dataContext.Documents.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'Document' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            if (entity.Tenant == null)
            {
                this.dataContext.LoadReference(original, "Tenant");
            }
            else if (entity.Tenant == Tenant.Null)
            {
                this.dataContext.SetReference(original, "Tenant", null);
            }
            else
            {
                this.dataContext.SetReference(original, "Tenant", this.dataContext.Tenants.Find(entity.Tenant.Id));
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class DocumentQueryableRepositoryFactory
    {
        static DocumentQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static DocumentQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<Document> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultDocumentQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(DocumentQueryableRepositoryFactory instance)
        {
            DocumentQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultDocumentQueryableRepositoryFactory : DocumentQueryableRepositoryFactory
        {
            static DefaultDocumentQueryableRepositoryFactory()
            {
                Instance = new DefaultDocumentQueryableRepositoryFactory();
            }

            public static DocumentQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<Document> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new DocumentRepository(dataContext);
            }
        }
    }
        
    public abstract class DocumentRepositoryFactory
    {
        static DocumentRepositoryFactory()
        {
            ResetInstance();
        }

        public static DocumentRepositoryFactory Current { get; private set; }

        public abstract IRepository<Document> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultDocumentRepositoryFactory.Instance);
        }

        public static void SetInstance(DocumentRepositoryFactory instance)
        {
            DocumentRepositoryFactory.Current = instance;
        }

        private sealed class DefaultDocumentRepositoryFactory : DocumentRepositoryFactory
        {
            static DefaultDocumentRepositoryFactory()
            {
                Instance = new DefaultDocumentRepositoryFactory();
            }

            public static DocumentRepositoryFactory Instance { get; private set; }

            public override IRepository<Document> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new DocumentRepository(dataContext);
            }
        }
    }
    
    public partial class DocumentVersionQueryableRepository : RepositoryBase, IQueryableRepository<DocumentVersion>
    {
        protected readonly CenterDataContext dataContext;

        public DocumentVersionQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(DocumentVersion entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<DocumentVersion> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.DocumentVersions.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<DocumentVersion> Query()
        {
            return this.dataContext.DocumentVersions.AsNoTracking();
        }
    }

    public partial class DocumentVersionRepository : DocumentVersionQueryableRepository, IRepository<DocumentVersion>
    {
        public DocumentVersionRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<DocumentVersion> AddAsync(DocumentVersion entity)
        {
            if (entity.Document == null)
            {
                throw new ArgumentException("Property 'Document' can't be null");
            }

            entity.Document = this.dataContext.Documents.Find(entity.Document.Id);
            if (entity.CreatingUser != null)
            {
                entity.CreatingUser = this.dataContext.Users.Find(entity.CreatingUser.Id);
            }

            this.dataContext.DocumentVersions.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.DocumentVersions.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(DocumentVersion entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.DocumentVersions.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.LoadReference(original, "Content");
            var originalContent = original.Content;
            this.dataContext.DocumentVersions.Remove(original);
            
            if (originalContent != null)
            {
                this.dataContext.XmlData.Remove(originalContent);
            }

            this.dataContext.SaveChanges();
        }

        public async Task<DocumentVersion> UpdateAsync(DocumentVersion entity)
        {
            var original = this.dataContext.DocumentVersions.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'DocumentVersion' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            if (entity.Document == null)
            {
                this.dataContext.LoadReference(original, "Document");
            }
            else if (entity.Document == Document.Null)
            {
                this.dataContext.SetReference(original, "Document", null);
            }
            else
            {
                this.dataContext.SetReference(original, "Document", this.dataContext.Documents.Find(entity.Document.Id));
            }

            if (entity.CreatingUser == null)
            {
                this.dataContext.LoadReference(original, "CreatingUser");
            }
            else if (entity.CreatingUser == User.Null)
            {
                this.dataContext.SetReference(original, "CreatingUser", null);
            }
            else
            {
                this.dataContext.SetReference(original, "CreatingUser", this.dataContext.Users.Find(entity.CreatingUser.Id));
            }
            
            this.dataContext.LoadReference(original, "Content");
            if (entity.Content != null)
            {
                if (original.Content == null)
                {
                    original.Content = entity.Content;
                }
                else
                {
                    entity.Content.Id = original.Content.Id;
                    this.dataContext.SetValues(original.Content, entity.Content);
                }
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class DocumentVersionQueryableRepositoryFactory
    {
        static DocumentVersionQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static DocumentVersionQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<DocumentVersion> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultDocumentVersionQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(DocumentVersionQueryableRepositoryFactory instance)
        {
            DocumentVersionQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultDocumentVersionQueryableRepositoryFactory : DocumentVersionQueryableRepositoryFactory
        {
            static DefaultDocumentVersionQueryableRepositoryFactory()
            {
                Instance = new DefaultDocumentVersionQueryableRepositoryFactory();
            }

            public static DocumentVersionQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<DocumentVersion> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new DocumentVersionRepository(dataContext);
            }
        }
    }
        
    public abstract class DocumentVersionRepositoryFactory
    {
        static DocumentVersionRepositoryFactory()
        {
            ResetInstance();
        }

        public static DocumentVersionRepositoryFactory Current { get; private set; }

        public abstract IRepository<DocumentVersion> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultDocumentVersionRepositoryFactory.Instance);
        }

        public static void SetInstance(DocumentVersionRepositoryFactory instance)
        {
            DocumentVersionRepositoryFactory.Current = instance;
        }

        private sealed class DefaultDocumentVersionRepositoryFactory : DocumentVersionRepositoryFactory
        {
            static DefaultDocumentVersionRepositoryFactory()
            {
                Instance = new DefaultDocumentVersionRepositoryFactory();
            }

            public static DocumentVersionRepositoryFactory Instance { get; private set; }

            public override IRepository<DocumentVersion> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new DocumentVersionRepository(dataContext);
            }
        }
    }
    
    public partial class LogEntryQueryableRepository : RepositoryBase, IQueryableRepository<LogEntry>
    {
        protected readonly CenterDataContext dataContext;

        public LogEntryQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(LogEntry entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<LogEntry> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.LogEntries.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<LogEntry> Query()
        {
            return this.dataContext.LogEntries.AsNoTracking();
        }
    }

    public partial class LogEntryRepository : LogEntryQueryableRepository, IBulkRepository<LogEntry, LogEntryFilter>
    {
        public LogEntryRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<LogEntry> AddAsync(LogEntry entity)
        {
            if (entity.Unit != null)
            {
                entity.Unit = this.dataContext.Units.Find(entity.Unit.Id);
            }

            this.dataContext.LogEntries.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.LogEntries.Find(entity.Id);
            return added;
        }

        public Task AddRangeAsync(IEnumerable<LogEntry> entities)
        {
            var options = new BulkInsertOptions
                              {
                                  EnableStreaming = true,
                                  SqlBulkCopyOptions = SqlBulkCopyOptions.CheckConstraints
                              };
            this.dataContext.BulkInsert(entities, options);
            return Task.FromResult(0);
        }

        public Task<int> DeleteAsync(LogEntryFilter filter)
        {
            var command = this.dataContext.BuildBulkDeleteCommand(filter);
            return this.dataContext.Database.ExecuteSqlCommandAsync(command);
        }

        public async Task RemoveAsync(LogEntry entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.LogEntries.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.LogEntries.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<LogEntry> UpdateAsync(LogEntry entity)
        {
            var original = this.dataContext.LogEntries.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'LogEntry' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            if (entity.Unit == null)
            {
                this.dataContext.LoadReference(original, "Unit");
            }
            else if (entity.Unit == Unit.Null)
            {
                this.dataContext.SetReference(original, "Unit", null);
            }
            else
            {
                this.dataContext.SetReference(original, "Unit", this.dataContext.Units.Find(entity.Unit.Id));
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class LogEntryQueryableRepositoryFactory
    {
        static LogEntryQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static LogEntryQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<LogEntry> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultLogEntryQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(LogEntryQueryableRepositoryFactory instance)
        {
            LogEntryQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultLogEntryQueryableRepositoryFactory : LogEntryQueryableRepositoryFactory
        {
            static DefaultLogEntryQueryableRepositoryFactory()
            {
                Instance = new DefaultLogEntryQueryableRepositoryFactory();
            }

            public static LogEntryQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<LogEntry> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new LogEntryRepository(dataContext);
            }
        }
    }
        
    public abstract class LogEntryRepositoryFactory
    {
        static LogEntryRepositoryFactory()
        {
            ResetInstance();
        }

        public static LogEntryRepositoryFactory Current { get; private set; }

        public abstract IBulkRepository<LogEntry, LogEntryFilter> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultLogEntryRepositoryFactory.Instance);
        }

        public static void SetInstance(LogEntryRepositoryFactory instance)
        {
            LogEntryRepositoryFactory.Current = instance;
        }

        private sealed class DefaultLogEntryRepositoryFactory : LogEntryRepositoryFactory
        {
            static DefaultLogEntryRepositoryFactory()
            {
                Instance = new DefaultLogEntryRepositoryFactory();
            }

            public static LogEntryRepositoryFactory Instance { get; private set; }

            public override IBulkRepository<LogEntry, LogEntryFilter> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new LogEntryRepository(dataContext);
            }
        }
    }
    
    public partial class TenantQueryableRepository : RepositoryBase, IQueryableRepository<Tenant>
    {
        protected readonly CenterDataContext dataContext;

        public TenantQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(Tenant entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<Tenant> FindAsync(params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length != 1 || !(keyValues[0] is int))
            {
                throw new ArgumentException("Key values must contain only the Id value of type 'int'", "keyValues");
            }

            var id = (int)keyValues[0];
            var entity = await ExtendQueryWithUserDefinedProperties(this.dataContext.Tenants).SingleOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<Tenant> Query()
        {
            return ExtendQueryWithUserDefinedProperties(this.dataContext.Tenants).AsNoTracking();
        }

        private static IQueryable<Tenant> ExtendQueryWithUserDefinedProperties(IQueryable<Tenant> query)
        {
            return query.Include("UserDefinedProperties.PropertyDefinition");
        }
    }

    public partial class TenantRepository : TenantQueryableRepository, IRepository<Tenant>
    {
        public TenantRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<Tenant> AddAsync(Tenant entity)
        {
            if (entity.UpdateGroups != null)
            {
                entity.UpdateGroups = entity.UpdateGroups.Select(i => this.dataContext.UpdateGroups.Find(i.Id)).ToList();
            }

            if (entity.Users != null)
            {
                entity.Users = entity.Users.Select(i => this.dataContext.Users.Find(i.Id)).ToList();
            }

            this.dataContext.Tenants.Add(entity);
            var userDefinedProperties =
                await this.dataContext.UserDefinedProperties.Where(
                    property => property.OwnerEntity == UserDefinedPropertyEnabledEntity.Tenant).ToListAsync();
            var validProperties = userDefinedProperties.Select(property => property.Name);
            if (entity.RawUserDefinedProperties.Keys.Except(validProperties).Any())
            {
                throw new InvalidDataException("Found invalid user defined properties");
            }

            entity.UserDefinedProperties.Clear();
            foreach (var userDefinedProperty in userDefinedProperties)
            {
                var value = entity.RawUserDefinedProperties.ContainsKey(userDefinedProperty.Name)
                                ? entity.RawUserDefinedProperties[userDefinedProperty.Name]
                                : null;
                entity.UserDefinedProperties.Add(new TenantUserDefinedProperty(userDefinedProperty, value));
            }

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.Tenants.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(Tenant entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.Tenants.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.Tenants.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<Tenant> UpdateAsync(Tenant entity)
        {
            var original =
                this.dataContext.Tenants.Include("UserDefinedProperties.PropertyDefinition")
                    .SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'Tenant' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            var userDefinedProperties =
                await this.dataContext.UserDefinedProperties.Where(
                    property => property.OwnerEntity == UserDefinedPropertyEnabledEntity.Tenant).ToListAsync();
            var validProperties = userDefinedProperties.Select(property => property.Name);
            if (entity.RawUserDefinedProperties.Keys.Except(validProperties).Any())
            {
                throw new InvalidDataException("Found invalid user defined properties");
            }

            entity.UserDefinedProperties.Clear();
            foreach (var userDefinedProperty in userDefinedProperties)
            {
                var userDefinedPropertyValueEntity =
                    original.UserDefinedProperties.SingleOrDefault(
                        property => property.PropertyDefinition.Id == userDefinedProperty.Id);
                if (userDefinedPropertyValueEntity == null)
                {
                    userDefinedPropertyValueEntity = new TenantUserDefinedProperty(userDefinedProperty, null);
                    original.UserDefinedProperties.Add(userDefinedPropertyValueEntity);
                }

                userDefinedPropertyValueEntity.Value =
                    entity.RawUserDefinedProperties.ContainsKey(userDefinedProperty.Name)
                        ? entity.RawUserDefinedProperties[userDefinedProperty.Name]
                        : null;
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class TenantQueryableRepositoryFactory
    {
        static TenantQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static TenantQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<Tenant> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultTenantQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(TenantQueryableRepositoryFactory instance)
        {
            TenantQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultTenantQueryableRepositoryFactory : TenantQueryableRepositoryFactory
        {
            static DefaultTenantQueryableRepositoryFactory()
            {
                Instance = new DefaultTenantQueryableRepositoryFactory();
            }

            public static TenantQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<Tenant> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new TenantRepository(dataContext);
            }
        }
    }
        
    public abstract class TenantRepositoryFactory
    {
        static TenantRepositoryFactory()
        {
            ResetInstance();
        }

        public static TenantRepositoryFactory Current { get; private set; }

        public abstract IRepository<Tenant> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultTenantRepositoryFactory.Instance);
        }

        public static void SetInstance(TenantRepositoryFactory instance)
        {
            TenantRepositoryFactory.Current = instance;
        }

        private sealed class DefaultTenantRepositoryFactory : TenantRepositoryFactory
        {
            static DefaultTenantRepositoryFactory()
            {
                Instance = new DefaultTenantRepositoryFactory();
            }

            public static TenantRepositoryFactory Instance { get; private set; }

            public override IRepository<Tenant> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new TenantRepository(dataContext);
            }
        }
    }
    
    public partial class UserQueryableRepository : RepositoryBase, IQueryableRepository<User>
    {
        protected readonly CenterDataContext dataContext;

        public UserQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(User entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<User> FindAsync(params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length != 1 || !(keyValues[0] is int))
            {
                throw new ArgumentException("Key values must contain only the Id value of type 'int'", "keyValues");
            }

            var id = (int)keyValues[0];
            var entity = await ExtendQueryWithUserDefinedProperties(this.dataContext.Users).SingleOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<User> Query()
        {
            return ExtendQueryWithUserDefinedProperties(this.dataContext.Users).AsNoTracking();
        }

        private static IQueryable<User> ExtendQueryWithUserDefinedProperties(IQueryable<User> query)
        {
            return query.Include("UserDefinedProperties.PropertyDefinition");
        }
    }

    public partial class UserRepository : UserQueryableRepository, IRepository<User>
    {
        public UserRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<User> AddAsync(User entity)
        {
            if (entity.OwnerTenant == null)
            {
                throw new ArgumentException("Property 'OwnerTenant' can't be null");
            }

            entity.OwnerTenant = this.dataContext.Tenants.Find(entity.OwnerTenant.Id);
            if (entity.AssociationTenantUserUserRoles != null)
            {
                entity.AssociationTenantUserUserRoles = entity.AssociationTenantUserUserRoles.Select(i => this.dataContext.AssociationTenantUserUserRoles.Find(i.Id)).ToList();
            }

            this.dataContext.Users.Add(entity);
            var userDefinedProperties =
                await this.dataContext.UserDefinedProperties.Where(
                    property => property.OwnerEntity == UserDefinedPropertyEnabledEntity.User).ToListAsync();
            var validProperties = userDefinedProperties.Select(property => property.Name);
            if (entity.RawUserDefinedProperties.Keys.Except(validProperties).Any())
            {
                throw new InvalidDataException("Found invalid user defined properties");
            }

            entity.UserDefinedProperties.Clear();
            foreach (var userDefinedProperty in userDefinedProperties)
            {
                var value = entity.RawUserDefinedProperties.ContainsKey(userDefinedProperty.Name)
                                ? entity.RawUserDefinedProperties[userDefinedProperty.Name]
                                : null;
                entity.UserDefinedProperties.Add(new UserUserDefinedProperty(userDefinedProperty, value));
            }

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.Users.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(User entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.Users.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.Users.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<User> UpdateAsync(User entity)
        {
            var original =
                this.dataContext.Users.Include("UserDefinedProperties.PropertyDefinition")
                    .SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'User' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            var userDefinedProperties =
                await this.dataContext.UserDefinedProperties.Where(
                    property => property.OwnerEntity == UserDefinedPropertyEnabledEntity.User).ToListAsync();
            var validProperties = userDefinedProperties.Select(property => property.Name);
            if (entity.RawUserDefinedProperties.Keys.Except(validProperties).Any())
            {
                throw new InvalidDataException("Found invalid user defined properties");
            }

            entity.UserDefinedProperties.Clear();
            foreach (var userDefinedProperty in userDefinedProperties)
            {
                var userDefinedPropertyValueEntity =
                    original.UserDefinedProperties.SingleOrDefault(
                        property => property.PropertyDefinition.Id == userDefinedProperty.Id);
                if (userDefinedPropertyValueEntity == null)
                {
                    userDefinedPropertyValueEntity = new UserUserDefinedProperty(userDefinedProperty, null);
                    original.UserDefinedProperties.Add(userDefinedPropertyValueEntity);
                }

                userDefinedPropertyValueEntity.Value =
                    entity.RawUserDefinedProperties.ContainsKey(userDefinedProperty.Name)
                        ? entity.RawUserDefinedProperties[userDefinedProperty.Name]
                        : null;
            }

            if (entity.OwnerTenant == null)
            {
                this.dataContext.LoadReference(original, "OwnerTenant");
            }
            else if (entity.OwnerTenant == Tenant.Null)
            {
                this.dataContext.SetReference(original, "OwnerTenant", null);
            }
            else
            {
                this.dataContext.SetReference(original, "OwnerTenant", this.dataContext.Tenants.Find(entity.OwnerTenant.Id));
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class UserQueryableRepositoryFactory
    {
        static UserQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static UserQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<User> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUserQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(UserQueryableRepositoryFactory instance)
        {
            UserQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUserQueryableRepositoryFactory : UserQueryableRepositoryFactory
        {
            static DefaultUserQueryableRepositoryFactory()
            {
                Instance = new DefaultUserQueryableRepositoryFactory();
            }

            public static UserQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<User> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UserRepository(dataContext);
            }
        }
    }
        
    public abstract class UserRepositoryFactory
    {
        static UserRepositoryFactory()
        {
            ResetInstance();
        }

        public static UserRepositoryFactory Current { get; private set; }

        public abstract IRepository<User> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUserRepositoryFactory.Instance);
        }

        public static void SetInstance(UserRepositoryFactory instance)
        {
            UserRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUserRepositoryFactory : UserRepositoryFactory
        {
            static DefaultUserRepositoryFactory()
            {
                Instance = new DefaultUserRepositoryFactory();
            }

            public static UserRepositoryFactory Instance { get; private set; }

            public override IRepository<User> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UserRepository(dataContext);
            }
        }
    }
    
    public partial class AssociationTenantUserUserRoleQueryableRepository : RepositoryBase, IQueryableRepository<AssociationTenantUserUserRole>
    {
        protected readonly CenterDataContext dataContext;

        public AssociationTenantUserUserRoleQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(AssociationTenantUserUserRole entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<AssociationTenantUserUserRole> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.AssociationTenantUserUserRoles.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<AssociationTenantUserUserRole> Query()
        {
            return this.dataContext.AssociationTenantUserUserRoles.AsNoTracking();
        }
    }

    public partial class AssociationTenantUserUserRoleRepository : AssociationTenantUserUserRoleQueryableRepository, IRepository<AssociationTenantUserUserRole>
    {
        public AssociationTenantUserUserRoleRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<AssociationTenantUserUserRole> AddAsync(AssociationTenantUserUserRole entity)
        {
            if (entity.User == null)
            {
                throw new ArgumentException("Property 'User' can't be null");
            }

            entity.User = this.dataContext.Users.Find(entity.User.Id);
            if (entity.UserRole == null)
            {
                throw new ArgumentException("Property 'UserRole' can't be null");
            }

            entity.UserRole = this.dataContext.UserRoles.Find(entity.UserRole.Id);
            if (entity.Tenant != null)
            {
                entity.Tenant = this.dataContext.Tenants.Find(entity.Tenant.Id);
            }

            this.dataContext.AssociationTenantUserUserRoles.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.AssociationTenantUserUserRoles.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(AssociationTenantUserUserRole entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.AssociationTenantUserUserRoles.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.AssociationTenantUserUserRoles.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<AssociationTenantUserUserRole> UpdateAsync(AssociationTenantUserUserRole entity)
        {
            var original = this.dataContext.AssociationTenantUserUserRoles.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'AssociationTenantUserUserRole' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            if (entity.Tenant == null)
            {
                this.dataContext.LoadReference(original, "Tenant");
            }
            else if (entity.Tenant == Tenant.Null)
            {
                this.dataContext.SetReference(original, "Tenant", null);
            }
            else
            {
                this.dataContext.SetReference(original, "Tenant", this.dataContext.Tenants.Find(entity.Tenant.Id));
            }

            if (entity.User == null)
            {
                this.dataContext.LoadReference(original, "User");
            }
            else if (entity.User == User.Null)
            {
                this.dataContext.SetReference(original, "User", null);
            }
            else
            {
                this.dataContext.SetReference(original, "User", this.dataContext.Users.Find(entity.User.Id));
            }

            if (entity.UserRole == null)
            {
                this.dataContext.LoadReference(original, "UserRole");
            }
            else if (entity.UserRole == UserRole.Null)
            {
                this.dataContext.SetReference(original, "UserRole", null);
            }
            else
            {
                this.dataContext.SetReference(original, "UserRole", this.dataContext.UserRoles.Find(entity.UserRole.Id));
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class AssociationTenantUserUserRoleQueryableRepositoryFactory
    {
        static AssociationTenantUserUserRoleQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static AssociationTenantUserUserRoleQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<AssociationTenantUserUserRole> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultAssociationTenantUserUserRoleQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(AssociationTenantUserUserRoleQueryableRepositoryFactory instance)
        {
            AssociationTenantUserUserRoleQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultAssociationTenantUserUserRoleQueryableRepositoryFactory : AssociationTenantUserUserRoleQueryableRepositoryFactory
        {
            static DefaultAssociationTenantUserUserRoleQueryableRepositoryFactory()
            {
                Instance = new DefaultAssociationTenantUserUserRoleQueryableRepositoryFactory();
            }

            public static AssociationTenantUserUserRoleQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<AssociationTenantUserUserRole> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new AssociationTenantUserUserRoleRepository(dataContext);
            }
        }
    }
        
    public abstract class AssociationTenantUserUserRoleRepositoryFactory
    {
        static AssociationTenantUserUserRoleRepositoryFactory()
        {
            ResetInstance();
        }

        public static AssociationTenantUserUserRoleRepositoryFactory Current { get; private set; }

        public abstract IRepository<AssociationTenantUserUserRole> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultAssociationTenantUserUserRoleRepositoryFactory.Instance);
        }

        public static void SetInstance(AssociationTenantUserUserRoleRepositoryFactory instance)
        {
            AssociationTenantUserUserRoleRepositoryFactory.Current = instance;
        }

        private sealed class DefaultAssociationTenantUserUserRoleRepositoryFactory : AssociationTenantUserUserRoleRepositoryFactory
        {
            static DefaultAssociationTenantUserUserRoleRepositoryFactory()
            {
                Instance = new DefaultAssociationTenantUserUserRoleRepositoryFactory();
            }

            public static AssociationTenantUserUserRoleRepositoryFactory Instance { get; private set; }

            public override IRepository<AssociationTenantUserUserRole> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new AssociationTenantUserUserRoleRepository(dataContext);
            }
        }
    }
    
    public partial class UserDefinedPropertyQueryableRepository : RepositoryBase, IQueryableRepository<UserDefinedProperty>
    {
        protected readonly CenterDataContext dataContext;

        public UserDefinedPropertyQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(UserDefinedProperty entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<UserDefinedProperty> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.UserDefinedProperties.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<UserDefinedProperty> Query()
        {
            return this.dataContext.UserDefinedProperties.AsNoTracking();
        }
    }

    public partial class UserDefinedPropertyRepository : UserDefinedPropertyQueryableRepository, IRepository<UserDefinedProperty>
    {
        public UserDefinedPropertyRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<UserDefinedProperty> AddAsync(UserDefinedProperty entity)
        {
            if (entity.Tenant != null)
            {
                entity.Tenant = this.dataContext.Tenants.Find(entity.Tenant.Id);
            }

            this.dataContext.UserDefinedProperties.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.UserDefinedProperties.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(UserDefinedProperty entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.UserDefinedProperties.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.UserDefinedProperties.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<UserDefinedProperty> UpdateAsync(UserDefinedProperty entity)
        {
            var original = this.dataContext.UserDefinedProperties.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'UserDefinedProperty' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            if (entity.Tenant == null)
            {
                this.dataContext.LoadReference(original, "Tenant");
            }
            else if (entity.Tenant == Tenant.Null)
            {
                this.dataContext.SetReference(original, "Tenant", null);
            }
            else
            {
                this.dataContext.SetReference(original, "Tenant", this.dataContext.Tenants.Find(entity.Tenant.Id));
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class UserDefinedPropertyQueryableRepositoryFactory
    {
        static UserDefinedPropertyQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static UserDefinedPropertyQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<UserDefinedProperty> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUserDefinedPropertyQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(UserDefinedPropertyQueryableRepositoryFactory instance)
        {
            UserDefinedPropertyQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUserDefinedPropertyQueryableRepositoryFactory : UserDefinedPropertyQueryableRepositoryFactory
        {
            static DefaultUserDefinedPropertyQueryableRepositoryFactory()
            {
                Instance = new DefaultUserDefinedPropertyQueryableRepositoryFactory();
            }

            public static UserDefinedPropertyQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<UserDefinedProperty> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UserDefinedPropertyRepository(dataContext);
            }
        }
    }
        
    public abstract class UserDefinedPropertyRepositoryFactory
    {
        static UserDefinedPropertyRepositoryFactory()
        {
            ResetInstance();
        }

        public static UserDefinedPropertyRepositoryFactory Current { get; private set; }

        public abstract IRepository<UserDefinedProperty> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUserDefinedPropertyRepositoryFactory.Instance);
        }

        public static void SetInstance(UserDefinedPropertyRepositoryFactory instance)
        {
            UserDefinedPropertyRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUserDefinedPropertyRepositoryFactory : UserDefinedPropertyRepositoryFactory
        {
            static DefaultUserDefinedPropertyRepositoryFactory()
            {
                Instance = new DefaultUserDefinedPropertyRepositoryFactory();
            }

            public static UserDefinedPropertyRepositoryFactory Instance { get; private set; }

            public override IRepository<UserDefinedProperty> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UserDefinedPropertyRepository(dataContext);
            }
        }
    }
    
    public partial class SystemConfigQueryableRepository : RepositoryBase, IQueryableRepository<SystemConfig>
    {
        protected readonly CenterDataContext dataContext;

        public SystemConfigQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(SystemConfig entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<SystemConfig> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.SystemConfigs.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<SystemConfig> Query()
        {
            return this.dataContext.SystemConfigs.AsNoTracking();
        }
    }

    public partial class SystemConfigRepository : SystemConfigQueryableRepository, IRepository<SystemConfig>
    {
        public SystemConfigRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<SystemConfig> AddAsync(SystemConfig entity)
        {
            this.dataContext.SystemConfigs.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.SystemConfigs.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(SystemConfig entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.SystemConfigs.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.LoadReference(original, "Settings");
            var originalSettings = original.Settings;
            this.dataContext.SystemConfigs.Remove(original);
            
            if (originalSettings != null)
            {
                this.dataContext.XmlData.Remove(originalSettings);
            }

            this.dataContext.SaveChanges();
        }

        public async Task<SystemConfig> UpdateAsync(SystemConfig entity)
        {
            var original = this.dataContext.SystemConfigs.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'SystemConfig' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);
            
            this.dataContext.LoadReference(original, "Settings");
            if (entity.Settings != null)
            {
                if (original.Settings == null)
                {
                    original.Settings = entity.Settings;
                }
                else
                {
                    entity.Settings.Id = original.Settings.Id;
                    this.dataContext.SetValues(original.Settings, entity.Settings);
                }
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class SystemConfigQueryableRepositoryFactory
    {
        static SystemConfigQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static SystemConfigQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<SystemConfig> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultSystemConfigQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(SystemConfigQueryableRepositoryFactory instance)
        {
            SystemConfigQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultSystemConfigQueryableRepositoryFactory : SystemConfigQueryableRepositoryFactory
        {
            static DefaultSystemConfigQueryableRepositoryFactory()
            {
                Instance = new DefaultSystemConfigQueryableRepositoryFactory();
            }

            public static SystemConfigQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<SystemConfig> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new SystemConfigRepository(dataContext);
            }
        }
    }
        
    public abstract class SystemConfigRepositoryFactory
    {
        static SystemConfigRepositoryFactory()
        {
            ResetInstance();
        }

        public static SystemConfigRepositoryFactory Current { get; private set; }

        public abstract IRepository<SystemConfig> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultSystemConfigRepositoryFactory.Instance);
        }

        public static void SetInstance(SystemConfigRepositoryFactory instance)
        {
            SystemConfigRepositoryFactory.Current = instance;
        }

        private sealed class DefaultSystemConfigRepositoryFactory : SystemConfigRepositoryFactory
        {
            static DefaultSystemConfigRepositoryFactory()
            {
                Instance = new DefaultSystemConfigRepositoryFactory();
            }

            public static SystemConfigRepositoryFactory Instance { get; private set; }

            public override IRepository<SystemConfig> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new SystemConfigRepository(dataContext);
            }
        }
    }
    
    public partial class ResourceQueryableRepository : RepositoryBase, IQueryableRepository<Resource>
    {
        protected readonly CenterDataContext dataContext;

        public ResourceQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(Resource entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<Resource> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.Resources.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<Resource> Query()
        {
            return this.dataContext.Resources.AsNoTracking();
        }
    }

    public partial class ResourceRepository : ResourceQueryableRepository, IRepository<Resource>
    {
        public ResourceRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<Resource> AddAsync(Resource entity)
        {
            if (entity.UploadingUser != null)
            {
                entity.UploadingUser = this.dataContext.Users.Find(entity.UploadingUser.Id);
            }

            this.dataContext.Resources.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.Resources.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(Resource entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.Resources.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.Resources.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<Resource> UpdateAsync(Resource entity)
        {
            var original = this.dataContext.Resources.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'Resource' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            if (entity.UploadingUser == null)
            {
                this.dataContext.LoadReference(original, "UploadingUser");
            }
            else if (entity.UploadingUser == User.Null)
            {
                this.dataContext.SetReference(original, "UploadingUser", null);
            }
            else
            {
                this.dataContext.SetReference(original, "UploadingUser", this.dataContext.Users.Find(entity.UploadingUser.Id));
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class ResourceQueryableRepositoryFactory
    {
        static ResourceQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static ResourceQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<Resource> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultResourceQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(ResourceQueryableRepositoryFactory instance)
        {
            ResourceQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultResourceQueryableRepositoryFactory : ResourceQueryableRepositoryFactory
        {
            static DefaultResourceQueryableRepositoryFactory()
            {
                Instance = new DefaultResourceQueryableRepositoryFactory();
            }

            public static ResourceQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<Resource> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new ResourceRepository(dataContext);
            }
        }
    }
        
    public abstract class ResourceRepositoryFactory
    {
        static ResourceRepositoryFactory()
        {
            ResetInstance();
        }

        public static ResourceRepositoryFactory Current { get; private set; }

        public abstract IRepository<Resource> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultResourceRepositoryFactory.Instance);
        }

        public static void SetInstance(ResourceRepositoryFactory instance)
        {
            ResourceRepositoryFactory.Current = instance;
        }

        private sealed class DefaultResourceRepositoryFactory : ResourceRepositoryFactory
        {
            static DefaultResourceRepositoryFactory()
            {
                Instance = new DefaultResourceRepositoryFactory();
            }

            public static ResourceRepositoryFactory Instance { get; private set; }

            public override IRepository<Resource> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new ResourceRepository(dataContext);
            }
        }
    }
    
    public partial class ContentResourceQueryableRepository : RepositoryBase, IQueryableRepository<ContentResource>
    {
        protected readonly CenterDataContext dataContext;

        public ContentResourceQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(ContentResource entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<ContentResource> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.ContentResources.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<ContentResource> Query()
        {
            return this.dataContext.ContentResources.AsNoTracking();
        }
    }

    public partial class ContentResourceRepository : ContentResourceQueryableRepository, IRepository<ContentResource>
    {
        public ContentResourceRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<ContentResource> AddAsync(ContentResource entity)
        {
            if (entity.UploadingUser != null)
            {
                entity.UploadingUser = this.dataContext.Users.Find(entity.UploadingUser.Id);
            }

            this.dataContext.ContentResources.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.ContentResources.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(ContentResource entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.ContentResources.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.ContentResources.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<ContentResource> UpdateAsync(ContentResource entity)
        {
            var original = this.dataContext.ContentResources.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'ContentResource' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            if (entity.UploadingUser == null)
            {
                this.dataContext.LoadReference(original, "UploadingUser");
            }
            else if (entity.UploadingUser == User.Null)
            {
                this.dataContext.SetReference(original, "UploadingUser", null);
            }
            else
            {
                this.dataContext.SetReference(original, "UploadingUser", this.dataContext.Users.Find(entity.UploadingUser.Id));
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class ContentResourceQueryableRepositoryFactory
    {
        static ContentResourceQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static ContentResourceQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<ContentResource> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultContentResourceQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(ContentResourceQueryableRepositoryFactory instance)
        {
            ContentResourceQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultContentResourceQueryableRepositoryFactory : ContentResourceQueryableRepositoryFactory
        {
            static DefaultContentResourceQueryableRepositoryFactory()
            {
                Instance = new DefaultContentResourceQueryableRepositoryFactory();
            }

            public static ContentResourceQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<ContentResource> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new ContentResourceRepository(dataContext);
            }
        }
    }
        
    public abstract class ContentResourceRepositoryFactory
    {
        static ContentResourceRepositoryFactory()
        {
            ResetInstance();
        }

        public static ContentResourceRepositoryFactory Current { get; private set; }

        public abstract IRepository<ContentResource> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultContentResourceRepositoryFactory.Instance);
        }

        public static void SetInstance(ContentResourceRepositoryFactory instance)
        {
            ContentResourceRepositoryFactory.Current = instance;
        }

        private sealed class DefaultContentResourceRepositoryFactory : ContentResourceRepositoryFactory
        {
            static DefaultContentResourceRepositoryFactory()
            {
                Instance = new DefaultContentResourceRepositoryFactory();
            }

            public static ContentResourceRepositoryFactory Instance { get; private set; }

            public override IRepository<ContentResource> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new ContentResourceRepository(dataContext);
            }
        }
    }
    
    public partial class PackageQueryableRepository : RepositoryBase, IQueryableRepository<Package>
    {
        protected readonly CenterDataContext dataContext;

        public PackageQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(Package entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<Package> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.Packages.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<Package> Query()
        {
            return this.dataContext.Packages.AsNoTracking();
        }
    }

    public partial class PackageRepository : PackageQueryableRepository, IRepository<Package>
    {
        public PackageRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<Package> AddAsync(Package entity)
        {
            if (entity.Versions != null)
            {
                entity.Versions = entity.Versions.Select(i => this.dataContext.PackageVersions.Find(i.Id)).ToList();
            }

            this.dataContext.Packages.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.Packages.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(Package entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.Packages.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.Packages.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<Package> UpdateAsync(Package entity)
        {
            var original = this.dataContext.Packages.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'Package' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class PackageQueryableRepositoryFactory
    {
        static PackageQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static PackageQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<Package> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultPackageQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(PackageQueryableRepositoryFactory instance)
        {
            PackageQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultPackageQueryableRepositoryFactory : PackageQueryableRepositoryFactory
        {
            static DefaultPackageQueryableRepositoryFactory()
            {
                Instance = new DefaultPackageQueryableRepositoryFactory();
            }

            public static PackageQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<Package> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new PackageRepository(dataContext);
            }
        }
    }
        
    public abstract class PackageRepositoryFactory
    {
        static PackageRepositoryFactory()
        {
            ResetInstance();
        }

        public static PackageRepositoryFactory Current { get; private set; }

        public abstract IRepository<Package> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultPackageRepositoryFactory.Instance);
        }

        public static void SetInstance(PackageRepositoryFactory instance)
        {
            PackageRepositoryFactory.Current = instance;
        }

        private sealed class DefaultPackageRepositoryFactory : PackageRepositoryFactory
        {
            static DefaultPackageRepositoryFactory()
            {
                Instance = new DefaultPackageRepositoryFactory();
            }

            public static PackageRepositoryFactory Instance { get; private set; }

            public override IRepository<Package> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new PackageRepository(dataContext);
            }
        }
    }
    
    public partial class PackageVersionQueryableRepository : RepositoryBase, IQueryableRepository<PackageVersion>
    {
        protected readonly CenterDataContext dataContext;

        public PackageVersionQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(PackageVersion entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<PackageVersion> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.PackageVersions.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<PackageVersion> Query()
        {
            return this.dataContext.PackageVersions.AsNoTracking();
        }
    }

    public partial class PackageVersionRepository : PackageVersionQueryableRepository, IRepository<PackageVersion>
    {
        public PackageVersionRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<PackageVersion> AddAsync(PackageVersion entity)
        {
            if (entity.Package == null)
            {
                throw new ArgumentException("Property 'Package' can't be null");
            }

            entity.Package = this.dataContext.Packages.Find(entity.Package.Id);
            this.dataContext.PackageVersions.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.PackageVersions.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(PackageVersion entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.PackageVersions.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.LoadReference(original, "Structure");
            var originalStructure = original.Structure;
            this.dataContext.PackageVersions.Remove(original);
            
            if (originalStructure != null)
            {
                this.dataContext.XmlData.Remove(originalStructure);
            }

            this.dataContext.SaveChanges();
        }

        public async Task<PackageVersion> UpdateAsync(PackageVersion entity)
        {
            var original = this.dataContext.PackageVersions.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'PackageVersion' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            if (entity.Package == null)
            {
                this.dataContext.LoadReference(original, "Package");
            }
            else if (entity.Package == Package.Null)
            {
                this.dataContext.SetReference(original, "Package", null);
            }
            else
            {
                this.dataContext.SetReference(original, "Package", this.dataContext.Packages.Find(entity.Package.Id));
            }
            
            this.dataContext.LoadReference(original, "Structure");
            if (entity.Structure != null)
            {
                if (original.Structure == null)
                {
                    original.Structure = entity.Structure;
                }
                else
                {
                    entity.Structure.Id = original.Structure.Id;
                    this.dataContext.SetValues(original.Structure, entity.Structure);
                }
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class PackageVersionQueryableRepositoryFactory
    {
        static PackageVersionQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static PackageVersionQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<PackageVersion> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultPackageVersionQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(PackageVersionQueryableRepositoryFactory instance)
        {
            PackageVersionQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultPackageVersionQueryableRepositoryFactory : PackageVersionQueryableRepositoryFactory
        {
            static DefaultPackageVersionQueryableRepositoryFactory()
            {
                Instance = new DefaultPackageVersionQueryableRepositoryFactory();
            }

            public static PackageVersionQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<PackageVersion> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new PackageVersionRepository(dataContext);
            }
        }
    }
        
    public abstract class PackageVersionRepositoryFactory
    {
        static PackageVersionRepositoryFactory()
        {
            ResetInstance();
        }

        public static PackageVersionRepositoryFactory Current { get; private set; }

        public abstract IRepository<PackageVersion> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultPackageVersionRepositoryFactory.Instance);
        }

        public static void SetInstance(PackageVersionRepositoryFactory instance)
        {
            PackageVersionRepositoryFactory.Current = instance;
        }

        private sealed class DefaultPackageVersionRepositoryFactory : PackageVersionRepositoryFactory
        {
            static DefaultPackageVersionRepositoryFactory()
            {
                Instance = new DefaultPackageVersionRepositoryFactory();
            }

            public static PackageVersionRepositoryFactory Instance { get; private set; }

            public override IRepository<PackageVersion> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new PackageVersionRepository(dataContext);
            }
        }
    }
    
    public partial class ProductTypeQueryableRepository : RepositoryBase, IQueryableRepository<ProductType>
    {
        protected readonly CenterDataContext dataContext;

        public ProductTypeQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(ProductType entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<ProductType> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.ProductTypes.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<ProductType> Query()
        {
            return this.dataContext.ProductTypes.AsNoTracking();
        }
    }

    public partial class ProductTypeRepository : ProductTypeQueryableRepository, IRepository<ProductType>
    {
        public ProductTypeRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<ProductType> AddAsync(ProductType entity)
        {
            if (entity.Units != null)
            {
                entity.Units = entity.Units.Select(i => this.dataContext.Units.Find(i.Id)).ToList();
            }

            this.dataContext.ProductTypes.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.ProductTypes.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(ProductType entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.ProductTypes.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.LoadReference(original, "HardwareDescriptor");
            var originalHardwareDescriptor = original.HardwareDescriptor;
            this.dataContext.ProductTypes.Remove(original);
            
            if (originalHardwareDescriptor != null)
            {
                this.dataContext.XmlData.Remove(originalHardwareDescriptor);
            }

            this.dataContext.SaveChanges();
        }

        public async Task<ProductType> UpdateAsync(ProductType entity)
        {
            var original = this.dataContext.ProductTypes.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'ProductType' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);
            
            this.dataContext.LoadReference(original, "HardwareDescriptor");
            if (entity.HardwareDescriptor != null)
            {
                if (original.HardwareDescriptor == null)
                {
                    original.HardwareDescriptor = entity.HardwareDescriptor;
                }
                else
                {
                    entity.HardwareDescriptor.Id = original.HardwareDescriptor.Id;
                    this.dataContext.SetValues(original.HardwareDescriptor, entity.HardwareDescriptor);
                }
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class ProductTypeQueryableRepositoryFactory
    {
        static ProductTypeQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static ProductTypeQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<ProductType> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultProductTypeQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(ProductTypeQueryableRepositoryFactory instance)
        {
            ProductTypeQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultProductTypeQueryableRepositoryFactory : ProductTypeQueryableRepositoryFactory
        {
            static DefaultProductTypeQueryableRepositoryFactory()
            {
                Instance = new DefaultProductTypeQueryableRepositoryFactory();
            }

            public static ProductTypeQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<ProductType> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new ProductTypeRepository(dataContext);
            }
        }
    }
        
    public abstract class ProductTypeRepositoryFactory
    {
        static ProductTypeRepositoryFactory()
        {
            ResetInstance();
        }

        public static ProductTypeRepositoryFactory Current { get; private set; }

        public abstract IRepository<ProductType> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultProductTypeRepositoryFactory.Instance);
        }

        public static void SetInstance(ProductTypeRepositoryFactory instance)
        {
            ProductTypeRepositoryFactory.Current = instance;
        }

        private sealed class DefaultProductTypeRepositoryFactory : ProductTypeRepositoryFactory
        {
            static DefaultProductTypeRepositoryFactory()
            {
                Instance = new DefaultProductTypeRepositoryFactory();
            }

            public static ProductTypeRepositoryFactory Instance { get; private set; }

            public override IRepository<ProductType> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new ProductTypeRepository(dataContext);
            }
        }
    }
    
    public partial class UnitQueryableRepository : RepositoryBase, IQueryableRepository<Unit>
    {
        protected readonly CenterDataContext dataContext;

        public UnitQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(Unit entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<Unit> FindAsync(params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length != 1 || !(keyValues[0] is int))
            {
                throw new ArgumentException("Key values must contain only the Id value of type 'int'", "keyValues");
            }

            var id = (int)keyValues[0];
            var entity = await ExtendQueryWithUserDefinedProperties(this.dataContext.Units).SingleOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<Unit> Query()
        {
            return ExtendQueryWithUserDefinedProperties(this.dataContext.Units).AsNoTracking();
        }

        private static IQueryable<Unit> ExtendQueryWithUserDefinedProperties(IQueryable<Unit> query)
        {
            return query.Include("UserDefinedProperties.PropertyDefinition");
        }
    }

    public partial class UnitRepository : UnitQueryableRepository, IRepository<Unit>
    {
        public UnitRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<Unit> AddAsync(Unit entity)
        {
            if (entity.Tenant == null)
            {
                throw new ArgumentException("Property 'Tenant' can't be null");
            }

            entity.Tenant = this.dataContext.Tenants.Find(entity.Tenant.Id);
            if (entity.ProductType == null)
            {
                throw new ArgumentException("Property 'ProductType' can't be null");
            }

            entity.ProductType = this.dataContext.ProductTypes.Find(entity.ProductType.Id);
            if (entity.UpdateGroup != null)
            {
                entity.UpdateGroup = this.dataContext.UpdateGroups.Find(entity.UpdateGroup.Id);
            }

            if (entity.UpdateCommands != null)
            {
                entity.UpdateCommands = entity.UpdateCommands.Select(i => this.dataContext.UpdateCommands.Find(i.Id)).ToList();
            }

            this.dataContext.Units.Add(entity);
            var userDefinedProperties =
                await this.dataContext.UserDefinedProperties.Where(
                    property => property.OwnerEntity == UserDefinedPropertyEnabledEntity.Unit).ToListAsync();
            var validProperties = userDefinedProperties.Select(property => property.Name);
            if (entity.RawUserDefinedProperties.Keys.Except(validProperties).Any())
            {
                throw new InvalidDataException("Found invalid user defined properties");
            }

            entity.UserDefinedProperties.Clear();
            foreach (var userDefinedProperty in userDefinedProperties)
            {
                var value = entity.RawUserDefinedProperties.ContainsKey(userDefinedProperty.Name)
                                ? entity.RawUserDefinedProperties[userDefinedProperty.Name]
                                : null;
                entity.UserDefinedProperties.Add(new UnitUserDefinedProperty(userDefinedProperty, value));
            }

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.Units.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(Unit entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.Units.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.Units.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<Unit> UpdateAsync(Unit entity)
        {
            var original =
                this.dataContext.Units.Include("UserDefinedProperties.PropertyDefinition")
                    .SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'Unit' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            var userDefinedProperties =
                await this.dataContext.UserDefinedProperties.Where(
                    property => property.OwnerEntity == UserDefinedPropertyEnabledEntity.Unit).ToListAsync();
            var validProperties = userDefinedProperties.Select(property => property.Name);
            if (entity.RawUserDefinedProperties.Keys.Except(validProperties).Any())
            {
                throw new InvalidDataException("Found invalid user defined properties");
            }

            entity.UserDefinedProperties.Clear();
            foreach (var userDefinedProperty in userDefinedProperties)
            {
                var userDefinedPropertyValueEntity =
                    original.UserDefinedProperties.SingleOrDefault(
                        property => property.PropertyDefinition.Id == userDefinedProperty.Id);
                if (userDefinedPropertyValueEntity == null)
                {
                    userDefinedPropertyValueEntity = new UnitUserDefinedProperty(userDefinedProperty, null);
                    original.UserDefinedProperties.Add(userDefinedPropertyValueEntity);
                }

                userDefinedPropertyValueEntity.Value =
                    entity.RawUserDefinedProperties.ContainsKey(userDefinedProperty.Name)
                        ? entity.RawUserDefinedProperties[userDefinedProperty.Name]
                        : null;
            }

            if (entity.Tenant == null)
            {
                this.dataContext.LoadReference(original, "Tenant");
            }
            else if (entity.Tenant == Tenant.Null)
            {
                this.dataContext.SetReference(original, "Tenant", null);
            }
            else
            {
                this.dataContext.SetReference(original, "Tenant", this.dataContext.Tenants.Find(entity.Tenant.Id));
            }

            if (entity.ProductType == null)
            {
                this.dataContext.LoadReference(original, "ProductType");
            }
            else if (entity.ProductType == ProductType.Null)
            {
                this.dataContext.SetReference(original, "ProductType", null);
            }
            else
            {
                this.dataContext.SetReference(original, "ProductType", this.dataContext.ProductTypes.Find(entity.ProductType.Id));
            }

            if (entity.UpdateGroup == null)
            {
                this.dataContext.LoadReference(original, "UpdateGroup");
            }
            else if (entity.UpdateGroup == UpdateGroup.Null)
            {
                this.dataContext.SetReference(original, "UpdateGroup", null);
            }
            else
            {
                this.dataContext.SetReference(original, "UpdateGroup", this.dataContext.UpdateGroups.Find(entity.UpdateGroup.Id));
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class UnitQueryableRepositoryFactory
    {
        static UnitQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static UnitQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<Unit> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUnitQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(UnitQueryableRepositoryFactory instance)
        {
            UnitQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUnitQueryableRepositoryFactory : UnitQueryableRepositoryFactory
        {
            static DefaultUnitQueryableRepositoryFactory()
            {
                Instance = new DefaultUnitQueryableRepositoryFactory();
            }

            public static UnitQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<Unit> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UnitRepository(dataContext);
            }
        }
    }
        
    public abstract class UnitRepositoryFactory
    {
        static UnitRepositoryFactory()
        {
            ResetInstance();
        }

        public static UnitRepositoryFactory Current { get; private set; }

        public abstract IRepository<Unit> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUnitRepositoryFactory.Instance);
        }

        public static void SetInstance(UnitRepositoryFactory instance)
        {
            UnitRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUnitRepositoryFactory : UnitRepositoryFactory
        {
            static DefaultUnitRepositoryFactory()
            {
                Instance = new DefaultUnitRepositoryFactory();
            }

            public static UnitRepositoryFactory Instance { get; private set; }

            public override IRepository<Unit> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UnitRepository(dataContext);
            }
        }
    }
    
    public partial class UpdateGroupQueryableRepository : RepositoryBase, IQueryableRepository<UpdateGroup>
    {
        protected readonly CenterDataContext dataContext;

        public UpdateGroupQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(UpdateGroup entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<UpdateGroup> FindAsync(params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length != 1 || !(keyValues[0] is int))
            {
                throw new ArgumentException("Key values must contain only the Id value of type 'int'", "keyValues");
            }

            var id = (int)keyValues[0];
            var entity = await ExtendQueryWithUserDefinedProperties(this.dataContext.UpdateGroups).SingleOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<UpdateGroup> Query()
        {
            return ExtendQueryWithUserDefinedProperties(this.dataContext.UpdateGroups).AsNoTracking();
        }

        private static IQueryable<UpdateGroup> ExtendQueryWithUserDefinedProperties(IQueryable<UpdateGroup> query)
        {
            return query.Include("UserDefinedProperties.PropertyDefinition");
        }
    }

    public partial class UpdateGroupRepository : UpdateGroupQueryableRepository, IRepository<UpdateGroup>
    {
        public UpdateGroupRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<UpdateGroup> AddAsync(UpdateGroup entity)
        {
            if (entity.Tenant == null)
            {
                throw new ArgumentException("Property 'Tenant' can't be null");
            }

            entity.Tenant = this.dataContext.Tenants.Find(entity.Tenant.Id);
            if (entity.UnitConfiguration != null)
            {
                entity.UnitConfiguration = this.dataContext.UnitConfigurations.Find(entity.UnitConfiguration.Id);
            }

            if (entity.MediaConfiguration != null)
            {
                entity.MediaConfiguration = this.dataContext.MediaConfigurations.Find(entity.MediaConfiguration.Id);
            }

            if (entity.Units != null)
            {
                entity.Units = entity.Units.Select(i => this.dataContext.Units.Find(i.Id)).ToList();
            }

            if (entity.UpdateParts != null)
            {
                entity.UpdateParts = entity.UpdateParts.Select(i => this.dataContext.UpdateParts.Find(i.Id)).ToList();
            }

            this.dataContext.UpdateGroups.Add(entity);
            var userDefinedProperties =
                await this.dataContext.UserDefinedProperties.Where(
                    property => property.OwnerEntity == UserDefinedPropertyEnabledEntity.UpdateGroup).ToListAsync();
            var validProperties = userDefinedProperties.Select(property => property.Name);
            if (entity.RawUserDefinedProperties.Keys.Except(validProperties).Any())
            {
                throw new InvalidDataException("Found invalid user defined properties");
            }

            entity.UserDefinedProperties.Clear();
            foreach (var userDefinedProperty in userDefinedProperties)
            {
                var value = entity.RawUserDefinedProperties.ContainsKey(userDefinedProperty.Name)
                                ? entity.RawUserDefinedProperties[userDefinedProperty.Name]
                                : null;
                entity.UserDefinedProperties.Add(new UpdateGroupUserDefinedProperty(userDefinedProperty, value));
            }

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.UpdateGroups.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(UpdateGroup entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.UpdateGroups.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.UpdateGroups.Remove(original);
            this.dataContext.SaveChanges();
        }

        public async Task<UpdateGroup> UpdateAsync(UpdateGroup entity)
        {
            var original =
                this.dataContext.UpdateGroups.Include("UserDefinedProperties.PropertyDefinition")
                    .SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'UpdateGroup' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            var userDefinedProperties =
                await this.dataContext.UserDefinedProperties.Where(
                    property => property.OwnerEntity == UserDefinedPropertyEnabledEntity.UpdateGroup).ToListAsync();
            var validProperties = userDefinedProperties.Select(property => property.Name);
            if (entity.RawUserDefinedProperties.Keys.Except(validProperties).Any())
            {
                throw new InvalidDataException("Found invalid user defined properties");
            }

            entity.UserDefinedProperties.Clear();
            foreach (var userDefinedProperty in userDefinedProperties)
            {
                var userDefinedPropertyValueEntity =
                    original.UserDefinedProperties.SingleOrDefault(
                        property => property.PropertyDefinition.Id == userDefinedProperty.Id);
                if (userDefinedPropertyValueEntity == null)
                {
                    userDefinedPropertyValueEntity = new UpdateGroupUserDefinedProperty(userDefinedProperty, null);
                    original.UserDefinedProperties.Add(userDefinedPropertyValueEntity);
                }

                userDefinedPropertyValueEntity.Value =
                    entity.RawUserDefinedProperties.ContainsKey(userDefinedProperty.Name)
                        ? entity.RawUserDefinedProperties[userDefinedProperty.Name]
                        : null;
            }

            if (entity.Tenant == null)
            {
                this.dataContext.LoadReference(original, "Tenant");
            }
            else if (entity.Tenant == Tenant.Null)
            {
                this.dataContext.SetReference(original, "Tenant", null);
            }
            else
            {
                this.dataContext.SetReference(original, "Tenant", this.dataContext.Tenants.Find(entity.Tenant.Id));
            }

            if (entity.UnitConfiguration == null)
            {
                this.dataContext.LoadReference(original, "UnitConfiguration");
            }
            else if (entity.UnitConfiguration == UnitConfiguration.Null)
            {
                this.dataContext.SetReference(original, "UnitConfiguration", null);
            }
            else
            {
                this.dataContext.SetReference(original, "UnitConfiguration", this.dataContext.UnitConfigurations.Find(entity.UnitConfiguration.Id));
            }

            if (entity.MediaConfiguration == null)
            {
                this.dataContext.LoadReference(original, "MediaConfiguration");
            }
            else if (entity.MediaConfiguration == MediaConfiguration.Null)
            {
                this.dataContext.SetReference(original, "MediaConfiguration", null);
            }
            else
            {
                this.dataContext.SetReference(original, "MediaConfiguration", this.dataContext.MediaConfigurations.Find(entity.MediaConfiguration.Id));
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class UpdateGroupQueryableRepositoryFactory
    {
        static UpdateGroupQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static UpdateGroupQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<UpdateGroup> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUpdateGroupQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(UpdateGroupQueryableRepositoryFactory instance)
        {
            UpdateGroupQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUpdateGroupQueryableRepositoryFactory : UpdateGroupQueryableRepositoryFactory
        {
            static DefaultUpdateGroupQueryableRepositoryFactory()
            {
                Instance = new DefaultUpdateGroupQueryableRepositoryFactory();
            }

            public static UpdateGroupQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<UpdateGroup> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UpdateGroupRepository(dataContext);
            }
        }
    }
        
    public abstract class UpdateGroupRepositoryFactory
    {
        static UpdateGroupRepositoryFactory()
        {
            ResetInstance();
        }

        public static UpdateGroupRepositoryFactory Current { get; private set; }

        public abstract IRepository<UpdateGroup> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUpdateGroupRepositoryFactory.Instance);
        }

        public static void SetInstance(UpdateGroupRepositoryFactory instance)
        {
            UpdateGroupRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUpdateGroupRepositoryFactory : UpdateGroupRepositoryFactory
        {
            static DefaultUpdateGroupRepositoryFactory()
            {
                Instance = new DefaultUpdateGroupRepositoryFactory();
            }

            public static UpdateGroupRepositoryFactory Instance { get; private set; }

            public override IRepository<UpdateGroup> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UpdateGroupRepository(dataContext);
            }
        }
    }
    
    public partial class UpdatePartQueryableRepository : RepositoryBase, IQueryableRepository<UpdatePart>
    {
        protected readonly CenterDataContext dataContext;

        public UpdatePartQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(UpdatePart entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<UpdatePart> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.UpdateParts.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<UpdatePart> Query()
        {
            return this.dataContext.UpdateParts.AsNoTracking();
        }
    }

    public partial class UpdatePartRepository : UpdatePartQueryableRepository, IRepository<UpdatePart>
    {
        public UpdatePartRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<UpdatePart> AddAsync(UpdatePart entity)
        {
            if (entity.UpdateGroup == null)
            {
                throw new ArgumentException("Property 'UpdateGroup' can't be null");
            }

            entity.UpdateGroup = this.dataContext.UpdateGroups.Find(entity.UpdateGroup.Id);
            if (entity.RelatedCommands != null)
            {
                entity.RelatedCommands = entity.RelatedCommands.Select(i => this.dataContext.UpdateCommands.Find(i.Id)).ToList();
            }

            this.dataContext.UpdateParts.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.UpdateParts.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(UpdatePart entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.UpdateParts.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.LoadReference(original, "Structure");
            var originalStructure = original.Structure;
            this.dataContext.LoadReference(original, "InstallInstructions");
            var originalInstallInstructions = original.InstallInstructions;
            this.dataContext.LoadReference(original, "DynamicContent");
            var originalDynamicContent = original.DynamicContent;
            this.dataContext.UpdateParts.Remove(original);
            
            if (originalStructure != null)
            {
                this.dataContext.XmlData.Remove(originalStructure);
            }

            
            if (originalInstallInstructions != null)
            {
                this.dataContext.XmlData.Remove(originalInstallInstructions);
            }

            
            if (originalDynamicContent != null)
            {
                this.dataContext.XmlData.Remove(originalDynamicContent);
            }

            this.dataContext.SaveChanges();
        }

        public async Task<UpdatePart> UpdateAsync(UpdatePart entity)
        {
            var original = this.dataContext.UpdateParts.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'UpdatePart' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            if (entity.UpdateGroup == null)
            {
                this.dataContext.LoadReference(original, "UpdateGroup");
            }
            else if (entity.UpdateGroup == UpdateGroup.Null)
            {
                this.dataContext.SetReference(original, "UpdateGroup", null);
            }
            else
            {
                this.dataContext.SetReference(original, "UpdateGroup", this.dataContext.UpdateGroups.Find(entity.UpdateGroup.Id));
            }
            
            this.dataContext.LoadReference(original, "Structure");
            if (entity.Structure != null)
            {
                if (original.Structure == null)
                {
                    original.Structure = entity.Structure;
                }
                else
                {
                    entity.Structure.Id = original.Structure.Id;
                    this.dataContext.SetValues(original.Structure, entity.Structure);
                }
            }
            
            this.dataContext.LoadReference(original, "InstallInstructions");
            if (entity.InstallInstructions != null)
            {
                if (original.InstallInstructions == null)
                {
                    original.InstallInstructions = entity.InstallInstructions;
                }
                else
                {
                    entity.InstallInstructions.Id = original.InstallInstructions.Id;
                    this.dataContext.SetValues(original.InstallInstructions, entity.InstallInstructions);
                }
            }
            
            this.dataContext.LoadReference(original, "DynamicContent");
            if (entity.DynamicContent != null)
            {
                if (original.DynamicContent == null)
                {
                    original.DynamicContent = entity.DynamicContent;
                }
                else
                {
                    entity.DynamicContent.Id = original.DynamicContent.Id;
                    this.dataContext.SetValues(original.DynamicContent, entity.DynamicContent);
                }
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class UpdatePartQueryableRepositoryFactory
    {
        static UpdatePartQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static UpdatePartQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<UpdatePart> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUpdatePartQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(UpdatePartQueryableRepositoryFactory instance)
        {
            UpdatePartQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUpdatePartQueryableRepositoryFactory : UpdatePartQueryableRepositoryFactory
        {
            static DefaultUpdatePartQueryableRepositoryFactory()
            {
                Instance = new DefaultUpdatePartQueryableRepositoryFactory();
            }

            public static UpdatePartQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<UpdatePart> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UpdatePartRepository(dataContext);
            }
        }
    }
        
    public abstract class UpdatePartRepositoryFactory
    {
        static UpdatePartRepositoryFactory()
        {
            ResetInstance();
        }

        public static UpdatePartRepositoryFactory Current { get; private set; }

        public abstract IRepository<UpdatePart> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUpdatePartRepositoryFactory.Instance);
        }

        public static void SetInstance(UpdatePartRepositoryFactory instance)
        {
            UpdatePartRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUpdatePartRepositoryFactory : UpdatePartRepositoryFactory
        {
            static DefaultUpdatePartRepositoryFactory()
            {
                Instance = new DefaultUpdatePartRepositoryFactory();
            }

            public static UpdatePartRepositoryFactory Instance { get; private set; }

            public override IRepository<UpdatePart> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UpdatePartRepository(dataContext);
            }
        }
    }
    
    public partial class UpdateCommandQueryableRepository : RepositoryBase, IQueryableRepository<UpdateCommand>
    {
        protected readonly CenterDataContext dataContext;

        public UpdateCommandQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(UpdateCommand entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<UpdateCommand> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.UpdateCommands.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<UpdateCommand> Query()
        {
            return this.dataContext.UpdateCommands.AsNoTracking();
        }
    }

    public partial class UpdateCommandRepository : UpdateCommandQueryableRepository, IRepository<UpdateCommand>
    {
        public UpdateCommandRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<UpdateCommand> AddAsync(UpdateCommand entity)
        {
            if (entity.Unit == null)
            {
                throw new ArgumentException("Property 'Unit' can't be null");
            }

            entity.Unit = this.dataContext.Units.Find(entity.Unit.Id);
            if (entity.Feedbacks != null)
            {
                entity.Feedbacks = entity.Feedbacks.Select(i => this.dataContext.UpdateFeedbacks.Find(i.Id)).ToList();
            }

            if (entity.IncludedParts != null)
            {
                entity.IncludedParts = entity.IncludedParts.Select(i => this.dataContext.UpdateParts.Find(i.Id)).ToList();
            }

            this.dataContext.UpdateCommands.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.UpdateCommands.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(UpdateCommand entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.UpdateCommands.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.LoadReference(original, "Command");
            var originalCommand = original.Command;
            this.dataContext.UpdateCommands.Remove(original);
            
            if (originalCommand != null)
            {
                this.dataContext.XmlData.Remove(originalCommand);
            }

            this.dataContext.SaveChanges();
        }

        public async Task<UpdateCommand> UpdateAsync(UpdateCommand entity)
        {
            var original = this.dataContext.UpdateCommands.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'UpdateCommand' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            if (entity.Unit == null)
            {
                this.dataContext.LoadReference(original, "Unit");
            }
            else if (entity.Unit == Unit.Null)
            {
                this.dataContext.SetReference(original, "Unit", null);
            }
            else
            {
                this.dataContext.SetReference(original, "Unit", this.dataContext.Units.Find(entity.Unit.Id));
            }
            
            this.dataContext.LoadReference(original, "Command");
            if (entity.Command != null)
            {
                if (original.Command == null)
                {
                    original.Command = entity.Command;
                }
                else
                {
                    entity.Command.Id = original.Command.Id;
                    this.dataContext.SetValues(original.Command, entity.Command);
                }
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class UpdateCommandQueryableRepositoryFactory
    {
        static UpdateCommandQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static UpdateCommandQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<UpdateCommand> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUpdateCommandQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(UpdateCommandQueryableRepositoryFactory instance)
        {
            UpdateCommandQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUpdateCommandQueryableRepositoryFactory : UpdateCommandQueryableRepositoryFactory
        {
            static DefaultUpdateCommandQueryableRepositoryFactory()
            {
                Instance = new DefaultUpdateCommandQueryableRepositoryFactory();
            }

            public static UpdateCommandQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<UpdateCommand> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UpdateCommandRepository(dataContext);
            }
        }
    }
        
    public abstract class UpdateCommandRepositoryFactory
    {
        static UpdateCommandRepositoryFactory()
        {
            ResetInstance();
        }

        public static UpdateCommandRepositoryFactory Current { get; private set; }

        public abstract IRepository<UpdateCommand> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUpdateCommandRepositoryFactory.Instance);
        }

        public static void SetInstance(UpdateCommandRepositoryFactory instance)
        {
            UpdateCommandRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUpdateCommandRepositoryFactory : UpdateCommandRepositoryFactory
        {
            static DefaultUpdateCommandRepositoryFactory()
            {
                Instance = new DefaultUpdateCommandRepositoryFactory();
            }

            public static UpdateCommandRepositoryFactory Instance { get; private set; }

            public override IRepository<UpdateCommand> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UpdateCommandRepository(dataContext);
            }
        }
    }
    
    public partial class UpdateFeedbackQueryableRepository : RepositoryBase, IQueryableRepository<UpdateFeedback>
    {
        protected readonly CenterDataContext dataContext;

        public UpdateFeedbackQueryableRepository(CenterDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataContext.Dispose();
            }
        }

        private void Detach(UpdateFeedback entity)
        {
            var entry = this.dataContext.Entry(entity);
            entry.State = EntityState.Unchanged;
        }

        public async Task<UpdateFeedback> FindAsync(params object[] keyValues)
        {
            var entity = this.dataContext.UpdateFeedbacks.Find(keyValues);
            if (entity == null)
            {
                return null;
            }

            var clone = entity.Clone();
            var result = await Task.FromResult(clone);
            return result;
        }

        public IQueryable<UpdateFeedback> Query()
        {
            return this.dataContext.UpdateFeedbacks.AsNoTracking();
        }
    }

    public partial class UpdateFeedbackRepository : UpdateFeedbackQueryableRepository, IRepository<UpdateFeedback>
    {
        public UpdateFeedbackRepository(CenterDataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<UpdateFeedback> AddAsync(UpdateFeedback entity)
        {
            if (entity.UpdateCommand == null)
            {
                throw new ArgumentException("Property 'UpdateCommand' can't be null");
            }

            entity.UpdateCommand = this.dataContext.UpdateCommands.Find(entity.UpdateCommand.Id);
            this.dataContext.UpdateFeedbacks.Add(entity);

            await this.dataContext.SaveChangesAsync();
            var added = this.dataContext.UpdateFeedbacks.Find(entity.Id);
            return added;
        }

        public async Task RemoveAsync(UpdateFeedback entity)
        {
            await Task.FromResult(0);
            var original = this.dataContext.UpdateFeedbacks.SingleOrDefault(e => e.Id == entity.Id);
            if (original == null)
            {
                return;
            }

            this.dataContext.LoadReference(original, "Feedback");
            var originalFeedback = original.Feedback;
            this.dataContext.UpdateFeedbacks.Remove(original);
            
            if (originalFeedback != null)
            {
                this.dataContext.XmlData.Remove(originalFeedback);
            }

            this.dataContext.SaveChanges();
        }

        public async Task<UpdateFeedback> UpdateAsync(UpdateFeedback entity)
        {
            var original = this.dataContext.UpdateFeedbacks.Find(entity.Id);
            if (original == null)
            {
                throw new DataException("Couldn't find an entity 'UpdateFeedback' with Id " + entity.Id);
            }

            this.dataContext.SetValues(original, entity);

            if (entity.UpdateCommand == null)
            {
                this.dataContext.LoadReference(original, "UpdateCommand");
            }
            else if (entity.UpdateCommand == UpdateCommand.Null)
            {
                this.dataContext.SetReference(original, "UpdateCommand", null);
            }
            else
            {
                this.dataContext.SetReference(original, "UpdateCommand", this.dataContext.UpdateCommands.Find(entity.UpdateCommand.Id));
            }
            
            this.dataContext.LoadReference(original, "Feedback");
            if (entity.Feedback != null)
            {
                if (original.Feedback == null)
                {
                    original.Feedback = entity.Feedback;
                }
                else
                {
                    entity.Feedback.Id = original.Feedback.Id;
                    this.dataContext.SetValues(original.Feedback, entity.Feedback);
                }
            }

            await this.dataContext.SaveChangesAsync();
            return original;
        }
    }
        
    public abstract class UpdateFeedbackQueryableRepositoryFactory
    {
        static UpdateFeedbackQueryableRepositoryFactory()
        {
            ResetInstance();
        }

        public static UpdateFeedbackQueryableRepositoryFactory Current { get; private set; }

        public abstract IQueryableRepository<UpdateFeedback> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUpdateFeedbackQueryableRepositoryFactory.Instance);
        }

        public static void SetInstance(UpdateFeedbackQueryableRepositoryFactory instance)
        {
            UpdateFeedbackQueryableRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUpdateFeedbackQueryableRepositoryFactory : UpdateFeedbackQueryableRepositoryFactory
        {
            static DefaultUpdateFeedbackQueryableRepositoryFactory()
            {
                Instance = new DefaultUpdateFeedbackQueryableRepositoryFactory();
            }

            public static UpdateFeedbackQueryableRepositoryFactory Instance { get; private set; }

            public override IQueryableRepository<UpdateFeedback> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UpdateFeedbackRepository(dataContext);
            }
        }
    }
        
    public abstract class UpdateFeedbackRepositoryFactory
    {
        static UpdateFeedbackRepositoryFactory()
        {
            ResetInstance();
        }

        public static UpdateFeedbackRepositoryFactory Current { get; private set; }

        public abstract IRepository<UpdateFeedback> Create();

        public static void ResetInstance()
        {
            SetInstance(DefaultUpdateFeedbackRepositoryFactory.Instance);
        }

        public static void SetInstance(UpdateFeedbackRepositoryFactory instance)
        {
            UpdateFeedbackRepositoryFactory.Current = instance;
        }

        private sealed class DefaultUpdateFeedbackRepositoryFactory : UpdateFeedbackRepositoryFactory
        {
            static DefaultUpdateFeedbackRepositoryFactory()
            {
                Instance = new DefaultUpdateFeedbackRepositoryFactory();
            }

            public static UpdateFeedbackRepositoryFactory Instance { get; private set; }

            public override IRepository<UpdateFeedback> Create()
            {
                var dataContext = DataContextFactory.Current.Create();
                return new UpdateFeedbackRepository(dataContext);
            }
        }
    }
    }

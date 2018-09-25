namespace Gorba.Center.BackgroundSystem.Core
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Threading.Tasks;
    using System.Xml;
    
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model;
    using Gorba.Center.BackgroundSystem.Data.Model.AccessControl;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
    using Gorba.Center.BackgroundSystem.Data.Model.Configurations;
    using Gorba.Center.Common.ServiceModel.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.BackgroundSystem.Data.Model.Documents;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.BackgroundSystem.Data.Model.Log;
    using Gorba.Center.Common.ServiceModel.Log;
    using Gorba.Center.Common.ServiceModel.Filters.Log;
    using Gorba.Center.BackgroundSystem.Data.Model.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.BackgroundSystem.Data.Model.Meta;
    using Gorba.Center.Common.ServiceModel.Meta;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.BackgroundSystem.Data.Model.Resources;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.BackgroundSystem.Data.Model.Software;
    using Gorba.Center.Common.ServiceModel.Software;
    using Gorba.Center.Common.ServiceModel.Filters.Software;
    using Gorba.Center.BackgroundSystem.Data.Model.Units;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.BackgroundSystem.Data.Model.Update;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Common.Utility.Core;

    using NLog;


    public class AuthorizationDataService : IAuthorizationDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.AccessControl.Authorization> AddAsync(Gorba.Center.Common.ServiceModel.AccessControl.Authorization entity)
        {
            using (var repository = AuthorizationRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'Authorization' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.AccessControl.Authorization entity)
        {
            using (var repository = AuthorizationRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'Authorization' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.AccessControl.Authorization> GetAsync(int id)
        {
            using (var repository = AuthorizationRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("Authorization with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'Authorization' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.AccessControl.Authorization>> QueryAsync(AuthorizationQuery query = null)
        {
            using (var repository = AuthorizationRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'Authorization'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(AuthorizationQuery query = null)
        {
            using (var repository = AuthorizationRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'Authorization'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.AccessControl.Authorization> UpdateAsync(Gorba.Center.Common.ServiceModel.AccessControl.Authorization entity)
        {
            using (var repository = AuthorizationRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'Authorization' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class UserRoleDataService : IUserRoleDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.AccessControl.UserRole> AddAsync(Gorba.Center.Common.ServiceModel.AccessControl.UserRole entity)
        {
            using (var repository = UserRoleRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'UserRole' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.AccessControl.UserRole entity)
        {
            using (var repository = UserRoleRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'UserRole' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.AccessControl.UserRole> GetAsync(int id)
        {
            using (var repository = UserRoleRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("UserRole with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'UserRole' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.AccessControl.UserRole>> QueryAsync(UserRoleQuery query = null)
        {
            using (var repository = UserRoleRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'UserRole'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(UserRoleQuery query = null)
        {
            using (var repository = UserRoleRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'UserRole'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.AccessControl.UserRole> UpdateAsync(Gorba.Center.Common.ServiceModel.AccessControl.UserRole entity)
        {
            using (var repository = UserRoleRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'UserRole' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class UnitConfigurationDataService : IUnitConfigurationDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration> AddAsync(Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration entity)
        {
            using (var repository = UnitConfigurationRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'UnitConfiguration' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration entity)
        {
            using (var repository = UnitConfigurationRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'UnitConfiguration' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration> GetAsync(int id)
        {
            using (var repository = UnitConfigurationRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("UnitConfiguration with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'UnitConfiguration' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration>> QueryAsync(UnitConfigurationQuery query = null)
        {
            using (var repository = UnitConfigurationRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'UnitConfiguration'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(UnitConfigurationQuery query = null)
        {
            using (var repository = UnitConfigurationRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'UnitConfiguration'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration> UpdateAsync(Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration entity)
        {
            using (var repository = UnitConfigurationRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'UnitConfiguration' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class MediaConfigurationDataService : IMediaConfigurationDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration> AddAsync(Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration entity)
        {
            using (var repository = MediaConfigurationRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'MediaConfiguration' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration entity)
        {
            using (var repository = MediaConfigurationRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'MediaConfiguration' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration> GetAsync(int id)
        {
            using (var repository = MediaConfigurationRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("MediaConfiguration with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'MediaConfiguration' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration>> QueryAsync(MediaConfigurationQuery query = null)
        {
            using (var repository = MediaConfigurationRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'MediaConfiguration'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(MediaConfigurationQuery query = null)
        {
            using (var repository = MediaConfigurationRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'MediaConfiguration'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration> UpdateAsync(Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration entity)
        {
            using (var repository = MediaConfigurationRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'MediaConfiguration' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class DocumentDataService : IDocumentDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Documents.Document> AddAsync(Gorba.Center.Common.ServiceModel.Documents.Document entity)
        {
            using (var repository = DocumentRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'Document' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Documents.Document entity)
        {
            using (var repository = DocumentRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'Document' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Documents.Document> GetAsync(int id)
        {
            using (var repository = DocumentRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("Document with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'Document' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Documents.Document>> QueryAsync(DocumentQuery query = null)
        {
            using (var repository = DocumentRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'Document'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(DocumentQuery query = null)
        {
            using (var repository = DocumentRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'Document'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Documents.Document> UpdateAsync(Gorba.Center.Common.ServiceModel.Documents.Document entity)
        {
            using (var repository = DocumentRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'Document' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class DocumentVersionDataService : IDocumentVersionDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Documents.DocumentVersion> AddAsync(Gorba.Center.Common.ServiceModel.Documents.DocumentVersion entity)
        {
            using (var repository = DocumentVersionRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'DocumentVersion' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Documents.DocumentVersion entity)
        {
            using (var repository = DocumentVersionRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'DocumentVersion' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Documents.DocumentVersion> GetAsync(int id)
        {
            using (var repository = DocumentVersionRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("DocumentVersion with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'DocumentVersion' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Documents.DocumentVersion>> QueryAsync(DocumentVersionQuery query = null)
        {
            using (var repository = DocumentVersionRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'DocumentVersion'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(DocumentVersionQuery query = null)
        {
            using (var repository = DocumentVersionRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'DocumentVersion'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Documents.DocumentVersion> UpdateAsync(Gorba.Center.Common.ServiceModel.Documents.DocumentVersion entity)
        {
            using (var repository = DocumentVersionRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'DocumentVersion' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class LogEntryDataService : ILogEntryDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Log.LogEntry> AddAsync(Gorba.Center.Common.ServiceModel.Log.LogEntry entity)
        {
            using (var repository = LogEntryRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'LogEntry' added");
                return result.ToDto();
            }
        }

        public async Task AddRangeAsync(IEnumerable<Gorba.Center.Common.ServiceModel.Log.LogEntry> entities)
        {
            using (var repository = LogEntryRepositoryFactory.Current.Create())
            {
                var databaseEntities = entities.Select(entity => entity.ToDatabase());
                await repository.AddRangeAsync(databaseEntities);
            }
        }

        public async Task<int> DeleteAsync(LogEntryFilter filter)
        {
            using (var repository = LogEntryRepositoryFactory.Current.Create())
            {
                return await repository.DeleteAsync(filter);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Log.LogEntry entity)
        {
            using (var repository = LogEntryRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'LogEntry' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Log.LogEntry> GetAsync(int id)
        {
            using (var repository = LogEntryRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("LogEntry with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'LogEntry' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Log.LogEntry>> QueryAsync(LogEntryQuery query = null)
        {
            using (var repository = LogEntryRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'LogEntry'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(LogEntryQuery query = null)
        {
            using (var repository = LogEntryRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'LogEntry'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Log.LogEntry> UpdateAsync(Gorba.Center.Common.ServiceModel.Log.LogEntry entity)
        {
            using (var repository = LogEntryRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'LogEntry' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class TenantDataService : ITenantDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Membership.Tenant> AddAsync(Gorba.Center.Common.ServiceModel.Membership.Tenant entity)
        {
            using (var repository = TenantRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'Tenant' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Membership.Tenant entity)
        {
            using (var repository = TenantRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'Tenant' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.Tenant> GetAsync(int id)
        {
            using (var repository = TenantRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("Tenant with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'Tenant' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Membership.Tenant>> QueryAsync(TenantQuery query = null)
        {
            using (var repository = TenantRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'Tenant'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(TenantQuery query = null)
        {
            using (var repository = TenantRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'Tenant'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.Tenant> UpdateAsync(Gorba.Center.Common.ServiceModel.Membership.Tenant entity)
        {
            using (var repository = TenantRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'Tenant' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class UserDataService : IUserDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Membership.User> AddAsync(Gorba.Center.Common.ServiceModel.Membership.User entity)
        {
            using (var repository = UserRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'User' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Membership.User entity)
        {
            using (var repository = UserRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'User' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.User> GetAsync(int id)
        {
            using (var repository = UserRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("User with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'User' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Membership.User>> QueryAsync(UserQuery query = null)
        {
            using (var repository = UserRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'User'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(UserQuery query = null)
        {
            using (var repository = UserRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'User'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.User> UpdateAsync(Gorba.Center.Common.ServiceModel.Membership.User entity)
        {
            using (var repository = UserRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'User' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class AssociationTenantUserUserRoleDataService : IAssociationTenantUserUserRoleDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole> AddAsync(Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole entity)
        {
            using (var repository = AssociationTenantUserUserRoleRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'AssociationTenantUserUserRole' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole entity)
        {
            using (var repository = AssociationTenantUserUserRoleRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'AssociationTenantUserUserRole' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole> GetAsync(int id)
        {
            using (var repository = AssociationTenantUserUserRoleRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("AssociationTenantUserUserRole with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'AssociationTenantUserUserRole' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole>> QueryAsync(AssociationTenantUserUserRoleQuery query = null)
        {
            using (var repository = AssociationTenantUserUserRoleRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'AssociationTenantUserUserRole'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(AssociationTenantUserUserRoleQuery query = null)
        {
            using (var repository = AssociationTenantUserUserRoleRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'AssociationTenantUserUserRole'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole> UpdateAsync(Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole entity)
        {
            using (var repository = AssociationTenantUserUserRoleRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'AssociationTenantUserUserRole' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class UserDefinedPropertyDataService : IUserDefinedPropertyDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty> AddAsync(Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty entity)
        {
            using (var repository = UserDefinedPropertyRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'UserDefinedProperty' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty entity)
        {
            using (var repository = UserDefinedPropertyRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'UserDefinedProperty' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty> GetAsync(int id)
        {
            using (var repository = UserDefinedPropertyRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("UserDefinedProperty with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'UserDefinedProperty' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty>> QueryAsync(UserDefinedPropertyQuery query = null)
        {
            using (var repository = UserDefinedPropertyRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'UserDefinedProperty'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(UserDefinedPropertyQuery query = null)
        {
            using (var repository = UserDefinedPropertyRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'UserDefinedProperty'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty> UpdateAsync(Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty entity)
        {
            using (var repository = UserDefinedPropertyRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'UserDefinedProperty' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class SystemConfigDataService : ISystemConfigDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Meta.SystemConfig> AddAsync(Gorba.Center.Common.ServiceModel.Meta.SystemConfig entity)
        {
            using (var repository = SystemConfigRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'SystemConfig' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Meta.SystemConfig entity)
        {
            using (var repository = SystemConfigRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'SystemConfig' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Meta.SystemConfig> GetAsync(int id)
        {
            using (var repository = SystemConfigRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("SystemConfig with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'SystemConfig' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Meta.SystemConfig>> QueryAsync(SystemConfigQuery query = null)
        {
            using (var repository = SystemConfigRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'SystemConfig'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(SystemConfigQuery query = null)
        {
            using (var repository = SystemConfigRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'SystemConfig'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Meta.SystemConfig> UpdateAsync(Gorba.Center.Common.ServiceModel.Meta.SystemConfig entity)
        {
            using (var repository = SystemConfigRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'SystemConfig' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class ResourceDataService : IResourceDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Resources.Resource> AddAsync(Gorba.Center.Common.ServiceModel.Resources.Resource entity)
        {
            using (var repository = ResourceRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'Resource' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Resources.Resource entity)
        {
            using (var repository = ResourceRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'Resource' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Resources.Resource> GetAsync(int id)
        {
            using (var repository = ResourceRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("Resource with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'Resource' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Resources.Resource>> QueryAsync(ResourceQuery query = null)
        {
            using (var repository = ResourceRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'Resource'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(ResourceQuery query = null)
        {
            using (var repository = ResourceRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'Resource'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Resources.Resource> UpdateAsync(Gorba.Center.Common.ServiceModel.Resources.Resource entity)
        {
            using (var repository = ResourceRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'Resource' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class ContentResourceDataService : IContentResourceDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Resources.ContentResource> AddAsync(Gorba.Center.Common.ServiceModel.Resources.ContentResource entity)
        {
            using (var repository = ContentResourceRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'ContentResource' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Resources.ContentResource entity)
        {
            using (var repository = ContentResourceRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'ContentResource' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Resources.ContentResource> GetAsync(int id)
        {
            using (var repository = ContentResourceRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("ContentResource with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'ContentResource' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Resources.ContentResource>> QueryAsync(ContentResourceQuery query = null)
        {
            using (var repository = ContentResourceRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'ContentResource'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(ContentResourceQuery query = null)
        {
            using (var repository = ContentResourceRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'ContentResource'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Resources.ContentResource> UpdateAsync(Gorba.Center.Common.ServiceModel.Resources.ContentResource entity)
        {
            using (var repository = ContentResourceRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'ContentResource' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class PackageDataService : IPackageDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Software.Package> AddAsync(Gorba.Center.Common.ServiceModel.Software.Package entity)
        {
            using (var repository = PackageRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'Package' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Software.Package entity)
        {
            using (var repository = PackageRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'Package' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Software.Package> GetAsync(int id)
        {
            using (var repository = PackageRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("Package with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'Package' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Software.Package>> QueryAsync(PackageQuery query = null)
        {
            using (var repository = PackageRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'Package'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(PackageQuery query = null)
        {
            using (var repository = PackageRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'Package'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Software.Package> UpdateAsync(Gorba.Center.Common.ServiceModel.Software.Package entity)
        {
            using (var repository = PackageRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'Package' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class PackageVersionDataService : IPackageVersionDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Software.PackageVersion> AddAsync(Gorba.Center.Common.ServiceModel.Software.PackageVersion entity)
        {
            using (var repository = PackageVersionRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'PackageVersion' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Software.PackageVersion entity)
        {
            using (var repository = PackageVersionRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'PackageVersion' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Software.PackageVersion> GetAsync(int id)
        {
            using (var repository = PackageVersionRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("PackageVersion with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'PackageVersion' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Software.PackageVersion>> QueryAsync(PackageVersionQuery query = null)
        {
            using (var repository = PackageVersionRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'PackageVersion'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(PackageVersionQuery query = null)
        {
            using (var repository = PackageVersionRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'PackageVersion'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Software.PackageVersion> UpdateAsync(Gorba.Center.Common.ServiceModel.Software.PackageVersion entity)
        {
            using (var repository = PackageVersionRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'PackageVersion' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class ProductTypeDataService : IProductTypeDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Units.ProductType> AddAsync(Gorba.Center.Common.ServiceModel.Units.ProductType entity)
        {
            using (var repository = ProductTypeRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'ProductType' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Units.ProductType entity)
        {
            using (var repository = ProductTypeRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'ProductType' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Units.ProductType> GetAsync(int id)
        {
            using (var repository = ProductTypeRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("ProductType with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'ProductType' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Units.ProductType>> QueryAsync(ProductTypeQuery query = null)
        {
            using (var repository = ProductTypeRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'ProductType'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(ProductTypeQuery query = null)
        {
            using (var repository = ProductTypeRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'ProductType'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Units.ProductType> UpdateAsync(Gorba.Center.Common.ServiceModel.Units.ProductType entity)
        {
            using (var repository = ProductTypeRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'ProductType' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class UnitDataService : IUnitDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Units.Unit> AddAsync(Gorba.Center.Common.ServiceModel.Units.Unit entity)
        {
            using (var repository = UnitRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'Unit' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Units.Unit entity)
        {
            using (var repository = UnitRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'Unit' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Units.Unit> GetAsync(int id)
        {
            using (var repository = UnitRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("Unit with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'Unit' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Units.Unit>> QueryAsync(UnitQuery query = null)
        {
            using (var repository = UnitRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'Unit'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(UnitQuery query = null)
        {
            using (var repository = UnitRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'Unit'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Units.Unit> UpdateAsync(Gorba.Center.Common.ServiceModel.Units.Unit entity)
        {
            using (var repository = UnitRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'Unit' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class UpdateGroupDataService : IUpdateGroupDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateGroup> AddAsync(Gorba.Center.Common.ServiceModel.Update.UpdateGroup entity)
        {
            using (var repository = UpdateGroupRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'UpdateGroup' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Update.UpdateGroup entity)
        {
            using (var repository = UpdateGroupRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'UpdateGroup' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateGroup> GetAsync(int id)
        {
            using (var repository = UpdateGroupRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("UpdateGroup with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'UpdateGroup' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Update.UpdateGroup>> QueryAsync(UpdateGroupQuery query = null)
        {
            using (var repository = UpdateGroupRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'UpdateGroup'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(UpdateGroupQuery query = null)
        {
            using (var repository = UpdateGroupRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'UpdateGroup'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateGroup> UpdateAsync(Gorba.Center.Common.ServiceModel.Update.UpdateGroup entity)
        {
            using (var repository = UpdateGroupRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'UpdateGroup' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class UpdatePartDataService : IUpdatePartDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdatePart> AddAsync(Gorba.Center.Common.ServiceModel.Update.UpdatePart entity)
        {
            using (var repository = UpdatePartRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'UpdatePart' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Update.UpdatePart entity)
        {
            using (var repository = UpdatePartRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'UpdatePart' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdatePart> GetAsync(int id)
        {
            using (var repository = UpdatePartRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("UpdatePart with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'UpdatePart' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Update.UpdatePart>> QueryAsync(UpdatePartQuery query = null)
        {
            using (var repository = UpdatePartRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'UpdatePart'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(UpdatePartQuery query = null)
        {
            using (var repository = UpdatePartRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'UpdatePart'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdatePart> UpdateAsync(Gorba.Center.Common.ServiceModel.Update.UpdatePart entity)
        {
            using (var repository = UpdatePartRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'UpdatePart' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class UpdateCommandDataService : IUpdateCommandDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateCommand> AddAsync(Gorba.Center.Common.ServiceModel.Update.UpdateCommand entity)
        {
            using (var repository = UpdateCommandRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'UpdateCommand' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Update.UpdateCommand entity)
        {
            using (var repository = UpdateCommandRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'UpdateCommand' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateCommand> GetAsync(int id)
        {
            using (var repository = UpdateCommandRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("UpdateCommand with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'UpdateCommand' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Update.UpdateCommand>> QueryAsync(UpdateCommandQuery query = null)
        {
            using (var repository = UpdateCommandRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'UpdateCommand'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(UpdateCommandQuery query = null)
        {
            using (var repository = UpdateCommandRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'UpdateCommand'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateCommand> UpdateAsync(Gorba.Center.Common.ServiceModel.Update.UpdateCommand entity)
        {
            using (var repository = UpdateCommandRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'UpdateCommand' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }

    public class UpdateFeedbackDataService : IUpdateFeedbackDataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateFeedback> AddAsync(Gorba.Center.Common.ServiceModel.Update.UpdateFeedback entity)
        {
            using (var repository = UpdateFeedbackRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.CreatedOn = TimeProvider.Current.UtcNow;
                var result = await repository.AddAsync(databaseEntity);
                Logger.Trace("Entity 'UpdateFeedback' added");
                return result.ToDto();
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Update.UpdateFeedback entity)
        {
            using (var repository = UpdateFeedbackRepositoryFactory.Current.Create())
            {
                var databaseEntity = await repository.FindAsync(entity.Id);
                if (databaseEntity == null)
                {
                    throw new DataException("Entity was not found in the database");
                }

                await repository.RemoveAsync(databaseEntity);
                Logger.Trace("Entity 'UpdateFeedback' deleted");
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateFeedback> GetAsync(int id)
        {
            using (var repository = UpdateFeedbackRepositoryFactory.Current.Create())
            {
                var entity = await repository.FindAsync(id);
                if (entity == null)
                {
                    throw new DataException(string.Format("UpdateFeedback with Id '{0}' not found", id));
                }

                Logger.Trace("Returning entity 'UpdateFeedback' with Id '{0}'", id);
                return entity.ToDto();
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Update.UpdateFeedback>> QueryAsync(UpdateFeedbackQuery query = null)
        {
            using (var repository = UpdateFeedbackRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var entities = await repoQuery.ToListAsync();
                var list = entities.Select(entity => entity.ToDto(query)).ToList();
                Logger.Trace("Returning {0} result(s) for entity 'UpdateFeedback'", list.Count);
                return list;
            }
        }

        public async Task<int> CountAsync(UpdateFeedbackQuery query = null)
        {
            using (var repository = UpdateFeedbackRepositoryFactory.Current.Create())
            {
                var repoQuery = repository.Query().Apply(query);

                var count = await repoQuery.CountAsync();
                Logger.Trace("Returning count {0} for entity 'UpdateFeedback'", count);
                return count;
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateFeedback> UpdateAsync(Gorba.Center.Common.ServiceModel.Update.UpdateFeedback entity)
        {
            using (var repository = UpdateFeedbackRepositoryFactory.Current.Create())
            {
                var databaseEntity = entity.ToDatabase();
                databaseEntity.LastModifiedOn = TimeProvider.Current.UtcNow;
                var result = await repository.UpdateAsync(databaseEntity);
                Logger.Trace("Entity 'UpdateFeedback' with Id '{0}' updated", entity.Id);
                return result.ToDto();
            }
        }
    }
}

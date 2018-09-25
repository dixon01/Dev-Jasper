namespace Gorba.Center.BackgroundSystem.Core
{
	using System;
	using System.Collections.Generic;
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

	using NLog;


	public sealed class AuthorizationConcurrentDataService : ConcurrentServiceBase, IAuthorizationDataService
	{
		private IAuthorizationDataService dataService;

		public AuthorizationConcurrentDataService(IAuthorizationDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.AccessControl.Authorization> AddAsync(Gorba.Center.Common.ServiceModel.AccessControl.Authorization entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.AccessControl.Authorization entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.AccessControl.Authorization>> QueryAsync(AuthorizationQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(AuthorizationQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.AccessControl.Authorization> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.AccessControl.Authorization> UpdateAsync(Gorba.Center.Common.ServiceModel.AccessControl.Authorization entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class UserRoleConcurrentDataService : ConcurrentServiceBase, IUserRoleDataService
	{
		private IUserRoleDataService dataService;

		public UserRoleConcurrentDataService(IUserRoleDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.AccessControl.UserRole> AddAsync(Gorba.Center.Common.ServiceModel.AccessControl.UserRole entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.AccessControl.UserRole entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.AccessControl.UserRole>> QueryAsync(UserRoleQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(UserRoleQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.AccessControl.UserRole> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.AccessControl.UserRole> UpdateAsync(Gorba.Center.Common.ServiceModel.AccessControl.UserRole entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class UnitConfigurationConcurrentDataService : ConcurrentServiceBase, IUnitConfigurationDataService
	{
		private IUnitConfigurationDataService dataService;

		public UnitConfigurationConcurrentDataService(IUnitConfigurationDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration> AddAsync(Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration>> QueryAsync(UnitConfigurationQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(UnitConfigurationQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration> UpdateAsync(Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class MediaConfigurationConcurrentDataService : ConcurrentServiceBase, IMediaConfigurationDataService
	{
		private IMediaConfigurationDataService dataService;

		public MediaConfigurationConcurrentDataService(IMediaConfigurationDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration> AddAsync(Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration>> QueryAsync(MediaConfigurationQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(MediaConfigurationQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration> UpdateAsync(Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class DocumentConcurrentDataService : ConcurrentServiceBase, IDocumentDataService
	{
		private IDocumentDataService dataService;

		public DocumentConcurrentDataService(IDocumentDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Documents.Document> AddAsync(Gorba.Center.Common.ServiceModel.Documents.Document entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Documents.Document entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Documents.Document>> QueryAsync(DocumentQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(DocumentQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Documents.Document> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Documents.Document> UpdateAsync(Gorba.Center.Common.ServiceModel.Documents.Document entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class DocumentVersionConcurrentDataService : ConcurrentServiceBase, IDocumentVersionDataService
	{
		private IDocumentVersionDataService dataService;

		public DocumentVersionConcurrentDataService(IDocumentVersionDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Documents.DocumentVersion> AddAsync(Gorba.Center.Common.ServiceModel.Documents.DocumentVersion entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Documents.DocumentVersion entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Documents.DocumentVersion>> QueryAsync(DocumentVersionQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(DocumentVersionQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Documents.DocumentVersion> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Documents.DocumentVersion> UpdateAsync(Gorba.Center.Common.ServiceModel.Documents.DocumentVersion entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class LogEntryConcurrentDataService : ConcurrentServiceBase, ILogEntryDataService
	{
		private ILogEntryDataService dataService;

		public LogEntryConcurrentDataService(ILogEntryDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Log.LogEntry> AddAsync(Gorba.Center.Common.ServiceModel.Log.LogEntry entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task AddRangeAsync(IEnumerable<Gorba.Center.Common.ServiceModel.Log.LogEntry> entities)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before AddRange");
				using (await this.AcquireWriterLockAsync())
				{
					var list = entities.ToList();
					this.Logger.Trace("Adding {0} item(s)", list.Count);
					await this.dataService.AddRangeAsync(list);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> DeleteAsync(LogEntryFilter filter)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete(filter)");
				using (await this.AcquireWriterLockAsync())
				{
					return await this.dataService.DeleteAsync(filter);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Log.LogEntry entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Log.LogEntry>> QueryAsync(LogEntryQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(LogEntryQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Log.LogEntry> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Log.LogEntry> UpdateAsync(Gorba.Center.Common.ServiceModel.Log.LogEntry entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class TenantConcurrentDataService : ConcurrentServiceBase, ITenantDataService
	{
		private ITenantDataService dataService;

		public TenantConcurrentDataService(ITenantDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Membership.Tenant> AddAsync(Gorba.Center.Common.ServiceModel.Membership.Tenant entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Membership.Tenant entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Membership.Tenant>> QueryAsync(TenantQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(TenantQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Membership.Tenant> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Membership.Tenant> UpdateAsync(Gorba.Center.Common.ServiceModel.Membership.Tenant entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class UserConcurrentDataService : ConcurrentServiceBase, IUserDataService
	{
		private IUserDataService dataService;

		public UserConcurrentDataService(IUserDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Membership.User> AddAsync(Gorba.Center.Common.ServiceModel.Membership.User entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Membership.User entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Membership.User>> QueryAsync(UserQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(UserQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Membership.User> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Membership.User> UpdateAsync(Gorba.Center.Common.ServiceModel.Membership.User entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class AssociationTenantUserUserRoleConcurrentDataService : ConcurrentServiceBase, IAssociationTenantUserUserRoleDataService
	{
		private IAssociationTenantUserUserRoleDataService dataService;

		public AssociationTenantUserUserRoleConcurrentDataService(IAssociationTenantUserUserRoleDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole> AddAsync(Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole>> QueryAsync(AssociationTenantUserUserRoleQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(AssociationTenantUserUserRoleQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole> UpdateAsync(Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class UserDefinedPropertyConcurrentDataService : ConcurrentServiceBase, IUserDefinedPropertyDataService
	{
		private IUserDefinedPropertyDataService dataService;

		public UserDefinedPropertyConcurrentDataService(IUserDefinedPropertyDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty> AddAsync(Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty>> QueryAsync(UserDefinedPropertyQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(UserDefinedPropertyQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty> UpdateAsync(Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class SystemConfigConcurrentDataService : ConcurrentServiceBase, ISystemConfigDataService
	{
		private ISystemConfigDataService dataService;

		public SystemConfigConcurrentDataService(ISystemConfigDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Meta.SystemConfig> AddAsync(Gorba.Center.Common.ServiceModel.Meta.SystemConfig entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Meta.SystemConfig entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Meta.SystemConfig>> QueryAsync(SystemConfigQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(SystemConfigQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Meta.SystemConfig> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Meta.SystemConfig> UpdateAsync(Gorba.Center.Common.ServiceModel.Meta.SystemConfig entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class ResourceConcurrentDataService : ConcurrentServiceBase, IResourceDataService
	{
		private IResourceDataService dataService;

		public ResourceConcurrentDataService(IResourceDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Resources.Resource> AddAsync(Gorba.Center.Common.ServiceModel.Resources.Resource entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Resources.Resource entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Resources.Resource>> QueryAsync(ResourceQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(ResourceQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Resources.Resource> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Resources.Resource> UpdateAsync(Gorba.Center.Common.ServiceModel.Resources.Resource entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class ContentResourceConcurrentDataService : ConcurrentServiceBase, IContentResourceDataService
	{
		private IContentResourceDataService dataService;

		public ContentResourceConcurrentDataService(IContentResourceDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Resources.ContentResource> AddAsync(Gorba.Center.Common.ServiceModel.Resources.ContentResource entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Resources.ContentResource entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Resources.ContentResource>> QueryAsync(ContentResourceQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(ContentResourceQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Resources.ContentResource> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Resources.ContentResource> UpdateAsync(Gorba.Center.Common.ServiceModel.Resources.ContentResource entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class PackageConcurrentDataService : ConcurrentServiceBase, IPackageDataService
	{
		private IPackageDataService dataService;

		public PackageConcurrentDataService(IPackageDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Software.Package> AddAsync(Gorba.Center.Common.ServiceModel.Software.Package entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Software.Package entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Software.Package>> QueryAsync(PackageQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(PackageQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Software.Package> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Software.Package> UpdateAsync(Gorba.Center.Common.ServiceModel.Software.Package entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class PackageVersionConcurrentDataService : ConcurrentServiceBase, IPackageVersionDataService
	{
		private IPackageVersionDataService dataService;

		public PackageVersionConcurrentDataService(IPackageVersionDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Software.PackageVersion> AddAsync(Gorba.Center.Common.ServiceModel.Software.PackageVersion entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Software.PackageVersion entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Software.PackageVersion>> QueryAsync(PackageVersionQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(PackageVersionQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Software.PackageVersion> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Software.PackageVersion> UpdateAsync(Gorba.Center.Common.ServiceModel.Software.PackageVersion entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class ProductTypeConcurrentDataService : ConcurrentServiceBase, IProductTypeDataService
	{
		private IProductTypeDataService dataService;

		public ProductTypeConcurrentDataService(IProductTypeDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Units.ProductType> AddAsync(Gorba.Center.Common.ServiceModel.Units.ProductType entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Units.ProductType entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Units.ProductType>> QueryAsync(ProductTypeQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(ProductTypeQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Units.ProductType> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Units.ProductType> UpdateAsync(Gorba.Center.Common.ServiceModel.Units.ProductType entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class UnitConcurrentDataService : ConcurrentServiceBase, IUnitDataService
	{
		private IUnitDataService dataService;

		public UnitConcurrentDataService(IUnitDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Units.Unit> AddAsync(Gorba.Center.Common.ServiceModel.Units.Unit entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Units.Unit entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Units.Unit>> QueryAsync(UnitQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(UnitQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Units.Unit> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Units.Unit> UpdateAsync(Gorba.Center.Common.ServiceModel.Units.Unit entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class UpdateGroupConcurrentDataService : ConcurrentServiceBase, IUpdateGroupDataService
	{
		private IUpdateGroupDataService dataService;

		public UpdateGroupConcurrentDataService(IUpdateGroupDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateGroup> AddAsync(Gorba.Center.Common.ServiceModel.Update.UpdateGroup entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Update.UpdateGroup entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Update.UpdateGroup>> QueryAsync(UpdateGroupQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(UpdateGroupQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateGroup> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateGroup> UpdateAsync(Gorba.Center.Common.ServiceModel.Update.UpdateGroup entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class UpdatePartConcurrentDataService : ConcurrentServiceBase, IUpdatePartDataService
	{
		private IUpdatePartDataService dataService;

		public UpdatePartConcurrentDataService(IUpdatePartDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Update.UpdatePart> AddAsync(Gorba.Center.Common.ServiceModel.Update.UpdatePart entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Update.UpdatePart entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Update.UpdatePart>> QueryAsync(UpdatePartQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(UpdatePartQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Update.UpdatePart> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Update.UpdatePart> UpdateAsync(Gorba.Center.Common.ServiceModel.Update.UpdatePart entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class UpdateCommandConcurrentDataService : ConcurrentServiceBase, IUpdateCommandDataService
	{
		private IUpdateCommandDataService dataService;

		public UpdateCommandConcurrentDataService(IUpdateCommandDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateCommand> AddAsync(Gorba.Center.Common.ServiceModel.Update.UpdateCommand entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Update.UpdateCommand entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Update.UpdateCommand>> QueryAsync(UpdateCommandQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(UpdateCommandQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateCommand> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateCommand> UpdateAsync(Gorba.Center.Common.ServiceModel.Update.UpdateCommand entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}

	public sealed class UpdateFeedbackConcurrentDataService : ConcurrentServiceBase, IUpdateFeedbackDataService
	{
		private IUpdateFeedbackDataService dataService;

		public UpdateFeedbackConcurrentDataService(IUpdateFeedbackDataService dataService)
		{
			this.dataService = dataService;
		}

		public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateFeedback> AddAsync(Gorba.Center.Common.ServiceModel.Update.UpdateFeedback entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Add");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Adding entity {0}", entity);
					return await this.dataService.AddAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Update.UpdateFeedback entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Delete");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Deleting entity {0}");
					await this.dataService.DeleteAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Update.UpdateFeedback>> QueryAsync(UpdateFeedbackQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Query");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Querying entities");
					return await this.dataService.QueryAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<int> CountAsync(UpdateFeedbackQuery query = null)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Count");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Counting entities");
					return await this.dataService.CountAsync(query);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateFeedback> GetAsync(int id)
		{
			try
			{
				this.Logger.Trace("Acquiring reader lock before Get");
				using (await this.AcquireReaderLockAsync())
				{
					this.Logger.Trace("Getting entity '{0}'", id);
					return await this.dataService.GetAsync(id);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}

		public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateFeedback> UpdateAsync(Gorba.Center.Common.ServiceModel.Update.UpdateFeedback entity)
		{
			try
			{
				this.Logger.Trace("Acquiring writer lock before Update");
				using (await this.AcquireWriterLockAsync())
				{
					this.Logger.Trace("Updating entity {0}", entity);
					return await this.dataService.UpdateAsync(entity);
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
				throw new FaultException(exception.Message);
			}
		}
	}
}

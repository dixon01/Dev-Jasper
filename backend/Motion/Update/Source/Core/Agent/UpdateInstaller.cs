// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateInstaller.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateInstaller type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.Async;
    using Gorba.Common.Utility.Files.Writable;

    using NLog;

    /// <summary>
    /// Installer for updates.
    /// </summary>
    public class UpdateInstaller
    {
        private const double CreateEngineProgress = 0.25;
        private static readonly Logger Logger = LogHelper.GetLogger<UpdateInstaller>();

        private readonly IWritableDirectoryInfo installationRoot;

        private readonly IProgressMonitor monitor;

        private readonly IResourceService resourceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateInstaller"/> class.
        /// </summary>
        /// <param name="command">
        /// The command to install.
        /// </param>
        /// <param name="installationRoot">
        /// The installation root where to install the update.
        /// </param>
        /// <param name="monitor">
        /// The progress monitor for the installation
        /// </param>
        public UpdateInstaller(UpdateCommand command, IWritableDirectoryInfo installationRoot, IProgressMonitor monitor)
        {
            this.Command = command;
            this.installationRoot = installationRoot;
            this.monitor = monitor;

            this.resourceService = MessageDispatcher.Instance.GetService<IResourceService>();
        }

        /// <summary>
        /// Gets the command executed by this installer.
        /// </summary>
        public UpdateCommand Command { get; private set; }

        /// <summary>
        /// Begins to verify that all resources are available locally.
        /// </summary>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// The asynchronous result to be used with <see cref="EndVerifyResources"/>.
        /// </returns>
        public IAsyncResult BeginVerifyResources(AsyncCallback callback, object state)
        {
            Logger.Trace($" {MethodBase.GetCurrentMethod().Name} state: {state}");
            var collector = new ResourceCollector();
            var resourceIds = collector.GetAllResourceHashes(this.Command).ConvertAll(h => new ResourceId(h));

            Logger.Debug("Found {0} resource IDs, trying to get them from the service", resourceIds.Count);

            var result = new VerifyResourcesAsyncResult(resourceIds, callback, state);

            if (resourceIds.Count > 0)
            {
                foreach (var resource in resourceIds)
                {
                    this.resourceService.BeginGetResource(resource, this.ReceivedResource, result);
                }
            }
            else
            {
                result.Complete(true);
            }

            return result;
        }

        /// <summary>
        /// Ends the resource verification started with <see cref="BeginVerifyResources"/>.
        /// </summary>
        /// <param name="ar">
        /// The async result returned by <see cref="BeginVerifyResources"/>.
        /// </param>
        /// <returns>
        /// The list of resources required for the <see cref="Command"/>.
        /// </returns>
        public IList<ResourceInfo> EndVerifyResources(IAsyncResult ar)
        {
            
             Logger.Info($" {MethodBase.GetCurrentMethod().Name} - Ends the resource verification started with BeginVerifyResources");
           
            var result = ar as VerifyResourcesAsyncResult;
            if (result == null)
            {
                throw new ArgumentException("Async result has to come form BeginVerifyResources()", "ar");
            }

            try
            {
                result.WaitForCompletionAndVerify();
            }
            catch (UpdateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new UpdateException("Couldn't verify resources", ex);
            }

            return new List<ResourceInfo>(result.Resources.Values);
        }

        /// <summary>
        /// Creates the installation engine. This method can take some time, it is
        /// strongly suggested to run it in a separate thread.
        /// </summary>
        /// <param name="resources">
        ///     The resources to use during the installation. Usually they come from
        ///     <see cref="BeginVerifyResources"/>/<see cref="EndVerifyResources"/>.
        /// </param>
        /// <param name="restartApplicationsConfig">
        /// The configuration about all the applications to be restarted depending on certain folder updates
        /// </param>
        /// <returns>
        /// The installation engine implementation to install the <see cref="Command"/>.
        /// </returns>
        public IInstallationEngine CreateInstallationEngine(
            IList<ResourceInfo> resources, RestartApplicationsConfig restartApplicationsConfig)
        {
            var engine = this.CreateInstallationEngine();
            engine.Configure(
                this.Command,
                this.installationRoot,
                resources,
                restartApplicationsConfig,
                this.monitor.CreatePart(CreateEngineProgress, 1));
            return engine;
        }

        private InstallationEngineBase CreateInstallationEngine()
        {
            var factory = new UpdateSetFactory(
                this.installationRoot, this.Command, this.monitor.CreatePart(0, CreateEngineProgress));

            Logger.Debug("Creating UpdateSet");
            var updateSet = factory.CreateUpdateSet();

            if (Logger.IsTraceEnabled)
            {
                this.LogUpdateSet(LogLevel.Trace, updateSet);
            }

            var progs = PathManager.Instance.GetPath(FileType.Application, string.Empty);
            progs = progs.Remove(0, this.installationRoot.FullName.Length);

            var config = PathManager.Instance.GetPath(FileType.Config, string.Empty);
            config = config.Remove(0, this.installationRoot.FullName.Length);

            Logger.Trace("Update progs: '{0}'", progs);
            Logger.Trace("Update config: '{0}'", config);

            if (TreeHelper.FindFolder(updateSet, progs) != null)
            {
                Logger.Debug("Found update for '{0}', using SelfInstaller", progs);
                return new SelfInstaller(
                    TreeHelper.FindFolder(this.Command, progs),
                    TreeHelper.FindFolder(this.Command, config));
            }

            var configUpdate = TreeHelper.FindFolder(updateSet, config);
            if (configUpdate != null)
            {
                Logger.Debug("Found update for '{0}', using SelfConfigInstaller", config);
                return new SelfConfigInstaller(configUpdate);
            }

            if (updateSet.Folders.Count == 0)
            {
                Logger.Debug("Nothing to update, using NullInstaller");
                return new NullInstaller();
            }

            Logger.Debug("No special update, using DefaultInstaller");
            return new DefaultInstaller(updateSet);
        }

        private void ReceivedResource(IAsyncResult ar)
        {
            var result = (VerifyResourcesAsyncResult)ar.AsyncState;
            ResourceInfo resourceInfo;
            try
            {
                resourceInfo = this.resourceService.EndGetResource(ar);
            }
            catch (Exception ex)
            {
                result.CompleteException(ex, false);
                return;
            }

            Logger.Trace("Found resource {0}", resourceInfo.Id);

            lock (result)
            {
                result.Resources[resourceInfo.Id] = resourceInfo;

                foreach (var resource in result.Resources.Values)
                {
                    if (resource == null)
                    {
                        return;
                    }
                }
            }

            Logger.Debug("Got all {0} resources", result.Resources.Count);
            result.Complete(ar.CompletedSynchronously);
        }

        private void LogUpdateSet(LogLevel logLevel, UpdateSet updateSet)
        {
            using (var writer = new StringWriter())
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);

                var xmlWriter = XmlWriter.Create(
                    writer, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
                new XmlSerializer(updateSet.GetType()).Serialize(xmlWriter, updateSet, namespaces);
                Logger.Log(logLevel, writer.ToString());
            }
        }

        private class VerifyResourcesAsyncResult : AsyncResultBase
        {
            public VerifyResourcesAsyncResult(IEnumerable<ResourceId> resourceIds, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.Resources = new Dictionary<ResourceId, ResourceInfo>();
                foreach (var resourceId in resourceIds)
                {
                    this.Resources[resourceId] = null;
                }
            }

            public Dictionary<ResourceId, ResourceInfo> Resources { get; private set; }

            public new void Complete(bool synchronously)
            {
                base.Complete(synchronously);
            }
        }
    }
}

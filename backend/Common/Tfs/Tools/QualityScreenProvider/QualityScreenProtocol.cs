// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualityScreenProtocol.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the QualityScreenProtocol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.QualityScreenProvider
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Config;
    using Gorba.Motion.Protran.Core.Dictionary;
    using Gorba.Motion.Protran.Core.Protocols;

    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Server;

    using NLog;

    using Settings = Gorba.Common.Tfs.Tools.QualityScreenProvider.Properties.Settings;
    using XimpleConstants = Gorba.Common.Protocols.Ximple.Utils.Constants;

    /// <summary>
    /// The quality screen <see cref="IProtocol"/> implementation.
    /// It is a fake Protran protocol that queries TFS for information about builds.
    /// </summary>
    public class QualityScreenProtocol : IProtocol
    {
        private static readonly TimeSpan RefreshWait = new TimeSpan(0, 0, 30);

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly AutoResetEvent waitEvent = new AutoResetEvent(false);

        private readonly Dictionary<string, Identity> users = new Dictionary<string, Identity>(); 

        private IProtocolHost protocolHost;

        private bool stop;

        private TfsTeamProjectCollection tfsProjectCollection;

        private IBuildServer buildServer;

        private IGroupSecurityService groupSecurity;

        private int lastAllBuildsRowCount;
        private int lastFailedBuildsRowCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="QualityScreenProtocol"/> class.
        /// </summary>
        public QualityScreenProtocol()
        {
            this.Name = "QualityScreen";
        }

        /// <summary>
        /// Event that is fired when the protocol has finished starting up.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Gets the name of this protocol.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the generic view dictionary.
        /// </summary>
        public GenViewDictionary Dictionary { get; set; }

        /// <summary>
        /// Configures this protocol with the given configuration.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public void Configure(ProtocolConfig config)
        {
        }

        /// <summary>
        /// Stop this protocol.
        /// </summary>
        public void Stop()
        {
            this.stop = true;
            this.waitEvent.Set();
        }

        /// <summary>
        /// Sets a Ximple object to the protocol.
        /// This Ximple object can refer to an answer received from InfoMedia.exe
        /// for example.
        /// </summary>
        /// <param name="message">The Ximple object to pass to the protocol.</param>
        public void PostXimple(Ximple message)
        {
        }

        /// <summary>
        /// The main function of your protocol.
        /// Will be invoked by the protocol's host.
        /// </summary>
        /// <param name="host">The owner of this protocol.</param>
        /// <param name="args">The command line's arguments that the protocol needs to start.</param>
        public void Run(IProtocolHost host, params string[] args)
        {
            this.tfsProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(Settings.Default.TfsUrl));
            this.buildServer = this.tfsProjectCollection.GetService<IBuildServer>();
            this.groupSecurity = this.tfsProjectCollection.GetService<IGroupSecurityService>();

            this.protocolHost = host;

            this.RaiseStarted();

            while (!this.stop)
            {
                var updateStart = TimeProvider.Current.UtcNow;
                try
                {
                    this.Update();
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't update quality screen", ex);
                }

                var updateEnd = TimeProvider.Current.UtcNow;
                var waitTime = RefreshWait - (updateEnd - updateStart);
                if (waitTime > TimeSpan.Zero)
                {
                    this.waitEvent.WaitOne(waitTime, true);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="Started"/> event.
        /// </summary>
        protected virtual void RaiseStarted()
        {
            var handler = this.Started;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void Update()
        {
            Logger.Debug("Updating quality information");
            var buildDefinitions = this.buildServer.QueryBuildDefinitions(Settings.Default.TeamProjectName);
            var allBuilds = new List<BuildInfo>(buildDefinitions.Length);
            var failedBuilds = new List<BuildInfo>(buildDefinitions.Length);
            foreach (var buildDefinition in buildDefinitions)
            {
                if (this.stop)
                {
                    return;
                }

                try
                {
                    var build = this.GetLastFinishedBuild(buildDefinition);
                    if (build == null)
                    {
                        continue;
                    }

                    var info = this.CreateBuildInfo(build);
                    allBuilds.Add(info);
                    if (build.Status != BuildStatus.Succeeded)
                    {
                        failedBuilds.Add(info);
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't get build definition information", ex);
                }
            }

            allBuilds.Sort((a, b) => b.FinishTime.CompareTo(a.FinishTime));
            failedBuilds.Sort((a, b) => b.FinishTime.CompareTo(a.FinishTime));

            var ximple = this.CreateXimple(allBuilds, failedBuilds);

            if (ximple.Cells.Count > 0)
            {
                Logger.Debug("Sending quality information ({0} cells)", ximple.Cells.Count);
                this.SendXimple(ximple);
            }
        }

        private Ximple CreateXimple(IEnumerable<BuildInfo> allBuilds, IEnumerable<BuildInfo> failedBuilds)
        {
            var ximple = new Ximple(XimpleConstants.Version2);
            var allBuildsRow = 0;
            var allBuildsTable = this.Dictionary.GetTableForNameOrNumber(Constants.Tables.AllBuilds);
            foreach (var build in allBuilds)
            {
                this.AddBuildInfoCells(
                    ximple,
                    allBuildsTable,
                    allBuildsRow++,
                    build.Name,
                    ((int)build.State).ToString(CultureInfo.InvariantCulture),
                    build.User.DisplayName,
                    build.GetFinishTimeString());
            }

            var failedBuildsRow = 0;
            var failedBuildsTable = this.Dictionary.GetTableForNameOrNumber(Constants.Tables.FailedBuilds);
            foreach (var build in failedBuilds)
            {
                this.AddBuildInfoCells(
                    ximple,
                    failedBuildsTable,
                    failedBuildsRow++,
                    build.Name,
                    ((int)build.State).ToString(CultureInfo.InvariantCulture),
                    build.User.DisplayName,
                    build.GetFinishTimeString());
            }

            // clear unused rows
            for (int row = allBuildsRow; row < this.lastAllBuildsRowCount; row++)
            {
                this.AddBuildInfoCells(ximple, allBuildsTable, row);
            }

            // clear unused rows
            for (int row = failedBuildsRow; row < this.lastFailedBuildsRowCount; row++)
            {
                this.AddBuildInfoCells(ximple, failedBuildsTable, row);
            }

            this.lastAllBuildsRowCount = allBuildsRow;
            this.lastFailedBuildsRowCount = failedBuildsRow;
            return ximple;
        }

        private BuildInfo CreateBuildInfo(IBuildDetail build)
        {
            var info = new BuildInfo { Name = build.BuildDefinition.Name };
            switch (build.Status)
            {
                case BuildStatus.Succeeded:
                    info.State = BuildState.Success;
                    break;
                case BuildStatus.PartiallySucceeded:
                    info.State = BuildState.Partial;
                    break;
                case BuildStatus.Failed:
                    info.State = BuildState.Failed;
                    break;
                default:
                    info.State = BuildState.Failed;
                    break;
            }

            info.FinishTime = build.FinishTime;
            info.User = this.GetIdentity(build.RequestedFor);
            return info;
        }

        private void AddBuildInfoCells(
            Ximple ximple,
            Table table,
            int row,
            string name = "",
            string state = "",
            string userName = "",
            string timestamp = "")
        {
            var tableNumber = int.Parse(table.Number);

            var nameColumn = table.GetColumnForNameOrNumber(Constants.Columns.BuildDefinitionName);
            var nameIndex = int.Parse(nameColumn.Index);
            ximple.Cells.Add(new XimpleCell
            {
                LanguageNumber = 0,
                TableNumber = tableNumber,
                ColumnNumber = nameIndex,
                RowNumber = row,
                Value = name
            });

            var stateColumn = table.GetColumnForNameOrNumber(Constants.Columns.ErrorState);
            var stateIndex = int.Parse(stateColumn.Index);
            ximple.Cells.Add(new XimpleCell
            {
                LanguageNumber = 0,
                TableNumber = tableNumber,
                ColumnNumber = stateIndex,
                RowNumber = row,
                Value = state
            });

            var userColumn = table.GetColumnForNameOrNumber(Constants.Columns.UserName);
            var userIndex = int.Parse(userColumn.Index);
            ximple.Cells.Add(new XimpleCell
            {
                LanguageNumber = 0,
                TableNumber = tableNumber,
                ColumnNumber = userIndex,
                RowNumber = row,
                Value = userName
            });

            var timestampColumn = table.GetColumnForNameOrNumber(Constants.Columns.Timestamp);
            var timestampIndex = int.Parse(timestampColumn.Index);
            ximple.Cells.Add(new XimpleCell
            {
                LanguageNumber = 0,
                TableNumber = tableNumber,
                ColumnNumber = timestampIndex,
                RowNumber = row,
                Value = timestamp
            });
        }

        private Identity GetIdentity(string username)
        {
            Identity identity;
            if (this.users.TryGetValue(username, out identity))
            {
                return identity;
            }

            identity = this.groupSecurity.ReadIdentity(SearchFactor.AccountName, username, QueryMembership.Expanded);
            this.users.Add(username, identity);
            
            return identity;
        }

        private IBuildDetail GetLastFinishedBuild(IBuildDefinition buildDefinition)
        {
            var lastBuildUri = buildDefinition.LastBuildUri;
            if (lastBuildUri == null)
            {
                return null;
            }

            var build = this.buildServer.GetBuild(lastBuildUri);
            if (build == null)
            {
                return null;
            }

            if (this.IsValidBuild(build))
            {
                return build;
            }

            var builds = buildDefinition.QueryBuilds();

            return builds.LastOrDefault(this.IsValidBuild);
        }

        private bool IsValidBuild(IBuildDetail detail)
        {
            return detail.BuildFinished && detail.Reason != BuildReason.ValidateShelveset && detail.Status != BuildStatus.Stopped;
        }

        private void SendXimple(Ximple ximple)
        {
            this.protocolHost.OnDataFromProtocol(this, ximple);
        }

        private class BuildInfo
        {
            public string Name { get; set; }

            public BuildState State { get; set; }

            public DateTime FinishTime { get; set; }

            public Identity User { get; set; }

            public string GetFinishTimeString()
            {
                var finishTime = this.FinishTime.ToString("HH:mm:ss");
                string finishDate;
                if (this.FinishTime.Date.Equals(TimeProvider.Current.Now.Date))
                {
                    finishDate = "Today";
                }
                else if (this.FinishTime.Date.Equals(TimeProvider.Current.Now.Date - TimeSpan.FromDays(1)))
                {
                    finishDate = "Yesterday";
                }
                else
                {
                    finishDate = this.FinishTime.ToString("dd.MM.yyyy");
                }

                return string.Format("{0} {1}", finishDate, finishTime);
            }
        }
    }
}

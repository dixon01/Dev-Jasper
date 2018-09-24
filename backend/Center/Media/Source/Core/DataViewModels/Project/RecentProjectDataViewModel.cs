// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecentProjectDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Project
{
    using System;
    using System.Runtime.Serialization;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// DataViewModel for a recent project.
    /// </summary>
    [DataContract(Name = "Project")]
    public class RecentProjectDataViewModel : ViewModelBase
    {
        private string projectName;

        private string previewThumbnailPath;

        private string filePath;

        private DateTime lastUsed;

        private Guid projectId;

        private int tenantId;

        private string serverName;

        private bool isCheckedIn;

        /// <summary>
        /// Gets or sets a value indicating whether is not checked in.
        /// </summary>
        [DataMember(Name = "IsCheckedIn")]
        public bool IsCheckedIn
        {
            get
            {
                return this.isCheckedIn;
            }

            set
            {
                this.SetProperty(ref this.isCheckedIn, value, () => this.IsCheckedIn);
            }
        }

        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        [DataMember(Name = "ProjectName")]
        public string ProjectName
        {
            get
            {
                return this.projectName;
            }

            set
            {
                this.SetProperty(ref this.projectName, value, () => this.ProjectName);
            }
        }

        /// <summary>
        /// Gets or sets the DateTime when the project was last saved.
        /// </summary>
        [DataMember(Name = "LastUsed")]
        public DateTime LastUsed
        {
            get
            {
                return this.lastUsed;
            }

            set
            {
                this.SetProperty(ref this.lastUsed, value, () => this.LastUsed);
            }
        }

        /// <summary>
        /// Gets or sets the file path of the project.
        /// </summary>
        [DataMember(Name = "FilePath")]
        public string FilePath
        {
            get
            {
                return this.filePath;
            }

            set
            {
                this.SetProperty(ref this.filePath, value, () => this.FilePath);
            }
        }

        /// <summary>
        /// Gets or sets the path to the preview thumbnail.
        /// </summary>
        [DataMember(Name = "ThumbnailPath")]
        public string PreviewThumbnailPath
        {
            get
            {
                return this.previewThumbnailPath;
            }

            set
            {
                this.SetProperty(ref this.previewThumbnailPath, value, () => this.PreviewThumbnailPath);
            }
        }

        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        /// <value>
        /// The project id.
        /// </value>
        [DataMember(Name = "ProjectId")]
        public Guid ProjectId
        {
            get
            {
                return this.projectId;
            }

            set
            {
                this.SetProperty(ref this.projectId, value, () => this.ProjectId);
            }
        }

        /// <summary>
        /// Gets or sets the Tenant id of this project.
        /// </summary>
        [DataMember(Name = "Tenant")]
        public int TenantId
        {
            get
            {
                return this.tenantId;
            }

            set
            {
                this.SetProperty(ref this.tenantId, value, () => this.TenantId);
            }
        }

        /// <summary>
        /// Gets or sets the server URI.
        /// </summary>
        [DataMember(Name = "ServerName")]
        public string ServerName
        {
            get
            {
                return this.serverName;
            }

            set
            {
                this.SetProperty(ref this.serverName, value, () => this.ServerName);
            }
        }
    }
}

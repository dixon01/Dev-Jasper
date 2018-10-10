// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioFileElementDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The audio file element data view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System;
    using System.IO;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Common.Configuration.Infomedia.Layout;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The audio file element data view model.
    /// </summary>
    public partial class AudioFileElementDataViewModel
    {
        private readonly Lazy<IResourceManager> lazyResourceManager = new Lazy<IResourceManager>(GetResourceManager);

        /// <summary>
        /// Gets or sets the audio resource.
        /// </summary>
        public ResourceInfoDataViewModel AudioFile
        {
            get
            {
                var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
                if (state.CurrentProject == null)
                {
                    return null;
                }

                var resource =
                    state.CurrentProject.Resources.FirstOrDefault(
                        model => Path.GetFileName(model.Filename) == this.Filename.Value);
                return resource;
            }

            set
            {
                if (value == null)
                {
                    this.Filename.Value = string.Empty;
                    return;
                }

                this.Filename.Value = Path.GetFileName(value.Filename);
            }
        }

        /// <summary>
        /// Gets the resource manager.
        /// </summary>
        protected IResourceManager ResourceManager
        {
            get
            {
                return this.lazyResourceManager.Value;
            }
        }

        /// <summary>
        /// The unset media reference.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public override void UnsetMediaReference(ICommandRegistry commandRegistry)
        {
            if (this.AudioFile != null && this.AudioFile.Hash != string.Empty)
            {
                this.DecreaseMediaReferenceByHash(this.AudioFile.Hash, commandRegistry);
            }

            this.ResourceManager.AudioElementManager.UnsetReferences(this);
        }

        /// <summary>
        /// The set media reference.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public override void SetMediaReference(ICommandRegistry commandRegistry)
        {
            if (this.AudioFile != null && this.AudioFile.Hash != string.Empty)
            {
                this.IncreaseMediaReferenceByHash(this.AudioFile.Hash, commandRegistry);
            }

            this.ResourceManager.AudioElementManager.SetReferences(this);
        }

        private static IResourceManager GetResourceManager()
        {
            return ServiceLocator.Current.GetInstance<IResourceManager>();
        }

        partial void ExportNotGeneratedValues(AudioFileElement model, object exportParameters)
        {
            if (model.Filename != null && this.Filename != null && this.Filename.Formula == null)
            {
                model.Filename = Path.Combine(Settings.Default.AudioExportPath, model.Filename);
            }
        }
    }
}

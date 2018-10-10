// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaProjectDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediaProjectDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Project
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// Defines the data view model for a media project.
    /// </summary>
    public class MediaProjectDataViewModel : DataViewModelBase, ICloneable
    {
        private readonly List<string> installedFonts = new List<string>();

        private readonly IMediaShell mediaShell;

        private DateTime dateCreated;

        private DateTime dateLastModified;

        private InfomediaConfigDataViewModel infomediaConfig;

        private ExtendedObservableCollection<TextualReplacementDataViewModel> replacements;

        private ExtendedObservableCollection<CsvMappingDataViewModel> csvMappings;

        private Guid projectId;

        private ExtendedObservableCollection<string> availableFonts;

        private ExtendedObservableCollection<string> availableLedFonts;

        private string name;

        private string description;

        private long projectSize;

        private bool isCheckedIn;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaProjectDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        public MediaProjectDataViewModel(
            IMediaShell mediaShell, ICommandRegistry commandRegistry, MediaProjectDataModel dataModel)
            : this()
        {
            this.mediaShell = mediaShell;
            this.dateCreated = dataModel.DateCreated;
            this.dateLastModified = dataModel.DateLastModified;
            this.infomediaConfig = new InfomediaConfigDataViewModel(mediaShell, dataModel.InfomediaConfig);
            this.projectId = dataModel.ProjectId;
            var modelReplacements =
                dataModel.Replacements.Select(
                    info => new TextualReplacementDataViewModel(mediaShell, commandRegistry, info));
            this.Replacements.AddRange(modelReplacements);

            var modelCsvMappings =
                dataModel.CsvMappings.Select(
                    info => new CsvMappingDataViewModel(mediaShell, commandRegistry, info));
            this.CsvMappings.AddRange(modelCsvMappings);

            this.Authors.AddRange(dataModel.Authors);
            var resources = dataModel.Resources.Select(info => new ResourceInfoDataViewModel(info));
            this.Resources.AddRange(resources);

            this.Name = dataModel.Name;
            this.Description = dataModel.Description;
            this.ProjectSize = dataModel.ProjectSize;
            this.ClearDirty();

            this.PropertyChanged += this.DirtyChanged;
            this.Resources.ChildChanged += this.ResourceChanged;
            this.CsvMappings.ChildChanged += this.ResourceChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaProjectDataViewModel"/> class.
        /// </summary>
        /// <remarks>
        /// The <see cref="ProjectId"/> is not initialized.
        /// </remarks>
        public MediaProjectDataViewModel()
        {
            this.Authors = new ExtendedObservableCollection<string>();
            this.Replacements = new ExtendedObservableCollection<TextualReplacementDataViewModel>();
            this.CsvMappings = new ExtendedObservableCollection<CsvMappingDataViewModel>();
            this.Resources = new ExtendedObservableCollection<ResourceInfoDataViewModel>()
                                 {
                                     ListenOnChildChanged = true
                                 };

            this.installedFonts = Settings.Default.InstalledFontList.Cast<string>().ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaProjectDataViewModel"/> class.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        protected MediaProjectDataViewModel(MediaProjectDataViewModel dataViewModel)
            : this()
        {
            this.dateCreated = dataViewModel.DateCreated;
            this.dateLastModified = dataViewModel.DateLastModified;
            this.infomediaConfig = (InfomediaConfigDataViewModel)dataViewModel.InfomediaConfig.Clone();
            this.projectId = dataViewModel.ProjectId;
            foreach (var textualReplacementDataViewModel in dataViewModel.Replacements)
            {
                var clonedReplacement = (TextualReplacementDataViewModel)textualReplacementDataViewModel.Clone();
                this.Replacements.Add(clonedReplacement);
            }

            foreach (var csvMappingDataViewModel in dataViewModel.CsvMappings)
            {
                var clonedCsv = (CsvMappingDataViewModel)csvMappingDataViewModel.Clone();
                this.CsvMappings.Add(clonedCsv);
            }

            foreach (var author in dataViewModel.Authors)
            {
                this.Authors.Add(author);
            }

            foreach (var resourceInfoDataViewModel in dataViewModel.Resources)
            {
                this.Resources.Add(resourceInfoDataViewModel.Clone());
            }

            this.Name = dataViewModel.Name;
            this.Description = dataViewModel.Description;
            this.ProjectSize = dataViewModel.ProjectSize;
            this.ClearDirty();
        }

        /// <summary>
        /// Gets or sets the authors.
        /// </summary>
        /// <value>
        /// The authors.
        /// </value>
        public ExtendedObservableCollection<string> Authors { get; set; }

        /// <summary>
        /// Gets or sets the replacements.
        /// </summary>
        public ExtendedObservableCollection<TextualReplacementDataViewModel> Replacements
        {
            get
            {
                return this.replacements;
            }

            set
            {
                this.SetProperty(ref this.replacements, value, () => this.Replacements);
            }
        }

        /// <summary>
        /// Gets or sets the csv mappings.
        /// </summary>
        public ExtendedObservableCollection<CsvMappingDataViewModel> CsvMappings
        {
            get
            {
                return this.csvMappings;
            }

            set
            {
                this.SetProperty(ref this.csvMappings, value, () => this.CsvMappings);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is checked in.
        /// </summary>
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
        /// Gets or sets the name of the project.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.SetProperty(ref this.name, value, () => this.Name);
            }
        }

        /// <summary>
        /// Gets or sets the description of the project.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.SetProperty(ref this.description, value, () => this.Description);
            }
        }

        /// <summary>
        /// Gets a value indicating whether any property of this instance was modified.
        /// </summary>
        /// <value>
        /// <c>true</c> if any property of this instance was modified; otherwise, <c>false</c>.
        /// </value>
        public override bool IsDirty
        {
            get
            {
                return base.IsDirty
                       || this.InfomediaConfig.IsDirty
                       || this.Authors.IsDirty
                       || this.Resources.IsDirty
                       || this.Replacements.IsDirty
                       || this.CsvMappings.IsDirty;
            }
        }

        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        /// <value>
        /// The project id.
        /// </value>
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
        /// Gets or sets the date when the project was created.
        /// </summary>
        /// <value>
        /// The date when the project was created.
        /// </value>
        public DateTime DateCreated
        {
            get
            {
                return this.dateCreated;
            }

            set
            {
                this.SetProperty(ref this.dateCreated, value, () => this.DateCreated);
            }
        }

        /// <summary>
        /// Gets or sets the date when the project was last modified.
        /// </summary>
        /// <value>
        /// The date when the project was last modified.
        /// </value>
        public DateTime DateLastModified
        {
            get
            {
                return this.dateLastModified;
            }

            set
            {
                this.SetProperty(ref this.dateLastModified, value, () => this.DateLastModified);
            }
        }

        /// <summary>
        /// Gets or sets the infomedia config.
        /// </summary>
        /// <value>
        /// The infomedia config.
        /// </value>
        public virtual InfomediaConfigDataViewModel InfomediaConfig
        {
            get
            {
                return this.infomediaConfig;
            }

            set
            {
                this.SetProperty(ref this.infomediaConfig, value, () => this.InfomediaConfig);
            }
        }

        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>
        /// The resources.
        /// </value>
        public virtual ExtendedObservableCollection<ResourceInfoDataViewModel> Resources { get; set; }

        /// <summary>
        /// Gets or sets the project file path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets the Filename without extension
        /// </summary>
        public string FileName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(this.FilePath);
            }
        }

        /// <summary>
        /// Gets the Folder path
        /// </summary>
        public string Folder
        {
            get
            {
                return Path.GetDirectoryName(this.FilePath);
            }
        }

        /// <summary>
        /// Gets or sets the available fonts.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Not implemented
        /// </exception>
        [XmlIgnore]
        public ExtendedObservableCollection<string> AvailableFonts
        {
            get
            {
                if (this.availableFonts == null)
                {
                    this.availableFonts = new ExtendedObservableCollection<string>();
                    foreach (
                        var resource in
                            this.Resources.Where(resource =>
                                resource.Type == ResourceType.Font
                                && !(resource.Filename.EndsWith(".fon", StringComparison.InvariantCultureIgnoreCase)
                                  || resource.Filename.EndsWith(".fnt", StringComparison.InvariantCultureIgnoreCase)
                                  || resource.Filename.EndsWith(".cux", StringComparison.InvariantCultureIgnoreCase))))
                    {
                            this.availableFonts.Add(resource.Facename);
                    }

                    this.availableFonts.AddRange(this.installedFonts);
                }

                return this.availableFonts;
            }

            set
            {
                this.SetProperty(ref this.availableFonts, value, () => this.AvailableFonts);
            }
        }

        /// <summary>
        /// Gets or sets the available led fonts.
        /// </summary>
        [XmlIgnore]
        public ExtendedObservableCollection<string> AvailableLedFonts
        {
            get
            {
                if (this.availableLedFonts == null)
                {
                    this.availableLedFonts = new ExtendedObservableCollection<string>();
                    foreach (var resource in
                        this.Resources.Where(
                            resource =>
                            resource.Type == ResourceType.Font
                            && (resource.Filename.EndsWith(".fon", StringComparison.InvariantCultureIgnoreCase)
                                || resource.Filename.EndsWith(".fnt", StringComparison.InvariantCultureIgnoreCase)
                                || resource.Filename.EndsWith(".cux", StringComparison.InvariantCultureIgnoreCase))))
                    {
                        this.availableLedFonts.Add(resource.Facename);
                    }
                }

                return this.availableLedFonts;
            }

            set
            {
                this.SetProperty(ref this.availableLedFonts, value, () => this.AvailableLedFonts);
            }
        }

        /// <summary>
        /// Gets or sets the project size in KBytes.
        /// </summary>
        public long ProjectSize
        {
            get
            {
                return this.projectSize;
            }

            set
            {
                this.SetProperty(ref this.projectSize, value, () => this.ProjectSize);
            }
        }

        /// <summary>
        /// Converts this data view model to an equivalent instance of <see cref="MediaProjectDataModel"/>.
        /// </summary>
        /// <returns>A <see cref="MediaProjectDataModel"/> equivalent to this data view model.</returns>
        public virtual MediaProjectDataModel ToDataModel()
        {
            return new MediaProjectDataModel
                {
                    Authors = this.Authors.ToList(),
                    DateCreated = this.DateCreated,
                    DateLastModified = this.DateLastModified,
                    InfomediaConfig = this.InfomediaConfig.ToDataModel(),
                    ProjectId = this.ProjectId,
                    Resources = this.Resources.Select(r => r.ToDataModel()).ToList(),
                    Replacements = this.Replacements.Select(r => r.ToDataModel()).ToList(),
                    CsvMappings = this.CsvMappings.Select(r => r.ToDataModel()).ToList(),
                    Name = this.Name,
                    Description = this.Description,
                    ProjectSize = this.ProjectSize
                };
        }

        /// <summary>
        /// Clears the IsDirty flag. It clears the flag on the current object and all
        /// its children.
        /// </summary>
        public override sealed void ClearDirty()
        {
            if (this.Authors != null)
            {
                this.Authors.ClearDirty();
            }

            if (this.InfomediaConfig != null)
            {
                this.InfomediaConfig.ClearDirty();
            }

            if (this.Resources != null)
            {
                this.Resources.ClearDirty();
            }

            if (this.Replacements != null)
            {
                this.Replacements.ClearDirty();
            }

            if (this.CsvMappings != null)
            {
                this.CsvMappings.ClearDirty();
            }

            base.ClearDirty();
        }

        /// <summary>
        /// The get media hash.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetMediaHash(string filename)
        {
            ResourceInfoDataViewModel resourceViewModel = null;
            if (filename != string.Empty)
            {
                var resourceViewModels = this.Resources.Where(
                        r => r.Filename.Contains(filename));
                foreach (var viewModel in resourceViewModels)
                {
                    var file = Path.GetFileName(viewModel.Filename);
                    if (file == filename)
                    {
                        resourceViewModel = viewModel;
                        break;
                    }
                }
            }

            string hash = null;
            if (resourceViewModel != null)
            {
                hash = resourceViewModel.Hash;
            }

            return hash;
        }

        /// <summary>
        /// Creates a deep clone of the current object.
        /// </summary>
        /// <returns>
        /// The deep clone.
        /// </returns>
        public object Clone()
        {
            return new MediaProjectDataViewModel(this);
        }

        /// <summary>
        /// The get elements of type iterator.
        /// </summary>
        /// <typeparam name="T">
        /// The element type to search for.
        /// </typeparam>
        /// <returns>
        /// The IEnumerable of all found elements/>.
        /// </returns>
        public IEnumerable<T> GetElementsOfType<T>()
        {
            return this.InfomediaConfig.GetElementsOfType<T>();
        }

        private void DirtyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsDirty")
            {
                return;
            }

            this.isCheckedIn = this.isCheckedIn && !this.IsDirty;
        }

        private void ResourceChanged()
        {
            if (this.mediaShell.MediaApplicationState.CurrentProject != null)
            {
                this.mediaShell.MediaApplicationState.MakeDirty();
                this.mediaShell.MediaApplicationState.CurrentProject.IsCheckedIn = false;
            }
        }
    }
}
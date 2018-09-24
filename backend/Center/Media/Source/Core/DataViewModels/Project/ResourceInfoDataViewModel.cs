// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceInfoDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Project
{
    using System.Linq;
    using System.Runtime.Serialization;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Models;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The led font types.
    /// </summary>
    public enum LedFontType
    {
        /// <summary>
        /// The unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Type is Fnt Font.
        /// </summary>
        FntFont,

        /// <summary>
        /// Type is Fon Font.
        /// </summary>
        FonFont,

        /// <summary>
        /// Type is arab Unicode.
        /// </summary>
        FonUnicodeArab,

        /// <summary>
        /// Type is hebrew Unicode.
        /// </summary>
        FonUnicodeHebrew,

        /// <summary>
        /// Type is chines Unicode.
        /// </summary>
        FonUnicodeChines,

        /// <summary>
        /// Type is Cux Font.
        /// </summary>
        CUxFont,
    }

    /// <summary>
    /// Defines the data view model for resource information.
    /// </summary>
    public class ResourceInfoDataViewModel : ReferenceTrackedDataViewModelBase, IUsageTrackedObject
    {
        private string dimension;
        private string duration;
        private string facename;
        private string filename;

        private bool forceExport;
        private string hash;

        private bool isLedFont;
        private LedFontType ledFontType;
        private bool isLedImage;

        private long length;
        private int referencesCount;
        private string thumbnailHash;
        private ResourceType type;
        private bool isUsedVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceInfoDataViewModel"/> class.
        /// </summary>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        public ResourceInfoDataViewModel(ResourceInfo dataModel)
        {
            this.Filename = dataModel.Filename;
            this.Hash = dataModel.Hash;
            this.Type = dataModel.Type;
            this.ReferencesCount = dataModel.ReferencesCount;
            this.Dimension = dataModel.Dimension;
            this.Duration = dataModel.Duration;
            this.ThumbnailHash = dataModel.ThumbnailHash;
            this.Facename = dataModel.Facename;
            this.IsLedFont = dataModel.IsLedFont;
            this.LedFontType = dataModel.LedFontType;
            this.IsLedImage = dataModel.IsLedImage;
            this.Length = dataModel.Length;
            this.ForceExport = dataModel.ForceExport;
            this.UpdateIsUsedVisible();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceInfoDataViewModel"/> class.
        /// </summary>
        /// <param name="dataViewmModel">
        /// The data model.
        /// </param>
        public ResourceInfoDataViewModel(ResourceInfoDataViewModel dataViewmModel)
        {
            this.Filename = dataViewmModel.Filename;
            this.Hash = dataViewmModel.Hash;
            this.Type = dataViewmModel.Type;
            this.ReferencesCount = dataViewmModel.ReferencesCount;
            this.Dimension = dataViewmModel.Dimension;
            this.Duration = dataViewmModel.Duration;
            this.ThumbnailHash = dataViewmModel.ThumbnailHash;
            this.Facename = dataViewmModel.Facename;
            this.IsLedFont = dataViewmModel.IsLedFont;
            this.LedFontType = dataViewmModel.LedFontType;
            this.IsLedImage = dataViewmModel.IsLedImage;
            this.Length = dataViewmModel.Length;
            this.ForceExport = dataViewmModel.ForceExport;
            this.UpdateIsUsedVisible();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceInfoDataViewModel"/> class.
        /// </summary>
        public ResourceInfoDataViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the dimension of the resource. (Width x Height).
        /// </summary>
        public string Dimension
        {
            get
            {
                return this.dimension;
            }

            set
            {
                this.SetProperty(ref this.dimension, value, () => this.Dimension);
            }
        }

        /// <summary>
        /// Gets or sets the duration of a video resource.
        /// </summary>
        public string Duration
        {
            get
            {
                return this.duration;
            }

            set
            {
                this.SetProperty(ref this.duration, value, () => this.Duration);
            }
        }

        /// <summary>
        /// Gets or sets the face name.
        /// </summary>
        public string Facename
        {
            get
            {
                return this.facename;
            }

            set
            {
                this.SetProperty(ref this.facename, value, () => this.Facename);
            }
        }

        /// <summary>
        /// Gets or sets the filename of the resource.
        /// </summary>
        /// <value>
        /// The filename of the resource.
        /// </value>
        public string Filename
        {
            get
            {
                return this.filename;
            }

            set
            {
                this.SetProperty(ref this.filename, value, () => this.Filename);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether force export.
        /// </summary>
        public bool ForceExport
        {
            get
            {
                return this.forceExport;
            }

            set
            {
                this.SetProperty(ref this.forceExport, value, () => this.ForceExport);
            }
        }

        /// <summary>
        /// Gets or sets the hash of the resource.
        /// </summary>
        /// <value>
        /// The hash of the resource.
        /// </value>
        public string Hash
        {
            get
            {
                return this.hash;
            }

            set
            {
                this.SetProperty(ref this.hash, value, () => this.Hash);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is led font.
        /// </summary>
        public bool IsLedFont
        {
            get
            {
                return this.isLedFont;
            }

            set
            {
                this.SetProperty(ref this.isLedFont, value, () => this.IsLedFont);
            }
        }

        /// <summary>
        /// Gets or sets the led font type.
        /// </summary>
        public LedFontType LedFontType
        {
            get
            {
                return this.ledFontType;
            }

            set
            {
                this.SetProperty(ref this.ledFontType, value, () => this.LedFontType);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is led image.
        /// </summary>
        public bool IsLedImage
        {
            get
            {
                return this.isLedImage;
            }

            set
            {
                this.SetProperty(ref this.isLedImage, value, () => this.isLedImage);
            }
        }

        /// <summary>
        /// Gets or sets the length of a resource.
        /// </summary>
        public long Length
        {
            get
            {
                return this.length;
            }

            set
            {
                this.SetProperty(ref this.length, value, () => this.Length);
            }
        }

        /// <summary>
        /// Gets or sets the references count.
        /// </summary>
        /// <value>
        /// The references count.
        /// </value>
        public int ReferencesCount
        {
            get
            {
                return this.referencesCount;
            }

            set
            {
                this.SetProperty(ref this.referencesCount, value, () => this.ReferencesCount);

                // this.IsUsed = this.ReferencesCount > 0;
            }
        }

        /// <summary>
        /// Gets or sets the hash of the thumbnail.
        /// </summary>
        /// <value>
        /// The hash of the thumbnail.
        /// </value>
        public string ThumbnailHash
        {
            get
            {
                return this.thumbnailHash;
            }

            set
            {
                this.SetProperty(ref this.thumbnailHash, value, () => this.ThumbnailHash);
            }
        }

        /// <summary>
        /// Gets or sets the type of the resource.
        /// </summary>
        /// <value>
        /// The type of the resource.
        /// </value>
        public ResourceType Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.SetProperty(ref this.type, value, () => this.Type);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the resource is used and visible.
        /// </summary>
        public bool IsUsedVisible
        {
            get
            {
                return this.isUsedVisible;
            }

            private set
            {
                this.SetProperty(ref this.isUsedVisible, value, () => this.IsUsedVisible);
            }
        }

        /// <summary>
        /// Gets a value indicating whether should export.
        /// </summary>
        [IgnoreDataMember]
        public bool ShouldExport
        {
            get
            {
                return this.IsUsedVisible || this.ForceExport;
            }
        }

        /// <summary>
        /// the clone function
        /// </summary>
        /// <returns>a cloned resource info</returns>
        public ResourceInfoDataViewModel Clone()
        {
            return new ResourceInfoDataViewModel(this);
        }

        /// <summary>
        /// Converts this data view model to an equivalent <see cref="ResourceInfo"/>.
        /// </summary>
        /// <returns>A <see cref="ResourceInfo"/> equivalent to this data view model.</returns>
        public ResourceInfo ToDataModel()
        {
            return new ResourceInfo
                       {
                           Filename = this.Filename,
                           Hash = this.Hash,
                           ThumbnailHash = this.ThumbnailHash,
                           Type = this.Type,
                           ReferencesCount = this.ReferencesCount,
                           Dimension = this.Dimension,
                           Duration = this.Duration,
                           Facename = this.Facename,
                           IsLedFont = this.IsLedFont,
                           LedFontType = this.LedFontType,
                           IsLedImage = this.IsLedImage,
                           Length = this.Length,
                           ForceExport = this.ForceExport
                       };
        }

        /// <summary>
        /// Updates the <see cref="IsUsedVisible"/> by checking if any reference and its parents
        /// are set to visible/enabled.
        /// </summary>
        public void UpdateIsUsedVisible()
        {
            var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            if (state.CurrentProject == null)
            {
                return;
            }

            var layouts = state.CurrentProject.InfomediaConfig.Layouts;
            if (!this.IsUsed)
            {
                this.IsUsedVisible = false;
                return;
            }

            if (this.Type == ResourceType.Audio || this.Type == ResourceType.Symbol || this.Type == ResourceType.Csv)
            {
                this.IsUsedVisible = this.IsUsed;
                return;
            }

            foreach (var trackedResourceReference in this.References)
            {
                var key = trackedResourceReference.Key.Key;

                // image and video sections
                foreach (var cycle in state.CurrentProject.InfomediaConfig.Cycles.StandardCycles)
                {
                    if (cycle.Enabled.Value || cycle.Enabled.Formula != null)
                    {
                        foreach (var sectionConfigDataViewModelBase in cycle.Sections)
                        {
                            if ((sectionConfigDataViewModelBase.GetHashCode() == key
                                 || sectionConfigDataViewModelBase.ClonedFrom == key)
                                && (sectionConfigDataViewModelBase.Enabled.Value
                                    || sectionConfigDataViewModelBase.Enabled.Formula != null))
                            {
                                this.IsUsedVisible = true;
                                return;
                            }
                        }
                    }
                }

                foreach (var cycle in state.CurrentProject.InfomediaConfig.Cycles.EventCycles)
                {
                    if (cycle.Enabled.Value || cycle.Enabled.Formula != null)
                    {
                        foreach (var sectionConfigDataViewModelBase in cycle.Sections)
                        {
                            if ((sectionConfigDataViewModelBase.GetHashCode() == key
                                 || sectionConfigDataViewModelBase.ClonedFrom == key)
                                && (sectionConfigDataViewModelBase.Enabled.Value
                                    || sectionConfigDataViewModelBase.Enabled.Formula != null))
                            {
                                this.IsUsedVisible = true;
                                return;
                            }
                        }
                    }
                }

                // Layoutelements
                if (layouts.Where(layout => this.FindEnabledParent(layout, state.CurrentProject.InfomediaConfig))
                        .Any(layout => layout.Resolutions
                            .Any(resolution => (from element in resolution.Elements
                                                where key == element.GetHashCode()
                                                select element as GraphicalElementDataViewModelBase)
                                                .Any(graphical =>
                                                   graphical == null
                                                   || graphical.Visible.Value
                                                   || graphical.Visible.Formula != null))))
                {
                    this.IsUsedVisible = true;
                    return;
                }
            }

            this.IsUsedVisible = false;
        }

        private bool FindEnabledParent(LayoutConfigDataViewModel layout, InfomediaConfigDataViewModel config)
        {
            // Layout is not used
            if (!layout.CycleSectionReferences.Any())
            {
                return false;
            }

            return (from layoutCycleSectionRefDataViewModel in layout.CycleSectionReferences
                    where
                        layoutCycleSectionRefDataViewModel.CycleReference.Enabled.Value
                        || layoutCycleSectionRefDataViewModel.CycleReference.Enabled.Formula != null
                    where
                        layoutCycleSectionRefDataViewModel.SectionReference.Enabled.Value
                        || layoutCycleSectionRefDataViewModel.SectionReference.Enabled.Formula != null
                    select layoutCycleSectionRefDataViewModel.CycleReference.CyclePackageReferences).Any(
                        cyclePackages =>
                        cyclePackages.Select(
                            cyclePackage =>
                            config.VirtualDisplays.FirstOrDefault(d => d.CyclePackageName == cyclePackage.Name.Value))
                            .Where(virtualDisplay => virtualDisplay != null)
                            .Select(
                                virtualDisplay =>
                                config.MasterPresentation.MasterLayouts.First()
                                    .PhysicalScreens.FirstOrDefault(
                                        s => s.VirtualDisplays.Any(v => v.ReferenceName == virtualDisplay.Name.Value)))
                            .Where(physicalScreen => physicalScreen != null)
                            .Any(
                                physicalScreen =>
                                physicalScreen.Reference.Visible.Value
                                || physicalScreen.Reference.Visible.Formula != null));
        }
    }
}
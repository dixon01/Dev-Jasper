// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdatePartsTimelineViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdatePartsTimelineViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Update
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// The view model for the update parts timeline inside an update group.
    /// </summary>
    public class UpdatePartsTimelineViewModel : ViewModelBase
    {
        private readonly EntityCollection<UpdatePartReadOnlyDataViewModel> updateParts;

        private DateTime startDate;

        private DateTime endDate;

        private DateTime visibleStartDate;

        private DateTime visibleEndDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePartsTimelineViewModel"/> class.
        /// </summary>
        /// <param name="updateParts">
        /// The list of update parts.
        /// </param>
        public UpdatePartsTimelineViewModel(EntityCollection<UpdatePartReadOnlyDataViewModel> updateParts)
        {
            this.updateParts = updateParts;
            this.updateParts.PropertyChanged += this.UpdatePartsOnPropertyChanged;
            this.updateParts.Items.CollectionChanged += this.ItemsOnCollectionChanged;

            this.Parts = new ObservableCollection<Part>();
            this.Annotations = new ObservableCollection<IAnnotation> { new NowAnnotation() };

            this.StartDate = new DateTime(TimeProvider.Current.Now.Year - 2, 1, 1);
            this.EndDate = new DateTime(TimeProvider.Current.Now.Year + 2, 1, 1).AddDays(-1);

            this.VisibleStartDate = TimeProvider.Current.Now - TimeSpan.FromDays(30);
            this.VisibleEndDate = this.VisibleStartDate + TimeSpan.FromDays(365);

            if (!this.updateParts.IsLoading)
            {
                this.LoadData();
            }
        }

        /// <summary>
        /// Interface for all annotations shown in the timeline.
        /// </summary>
        public interface IAnnotation
        {
            /// <summary>
            /// Gets the start date.
            /// </summary>
            DateTime StartDate { get; }

            /// <summary>
            /// Gets the duration.
            /// </summary>
            TimeSpan Duration { get; }

            /// <summary>
            /// Gets the z-index.
            /// </summary>
            int ZIndex { get; }
        }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return this.startDate;
            }

            set
            {
                this.SetProperty(ref this.startDate, value, () => this.StartDate);
            }
        }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return this.endDate;
            }

            set
            {
                this.SetProperty(ref this.endDate, value, () => this.EndDate);
            }
        }

        /// <summary>
        /// Gets or sets the visible start date.
        /// </summary>
        public DateTime VisibleStartDate
        {
            get
            {
                return this.visibleStartDate;
            }

            set
            {
                this.SetProperty(ref this.visibleStartDate, value, () => this.VisibleStartDate);
            }
        }

        /// <summary>
        /// Gets or sets the visible end date.
        /// </summary>
        public DateTime VisibleEndDate
        {
            get
            {
                return this.visibleEndDate;
            }

            set
            {
                this.SetProperty(ref this.visibleEndDate, value, () => this.VisibleEndDate);
            }
        }

        /// <summary>
        /// Gets the parts.
        /// </summary>
        public ObservableCollection<Part> Parts { get; private set; }

        /// <summary>
        /// Gets the annotations.
        /// </summary>
        public ObservableCollection<IAnnotation> Annotations { get; private set; }

        private void ClearData()
        {
            this.Parts.Clear();
        }

        private void LoadData()
        {
            foreach (var updatePart in this.updateParts.Items.OrderByDescending(p => p.CreatedOn))
            {
                this.Parts.Add(new Part(updatePart));
            }
        }

        private void ReloadData()
        {
            this.ClearData();
            this.LoadData();
        }

        private void UpdatePartsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsLoading")
            {
                return;
            }

            if (this.updateParts.IsLoading)
            {
                this.ClearData();
            }
            else
            {
                this.ReloadData();
            }
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!this.updateParts.IsLoading)
            {
                this.ReloadData();
            }
        }

        /// <summary>
        /// The part view model.
        /// </summary>
        public class Part : ViewModelBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Part"/> class.
            /// </summary>
            /// <param name="updatePart">
            /// The update part.
            /// </param>
            public Part(UpdatePartReadOnlyDataViewModel updatePart)
            {
                this.UpdatePart = updatePart;
                this.UpdatePart.PropertyChanged += this.UpdatePartOnPropertyChanged;
            }

            /// <summary>
            /// Gets the update part.
            /// </summary>
            public UpdatePartReadOnlyDataViewModel UpdatePart { get; private set; }

            /// <summary>
            /// Gets the type.
            /// </summary>
            public UpdatePartType Type
            {
                get
                {
                    return this.UpdatePart.Type;
                }
            }

            /// <summary>
            /// Gets the start date.
            /// </summary>
            public DateTime StartDate
            {
                get
                {
                    return this.UpdatePart.Start.ToLocalTime();
                }
            }

            /// <summary>
            /// Gets the end date.
            /// </summary>
            public DateTime EndDate
            {
                get
                {
                    return this.UpdatePart.End.ToLocalTime();
                }
            }

            /// <summary>
            /// Gets the duration.
            /// </summary>
            public TimeSpan Duration
            {
                get
                {
                    return this.EndDate - this.StartDate;
                }
            }

            private void UpdatePartOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                switch (e.PropertyName)
                {
                    case "Type":
                        this.RaisePropertyChanged("Type");
                        break;
                    case "Start":
                        this.RaisePropertyChanged("StartDate");
                        this.RaisePropertyChanged("Duration");
                        break;
                    case "End":
                        this.RaisePropertyChanged("Duration");
                        break;
                }
            }
        }

        private class NowAnnotation : ViewModelBase, IAnnotation
        {
            public DateTime StartDate
            {
                get
                {
                    return TimeProvider.Current.Now;
                }
            }

            public TimeSpan Duration
            {
                get
                {
                    return TimeSpan.Zero;
                }
            }

            public int ZIndex
            {
                get
                {
                    return 10;
                }
            }
        }
    }
}
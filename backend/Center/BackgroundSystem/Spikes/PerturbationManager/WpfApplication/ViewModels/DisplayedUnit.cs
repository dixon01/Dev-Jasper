// -----------------------------------------------------------------------
// <copyright file="DisplayedUnit.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WpfApplication.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    using Core.Annotations;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DisplayedUnit : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public string Name { get; set; }

        private bool isSelected;

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.isSelected == value)
                {
                    return;
                }

                this.isSelected = value;
                this.OnPropertyChanged("IsSelected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

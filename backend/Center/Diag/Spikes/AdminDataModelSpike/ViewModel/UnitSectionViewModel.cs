namespace AdminDataModelSpike.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Core;

    public class UnitSectionViewModel : ViewModelBase
    {
        private readonly ObservableCollection<UnitViewModelBase> allUnits;

        private readonly Func<UnitViewModelBase, bool> filter;

        public UnitSectionViewModel(
            string name, ObservableCollection<UnitViewModelBase> allUnits, Func<UnitViewModelBase, bool> filter)
        {
            this.Name = name;

            this.allUnits = allUnits;
            this.filter = filter;

            /*this.Units = new CollectionViewSource();
            this.Units.Filter += this.UnitsOnFilter;
            this.Units.Source = allUnits;*/

            this.Units = new ObservableCollection<UnitViewModelBase>(allUnits.Where(filter));

            foreach (var unit in this.allUnits)
            {
                unit.PropertyChanged += this.UnitOnPropertyChanged;
            }

            this.allUnits.CollectionChanged += this.AllUnitsOnCollectionChanged;
        }

        /*private void UnitsOnFilter(object sender, FilterEventArgs e)
        {
            var unit = e.Item as UnitViewModelBase;
            e.Accepted = unit != null && this.filter(unit);
        }*/

        public string Name { get; private set; }

        //public CollectionViewSource Units { get; private set; }

        public ObservableCollection<UnitViewModelBase> Units { get; private set; }

        private void UpdateUnits()
        {
            var newList = this.allUnits.Where(this.filter).ToList();
            for (int index = 0; index < this.Units.Count; index++)
            {
                var unit = this.Units[index];
                var newIndex = newList.IndexOf(unit);
                if (newIndex < 0)
                {
                    unit.PropertyChanged -= this.UnitOnPropertyChanged;
                    this.Units.RemoveAt(index);
                    index--;
                    continue;
                }

                for (int i = 0; i < newIndex; i++)
                {
                    newList[0].PropertyChanged += this.UnitOnPropertyChanged;
                    this.Units.Add(newList[0]);
                    newList.RemoveAt(0);
                }

                newList.RemoveAt(0);
            }

            foreach (var unit in newList)
            {
                unit.PropertyChanged += this.UnitOnPropertyChanged;
                this.Units.Add(unit);
            }
        }

        private void AllUnitsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (UnitViewModelBase newUnit in e.NewItems)
                    {
                        newUnit.PropertyChanged += this.UnitOnPropertyChanged;
                    }

                    this.UpdateUnits();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (UnitViewModelBase oldUnit in e.OldItems)
                    {
                        oldUnit.PropertyChanged -= this.UnitOnPropertyChanged;
                        this.Units.Remove(oldUnit);
                    }

                    break;
                default:
                    foreach (var unit in this.Units)
                    {
                        unit.PropertyChanged -= this.UnitOnPropertyChanged;
                    }

                    foreach (UnitViewModelBase oldUnit in e.OldItems)
                    {
                        oldUnit.PropertyChanged -= this.UnitOnPropertyChanged;
                    }

                    this.Units.Clear();
                    this.UpdateUnits();
                    break;
            }
        }

        private void UnitOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateUnits();
        }
    }
}
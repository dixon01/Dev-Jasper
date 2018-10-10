// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Shell.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Shell type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfApplication
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;

    using Core;
    using Core.Annotations;

    using Google.Transit.Realtime;

    using Newtonsoft.Json;

    using WpfApplication.ViewModels;

    public class Shell : INotifyPropertyChanged
    {
        private static readonly DateTime EpochReference = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly PerturbationManagerServer server;

        private Thread thread;

        private bool isStarted;

        public bool IsStarted
        {
            get
            {
                return this.isStarted;
            }

            set
            {
                if (this.isStarted == value)
                {
                    return;
                }

                this.isStarted = value;
                this.UpdateIsSimulationModeSelectorEnabled();
                this.OnPropertyChanged("IsStarted");
            }
        }

        public Startup Startup { get; private set; }

        public ObservableCollection<DisplayedUnit> Units { get; set; }

        private bool simulationMode;

        public bool SimulationMode
        {
            get
            {
                return this.simulationMode;
            }

            set
            {
                if (this.simulationMode == value)
                {
                    return;
                }

                this.simulationMode = value;
                this.OnPropertyChanged("SimulationMode");
            }
        }

        private bool isSimulationModeSelectorEnabled;

        public bool IsSimulationModeSelectorEnabled
        {
            get
            {
                return this.isSimulationModeSelectorEnabled;
            }

            set
            {
                if (this.isSimulationModeSelectorEnabled == value)
                {
                    return;
                }

                this.isSimulationModeSelectorEnabled = value;
                this.OnPropertyChanged("IsSimulationModeSelectorEnabled");
            }
        }

        private void UpdateIsSimulationModeSelectorEnabled()
        {
            this.IsSimulationModeSelectorEnabled = !this.IsStarted && this.IsValid;
        }

        public bool SendWrongContent
        {
            get
            {
                return this.Startup.SendWrongContent;
            }

            set
            {
                if (this.Startup.SendWrongContent != value)
                {
                    this.Startup.SendWrongContent = value;
                    this.OnPropertyChanged("SendWrongContent");
                }
            }
        }

        private string fromDateTimeInput;

        public string FromDateTimeInput
        {
            get
            {
                return this.fromDateTimeInput;
            }

            set
            {
                if (!string.Equals(this.fromDateTimeInput, value))
                {
                    this.fromDateTimeInput = value;
                    DateTime dateTime;
                    if (DateTime.TryParseExact(
                        value, "u", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTime))
                    {
                        var epoch = Startup.ToEpoch(dateTime);
                        this.FromDateTimeOutput = epoch.ToString(CultureInfo.InvariantCulture);
                    }

                    this.OnPropertyChanged("FromDateTimeInput");
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private string fromDateTimeOutput;

        public string FromDateTimeOutput
        {
            get
            {
                return this.fromDateTimeOutput;
            }

            set
            {
                if (!string.Equals(this.fromDateTimeOutput, value))
                {
                    this.fromDateTimeOutput = value;
                    this.OnPropertyChanged("FromDateTimeOutput");
                }
            }
        }

        private string fromEpochInput;

        public string FromEpochInput
        {
            get
            {
                return this.fromEpochInput;
            }

            set
            {
                if (!string.Equals(this.fromEpochInput, value))
                {
                    this.fromEpochInput = value;
                    var dateTime = Startup.FromEpoch(this.fromEpochInput);
                    this.FromEpochOutput = dateTime.HasValue ? dateTime.Value.ToString("u") : null;

                    this.OnPropertyChanged("FromEpochInput");
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private string fromEpochOutput;

        public string FromEpochOutput
        {
            get
            {
                return this.fromEpochOutput;
            }

            set
            {
                if (!string.Equals(this.fromEpochOutput, value))
                {
                    this.fromEpochOutput = value;
                    this.OnPropertyChanged("FromEpochOutput");
                }
            }
        }

        public string Text
        {
            get
            {
                return this.Startup.Text;
            }

            set
            {
                if (string.Equals(value, this.Startup.Text))
                {
                    return;
                }

                this.Startup.Text = value;
                this.IsValid = this.ValidateText();
                if (!this.IsValid)
                {
                    this.SimulationMode = false;
                }

                this.OnPropertyChanged("Text");
            }
        }

        private bool ValidateText()
        {
            if (string.IsNullOrWhiteSpace(this.Text))
            {
                return false;
            }

            try
            {
                var jsonSerializer = new JsonSerializer();
                var feedMessage = JsonConvert.DeserializeObject<FeedMessage>(this.Text);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool isValid;

        public bool IsValid
        {
            get
            {
                return this.isValid;
            }

            set
            {
                if (value == this.isValid)
                {
                    return;
                }

                this.isValid = value;
                this.UpdateIsSimulationModeSelectorEnabled();
                this.OnPropertyChanged("IsValid");
            }
        }

        private bool truncateSeconds;

        public bool TruncateSeconds
        {
            get
            {
                return this.truncateSeconds;
            }

            set
            {
                this.OnPropertyChanged("TruncateSeconds");
            }
        }

        private DateTime date;

        public DateTime Date
        {
            get
            {
                return this.date;
            }

            set
            {
                var truncatedValue = this.truncateSeconds ? value.AddSeconds(-value.Second) : value;
                if (truncatedValue == this.date)
                {
                    return;
                }

                this.date = truncatedValue;
                this.OnPropertyChanged("Date");
                this.StatusBar = string.Format("Date set to {0}", truncatedValue);
            }
        }

        private string statusBar;

        public string StatusBar
        {
            get
            {
                return this.statusBar;
            }

            set
            {
                if (string.Equals(value, this.statusBar))
                {
                    return;
                }

                this.statusBar = value;
                this.OnPropertyChanged("StatusBar");
            }
        }

        private int selectedUnitId;

        public int SelectedUnitId
        {
            get
            {
                return this.selectedUnitId;
            }

            set
            {
                if (this.selectedUnitId == value)
                {
                    return;
                }

                this.selectedUnitId = value;
                this.OnPropertyChanged("SelectedUnitId");
            }
        }

        public Shell()
        {
            this.Startup = new Startup();
            this.server = new PerturbationManagerServer(this.Startup);
            this.server.ServerStarted = () =>
                { };
            this.server.ServerStopped = () =>
                {
                    this.IsStarted = false;
                    CommandManager.InvalidateRequerySuggested();
                };
            this.truncateSeconds = true;
            this.date = DateTime.UtcNow;
            this.date = this.date.AddSeconds(-this.date.Second);
            this.statusBar = "Ready";
            this.StartCommand = new RelayCommand(this.Start, this.CanStart);
            this.StopCommand = new RelayCommand(this.Stop, this.CanStop);
            this.CopyDateAsEpochCommand = new RelayCommand(this.CopyDateAsEpoch);
            this.CreateSampleMessageCommand = new RelayCommand(this.CreateSampleMessage, this.CanCreateSampleMessage);
            this.ReformatTextCommand = new RelayCommand(this.ReformatText, this.CanReformatText);
            this.CopyEpochCommand = new RelayCommand(this.CopyEpoch, this.CanCopyEpoch);
            this.CopyDateTimeCommand = new RelayCommand(this.CopyDateTime, this.CanCopyDateTime);
            this.SelectNowCommand = new RelayCommand(this.SelectNow);
            this.AddMinuteCommand = new RelayCommand(this.AddMinute, this.CanAddMinute);
            this.SubstractMinuteCommand = new RelayCommand(this.SubstractMinute, this.CanSubstractMinute);
            this.Units = new ObservableCollection<DisplayedUnit>();
        }

        private bool CanCreateSampleMessage(object obj)
        {
            return !this.SimulationMode;
        }

        public ICommand CopyEpochCommand { get; private set; }

        public ICommand CopyDateAsEpochCommand { get; private set; }

        public ICommand CopyDateTimeCommand { get; private set; }

        public ICommand CreateSampleMessageCommand { get; private set; }

        public ICommand ReformatTextCommand { get; private set; }

        public ICommand StartCommand { get; private set; }

        public ICommand StopCommand { get; private set; }

        public ICommand SelectNowCommand { get; private set; }

        public ICommand AddMinuteCommand { get; private set; }

        public ICommand SubstractMinuteCommand { get; private set; }

        public void Start()
        {
            if (this.isStarted)
            {
                return;
            }

            if (this.SimulationMode)
            {
                this.Startup.ConfigureForSimulation();
                SimulationManager.Current.Initialize(this.Startup.FeedMessage);
            }

            this.IsStarted = true;
            CommandManager.InvalidateRequerySuggested();

            this.thread = new Thread(this.server.Start);
            this.thread.IsBackground = true;
            this.thread.Start();
        }

        private void SelectNow()
        {
            this.Date = DateTime.UtcNow;
            this.FromDateTimeInput = DateTime.UtcNow.ToString("u");
            this.FromEpochInput = this.FromDateTimeOutput;
        }

        private void AddMinute()
        {
            this.AddMinutes(1);
        }

        private bool CanAddMinute(object parameter)
        {
            return true;
        }

        private void SubstractMinute()
        {
            this.AddMinutes(-1);
        }

        private bool CanSubstractMinute(object parameter)
        {
            return true;
        }

        private void AddMinutes(long minutes)
        {
            this.Date = this.date.AddMinutes(minutes);
                this.FromDateTimeInput = this.date.ToString("u");
                this.FromEpochInput = this.fromDateTimeOutput;
        }

        private bool CanStart(object parameter)
        {
            return !this.isStarted && (this.thread == null || !this.thread.IsAlive);
        }

        private bool CanStop(object parameter)
        {
            return this.isStarted;
        }

        private void CopyEpoch()
        {
            Clipboard.SetText(this.FromDateTimeOutput);
        }

        private void CopyDateAsEpoch()
        {
            var dateAsEpoch = ToEpoch(this.date).ToString(CultureInfo.InvariantCulture);
            Clipboard.SetText(dateAsEpoch);
            this.StatusBar = string.Format("Copied date {0} as expoch {1}", this.date, dateAsEpoch);
        }

        private bool CanCopyEpoch(object parameter)
        {
            return !string.IsNullOrWhiteSpace(this.FromDateTimeOutput);
        }

        private void CopyDateTime()
        {
            Clipboard.SetText(this.FromEpochOutput);
        }

        private bool CanCopyDateTime(object parameter)
        {
            return !string.IsNullOrWhiteSpace(this.FromEpochOutput);
        }

        private void Stop()
        {
            if (!this.isStarted)
            {
                return;
            }

            this.server.Stop();
        }

        private void CreateSampleMessage()
        {
            var feedMessage = CreateFeedMessage(this.selectedUnitId);
            var sb = new StringBuilder();
            using (var textWriter = new StringWriter(sb))
            {
                var serializer = new JsonSerializer
                    {
                        Formatting = Formatting.Indented
                    };
                serializer.Serialize(textWriter, feedMessage);
            }

            this.Text = sb.ToString();
        }

        private static FeedMessage CreateFeedMessage(params int[] units)
        {
            var feedMessage = new FeedMessage
            {
                header =
                    new FeedHeader
                    {
                        gtfs_realtime_version = "1.0",
                        incrementality = FeedHeader.Incrementality.FULL_DATASET,
                        timestamp = ToEpoch(DateTime.UtcNow)
                    }
            };
            foreach (var id in units)
            {
                var feedEntity = new FeedEntity
                    {
                        alert =
                            new Alert
                                {
                                    active_period = { new TimeRange { end = 0, start = 0 } },
                                    description_text =
                                        new TranslatedString
                                            {
                                                translation =
                                                    {
                                                        new TranslatedString.Translation { text = "hide@LINE,1" }
                                                    }
                                            },
                                    header_text =
                                        new TranslatedString
                                            {
                                                translation = { new TranslatedString.Translation { text = "Alert!" } }
                                            },
                                    informed_entity =
                                        {
                                            new EntitySelector { stop_id = id.ToString(CultureInfo.InvariantCulture) }
                                        }
                                },
                        id = "1"
                    };
                feedMessage.entity.Add(feedEntity);
            }

            return feedMessage;
        }

        private static ulong ToEpoch(DateTime date)
        {
            return (ulong)date.Subtract(EpochReference).TotalSeconds;
        }

        private static DateTime FromEpoch(ulong epoch)
        {
            return EpochReference.AddSeconds(epoch);
        }

        private bool CanReformatText(object parameter)
        {
            return !this.SimulationMode && !string.IsNullOrWhiteSpace(this.Text);
        }

        private void ReformatText()
        {
            try
            {
                var message = JsonConvert.DeserializeObject<FeedMessage>(this.Text);
                var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
                this.Text = JsonConvert.SerializeObject(message, settings);
            }
            catch (Exception)
            {
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

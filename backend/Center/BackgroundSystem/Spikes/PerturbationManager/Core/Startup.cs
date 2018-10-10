namespace Core
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Core.Annotations;

    using Google.Transit.Realtime;

    using NLog;

    using Newtonsoft.Json;

    using Owin;
    using Owin.Types;

    using ProtoBuf;

    public class Startup
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly DateTime EpochReference = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configuration(IAppBuilder app)
        {
            app.UseHandler(
                (req, res) =>
                {
                    const int Count = 1;
                    switch (req.Path)
                    {
                        case "/alert":
                            Logger.Info("Sending alert with {0} entries", Count);
                            return this.GetProtoBuf(res, Count);
                        case "/alert/txt":
                            Logger.Info("Sending alert text with {0} entries", Count);
                            return GetText(res, Count);
                    }

                    Logger.Info("Returning error for path '{0}'", req.Path);
                    res.StatusCode = 400;
                    var taskCompletionSource = new TaskCompletionSource<int>();
                    taskCompletionSource.SetResult(0);
                    return taskCompletionSource.Task;
                });
        }

        public string Text { get; set; }

        public bool SendWrongContent { get; set; }

        public FeedMessage FeedMessage { get; set; }

        private Task GetText(OwinResponse res, int count = 1)
        {
            res.ContentType = "application/json; charset=utf-8";
            res.StatusCode = 200;
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(this.Text);
            streamWriter.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            var buffer = memoryStream.GetBuffer();
            return res.WriteAsync(buffer, 0, (int)memoryStream.Length);
        }

        private Task GetProtoBuf(Owin.Types.OwinResponse res, int count = 1)
        {
            MemoryStream memoryStream;
            if (this.TryGetMessage(out memoryStream))
            {
                res.ContentType = "application/json; charset=utf-8";
                res.StatusCode = 200;
                var buffer = memoryStream.GetBuffer();
                var length = this.SendWrongContent ? Math.Min(0, memoryStream.Length - 7) : memoryStream.Length;
                return res.WriteAsync(buffer, 0, (int)length);
            }

            res.StatusCode = 500;
            var taskCompletionSource = new TaskCompletionSource<int>();
            taskCompletionSource.SetResult(0);
            return taskCompletionSource.Task;
        }

        private bool TryGetMessage(out MemoryStream memoryStream)
        {
            if (this.FeedMessage != null)
            {
                memoryStream = Serialize(this.FeedMessage);
                return true;
            }

            memoryStream = new MemoryStream();
            if (string.IsNullOrWhiteSpace(this.Text))
            {
                return false;
            }

            try
            {
                var feedMessage = JsonConvert.DeserializeObject<FeedMessage>(this.Text);
                memoryStream = Serialize(feedMessage);
            }
            catch (Exception)
            {
                return false;
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return true;
        }

        private static MemoryStream Serialize(FeedMessage feedMessage)
        {
            var memoryStream = new MemoryStream();
            Serializer.Serialize(memoryStream, feedMessage);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        private static MemoryStream GetMessage(int count)
        {
            var feedMessage = CreateFeedMessage(count);
            var memoryStream = new MemoryStream();
            Serializer.Serialize(memoryStream, feedMessage);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
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
                                            }
                                },
                        id = "1"
                    };
                feedEntity.alert.informed_entity.AddRange(
                    units.Select(unit => new EntitySelector { stop_id = unit.ToString(CultureInfo.InvariantCulture) }));
                feedMessage.entity.Add(feedEntity);

            return feedMessage;
        }

        public static ulong ToEpoch(DateTime date)
        {
            return (ulong)date.Subtract(EpochReference).TotalSeconds;
        }

        public static DateTime? FromEpoch(string epoch)
        {
            ulong epochValue;
            if (!ulong.TryParse(epoch, out epochValue))
            {
                return null;
            }

            return EpochReference.AddSeconds(epochValue);
        }

        public static DateTime FromEpoch(ulong epoch)
        {
            return EpochReference.AddSeconds(epoch);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void ConfigureForSimulation()
        {
            try
            {
                this.FeedMessage = JsonConvert.DeserializeObject<FeedMessage>(this.Text);
            }
            catch (Exception)
            {
            }
        }
    }
}
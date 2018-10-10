// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AcapelaSpeechItem.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AcapelaSpeechItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading;

namespace Gorba.Motion.Infomedia.AudioRenderer.Playback
{
    using System;
    using System.Threading.Tasks;

    using AcapelaGroup.BabTTSNet;

    using NLog;

    /// <summary>
    /// <see cref="SpeechItemBase"/> implementation that uses Acapela TTS.
    /// </summary>
    internal class AcapelaSpeechItem : SpeechItemBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private string acapelaVoice;

        private string textToSpeak;

#if __UseLuminatorTftDisplay
        public static object Locker = new object();

        public static FIFOTaskScheduler TaskScheduler = new FIFOTaskScheduler();
#else
        private readonly BabTTS engine;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="AcapelaSpeechItem"/> class.
        /// </summary>
        /// <param name="volume">
        /// The audio volume at which to speak the text (0..100).
        /// </param>
        public AcapelaSpeechItem(int volume)
            : base(volume)
        {
            if (!AcapelaHelper.Available)
            {
                throw new DllNotFoundException("Acapela is not properly installed on this system");
            }

#if !__UseLuminatorTftDisplay
            this.engine = new BabTTS();
            this.engine.OnEnd += this.EngineOnEnd;
#endif
        }
        
        /// <summary>
        /// Start playing this item.
        /// </summary>
        public override void Start()
        {
#if __UseLuminatorTftDisplay
            if (string.IsNullOrEmpty(this.acapelaVoice))
            {
                Logger.Warn(string.Format("Unspecified voice, using '{0}'", AcapelaHelper.TestVoice));
                this.acapelaVoice = AcapelaHelper.TestVoice;
            }
            
            Task speakTask = Task.Factory.StartNew(() => {
                    lock (Locker)
                    {
                        Logger.Info("Speaking [{0}] with {1}: {2}", this.Volume, this.acapelaVoice, this.textToSpeak);

                        var error = AcapelaHelper.Engine.Speak(this.textToSpeak, BabTtsSpeakFlags.BABTTS_TAG_NO | BabTtsSpeakFlags.BABTTS_READ_TEXT | BabTtsSpeakFlags.BABTTS_SYNC);

                        HandleError("Speak", error);
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler).ContinueWith(
                (x) =>
                    {
                        Logger.Info("Acapela TTS Speak Prompt Ended");
                        this.RaiseCompleted(EventArgs.Empty);
                    });
            
#else
            Logger.Info("Speaking with {0}: {1}", this.acapelaVoice, this.textToSpeak);

            var error = this.engine.Speak(
                this.textToSpeak,
                BabTtsSpeakFlags.BABTTS_TAG_SAPI | BabTtsSpeakFlags.BABTTS_READ_TEXT | BabTtsSpeakFlags.BABTTS_ASYNC);
            HandleError("Speak", error);
#endif

        }

        /// <summary>
        /// Immediately stop playing this item.
        /// </summary>
        public override void Stop()
        {
#if __UseLuminatorTftDisplay
            try
            {
                Logger.Info("Acapela TTS Pause()");
                HandleError("Pause", AcapelaHelper.Engine.Pause());
            }
            finally
            {
                Logger.Info("Acapela TTS Reset()");
                HandleError("Reset", AcapelaHelper.Engine.Reset());
            }
#else
            try
            {
                HandleError("Pause", this.engine.Pause());
            }
            finally
            {
                HandleError("Reset", this.engine.Reset());
            }
#endif
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
#if !__UseLuminatorTftDisplay
            this.engine.Dispose();
#endif
        }

        /// <summary>
        /// Configure this item.
        /// </summary>
        /// <param name="voice">
        /// The voice to be used.
        /// </param>
        /// <param name="text">
        /// The text to be spoken.
        /// </param>
        protected override void Configure(string voice, string text)
        {
#if __UseLuminatorTftDisplay
            Task speakTask = Task.Factory.StartNew(() =>
                {
                    lock (Locker)
                    {
                        this.acapelaVoice = voice;
                        this.textToSpeak = text;

                        HandleError(
                            string.Format("Open(\"{0}\")", voice),
                            AcapelaHelper.Engine.Open(voice, BabTtsOpenModes.BABTTS_USEDEFDICT));
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler);
#else
            this.acapelaVoice = voice;
            this.textToSpeak = text;

            HandleError(
                string.Format("Open(\"{0}\")", voice),
                this.engine.Open(voice, BabTtsOpenModes.BABTTS_USEDEFDICT));
#endif

        }

        private static void HandleError(string methodName, BabTtsError error)
        {
            if (error == BabTtsError.E_BABTTS_NOERROR)
            {
                return;
            }

#if __UseLuminatorTftDisplay
            Logger.Error("BabTTS.{0} returned an error: {1}", methodName, error);
            Logger.Info("Resetting Acapela TTS Engine.");
            AcapelaHelper.Engine.Reset();
            return;
#else
            throw new PlaybackException(string.Format("BabTTS.{0} returned an error: {1}", methodName, error));
#endif
        }
        
        private int EngineOnEnd()
        {
            Logger.Info("Acapela TTS Speak Prompt Ended");
            this.RaiseCompleted(EventArgs.Empty);
            return 0;
        }
    }
}
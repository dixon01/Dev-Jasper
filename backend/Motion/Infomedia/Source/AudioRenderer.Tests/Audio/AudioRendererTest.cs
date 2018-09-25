﻿


using System.Linq;

namespace AudioRenderer.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Xml.Serialization;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NAudio.Wave;

    using NLog;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Infomedia.AudioRenderer;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Motion.Infomedia.AudioRenderer;
    using Gorba.Motion.Infomedia.AudioRenderer.Playback;

    [TestClass]
    [DeploymentItem("../App.config")]
    public class AudioRendererTest
    {
        #region Constants

        private const string CONSTITUTION =
            "We the People of the United States, in Order to form a more perfect Union, establish Justice, insure domestic Tranquility, "
            + "provide for the common defense, promote the general Welfare, and secure the Blessings of Liberty to ourselves and our Posterity, "
            + "do ordain and establish this Constitution for the United States of America. "
            + "All legislative Powers herein granted shall be vested in a Congress of the United States, which shall consist of a Senate and House of Representatives. "
            + "The House of Representatives shall be composed of Members chosen every second Year by the People of the several States, "
            + "and the Electors in each State shall have the Qualifications requisite for Electors of the most numerous Branch of the State Legislature. "
            + "No Person shall be a Representative who shall not have attained to the Age of twenty five Years, "
            + "and been seven Years a Citizen of the United States, and who shall not, when elected, "
            + "be an Inhabitant of that State in which he shall be chosen.  "
            + "Representatives and direct Taxes shall be apportioned among the several States which may be included within this Union, "
            + "according to their respective Numbers, "
            + "which shall be determined by adding to the whole Number of free Persons, "
            + "including those bound to Service for a Term of Years, "
            + "and excluding Indians not taxed, three fifths of all other Persons. "
            + "The actual Enumeration shall be made within three Years after the first Meeting of the Congress of the United States, "
            + "and within every subsequent Term of ten Years, " + "in such Manner as they shall by Law direct. "
            + "The Number of Representatives shall not exceed one for every thirty Thousand, "
            + "but each State shall have at Least one Representative; and until such enumeration shall be made, "
            + "the State of New Hampshire shall be entitled to choose three, Massachusetts eight, "
            + "Rhode-Island and Providence Plantations one, Connecticut five, New-York six, New Jersey four, Pennsylvania eight, "
            + "Delaware one, Maryland six, Virginia ten, North Carolina five, South Carolina five, and Georgia three.  "
            + "When vacancies happen in the Representation from any State, the Executive Authority thereof shall issue Writs of Election to fill such Vacancies.  "
            + "The House of Representatives shall choose their Speaker and other Officers; and shall have the sole Power of Impeachment.  "
            + "The Senate of the United States shall be composed of two Senators from each State, chosen by the Legislature thereof, "
            + "for six Years; and each Senator shall have one Vote.  Immediately after they shall be assembled in Consequence of the first Election, "
            + "they shall be divided as equally as may be into three Classes. "
            + "The Seats of the Senators of the first Class shall be vacated at the Expiration of the second Year, "
            + "of the second Class at the Expiration of the fourth Year, and of the third Class at the Expiration of the sixth Year, "
            + "so that one third may be chosen every second Year; and if Vacancies happen by Resignation, "
            + "or otherwise, during the Recess of the Legislature of any State, "
            + "the Executive thereof may make temporary Appointments until the next Meeting of the Legislature, "
            + "which shall then fill such Vacancies.  No Person shall be a Senator who shall not have attained to the Age of thirty Years, "
            + "and been nine Years a Citizen of the United States, and who shall not, when elected, "
            + "be an Inhabitant of that State for which he shall be chosen.  The Vice President of the United States shall be President of the Senate, "
            + "but shall have no Vote, unless they be equally divided.  The Senate shall choose their other Officers, and also a President pro tempore, in the Absence of the Vice President, "
            + "or when he shall exercise the Office of President of the United States.  The Senate shall have the sole Power to try all Impeachments. "
            + "When sitting for that Purpose, they shall be on Oath or Affirmation. "
            + "When the President of the United States is tried, the Chief Justice shall preside: And no Person shall be convicted without the Concurrence of two thirds of the Members present.  "
            + "Judgment in Cases of Impeachment shall not extend further than to removal from Office, "
            + "and disqualification to hold and enjoy any Office of honor, "
            + "Trust or Profit under the United States: but the Party convicted shall nevertheless be liable and subject to Indictment, "
            + "Trial, Judgment and Punishment, according to Law.  The Times, "
            + "Places and Manner of holding Elections for Senators and Representatives, "
            + "shall be prescribed in each State by the Legislature thereof; but the Congress may at any time by Law make or alter such Regulations, "
            + "except as to the Places of choosing Senators.  The Congress shall assemble at least once in every Year, and such Meeting shall be on the first Monday in December, "
            + "unless they shall by Law appoint a different Day.  Each House shall be the Judge of the Elections, "
            + "Returns and Qualifications of its own Members, "
            + "and a Majority of each shall constitute a Quorum to do Business; but a smaller Number may adjourn from day to day, "
            + "and may be authorized to compel the Attendance of absent Members, in such Manner, "
            + "and under such Penalties as each House may provide.  Each House may determine the Rules of its Proceedings, "
            + "punish its Members for disorderly Behaviour, and, with the Concurrence of two thirds, expel a Member.  "
            + "Each House shall keep a Journal of its Proceedings, and from time to time publish the same, "
            + "excepting such Parts as may in their Judgment require Secrecy; and the Yeas and Nays of the Members of either House on any question shall, "
            + "at the Desire of one fifth of those Present, be entered on the Journal.  Neither House, during the Session of Congress, "
            + "shall, without the Consent of the other, adjourn for more than three days, "
            + "nor to any other Place than that in which the two Houses shall be sitting.  "
            + "The Senators and Representatives shall receive a Compensation for their Services, "
            + "to be ascertained by Law, and paid out of the Treasury of the United States. "
            + "They shall in all Cases, except Treason, "
            + "Felony and Breach of the Peace, be privileged from Arrest during their Attendance at the Session of their respective Houses, "
            + "and in going to and returning from the same; and for any Speech or Debate in either House, "
            + "they shall not be questioned in any other Place.  No Senator or Representative shall, "
            + "during the Time for which he was elected, be appointed to any civil Office under the Authority of the United States, "
            + "which shall have been created, "
            + "or the Emoluments whereof shall have been increased during such time; and no Person holding any Office under the United States, "
            + "shall be a Member of either House during his Continuance in Office.  "
            + "All Bills for raising Revenue shall originate in the House of Representatives; but the Senate may propose or concur with Amendments as on other Bills."
            + "Every Bill which shall have passed the House of Representatives and the Senate, "
            + "shall, before it become a Law, be presented to the President of the United States: If he approve he shall sign it, "
            + "but if not he shall return it, with his Objections to that House in which it shall have originated, "
            + "who shall enter the Objections at large on their Journal, "
            + "and proceed to reconsider it. If after such Reconsideration two thirds of that House shall agree to pass the Bill, "
            + "it shall be sent, together with the Objections, to the other House, "
            + "by which it shall likewise be reconsidered, and if approved by two thirds of that House, "
            + "it shall become a Law. But in all such Cases the Votes of both Houses shall be determined by Yeas and Nays, "
            + "and the Names of the Persons voting for and against the Bill shall be entered on the Journal of each House respectively. "
            + "If any Bill shall not be returned by the President within ten Days (Sundays excepted) after it shall have been presented to him, "
            + "the Same shall be a Law, in like Manner as if he had signed it, unless the Congress by their Adjournment prevent its Return, "
            + "in which Case it shall not be a Law.  Every Order, Resolution, "
            + "or Vote to which the Concurrence of the Senate and House of Representatives may be necessary (except on a question of Adjournment) shall be presented to the President of the United States; and before the Same shall take Effect, "
            + "shall be approved by him, or being disapproved by him, "
            + "shall be repassed by two thirds of the Senate and House of Representatives, "
            + "according to the Rules and Limitations prescribed in the Case of a Bill.  The Congress shall have Power To lay and collect Taxes, "
            + "Duties, Imposts and Excises, to pay the Debts and provide for the common Defence and general Welfare of the United States; but all Duties, "
            + "Imposts and Excises shall be uniform throughout the United States;";

        #endregion

        #region Fields

        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private FileConfigurator fileConfigurator;
        private TestContext testContext;

        private ConfigManager<AudioRendererConfig> audioConfigManager;
        private readonly List<AudioChannelHandler> audioChannels = new List<AudioChannelHandler>();
        private AudioChannelHandler interiorAudioChannel;
        private AudioChannelHandler exteriorAudioChannel;
        private AudioChannelHandler interiorExteriorAudioChannel;
        private AudioChannelConfig interiorAudioConfig;
        private AudioChannelConfig exteriorAudioConfig;
        private AudioChannelConfig interiorExteriorAudioConfig;
        private AudioPlayer audioPlayer;
        private PlayerEngine playerEngine;
        private DirectSoundOut wavePlayer;
        private int systemVolume;
        private string delimeter = " ";

        private const string AppConfigFilepath = @".\App.config";
        private const string AudioRendererConfigFilepath = @".\AudioRenderer.xml";
        private const string AudioMediConfigFilepath = @".\medi.config";
        private const string TempDir = @"C:\Temp";
        private readonly string SleepAwayMP3 = Path.Combine(TempDir, @"SleepAway.mp3");

        private const int TestPlaybackDuration = 8000;
        private const int TestPlaybackCooldown = 200;

        #endregion

        #region Public Properties
        #endregion
        
        public AudioRendererTest()
        {
            Assert.IsTrue(File.Exists(AppConfigFilepath));
            Assert.IsTrue(File.Exists(AudioMediConfigFilepath));
            
            Debug.WriteLine("AudioRendererTest.TestInitialize() - NLogger Output");

            // Load Medi configuration
            string mediXml = File.ReadAllText(AudioMediConfigFilepath);
            MediConfig mediConfig;

            using (var reader = new StringReader(mediXml))
            {
                mediConfig = (MediConfig)new XmlSerializer(typeof(MediConfig)).Deserialize(reader);
                MessageDispatcher.Instance.Configure(
                    new ObjectConfigurator(
                        mediConfig,
                        ApplicationHelper.MachineName,
                        string.Format("AppDomain-{0}", Guid.NewGuid())));
            }

            // Load the audio configuration
            audioConfigManager = new ConfigManager<AudioRendererConfig>();
            audioConfigManager.FileName = AudioRendererConfigFilepath;
            audioConfigManager.EnableCaching = true;
            audioConfigManager.XmlSchema = AudioRendererConfig.Schema;
            var config = audioConfigManager.Config;

            logger.Trace("*** AudioRendererTest() has {0} audio channels. ***", config.AudioChannels.Count);

            // 1 = Interior
            // 2 = Exterior
            // 3 = Interior and Exterior
            foreach (var audioChannelConfig in config.AudioChannels)
            {
                // Default the config name
                foreach (var port in audioChannelConfig.SpeakerPorts)
                    port.Unit = "localhost";

                var handler = new AudioChannelHandler();
                logger.Trace("AudioRendererTest() before handler.Configure().");
                handler.Configure(audioChannelConfig, config);
                audioChannels.Add(handler);

                switch (audioChannelConfig.Id)
                {
                    case "1":
                        interiorAudioConfig = audioChannelConfig;
                        interiorAudioChannel = handler;
                        break;

                    case "2":
                        exteriorAudioConfig = audioChannelConfig;
                        exteriorAudioChannel = handler;
                        break;

                    case "3":
                        interiorExteriorAudioConfig = audioChannelConfig;
                        interiorExteriorAudioChannel = handler;
                        break;
                }
            }

            playerEngine = new PlayerEngine();

            // We need to playback some short voice in order to initialize the Audio Codec
            this.InitializeAudioCodec();
            if (config.TextToSpeech != null && config.TextToSpeech.Api == TextToSpeechApi.Acapela)
            {
                logger.Trace("AudioRendererTest() initializing AcapelaHelper. HintPath={0}", config.TextToSpeech.HintPath);
                AcapelaHelper.Initialize(config.TextToSpeech.HintPath);
            }

            this.systemVolume = config.IO.VolumePort.Value;

            InitializeTestFiles();

            Debug.WriteLine("AudioRendererTest.TestInitialize() - Completed");
        }

        #region Public Methods and Operators

        [TestMethod]
        public void InteriorAudioChannel()
        {
            Debug.WriteLine("AudioRendererTest.InteriorAudioChannel() - Started");
            Assert.IsNotNull(interiorAudioConfig);
            Assert.IsNotNull(interiorAudioChannel);
            Assert.IsNotNull(playerEngine);

            audioPlayer = new AudioPlayer(audioConfigManager.Config);
            switch (audioConfigManager.Config.TextToSpeech.Api)
            {
                case TextToSpeechApi.Acapela:
                    audioPlayer.AddSpeech(AcapelaHelper.TestVoice, string.Format("Testing {0} Audio.", string.Join(delimeter, interiorAudioConfig.SpeakerPorts.Select(x => x.PortName))), 80);
                    break;
                case TextToSpeechApi.Microsoft:
                    audioPlayer.AddSpeech("", string.Format("Testing {0} Audio.", string.Join(delimeter, interiorAudioConfig.SpeakerPorts.Select(x => x.PortName))), 80);
                    break;
            }

            audioPlayer.AddFile(SleepAwayMP3, 80);
            playerEngine.Enqueue(1, audioPlayer, interiorAudioChannel.AudioIOHandler);

            logger.Info("Playing Interior Audio...");
            interiorAudioChannel.Start(playerEngine);
            System.Threading.Thread.Sleep(TestPlaybackDuration);
            audioPlayer.Stop();
            audioPlayer.Dispose();
            interiorAudioChannel.Stop();
            logger.Info("Interior Audio Stopped.");
            System.Threading.Thread.Sleep(TestPlaybackCooldown);

            Debug.WriteLine("AudioRendererTest.InteriorAudioChannel() - Completed");
        }

        [TestMethod]
        public void ExteriorAudioChannel()
        {
            Debug.WriteLine("AudioRendererTest.ExteriorAudioChannel() - Started");
            Assert.IsNotNull(exteriorAudioConfig);
            Assert.IsNotNull(exteriorAudioChannel);
            Assert.IsNotNull(playerEngine);

            audioPlayer = new AudioPlayer(audioConfigManager.Config);
            switch (audioConfigManager.Config.TextToSpeech.Api)
            {
                case TextToSpeechApi.Acapela:
                    audioPlayer.AddSpeech(AcapelaHelper.TestVoice, string.Format("Testing {0} Audio.", string.Join(delimeter, exteriorAudioConfig.SpeakerPorts.Select(x => x.PortName))), 80);
                    break;
                case TextToSpeechApi.Microsoft:
                    audioPlayer.AddSpeech("", string.Format("Testing {0} Audio.", string.Join(delimeter, exteriorAudioConfig.SpeakerPorts.Select(x => x.PortName))), 80);
                    break;
            }

            audioPlayer.AddFile(SleepAwayMP3, 80);
            playerEngine.Enqueue(1, audioPlayer, exteriorAudioChannel.AudioIOHandler);

            logger.Info("Playing Exterior Audio...");
            exteriorAudioChannel.Start(playerEngine);
            System.Threading.Thread.Sleep(TestPlaybackDuration);
            audioPlayer.Stop();
            audioPlayer.Dispose();
            exteriorAudioChannel.Stop();
            logger.Info("Interior Exterior Stopped.");
            System.Threading.Thread.Sleep(TestPlaybackCooldown);

            Debug.WriteLine("AudioRendererTest.ExteriorAudioChannel() - Completed");
        }

        [TestMethod]
        public void InteriorExteriorAudioChannel()
        {
            Debug.WriteLine("AudioRendererTest.InteriorExteriorAudioChannel() - Started");
            Assert.IsNotNull(interiorExteriorAudioConfig);
            Assert.IsNotNull(interiorExteriorAudioChannel);
            Assert.IsNotNull(playerEngine);

            audioPlayer = new AudioPlayer(audioConfigManager.Config);
            switch (audioConfigManager.Config.TextToSpeech.Api)
            {
                case TextToSpeechApi.Acapela:
                    audioPlayer.AddSpeech(AcapelaHelper.TestVoice, string.Format("Testing {0} Audio.", string.Join(delimeter, interiorExteriorAudioConfig.SpeakerPorts.Select(x => x.PortName))), 80);
                    break;
                case TextToSpeechApi.Microsoft:
                    audioPlayer.AddSpeech("", string.Format("Testing {0} Audio.", string.Join(delimeter, interiorExteriorAudioConfig.SpeakerPorts.Select(x => x.PortName))), 80);
                    break;
            }

            audioPlayer.AddFile(SleepAwayMP3, 80);
            playerEngine.Enqueue(1, audioPlayer, interiorExteriorAudioChannel.AudioIOHandler);

            logger.Info("Playing Interior Exterior Audio...");
            interiorExteriorAudioChannel.Start(playerEngine);
            System.Threading.Thread.Sleep(TestPlaybackDuration);
            audioPlayer.Stop();
            audioPlayer.Dispose();
            interiorExteriorAudioChannel.Stop();
            logger.Info("Interior Interior Exterior Stopped.");
            System.Threading.Thread.Sleep(TestPlaybackCooldown);

            Debug.WriteLine("AudioRendererTest.InteriorExteriorAudioChannel() - Completed");
        }

       // [TestMethod]
        public void LongAcapelaTest()
        {


#if !DEBUG
            return;
#else

            Debug.WriteLine("AudioRendererTest.LongAcapelaTest() - Started");
            


            Debug.WriteLine("AudioRendererTest.LongAcapelaTest() - Completed");
#endif
        }

        #endregion

        #region Methods

        private void InitializeAudioCodec()
        {
            var reader = new Mp3FileReader(this.GetResourceStream("Resources.point1sec.mp3"));
            this.wavePlayer = new DirectSoundOut();
            this.wavePlayer.Init(reader);
            this.wavePlayer.PlaybackStopped += this.FinishedCodecInitialization;
            this.wavePlayer.Play();
        }

        private void InitializeTestFiles()
        {
            if (!Directory.Exists(TempDir))
            {
                Directory.CreateDirectory(TempDir);
            }

            if (!File.Exists(SleepAwayMP3))
            {
                using (Stream resource = this.GetResourceStream("Resources.SleepAway.mp3"))
                {
                    using (var file = new FileStream(SleepAwayMP3, FileMode.Create, FileAccess.Write))
                    {
                        resource.CopyTo(file);
                    }
                }
            }
        }
        
        private Stream GetResourceStream(string fileName)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resname = asm.GetName().Name + "." + fileName;
            return asm.GetManifestResourceStream(resname);
        }

        private void FinishedCodecInitialization(object sender, EventArgs eventArgs)
        {
            this.wavePlayer.Stop();
            this.wavePlayer.Dispose();
            this.logger.Info("Finished codec initialization", eventArgs.ToString());
        }

        #endregion

    }
}

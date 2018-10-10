// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AcapelaHelper.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AcapelaHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer.Playback
{
    using System;
    using System.IO;
    using System.Linq;

    using AcapelaGroup.BabTTSNet;

    using NLog;
    
    using Gorba.Common.Configuration.Infomedia.AudioRenderer;

    /// <summary>
    /// Helper class to initialize Acapela TTS.
    /// </summary>
#if __UseLuminatorTftDisplay
    public static class AcapelaHelper
#else
        
    internal static class AcapelaHelper
#endif

    {
        private const string License =
            "#REVT30330NsH7oktT4RU6jZQIdJ8J9F8J45d7!EF7asfVjRAGAkvWVEXQjFWQTgV5ysVY%wTUcEt4aEd"
            + "5jNu5qQRXtAcMeF8MetAKcZAKcZ8McRoI8xDGCscYrcQI4RGYxAtLcFAJ4Zu58cQIe5QIexTSSUdZvc"
            + "GGSUdZvcQ674tVBIH7aNu648V5vIuMwoWIVot4n$tVxwURFo8XbdH5pUH58FpRxkHIaZAYvwc7nRuJoE"
            + "8Z9UdJR@tKPouKkAcS$MX7RYUJ8J8NEoc6zAfW5YA6SMAYqMWX8sWZD4cV8MdZWAFWw@VWUMv5c8H7!U"
            + "EYa$d2YwEJHYvR7UpXdYQWeAs6adF7msuQrY87vBBRvQeUm4AWtsvSCQG2pIsRTAuXQURXoUW4fouR$Y"
            + "dSXgVYaMXYeAG2OEXXaNXWGou6HUB4yId2oQfYbpo52sH7wMoXsAF66ctNfQc5UEAWQUpXvJfWDUGVXIE"
            + "I6AsU9cFRmAV4nBtV$4vWqERWeYFSsIHWWMWRRYQ62Ep5ZUG6ZIvSSUU7pIE4rItMxMQJ!A9KWUt6bRvJ@"
            + "QH7OQUVTsG77wFWr4cW6EQVFAu6dgXQaJdU8tuXzIdNzIWVW4s2rtEQSUB4qA9NnIuZvNfZ7AfN9AtRRw"
            + "uJdsER6YE5F8F4yQUX$oG5R@HJmIs7TEoQE@fZA4vU$YV4ZwW7nEFYaEG4qVpQPQA4vUoU7weU8sQZ7oF"
            + "UXoVVt4W4F8dVHItYwAHQyMo6rVR76MfXWAQIUQW53s82cAe2BYH5kst4rYc7tYVVcEE7QwsK!EF5!ERY"
            + "xQvMoEt68YfKMsQ5eUR5B8dYSQu7vYoWeUv2r5FQoE82rNA4qse6$YWJCkfR8Ud6EE8YqVRWrFvSQMu6B"
            + "Ap2mNVI$QfS24E4CMsQ9pcVREUQt8VVDIvUqYeSTEB4SUH2R4sXO4sZRsdWWcV5Q4fKwo8KcB9XGQEZRE"
            + "pQSwfZ5@XRHsf2bBQ4aJsZdIVV7Me285oXkwFW8QWQRsF2E8HIq8f7TQeXYgtUdgXI!MESYoHWGAvYQkX"
            + "Y9lvN@sARdwuRF4EX9kX5TcX6UIcUzA8Wmdv52oUVA4d7W4vWmxX7bMcJZsAWSABZ%QWZ8YFQcVtIdpVQ"
            + "3QfVkQvJQwERtARW!4t4BUFSAEX4XQEQAAF5esXQrlH2F4F288d7MgvVDcFSyIE6mFtJoEEVd5H6aIe6o"
            + "Ys22UeVZIoK$IuK3E97mIoWeoHX9Qc7GI8U6UuM$MvMGAsSv5cZDAWMtxb#";

        private const string evalLicense =
            "kIcEWdZfZrv!s7rVDQefnOExPN7@w5OOmYJfuKS9A$OCz2tLCVSQvdctF4HGQIAvG3W!L2S3GcsmCJxl"
            + "Edny%ft4FGhyBxC$@6Nrd2HFaKXHJWJVNZB$dqBf2KNITHQGIHPkv5ByhTKMF8YyBNHhcsfxC2jheT"
            + "OV!WddJC8qEm6uJN!I7SiQQVbaz!7ZMu923ewKWi%xZR8ngNxg%QMhB7XvEdRQX3%@tZOdfviFHCVC"
            + "%3mLup$d7EbqLRPKSu6C!Ylxpq$cW@IOtrpRYbGMaaU59zPyYekZVwXS%OQZrO84JXv@rj%EZyPM4%"
            + "GPm96Tppc2WsV9enz3NMHofKpS3aP7Hl!6GjjZnpuc7lIDe@Lw3jFzx3Zv2hJvusZFw2EsqzHunTxE"
            + "ORuGzNjctVE78691k6gxRelCJTNXaziiRvpC4sBjCneRx$fwslZ4eGQ8hTL5ccwMscZoeMtr8WClE!"
            + "I%RU6Aq%LuaGSp64otZZKAgbMBMynE%6EWCO@ZfldhUGAqFA3rYpC4%@aSHXmugsWNjuVXNKBakwj3"
            + "gYlRb5op7mTXiQ7E2QzOWkdvSEMzOtnil6gpl34sfJUF%2jYUIKN5C6uJ@f!x!d792jFQkMD57vd@b"
            + "kk2QM!ezs@tui7IVu3vijXoaVdyfJLIQjYtovxHd7Ha%ccu8wangPUwVHg%%ASbSdaoHCza9p$tUZ4"
            + "$my66P$tH7Fe456IuISdV9tdaxZ3u@9SYIC%mF8seM34pL7izzQIjSoK@QbmxL!fa5fe9OzjjId7CS"
            + "$YHCMcVRVvMTtyLYtdiTG5cAXb5mtkra4YaLPrkS##";

        private const string AcaTtsDllName = "AcaTTS.dll";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly string[] AcapelaLocations =
            {
                "C:\\Program Files\\Acapela Group\\Acapela TTS for Windows",
                "C:\\Program Files (x86)\\Acapela Group\\Acapela TTS for Windows",
                "D:\\Progs\\Acapela"
            };

        private static readonly object Locker = new object();
        private static bool initialized;

        /// <summary>
        /// Gets a value indicating whether Acapela is available on the current system.
        /// </summary>
        public static bool Available { get; private set; }

#if __UseLuminatorTftDisplay
        public static BabTTS Engine { get; private set; }
        
        public const string TestVoice = "Sharon22k_HQ";
        
#endif

        /// <summary>
        /// Initializes Acapela, providing a hint where <code>AcaTTS.dll</code>
        /// could be located on the local file system.
        /// </summary>
        /// <param name="acapelaPathHint">
        /// The directory where <code>AcaTTS.dll</code> is installed.
        /// This can be null or empty, in this case, we will search in the default locations.
        /// </param>
        public static void Initialize(string acapelaPathHint)
        {
            if (initialized)
            {
                return;
            }

            lock (Locker)
            {
                if (initialized)
                {
                    return;
                }

                try
                {
                    SetupAcaTtsPath(acapelaPathHint);
                    LoadLicense();

#if __UseLuminatorTftDisplay
                    Engine = new BabTTS();
#endif

                    Available = true;
                    initialized = true;

#if __UseLuminatorTftDisplay
#if DEBUG
                    //SpeechItemBase item = SpeechItemBase.Create(
                    //    TextToSpeechApi.Acapela,
                    //    TestVoice,
                    //    "Acapela 1.",
                    //    10);
                    //item.Start();

                    //SpeechItemBase item2 = SpeechItemBase.Create(
                    //    TextToSpeechApi.Acapela,
                    //    TestVoice,
                    //    "Acapela 2.",
                    //    10);
                    //item2.Start();

                    //SpeechItemBase item3 = SpeechItemBase.Create(
                    //    TextToSpeechApi.Acapela,
                    //    TestVoice,
                    //    "Acapela 3.",
                    //    10);
                    //item3.Start();
#endif
#endif
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Couldn't initialize Acapela");
                }
            }
        }
        
        private static void SetupAcaTtsPath(string acapelaPathHint)
        {
            if (!string.IsNullOrEmpty(acapelaPathHint))
            {
                if (FindAcaTtsDll(acapelaPathHint))
                {
#if __UseLuminatorTftDisplay
                    Logger.Trace("SetupAcaTtsPath() - found path '{0}'", acapelaPathHint);
#endif
                    return;
                }
            }

            var paths = "";
            foreach (var path in AcapelaLocations)
            {
              paths = string.Concat(paths, $"      \'{path}\'\n");
            }

            Logger.Trace("SetupAcaTtsPath() - paths are:\n{0}.", paths);
            if (AcapelaLocations.Any(FindAcaTtsDll))
            {
                return;
            }

            throw new DllNotFoundException("Couldn't find " + AcaTtsDllName);
        }

        private static bool FindAcaTtsDll(string directory)
        {
            var path = Path.Combine(directory, AcaTtsDllName);
            
            if (!File.Exists(path))
            {
                return false;
            }

            Helpers.AcaTTSPath = path;
            return true;
        }

        private static void LoadLicense()
        {
            Helpers.Bundle(License);
        }
    }
}
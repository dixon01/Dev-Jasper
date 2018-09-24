// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="AudioSwitchEchoProgram.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace AudioSwitchEchoConsoleApp
{
    using System;
    using System.Linq;

    using AudioSwitchEchoConsoleApp.Properties;


    /// <summary>The audio switch echo program.</summary>
    internal class AudioSwitchEchoProgram
    {
        #region Methods

        private static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            bool replayWithAck = args.Length > 0 && (args.Contains("/e") || args.Contains("/E"));
            var audioSwitchEchoTest = new AudioSwitchEchoTest(Settings.Default.ComPort, Settings.Default.BaudRate) { ReplayWithAck = replayWithAck };
            audioSwitchEchoTest.Run();
            if (replayWithAck)
            {
                Console.WriteLine("Echo all received message with Perip[eral Ack Enabled");
            }
            Console.WriteLine("Enter any Key to Exit");
            Console.ReadLine();
            audioSwitchEchoTest.Stop();
        }

        #endregion
    }
}
namespace Luminator.AudioSwitch.EnableAudio
{
    using System;
    using System.IO;

    using CLAP;

    using Luminator.AudioSwitch.EnableAudio.Properties;
    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Types;

    public class AudioSwitchCmdTool
    {
        public static AudioSwitchSerialClient audioSwitchSerialClient;
        public static bool IsConnected { get; set; }
        public static SerialPortInfo SelectedComPort { get; set; }
        private static void ConnectToAudioSwitch()
        {
            try
            {
                //Connect to AudioSwitch
                if (SelectedComPort != null)
                {
                    var settings = new SerialPortSettings(SelectedComPort.Name);
                    audioSwitchSerialClient = new AudioSwitchSerialClient(settings);
                    if (audioSwitchSerialClient.IsComPortOpen)
                    {
                        IsConnected = true;
                    }
                }
            }
            catch (IOException ioException)
            {
                Console.WriteLine(ioException.Message);
            }
            catch (UnauthorizedAccessException exception)
            {
                Console.WriteLine(exception.Message);
            }

        }

        [Verb]
        public void ToolBegin(bool enable)
        {
            try
            {
                SelectedComPort = new SerialPortInfo { Name = Settings.Default.ComPortToUse };
                ConnectToAudioSwitch();
                if (audioSwitchSerialClient != null)
                {
                    audioSwitchSerialClient.WriteAudioEnabled(Settings.Default.AudioEnable ? ActiveSpeakerZone.Both : ActiveSpeakerZone.None);
                    Console.WriteLine("WriteAudioEnabled Called Successfully...");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Parser.RunConsole<AudioSwitchCmdTool>(args);
        }
    }

    
}

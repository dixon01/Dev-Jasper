// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the main program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.ComPortChecker
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Ports;
    using System.Threading;

    using Gorba.Motion.Protran.Tools.ComPortChecker.Properties;

    /// <summary>
    /// Main program
    /// </summary>
    public class Program
    {
        private static StreamWriter logFile;

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            OpenLog();
            try
            {
                CheckPorts();
            }
            catch (Exception ex)
            {
                Log("Unhandled Exception: {0}", ex);
            }
            finally
            {
                if (logFile != null)
                {
                    logFile.Close();
                }
            }
        }

        private static void CheckPorts()
        {
            int successCount = 0;
            int startTicks = Environment.TickCount;
            while (true)
            {
                bool success = true;
                try
                {
                    foreach (var portName in Settings.Default.ComPorts)
                    {
                        Log("Creating {0}", portName);
                        success &= OpenPort(portName);
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                    success = false;
                }

                if (success)
                {
                    successCount++;
                    if (successCount >= Settings.Default.SuccessCount)
                    {
                        // if we were successful, we still have to try [SuccessCount]
                        // times to be sure the COM port isn't taken from us again
                        Log("Sucessfully opened all ports {0} times", successCount);
                        break;
                    }
                }
                else if (successCount > 0)
                {
                    Log("Failed again after {0} times successful", successCount);
                    successCount = 0;
                }

                if (Settings.Default.RestartTimeOut > 0
                    && Environment.TickCount - startTicks >= Settings.Default.RestartTimeOut)
                {
                    Reboot();
                    return;
                }

                Log("{0}, let's wait {1} ms", success ? "Sucessful" : "Not successful", Settings.Default.SleepTime);
                Thread.Sleep(Settings.Default.SleepTime);
            }

            Log("All ports are now available");
            if (File.Exists(Settings.Default.StartApp))
            {
                Log("Starting {0}", Settings.Default.StartApp);
                Process.Start(Settings.Default.StartApp);
            }
        }

        private static bool OpenPort(string portName)
        {
            bool success;
            using (var port = new SerialPort(portName))
            {
                port.Parity = Parity.None;
                port.Handshake = Handshake.None;
                port.BaudRate = 2400;
                Log("Opening {0}", portName);
                port.Open();
                Log("Opened {0}: {1}", portName, port.IsOpen);
                success = port.IsOpen;
            }

            return success;
        }

        private static void Reboot()
        {
            Log("Rebooting now: {0} {1}", Settings.Default.RestartCommand, Settings.Default.RestartArguments);
            Process.Start(Settings.Default.RestartCommand, Settings.Default.RestartArguments);
        }

        private static void OpenLog()
        {
            if (!Settings.Default.Logging)
            {
                return;
            }

            logFile = File.AppendText("ComPortChecker.log");
            Log("===== Started ComPortChecker {0} =====", typeof(Program).Assembly.GetName().Version);
        }

        private static void Log(string format, params object[] args)
        {
            if (logFile == null)
            {
                return;
            }

            logFile.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff "));
            logFile.WriteLine(format, args);
            logFile.Flush();
        }
    }
}

using System;

namespace WindowsMediaPlayerTest
{
    using System.Windows.Forms;

    using Vlc.DotNet.Core;

#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //RunAxVideoLan();
            //RunVideoLanDotNet();
            RunVideoLanClientGame();
            //RunWindowsMediaPlayerGame();
        }

        private static void RunVideoLanClientGame()
        {
            using (var game = new VideoLanClientGame())
            {
                game.Run();
            }
        }

        private static void RunWindowsMediaPlayerGame()
        {
            using (var game = new WindowsMediaPlayerGame())
            {
                game.Run();
            }
        }

        private static void RunVideoLanDotNet()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Set libvlc.dll and libvlccore.dll directory path
            VlcContext.LibVlcDllsPath = CommonStrings.LIBVLC_DLLS_PATH_DEFAULT_VALUE_AMD64;
            // Set the vlc plugins directory path
            VlcContext.LibVlcPluginsPath = CommonStrings.PLUGINS_PATH_DEFAULT_VALUE_AMD64;

            // Set the startup options
            VlcContext.StartupOptions.IgnoreConfig = true;
            VlcContext.StartupOptions.LogOptions.LogInFile = true;
            VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = true;
            VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.Debug;

            // Initialize the VlcContext
            VlcContext.Initialize();

            Application.Run(new VideoLanDotNetForm());

            // Close the VlcContext
            VlcContext.CloseAll(); 
        }

        private static void RunAxVideoLan()
        {
            Application.Run(new AxVideoLanClientForm());
        }
    }
#endif
}


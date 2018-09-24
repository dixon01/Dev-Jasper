// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="Program.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UpdateManagerFtpFolderCheck
{
    using System;
    using System.Linq;
    using System.Reflection;

    using CommandLine;

    using Gorba.Motion.Update.Core.Tests.UpdateClient;

    /// <summary>The program.</summary>
    internal class Program
    {
        #region Methods
        // Default cmdline args Used:
        // -H10.210.1.234 -PAsdf1234 -FResources -Ugorba
        // 
        private static bool DirectoryExists(FtpTestOptions ftpTestOptions)
        {
            return UnitTestFtpClient.DirectoryExists(ftpTestOptions.Host, ftpTestOptions.Path, ftpTestOptions.UserName, ftpTestOptions.Password);
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Ftp Folder test Version " + Assembly.GetEntryAssembly().GetName().Version);
            var ftpTestOptions = new FtpTestOptions(Environment.MachineName, "/", UnitTestFtpClient.UserName, UnitTestFtpClient.Password);
            if (args.Length > 0 && !args.Contains("?"))
            {
                var result = Parser.Default.ParseArguments(args, ftpTestOptions);
                if (result)
                {
                    result = DirectoryExists(ftpTestOptions);
                }
            }
            else
            {
                Console.WriteLine("Arguments!");
                Console.WriteLine("-H = Host Name.");
                Console.WriteLine("-F = Folder Default=" + ftpTestOptions.Path);
                Console.WriteLine("-U = Ftp UserName. Default=" + ftpTestOptions.UserName);
                Console.WriteLine("-P = Ftp Password. Default=" + ftpTestOptions.Password);
                Console.WriteLine("Example: -H10.210.1.234 -FResources -PAsdf1234 -Ugorba");
            }

            Console.WriteLine("Done - Enter any key to exit.");
            Console.ReadLine();
        }

        #endregion
    }

    /// <summary>The ftp test options.</summary>
    internal class FtpTestOptions
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="FtpTestOptions"/> class.</summary>
        /// <param name="host">The host.</param>
        /// <param name="path">The path.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        public FtpTestOptions(string host, string path, string userName = UnitTestFtpClient.UserName, string password = UnitTestFtpClient.Password)
        {
            this.Host = host;
            this.Path = path;
            this.Password = password;
            this.UserName = userName;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the host.</summary>
        [Option('H', HelpText = "Ftp Host IP.")]
        public string Host { get; set; }

        /// <summary>Gets or sets the password.</summary>
        [Option('P', HelpText = "Ftp password.", DefaultValue = "Asdf1234")]
        public string Password { get; set; }

        /// <summary>Gets or sets the path.</summary>
        [Option('F', HelpText = "Ftp Folder", DefaultValue = "/")]
        public string Path { get; set; }

        /// <summary>Gets or sets the user name.</summary>
        [Option('U', HelpText = "Ftp UserName.", DefaultValue = "gorba")]
        public string UserName { get; set; }

        #endregion
    }
}
namespace ServerInstaller.CustomAction
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    /// **** HERE ARE SOME USEFUL TIPS.
    /// ---- Parameters
    /// To create new parameters (or modify), first go to the dialog (User Interface) and make sure you have parameters listed there, and that they
    ///     are visible. Like..Edit1 visible, etc. Then View Custom Actions, select the commit primary node, and look at its properties. There is a parameter string there.
    ///     Ex: /PORT="[PORT]" /BACKGROUNDSYSTEMPORTAL="[BACKGROUNDSYSTEMPORTAL]" /DataSource="[SERVERNAME]"  /DatabaseName="[DATABASENAME]"
    ///     This assings the dialog values to the parameter value.
    /// 
    /// If you modify parameters in a dialog, be sure to View Custom Actions (right click on server installer, view custom actions)
    ///     and select the commit primary output node. Looks at its properties, there you will see how the installer "passes" the parameter data to the internal dictionary.
    ///     Do this if you are not seeing your new parameters defined in Context.Parameters.
    /// 
    /// Current Parameters:
    /// Dictionary Key            Dialog Parameter Name
    /// 
    /// PORT                      PORT
    /// BACKGROUNDSYSTEMPORTAL    BACKGROUNDSYSTEMPORTAL
    /// DataSource                SERVERNAME
    /// DatabaseName              DATABASENAME
    /// UserID                    USERID
    /// Password                  PASSWORD
    /// ApiHostPort               APIHOSTPORT
    /// PPLSqlConnection          PPLSQLCONNECTION
    /// PresImportWatchFolder     PRESIMPORTWATCHFOLDER
    /// PresImportInvalidFolder   PRESIMPORTINVALIDFOLDER
    ///
    /// Installation Errors:
    /// If you get an error Error 1001 Usage...with the command line parameter options,
    /// There is an issue with the default values you put in the edit fields for parameters
    /// having an ending slash (like in a directory path).
    ///
    /// If you want to get installer logs, run the installer from a command line, like this:
    /// msiexec /i ServerInstaller.msi /L*V .\Log.txt


    /// <summary>
    /// The server installer.
    /// </summary>
    [RunInstaller(true)]
    public partial class ServerInstaller : System.Configuration.Install.Installer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerInstaller"/> class.
        /// </summary>
        public ServerInstaller()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Completes the install transaction.
        /// </summary>
        /// <param name="savedState">An System.Collections.IDictionary that contains the state of the computer after all the installers in the collection have run.</param>
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            try
            {
                this.PerformPortalConfiguration();
                this.PerformSystemConfiguration();
                this.PerformClickOnceDeployment();
                this.PerformApiHostPortConfiguration();
                this.PerformSystemManagerConfiguration();
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to update the application configuration file: " + e.Message);
                base.Rollback(savedState);
            }
        }

        /// <summary>
        /// Update the background system 
        /// </summary>
        public void PerformApiHostPortConfiguration()
        {
            string apiHostPort = this.GetParameterValue("ApiHostPort");

            XmlDocument doc = new XmlDocument();
            doc.Load(this.BackgroundSystemConfigurationPath);

            UpdateConfigValue(doc, "ApiHostPort", apiHostPort);
            doc.Save(this.BackgroundSystemConfigurationPath);
        }

        private void PerformSystemManagerConfiguration()
        {
            var pplConnection = this.GetParameterValue("PPLSQLCONNECTION");
            var importWatchFolder = this.GetParameterValue("PRESIMPORTWATCHFOLDER");
            var invalidFileFolder = this.GetParameterValue("PresImportInvalidFolder");
            
            var doc = new XmlDocument();
            doc.Load(this.PresentationLoggingExeConfigPath);

            UpdateConfigValue(doc, "FileWatchFolder", importWatchFolder);
            UpdateConfigValue(doc, "BadFileFolder", invalidFileFolder);
            UpdateConfigValue(doc, "ConnectionStringDebug", pplConnection);
            UpdateConfigValue(doc, "ConnectionStringRelease", pplConnection);
            
            doc.Save(this.PresentationLoggingExeConfigPath);
        }

        public string PresentationLoggingExeConfigPath
        {
            get
            {
                string presPath = Path.Combine(this.AppsPath, "PresentationPlayLogging");
                string presConfigPath = Path.Combine(presPath, "PresentationPlayLoggingConfig.xml");

                return presConfigPath;
            }
        }

        public string SystemManagerConfigurationPath
        {
            get
            {
                string systemManagerPath = Path.Combine(this.AppsPath, "SystemManager");
                string configurationPath = Path.Combine(systemManagerPath, "SystemManager.xml");
                
                return configurationPath;
            }
        }

        public string BackgroundSystemConfigurationPath
        {
            get
            {
                string portalPath = Path.Combine(this.AppsPath, "Portal");
                string backgroundsystemConfiguration = Path.Combine(portalPath, @"Website\BackgroundSystemConfiguration.xml");

                return backgroundsystemConfiguration;
            }
        }

        /// <summary>
        /// The location of the Apps folder.
        /// </summary>
        public string AppsPath
        {
            get
            {
                string appPath = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).FullName;
                string configPath = Path.Combine(appPath, "");
                return configPath;
            }
        }

        /// <summary>
        /// Unzip the desktop deployments and use mage.exe to deploy the apps as a click-once
        /// </summary>
        public void PerformClickOnceDeployment()
        {
            string targetProcessor = "x86";
            string publisherName = "Luminator Technology Group";
            string appPath =
                Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                    .FullName;

            // Default the version to same as the portal executable version
            string assemblyPath = Path.Combine(appPath, "SystemManager", "SystemManager.exe");
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assemblyPath);
            string assemblyVersion = fileVersionInfo.ProductVersion;
            string assemblyVersionAbrev = assemblyVersion.Replace('.', '_');

            string adminSourcePath = Path.Combine(appPath, "Install", "Admin", "setup.zip");
            string diagSourcePath = Path.Combine(appPath, "Install", "Diag", "setup.zip");
            string mediaSourcePath = Path.Combine(appPath, "Install", "Media", "setup.zip");

            string adminReleasePath = Path.Combine(appPath, "Install", "Admin", "Release");
            string diagReleasePath = Path.Combine(appPath, "Install", "Diag", "Release");
            string mediaReleasePath = Path.Combine(appPath, "Install", "Media", "Release");

            if (Directory.Exists(adminReleasePath)) Directory.Delete(adminReleasePath, true);
            if (Directory.Exists(diagReleasePath)) Directory.Delete(diagReleasePath, true);
            if (Directory.Exists(mediaReleasePath)) Directory.Delete(mediaReleasePath, true);

            string adminUnzipPath = Path.Combine(
                adminReleasePath,
                "Application Files",
                "icenter.admin_" + assemblyVersionAbrev);
            string diagUnzipPath = Path.Combine(
                diagReleasePath,
                "Application Files",
                "icenter.diag_" + assemblyVersionAbrev);
            string mediaUnzipPath = Path.Combine(
                mediaReleasePath,
                "Application Files",
                "icenter.media_" + assemblyVersionAbrev);

            ZipFile.ExtractToDirectory(adminSourcePath, adminUnzipPath);
            ZipFile.ExtractToDirectory(diagSourcePath, diagUnzipPath);
            ZipFile.ExtractToDirectory(mediaSourcePath, mediaUnzipPath);

            string adminManifestFilename = "icenter.admin.exe.manifest";
            string adminApplicationFilename = "icenter.admin.application";
            string diagManifestFilename = "icenter.diag.exe.manifest";
            string diagApplicationFilename = "icenter.diag.application";
            string mediaManifestFilename = "icenter.media.exe.manifest";
            string mediaApplicationFilename = "icenter.media.application";

            // Create the batch file to perform all this
            string createManifestBat = Path.Combine(appPath, "Install", "createManifestBat.bat");
            string createDeployBat = Path.Combine(appPath, "Install", "createDeployBat.bat");
            string mageToolsPath = Path.Combine(appPath, "Tools", "Mage", "mage.exe");
            string newAppCmd = mageToolsPath
                               + " -New Application -FromDirectory \"{0}\" -Name \"{1}\" -Version {2} -Processor \"{3}\" -IconFile \"{4}\" -ToFile \"{5}\"";
            string newDeployCmd = mageToolsPath
                                  + " -New Deployment -Name \"{0}\" -Install {1} -Processor \"{2}\" -Publisher \"{3}\" -ProviderUrl \"{4}\" -AppManifest \"{5}\" -ToFile \"{6}\" -Version {7}";

            string line = string.Empty;

            if (File.Exists(createManifestBat)) File.Delete(createManifestBat);

            using (StreamWriter writer = new StreamWriter(createManifestBat))
            {
                line = string.Format(
                    newAppCmd,
                    adminUnzipPath,
                    "icenter.admin",
                    assemblyVersion,
                    targetProcessor,
                    "admin.ico",
                    Path.Combine(adminUnzipPath, adminManifestFilename));

                writer.WriteLine(line);

                line = string.Format(
                    newAppCmd,
                    diagUnzipPath,
                    "icenter.diag",
                    assemblyVersion,
                    targetProcessor,
                    "diag.ico",
                    Path.Combine(diagUnzipPath, diagManifestFilename));

                writer.WriteLine(line);

                line = string.Format(
                    newAppCmd,
                    mediaUnzipPath,
                    "icenter.media",
                    assemblyVersion,
                    targetProcessor,
                    "media.ico",
                    Path.Combine(mediaUnzipPath, mediaManifestFilename));

                writer.WriteLine(line);
            }

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            process.StartInfo = startInfo;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + createManifestBat;
            process.Start();
            process.WaitForExit();

            this.DeployFileExtensions(adminUnzipPath, adminManifestFilename);
            this.DeployFileExtensions(diagUnzipPath, diagManifestFilename);
            this.DeployFileExtensions(mediaUnzipPath, mediaManifestFilename);

            if (File.Exists(createDeployBat)) File.Delete(createDeployBat);

            using (StreamWriter writer = new StreamWriter(createDeployBat))
            {
                // C:\Apps\Tools\Mage\mage.exe -New Deployment -Processor "x86" -Install true 
                // -Publisher "My Co." -ProviderUrl "http://localhost/ClickOnce/Admin" 
                // -AppManifest C:\Apps\Install\Admin\Release\icenter.admin.exe.manifest 
                // -ToFile icenter.admin.application
                line = string.Format(
                    newDeployCmd,
                    "icenter.admin",
                    "true",
                    targetProcessor,
                    publisherName,
                    this.Context.Parameters["BACKGROUNDSYSTEMPORTAL"].Trim().TrimEnd('/'),
                    Path.Combine(adminUnzipPath, adminManifestFilename),
                    Path.Combine(adminReleasePath, adminApplicationFilename),
                    assemblyVersion);
                writer.WriteLine(line);

                line = string.Format(
                    newDeployCmd,
                    "icenter.diag",
                    "true",
                    targetProcessor,
                    publisherName,
                    this.Context.Parameters["BACKGROUNDSYSTEMPORTAL"].Trim().TrimEnd('/'),
                    Path.Combine(diagUnzipPath, diagManifestFilename),
                    Path.Combine(diagReleasePath, diagApplicationFilename),
                    assemblyVersion);
                writer.WriteLine(line);

                line = string.Format(
                    newDeployCmd,
                    "icenter.media",
                    "true",
                    targetProcessor,
                    publisherName,
                    this.Context.Parameters["BACKGROUNDSYSTEMPORTAL"].Trim().TrimEnd('/'),
                    Path.Combine(mediaUnzipPath, mediaManifestFilename),
                    Path.Combine(mediaReleasePath, mediaApplicationFilename),
                    assemblyVersion);
                writer.WriteLine(line);
            }

            startInfo.Arguments = "/C " + createDeployBat;
            process.Start();
            process.WaitForExit();

            this.MapFileExtensions(adminReleasePath, adminApplicationFilename);
            this.MapFileExtensions(diagReleasePath, diagApplicationFilename);
            this.MapFileExtensions(mediaReleasePath, mediaApplicationFilename);

            string clickOncePath = Path.Combine(appPath, "Portal", "WebSite", "StaticContent", "ClickOnce");
            string clickOnceAdminPath = Path.Combine(clickOncePath, "Admin");
            string clickOnceDiagPath = Path.Combine(clickOncePath, "Diag");
            string clickOnceMediaPath = Path.Combine(clickOncePath, "Media");

            if (Directory.Exists(clickOnceAdminPath)) Directory.Delete(clickOnceAdminPath, true);
            if (Directory.Exists(clickOnceDiagPath)) Directory.Delete(clickOnceDiagPath, true);
            if (Directory.Exists(clickOnceMediaPath)) Directory.Delete(clickOnceMediaPath, true);

            this.DirectoryCopy(adminReleasePath, clickOnceAdminPath);
            this.DirectoryCopy(diagReleasePath, clickOnceDiagPath);
            this.DirectoryCopy(mediaReleasePath, clickOnceMediaPath);

            // Cleanup
            Directory.Delete(adminReleasePath, true);
            Directory.Delete(diagReleasePath, true);
            Directory.Delete(mediaReleasePath, true);
            File.Delete(createManifestBat);
            File.Delete(createDeployBat);

            try
            {
                string backgroundSystemResourcesPath = Path.Combine(appPath, "BackgroundSystem", "Resources");
                string logPath = @"C:\Log";

                if (!Directory.Exists(backgroundSystemResourcesPath))
                {
                    Directory.CreateDirectory(backgroundSystemResourcesPath);
                }

                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
            }
            catch
            {
                // oh well
            }
        }
        
        /// <summary>
        /// Update the xml file and add .deploy extension since this is a manual step
        /// </summary>
        /// <param name="manifestPath"></param>
        /// <param name="manifestFileName"></param>
        private void DeployFileExtensions(string manifestPath, string manifestFileName)
        {
            List<FileInfo> files = new List<FileInfo>();
            this.GetFiles(new DirectoryInfo(manifestPath), ref files);

            foreach (FileInfo file in files)
            {
                if (file.Extension != ".manifest")
                {
                    // rename the file to deploy
                    File.Move(file.FullName, file.FullName + ".deploy");
                }
            }

            // Found the manifest file, microsoft does not have an option to generate this flag
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(manifestPath, manifestFileName));
            XmlNode node = doc.SelectSingleNode("deployment");

            if (node != null)
            {
                XmlAttribute attr = doc.CreateAttribute("mapFileExtensions");
                attr.Value = "true";
                node.Attributes.SetNamedItem(attr);
                doc.Save(Path.Combine(manifestPath, manifestFileName));
            }
        }

        /// <summary>
        /// Directory copy from MSDN
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: " + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    this.DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void GetFiles(DirectoryInfo dir, ref List<FileInfo> files)
        {
            try
            {
                files.AddRange(dir.GetFiles());
                DirectoryInfo[] dirs = dir.GetDirectories();
                foreach (var d in dirs)
                {
                    this.GetFiles(d, ref files);
                }
            }
            catch
            {
                // Bad, but ignore for now
            }
        }

        private void MapFileExtensions(string manifestPath, string manifestFileName)
        {
            // Found the manifest file, microsoft does not have an option to generate this flag
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(manifestPath, manifestFileName));

            XmlNode asmv1 = null;
            foreach (XmlNode node in doc.ChildNodes) if (node.Name == "asmv1:assembly") asmv1 = node;

            if (asmv1 != null)
            {
                // Get the ‘deployment’ node  
                XmlNode deployment = null;
                foreach (XmlNode node in asmv1.ChildNodes)
                {
                    if (node.Name == "deployment") deployment = node;
                }

                if (deployment != null)
                {
                    XmlAttribute attr = doc.CreateAttribute("mapFileExtensions");
                    attr.Value = "true";
                    this.SetAttrSafe(deployment, attr);

                    // Get the ‘subscription’ node  
                    XmlNode subscription = null;
                    foreach (XmlNode node in deployment.ChildNodes)
                    {
                        if (node.Name == "subscription") subscription = node;
                    }

                    if (subscription != null)
                    {
                        XmlNode update = null;
                        foreach (XmlNode node in subscription.ChildNodes)
                        {
                            if (node.Name == "update") update = node;
                        }

                        if (update != null)
                        {
                            update.RemoveAll();
                            XmlNode newNode = doc.CreateNode(
                                XmlNodeType.Element,
                                "beforeApplicationStartup",
                                update.NamespaceURI);
                            update.AppendChild(newNode);
                        }
                    }

                    // Get the ‘deploymentProvider’ node  
                    XmlNode deploymentProvider = null;
                    foreach (XmlNode node in deployment.ChildNodes)
                    {
                        if (node.Name == "deploymentProvider") deploymentProvider = node;
                    }

                    if (deploymentProvider != null)
                    {
                        deployment.RemoveChild(deploymentProvider);
                    }
                }

                // Get the ‘compatibleFrameworks’ node  
                // XmlNode compatibleFrameworks = null;
                // foreach (XmlNode node in asmv1.ChildNodes)
                // {
                // if (node.Name == "compatibleFrameworks")
                // compatibleFrameworks = node;
                // }

                // XmlNode last = compatibleFrameworks?.LastChild;

                // if (last != null)
                // {
                // last.Attributes["targetVersion"].Value = "4.5.2";
                // }
                doc.Save(Path.Combine(manifestPath, manifestFileName));
            }
        }

        /// <summary>
        /// Modify the configuration files for the portal according to the installer form inputs.
        /// </summary>
        private void PerformPortalConfiguration()
        {
            try
            {
                // Debugger.Launch();
                // Get the path to the executable file that is being installed on the target computer 
                string appPath =
                    Directory.GetParent(
                        Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).FullName;

                string portalPath = Path.Combine(appPath, "Portal");
                string mediConfigPath = Path.Combine(portalPath, "medi.config");
                string backgroungdsystemConfiguration = Path.Combine(
                    portalPath,
                    @"Website\BackgroundSystemConfiguration.xml");

                // MessageBox.Show(backgroungdsystemConfiguration);
                string backgroundsystemportal = this.Context.Parameters["BACKGROUNDSYSTEMPORTAL"].Trim();
                Uri backgroundSystemPortalUri = new Uri(backgroundsystemportal);
                Uri mediUri = new Uri(@"medi://" + backgroundSystemPortalUri.Host + ":1596");

                // Uri loopbackUri = new Uri(@"medi://127.0.0.1:1596");
                string port = this.Context.Parameters["PORT"].Trim();
                string portalConfigPath = Path.Combine(portalPath, "CenterPortal.exe.config");

                // Portal/medi.config
                XmlDocument doc = new XmlDocument();

                doc.Load(mediConfigPath);

                XmlNodeList nodes = doc.SelectNodes("/MediConfig/Peers/PeerConfig/Transport/RemoteHost");

                if (nodes != null)
                {
                    foreach (XmlNode node in nodes)
                    {
                        node.InnerText = backgroundSystemPortalUri.ToString().TrimEnd('/');
                    }
                }

                doc.Save(mediConfigPath);

                doc.Load(portalConfigPath);

                XmlNode configuration = null;
                foreach (XmlNode node in doc.ChildNodes) if (node.Name == "configuration") configuration = node;

                if (configuration != null)
                {
                    // Get the ‘appSettings’ node  
                    XmlNode settingNode = null;
                    foreach (XmlNode node in configuration.ChildNodes)
                    {
                        if (node.Name == "appSettings") settingNode = node;
                    }

                    if (settingNode != null)
                    {
                        foreach (XmlNode node in settingNode.ChildNodes)
                        {
                            if (node.Attributes == null) continue;

                            XmlAttribute attribute = node.Attributes["value"];

                            if (node.Attributes["key"] != null)
                            {
                                switch (node.Attributes["key"].Value)
                                {
                                    case "HttpPort":
                                        attribute.Value = port;
                                        break;
                                }
                            }
                        }
                    }

                    doc.Save(portalConfigPath);

                    // Change BackgroundSystemConfiguration.xml host address
                    doc.Load(backgroungdsystemConfiguration);

                    // Get the Data Service configuration and Change the Host address based on user input
                    XmlNodeList nodesBackgroundsystemConfigurationDataServicesNodeList =
                        doc.SelectNodes("/BackgroundSystemConfiguration/DataServices/Host");

                    if (nodesBackgroundsystemConfigurationDataServicesNodeList != null)
                    {
                        foreach (XmlNode node in nodesBackgroundsystemConfigurationDataServicesNodeList)
                        {
                            node.InnerText = backgroundSystemPortalUri.Host;
                        }
                    }

                    // Get the Functional Service configuration and Change the Host address based on user input
                    XmlNodeList nodesBackgroundsystemConfigurationFunctionalServicesNodeList =
                        doc.SelectNodes("/BackgroundSystemConfiguration/FunctionalServices/Host");

                    if (nodesBackgroundsystemConfigurationFunctionalServicesNodeList != null)
                    {
                        foreach (XmlNode node in nodesBackgroundsystemConfigurationFunctionalServicesNodeList)
                        {
                            node.InnerText = backgroundSystemPortalUri.Host;
                        }
                    }

                    // Get the Notifications Connection String and Change the Host address based on user input
                    XmlNodeList nodesBackgroundsystemConfigurationNotificationsConnectionStringNodeList =
                        doc.SelectNodes("/BackgroundSystemConfiguration/NotificationsConnectionString");

                    if (nodesBackgroundsystemConfigurationNotificationsConnectionStringNodeList != null)
                    {
                        foreach (XmlNode node in nodesBackgroundsystemConfigurationNotificationsConnectionStringNodeList)
                        {
                            node.InnerText = mediUri.ToString().TrimEnd('/');
                        }
                    }

                    doc.Save(backgroungdsystemConfiguration);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("AddConfigurationFileDetails Failed: " + ex.Message);
                throw;
            }
        }

        public static bool UpdateConfigValue(XmlNode node, string setting, string value)
        {
            if (node.Name == setting)
            {
                node.ChildNodes[0].Value = value;
                return true;
            }

            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (UpdateConfigValue(childNode, setting, value))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetParameterValue(string parameter)
        {
            return this.Context.Parameters[parameter].Trim();
        }
        
        /// <summary>
        /// Modify the configuration files for the system manager and background system according to the installer form inputs.
        /// </summary>
        private void PerformSystemConfiguration()
        {
            try
            {
                string backgroundsystemportal = this.Context.Parameters["BACKGROUNDSYSTEMPORTAL"];
                Uri tempUri = new Uri(backgroundsystemportal);
                Uri backgroundSystemPortalUri =
                    new Uri(@"http://" + tempUri.Host + ":" + this.Context.Parameters["PORT"].Trim());

                SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();

                sb.DataSource = this.Context.Parameters["DataSource"].Trim();
                sb.InitialCatalog = this.Context.Parameters["DatabaseName"].Trim();

                string username = this.Context.Parameters["UserID"].Trim();

                if (!string.IsNullOrEmpty(username))
                {
                    sb.UserID = username;
                    sb.Password = this.Context.Parameters["Password"].Trim();
                    sb.IntegratedSecurity = false;
                }
                else
                {
                    sb.IntegratedSecurity = true;
                }

                sb.ConnectTimeout = 30;

                // TODO: Clean this up

                // Get the path to the executable file that is being installed on the target computer  
                string appPath =
                    Directory.GetParent(
                        Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).FullName;

                string backgroundSystemPath = Path.Combine(appPath, "BackgroundSystem");
                string backgroundSystemHostConfigPath = Path.Combine(
                    backgroundSystemPath,
                    "BackgroundSystemConsoleHost.exe.config");

                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = backgroundSystemHostConfigPath;
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

                var connectionSection = config.ConnectionStrings;
                if (connectionSection == null)
                {
                    throw new Exception(
                        "Connection section not found config.ConnectionStrings.ConnectionStrings['connectionString'] == null");
                }

                // remove the existing one
                connectionSection.ConnectionStrings.Remove("CenterDataContext");

                // Get the current conneciton string settings
                // add a new Connection string
                var connectionstring = new ConnectionStringSettings(
                    "CenterDataContext",
                    sb.ConnectionString,
                    "System.Data.SqlClient");
                config.ConnectionStrings.ConnectionStrings.Add(connectionstring);

                try
                {
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("connectionStrings");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                // Write the path to the app.config file  
                XmlDocument doc = new XmlDocument();
                doc.Load(backgroundSystemHostConfigPath);

                XmlNode configuration = null;
                foreach (XmlNode node in doc.ChildNodes) if (node.Name == "configuration") configuration = node;

                if (configuration != null)
                {
                    // Get the ‘appSettings’ node  
                    XmlNode settingNode = null;
                    foreach (XmlNode node in configuration.ChildNodes)
                    {
                        if (node.Name == "appSettings") settingNode = node;
                    }

                    if (settingNode != null)
                    {
                        foreach (XmlNode node in settingNode.ChildNodes)
                        {
                            if (node.Attributes == null) continue;
                            XmlAttribute attribute = node.Attributes["value"];
                            if (node.Attributes["key"] != null)
                            {
                                switch (node.Attributes["key"].Value)
                                {
                                    case "BackgroundSystemPortal":
                                        attribute.Value = backgroundSystemPortalUri.ToString().TrimEnd('/');
                                        break;
                                }
                            }
                        }
                    }

                    doc.Save(backgroundSystemHostConfigPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("AddConfigurationFileDetails Failed: " + ex.Message);
                throw;
            }
        }

        private void SetAttrSafe(XmlNode node, params XmlAttribute[] attrList)
        {
            foreach (var attr in attrList)
            {
                if (node.Attributes[attr.Name] != null)
                {
                    node.Attributes[attr.Name].Value = attr.Value;
                }
                else
                {
                    node.Attributes.Append(attr);
                }
            }
        }

        private void ShowParameters()
        {
            StringBuilder sb = new StringBuilder();
            StringDictionary myStringDictionary = this.Context.Parameters;

            if (this.Context.Parameters.Count > 0)
            {
                foreach (string myString in this.Context.Parameters.Keys)
                {
                    sb.AppendFormat("String={0} Value= {1}\n", myString, this.Context.Parameters[myString]);
                }
            }
        }
    }
}
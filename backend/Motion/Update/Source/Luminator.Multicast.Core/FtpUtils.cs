namespace Luminator.Multicast.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;

    using Gorba.Common.Utility.Core;

    using NLog;

    public class FtpUtils : IDisposable
    {
        #region Static Fields

        private static readonly Logger Logger = LogHelper.GetLogger<FtpUtils>();

        #endregion

        #region Constructors and Destructors

        public FtpUtils()
        {
            this.Username = "Gorba";
            this.Password = "Asdf1234";
        }

        public FtpUtils(string server, string username, string password)
        {
            this.FtpServer = server;
            this.Username = username;
            this.Password = password;
        }

        #endregion

        #region Public Properties

        public string FtpServer { get; set; }

        public string Password { get; set; }

        public string Username { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
        }

        public bool CreateSlaveFile()
        {
            return this.CreateSlaveFile(this.FtpServer, this.Username, this.Password);
            
        }

        public bool CreateSlaveFile(string server, string user, string password)
        {
            try
            {
                if (!server.Contains("ftp://")) server = "ftp://" + server + "//";
                Console.WriteLine(MethodBase.GetCurrentMethod().Name + $" ftpServer is {server}");
                using (var ftp = new FtpUtils(server, user, password))
                {
                    int idx = 0;
                    string file = "Slave" + idx + ".txt";
                    var serverFtpUri = new Uri(server + file);
                    Console.WriteLine(MethodBase.GetCurrentMethod().Name + $"serverFtpUri is {serverFtpUri}");
                    while (ftp.FtpFileExists(serverFtpUri))
                    {
                        idx++;
                        file = "Slave" + idx + ".txt";
                        serverFtpUri = new Uri(server + file);
                    }
                    Console.WriteLine(MethodBase.GetCurrentMethod().Name + $" available serverFtpUri is {serverFtpUri}");
                    if (this.CreateTempFile("temp.txt"))
                    {
                        if (ftp.FtpFileUpload("./temp.txt", file))
                        {
                            Console.WriteLine(MethodBase.GetCurrentMethod().Name + $"Ftp Upload Success Status  : {ftp.FtpFileExists(serverFtpUri) }");
                            return true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Local Resource File was missing.");
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(MethodBase.GetCurrentMethod().Name + $"Error when creating slave files {e.Message}  {e.InnerException?.Message}");
            }
            return false;
        }

        public bool CreateTempFile(string pathString)
        {
            try
            {
                Console.WriteLine(MethodBase.GetCurrentMethod().Name + $"Trying to create a tempfile : {pathString}");
                if (!File.Exists(pathString))
                {
                    using (FileStream fs = File.Create(pathString))
                    {
                        for (byte i = 0; i < 100; i++)
                        {
                            fs.WriteByte(i);
                        }
                        Console.WriteLine(MethodBase.GetCurrentMethod().Name + "Sucessfully");
                    }
                }
                else
                {
                    Console.WriteLine(MethodBase.GetCurrentMethod().Name + $"File \"{pathString}\" already exists." );
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(MethodBase.GetCurrentMethod().Name + e.Message);
                return false;
            }
            return true;
        }

        public bool DeleteAllSlaveFiles()
        {
            return this.DeleteAllSlaveFiles(this.FtpServer, this.Username, this.Password);

        }
        public bool DeleteAllSlaveFiles(string server , string user, string password, int numberofSlaves = 10)
        {
            try
            {
                using (var ftp = new FtpUtils(server, user, password))
                {
                    if (!server.Contains("ftp://")) server = "ftp://" + server + "//";
                    Console.WriteLine(MethodBase.GetCurrentMethod().Name + $" ftpServer base URI is :  {server}");
                    for (int i = 0; i < numberofSlaves; i++)
                    {
                        string file = "Slave" + i + ".txt";
                        var serverFtpUri = new Uri(server + file);
                        Console.WriteLine(MethodBase.GetCurrentMethod().Name + $" serverFtpUri is {serverFtpUri}");
                        if (ftp.FtpFileExists(serverFtpUri))
                        {
                            Console.WriteLine(MethodBase.GetCurrentMethod().Name + $" file exists - try to delete:  {serverFtpUri}");
                            ftp.FtpFileDelete(serverFtpUri);
                        }
                        else
                        {
                            Console.WriteLine(MethodBase.GetCurrentMethod().Name + $" file did not exist returning :  {serverFtpUri}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return true;
        }

        public bool FtpFileDelete(Uri serverUri)
        {
            try
            {
                var request = (FtpWebRequest)WebRequest.Create(serverUri);

                //If you need to use network credentials
                request.Credentials = new NetworkCredential(this.Username, this.Password);
                //additionally, if you want to use the current user's network credentials, just use:
                //System.Net.CredentialCache.DefaultNetworkCredentials

                request.Method = WebRequestMethods.Ftp.DeleteFile;
                var response = (FtpWebResponse)request.GetResponse();
                Logger.Trace("Delete status: {0}", response.StatusDescription);
                response.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return true;
        }

        public bool FtpFileExists(Uri serverUri)
        {
            var request = (FtpWebRequest)WebRequest.Create(serverUri);
            request.Credentials = new NetworkCredential(this.Username, this.Password);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                var response = (FtpWebResponse)request.GetResponse();
                Console.WriteLine("File Exists status: {0}", response.StatusDescription);
                return true;
            }
            catch (WebException ex)
            {
                var response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }
            }

            return false;
        }

        public bool FtpFileUpload(string sourceFile, string targetFile)
        {
            try
            {
                string filename;
                if (this.FtpServer.Contains("ftp://"))
                {
                    filename = this.FtpServer + "//" + targetFile;
                }
                else
                {
                    filename = "ftp://" + this.FtpServer + "/" + targetFile;
                }
                //  Logger.Warn("Web Request Url => " + filename);
                //  Logger.Warn(this.Username + " : " + this.Password);
                var ftpReq = (FtpWebRequest)WebRequest.Create(filename);
                ftpReq.UseBinary = true;
                ftpReq.Method = WebRequestMethods.Ftp.UploadFile;

                ftpReq.Credentials = new NetworkCredential(this.Username, this.Password);
                if (File.Exists(sourceFile))
                {
                    var b = File.ReadAllBytes(sourceFile);

                    ftpReq.ContentLength = b.Length;
                    using (var s = ftpReq.GetRequestStream())
                    {
                        s.Write(b, 0, b.Length);
                    }

                    var ftpResp = (FtpWebResponse)ftpReq.GetResponse();

                    Logger.Warn(ftpResp.StatusDescription);
                }
                else
                {
                    Logger.Error("Source file not found => " + sourceFile);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return false;
            }
            return true;
        }

        public bool FtpSendFile(string ftpSite, string source, string destination)
        {
            try
            {
                if (this.IsValidConnection(ftpSite))
                {
                    if (!NetworkManagement.IsNetworkedStatusDown())
                    {
                        var baseUri = new Uri(ftpSite);
                        if (!ftpSite.Contains("ftp"))
                        {
                            baseUri = new Uri("ftp://" + ftpSite);
                        }
                        var ftpUrl = new Uri(baseUri, destination);
                        Console.WriteLine("ftpUrl => " + ftpUrl);
                        Console.WriteLine("localFile => " + source);

                        var request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                        request.Method = WebRequestMethods.Ftp.UploadFile;
                        request.Credentials = new NetworkCredential(this.Username, this.Password);

                        var response = (FtpWebResponse)request.GetResponse();
                        Console.WriteLine("Upload status: {0}", response.StatusDescription);
                        response.Close();
                    }
                    else
                    {
                        Console.WriteLine("No Network Avaialble");
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }
            return true;
        }

        public bool IsValidConnection(string url, string user = "", string password = "")
        {
            try
            {
                var request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                var usr = user == string.Empty ? this.Username : user;
                var pass = password == string.Empty ? this.Password : password;
                request.Credentials = new NetworkCredential(usr, pass);
                Console.WriteLine(usr);
                Console.WriteLine(pass);
                request.GetResponse();
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        #endregion
    }
}
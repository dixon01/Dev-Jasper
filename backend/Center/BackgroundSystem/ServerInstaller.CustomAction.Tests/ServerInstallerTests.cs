using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServerInstaller.CustomAction.Tests
{
    [TestClass]
    public class ServerInstallerTests
    {
        //[TestMethod]
        //public void Test_UpdatingConfig()
        //{
        //    ServerInstaller installer = new ServerInstaller();
        //    installer.PerformApiHostPortConfiguration();
        //}
        
        //public ServerInstallerTests()
        //{
        //    string appPath =
        //            Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
        //                .FullName;

        //    string adminSourcePath = Path.Combine(appPath, "Install", "Admin");
        //    string diagSourcePath = Path.Combine(appPath, "Install", "Diag");
        //    string mediaSourcePath = Path.Combine(appPath, "Install", "Media");

        //    string clickOncePath = Path.Combine(appPath, "Portal", "Website", "StaticContent", "ClickOnce");
        //    string adminDestPath = Path.Combine(clickOncePath, "admin");
        //    string diagDestPath = Path.Combine(clickOncePath, "diag");
        //    string mediaDestPath = Path.Combine(clickOncePath, "media");

        //    if (!Directory.Exists(adminSourcePath))
        //        Directory.CreateDirectory(adminSourcePath);

        //    if (!Directory.Exists(diagSourcePath))
        //        Directory.CreateDirectory(diagSourcePath);

        //    if (!Directory.Exists(mediaSourcePath))
        //        Directory.CreateDirectory(mediaSourcePath);

        //    if (!Directory.Exists(clickOncePath))
        //        Directory.CreateDirectory(clickOncePath);

        //    if (!Directory.Exists(adminDestPath))
        //        Directory.CreateDirectory(adminDestPath);

        //    if (!Directory.Exists(diagDestPath))
        //        Directory.CreateDirectory(diagDestPath);

        //    if (!Directory.Exists(mediaDestPath))
        //        Directory.CreateDirectory(mediaDestPath);

        //    string baseAppPath = 
        //        Directory.GetParent(
        //            Directory.GetParent(
        //                Directory.GetParent(appPath).ToString()).ToString()).ToString();

        //    string adminFile = Path.Combine(baseAppPath, "Admin", "Deploy", "setup.zip");
        //    string diagFile = Path.Combine(baseAppPath, "Diag", "Deploy", "setup.zip");
        //    string mediaFile = Path.Combine(baseAppPath, "Media", "Deploy", "setup.zip");

        //    File.Copy(adminFile, Path.Combine(adminSourcePath, "setup.zip"), true);
        //    File.Copy(diagFile, Path.Combine(diagSourcePath, "setup.zip"), true);
        //    File.Copy(mediaFile, Path.Combine(mediaSourcePath, "setup.zip"), true);
        //}

        //[TestMethod]
        //public void PerformClickOnceSetup()
        //{
        //    ServerInstaller installer = new ServerInstaller();

        //    installer.PerformClickOnceSetup();
        //}
    }
}

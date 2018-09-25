namespace Luminator.UIFramework.ResourceLibrary.Utils
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// The folder Utilities.
    /// </summary>
    public static class FolderUtils
    {
        /// <summary>
        /// The dump log.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        public static void DumpLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }

        public static void Log(string logMessage)
        {
            string path = AssemblyLoadDirectory + "log.txt";
            using (StreamWriter streamWriter = File.AppendText(path))
            {
                streamWriter.WriteLine("{0} {1} : {2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), logMessage);
            }
        }

        /// <summary>
        /// The directory copy.
        /// </summary>
        /// <param name="sourceDirectoryName">
        /// The source directory name.
        /// </param>
        /// <param name="destinationDirectoryName">
        /// The destination directory name.
        /// </param>
        /// <param name="copySubDirectories">
        /// The copy sub directories.
        /// </param>
        /// <exception cref="DirectoryNotFoundException">
        /// </exception>
        public static void DirectoryCopy(string sourceDirectoryName, string destinationDirectoryName, bool copySubDirectories)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirectoryName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirectoryName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destinationDirectoryName))
            {
                Directory.CreateDirectory(destinationDirectoryName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destinationDirectoryName, file.Name);
                Log("Copy file:" + temppath);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirectories)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destinationDirectoryName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirectories);
                }
            }
        }

        /// <summary>
        ///     Gets the assembly load directory.
        /// </summary>
        public static string AssemblyLoadDirectory
        {
            get
            {
                string codeBase = Assembly.GetCallingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}

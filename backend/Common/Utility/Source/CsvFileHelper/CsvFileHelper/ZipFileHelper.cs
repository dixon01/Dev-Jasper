namespace Luminator.Utility.CsvFileHelper
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;

    /// <summary>The zip file helper class.</summary>
    public static class ZipFileHelper
    {
        /// <summary>Get zip file name entries.</summary>
        /// <param name="zipFileName">The zip file name.</param>
        /// <returns>The <see cref="List" />.</returns>
        public static List<string> GetFileEntries(string zipFileName)
        {
            using (var zip = ZipFile.Open(zipFileName, ZipArchiveMode.Read))
            {
                return zip.Entries.Select(m => m.Name).ToList();
            }
        }

        /// <summary>
        /// Create a zip file name for the given source file
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <returns></returns>
        public static string GetZipFileName(string sourceFileName)
        {
            if (string.IsNullOrEmpty(sourceFileName))
            {
                return string.Empty;
            }

            var path = Path.GetDirectoryName(sourceFileName);
            if (string.IsNullOrEmpty(path))
            {
                path = Directory.GetCurrentDirectory();
            }

            return Path.Combine(path, $"{Path.GetFileNameWithoutExtension(sourceFileName)}.zip");
        }
    }
}
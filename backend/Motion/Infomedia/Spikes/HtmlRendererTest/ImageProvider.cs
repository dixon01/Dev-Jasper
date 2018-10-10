namespace HtmlRendererTest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    public class ImageProvider
    {
        public static readonly ImageProvider Instance = new ImageProvider();

        private readonly ReaderWriterLock locker = new ReaderWriterLock();

        private readonly Dictionary<string, FileInfo> imageFiles = new Dictionary<string, FileInfo>();
        private readonly Dictionary<string, string> imageUrls = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        private ImageProvider()
        {
        }

        public string GetPathFor(FileInfo file)
        {
            string imgUrl;
            this.locker.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (!this.imageUrls.TryGetValue(file.FullName, out imgUrl))
                {
                    imgUrl = "/images/" + Guid.NewGuid();
                    this.imageUrls[file.FullName] = imgUrl;
                    this.imageFiles[imgUrl] = file;
                }
            }
            finally
            {
                this.locker.ReleaseWriterLock();
            }

            return imgUrl;
        }

        public FileInfo GetFileForPath(string url)
        {
            FileInfo file;
            this.locker.AcquireReaderLock(Timeout.Infinite);
            try
            {
                this.imageFiles.TryGetValue(url, out file);
            }
            finally
            {
                this.locker.ReleaseReaderLock();
            }

            return file;
        }
    }
}

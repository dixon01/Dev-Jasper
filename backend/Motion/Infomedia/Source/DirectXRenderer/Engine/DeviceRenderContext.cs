// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceRenderContext.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeviceRenderContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Threading;

    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Image;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Text;

    using Microsoft.DirectX.Direct3D;

    using NLog;

    /// <summary>
    /// Implementation of <see cref="IDxDeviceRenderContext"/>.
    /// </summary>
    public class DeviceRenderContext : IDxDeviceRenderContext
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly List<string> ImageExtensions =
            new List<string> { ".jpg", ".jpeg", ".gif", ".png", ".bmp" };

        private readonly IDxRenderContext parentContext;

        private readonly Dictionary<ImageKey, ImageTextureBase> imageTextures =
            new Dictionary<ImageKey, ImageTextureBase>();

        private readonly Dictionary<FontDescription, FontInfo> fonts =
            new Dictionary<FontDescription, FontInfo>(new FontDescriptionComparer());

        private readonly long bitmapCacheTimeoutMilliseconds;

        private readonly long maxBitmapCacheBytes;

        private readonly long maxCacheBytesPerBitmap;

        private PersistenceViewManager persistenceViewManager = new PersistenceViewManager();

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceRenderContext"/> class.
        /// </summary>
        /// <param name="device">
        /// The DirectX device.
        /// </param>
        /// <param name="parentContext">
        /// The parent render context.
        /// </param>
        public DeviceRenderContext(Device device, IDxRenderContext parentContext)
        {
            this.parentContext = parentContext;
            this.Device = device;

            var imageConfig = this.Config.Image;
            this.bitmapCacheTimeoutMilliseconds = (long)imageConfig.BitmapCacheTimeout.TotalMilliseconds;
            this.maxBitmapCacheBytes = imageConfig.MaxBitmapCacheBytes;
            this.maxCacheBytesPerBitmap = imageConfig.MaxCacheBytesPerBitmap;

            var preloadThread = new Thread(this.PreloadImages);
            preloadThread.Name = "PreloadImages";
            preloadThread.IsBackground = true;
            preloadThread.Start();
        }

        /// <summary>
        /// Delegate to create image textures.
        /// This is used in <see cref="GetImageTexture(ImageKey,string,TextureCreator)"/>.
        /// </summary>
        /// <returns>
        /// The new image texture, never null.
        /// </returns>
        protected delegate ImageTextureBase TextureCreator();

        /// <summary>
        /// Gets the renderer configuration.
        /// </summary>
        public RendererConfig Config
        {
            get
            {
                return this.parentContext.Config;
            }
        }

        /// <summary>
        /// Gets a counter that is increased by one every millisecond.
        /// This counter doesn't change during a single rendering pass.
        /// </summary>
        public long MillisecondsCounter
        {
            get
            {
                return this.parentContext.MillisecondsCounter;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a blinking item should be rendered or not.
        /// This flag changes approximately every 0.5 seconds.
        /// <code>[bl][/bl]</code> BBCode requires this.
        /// </summary>
        public bool BlinkOn
        {
            get
            {
                return this.parentContext.BlinkOn;
            }
        }

        /// <summary>
        /// Gets a the current counter which is incremented every millisecond.
        /// This is used for the scrolling feature.
        /// </summary>
        public long ScrollCounter
        {
            get
            {
                return this.parentContext.ScrollCounter;
            }
        }

        /// <summary>
        /// Gets the persistence view manager.
        /// </summary>
        public PersistenceViewManager PersistenceView
        {
            get
            {
                return this.persistenceViewManager;
            }
        }

        /// <summary>
        /// Gets the DirectX device.
        /// </summary>
        public Device Device { get; private set; }

        /// <summary>
        /// Creates a new font or takes one from the cache.
        /// </summary>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="weight">
        /// The weight.
        /// </param>
        /// <param name="mipLevels">
        /// The number of MIP levels.
        /// </param>
        /// <param name="italic">
        /// The italic.
        /// </param>
        /// <param name="charSet">
        /// The char set.
        /// </param>
        /// <param name="outputPrecision">
        /// The output precision.
        /// </param>
        /// <param name="quality">
        /// The quality.
        /// </param>
        /// <param name="pitchAndFamily">
        /// The pitch and family.
        /// </param>
        /// <param name="fontName">
        /// The font name.
        /// </param>
        /// <returns>
        /// The <see cref="FontInfo"/>.
        /// </returns>
        public virtual IFontInfo GetFontInfo(
            int height,
            int width,
            FontWeight weight,
            int mipLevels,
            bool italic,
            CharacterSet charSet,
            Precision outputPrecision,
            FontQuality quality,
            PitchAndFamily pitchAndFamily,
            string fontName)
        {
            // Create the font description
            var desc = new FontDescription
            {
                Height = height,
                Width = width,
                Weight = weight,
                MipLevels = mipLevels,
                IsItalic = italic,
                CharSet = charSet,
                OutputPrecision = outputPrecision,
                Quality = quality,
                PitchAndFamily = pitchAndFamily,
                FaceName = fontName
            };

            // return the font
            return this.GetFontInfo(desc);
        }

        /// <summary>
        /// Creates a new font or takes one from the cache.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <returns>
        /// The <see cref="FontInfo"/>.
        /// </returns>
        public virtual IFontInfo GetFontInfo(FontDescription description)
        {
            FontInfo font;
            if (!this.fonts.TryGetValue(description, out font))
            {
                Logger.Debug(
                    "Creating font for {0} ({1},{2},I={3})",
                    description.FaceName,
                    description.Height,
                    description.Weight,
                    description.IsItalic);

                font = new FontInfo(this.Device, description);
                font.Disposing += (s, e) => this.fonts.Remove(description);
                this.fonts.Add(description, font);
                Logger.Trace("Font created");
            }

            return font;
        }

        /// <summary>
        /// Creates a new image texture or takes it from the cache.
        /// It is important to release the returned texture again using
        /// <see cref="ReleaseImageTexture"/> once you are not displaying
        /// the image anymore.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="IImageTexture"/>.
        /// </returns>
        public virtual IImageTexture GetImageTexture(string filename)
        {
            if (!File.Exists(filename))
            {
                Logger.Warn("Image file does not exist: {0}", filename);
                return null;
            }

            return this.GetImageTexture(
                new ImageKey(filename),
                filename,
                () => new FileImageTexture(filename, this.Device));
        }

        /// <summary>
        /// Creates a new image texture or takes it from the cache.
        /// It is important to release the returned texture again using
        /// <see cref="IDxDeviceRenderContext.ReleaseImageTexture"/> once you are not displaying
        /// the image anymore.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap for which the texture is created.
        /// </param>
        /// <returns>
        /// The <see cref="IImageTexture"/>.
        /// </returns>
        public virtual IImageTexture GetImageTexture(Bitmap bitmap)
        {
            return this.GetImageTexture(
                new ImageKey(bitmap),
                string.Format("Bitmap[{0}x{1};{2:X8}]", bitmap.Width, bitmap.Height, bitmap.GetHashCode()),
                () => new BitmapImageTexture(bitmap, this.Device));
        }

        /// <summary>
        /// Releases the given image texture.
        /// This will release the underlying resources if the image
        /// is no longer used and shouldn't be kept in a cache.
        /// </summary>
        /// <param name="texture">
        /// The texture.
        /// </param>
        public void ReleaseImageTexture(IImageTexture texture)
        {
            var image = texture as ImageTextureBase;
            if (image == null)
            {
                throw new ArgumentException(@"Texture returned from GetImageTexture() is required", @"texture");
            }

            lock (this.imageTextures)
            {
                image.Usages--;
                if (image.Usages < 0)
                {
                    Logger.Warn("Image usage below zero ({0}): {1}", image, image.Usages);
                    image.Usages = 0;
                }

                Logger.Trace("Image texture {0} is used {1} times", image, image.Usages);
                this.RemoveUnusedImages();
            }
        }

        /// <summary>
        /// Creates a new image texture or takes it from the cache.
        /// </summary>
        /// <param name="key">
        /// The key with which the texture can be found again.
        /// </param>
        /// <param name="name">
        /// The name of the texture used for logging.
        /// </param>
        /// <param name="creator">
        /// The method to call when the texture should be created.
        /// </param>
        /// <returns>
        /// The <see cref="IImageTexture"/>.
        /// </returns>
        protected IImageTexture GetImageTexture(ImageKey key, string name, TextureCreator creator)
        {
            lock (this.imageTextures)
            {
                this.RemoveUnusedImages();

                ImageTextureBase image;
                if (!this.imageTextures.TryGetValue(key, out image))
                {
                    Logger.Debug("Creating image texture for {0}", name);
                    image = creator();
                    this.AddTexture(key, image);
                    Logger.Trace("Image texture created");
                }

                image.LastUsed = this.MillisecondsCounter;
                image.Usages++;
                Logger.Trace("Image texture {0} is used {1} times", name, image.Usages);
                return image;
            }
        }

        private void PreloadImages()
        {
            var preloadImages = new List<FileInfo>();
            foreach (var directory in this.Config.Image.PreloadDirectories)
            {
                var info = new DirectoryInfo(directory);
                if (info.Exists)
                {
                    this.GetPreloadImages(info, preloadImages);
                }
            }

            Logger.Debug("Pre-loading {0} images", preloadImages.Count);
            long totalSize = 0;
            foreach (var file in preloadImages)
            {
                var key = new ImageKey(file.FullName);
                lock (this.imageTextures)
                {
                    if (this.imageTextures.ContainsKey(key))
                    {
                        continue;
                    }
                }

                Logger.Trace("Pre-loading image texture {0}", file.FullName);
                try
                {
                    if (totalSize > this.maxBitmapCacheBytes)
                    {
                        Logger.Info("Couldn't pre-load all images, reached limit: {0} bytes", this.maxBitmapCacheBytes);
                        return;
                    }

                    var bitmap = new Bitmap(file.FullName);
                    var bitmapBytes = bitmap.Width * bitmap.Height * 4;
                    if (bitmapBytes > this.maxCacheBytesPerBitmap)
                    {
                        Logger.Info(
                            "Couldn't pre-load {0} since it is too big ({1} > {2} bytes)",
                            file.FullName,
                            bitmapBytes,
                            this.maxCacheBytesPerBitmap);
                        bitmap.Dispose();
                        continue;
                    }

                    totalSize += bitmapBytes;
                    var image = new FileImageTexture(file.FullName, this.Device);
                    lock (this.imageTextures)
                    {
                        this.AddTexture(key, image);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't pre-load File " + file.FullName);
                }
            }

            Logger.Info("Pre-loaded all images");
        }

        private void AddTexture(ImageKey key, ImageTextureBase image)
        {
            if (this.imageTextures.ContainsKey(key))
            {
                image.Dispose();
                return;
            }

            image.Disposing += this.ImageOnDisposing;
            this.imageTextures.Add(key, image);
        }

        private void GetPreloadImages(DirectoryInfo directory, List<FileInfo> preloadImages)
        {
            foreach (var file in directory.GetFiles())
            {
                var extension = file.Extension;
                if (!string.IsNullOrEmpty(extension)
                    && ImageExtensions.Find(e => e.Equals(extension, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    preloadImages.Add(file);
                }
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
                this.GetPreloadImages(subDirectory, preloadImages);
            }
        }

        private void RemoveUnusedImages()
        {
            var expiry = this.MillisecondsCounter - this.bitmapCacheTimeoutMilliseconds;
            var textures = new List<ImageTextureBase>(this.imageTextures.Values);
            textures.Sort((a, b) => (int)(b.LastUsed - a.LastUsed));

            for (int i = textures.Count - 1; i >= 0; i--)
            {
                var image = textures[i];
                if (image.Usages == 0
                    && ((image.LastUsed > 0 && image.LastUsed < expiry)
                        || image.BitmapBytes > this.maxCacheBytesPerBitmap))
                {
                    Logger.Debug("Disposing unused image texture for {0}", image);
                    textures.RemoveAt(i);

                    // this will remove it through the Disposing event handler
                    image.Dispose();
                }
            }

            long totalSize = 0;
            foreach (var image in textures)
            {
                if (image.Usages > 0)
                {
                    totalSize += image.BitmapBytes;
                }
            }

            // totalSize contains now the total of bytes that are currently required (shown on the screen)
            var found = false;
            foreach (var image in textures)
            {
                if (image.Usages > 0)
                {
                    continue;
                }

                totalSize += image.BitmapBytes;
                if (totalSize <= this.maxBitmapCacheBytes)
                {
                    continue;
                }

                // ok, we reached the memory limit, we need to remove some images
                if (!found)
                {
                    Logger.Debug("Reached image memory limit: {0} > {1}", totalSize, this.maxBitmapCacheBytes);
                    found = true;
                }

                Logger.Debug("Disposing image texture to gain memory: {0}", image);

                // this will remove it through the Disposing event handler
                image.Dispose();
            }
        }

        private void ImageOnDisposing(object sender, EventArgs eventArgs)
        {
            var image = sender as ImageTextureBase;
            if (image == null)
            {
                return;
            }

            image.Disposing -= this.ImageOnDisposing;
            lock (this.imageTextures)
            {
                foreach (var pair in this.imageTextures)
                {
                    if (pair.Value == image)
                    {
                        this.imageTextures.Remove(pair.Key);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// The key to identify an image.
        /// </summary>
        protected class ImageKey : IEquatable<ImageKey>
        {
            private readonly string filename;

            private readonly int bitmapHash;

            /// <summary>
            /// Initializes a new instance of the <see cref="ImageKey"/> class.
            /// </summary>
            /// <param name="filename">
            /// The filename of the image to use as the key.
            /// </param>
            public ImageKey(string filename)
            {
                this.filename = filename.ToLower();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ImageKey"/> class.
            /// </summary>
            /// <param name="bitmap">
            /// The bitmap to use as the key.
            /// </param>
            public ImageKey(Bitmap bitmap)
            {
                this.bitmapHash = bitmap.GetHashCode();
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            /// <param name="other">
            /// An object to compare with this object.
            /// </param>
            public bool Equals(ImageKey other)
            {
                if (other == null)
                {
                    return false;
                }

                if (this.filename == null || other.filename == null)
                {
                    return this.bitmapHash == other.bitmapHash;
                }

                return this.filename == other.filename;
            }

            /// <summary>
            /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
            /// </summary>
            /// <returns>
            /// true if the specified <see cref="Object"/> is equal
            /// to the current <see cref="Object"/>; otherwise, false.
            /// </returns>
            /// <param name="obj">
            /// The <see cref="Object"/> to compare with the current <see cref="Object"/>.
            /// </param>
            public override bool Equals(object obj)
            {
                return this.Equals(obj as ImageKey);
            }

            /// <summary>
            /// Serves as a hash function for a particular type.
            /// </summary>
            /// <returns>
            /// A hash code for the current <see cref="Object"/>.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public override int GetHashCode()
            {
                return this.filename == null ? this.bitmapHash : this.filename.GetHashCode();
            }
        }

        private class FontDescriptionComparer : IEqualityComparer<FontDescription>
        {
            public bool Equals(FontDescription x, FontDescription y)
            {
                return x.Height == y.Height
                    && x.Width == y.Width
                    && x.MipLevels == y.MipLevels
                    && x.IsItalic == y.IsItalic
                    && x.CharSet == y.CharSet
                    && x.OutputPrecision == y.OutputPrecision
                    && x.Quality == y.Quality
                    && x.PitchAndFamily == y.PitchAndFamily
                    && x.FaceName == y.FaceName;
            }

            public int GetHashCode(FontDescription obj)
            {
                return obj.Height ^ obj.Weight.GetHashCode() ^ obj.FaceName.GetHashCode();
            }
        }
    }
}
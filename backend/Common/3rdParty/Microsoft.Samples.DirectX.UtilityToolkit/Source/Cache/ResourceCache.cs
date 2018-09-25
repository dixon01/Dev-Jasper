namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Will be a resource cache for any resources that may be required by a sample
    /// This class will be 'static'
    /// </summary>
    public class ResourceCache
    {
        #region Creation
        private ResourceCache() { } // Don't allow creation
        private static ResourceCache localObject = null;
        public static ResourceCache GetGlobalInstance()
        {
            if (localObject == null)
                localObject = new ResourceCache();

            return localObject;
        }
        #endregion

        protected Hashtable textureCache = new Hashtable(); // Cache of textures
        protected Hashtable effectCache = new Hashtable(); // Cache of effects
        protected Dictionary<FontDescription, Font> fontCache = new Dictionary<FontDescription, Font>(new FontDescriptionComparer()); // Cache of fonts

        #region Cache Creation Methods

        /// <summary>Create a texture from a file</summary>
        public Texture CreateTextureFromFile(Device device, string filename)
        {
            return this.CreateTextureFromFileEx(device, filename, D3DX.Default, D3DX.Default, D3DX.Default, Usage.None,
                Format.Unknown, Pool.Managed, (Filter)D3DX.Default, (Filter)D3DX.Default, 0);
        }
        /// <summary>Create a texture from a file</summary>
        public Texture CreateTextureFromFileEx(Device device, string filename, int w, int h, int mip, Usage usage, Format fmt, Pool pool, Filter filter, Filter mipfilter, int colorkey)
        {
            // Search the cache first
            foreach(CachedTexture ct in this.textureCache.Keys)
            {
                if ( (string.Compare(ct.Source, filename, true) == 0) &&
                     ct.Width == w &&
                     ct.Height == h &&
                     ct.MipLevels == mip &&
                     ct.Usage == usage &&
                     ct.Format == fmt &&
                     ct.Pool == pool &&
                     ct.Type == ResourceType.Textures)
                {
                    // A match was found, return that
                    return this.textureCache[ct] as Texture;
                }
            }

            // No matching entry, load the resource and add it to the cache
            Texture t = TextureLoader.FromFile(device, filename, w, h, mip, usage, fmt, pool, filter, mipfilter, colorkey);
            CachedTexture entry = new CachedTexture();
            entry.Source = filename;
            entry.Width = w;
            entry.Height = h;
            entry.MipLevels = mip;
            entry.Usage = usage;
            entry.Format = fmt;
            entry.Pool = pool;
            entry.Type = ResourceType.Textures;

            this.textureCache.Add(entry, t);

            return t;
        }
        /// <summary>Create a cube texture from a file</summary>
        public CubeTexture CreateCubeTextureFromFile(Device device, string filename)
        {
            return this.CreateCubeTextureFromFileEx(device, filename, D3DX.Default, D3DX.Default, Usage.None,
                Format.Unknown, Pool.Managed, (Filter)D3DX.Default, (Filter)D3DX.Default, 0);
        }
        /// <summary>Create a cube texture from a file</summary>
        public CubeTexture CreateCubeTextureFromFileEx(Device device, string filename, int size, int mip, Usage usage, Format fmt, Pool pool, Filter filter, Filter mipfilter, int colorkey)
        {
            // Search the cache first
            foreach(CachedTexture ct in this.textureCache.Keys)
            {
                if ( (string.Compare(ct.Source, filename, true) == 0) &&
                     ct.Width == size &&
                     ct.MipLevels == mip &&
                     ct.Usage == usage &&
                     ct.Format == fmt &&
                     ct.Pool == pool &&
                     ct.Type == ResourceType.CubeTexture)
                {
                    // A match was found, return that
                    return this.textureCache[ct] as CubeTexture;
                }
            }

            // No matching entry, load the resource and add it to the cache
            CubeTexture t = TextureLoader.FromCubeFile(device, filename, size, mip, usage, fmt, pool, filter, mipfilter, colorkey);
            CachedTexture entry = new CachedTexture();
            entry.Source = filename;
            entry.Width = size;
            entry.MipLevels = mip;
            entry.Usage = usage;
            entry.Format = fmt;
            entry.Pool = pool;
            entry.Type = ResourceType.CubeTexture;

            this.textureCache.Add(entry, t);

            return t;
        }
        /// <summary>Create a volume texture from a file</summary>
        public VolumeTexture CreateVolumeTextureFromFile(Device device, string filename)
        {
            return this.CreateVolumeTextureFromFileEx(device, filename, D3DX.Default, D3DX.Default, D3DX.Default, D3DX.Default, Usage.None,
                Format.Unknown, Pool.Managed, (Filter)D3DX.Default, (Filter)D3DX.Default, 0);
        }
        /// <summary>Create a volume texture from a file</summary>
        public VolumeTexture CreateVolumeTextureFromFileEx(Device device, string filename, int w, int h, int d, int mip, Usage usage, Format fmt, Pool pool, Filter filter, Filter mipfilter, int colorkey)
        {
            // Search the cache first
            foreach(CachedTexture ct in this.textureCache.Keys)
            {
                if ( (string.Compare(ct.Source, filename, true) == 0) &&
                     ct.Width == w &&
                     ct.Height == h &&
                     ct.Depth == d &&
                     ct.MipLevels == mip &&
                     ct.Usage == usage &&
                     ct.Format == fmt &&
                     ct.Pool == pool &&
                     ct.Type == ResourceType.VolumeTexture)
                {
                    // A match was found, return that
                    return this.textureCache[ct] as VolumeTexture;
                }
            }

            // No matching entry, load the resource and add it to the cache
            VolumeTexture t = TextureLoader.FromVolumeFile(device, filename, w, h, d, mip, usage, fmt, pool, filter, mipfilter, colorkey);
            CachedTexture entry = new CachedTexture();
            entry.Source = filename;
            entry.Width = w;
            entry.Height = h;
            entry.Depth = d;
            entry.MipLevels = mip;
            entry.Usage = usage;
            entry.Format = fmt;
            entry.Pool = pool;
            entry.Type = ResourceType.VolumeTexture;

            this.textureCache.Add(entry, t);

            return t;
        }

        /// <summary>Create an effect from a file</summary>
        public Effect CreateEffectFromFile(Device device, string filename, Macro[] defines, Include includeFile, ShaderFlags flags, EffectPool effectPool, out string errors)
        {
            // No errors at first!
            errors = string.Empty;
            // Search the cache first
            foreach(CachedEffect ce in this.effectCache.Keys)
            {
                if ( (string.Compare(ce.Source, filename, true) == 0) &&
                     ce.Flags == flags)
                {
                    // A match was found, return that
                    return this.effectCache[ce] as Effect;
                }
            }

            // Nothing found in the cache
            Effect e = Effect.FromFile(device, filename, defines, includeFile, null, flags, effectPool, out errors);
            // Add this to the cache
            CachedEffect entry = new CachedEffect();
            entry.Flags = flags;
            entry.Source = filename;
            this.effectCache.Add(entry, e);

            // Return the new effect
            return e;
        }

        /// <summary>Create an effect from a file</summary>
        public Effect CreateEffectFromFile(Device device, string filename, Macro[] defines, Include includeFile, ShaderFlags flags, EffectPool effectPool)
        { 
            string temp; return this.CreateEffectFromFile(device, filename, defines, includeFile, flags, effectPool, out temp);
        }
        /// <summary>Create a font object</summary>
        public Font CreateFont(Device device, int height, int width, FontWeight weight, int mip, bool italic,
                               CharacterSet charSet, Precision outputPrecision, FontQuality quality, PitchAndFamily pandf, string fontName)
        {
            // Create the font description
            FontDescription desc = new FontDescription();
            desc.Height = height;
            desc.Width = width;
            desc.Weight = weight;
            desc.MipLevels = mip;
            desc.IsItalic = italic;
            desc.CharSet = charSet;
            desc.OutputPrecision = outputPrecision;
            desc.Quality = quality;
            desc.PitchAndFamily = pandf;
            desc.FaceName = fontName;

            // return the font
            return this.CreateFont(device, desc);
        }
        /// <summary>Create a font object</summary>
        public Font CreateFont(Device device, FontDescription desc)
        {
            // Search the cache first
            Font font;
            if (this.fontCache.TryGetValue(desc, out font))
            {
                return font;
            }

            // Couldn't find anything in the cache, create one
            font = new Font(device, desc);

            // Create a new entry
            this.fontCache.Add(desc, font);

            // return the new font
            return font;
        }

        #endregion

        #region Device event callbacks
        /// <summary>
        /// Called when the device is created
        /// </summary>
        public void OnCreateDevice(Device device) {} // Nothing to do on device create
        /// <summary>
        /// Called when the device is reset
        /// </summary>
        public void OnResetDevice(Device device)
        {
            // Call OnResetDevice on all effect and font objects
            foreach(Font f in this.fontCache.Values)
                f.OnResetDevice();
            foreach(Effect e in this.effectCache.Values)
                e.OnResetDevice();
        }
        /// <summary>
        /// Clear any resources that need to be lost
        /// </summary>
        public void OnLostDevice()
        {
            foreach(Font f in this.fontCache.Values)
                f.OnLostDevice();
            foreach(Effect e in this.effectCache.Values)
                e.OnLostDevice();

            // Search the texture cache 
            foreach(CachedTexture ct in this.textureCache.Keys)
            {
                if (ct.Pool == Pool.Default)
                {
                    // A match was found, get rid of it
                    switch(ct.Type)
                    {
                        case ResourceType.Textures:
                            (this.textureCache[ct] as Texture).Dispose(); break;
                        case ResourceType.CubeTexture:
                            (this.textureCache[ct] as CubeTexture).Dispose();break;
                        case ResourceType.VolumeTexture:
                            (this.textureCache[ct] as VolumeTexture).Dispose();break;
                    }
                }
            }
        }
        /// <summary>
        /// Destroy any resources and clear the caches
        /// </summary>
        public void OnDestroyDevice()
        {
            // Cleanup the fonts
            foreach(Font f in this.fontCache.Values)
                f.Dispose();

            // Cleanup the effects
            foreach(Effect e in this.effectCache.Values)
                e.Dispose();

            // Dispose of any items in the caches
            foreach(BaseTexture texture in this.textureCache.Values)
            {
                if (texture != null)
                    texture.Dispose();
            }

            // Clear all of the caches
            this.textureCache.Clear();
            this.fontCache.Clear();
            this.effectCache.Clear();
        }

        #endregion

        private class FontDescriptionComparer : IEqualityComparer<FontDescription>
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            /// <param name="x">The first object to compare.
            /// </param>
            /// <param name="y">The second object to compare.
            /// </param>
            public bool Equals(FontDescription x, FontDescription y)
            {
                return (string.Compare(x.FaceName.TrimEnd('\0'), y.FaceName.TrimEnd('\0'), StringComparison.OrdinalIgnoreCase) == 0)
                       && x.CharSet == y.CharSet && x.Height == y.Height && x.IsItalic == y.IsItalic
                       && x.MipLevels == y.MipLevels && x.OutputPrecision == y.OutputPrecision
                       && x.PitchAndFamily == y.PitchAndFamily && x.Quality == y.Quality
                       && x.Weight == y.Weight && x.Width == y.Width;
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            /// <returns>
            /// A hash code for the specified object.
            /// </returns>
            /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.
            /// </param>
            /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
            /// </exception>
            public int GetHashCode(FontDescription obj)
            {
                return obj.FaceName.TrimEnd('\0').GetHashCode() ^ obj.Height ^ (int)obj.Weight;
            }
        }
    }
}
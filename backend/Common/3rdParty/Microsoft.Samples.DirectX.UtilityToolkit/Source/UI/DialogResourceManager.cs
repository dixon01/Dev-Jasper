namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System.Collections;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Manages shared resources of dialogs
    /// </summary>
    public sealed class DialogResourceManager
    {
        private StateBlock dialogStateBlock;  // Stateblock shared amongst all dialogs
        private Sprite dialogSprite; // Sprite used for drawing
        public StateBlock StateBlock { get { return this.dialogStateBlock; } }
        public Sprite Sprite { get { return this.dialogSprite; } }
        private Device device; // Device

        // Lists of textures/fonts
        private ArrayList textureCache = new ArrayList();
        private ArrayList fontCache = new ArrayList();

        #region Creation
        /// <summary>Do not allow creation</summary>
        private DialogResourceManager()  {
            this.device = null;
            this.dialogSprite = null;
            this.dialogStateBlock = null;
        } 

        private static DialogResourceManager localObject = null;
        public static DialogResourceManager GetGlobalInstance()
        {
            if (localObject == null)
                localObject = new DialogResourceManager();

            return localObject;
        }
        #endregion

        /// <summary>Gets a font node from the cache</summary>
        public FontNode GetFontNode(int index) { return this.fontCache[index] as FontNode; }
        /// <summary>Gets a texture node from the cache</summary>
        public TextureNode GetTextureNode(int index) { return this.textureCache[index] as TextureNode; }
        /// <summary>Gets the device</summary>
        public Device Device { get { return this.device; } }

        /// <summary>
        /// Adds a font to the resource manager
        /// </summary>
        public int AddFont(string faceName, uint height, FontWeight weight)
        {
            // See if this font exists
            for(int i = 0; i < this.fontCache.Count; i++)
            {
                FontNode fn = this.fontCache[i] as FontNode;
                if ( (string.Compare(fn.FaceName, faceName, true) == 0) &&
                     fn.Height == height &&
                     fn.Weight == weight)
                {
                    // Found it
                    return i;
                }
            }

            // Doesn't exist, add a new one and try to create it
            FontNode newNode = new FontNode();
            newNode.FaceName = faceName;
            newNode.Height = height;
            newNode.Weight = weight;
            this.fontCache.Add(newNode);

            int fontIndex = this.fontCache.Count-1;
            // If a device is available, try to create immediately
            if (this.device != null)
                this.CreateFont(fontIndex);

            return fontIndex;
        }
        /// <summary>
        /// Adds a texture to the resource manager
        /// </summary>
        public int AddTexture(string filename)
        {
            // See if this font exists
            for(int i = 0; i < this.textureCache.Count; i++)
            {
                TextureNode tn = this.textureCache[i] as TextureNode;
                if (string.Compare(tn.Filename, filename, true) == 0)
                {
                    // Found it
                    return i;
                }
            }
            // Doesn't exist, add a new one and try to create it
            TextureNode newNode = new TextureNode();
            newNode.Filename = filename;
            this.textureCache.Add(newNode);

            int texIndex = this.textureCache.Count-1;

            // If a device is available, try to create immediately
            if (this.device != null)
                this.CreateTexture(texIndex);

            return texIndex;

        }

        /// <summary>
        /// Creates a font
        /// </summary>
        public void CreateFont(int font)
        {
            // Get the font node here
            FontNode fn = this.GetFontNode(font);
            if (fn.Font != null)
                fn.Font.Dispose(); // Get rid of this

            // Create the new font
            fn.Font = new Font(this.device, (int)fn.Height, 0, fn.Weight, 1, false, CharacterSet.Default,
                Precision.Default, FontQuality.Default, PitchAndFamily.DefaultPitch | PitchAndFamily.FamilyDoNotCare,
                fn.FaceName);
        }

        /// <summary>
        /// Creates a texture
        /// </summary>
        public void CreateTexture(int tex)
        {
            // Get the texture node here
            TextureNode tn = this.GetTextureNode(tex);

            // Make sure there's a texture to create
            if ((tn.Filename == null) || (tn.Filename.Length == 0))
                return;

            // Find the texture
            string path = Utility.FindMediaFile(tn.Filename);

            // Create the new texture
            ImageInformation info = new ImageInformation();
            tn.Texture = TextureLoader.FromFile(this.device, path, D3DX.Default, D3DX.Default, D3DX.Default, Usage.None,
                Format.Unknown, Pool.Managed, (Filter)D3DX.Default, (Filter)D3DX.Default, 0, ref info);

            // Store dimensions
            tn.Width = (uint)info.Width;
            tn.Height = (uint)info.Height;

        }

        #region Device event callbacks
        /// <summary>
        /// Called when the device is created
        /// </summary>
        public void OnCreateDevice(Device d) 
        {
            // Store device
            this.device = d;

            // create fonts and textures
            for (int i = 0; i < this.fontCache.Count; i++)
                this.CreateFont(i);

            for (int i = 0; i < this.textureCache.Count; i++)
                this.CreateTexture(i);

            this.dialogSprite = new Sprite(d); // Create the sprite
        } 
        /// <summary>
        /// Called when the device is reset
        /// </summary>
        public void OnResetDevice(Device device)
        {
            foreach(FontNode fn in this.fontCache)
                fn.Font.OnResetDevice();

            if (this.dialogSprite != null)
                this.dialogSprite.OnResetDevice();
            
            // Create new state block
            this.dialogStateBlock = new StateBlock(device, StateBlockType.All);
        }

        /// <summary>
        /// Clear any resources that need to be lost
        /// </summary>
        public void OnLostDevice()
        {
            foreach(FontNode fn in this.fontCache)
            {
                if ( (fn.Font != null) && (!fn.Font.Disposed) )
                    fn.Font.OnLostDevice();
            }

            if (this.dialogSprite != null)
                this.dialogSprite.OnLostDevice();

            if (this.dialogStateBlock != null)
            {
                this.dialogStateBlock.Dispose();
                this.dialogStateBlock = null;
            }
        }
        
        /// <summary>
        /// Destroy any resources and clear the caches
        /// </summary>
        public void OnDestroyDevice()
        {
            foreach(FontNode fn in this.fontCache)
            {
                if (fn.Font != null)
                    fn.Font.Dispose();
            }
            
            foreach(TextureNode tn in this.textureCache)
            {
                if (tn.Texture != null)
                    tn.Texture.Dispose();
            }

            if (this.dialogSprite != null)
            {
                this.dialogSprite.Dispose();
                this.dialogSprite = null;
            }

            if (this.dialogStateBlock != null)
            {
                this.dialogStateBlock.Dispose();
                this.dialogStateBlock = null;
            }
        }

        #endregion
    }
}
namespace MeshTextDirectXTest
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Microsoft.DirectX;
    using Microsoft.DirectX.AudioVideoPlayback;
    using Microsoft.DirectX.Direct3D;

    using Font = Microsoft.DirectX.Direct3D.Font;

    public partial class Form1 : Form
    {
        private Device device;

        private int pos = 50;

        private CustomVertex.TransformedColored[] colorTriangle;

        private Font font;
        private Font fpsFont;

        private long frameCounter;
        private int lastTickCount;
        private float lastFrameRate;

        private bool videoMode;

        private VertexBuffer videoVertexBuffer;

        private VideoTexture videoTexture;

        private PresentParameters presentParams;

        private string displayModeText;

        private Mesh mesh;

        private Material meshMaterial;

        private float angle;

        public Form1()
        {
            this.InitializeComponent();

            this.InitGraphics(true);
            this.displayModeText = "Fullscreen Window Mode";
            this.InitPrimitives();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.videoMode)
            {
                return;
            }

            this.UpdateScene();
            this.DrawScene(null);
            this.Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.F2:
                    this.InitVideo();
                    break;
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.F:
                    {
                        this.PresentationParam(false, Manager.Adapters.Default);
                        this.device.Reset(this.presentParams);
                        //this.InitGraphics(false);
                        this.displayModeText = "Fullscreen Mode";
                        this.InitPrimitives();
                    }
                    break;
                case Keys.W:
                    {
                        this.PresentationParam(true, Manager.Adapters.Default);
                        this.device.Reset(this.presentParams);
                        //this.InitGraphics(true);
                        this.displayModeText = "Fullscreen Window Mode";
                        this.InitPrimitives();
                    }
                    break;
            }
        }

        private void InitGraphics(bool windowed)
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            var adapter = Manager.Adapters.Default;
            this.PresentationParam(windowed, adapter);

            this.device = new Device(adapter.Adapter, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, this.presentParams);
            this.device.DeviceReset += (sender, args) => this.ResetCameraAndLights();

            // What font do we want to use?
            //var localFont = new System.Drawing.Font("Arial", 14.0f, FontStyle.Italic);
            var family = new FontFamily("Arial");
            var localFont = new System.Drawing.Font(family, 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            // Create an extruded version of this font
            this.mesh = Mesh.TextFromFont(this.device, localFont, "Managed DirectX", 0, 0.000000000001f);

            // Create a material for our text mesh
            this.meshMaterial = new Material();
            this.meshMaterial.Diffuse = Color.Peru;

            this.ResetCameraAndLights();
        }

        private void ResetCameraAndLights()
        {
            this.device.Transform.Projection = Matrix.PerspectiveFovLH(
                (float)Math.PI / 4, (float)this.Width / this.Height, 1.0f, 100.0f);

            this.device.Transform.View = Matrix.LookAtLH(new Vector3(0, 0, -9.0f), new Vector3(), new Vector3(0, 1, 0));

            this.device.Lights[0].Type = LightType.Directional;
            this.device.Lights[0].Diffuse = Color.White;
            this.device.Lights[0].Direction = new Vector3(0, 0, 1);
            this.device.Lights[0].Update();
            this.device.Lights[0].Enabled = true;
        }

        private void Draw3DText(Vector3 axis, Vector3 location)
        {
            this.device.Transform.World = Matrix.Translation(location);
            this.device.Material = this.meshMaterial;
            this.mesh.DrawSubset(0);

            this.angle += 0.01f;
        }

        private void PresentationParam(bool windowed, AdapterInformation adapter)
        {
            this.presentParams = new PresentParameters
                {
                    Windowed = windowed,
                    SwapEffect = SwapEffect.Discard,
                    PresentationInterval = PresentInterval.One
                };

            ////this.presentParams.AutoDepthStencilFormat = DepthFormat.D16;
            ////this.presentParams.EnableAutoDepthStencil = true;

            if (!windowed)
            {
                var bounds = Screen.FromControl(this).Bounds;
                this.Bounds = bounds;
                this.presentParams.BackBufferCount = 1;
                this.presentParams.BackBufferWidth = bounds.Width;
                this.presentParams.BackBufferHeight = bounds.Height;
                this.presentParams.BackBufferFormat = adapter.CurrentDisplayMode.Format;

                this.Cursor.Dispose();
            }

            this.Location = Screen.PrimaryScreen.Bounds.Location;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void InitPrimitives()
        {
            // fonts
            this.font = new Font(
                this.device,
                40,
                0,
                FontWeight.Normal,
                10,
                false,
                CharacterSet.Default,
                Precision.Default,
                FontQuality.AntiAliased,
                PitchAndFamily.DefaultPitch,
                "Arial Unicode MS");

            this.fpsFont = new Font(
                this.device,
                20,
                0,
                FontWeight.Normal,
                10,
                false,
                CharacterSet.Default,
                Precision.Default,
                FontQuality.AntiAliased,
                PitchAndFamily.DefaultPitch,
                "Courier New");

            // primitives
            this.colorTriangle = new CustomVertex.TransformedColored[3];
            this.colorTriangle[0].Position = new Vector4(this.pos, 50, 0, 1.0f);
            this.colorTriangle[0].Color = Color.FromArgb(0, 255, 0).ToArgb();
            this.colorTriangle[1].Position = new Vector4(this.pos + 200, 50, 0, 1.0f);
            this.colorTriangle[1].Color = Color.FromArgb(0, 0, 255).ToArgb();
            this.colorTriangle[2].Position = new Vector4(this.pos, 250, 0, 1.0f);
            this.colorTriangle[2].Color = Color.FromArgb(255, 0, 0).ToArgb();

            // video
            try
            {
                //this.InitVideo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitVideo()
        {
            this.videoTexture = new VideoTexture("video.mpg", this.device);

            var quad = new CustomVertex.PositionTextured[4];
            quad[0] = new CustomVertex.PositionTextured(new Vector3(1, 1, 0) * 0.5f, 1, 0);
            quad[1] = new CustomVertex.PositionTextured(new Vector3(-1, 1, 0) * 0.5f, 0, 0);
            quad[2] = new CustomVertex.PositionTextured(new Vector3(1, -1, 0) * 0.5f, 1, 1);
            quad[3] = new CustomVertex.PositionTextured(new Vector3(-1, -1, 0) * 0.5f, 0, 1);

            this.videoVertexBuffer = new VertexBuffer(
                typeof(CustomVertex.PositionTextured), // What type of vertices
                4, // How many
                this.device, // The device
                0, // Default usage
                CustomVertex.PositionTextured.Format, // Vertex format
                Pool.Default); // Default pooling

            var stm = this.videoVertexBuffer.Lock(0, 0, 0);
            stm.Write(quad);
            this.videoVertexBuffer.Unlock();

            this.device.RenderState.CullMode = Cull.None;
            this.device.RenderState.Lighting = false;
        }

        private void InitVideo_DirectX()
        {
            var video = Video.FromFile("video.mpg");
            ////video.Ending += new EventHandler(MovieOver); // TODO: DISPOSING DOESN'T WORK!!
            video.Ending += (s, e) =>
                {
                    // exit video mode
                    this.videoMode = false;
                    this.videoVertexBuffer.Dispose();
                    this.videoVertexBuffer = null;
                    this.Invalidate();
                    video.Pause();
                };
            video.TextureReadyToRender += this.TextureReadyToRender;
            //video.TextureReadyToRender += RenderIt;
            video.RenderToTexture(this.device);
            video.Play();

            var quad = new CustomVertex.PositionTextured[4];
            quad[0] = new CustomVertex.PositionTextured(new Vector3(1, 1, 0) * 0.5f, 1, 0);
            quad[1] = new CustomVertex.PositionTextured(new Vector3(-1, 1, 0) * 0.5f, 0, 0);
            quad[2] = new CustomVertex.PositionTextured(new Vector3(1, -1, 0) * 0.5f, 1, 1);
            quad[3] = new CustomVertex.PositionTextured(new Vector3(-1, -1, 0) * 0.5f, 0, 1);

            this.videoVertexBuffer = new VertexBuffer(
                typeof(CustomVertex.PositionTextured), // What type of vertices
                4, // How many
                this.device, // The device
                0, // Default usage
                CustomVertex.PositionTextured.Format, // Vertex format
                Pool.Default); // Default pooling

            var stm = this.videoVertexBuffer.Lock(0, 0, 0);
            stm.Write(quad);
            this.videoVertexBuffer.Unlock();

            ////return buf;

            this.device.RenderState.CullMode = Cull.None;
            this.device.RenderState.Lighting = false;

            this.videoMode = true;
        }

        /*void RenderIt(object sender, TextureRenderEventArgs e)
        {
            lock (this)
            {
                // We will be updating the texture now
                using (e.Texture)
                {
                    bool beginSceneCalled = false;

                    // Clear the render target and the zbuffer 
                    this.device.Clear(ClearFlags.Target, 0x000000ff, 1.0f, 0);

                    try
                    {
                        this.device.BeginScene();
                        beginSceneCalled = true;

                        // Make sure this isn't being updated at this time
                        // Setup the world matrix
                        device.Transform.World = Matrix.RotationAxis(
                            new Vector3((float)Math.Cos(Environment.TickCount / 250.0f),1,(float)Math.Sin(Environment.TickCount / 250.0f)), 
                            Environment.TickCount / 1000.0f );

                        // Set the texture
                        this.device.SetTexture(0, e.Texture);
                        this.device.SetStreamSource(0, this.videoVertexBuffer, 0);
                        this.device.VertexFormat = CustomVertex.PositionTextured.Format;
                        this.device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                    }
                    finally
                    {
                        if (beginSceneCalled)
                        {
                            this.device.EndScene();
                        }
                    }

                    this.device.Present();
                }
            }
        }
        */

        private void TextureReadyToRender(object sender, TextureRenderEventArgs e)
        {
            if (e.Texture == null || !this.videoMode)
            {
                return;
            }

            SurfaceDescription ds = e.Texture.GetLevelDescription(0);

            /*if (ds.Pool == Pool.Default)
            {
                sysSurf = this.device.CreateOffscreenPlainSurface(ds.Width, ds.Height,
                ds.Format, Pool.SystemMemory);
            }*/

            /*if (this.videoTexture == null)
            {
                this.videoTexture = new Texture(
                    this.device, ds.Width, ds.Height, 1, Usage.Dynamic, ds.Format, ds.Pool);
            }

            using (Surface vidSurf = e.Texture.GetSurfaceLevel(0))
            {
                using (Surface texSurf = this.videoTexture.GetSurfaceLevel(0))
                {
                    ////_device.GetRenderTargetData(vidSurf, sysSurf);
                    ////_device.UpdateSurface(sysSurf, texSurf);
                    SurfaceLoader.FromSurface(texSurf, vidSurf, Filter.Linear, -1);
                }
            }*/

            //Invalidate();
            this.UpdateScene();
            this.DrawScene(e.Texture);
        }

        private void UpdateScene()
        {
            this.colorTriangle[0].X = this.pos;
            this.colorTriangle[1].X = this.pos + 200;
            this.colorTriangle[2].X = this.pos;

            this.pos += 1;
            if (this.pos > this.Width - 50)
            {
                this.pos = 50;
            }
        }

        private void DrawScene(Texture vidTexture)
        {
            this.device.Clear(ClearFlags.Target, Color.Black.ToArgb(), 1.0f, 0);
            this.device.BeginScene();

            this.device.VertexFormat = CustomVertex.TransformedColored.Format;
            this.device.DrawUserPrimitives(PrimitiveType.TriangleList, 1, this.colorTriangle);

            if (this.videoVertexBuffer != null && vidTexture != null)
            {
                this.device.SetTexture(0, vidTexture);
                this.device.SetStreamSource(0, this.videoVertexBuffer, 0);
                this.device.VertexFormat = CustomVertex.PositionTextured.Format;
                this.device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            }
            else if (this.videoTexture != null && this.videoTexture.Texture != null)
            {
                lock (this.videoTexture.Texture)
                {
                    this.device.SetTexture(0, this.videoTexture.Texture);
                    this.device.SetStreamSource(0, this.videoVertexBuffer, 0);
                    this.device.VertexFormat = CustomVertex.PositionTextured.Format;
                    this.device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                }
            }

            // Draw our two spinning meshes
            this.Draw3DText(new Vector3(1.0f, 1.0f, 0.0f), new Vector3(-3.0f, 0.0f, 0.0f));
            this.Draw3DText(new Vector3(0.0f, 1.0f, 1.0f), new Vector3(0.0f, -1.0f, 1.0f));

            this.UpdateFramerate();
            this.UpdateTexts();
            this.device.EndScene();
            this.device.Present();
        }

        private void UpdateFramerate()
        {
            this.frameCounter++;
            if (Math.Abs(Environment.TickCount - this.lastTickCount) > 200)
            {
                this.lastFrameRate = (float)this.frameCounter * 1000 / Math.Abs(Environment.TickCount - this.lastTickCount);
                this.lastTickCount = Environment.TickCount;
                this.frameCounter = 0;
            }

            this.fpsFont.DrawText(null, string.Format("{0:0.00} fps", this.lastFrameRate), new Point(20, 20), Color.Red);
        }

        private void UpdateTexts()
        {
            this.font.DrawText(null, "Hello World", 20, 50, Color.White);
            this.font.DrawText(null, "محتويات", 20, 100, Color.White);
            this.font.DrawText(null, "הרפורמות בספרד", 20, 150, Color.White);
            this.font.DrawText(null, "Some scrolling text (without a Sprite)", this.pos, 200, Color.White);
            this.font.DrawText(null, this.displayModeText, 20, 300, Color.White);
        }
    }
}

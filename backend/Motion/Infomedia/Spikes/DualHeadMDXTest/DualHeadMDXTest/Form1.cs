namespace DualHeadMDXTest
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Microsoft.DirectX;
    using Microsoft.DirectX.AudioVideoPlayback;
    using Microsoft.DirectX.Direct3D;

    using DxFont = Microsoft.DirectX.Direct3D.Font;

    public partial class Form1 : Form
    {
        private Device device;

        private int pos = 50;

        private CustomVertex.TransformedColored[] colorTriangle;

        private DxFont font;
        private DxFont fpsFont;

        private long frameCounter;
        private int lastTickCount;
        private float lastFrameRate;

        private bool videoMode;
        private Texture videoTexture;
        private VertexBuffer videoVertexBuffer;

        private Device device1;
        private DxFont font1;

        private Form window1;
        private long frameCounter1;
        private int lastTickCount1;
        private float lastFrameRate1;

        private DxFont fpsFont1;

        private int pos1;

        public Form1()
        {
            InitializeComponent();

            this.InitGraphics(true);
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
            }
        }

        private void InitGraphics(bool windowed)
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            var adapter = Manager.Adapters.Default;
            var presentParams = new PresentParameters
            {
                Windowed = windowed,
                SwapEffect = SwapEffect.Discard,
                PresentationInterval = PresentInterval.Immediate
            };

            if (!windowed)
            {
                var bounds = Screen.FromControl(this).Bounds;
                this.Bounds = bounds;
                presentParams.BackBufferCount = 1;
                presentParams.BackBufferWidth = bounds.Width;
                presentParams.BackBufferHeight = bounds.Height;
                presentParams.BackBufferFormat = adapter.CurrentDisplayMode.Format;

                this.Cursor.Dispose();
            }

            this.window1 = new Form();
            window1.Text = "Second Window";
            Screen scrn = Screen.FromControl(this);
            Screen scrn1 = Screen.FromControl(window1);
            var sc = Screen.AllScreens;
            Screen targetScreen = Screen.PrimaryScreen;
            foreach (Screen scr in sc)
            {
                if (!scr.Primary)
                {
                    // This is not the primary monitor.
                    targetScreen = scr;
                    break;
                }
            }
            window1.Location = targetScreen.Bounds.Location;
            this.Location = Screen.PrimaryScreen.Bounds.Location;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            window1.WindowState = FormWindowState.Maximized;
            window1.FormBorderStyle = FormBorderStyle.None;
            window1.Show();

            var presentParams1 = new PresentParameters
            {
                Windowed = windowed,
                SwapEffect = SwapEffect.Discard,
                PresentationInterval = PresentInterval.Immediate
            };

            if (!windowed)
            {
                var bounds = Screen.FromControl(window1).Bounds;
                this.Bounds = bounds;
                presentParams1.BackBufferCount = 1;
                presentParams1.BackBufferWidth = bounds.Width;
                presentParams1.BackBufferHeight = bounds.Height;
                presentParams1.BackBufferFormat = Manager.Adapters[1].CurrentDisplayMode.Format;

                this.Cursor.Dispose();
            }

            //this.device = new Device(adapter.Adapter, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);
            try
            {
                this.device = new Device(
                    adapter.Adapter, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);
            }
            catch (Exception e)
            {
                Console.Write("Device not created due to: {0}", e);
            }
            try
            {
                this.device1 = new Device(Manager.Adapters[1].Adapter, DeviceType.Hardware, window1, CreateFlags.SoftwareVertexProcessing, presentParams1);
            }
            catch (Exception e)
            {
                Console.Write("Device1 not created due to: {0}", e);
            }
        }

        private void InitPrimitives()
        {
            // fonts
            this.font = new DxFont(
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

            this.fpsFont = new DxFont(
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

            this.font1 = new DxFont(
                this.device1,
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

            this.fpsFont1 = new DxFont(
                this.device1,
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

            this.pos1 += 1;
            if (this.pos1 > window1.Width - 50)
            {
                this.pos1 = 50;
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

            this.UpdateFramerate();
            this.UpdateTexts();
            this.device.EndScene();
            this.device.Present();

            this.device1.Clear(ClearFlags.Target, Color.Blue.ToArgb(), 1.0f, 0);
            this.device1.BeginScene();
            this.UpdateFramerate1();
            this.font1.DrawText(null, "Secondary monitor", 20, 50, Color.White);
            this.font1.DrawText(null, "Some scrolling text (without a Sprite)", this.pos1, 200, Color.White);
            this.device1.EndScene();
            this.device1.Present();
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

        private void UpdateFramerate1()
        {
            this.frameCounter1++;
            if (Math.Abs(Environment.TickCount - this.lastTickCount1) > 200)
            {
                this.lastFrameRate1 = (float)this.frameCounter1 * 1000 / Math.Abs(Environment.TickCount - this.lastTickCount1);
                this.lastTickCount1 = Environment.TickCount;
                this.frameCounter1 = 0;
            }

            this.fpsFont1.DrawText(null, string.Format("{0:0.00} fps", this.lastFrameRate1), new Point(20, 20), Color.Red);
        }

        private void UpdateTexts()
        {
            this.font.DrawText(null, "Primary Monitor", 20, 50, Color.White);
            this.font.DrawText(null, "محتويات", 20, 100, Color.White);
            this.font.DrawText(null, "הרפורמות בספרד", 20, 150, Color.White);
            this.font.DrawText(null, "Some scrolling text (without a Sprite)", this.pos, 200, Color.White);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Form1.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace VideoEvaluation
{
    using System;
    using System.Configuration;
    using System.Drawing;
    using System.Windows.Forms;

    using Microsoft.DirectX;
    using Microsoft.DirectX.AudioVideoPlayback;
    using Microsoft.DirectX.Direct3D;

    using NLog;

    using DFont = Microsoft.DirectX.Direct3D.Font;

    /// <summary>
    /// The form 1.
    /// </summary>
    public partial class Form1 : Form
    {
        #region Constants and Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The device.
        /// </summary>
        private Device device;

        /// <summary>
        /// The draw panel.
        /// </summary>
        private Panel drawPanel;

        /// <summary>
        /// The fps font.
        /// </summary>
        private DFont fpsFont;

        /// <summary>
        /// The frame counter.
        /// </summary>
        private long frameCounter;

        /// <summary>
        /// The hebrew position.
        /// </summary>
        private int hebrewPosition;

        private bool isPlaying;

        /// <summary>
        /// The last frame rate.
        /// </summary>
        private float lastFrameRate;

        /// <summary>
        /// The last tick count.
        /// </summary>
        private int lastTickCount;

        /// <summary>
        /// The locker.
        /// </summary>
        private object locker = new object();

        /// <summary>
        /// The position.
        /// </summary>
        private int position;

        /// <summary>
        /// The scroll font.
        /// </summary>
        private DFont scrollFont;

        /// <summary>
        /// The video.
        /// </summary>
        private Video video;

        /// <summary>
        /// The video panel.
        /// </summary>
        private Panel videoPanel;

        /// <summary>
        /// The video texture.
        /// </summary>
        private Texture videoTexture;

        /// <summary>
        /// The video vertex buffer.
        /// </summary>
        private VertexBuffer videoVertexBuffer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();
            try
            {
                this.InitGraphics();
                this.InitFonts();
                if (ConfigurationManager.AppSettings["StartVideoTopMost"].Equals("0"))
                {
                    this.radioButton2.Checked = true;
                }
                else
                {
                    this.radioButton1.Checked = true;
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().ErrorException("directx error", e);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The init graphics.
        /// </summary>
        public void InitGraphics()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.Opaque, false);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.splitContainer1.Size = this.Size;

            this.drawPanel = new Panel { Size = this.splitContainer1.Panel2.Size };
            this.splitContainer1.Panel2.Controls.Add(this.drawPanel);

            var presentParams = new PresentParameters
                {
                    BackBufferCount = 1, 
                    DeviceWindow = this.drawPanel, 
                    Windowed = true, 
                    SwapEffect = SwapEffect.Discard, 
                    PresentationInterval = PresentInterval.Immediate, 
                    BackBufferFormat = Format.X8R8G8B8
                };
            var adapter = Manager.Adapters.Default;
            try
            {
                this.device = new Device(
                    adapter.Adapter, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);
            }
            catch (Exception)
            {
                Logger.Info("Create device");
            }

            var caps = Manager.GetDeviceCaps(adapter.Adapter, DeviceType.Hardware);
            if (this.device == null)
            {
                Logger.Info("device = null");
            }

            this.vName.Text = Manager.Adapters[0].CurrentDisplayMode.Format.ToString();
            Logger.Info(
                "StretchRectangleFromTextures {0}, SupportsTextureSystemMem {1}, SupportsTextureVideoMem {2} CanDrawSystemToNonLocal {3}", 
                caps.DeviceCaps.CanStretchRectangleFromTextures, 
                caps.DeviceCaps.SupportsTextureSystemMemory, 
                caps.DeviceCaps.SupportsTextureVideoMemory, 
                caps.DeviceCaps.CanDrawSystemToNonLocal);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on paint.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.isPlaying)
            {
                return;
            }

            this.DrawScene(null);

            // this.Invalidate();
        }

        /// <summary>
        /// The button 1_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Button1Click(object sender, EventArgs e)
        {
            if (this.isPlaying || this.video == null)
            {
                return;
            }

            this.isPlaying = true;
            this.video.Play();
        }

        /// <summary>
        /// The button 2_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Button2Click(object sender, EventArgs e)
        {
            if (this.video == null)
            {
                return;
            }

            if (this.isPlaying)
            {
                this.video.Pause();
                this.isPlaying = false;
            }
        }

        /// <summary>
        /// The button 3_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Button3Click(object sender, EventArgs e)
        {
            if (this.video == null)
            {
                return;
            }

            if (this.isPlaying)
            {
                this.video.Stop();
                this.isPlaying = false;
            }
        }

        /// <summary>
        /// The check box 1_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                this.timer1.Start();
            }
            else
            {
                this.timer1.Stop();
            }
        }

        /// <summary>
        /// The draw scene.
        /// </summary>
        /// <param name="vidTexture">
        /// The vid Texture.
        /// </param>
        private void DrawScene(Texture vidTexture)
        {
            lock (this.locker)
            {
                this.device.Clear(ClearFlags.Target, Color.Black, 1.0f, 0);
                this.device.BeginScene();

                this.device.VertexFormat = CustomVertex.TransformedColored.Format;
                try
                {
                    if (this.videoVertexBuffer != null && vidTexture != null)
                    {
                        this.device.SetTexture(0, vidTexture);
                        this.device.SetStreamSource(0, this.videoVertexBuffer, 0);
                        this.device.VertexFormat = CustomVertex.PositionTextured.Format;
                        this.device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorException("DrawScene vertex.", e);
                }

                this.UpdateFramerate();
                this.UpdateText();
                this.device.EndScene();
                this.device.Present();
            }
        }

        private void Form1KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Keys.F))
            {
                this.checkBox2.Checked = !this.checkBox2.Checked;
            }
        }

        /// <summary>
        /// The init texts.
        /// </summary>
        private void InitFonts()
        {
            this.fpsFont = new DFont(
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
            this.scrollFont = new DFont(
                this.device, 
                90, 
                0, 
                FontWeight.Normal, 
                10, 
                false, 
                CharacterSet.Default, 
                Precision.Default, 
                FontQuality.AntiAliased, 
                PitchAndFamily.DefaultPitch, 
                "Arial");
        }

        private void InitVideo()
        {
            if (this.radioButton1.Checked)
            {
                this.InitVideoTopMost();
            }
            else
            {
                this.InitVideoWithTexture();
            }
        }

        /// <summary>
        /// The init video top most.
        /// </summary>
        private void InitVideoTopMost()
        {
            if (this.video == null)
            {
                return;
            }

            if (this.videoPanel == null)
            {
                this.videoPanel = new Panel
                    {
                       Size = new Size(this.video.DefaultSize.Width, this.video.DefaultSize.Height + 20) 
                    };
                this.splitContainer1.Panel2.Controls.Add(this.videoPanel);
            }
            else
            {
                this.videoPanel.Size = this.video.DefaultSize;
            }

            this.videoPanel.Location = new Point(this.videoPanel.Location.X, this.videoPanel.Location.Y + 30);

            ////this.splitContainer1.Panel2.Size = this.videoPanel.Size;
            this.videoPanel.BringToFront();
            this.video.Owner = this.videoPanel;
            this.video.Play();
            this.video.Stop();
        }

        /// <summary>
        /// The init video.
        /// </summary>
        private void InitVideoWithTexture()
        {
            Logger.Info("Entering InitVideoWithTexture...");
            if (this.video == null)
            {
                Logger.Info("Video = null");
                return;
            }

            if (this.videoPanel != null)
            {
                Logger.Info("Send videopanel to back.");
                this.videoPanel.SendToBack();
            }

            if (this.device == null)
            {
                Logger.Info("Device = null.");
                return;
            }

            ////var aspect = (float)this.video.DefaultSize.Width / this.video.DefaultSize.Height;
            ////var width = (float)this.video.DefaultSize.Width;
            ////var height = (float)this.video.DefaultSize.Height;
            Logger.Info("Add TextureReadyToRender Event.");
            this.video.TextureReadyToRender += this.TextureReadyToRender;
            Logger.Info("Set owner to drawPanel.");
            this.video.Owner = this.drawPanel;
            Logger.Info("Bring drawPanel to front.");
            this.drawPanel.BringToFront();
            if (this.video == null)
            {
                Logger.Info("Video = null");
            }

            Logger.Info("Trying to set RenderToTexture");
            this.video.RenderToTexture(this.device);
            Logger.Info("After RenderToTexture");

            // this.video.Play();
            this.video.Stop();
            Logger.Info("Video stopped.");
            var quad = new CustomVertex.PositionTextured[4];
            quad[1] = new CustomVertex.PositionTextured(new Vector3(-1, 1, 0), 0, 0);
            quad[0] = new CustomVertex.PositionTextured(new Vector3(1, 1, 0), 1, 0);
            quad[2] = new CustomVertex.PositionTextured(new Vector3(1, -1, 0), 1, 1);
            quad[3] = new CustomVertex.PositionTextured(new Vector3(-1, -1, 0), 0, 1);

            try
            {
                this.videoVertexBuffer = new VertexBuffer(
                    typeof(CustomVertex.PositionTextured), 
                    4, 
                    this.device, 
                    0, 
                    CustomVertex.PositionTextured.Format, 
                    Pool.Default); // Default pooling
            }
            catch (Exception)
            {
                Logger.Error("InitVideo, create Vertexbuffer.");
            }

            var stm = this.videoVertexBuffer.Lock(0, 0, 0);
            stm.Write(quad);
            this.videoVertexBuffer.Unlock();
            this.device.RenderState.CullMode = Cull.None;
            this.device.RenderState.Lighting = false;
        }

        /// <summary>
        /// The radio button 1_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RadioButton1CheckedChanged(object sender, EventArgs e)
        {
            if (this.video == null)
            {
                return;
            }

            if (this.radioButton1.Checked)
            {
                this.InitVideoTopMost();
            }
            else
            {
                this.InitVideoWithTexture();
            }
        }

        /// <summary>
        /// The texture ready to render.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextureReadyToRender(object sender, TextureRenderEventArgs e)
        {
            Logger.Info("Entering TextureReadyToRender...");
            if (e.Texture == null)
            {
                return;
            }

            var ds = e.Texture.GetLevelDescription(0);

            // if (ds.Pool == Pool.Default)
            // {
            // sysSurf = this.device.CreateOffscreenPlainSurface(ds.Width, ds.Height, ds.Format, Pool.SystemMemory);
            // }
            using (Surface vidSurf = e.Texture.GetSurfaceLevel(0))
            {
                if (this.videoTexture == null)
                {
                    this.videoTexture = new Texture(
                    this.device, ds.Width, ds.Height, 1, Usage.Dynamic, ds.Format, ds.Pool);
                }

                using (Surface texSurf = this.videoTexture.GetSurfaceLevel(0))
                {
                    SurfaceLoader.FromSurface(texSurf, vidSurf, Filter.Linear, unchecked((int)0xffffffff));
                }
            }

            this.DrawScene(e.Texture);
            this.Invalidate();
        }

        /// <summary>
        /// The timer 1_ tick_1.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Timer1Tick1(object sender, EventArgs e)
        {
            if (this.position < this.splitContainer1.Panel2.ClientRectangle.Left)
            {
                this.position = this.splitContainer1.Panel2.ClientRectangle.Right - 10;
            }

            this.position--;

            if (this.hebrewPosition > this.splitContainer1.Panel2.ClientRectangle.Right - 20)
            {
                this.hebrewPosition = this.splitContainer1.Panel2.ClientRectangle.Left + 10;
            }

            this.hebrewPosition++;

            if (this.radioButton1.Checked || !this.isPlaying)
            {
                this.DrawScene(null);
            }
        }

        /// <summary>
        /// The update framerate.
        /// </summary>
        private void UpdateFramerate()
        {
            this.frameCounter++;
            if (Math.Abs(Environment.TickCount - this.lastTickCount) > 200)
            {
                this.lastFrameRate = (float)this.frameCounter * 1000
                                     / Math.Abs(Environment.TickCount - this.lastTickCount);
                this.lastTickCount = Environment.TickCount;
                this.frameCounter = 0;
            }

            this.fpsFont.DrawText(
                null, 
                string.Format("{0:0.00} fps", this.lastFrameRate), 
                new Point(
                    this.splitContainer1.Panel2.ClientRectangle.Left + 5, 
                    this.splitContainer1.Panel2.ClientRectangle.Top + 10), 
                Color.Red);
        }

        /// <summary>
        /// The update text.
        /// </summary>
        private void UpdateText()
        {
            this.scrollFont.DrawText(
                null, 
                "Scrolling text with some more text to display.", 
                this.position, 
                this.ClientRectangle.Top + 20, 
                Color.Aqua);

            ////this.hebrewFont.DrawText(
            ////    null, 
            ////    "א  בְּרֵאשִׁית, בָּרָא אֱלֹהִים, אֵת הַשָּׁמַיִם, וְאֵת הָאָרֶץ.", 
            ////    this.hebrewPosition, 
            ////    this.ClientRectangle.Top + 30, 
            ////    Color.BlanchedAlmond);
        }

        private void Button4Click(object sender, EventArgs e)
        {
            if (this.video != null)
            {
                this.video.Stop();
                this.video = null;
            }

            var fileDialog = new OpenFileDialog
                {
                   InitialDirectory = @"D:\Infomedia\Data\Pool\Videos", CheckFileExists = true 
                };
            fileDialog.ShowDialog();
            this.video = new Video(fileDialog.FileName);

            this.vName.Text = fileDialog.FileName;
            this.vRes.Text = string.Format("{0} x {1}", this.video.DefaultSize.Width, this.video.DefaultSize.Height);
            this.vCodec.Text = this.video.State.ToString();
            this.InitVideo();
        }

        private void CheckBox2CheckedChanged(object sender, EventArgs e)
        {
            if (this.video != null)
            {
                this.video.Fullscreen = this.checkBox2.Checked;
            }
        }

        #endregion
    }
}
namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    using Microsoft.DirectX;

    /// <summary>
    /// Class holds arcball data
    /// </summary>
    public class ArcBall
    {
        #region Instance Data
        protected Matrix rotation; // Matrix for arc ball's orientation
        protected Matrix translation; // Matrix for arc ball's position
        protected Matrix translationDelta; // Matrix for arc ball's position

        protected int width; // arc ball's window width
        protected int height; // arc ball's window height
        protected Vector2 center;  // center of arc ball 
        protected float radius; // arc ball's radius in screen coords
        protected float radiusTranslation; // arc ball's radius for translating the target

        protected Quaternion downQuat; // Quaternion before button down
        protected Quaternion nowQuat; // Composite quaternion for current drag
        protected bool isDragging; // Whether user is dragging arc ball

        protected System.Drawing.Point lastMousePosition; // position of last mouse point
        protected Vector3 downPt; // starting point of rotation arc
        protected Vector3 currentPt; // current point of rotation arc
        #endregion

        #region Simple Properties
        /// <summary>Gets the rotation matrix</summary>
        public Matrix RotationMatrix { get { return this.rotation = Matrix.RotationQuaternion(this.nowQuat); } }
        /// <summary>Gets the translation matrix</summary>
        public Matrix TranslationMatrix { get { return this.translation; } }
        /// <summary>Gets the translation delta matrix</summary>
        public Matrix TranslationDeltaMatrix { get { return this.translationDelta; } }
        /// <summary>Gets the dragging state</summary>
        public bool IsBeingDragged { get { return this.isDragging; } }
        /// <summary>Gets or sets the current quaternion</summary>
        public Quaternion CurrentQuaternion { get { return this.nowQuat; } set { this.nowQuat = value; } }
        #endregion

        // Class methods

        /// <summary>
        /// Create new instance of the arcball class
        /// </summary>
        public ArcBall()
        {
            this.Reset();
            this.downPt = Vector3.Empty;
            this.currentPt = Vector3.Empty;

            System.Windows.Forms.Form active = System.Windows.Forms.Form.ActiveForm;
            if (active != null)
            {
                System.Drawing.Rectangle rect = active.ClientRectangle;
                this.SetWindow(rect.Width, rect.Height);
            }
        }

        /// <summary>
        /// Resets the arcball
        /// </summary>
        public void Reset()
        {
            this.downQuat = Quaternion.Identity;
            this.nowQuat = Quaternion.Identity;
            this.rotation = Matrix.Identity;
            this.translation = Matrix.Identity;
            this.translationDelta = Matrix.Identity;
            this.isDragging = false;
            this.radius = 1.0f;
            this.radiusTranslation = 1.0f;
        }

        /// <summary>
        /// Convert a screen point to a vector
        /// </summary>
        public Vector3 ScreenToVector(float screenPointX, float screenPointY)
        {
            float x = -(screenPointX - this.width / 2.0f) / (this.radius * this.width/2.0f);
            float y = (screenPointY - this.height / 2.0f) / (this.radius * this.height/2.0f);
            float z = 0.0f;
            float mag = (x*x) + (y*y);

            if (mag > 1.0f)
            {
                float scale = 1.0f / (float)Math.Sqrt(mag);
                x *= scale;
                y *= scale;
            }
            else
                z = (float)Math.Sqrt(1.0f - mag);

            return new Vector3(x,y,z);
        }

        /// <summary>
        /// Set window paramters
        /// </summary>
        public void SetWindow(int w, int h, float r)
        {
            this.width = w; this.height = h; this.radius = r;
            this.center = new Vector2(w / 2.0f, h / 2.0f);
        }
        public void SetWindow(int w, int h)
        {
            this.SetWindow(w,h,0.9f); // default radius
        }

        /// <summary>
        /// Computes a quaternion from ball points
        /// </summary>
        public static Quaternion QuaternionFromBallPoints(Vector3 from, Vector3 to)
        {
            float dot = Vector3.Dot(from, to);
            Vector3 part = Vector3.Cross(from, to);
            return new Quaternion(part.X, part.Y, part.Z, dot);
        }

        /// <summary>
        /// Begin the arcball 'dragging'
        /// </summary>
        public void OnBegin(int x, int y)
        {
            this.isDragging = true;
            this.downQuat = this.nowQuat;
            this.downPt = this.ScreenToVector((float)x, (float)y);
        }
        /// <summary>
        /// The arcball is 'moving'
        /// </summary>
        public void OnMove(int x, int y)
        {
            if (this.isDragging)
            {
                this.currentPt = this.ScreenToVector((float)x, (float)y);
                this.nowQuat = this.downQuat * QuaternionFromBallPoints(this.downPt, this.currentPt);
            }
        }
        /// <summary>
        /// Done dragging the arcball
        /// </summary>
        public void OnEnd()
        {
            this.isDragging = false;
        }

        /// <summary>
        /// Handle messages from the window
        /// </summary>
        public bool HandleMessages(IntPtr hWnd, NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam)
        {
            // Current mouse position
            short mouseX = NativeMethods.LoWord((uint)lParam.ToInt32());
            short mouseY = NativeMethods.HiWord((uint)lParam.ToInt32());

            switch(msg)
            {
                case NativeMethods.WindowMessage.LeftButtonDown:
                case NativeMethods.WindowMessage.LeftButtonDoubleClick:
                    // Set capture
                    NativeMethods.SetCapture(hWnd);
                    this.OnBegin(mouseX, mouseY);
                    return true;
                case NativeMethods.WindowMessage.LeftButtonUp:
                    // Release capture
                    NativeMethods.ReleaseCapture();
                    this.OnEnd();
                    return true;

                case NativeMethods.WindowMessage.RightButtonDown:
                case NativeMethods.WindowMessage.RightButtonDoubleClick:
                case NativeMethods.WindowMessage.MiddleButtonDown:
                case NativeMethods.WindowMessage.MiddleButtonDoubleClick:
                    // Set capture
                    NativeMethods.SetCapture(hWnd);
                    // Store off the position of the cursor
                    this.lastMousePosition = new System.Drawing.Point(mouseX, mouseY);
                    return true;

                case NativeMethods.WindowMessage.RightButtonUp:
                case NativeMethods.WindowMessage.MiddleButtonUp:
                    // Release capture
                    NativeMethods.ReleaseCapture();
                    return true;

                case NativeMethods.WindowMessage.MouseMove:
                    short buttonState = NativeMethods.LoWord((uint)wParam.ToInt32());
                    bool leftButton = ((buttonState & (short)NativeMethods.MouseButtons.Left) != 0);
                    bool rightButton = ((buttonState & (short)NativeMethods.MouseButtons.Right) != 0);
                    bool middleButton = ((buttonState & (short)NativeMethods.MouseButtons.Middle) != 0);

                    if (leftButton)
                    {
                        this.OnMove(mouseX, mouseY);
                    }
                    else if (rightButton || middleButton)
                    {
                        // Normalize based on size of window and bounding sphere radius
                        float deltaX = (this.lastMousePosition.X - mouseX) * this.radiusTranslation / this.width;
                        float deltaY = (this.lastMousePosition.Y - mouseY) * this.radiusTranslation / this.height;

                        if (rightButton)
                        {
                            this.translationDelta = Matrix.Translation(-2*deltaX,2*deltaY, 0.0f);
                            this.translation *= this.translationDelta;
                        }
                        else // Middle button
                        {
                            this.translationDelta = Matrix.Translation(0.0f, 0.0f, 5*deltaY);
                            this.translation *= this.translationDelta;
                        }
                        // Store off the position of the cursor
                        this.lastMousePosition = new System.Drawing.Point(mouseX, mouseY);
                    }
                    return true;
            }

            return false;
        }
    }
}
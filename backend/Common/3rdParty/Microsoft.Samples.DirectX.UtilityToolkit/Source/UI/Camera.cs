namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    using Microsoft.DirectX;

    /// <summary>
    /// Simple base camera class that moves and rotates.  The base class
    /// records mouse and keyboard input for use by a derived class, and 
    /// keeps common state.
    /// </summary>
    public abstract class Camera
    {
        /// <summary>
        /// Maps NativeMethods.WindowMessage.Key* msg to a camera key
        /// </summary>
        protected static CameraKeys MapKey(IntPtr param)
        {
            System.Windows.Forms.Keys key = (System.Windows.Forms.Keys)param.ToInt32();
            switch(key)
            {
                case System.Windows.Forms.Keys.ControlKey: return CameraKeys.ControlDown;
                case System.Windows.Forms.Keys.Left: return CameraKeys.StrafeLeft;
                case System.Windows.Forms.Keys.Right: return CameraKeys.StrafeRight;
                case System.Windows.Forms.Keys.Up: return CameraKeys.MoveForward;
                case System.Windows.Forms.Keys.Down: return CameraKeys.MoveBackward;
                case System.Windows.Forms.Keys.Prior: return CameraKeys.MoveUp; // pgup
                case System.Windows.Forms.Keys.Next: return CameraKeys.MoveDown; // pgdn

                case System.Windows.Forms.Keys.A: return CameraKeys.StrafeLeft;
                case System.Windows.Forms.Keys.D: return CameraKeys.StrafeRight;
                case System.Windows.Forms.Keys.W: return CameraKeys.MoveForward;
                case System.Windows.Forms.Keys.S: return CameraKeys.MoveBackward;
                case System.Windows.Forms.Keys.Q: return CameraKeys.MoveUp; 
                case System.Windows.Forms.Keys.E: return CameraKeys.MoveDown; 

                case System.Windows.Forms.Keys.NumPad4: return CameraKeys.StrafeLeft;
                case System.Windows.Forms.Keys.NumPad6: return CameraKeys.StrafeRight;
                case System.Windows.Forms.Keys.NumPad8: return CameraKeys.MoveForward;
                case System.Windows.Forms.Keys.NumPad2: return CameraKeys.MoveBackward;
                case System.Windows.Forms.Keys.NumPad9: return CameraKeys.MoveUp; 
                case System.Windows.Forms.Keys.NumPad3: return CameraKeys.MoveDown; 

                case System.Windows.Forms.Keys.Home: return CameraKeys.Reset; 
            }
            // No idea
            return (CameraKeys)byte.MaxValue;
        }


        #region Instance Data
        protected Matrix viewMatrix; // View Matrix
        protected Matrix projMatrix; // Projection matrix

        protected System.Drawing.Point lastMousePosition;  // Last absolute position of mouse cursor
        protected bool isMouseLButtonDown;    // True if left button is down 
        protected bool isMouseMButtonDown;    // True if middle button is down 
        protected bool isMouseRButtonDown;    // True if right button is down 
        protected int currentButtonMask;   // mask of which buttons are down
        protected int mouseWheelDelta;     // Amount of middle wheel scroll (+/-) 
        protected Vector2 mouseDelta;          // Mouse relative delta smoothed over a few frames
        protected float framesToSmoothMouseData; // Number of frames to smooth mouse data over

        protected Vector3 defaultEye;          // Default camera eye position
        protected Vector3 defaultLookAt;       // Default LookAt position
        protected Vector3 eye;                 // Camera eye position
        protected Vector3 lookAt;              // LookAt position
        protected float cameraYawAngle;      // Yaw angle of camera
        protected float cameraPitchAngle;    // Pitch angle of camera

        protected System.Drawing.Rectangle dragRectangle; // Rectangle within which a drag can be initiated.
        protected Vector3 velocity;            // Velocity of camera
        protected bool isMovementDrag;        // If true, then camera movement will slow to a stop otherwise movement is instant
        protected Vector3 velocityDrag;        // Velocity drag force
        protected float dragTimer;           // Countdown timer to apply drag
        protected float totalDragTimeToZero; // Time it takes for velocity to go from full to 0
        protected Vector2 rotationVelocity;         // Velocity of camera

        protected float fieldOfView;                 // Field of view
        protected float aspectRatio;              // Aspect ratio
        protected float nearPlane;           // Near plane
        protected float farPlane;            // Far plane

        protected float rotationScaler;      // Scaler for rotation
        protected float moveScaler;          // Scaler for movement

        protected bool isInvertPitch;         // Invert the pitch axis
        protected bool isEnablePositionMovement; // If true, then the user can translate the camera/model 
        protected bool isEnableYAxisMovement; // If true, then camera can move in the y-axis

        protected bool isClipToBoundary;      // If true, then the camera will be clipped to the boundary
        protected Vector3 minBoundary;         // Min point in clip boundary
        protected Vector3 maxBoundary;         // Max point in clip boundary

        protected bool isResetCursorAfterMove;// If true, the class will reset the cursor position so that the cursor always has space to move 

        // State of the input
        protected bool[] keys;
        public static readonly Vector3 UpDirection = new Vector3(0,1,0);
        #endregion

        #region Simple Properties
        /// <summary>Is the camera being 'dragged' at all?</summary>
        public bool IsBeingDragged { get { return (this.isMouseLButtonDown || this.isMouseMButtonDown || this.isMouseRButtonDown); } }
        /// <summary>Is the left mouse button down</summary>
        public bool IsMouseLeftButtonDown { get { return this.isMouseLButtonDown; } }
        /// <summary>Is the right mouse button down</summary>
        public bool IsMouseRightButtonDown { get { return this.isMouseRButtonDown; } }
        /// <summary>Is the middle mouse button down</summary>
        public bool IsMouseMiddleButtonDown { get { return this.isMouseMButtonDown; } }
        /// <summary>Returns the view transformation matrix</summary>
        public Matrix ViewMatrix { get { return this.viewMatrix; } }
        /// <summary>Returns the projection transformation matrix</summary>
        public Matrix ProjectionMatrix { get { return this.projMatrix; } }
        /// <summary>Returns the location of the eye</summary>
        public Vector3 EyeLocation { get { return this.eye; } }
        /// <summary>Returns the look at point of the camera</summary>
        public Vector3 LookAtPoint { get { return this.lookAt; } }
        /// <summary>Is position movement enabled</summary>
        public bool IsPositionMovementEnabled { get {return this.isEnablePositionMovement; } set { this.isEnablePositionMovement = value; } }
        #endregion
        
        /// <summary>
        /// Abstract method to control camera during frame move
        /// </summary>
        public abstract void FrameMove(float elapsedTime);

        /// <summary>
        /// Constructor for the base camera class (Sets up camera defaults)
        /// </summary>
        protected Camera()
        {
            // Create the keys
            this.keys = new bool[(int)CameraKeys.MaxKeys];

            // Set attributes for the view matrix
            this.eye = Vector3.Empty;
            this.lookAt = new Vector3(0.0f, 0.0f, 1.0f);

            // Setup the view matrix
            this.SetViewParameters(this.eye, this.lookAt);

            // Setup the projection matrix
            this.SetProjectionParameters((float)Math.PI / 4, 1.0f, 1.0f, 1000.0f);

            // Store mouse information
            this.lastMousePosition = System.Windows.Forms.Cursor.Position;
            this.isMouseLButtonDown = false;
            this.isMouseRButtonDown = false;
            this.isMouseMButtonDown = false;
            this.mouseWheelDelta = 0;
            this.currentButtonMask = 0;

            // Setup camera rotations
            this.cameraYawAngle = 0.0f;
            this.cameraPitchAngle = 0.0f;

            this.dragRectangle = new System.Drawing.Rectangle(0, 0, int.MaxValue, int.MaxValue);
            this.velocity = Vector3.Empty;
            this.isMovementDrag = false;
            this.velocityDrag = Vector3.Empty;
            this.dragTimer = 0.0f;
            this.totalDragTimeToZero = 0.25f;
            this.rotationVelocity = Vector2.Empty;
            this.rotationScaler = 0.1f;
            this.moveScaler = 5.0f;
            this.isInvertPitch = false;
            this.isEnableYAxisMovement = true;
            this.isEnablePositionMovement = true;
            this.mouseDelta = Vector2.Empty;
            this.framesToSmoothMouseData = 2.0f;
            this.isClipToBoundary = false;
            this.minBoundary = new Vector3(-1.0f,-1.0f, -1.0f);
            this.maxBoundary = new Vector3(1,1,1);
            this.isResetCursorAfterMove = false;
        }

        /// <summary>
        /// Call this from your message proc so this class can handle window messages
        /// </summary>
        public virtual bool HandleMessages(IntPtr hWnd, NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam)
        {
            switch(msg)
            {
                    // Handle the keyboard
                case NativeMethods.WindowMessage.KeyDown:
                    CameraKeys mappedKeyDown = MapKey(wParam);
                    if (mappedKeyDown != (CameraKeys)byte.MaxValue)
                    {
                        // Valid key was pressed, mark it as 'down'
                        this.keys[(int)mappedKeyDown] = true;
                    }
                    break;
                case NativeMethods.WindowMessage.KeyUp:
                    CameraKeys mappedKeyUp = MapKey(wParam);
                    if (mappedKeyUp != (CameraKeys)byte.MaxValue)
                    {
                        // Valid key was let go, mark it as 'up'
                        this.keys[(int)mappedKeyUp] = false;
                    }
                    break;

                    // Handle the mouse
                case NativeMethods.WindowMessage.LeftButtonDoubleClick:
                case NativeMethods.WindowMessage.LeftButtonDown:
                case NativeMethods.WindowMessage.RightButtonDoubleClick:
                case NativeMethods.WindowMessage.RightButtonDown:
                case NativeMethods.WindowMessage.MiddleButtonDoubleClick:
                case NativeMethods.WindowMessage.MiddleButtonDown:
                    {
                        // Compute the drag rectangle in screen coord.
                        System.Drawing.Point cursor = new System.Drawing.Point(
                            NativeMethods.LoWord((uint)lParam.ToInt32()),
                            NativeMethods.HiWord((uint)lParam.ToInt32()));

                        // Update the variable state
                        if ( ((msg == NativeMethods.WindowMessage.LeftButtonDown) ||
                              (msg == NativeMethods.WindowMessage.LeftButtonDoubleClick) )
                             && this.dragRectangle.Contains(cursor) )
                        {
                            this.isMouseLButtonDown = true; this.currentButtonMask |= (int)MouseButtonMask.Left;
                        }
                        if ( ((msg == NativeMethods.WindowMessage.MiddleButtonDown) ||
                              (msg == NativeMethods.WindowMessage.MiddleButtonDoubleClick) )
                             && this.dragRectangle.Contains(cursor) )
                        {
                            this.isMouseMButtonDown = true; this.currentButtonMask |= (int)MouseButtonMask.Middle;
                        }
                        if ( ((msg == NativeMethods.WindowMessage.RightButtonDown) ||
                              (msg == NativeMethods.WindowMessage.RightButtonDoubleClick) )
                             && this.dragRectangle.Contains(cursor) )
                        {
                            this.isMouseRButtonDown = true; this.currentButtonMask |= (int)MouseButtonMask.Right;
                        }

                        // Capture the mouse, so if the mouse button is 
                        // released outside the window, we'll get the button up messages
                        NativeMethods.SetCapture(hWnd);

                        this.lastMousePosition = System.Windows.Forms.Cursor.Position;
                        return true;
                    }
                case NativeMethods.WindowMessage.LeftButtonUp:
                case NativeMethods.WindowMessage.RightButtonUp:
                case NativeMethods.WindowMessage.MiddleButtonUp:
                    {
                        // Update member var state
                        if (msg == NativeMethods.WindowMessage.LeftButtonUp) { this.isMouseLButtonDown = false; this.currentButtonMask &= ~(int)MouseButtonMask.Left; }
                        if (msg == NativeMethods.WindowMessage.RightButtonUp) { this.isMouseRButtonDown = false; this.currentButtonMask &= ~(int)MouseButtonMask.Right; }
                        if (msg == NativeMethods.WindowMessage.MiddleButtonUp) { this.isMouseMButtonDown = false; this.currentButtonMask &= ~(int)MouseButtonMask.Middle; }

                        // Release the capture if no mouse buttons are down
                        if (!this.isMouseLButtonDown && !this.isMouseMButtonDown && !this.isMouseRButtonDown)
                        {
                            NativeMethods.ReleaseCapture();
                        }
                    }
                    break;

                    // Handle the mouse wheel
                case NativeMethods.WindowMessage.MouseWheel:
                    this.mouseWheelDelta = NativeMethods.HiWord((uint)wParam.ToInt32()) / 120;
                    break;
            }

            return false;
        }


        /// <summary>
        /// Reset the camera's position back to the default
        /// </summary>
        public virtual void Reset()
        {
            this.SetViewParameters(this.defaultEye, this.defaultLookAt);
        }

        /// <summary>
        /// Client can call this to change the position and direction of camera
        /// </summary>
        public unsafe virtual void SetViewParameters(Vector3 eyePt, Vector3 lookAtPt)
        {
            // Store the data
            this.defaultEye = this.eye = eyePt;
            this.defaultLookAt = this.lookAt = lookAtPt;

            // Calculate the view matrix
            this.viewMatrix = Matrix.LookAtLH(this.eye, this.lookAt, UpDirection);

            // Get the inverted matrix
            Matrix inverseView = Matrix.Invert(this.viewMatrix);

            // The axis basis vectors and camera position are stored inside the 
            // position matrix in the 4 rows of the camera's world matrix.
            // To figure out the yaw/pitch of the camera, we just need the Z basis vector
            Vector3* pZBasis = (Vector3*)&inverseView.M31;
            this.cameraYawAngle = (float)Math.Atan2(pZBasis->X, pZBasis->Z);
            float len = (float)Math.Sqrt(pZBasis->Z * pZBasis->Z + pZBasis->X * pZBasis->X);
            this.cameraPitchAngle = -(float)Math.Atan2(pZBasis->Y, len);
        }

        /// <summary>
        /// Calculates the projection matrix based on input params
        /// </summary>
        public virtual void SetProjectionParameters(float fov, float aspect, float near, float far)
        {
            // Set attributes for the projection matrix
            this.fieldOfView = fov;
            this.aspectRatio = aspect;
            this.nearPlane = near;
            this.farPlane = far;

            this.projMatrix = Matrix.PerspectiveFovLH(fov, aspect, near, far);
        }

        /// <summary>
        /// Figure out the mouse delta based on mouse movement
        /// </summary>
        protected void UpdateMouseDelta(float elapsedTime)
        {
            // Get the current mouse position
            System.Drawing.Point current = System.Windows.Forms.Cursor.Position;

            // Calculate how far it's moved since the last frame
            System.Drawing.Point delta = new System.Drawing.Point(current.X - this.lastMousePosition.X,
                current.Y - this.lastMousePosition.Y);

            // Record the current position for next time
            this.lastMousePosition = current;

            if (this.isResetCursorAfterMove)
            {
                // Set position of camera to center of desktop, 
                // so it always has room to move.  This is very useful
                // if the cursor is hidden.  If this isn't done and cursor is hidden, 
                // then invisible cursor will hit the edge of the screen 
                // and the user can't tell what happened
                System.Windows.Forms.Screen activeScreen = System.Windows.Forms.Screen.PrimaryScreen;
                System.Drawing.Point center = new System.Drawing.Point(activeScreen.Bounds.Width / 2,
                    activeScreen.Bounds.Height / 2);
                System.Windows.Forms.Cursor.Position = center;
                this.lastMousePosition = center;
            }

            // Smooth the relative mouse data over a few frames so it isn't 
            // jerky when moving slowly at low frame rates.
            float percentOfNew = 1.0f / this.framesToSmoothMouseData;
            float percentOfOld = 1.0f - percentOfNew;
            this.mouseDelta.X = this.mouseDelta.X*percentOfNew + delta.X*percentOfNew;
            this.mouseDelta.Y = this.mouseDelta.Y*percentOfNew + delta.Y*percentOfNew;

            this.rotationVelocity = this.mouseDelta * this.rotationScaler;
        }

        /// <summary>
        /// Figure out the velocity based on keyboard input & drag if any
        /// </summary>
        protected void UpdateVelocity(float elapsedTime)
        {
            Vector3 accel = Vector3.Empty;

            if (this.isEnablePositionMovement)
            {
                // Update acceleration vector based on keyboard state
                if (this.keys[(int)CameraKeys.MoveForward])
                    accel.Z += 1.0f;
                if (this.keys[(int)CameraKeys.MoveBackward])
                    accel.Z -= 1.0f;
                if (this.isEnableYAxisMovement)
                {
                    if (this.keys[(int)CameraKeys.MoveUp])
                        accel.Y += 1.0f;
                    if (this.keys[(int)CameraKeys.MoveDown])
                        accel.Y -= 1.0f;
                }
                if (this.keys[(int)CameraKeys.StrafeRight])
                    accel.X += 1.0f;
                if (this.keys[(int)CameraKeys.StrafeLeft])
                    accel.X -= 1.0f;
            }
            // Normalize vector so if moving 2 dirs (left & forward), 
            // the camera doesn't move faster than if moving in 1 dir
            accel.Normalize();
            // Scale the acceleration vector
            accel *= this.moveScaler;

            if (this.isMovementDrag)
            {
                // Is there any acceleration this frame?
                if (accel.LengthSq() > 0)
                {
                    // If so, then this means the user has pressed a movement key
                    // so change the velocity immediately to acceleration 
                    // upon keyboard input.  This isn't normal physics
                    // but it will give a quick response to keyboard input
                    this.velocity = accel;
                    this.dragTimer = this.totalDragTimeToZero;
                    this.velocityDrag = accel * (1 / this.dragTimer);
                }
                else
                {
                    // If no key being pressed, then slowly decrease velocity to 0
                    if (this.dragTimer > 0)
                    {
                        this.velocity -= (this.velocityDrag * elapsedTime);
                        this.dragTimer -= elapsedTime;
                    }
                    else
                    {
                        // Zero velocity
                        this.velocity = Vector3.Empty;
                    }
                }
            }
            else
            {
                // No drag, so immediately change the velocity
                this.velocity = accel;
            }
        }

        /// <summary>
        /// Clamps V to lie inside boundaries
        /// </summary>
        protected void ConstrainToBoundary(ref Vector3 v)
        {
            // Constrain vector to a bounding box 
            v.X = Math.Max(v.X, this.minBoundary.X);
            v.Y = Math.Max(v.Y, this.minBoundary.Y);
            v.Z = Math.Max(v.Z, this.minBoundary.Z);

            v.X = Math.Min(v.X, this.maxBoundary.X);
            v.Y = Math.Min(v.Y, this.maxBoundary.Y);
            v.Z = Math.Min(v.Z, this.maxBoundary.Z);

        }
    }
}
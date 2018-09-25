namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    using Microsoft.DirectX;

    /// <summary>
    /// Simple model viewing camera class that rotates around the object.
    /// </summary>
    public class ModelViewerCamera : Camera
    {
        #region Instance Data
        protected ArcBall worldArcball = new ArcBall();
        protected ArcBall viewArcball = new ArcBall();
        protected Vector3 modelCenter;
        protected Matrix lastModelRotation; // Last arcball rotation matrix for model 
        protected Matrix lastCameraRotation; // Last rotation matrix for camera
        protected Matrix modelRotation; // Rotation matrix of model
        protected Matrix world; // World Matrix of model

        protected int rotateModelButtonMask;
        protected int zoomButtonMask;
        protected int rotateCameraButtonMask;

        protected bool isPitchLimited;
        protected float radius; // Distance from the camera to model 
        protected float defaultRadius; // Distance from the camera to model 
        protected float minRadius; // Min radius
        protected float maxRadius; // Max radius
        protected bool attachCameraToModel;
        #endregion 

        #region Simple Properties/Set Methods
        /// <summary>The minimum radius</summary>
        public float MinimumRadius { get { return this.minRadius; } set { this.minRadius = value; } }
        /// <summary>The maximum radius</summary>
        public float MaximumRadius { get { return this.maxRadius; } set { this.maxRadius = value; } }
        /// <summary>Gets the world matrix</summary>
        public Matrix WorldMatrix { get { return this.world; } }
        /// <summary>Sets the world quat</summary>
        public void SetWorldQuat(Quaternion q) { this.worldArcball.CurrentQuaternion = q; }
        /// <summary>Sets the view quat</summary>
        public void SetViewQuat(Quaternion q) { this.viewArcball.CurrentQuaternion = q; }
        /// <summary>Sets whether the pitch is limited or not</summary>
        public void SetIsPitchLimited(bool limit) { this.isPitchLimited = limit; }
        /// <summary>Sets the model's center</summary>
        public void SetModelCenter(Vector3 c) { this.modelCenter = c; }
        /// <summary>Sets radius</summary>
        public void SetRadius(float r, float min, float max) { this.radius = this.defaultRadius = r; this.minRadius = min; this.maxRadius = max;}
        /// <summary>Sets radius</summary>
        public void SetRadius(float r) { this.defaultRadius = r; this.minRadius = 1.0f; this.maxRadius = float.MaxValue;}
        /// <summary>Sets arcball window</summary>
        public void SetWindow(int w, int h, float r) { this.worldArcball.SetWindow(w,h,r); this.viewArcball.SetWindow(w,h,r); }
        /// <summary>Sets arcball window</summary>
        public void SetWindow(int w, int h) { this.worldArcball.SetWindow(w,h,0.9f); this.viewArcball.SetWindow(w,h,0.9f); }
        /// <summary>Sets button masks</summary>
        public void SetButtonMasks(int rotateModel, int zoom, int rotateCamera) { this.rotateCameraButtonMask = rotateCamera; this.zoomButtonMask = zoom; this.rotateModelButtonMask = rotateModel; }
        /// <summary>Is the camera attached to a model</summary>
        public bool IsAttachedToModel { get { return this.attachCameraToModel; } set { this.attachCameraToModel = value; } }
        #endregion

        /// <summary>
        /// Creates new instance of the model viewer camera
        /// </summary>
        public ModelViewerCamera()
        {
            this.world = Matrix.Identity;
            this.modelRotation = Matrix.Identity;
            this.lastModelRotation = Matrix.Identity;
            this.lastCameraRotation = Matrix.Identity;
            this.modelCenter = Vector3.Empty;
            this.radius = 5.0f;
            this.defaultRadius = 5.0f;
            this.minRadius = 1.0f;
            this.maxRadius = float.MaxValue;
            this.isPitchLimited = false;
            this.isEnablePositionMovement = false;
            this.attachCameraToModel = false;

            // Set button masks
            this.rotateModelButtonMask = (int)MouseButtonMask.Left;
            this.zoomButtonMask = (int)MouseButtonMask.Wheel;
            this.rotateCameraButtonMask = (int)MouseButtonMask.Right;
        }

        /// <summary>
        /// Update the view matrix based on user input & elapsed time
        /// </summary>
        public unsafe override void FrameMove(float elapsedTime)
        {
            // Reset the camera if necessary
            if (this.keys[(int)CameraKeys.Reset])
                this.Reset();

            // Get the mouse movement (if any) if the mouse buttons are down
            if (this.currentButtonMask != 0)
                this.UpdateMouseDelta(elapsedTime);

            // Get amount of velocity based on the keyboard input and drag (if any)
            this.UpdateVelocity(elapsedTime);

            // Simple euler method to calculate position delta
            Vector3 posDelta = this.velocity * elapsedTime;

            // Change the radius from the camera to the model based on wheel scrolling
            if ( (this.mouseWheelDelta != 0) && (this.zoomButtonMask == (int)MouseButtonMask.Wheel) )
                this.radius -= this.mouseWheelDelta * this.radius * 0.1f;
            this.radius = Math.Min( this.maxRadius, this.radius );
            this.radius = Math.Max( this.minRadius, this.radius );
            this.mouseWheelDelta = 0;

            // Get the inverse of the arcball's rotation matrix
            Matrix cameraRotation = Matrix.Invert(this.viewArcball.RotationMatrix);
        
            // Transform vectors based on camera's rotation matrix
            Vector3 localUp = new Vector3(0,1,0);
            Vector3 localAhead = new Vector3(0,0,1);
            Vector3 worldUp = Vector3.TransformCoordinate(localUp, cameraRotation);
            Vector3 worldAhead = Vector3.TransformCoordinate(localAhead, cameraRotation);

            // Transform the position delta by the camera's rotation 
            Vector3 posDeltaWorld = Vector3.TransformCoordinate(posDelta, cameraRotation);

            // Move the lookAt position 
            this.lookAt += posDeltaWorld;
            if (this.isClipToBoundary)
                this.ConstrainToBoundary(ref this.lookAt);

            // Update the eye point based on a radius away from the lookAt position
            this.eye = this.lookAt - worldAhead * this.radius;

            // Update the view matrix
            this.viewMatrix = Matrix.LookAtLH(this.eye, this.lookAt, worldUp);
            Matrix invView = Matrix.Invert(this.viewMatrix);
            invView.M41 = invView.M42 = invView.M43 = 0;
            Matrix modelLastRotInv = Matrix.Invert(this.lastModelRotation);

            // Accumulate the delta of the arcball's rotation in view space.
            // Note that per-frame delta rotations could be problematic over long periods of time.
            Matrix localModel = this.worldArcball.RotationMatrix;
            this.modelRotation *= this.viewMatrix * modelLastRotInv * localModel * invView;
            if ( this.viewArcball.IsBeingDragged && this.attachCameraToModel && !this.keys[(int)CameraKeys.ControlDown])
            {
                // Attah camera to model by inverse of the model rotation
                Matrix cameraRotInv = Matrix.Invert(this.lastCameraRotation);
                Matrix delta = cameraRotInv * cameraRotation; // local to world matrix
                this.modelRotation *= delta;
            }
            this.lastCameraRotation = cameraRotation;
            this.lastModelRotation = localModel;

            // Since we're accumulating delta rotations, we need to orthonormalize 
            // the matrix to prevent eventual matrix skew
            fixed(void* pxBasis = &this.modelRotation.M11)
            {
                fixed(void* pyBasis = &this.modelRotation.M21)
                {
                    fixed(void* pzBasis = &this.modelRotation.M31)
                    {
                        UnsafeNativeMethods.Vector3.Normalize((Vector3*)pxBasis, (Vector3*)pxBasis);
                        UnsafeNativeMethods.Vector3.Cross((Vector3*)pyBasis, (Vector3*)pzBasis, (Vector3*)pxBasis);
                        UnsafeNativeMethods.Vector3.Normalize((Vector3*)pyBasis, (Vector3*)pyBasis);
                        UnsafeNativeMethods.Vector3.Cross((Vector3*)pzBasis, (Vector3*)pxBasis, (Vector3*)pyBasis);
                    }
                }
            }

            // Translate the rotation matrix to the same position as the lookAt position
            this.modelRotation.M41 = this.lookAt.X;
            this.modelRotation.M42 = this.lookAt.Y;
            this.modelRotation.M43 = this.lookAt.Z;

            // Translate world matrix so its at the center of the model
            Matrix trans = Matrix.Translation(-this.modelCenter.X, -this.modelCenter.Y, -this.modelCenter.Z);
            this.world = trans * this.modelRotation;
        }

        /// <summary>
        /// Reset the camera's position back to the default
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this.world = Matrix.Identity;
            this.modelRotation = Matrix.Identity;
            this.lastModelRotation = Matrix.Identity;
            this.lastCameraRotation = Matrix.Identity;
            this.radius = this.defaultRadius;
            this.worldArcball.Reset();
            this.viewArcball.Reset();
        }

        /// <summary>
        /// Override for setting the view parameters
        /// </summary>
        public override void SetViewParameters(Vector3 eyePt, Vector3 lookAtPt)
        {
            // Call base first
            base.SetViewParameters (eyePt, lookAtPt);

            // Propogate changes to the member arcball
            Matrix rotation = Matrix.LookAtLH(eyePt, lookAtPt, UpDirection);
            this.viewArcball.CurrentQuaternion = Quaternion.RotationMatrix(rotation);

            // Set the radius according to the distance
            Vector3 eyeToPoint = lookAtPt - eyePt;
            this.SetRadius(eyeToPoint.Length());
        }

        /// <summary>
        /// Call this from your message proc so this class can handle window messages
        /// </summary>
        public override bool HandleMessages(IntPtr hWnd, NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam)
        {
            // Call base first
            base.HandleMessages(hWnd, msg, wParam, lParam);

            if ( ( (msg == NativeMethods.WindowMessage.LeftButtonDown || msg == NativeMethods.WindowMessage.LeftButtonDoubleClick) && ((this.rotateModelButtonMask & (int)MouseButtonMask.Left) != 0 ) ) ||
                 ( (msg == NativeMethods.WindowMessage.RightButtonDown || msg == NativeMethods.WindowMessage.RightButtonDoubleClick) && ((this.rotateModelButtonMask & (int)MouseButtonMask.Right) != 0 ) ) ||
                 ( (msg == NativeMethods.WindowMessage.MiddleButtonDown || msg == NativeMethods.WindowMessage.MiddleButtonDoubleClick) && ((this.rotateModelButtonMask & (int)MouseButtonMask.Middle) != 0 ) ) )
            {
                // Current mouse position
                short mouseX = NativeMethods.LoWord((uint)lParam.ToInt32());
                short mouseY = NativeMethods.HiWord((uint)lParam.ToInt32());
                this.worldArcball.OnBegin(mouseX, mouseY);
            }
            if ( ( (msg == NativeMethods.WindowMessage.LeftButtonDown || msg == NativeMethods.WindowMessage.LeftButtonDoubleClick) && ((this.rotateCameraButtonMask & (int)MouseButtonMask.Left) != 0 ) ) ||
                 ( (msg == NativeMethods.WindowMessage.RightButtonDown || msg == NativeMethods.WindowMessage.RightButtonDoubleClick) && ((this.rotateCameraButtonMask & (int)MouseButtonMask.Right) != 0 ) ) ||
                 ( (msg == NativeMethods.WindowMessage.MiddleButtonDown || msg == NativeMethods.WindowMessage.MiddleButtonDoubleClick) && ((this.rotateCameraButtonMask & (int)MouseButtonMask.Middle) != 0 ) ) )
            {
                // Current mouse position
                short mouseX = NativeMethods.LoWord((uint)lParam.ToInt32());
                short mouseY = NativeMethods.HiWord((uint)lParam.ToInt32());
                this.viewArcball.OnBegin(mouseX, mouseY);
            }
            if (msg == NativeMethods.WindowMessage.MouseMove)
            {
                // Current mouse position
                short mouseX = NativeMethods.LoWord((uint)lParam.ToInt32());
                short mouseY = NativeMethods.HiWord((uint)lParam.ToInt32());
                this.worldArcball.OnMove(mouseX, mouseY);
                this.viewArcball.OnMove(mouseX, mouseY);
            }

            if ( (msg == NativeMethods.WindowMessage.LeftButtonUp) && ((this.rotateModelButtonMask & (int)MouseButtonMask.Left) != 0 ) ||
                 (msg == NativeMethods.WindowMessage.RightButtonUp) && ((this.rotateModelButtonMask & (int)MouseButtonMask.Right) != 0 ) ||
                 (msg == NativeMethods.WindowMessage.MiddleButtonUp) && ((this.rotateModelButtonMask & (int)MouseButtonMask.Middle) != 0 ) )
            {
                this.worldArcball.OnEnd();
            }

            if ( (msg == NativeMethods.WindowMessage.LeftButtonUp) && ((this.rotateCameraButtonMask & (int)MouseButtonMask.Left) != 0 ) ||
                 (msg == NativeMethods.WindowMessage.RightButtonUp) && ((this.rotateCameraButtonMask & (int)MouseButtonMask.Right) != 0 ) ||
                 (msg == NativeMethods.WindowMessage.MiddleButtonUp) && ((this.rotateCameraButtonMask & (int)MouseButtonMask.Middle) != 0 ) )
            {
                this.viewArcball.OnEnd();
            }

            return false;
        }
    }
}
namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    using Microsoft.DirectX;

    /// <summary>
    /// Simple first person camera class that moves and rotates.
    /// It allows yaw and pitch but not roll.  It uses keyboard and 
    /// cursor to respond to keyboard and mouse input and updates the 
    /// view matrix based on input.  
    /// </summary>
    public class FirstPersonCamera : Camera
    {    
        // Mask to determine which button to enable for rotation
        protected int activeButtonMask = (int)(MouseButtonMask.Left | MouseButtonMask.Middle | MouseButtonMask.Right);
        // World matrix of the camera (inverse of the view matrix)
        protected Matrix cameraWorld;

        /// <summary>
        /// Update the view matrix based on user input & elapsed time
        /// </summary>
        public override void FrameMove(float elapsedTime)
        {
            // Reset the camera if necessary
            if (this.keys[(int)CameraKeys.Reset])
                this.Reset();

            // Get the mouse movement (if any) if the mouse buttons are down
            if ((this.activeButtonMask & this.currentButtonMask) != 0)
                this.UpdateMouseDelta(elapsedTime);

            // Get amount of velocity based on the keyboard input and drag (if any)
            this.UpdateVelocity(elapsedTime);

            // Simple euler method to calculate position delta
            Vector3 posDelta = this.velocity * elapsedTime;

            // If rotating the camera 
            if ((this.activeButtonMask & this.currentButtonMask) != 0)
            {
                // Update the pitch & yaw angle based on mouse movement
                float yawDelta   = this.rotationVelocity.X;
                float pitchDelta = this.rotationVelocity.Y;

                // Invert pitch if requested
                if (this.isInvertPitch)
                    pitchDelta = -pitchDelta;

                this.cameraPitchAngle += pitchDelta;
                this.cameraYawAngle   += yawDelta;

                // Limit pitch to straight up or straight down
                this.cameraPitchAngle = Math.Max(-(float)Math.PI/2.0f,  this.cameraPitchAngle);
                this.cameraPitchAngle = Math.Min(+(float)Math.PI/2.0f,  this.cameraPitchAngle);
            }

            // Make a rotation matrix based on the camera's yaw & pitch
            Matrix cameraRotation = Matrix.RotationYawPitchRoll(this.cameraYawAngle, this.cameraPitchAngle, 0 );

            // Transform vectors based on camera's rotation matrix
            Vector3 localUp = new Vector3(0,1,0);
            Vector3 localAhead = new Vector3(0,0,1);
            Vector3 worldUp = Vector3.TransformCoordinate(localUp, cameraRotation);
            Vector3 worldAhead = Vector3.TransformCoordinate(localAhead, cameraRotation);

            // Transform the position delta by the camera's rotation 
            Vector3 posDeltaWorld = Vector3.TransformCoordinate(posDelta, cameraRotation);
            if (!this.isEnableYAxisMovement)
                posDeltaWorld.Y = 0.0f;

            // Move the eye position 
            this.eye += posDeltaWorld;
            if (this.isClipToBoundary)
                this.ConstrainToBoundary(ref this.eye);

            // Update the lookAt position based on the eye position 
            this.lookAt = this.eye + worldAhead;

            // Update the view matrix
            this.viewMatrix = Matrix.LookAtLH(this.eye, this.lookAt, worldUp);
            this.cameraWorld = Matrix.Invert(this.viewMatrix);
        }

        /// <summary>
        /// Enable or disable each of the mouse buttons for rotation drag.
        /// </summary>
        public void SetRotationButtons(bool left, bool middle, bool right)
        {
            this.activeButtonMask = (left ? (int)MouseButtonMask.Left : 0) |
                               (middle ? (int)MouseButtonMask.Middle : 0) |
                               (right ? (int)MouseButtonMask.Right : 0);
        }
    }
}
namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;

    /// <summary>Widget for controlling direction</summary>
    public class DirectionWidget
    {
        #region Class level data (Instance/Static)
        // Instance members
        private float widgetRadius = 1.0f;
        private ArcBall arc = new ArcBall();
        private Vector3 defaultDir = new Vector3(0,1,0);
        private Vector3 currentDir = new Vector3(0,1,0);
        private Matrix viewMatrix = Matrix.Identity;
        private Matrix rotation = Matrix.Identity;
        private Matrix rotationSnapshot = Matrix.Identity;
        private MouseButtonMask rotateMask = MouseButtonMask.Right;

        // Static members
        private static Device device = null;
        private static Effect effect = null;
        private static Mesh mesh = null;
        #endregion

        #region Properties
        /// <summary>Radius of this widget</summary>
        public float Radius { get { return this.widgetRadius; } set { this.widgetRadius = value; } }
        /// <summary>Light direction of this widget</summary>
        public Vector3 LightDirection { get { return this.currentDir; } set { this.currentDir = this.defaultDir = value; } }
        /// <summary>Is this widget being dragged</summary>
        public bool IsBeingDragged{ get { return this.arc.IsBeingDragged; } }
        /// <summary>Rotation button mask</summary>
        public MouseButtonMask RotateButtonMask{ get { return this.rotateMask; } set { this.rotateMask = value; }}
        #endregion

        #region Device handlers
        /// <summary>Called when the device has been created</summary>
        public static void OnCreateDevice(Device device)
        {
            // Store the device
            DirectionWidget.device = device;

            // Read the effect file
            string path = Utility.FindMediaFile("UI\\DXUTShared.fx");

            // If this fails, there should be debug output as to 
            // why the .fx file failed to compile (assuming you have dbmon running).
            // If you do not, you can turn on unmanaged debugging for this project.
            effect = Effect.FromFile(device, path, null, null, ShaderFlags.NotCloneable, null);

            // Load the mesh with D3DX and get back a Mesh.  For this
            // sample we'll ignore the X file's embedded materials since we know 
            // exactly the model we're loading.  See the mesh samples such as
            // "OptimizedMesh" for a more generic mesh loading example.
            path = Utility.FindMediaFile("UI\\arrow.x");
            mesh = Mesh.FromFile(path, MeshFlags.Managed, device);

            // Optimize the mesh for this graphics card's vertex cache 
            // so when rendering the mesh's triangle list the vertices will 
            // cache hit more often so it won't have to re-execute the vertex shader 
            // on those vertices so it will improve perf.     
            int[] adj = new int[mesh.NumberFaces * 3];
            mesh.GenerateAdjacency(1e-6f, adj);
            mesh.OptimizeInPlace(MeshFlags.OptimizeVertexCache, adj);
        }

        /// <summary>Called when the device has been reset</summary>
        public void OnResetDevice(SurfaceDescription desc)
        {
            this.arc.SetWindow(desc.Width, desc.Height);
        }
        
        /// <summary>Called when the device has been lost</summary>
        public static void OnLostDevice()
        {
            if (effect != null)
                effect.OnLostDevice();
        }
        
        /// <summary>Called when the device has been destroyed</summary>
        public static void OnDestroyDevice()
        {
            if (effect != null)
                effect.Dispose();
            if (mesh != null)
                mesh.Dispose();
            effect = null;
            mesh = null;
        }
        #endregion
        
        /// <summary>Handle messages from the window</summary>
        public bool HandleMessages(IntPtr hWnd, NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam)
        {
            // Current mouse position
            short mouseX = NativeMethods.LoWord((uint)lParam.ToInt32());
            short mouseY = NativeMethods.HiWord((uint)lParam.ToInt32());

            switch(msg)
            {
                case NativeMethods.WindowMessage.LeftButtonDown:
                case NativeMethods.WindowMessage.MiddleButtonDown:
                case NativeMethods.WindowMessage.RightButtonDown:
                    {
                        if ( ((this.rotateMask & MouseButtonMask.Left) == MouseButtonMask.Left && msg == NativeMethods.WindowMessage.LeftButtonDown) ||
                             ((this.rotateMask & MouseButtonMask.Right) == MouseButtonMask.Right && msg == NativeMethods.WindowMessage.RightButtonDown) ||
                             ((this.rotateMask & MouseButtonMask.Middle) == MouseButtonMask.Middle && msg == NativeMethods.WindowMessage.MiddleButtonDown) )
                        {
                            this.arc.OnBegin(mouseX, mouseY);
                            NativeMethods.SetCapture(hWnd);
                        }
                        return true;
                    }
                case NativeMethods.WindowMessage.MouseMove:
                    {
                        if (this.arc.IsBeingDragged)
                        {
                            this.arc.OnMove(mouseX, mouseY);
                            this.UpdateLightDirection();
                        }
                        return true;
                    }
                case NativeMethods.WindowMessage.LeftButtonUp:
                case NativeMethods.WindowMessage.RightButtonUp:
                case NativeMethods.WindowMessage.MiddleButtonUp:
                    {
                        if ( ((this.rotateMask & MouseButtonMask.Left) == MouseButtonMask.Left && msg == NativeMethods.WindowMessage.LeftButtonUp) ||
                             ((this.rotateMask & MouseButtonMask.Right) == MouseButtonMask.Right && msg == NativeMethods.WindowMessage.RightButtonUp) ||
                             ((this.rotateMask & MouseButtonMask.Middle) == MouseButtonMask.Middle && msg == NativeMethods.WindowMessage.MiddleButtonUp) )
                        {
                            this.arc.OnEnd();
                            NativeMethods.ReleaseCapture();
                        }
                    
                        this.UpdateLightDirection();
                        return true;
                    }
            }

            // Didn't handle the message
            return false;
        }

        /// <summary>Updates the light direction</summary>
        private unsafe void UpdateLightDirection()
        {
            Matrix invView = Matrix.Invert(this.viewMatrix);
            invView.M41 = invView.M42 = invView.M43 = 0;

            Matrix lastRotationInv = Matrix.Invert(this.rotationSnapshot);
            Matrix rot = this.arc.RotationMatrix;
            this.rotationSnapshot = rot;

            // Accumulate the delta of the arcball's rotation in view space.
            // Note that per-frame delta rotations could be problematic over long periods of time.
            this.rotation *= (this.viewMatrix * lastRotationInv * rot * invView);

            // Since we're accumulating delta rotations, we need to orthonormalize 
            // the matrix to prevent eventual matrix skew
            fixed(void* pxBasis = &this.rotation.M11)
            {
                fixed(void* pyBasis = &this.rotation.M21)
                {
                    fixed(void* pzBasis = &this.rotation.M31)
                    {
                        UnsafeNativeMethods.Vector3.Normalize((Vector3*)pxBasis, (Vector3*)pxBasis);
                        UnsafeNativeMethods.Vector3.Cross((Vector3*)pyBasis, (Vector3*)pzBasis, (Vector3*)pxBasis);
                        UnsafeNativeMethods.Vector3.Normalize((Vector3*)pyBasis, (Vector3*)pyBasis);
                        UnsafeNativeMethods.Vector3.Cross((Vector3*)pzBasis, (Vector3*)pxBasis, (Vector3*)pyBasis);
                    }
                }
            }
     
            // Transform the default direction vector by the light's rotation matrix
            this.currentDir = Vector3.TransformNormal(this.defaultDir, this.rotation);
        }

        /// <summary>Render the light widget</summary>
        public unsafe void OnRender(ColorValue color, Matrix view, Matrix proj, Vector3 eye)
        {
            // Store the view matrix
            this.viewMatrix = view;

            // Render the light arrows so the user can visually see the light direction
            effect.Technique = "RenderWith1LightNoTexture";
            effect.SetValue("g_MaterialDiffuseColor", color);
            Vector3 eyePt = Vector3.Normalize(eye);

            // Set the light direction value
            effect.SetValue("g_LightDir", &eyePt, sizeof(Vector3));
            
            // Rotate arrow model to point towards origin
            Vector3 at = Vector3.Empty;
            Vector3 up = new Vector3(0,1,0);
            Matrix rotateB = Matrix.RotationX((float)Math.PI);
            Matrix rotateA = Matrix.LookAtLH(this.currentDir, at, up);
            rotateA.Invert();
            Matrix rotate = rotateB * rotateA;
            Vector3 l = this.currentDir * this.widgetRadius * 1.0f;
            Matrix trans = Matrix.Translation(l);
            Matrix scale = Matrix.Scaling(this.widgetRadius * 0.2f, this.widgetRadius * 0.2f, this.widgetRadius * 0.2f);

            Matrix world = rotate * scale * trans;
            Matrix worldViewProj = world * this.viewMatrix * proj;

            effect.SetValue("g_mWorldViewProjection", worldViewProj);
            effect.SetValue("g_mWorld", world);

            // Render the arrows
            for (int subset = 0; subset < 2; subset++)
            {
                int passes = effect.Begin(0);
                for (int pass = 0; pass < passes; pass++)
                {
                    effect.BeginPass(pass);
                    mesh.DrawSubset(subset);
                    effect.EndPass();
                }
                effect.End();
            }

        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusMain.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StatusMain type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using System;

    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The status main field.
    /// </summary>
    public partial class StatusMain : MainField, IStatusMainField
    {
        private IDriveInfo driveInfo;

        private IGpsInfo gpsInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusMain"/> class.
        /// </summary>
        public StatusMain()
        {
            this.InitializeComponent();

            this.UpdateLanguage();
            LanguageManager.Instance.CurrentLanguageChanged += this.CurrentLanguageChanged;
        }

        /// <summary>
        /// Make sure that calling code is running in the GUI thread!!!
        ///   Check with GetControl().InvokeRequired
        /// </summary>
        /// <param name="visible">
        /// A flag indicating if this field should be made visible.
        /// </param>
        public override void MakeVisible(bool visible)
        {
            if (visible)
            {
                this.UpdateDriveInfo(true);
                this.UpdateGpsInfo(true);
            }

            base.MakeVisible(visible);
        }

        /// <summary>
        /// Updates this field with the given drive information.
        /// </summary>
        /// <param name="info">
        /// The drive information.
        /// </param>
        public void Update(IDriveInfo info)
        {
            this.driveInfo = info;
            this.UpdateDriveInfo(false);
        }

        /// <summary>
        /// Updates this field with the given GPS information.
        /// </summary>
        /// <param name="info">
        /// The GPS information.
        /// </param>
        public void Update(IGpsInfo info)
        {
            this.gpsInfo = info;
            this.UpdateGpsInfo(false);
        }

        private void CurrentLanguageChanged(object sender, EventArgs e)
        {
            this.UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            this.label1.Text = ml.ml_string(68, "GPS:");
            this.label3.Text = ml.ml_string(70, "Direction:");
            this.lblCaption.Text = ml.ml_string(52, "Informations");
            this.lblDescrB.Text = ml.ml_string(53, "Block:");
            this.lblDescrD.Text = ml.ml_string(57, "Destination:");
            this.lblDescrDr.Text = ml.ml_string(59, "Driver:");
            this.lblDescrGPSValid.Text = ml.ml_string(60, "Valid:");
            this.lblDescrRo.Text = ml.ml_string(54, "Line:");
            this.lblDescrRp.Text = ml.ml_string(56, "Route Path:");
            this.lblDescrRu.Text = ml.ml_string(55, "Run:");
            this.lblDescrXCoordinate.Text = ml.ml_string(61, "x-Coord:");
            this.lblDescrYCoordinate.Text = ml.ml_string(62, "y-Coord:");
            this.lblDescrZ.Text = ml.ml_string(58, "Zone:");
        }

        private void UpdateDriveInfo(bool force)
        {
            if (!this.IsActive && !force)
            {
                return;
            }

            this.SafeBeginInvoke(
                () =>
                    {
                        if (this.driveInfo != null)
                        {
                            this.lblB.Text = this.driveInfo.SBlockNumber;
                            this.lblD.Text = this.driveInfo.SDestinationNumber;
                            this.lblDr.Text = this.driveInfo.SDriverNumber;
                            this.lblL.Text = this.driveInfo.SLineName;
                            this.lblRp.Text = this.driveInfo.SRoutePathNumber;
                            this.lblRu.Text = this.driveInfo.SRunNumber;
                            this.lblZ.Text = this.driveInfo.SZoneNumber;
                        }
                    });
        }

        private void UpdateGpsInfo(bool force)
        {
            if (!this.IsActive && !force)
            {
                return;
            }

            this.SafeBeginInvoke(
                () =>
                    {
                        if (this.gpsInfo != null)
                        {
                            this.lblGPSValid.Text = this.gpsInfo.SIsGpsValid;
                            this.lblXCoordinate.Text = this.gpsInfo.SXCoordinate;
                            this.lblYCoordinate.Text = this.gpsInfo.SYCoordinate;
                            this.lblGPSDirection.Text = this.gpsInfo.SDirection;
                        }
                    });
        }

        private void StatusMainClick(object sender, EventArgs e)
        {
            this.OnEscapePressed(e);
        }

        /*
      protected override void ml_UpdateControls()
      {
        base.ml_UpdateControls() ;
        System.ComponentModel.ComponentResourceManager resources =
          new System.ComponentModel.ComponentResourceManager ( typeof ( StatusMain ) );
        resources.ApplyResources(this.label1, "label1") ;
        resources.ApplyResources(this.label3, "label3") ;
        resources.ApplyResources(this.lblCaption, "lblCaption") ;
        resources.ApplyResources(this.lblDescrB, "lblDescrB") ;
        resources.ApplyResources(this.lblDescrD, "lblDescrD") ;
        resources.ApplyResources(this.lblDescrDr, "lblDescrDr") ;
        resources.ApplyResources(this.lblDescrGPSValid, "lblDescrGPSValid") ;
        resources.ApplyResources(this.lblDescrRo, "lblDescrRo") ;
        resources.ApplyResources(this.lblDescrRp, "lblDescrRp") ;
        resources.ApplyResources(this.lblDescrRu, "lblDescrRu") ;
        resources.ApplyResources(this.lblDescrXCoordinate, "lblDescrXCoordinate") ;
        resources.ApplyResources(this.lblDescrYCoordinate, "lblDescrYCoordinate") ;
        resources.ApplyResources(this.lblDescrZ, "lblDescrZ") ;
        resources.ApplyResources(this.lblGPSValid, "lblGPSValid") ;
      }*/
    }
}
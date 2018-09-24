/***************************************************************************

Copyright (c) Microsoft Corporation 2008.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.ComponentModel;

namespace OpenXmlPowerTools
{
    /// <summary>
    /// SnapIn containing OpenXml Power Tools Cmdlets
    /// </summary>
    [RunInstaller(true)]
    public class OpenXmlPowerToolsSnapIn : PSSnapIn
    {
        /// <summary>
        /// SnapIn name
        /// </summary>
        public override string Name
        {
            get
            {
                return "OpenXmlPowerTools";
            }
        }

        /// <summary>
        /// SnapIn vendor
        /// </summary>
        public override string Vendor
        {
            get
            {
                return "Staff DotNet";
            }
        }

        /// <summary>
        /// SnapIn vendor resource
        /// </summary>
        public override string VendorResource
        {
            get
            {
                return "OpenXml Power Tools, Staff DotNet";
            }
        }

        /// <summary>
        /// SnapIn description
        /// </summary>
        public override string Description
        {
            get
            {
                return "A set of cmdlets to manipulate OpenXml documents";
            }
        }

        /// <summary>
        /// SnapIn description resource
        /// </summary>
        public override string DescriptionResource
        {
            get
            {
                return "OpenXml Power Tools, A set of cmdlets to manipulate OpenXml documents";
            }
        }
    }
}

/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Management.Automation;

namespace OpenXmlPowerTools.Commands
{
    [Cmdlet(VerbsCommon.Remove, "OpenXmlMarkup", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class RemoveOpenXmlMarkupCmdlet : PowerToolsModifierCmdlet
    {
        private SimplifyMarkupSettings settings = new SimplifyMarkupSettings();

        #region Parameters
        [Parameter(Mandatory = false)]
        public SwitchParameter AcceptRevisions
        {
            get { return settings.AcceptRevisions; }
            set { settings.AcceptRevisions = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter RemoveContentControls
        {
            get { return settings.RemoveContentControls; }
            set { settings.RemoveContentControls = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter RemoveSmartTags
        {
            get { return settings.RemoveSmartTags; }
            set { settings.RemoveSmartTags = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter RemoveRsidInfo
        {
            get { return settings.RemoveRsidInfo; }
            set { settings.RemoveRsidInfo = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter RemoveComments
        {
            get { return settings.RemoveComments; }
            set { settings.RemoveComments = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter RemoveEndAndFootNotes
        {
            get { return settings.RemoveEndAndFootNotes; }
            set { settings.RemoveEndAndFootNotes = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter ReplaceTabsWithSpaces
        {
            get { return settings.ReplaceTabsWithSpaces; }
            set { settings.ReplaceTabsWithSpaces = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter RemoveFieldCodes
        {
            get { return settings.RemoveFieldCodes; }
            set { settings.RemoveFieldCodes = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter RemovePermissions
        {
            get { return settings.RemovePermissions; }
            set { settings.RemovePermissions = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter RemoveProof
        {
            get { return settings.RemoveProof; }
            set { settings.RemoveProof = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter RemoveSoftHyphens
        {
            get { return settings.RemoveSoftHyphens; }
            set { settings.RemoveSoftHyphens = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter RemoveLastRenderedPageBreak
        {
            get { return settings.RemoveLastRenderedPageBreak; }
            set { settings.RemoveLastRenderedPageBreak = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter RemoveBookmarks
        {
            get { return settings.RemoveBookmarks; }
            set { settings.RemoveBookmarks = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter RemoveWebHidden
        {
            get { return settings.RemoveWebHidden; }
            set { settings.RemoveWebHidden = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter RemoveGoBackBookmark
        {
            get { return settings.RemoveGoBackBookmark; }
            set { settings.RemoveGoBackBookmark = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter NormalizeXml
        {
            get { return settings.NormalizeXml; }
            set { settings.NormalizeXml = value; }
        }

        #endregion

        #region Cmdlet Overrides
        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Remove-OpenXmlMarkup"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    OutputDocument(MarkupSimplifier.SimplifyMarkup((WmlDocument)document, settings));
                }
                catch (Exception e)
                {
                    WriteError(PowerToolsExceptionHandling.GetExceptionErrorRecord(e, document));
                }
            }
        }
        #endregion
    }
}

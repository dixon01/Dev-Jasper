/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Management.Automation;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;

namespace OpenXmlPowerTools.Commands
{
    public class ValidationInfo
    {
        public OpenXmlPowerToolsDocument Document;
        public string FileName;
        public string Description;
        public ValidationErrorType ErrorType;
        public string Id;
        public OpenXmlElement Node;
        public OpenXmlPart Part;
        public string XPath;
        public OpenXmlElement RelatedNode;
        public OpenXmlPart RelatedPart;

        public ValidationInfo(OpenXmlPowerToolsDocument doc, ValidationErrorInfo err)
        {
            Document = doc;
            FileName = doc.FileName;
            Description = err.Description;
            ErrorType = err.ErrorType;
            Id = err.Id;
            Node = err.Node;
            Part = err.Part;
            XPath = err.Path.XPath;
            RelatedNode = err.RelatedNode;
            RelatedPart = err.RelatedPart;
        }
    }

    [Cmdlet(VerbsLifecycle.Confirm, "OpenXmlValid")]
    [OutputType("ValidationInfo")]
    public class ConfirmOpenXmlValidCmdlet : PowerToolsReadOnlyCmdlet
    {
        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Confirm-OpenXmlValid"))
            {
                try
                {
                    foreach (ValidationErrorInfo item in PowerToolsExtensions.ValidateXml(document))
                        WriteObject(new ValidationInfo(document, item), true);
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

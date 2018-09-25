// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomerInformationServiceInputControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomerInformationServiceInputControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Vdv301
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Motion.Common.IbisIP.Schema;
    using Gorba.Motion.Protran.Visualizer.Data.Vdv301;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The input control for <c>CustomerInformationService</c>.
    /// </summary>
    public partial class CustomerInformationServiceInputControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerInformationServiceInputControl"/> class.
        /// </summary>
        public CustomerInformationServiceInputControl()
        {
            this.InitializeComponent();
        }

        private void ButtonVerifyOnClick(object sender, EventArgs e)
        {
            this.VerifyXml(true);
        }

        private void ButtonSendOnClick(object sender, EventArgs e)
        {
            if (!this.VerifyXml(false))
            {
                return;
            }

            CustomerInformationServiceGetAllDataResponseStructure data;
            try
            {
                var serializer = new XmlSerializer(typeof(CustomerInformationServiceGetAllDataResponseStructure));
                using (var reader = XmlReader.Create(new StringReader(this.xmlEditorControl.XmlText)))
                {
                    data = (CustomerInformationServiceGetAllDataResponseStructure)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var visualizationService = ServiceLocator.Current.GetInstance<IVdv301VisualizationService>();
            visualizationService.IbisServiceLocator.CustomerInformationService.SetAllData(
                (CustomerInformationServiceAllData)data.Item);
        }

        private bool VerifyXml(bool showAsError)
        {
            var results = new List<ValidationEventArgs>();
            Exception exception = null;
            var settings = new XmlReaderSettings();
            settings.Schemas.Add(SchemaSetFactory.LoadSchemaSet());
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += (s, ev) => results.Add(ev);

            try
            {
                using (var reader = XmlReader.Create(new StringReader(this.xmlEditorControl.XmlText), settings))
                {
                    while (reader.Read())
                    {
                    }
                }
            }
            catch (XmlException ex)
            {
                exception = ex;
            }

            if (results.Count == 0 && exception == null)
            {
                if (showAsError)
                {
                    MessageBox.Show(
                        this,
                        "The XML was validated successfully",
                        "Validation successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                return true;
            }

            var message = new StringBuilder();
            message.Append("There were issues when validating the XML:");
            if (exception != null)
            {
                message.AppendLine().AppendFormat("{0}: {1}", exception.GetType().Name, exception.Message);
            }

            foreach (var result in results)
            {
                message.AppendLine().AppendFormat("{0}: {1}", result.Severity, result.Message);
            }

            var icon = exception != null
                       || (results.Find(ev => ev.Severity == XmlSeverityType.Error) != null && showAsError)
                           ? MessageBoxIcon.Error
                           : MessageBoxIcon.Warning;
            var buttons = showAsError ? MessageBoxButtons.OK : MessageBoxButtons.OKCancel;
            if (MessageBox.Show(this, message.ToString(), "Validation not successful", buttons, icon)
                == DialogResult.Cancel)
            {
                return false;
            }

            return icon != MessageBoxIcon.Error;
        }
    }
}

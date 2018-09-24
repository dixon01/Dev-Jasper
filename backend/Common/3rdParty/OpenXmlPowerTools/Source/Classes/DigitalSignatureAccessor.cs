/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Collections.ObjectModel;
using System.IO.Packaging;

namespace OpenXmlPowerTools
{
    public class DigitalSignatureAccessor
    {
        public static OpenXmlPowerToolsDocument Insert(OpenXmlPowerToolsDocument doc, IEnumerable<string> certificateList)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (Package package = streamDoc.GetPackage())
                {
                    foreach (string digitalCertificate in certificateList)
                    {
                        X509Certificate x509Certificate = X509Certificate2.CreateFromCertFile(digitalCertificate);
                        PackageDigitalSignatureManager digitalSigntaureManager = new PackageDigitalSignatureManager(package);
                        digitalSigntaureManager.CertificateOption = CertificateEmbeddingOption.InSignaturePart;
                        System.Collections.Generic.List<Uri> partsToSign = new System.Collections.Generic.List<Uri>();
                        //Adds each part to the list, except relationships parts.
                        foreach (PackagePart openPackagePart in package.GetParts())
                        {
                            if (!PackUriHelper.IsRelationshipPartUri(openPackagePart.Uri))
                                partsToSign.Add(openPackagePart.Uri);
                        }
                        List<PackageRelationshipSelector> relationshipSelectors = new List<PackageRelationshipSelector>();
                        //Creates one selector for each package-level relationship, based on id
                        foreach (PackageRelationship relationship in package.GetRelationships())
                        {
                            PackageRelationshipSelector relationshipSelector =
                                new PackageRelationshipSelector(relationship.SourceUri, PackageRelationshipSelectorType.Id, relationship.Id);
                            relationshipSelectors.Add(relationshipSelector);
                        }
                        digitalSigntaureManager.Sign(partsToSign, x509Certificate, relationshipSelectors);
                    }
                }
                return streamDoc.GetModifiedDocument();
            }
        }

        /// <summary>
        ///  Tests a Digital Signature from a package
        /// </summary>
        /// <returns>Digital signatures list</returns>
        public static Collection<string> GetList(OpenXmlPowerToolsDocument doc)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                // Creates the PackageDigitalSignatureManager
                PackageDigitalSignatureManager digitalSignatureManager = new PackageDigitalSignatureManager(streamDoc.GetPackage());
                // Verifies the collection of certificates in the package
                Collection<string> digitalSignatureDescriptions = new Collection<string>();
                ReadOnlyCollection<PackageDigitalSignature> digitalSignatures = digitalSignatureManager.Signatures;
                if (digitalSignatures.Count > 0)
                {
                    foreach (PackageDigitalSignature signature in digitalSignatures)
                    {
                        if (PackageDigitalSignatureManager.VerifyCertificate(signature.Signer) != X509ChainStatusFlags.NoError)
                        {
                            digitalSignatureDescriptions.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Signature: {0} ({1})", signature.Signer.Subject, PackageDigitalSignatureManager.VerifyCertificate(signature.Signer)));
                        }
                        else
                            digitalSignatureDescriptions.Add("Signature: " + signature.Signer.Subject);
                    }
                }
                else
                {
                    digitalSignatureDescriptions.Add("No digital signatures found");
                }
                return digitalSignatureDescriptions;
            }
        }

        /// <summary>
        /// RemoveAll
        /// </summary>
        public static OpenXmlPowerToolsDocument RemoveAll(OpenXmlPowerToolsDocument doc)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (Package package = streamDoc.GetPackage())
                {
                    // Creates the PackageDigitalSignatureManager
                    PackageDigitalSignatureManager digitalSignatureManager = new PackageDigitalSignatureManager(package);
                    digitalSignatureManager.RemoveAllSignatures();
                }
                return streamDoc.GetModifiedDocument();
            }
        }
    }
}
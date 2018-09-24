// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ConsoleApplication
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;

    /// <summary>
    /// The program class.
    /// </summary>
    internal class Program
    {
        private const string Address = "localhost";

        private const string TableName = "storagespikeresources";

        private const string LocalStorageAccount = "devstoreaccount1";

        private const string AzureStorageAccount = "azurestoragespike";

        private const string LocalStorageKey =
                "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

        private const string AzureStorageKey =
                "8EpbQQD8OJfPAQso1pacCOqmaA+ewmgaj1HXIqJcohriY94kUw4Y7e3QwuA3WATj656oHdTFj2D6WswA6nNT4g==";

        private static void Main(string[] args)
        {
            var completed = false;
            while (!completed)
            {
                Console.WriteLine("Type [blob|file]");
                string storageKey;
                string storageAccount;
                var isLocal = false;
                var type = Console.ReadLine();
                if (!string.Equals("blob", type) && !string.Equals("file", type))
                {
                    Console.WriteLine("Type not understood");
                    continue;
                }

                switch (type)
                {
                    case "blob":
                        storageKey = LocalStorageKey;
                        storageAccount = LocalStorageAccount;
                        isLocal = true;
                        break;
                    case "file":
                        storageKey = AzureStorageKey;
                        storageAccount = AzureStorageAccount;
                        break;
                    default:
                        throw new NotSupportedException("Type not valid");
                }

                var resources = GetResources(storageAccount, storageKey, isLocal);
                if (resources.Count < 1)
                {
                    Console.WriteLine("No resources");
                    continue;
                }

                for (var i = 0; i < resources.Count; i++)
                {
                    var resourceEntity = resources[i];
                    Console.WriteLine(
                        "Type {0} for entity {1} (original filename: '{2}', size: {3}",
                        i,
                        resourceEntity.Hash,
                        resourceEntity.OriginalFileName,
                        resourceEntity.Length);
                }

                Console.WriteLine("Enter the index");
                var indexString = Console.ReadLine();
                int index;
                if (!int.TryParse(indexString, out index) || index < 0 || index > resources.Count - 1)
                {
                    Console.WriteLine("Entry not valid");
                    return;
                }

                Console.WriteLine("Enter the max length per request");
                var maxLengthString = Console.ReadLine();
                int maxLength;
                if (!int.TryParse(maxLengthString, out maxLength))
                {
                    Console.WriteLine("Max length not valid");
                    return;
                }

                var entity = resources[index];
                string fullPath;
                switch (type)
                {
                    case "blob":
                        fullPath = GetFromBlobStorage(entity.Hash, entity.OriginalFileName, entity.Length, maxLength);
                        break;
                    case "file":
                        fullPath = GetFromFileStorage(entity.Hash, entity.OriginalFileName, entity.Length, maxLength);
                        break;
                    default:
                        throw new NotSupportedException();
                }

                Process.Start(fullPath);
                Console.WriteLine("Type ctrl+q to exit, anything else to continue");
                var key = Console.ReadKey(true);
                completed = key.Key == ConsoleKey.Q && key.Modifiers == ConsoleModifiers.Control;
            }
        }

        private static string GetFromBlobStorage(string hash, string fileName, long length, int maxLength)
        {
            var uri = new Uri(string.Format(@"http://{0}:10000/devstoreaccount1/resources/{1}", Address, hash));
            var now = DateTime.UtcNow;
            var fullPath = GetFullPath(hash + Path.GetExtension(fileName));
            using (var memoryStream = new MemoryStream())
            {
                while (memoryStream.Length < length)
                {
                    var request = (HttpWebRequest)HttpWebRequest.Create(uri);
                    request.Method = "GET";
                    var remaining = length - memoryStream.Length;
                    var nextBytes = Math.Min(maxLength, remaining) - 1;
                    var nowString = now.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
                    request.Headers.Add("x-ms-date", now.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
                    request.Headers.Add("x-ms-version", "2014-02-14");
                    var end = memoryStream.Length + nextBytes;
                    Console.WriteLine("Requesting bytes {0}-{1}. Bytes remaining: {2}", memoryStream.Length, end, remaining);
                    request.Headers.Add("x-ms-range", string.Format("bytes={0}-{1}", memoryStream.Length, end));

                    var authorizationHeader = GetAuthorizationHeader("GET", now, request, LocalStorageAccount, LocalStorageKey, ifMatch: string.Empty, md5: string.Empty);
                    request.Headers.Add("Authorization", authorizationHeader);
                    var response = (HttpWebResponse)request.GetResponse();
                    using (var readStream = response.GetResponseStream())
                    {
                        CopyStream(readStream, memoryStream);
                    }
                }

                using (var fileStream = File.OpenWrite(fullPath))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    CopyStream(memoryStream, fileStream);
                }
            }
            return fullPath;
        }

        private static string GetFromFileStorage(string hash, string fileName, long length, int maxLength)
        {
            var uri = new Uri(string.Format(@"https://azurestoragespike.file.core.windows.net/resources/{0}.rx", hash));
            var now = DateTime.UtcNow;
            var fullPath = GetFullPath(hash + Path.GetExtension(fileName));
            using (var memoryStream = new MemoryStream())
            {
                while (memoryStream.Length < length)
                {
                    var request = (HttpWebRequest)HttpWebRequest.Create(uri);
                    request.Method = "GET";
                    var nextBytes = Math.Min(maxLength, length - memoryStream.Length) - 1;
                    var nowString = now.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
                    request.Headers.Add(
                        "x-ms-date",
                        now.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
                    request.Headers.Add("x-ms-version", "2014-02-14");
                    var end = memoryStream.Length + nextBytes;
                    Console.WriteLine("Requesting bytes {0}-{1}", memoryStream.Length, end);
                    request.Headers.Add("x-ms-range", string.Format("bytes={0}-{1}", memoryStream.Length, end));

                    var authorizationHeader = GetAuthorizationHeader(
                        "GET",
                        now,
                        request,
                        "azurestoragespike",
                        AzureStorageKey,
                        ifMatch: string.Empty,
                        md5: string.Empty);
                    request.Headers.Add("Authorization", authorizationHeader);
                    var response = (HttpWebResponse)request.GetResponse();
                    using (var readStream = response.GetResponseStream())
                    {
                        CopyStream(readStream, memoryStream);
                    }
                }

                using (var fileStream = File.OpenWrite(fullPath))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    CopyStream(memoryStream, fileStream);
                }
            }

            return fullPath;
        }

        private static List<ResourceEntity> GetResources(string storageAccount, string storageKey, bool isLocal)
        {
            var tableHelper = new TableHelper(storageAccount, storageKey, isLocal);
            var tables = tableHelper.ListTables();
            var list = new List<ResourceEntity>();
            foreach (var table in tables)
            {
                if (!string.Equals(table.Name, TableName, StringComparison.InvariantCulture))
                {
                    continue;
                }

                var entities = tableHelper.QueryEntities(table.Name, string.Empty);
                foreach (var entity in entities)
                {
                    var entityDetails = tableHelper.GetEntity(table.Name, entity.PartitionKey, entity.RowKey);
                    list.Add(
                        new ResourceEntity(
                            entityDetails.RowKey,
                            entityDetails["OriginalFileName"],
                            entityDetails.GetValue<long>("Length")));
                }
            }

            return list;
        }

        private static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[512];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        private static string GetFullPath(string name)
        {
            var originalFileInfo = new FileInfo(name);
            var newFileName = string.Format("{0}{1}", Guid.NewGuid(), originalFileInfo.Extension);
            var tempPath = Path.GetTempPath();
            var fullPath = Path.Combine(tempPath, newFileName);
            return fullPath;
        }

        private static string GetCanonicalizedResource(Uri address, string accountName)
        {
            StringBuilder str = new StringBuilder();
            StringBuilder builder = new StringBuilder("/");
            builder.Append(accountName);     //this is testsnapshots
            builder.Append(address.AbsolutePath);  //this is "/" because for getting a list of containers 
            str.Append(builder.ToString());

            NameValueCollection values2 = new NameValueCollection();
            //address.Query is ?comp=list
            //this ends up with a namevaluecollection with 1 entry having key=comp, value=list 
            //it will have more entries if you have more query parameters
            NameValueCollection values = HttpUtility.ParseQueryString(address.Query);
            foreach (string str2 in values.Keys)
            {
                ArrayList list = new ArrayList(values.GetValues(str2));
                list.Sort();
                StringBuilder builder2 = new StringBuilder();
                foreach (object obj2 in list)
                {
                    if (builder2.Length > 0)
                    {
                        builder2.Append(",");
                    }
                    builder2.Append(obj2.ToString());
                }
                values2.Add((str2 == null) ? str2 : str2.ToLowerInvariant(), builder2.ToString());
            }
            ArrayList list2 = new ArrayList(values2.AllKeys);
            list2.Sort();
            foreach (string str3 in list2)
            {
                StringBuilder builder3 = new StringBuilder(string.Empty);
                builder3.Append(str3);
                builder3.Append(":");
                builder3.Append(values2[str3]);
                str.Append("\n");
                str.Append(builder3.ToString());
            }
            return str.ToString();
        }

        public static ArrayList GetHeaderValues(NameValueCollection headers, string headerName)
        {
            ArrayList list = new ArrayList();
            string[] values = headers.GetValues(headerName);
            if (values != null)
            {
                foreach (string str in values)
                {
                    list.Add(str.TrimStart(null));
                }
            }
            return list;
        }

        public static string GetCanonicalizedHeaders(HttpWebRequest request)
        {
            ArrayList headerNameList = new ArrayList();
            StringBuilder sb = new StringBuilder();

            //retrieve any headers starting with x-ms-, put them in a list and sort them by value.
            foreach (string headerName in request.Headers.Keys)
            {
                if (headerName.ToLowerInvariant().StartsWith("x-ms-", StringComparison.Ordinal))
                {
                    headerNameList.Add(headerName.ToLowerInvariant());
                }
            }
            headerNameList.Sort();

            //create the string that will be the in the right format
            foreach (string headerName in headerNameList)
            {
                StringBuilder builder = new StringBuilder(headerName);
                string separator = ":";
                //get the value for each header, strip out \r\n if found, append it with the key
                foreach (string headerValue in GetHeaderValues(request.Headers, headerName))
                {
                    string trimmedValue = headerValue.Replace("\r\n", String.Empty);
                    builder.Append(separator);
                    builder.Append(trimmedValue);
                    //set this to a comma; this will only be used 
                    //if there are multiple values for one of the headers
                    separator = ",";
                }
                sb.Append(builder.ToString());
                sb.Append("\n");
            }
            return sb.ToString();
        }

        private static string GetAuthorizationHeader(string method, DateTime now, HttpWebRequest request, string storageAccount, string storageKey, string canonicalizedResource = null, string ifMatch = "", string md5 = "")
        {
            string MessageSignature;

            if (canonicalizedResource == null)
            {
                canonicalizedResource = GetCanonicalizedResource(request.RequestUri, storageAccount);
            }

            //this is the raw representation of the message signature 
            MessageSignature = String.Format("{0}\n\n\n{1}\n{5}\n\n\n\n{2}\n\n\n\n{3}{4}",
                method,
                (method == "GET" || method == "HEAD") ? String.Empty : request.ContentLength.ToString(),
                ifMatch,
                GetCanonicalizedHeaders(request),
                canonicalizedResource,
                md5
                );

            //now turn it into a byte array
            byte[] SignatureBytes = System.Text.Encoding.UTF8.GetBytes(MessageSignature);

            //create the HMACSHA256 version of the storage key
            System.Security.Cryptography.HMACSHA256 SHA256 =
                new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(storageKey));

            //Compute the hash of the SignatureBytes and convert it to a base64 string.
            string signature = Convert.ToBase64String(SHA256.ComputeHash(SignatureBytes));

            //this is the actual header that will be added to the list of request headers
            string AuthorizationHeader = "SharedKey " + storageAccount
                + ":" + signature;

            return AuthorizationHeader;
        }
    }

    internal class ResourceEntity
    {
        public ResourceEntity(string hash, string originalFileName, long length)
        {
            this.Hash = hash;
            this.OriginalFileName = originalFileName;
            this.Length = length;
        }

        public string Hash { get; private set; }

        public string OriginalFileName { get; private set; }

        public long Length { get; private set; }
    }
}

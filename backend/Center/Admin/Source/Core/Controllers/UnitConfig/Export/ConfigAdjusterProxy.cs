// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigAdjusterProxy.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigAdjusterProxy type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// The proxy that allows to work with the temporary <see cref="AppDomain"/>.
    /// </summary>
    internal class ConfigAdjusterProxy : MarshalByRefObject
    {
        private readonly List<Assembly> assemblies = new List<Assembly>();

        private IAssemblyResolver assemblyResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigAdjusterProxy"/> class.
        /// </summary>
        public ConfigAdjusterProxy()
        {
            AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomainOnAssemblyResolve;
        }

        /// <summary>
        /// Registers the given <see cref="IAssemblyResolver"/> to be used for finding unknown assemblies.
        /// </summary>
        /// <param name="resolver">
        /// The resolver that should be used from now on to find unknown assemblies.
        /// </param>
        public void RegisterResolver(IAssemblyResolver resolver)
        {
            this.assemblyResolver = resolver;
        }

        /// <summary>
        /// Loads the assembly from the given path into the current <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="assemblyPath">
        /// The full path to the assembly (DLL).
        /// </param>
        public void LoadAssembly(string assemblyPath)
        {
            this.DoLoadAssembly(assemblyPath);
        }

        /// <summary>
        /// Gets the <see cref="XmlSchema"/> for the given type name. This method requires the assembly containing
        /// the type being loaded before with <see cref="LoadAssembly"/>.
        /// </summary>
        /// <param name="typeName">
        /// The full type name (without assembly name).
        /// </param>
        /// <returns>
        /// The serialized <see cref="XmlSchema"/>.
        /// </returns>
        public string GetXmlSchema(string typeName)
        {
            var type = this.GetType(typeName);

            var property = type.GetProperty("Schema", BindingFlags.Static | BindingFlags.Public);
            if (property == null)
            {
                return null;
            }

            var schema = property.GetValue(null) as XmlSchema;
            if (schema == null)
            {
                return null;
            }

            var writer = new StringWriter();
            schema.Write(writer);
            return writer.ToString();
        }

        /// <summary>
        /// Converts the given <paramref name="xml"/> structure using the given
        /// type (<paramref name="typeName"/>). This method requires the assembly containing
        /// the type being loaded before with <see cref="LoadAssembly"/>.
        /// </summary>
        /// <param name="typeName">
        /// The full type name (without assembly name).
        /// </param>
        /// <param name="xml">
        /// The original XML structure.
        /// </param>
        /// <returns>
        /// The converted XML structure.
        /// </returns>
        public string ConvertConfig(string typeName, string xml)
        {
            var type = this.GetType(typeName);
            if (type == null)
            {
                throw new TypeLoadException("Couldn't find " + typeName);
            }

            var serializer = new XmlSerializer(type);
            var reader = new StringReader(xml);
            var config = serializer.Deserialize(reader);

            var memory = new MemoryStream();
            serializer.Serialize(new StreamWriter(memory, Encoding.UTF8), config);
            return Encoding.UTF8.GetString(memory.ToArray());
        }

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains(".XmlSerializers"))
            {
                return null;
            }

            var assembly = this.assemblies.FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
            {
                return assembly;
            }

            if (args.Name == this.GetType().Assembly.FullName)
            {
                return this.GetType().Assembly;
            }

            if (this.assemblyResolver != null)
            {
                var path = this.assemblyResolver.Resolve(args.Name);
                if (path != null)
                {
                    assembly = this.DoLoadAssembly(path);
                }
            }

            return assembly;
        }

        private Type GetType(string typeName)
        {
            foreach (var assembly in this.assemblies)
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            throw new TypeLoadException("Couldn't find " + typeName);
        }

        private Assembly DoLoadAssembly(string assemblyPath)
        {
            var assembly = Assembly.LoadFrom(assemblyPath);
            this.assemblies.Add(assembly);
            return assembly;
        }
    }
}
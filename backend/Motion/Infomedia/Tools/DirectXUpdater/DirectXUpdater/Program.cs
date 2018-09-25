// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DirectXUpdater
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Defines the program.
    /// </summary>
    internal class Program
    {
        private static readonly string[] DefaultDirectories = new[] { @"D:\Progs", @"D:\Infomedia" };

        /// <summary>
        /// Entry point for the application.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            var directories = new List<string>();
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("No arguments found. Using default directories.");
                directories.AddRange(DefaultDirectories);
            }
            else
            {
                Console.WriteLine("Using directories passed as argument");
                directories.AddRange(args);
            }

            foreach (var directory in directories)
            {
                try
                {
                    Console.WriteLine("Directory '{0}'", directory);
                    var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var attributes = File.GetAttributes(file);
                            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                            {
                                Console.WriteLine("Removing ReadOnly flag from file '{0}'", file);
                                File.SetAttributes(file, attributes & ~FileAttributes.ReadOnly);
                                continue;
                            }

                            Console.WriteLine("File '{0}' doesn't have the ReadOnly flag", file);
                        }
                        catch (Exception fileException)
                        {
                            Console.WriteLine("Error while processing file '{0}': {1}", file, fileException.Message);
                        }
                    }
                }
                catch (Exception directoryException)
                {
                    Console.WriteLine(
                        "Error while processing directory '{0}': {1}", directory, directoryException.Message);
                }
            }

            Console.WriteLine("Process completed");
        }
    }
}



namespace TeamCityBuildNumberConsoleApp
{
    using System;
    using System.Reflection;

    class Program
    {
        static void Main(string[] args)
        {
            var version = Assembly.GetEntryAssembly().GetName().Version;
            Console.WriteLine(version.ToString());
            Console.ReadLine();
        }
    }
}

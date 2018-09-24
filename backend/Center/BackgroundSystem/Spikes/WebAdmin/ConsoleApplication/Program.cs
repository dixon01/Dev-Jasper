using System;
using Gorba.Center.BackgroundSystem.Host;
using Microsoft.Owin.Hosting;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server");
            try
            {
                var backgroundSystemHost = new BackgroundSystemHost();
                backgroundSystemHost.Start();
                using (WebApp.Start("http://localhost:9191"))
                {
                    Console.WriteLine("Type <Enter> to stop and exit");
                    Console.ReadLine();
                    Console.WriteLine("Stopping services");
                }

                backgroundSystemHost.Stop();
            }
            catch (Exception exception)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(exception);
                Console.ResetColor();
            }
        }
    }
}
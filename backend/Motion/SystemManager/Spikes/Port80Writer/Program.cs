namespace Port80Writer
{
    using System;
    using System.IO;
    using System.ServiceProcess;
    using System.Threading;

    public class Program
    {
        public static void Main(string[] args)
        {
            foreach (var service in ServiceController.GetServices())
            {
                Console.WriteLine(" - {0} ({1})", service.ServiceName, service.Status);
            }

            using (var writer = new StreamWriter(
                new FileStream("C:\\Temp\\Port80.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
            {
                byte counter = 1;
                while (true)
                {
                    writer.Write("{0:X2}\n", counter++);
                    writer.Flush();

                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
        }
    }
}

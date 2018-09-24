using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminator.Multicast.ReceiveSolo
{
    public class ConsoleMessage : IConsoleMessage
    {
        public  void Success(string message)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine();
            Console.ResetColor();
        }

        public  void Error(string message)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine();
            Console.ResetColor();
        }

        public  void Warning(string message)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine();
            Console.ResetColor();
        }

        public  void Highlight(string message)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}

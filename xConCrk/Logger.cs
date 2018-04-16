using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xConCrk
{
    public class Logger
    {
        static object SpinLock = new object();

        public static void err(string message, params object[] args)
        {
            lock (SpinLock)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Error");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(message, args);
            }
        }
        public static void wfromClass(string ccl, string[] message, ConsoleColor[] color)
        {
            lock (SpinLock)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(ccl);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("] ");
                w(message, color);
            }
        }
        public static void wlfromClass(string ccl, string message, ConsoleColor color = ConsoleColor.White, params object[] args)
        {
            lock (SpinLock) {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(ccl);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("] ");
                wl(message, color, args);
            }
        }
        public static void wl(string message, ConsoleColor color, params object[] args)
        {
            lock (SpinLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message, args);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        public static void w(string[] message, ConsoleColor[] color)
        {
            lock (SpinLock)
            {
                for (int i = 0; i < message.Length; i++)
                {
                    string msg = message[i];
                    ConsoleColor col = color[i];
                    Console.ForegroundColor = col;
                    Console.Write(msg);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}

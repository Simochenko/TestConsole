using System;
using System.IO;

namespace TestConsole
{
    public class Logger
    {
        private readonly string _fileName;
        public Logger(string fileName)
        {
            _fileName = fileName;
        }
        public void WriteLine(string str)
        {
            System.Console.WriteLine(str);
            WriteFile(str);
        }

        public void WriteError(string str)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(str);
            System.Console.ForegroundColor = ConsoleColor.White;
        }

        public void WriteInfo(string str)
        {
            System.Console.ForegroundColor = ConsoleColor.Green;
            WriteLine(str);
            System.Console.ForegroundColor = ConsoleColor.White;
        }

        private void WriteFile(string str)
        {
            File.AppendAllText(_fileName, str + "\n");
        }
    }
}

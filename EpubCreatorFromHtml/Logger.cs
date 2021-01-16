using System;
using System.Collections.Generic;
using System.Text;

namespace EpubCreatorFromHtml
{
    public class Logger
    {
        public static void LogToConsole(string logInformation)
        {
            Console.WriteLine($"{LogPrefix} {logInformation}");
        }

        private const string LogPrefix = "[LOG]";
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XConnectTest.Utilitys
{
    internal class LoggingUtility
    {
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log.txt");

        public static void Log(string message)
        {
            try
            {
                using (StreamWriter streamWriter = System.IO.File.AppendText(LogFilePath))
                {
                    streamWriter.WriteLine($"*** {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")} ***");
                    streamWriter.WriteLine(message);
                    streamWriter.WriteLine("******");
                    streamWriter.WriteLine();
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                Console.WriteLine($"Error saving log: {ex.Message}");
            }
        }
    }
}

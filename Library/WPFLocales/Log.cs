using System;
using System.IO;

namespace WPFLocales
{
    internal static class Log
    {
        public static void Info(params string[] messages)
        {
            var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WpfLocales", "log.txt");
            using (var writer = new StreamWriter(file, true))
            {
                foreach (var message in messages)
                {
                    writer.WriteLine(message);
                }
            }
        }
    }
}

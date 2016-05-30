using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sort
{
    public class LogUtil
    {
        private static object writelock = new object();
        private static string DefaultPath = AppDomain.CurrentDomain.BaseDirectory + "logs/";
        private static bool IsExistsPath = false;
        public static void Write(string txt)
        {
            Write(txt, DefaultPath, false);
        }
        public static void Write(string txt, string path, bool isError = false)
        {
            if (isError)
            {
                Console.WriteLine("Error:" + txt);
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString() + "\t" + txt);
            }
            lock (writelock)
            {
                if (!IsExistsPath)
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        if (!System.IO.Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                            IsExistsPath = true;
                        }
                    }
                }
                System.IO.File.AppendAllText(string.IsNullOrEmpty(path) ? (AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyy-MM-dd") + ".log") : (path + DateTime.Now.ToString("yyyy-MM-dd") + ".log"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + txt + Environment.NewLine);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Launcher.Source
{
    public class Global
    {
        private string LOG_PATH = $"{AppDomain.CurrentDomain.BaseDirectory}\\log.txt";

        public enum LOG_TYPE
        {
            INFO, ERROR
        }

        public void ErrorLog(string message)
        {
            string msg = $"[{DateTime.Now.ToString("HH:mm:ss")}][{LOG_TYPE.ERROR}] : {message}\n";

            File.AppendAllText(LOG_PATH, msg);
        }

        public void InfoLog(string message)
        {
            string msg = $"[{DateTime.Now.ToString("HH:mm:ss")}][{LOG_TYPE.INFO}] : {message}\n";

            File.AppendAllText(LOG_PATH, msg);
        }
    }
}

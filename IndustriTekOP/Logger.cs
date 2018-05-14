using IndustriTekOP.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustriTekOP
{
    class Logger
    {
        private string _path;

        public Logger(string path)
        {
            this._path = path;

        }

        public void AddEntry(string message)
        {
            //Set culture info to format time correctly
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            //Create entry
            string entry = DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss - ") + message;

            //Create filepath based on date - New file per day.
            string file = DateTime.Today.ToString("dd-MM-yyyy") + ".txt";
            string fullPath = Path.Combine(this._path, file);

            if(!Directory.Exists(this._path))
            {
                Directory.CreateDirectory(this._path);
            }

            //Write log to file
            try
            {
                StreamWriter log = new StreamWriter(fullPath, true);
                log.WriteLine(entry);
                log.Close();
            }
            catch
            {
                //throw new InaccessibleFileException("Failed to write to log file");
            }

        }

        public void OpenLogPath()
        {
            Process.Start("explorer.exe", this._path);
        }

        public void OpenLogFile()
        {
            string file = DateTime.Today.ToString("dd-MM-yyyy") + ".txt";
            Process.Start("notepad.exe", this._path + file);
        }

    }
}

using IndustriTekOP.Database.Models;
using IndustriTekOP.Database.Tables;
using IndustriTekOP.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustriTekOP
{
    public partial class OperatorPanel : Form
    {
        //Define constants
        private const int CS_DROPSHADOW = 0x00020000;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private TimeSpan _logTime = DateTime.Now.TimeOfDay.Subtract(TimeSpan.FromSeconds(10));
        private TimeSpan _lastCheck = DateTime.Now.TimeOfDay.Subtract(TimeSpan.FromSeconds(10));
        private int lineCount = 0;
        private int _prevCount = 0;
        private string _prevActivity = "";
        private Position _prevPosition = new Position(0, 0, 90);

        private Vehicle _vehicle;
        private Reader _reader;
        private Logger _log;
        private Tag tag;

        private Thread TaskThread;
        private Thread ListenerThread;

        private bool isManual = false;
        
        //Add dropshadow to form.
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                //Add dropshadow
                cp.ClassStyle |= CS_DROPSHADOW;

                return cp;
            }
        }

        public OperatorPanel()
        {

        }

        public OperatorPanel(Vehicle vh)
        {
            InitializeComponent();

            //Remove button borders when form is out of focus
            ExitButton.FlatAppearance.BorderColor = Color.FromArgb(0, 38, 38, 38);
            MinimizeButton.FlatAppearance.BorderColor = Color.FromArgb(0, 38, 38, 38);

            //Set objects
            this._vehicle = vh;
            this._reader = new Reader();
            this._log = new Logger(Properties.Settings.Default.logPath);

        }

        private void OperatorPanel_Load(object sender, EventArgs e)
        {
            ConnectionTimer.Enabled = true;
            ActivityTimer.Enabled = true;

            this._log.AddEntry("Arduino vehicle connected");

            StartReaderConnector();

            //Start Thread Managing the NFC Reader.
            StartListener();

        }

        /// <summary>
        /// Start Thread that starts and stops Reader Listener as needed.
        /// </summary>
        private void StartListener()
        {
            this.ListenerThread = new Thread(this.Listener);
            this.ListenerThread.Start();
        }

        private void StartReaderConnector()
        {
            this.TaskThread = new Thread(this.ReaderConnector);
            this.TaskThread.Start();
        }

        private void Listener()
        {
            while(true)
            {
                if(this._reader.isConnected && !this._vehicle.isBusy)
                {
                    if((!this._reader.isListenerAborted && this._reader.Thread == null) || (!this._reader.isListenerAborted && !this._reader.IsListenerThreadAlive))
                    {
                        this._reader.StartListener();

                        SetActivity("Waiting for scanned tag", "Available", true);

                        //Continuously check if thread is alive - Thread shuts down when tag is received.
                        while (this._reader.IsListenerThreadAlive && !this._reader.isListenerAborted)
                        {
                            Thread.Sleep(200);
                        }

                        if(!this._reader.isListenerAborted)
                        {
                            this._vehicle.isBusy = true;

                            SetActivity("Tag received - Identifying...", "Busy", true);

                            IdentifyTag();

                            SetActivity("Moving " + tag.TypeName + " tag to destination", "Busy", true);

                            Sort();

                            this._log.AddEntry("Task completed");
                            
                        }

                    }

                    if(this._reader.isListenerAborted && this._reader.IsListenerThreadAlive)
                    {
                        this._reader.StopListener();
                    }

                }

                Thread.Sleep(200);
            }
        }

        private void ReaderConnector()
        {
            //Make sure vehicle isn't running a manual task
            if(!this._vehicle.isBusy)
            {
                //Set isConnecting bool to true (Makes sure retry connection button stays hidden while its working)
                this._reader.isConnecting = true;

                //Get every available COM port
                string[] ports = this._reader.GetComPorts();

                if (ports.Length < 2)
                {
                    this._log.AddEntry("Could not connect reader - No ports available");
                }
                else
                {
                    //Loop through available ports
                    foreach (string port in ports)
                    {
                        if(port != this._vehicle.vehicle.PortName)
                        {
                            SetActivity("Connecting to NFC Reader - Trying port: " + port, "Busy", true);

                            if (this._reader.PortMatches(port))
                            {
                                this._log.AddEntry("NFC Reader connected");
                                SetActivity("Waiting for scanned tag", "Available", true);

                                this._reader.isConnected = true;

                                //Break the loop as we are already connected.
                                break;
                            }
                        }
                            
                    }

                    if (!this._reader.isConnected)
                    {
                        this._log.AddEntry("Could not connect reader - No ports match");
                        SetActivity("Waiting for user input", "Available", true);
                        this._reader.isConnecting = false;
                    }
                }
            }
            else
            {
                throw new OngoingActivityException();
            }
           
        }

        public bool IsThreadAlive => this.TaskThread.IsAlive;

        private void SetActivity(string status, string type = null, bool invoke = false)
        {
            if(type != null)
            {
                switch(type)
                {
                    case "Available":
                        StatusLabel.ForeColor = ColorTranslator.FromHtml("#30ba40");
                        break;
                    case "Busy":
                        StatusLabel.ForeColor = ColorTranslator.FromHtml("#e99300");
                        break;
                    case "Error":
                        StatusLabel.ForeColor = ColorTranslator.FromHtml("#ff4d4d");
                        break;
                }
            }
            else
            {
                if (this._vehicle.isBusy)
                {
                    StatusLabel.ForeColor = ColorTranslator.FromHtml("#e99300");
                }
                else
                {
                    StatusLabel.ForeColor = ColorTranslator.FromHtml("#30ba40");
                }
            }

            if (invoke == true)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    StatusLabel.Text = status;
                });
            }
            else
            {
                StatusLabel.Text = status;
            }

        }

        private void IdentifyTag()
        {
            //Log received tag UUID
            this._log.AddEntry("Received from Reader: " + this._reader.tag);

            //Tags table object
            Tags tags = new Tags(Application.StartupPath + "\\Resources\\NFCSorter.db");

            //Get tag information
            tag = tags.GetTagByUUID(this._reader.tag);

            this._reader.tag = "";

            //Log tag identity
            this._log.AddEntry("Tag identified as: " + tag.TypeName);

            //Let reader know that tag is received - Reader will no longer send data until its told to.
            this._reader.SendMessage("Tag Received;");
        }

        private void SimulateTag(string typeName)
        {
            this._log.AddEntry("Simulating scanned " + typeName + " tag");
            SetActivity("Running simulation", "Busy");

            Tags tags = new Tags(Application.StartupPath + "\\Resources\\NFCSorter.db");

            tag = tags.GetTagByTypeName(typeName);

            this.TaskThread = new Thread(Sort);

            this.TaskThread.Start();

        }

        private void Sort()
        {
            this._vehicle.SetDestination(this.tag.PosX, this.tag.PosY, this.tag.Rotation);

            this._vehicle.MoveToPosition();

            this._vehicle.Unload();

            if (!isManual)
            {
                this._log.AddEntry("Tag delivered to destination succesfully");

                this._log.AddEntry("Returning to base");

                this._vehicle.Reset();
            }

            Reset();
        }

        public void Reset()
        {
            this._vehicle.isBusy = false;
            this._reader.isListenerAborted = false;
            tag = null;
            this._reader.SendMessage("Task complete;");
        }

        //Handlers
        private void ExitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void ExitButton_MouseEnter(object sender, EventArgs e)
        {
            ExitButton.BackColor = ColorTranslator.FromHtml("#FF4D4D");
        }

        private void ExitButton_MouseLeave(object sender, EventArgs e)
        {
            ExitButton.BackColor = ColorTranslator.FromHtml("#3a4047");
        }

        private void bunifuSeparator2_Load(object sender, EventArgs e)
        {

        }

        private void LogContainer_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                object item = LogContainer.Items[e.Index];
                Color background;

                //Check if odd or even.
                if (e.Index % 2 == 0)
                {
                    background = ColorTranslator.FromHtml("#484b52");
                }
                else
                {
                    background = ColorTranslator.FromHtml("#3b3f45");
                }

                e.DrawBackground();
                Graphics g = e.Graphics;
                g.FillRectangle(new SolidBrush(background), e.Bounds);
                g.DrawString(
                    item.ToString(),
                    e.Font,
                    new SolidBrush(Color.White),
                    e.Bounds.X + 10,
                    e.Bounds.Y + 6
                );
            }
            catch
            {

            }
            

        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            //Remove all items from log Listbox.
            LogContainer.Items.Clear();
        }

        private void FolderButton_Click(object sender, EventArgs e)
        {
            this._log.OpenLogPath();
        }

        private void FileButton_Click(object sender, EventArgs e)
        {
            this._log.OpenLogFile();
        }

        private void ApplicationHandle_MouseDown(object sender, MouseEventArgs e)
        {
            //Simulate titlebar and make it draggable.
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            //Minimize Application
            base.WindowState = FormWindowState.Minimized;
        }

        private void LogReader_Tick(object sender, EventArgs e)
        {
            //Make sure filename is current
            string file = DateTime.Today.ToString("dd-MM-yyyy") + ".txt";

            //Get all lines in the file
            var lines = File.ReadLines(Properties.Settings.Default.logPath + file);

            try
            {
                this.lineCount = lines.Count();

                if (this.lineCount != this._prevCount)
                {
                    foreach (var line in lines)
                    {
                        string[] pieces = line.Split('-');

                        TimeSpan time = TimeSpan.Parse(pieces[1]);

                        if (time > this._logTime && time > this._lastCheck)
                        {
                            //Output to UI
                            LogContainer.Items.Add(line);
                            LogContainer.TopIndex = LogContainer.Items.Count - 1;
                        }

                    }

                }
            }
            catch
            {

            }

            this._prevCount = this.lineCount;
            this._lastCheck = DateTime.Now.TimeOfDay.Subtract(TimeSpan.FromSeconds(2));
        }

        private void ConnectionTimer_Tick(object sender, EventArgs e)
        {
            if(this._vehicle.isConnected && this._reader.isConnected)
            {
                ConnectionLabel.Text = "Connected to Arduino vehicle & reader";
                ConnectionPanel.BackColor = ColorTranslator.FromHtml("#30ba40");

                RetryButton.Visible = false;
            }
            else if(this._vehicle.isConnected && !this._reader.isConnected)
            {
                ConnectionLabel.Text = "Connected to Arduino vehicle - Reader unavailable";
                ConnectionPanel.BackColor = ColorTranslator.FromHtml("#e99300");

                if (!this._reader.isConnecting)
                {
                    //Show retry button
                    RetryButton.Visible = true;
                }
                else
                {
                    RetryButton.Visible = false;
                }
            }
        }

        private void RetryButton_Click(object sender, EventArgs e)
        {
            RetryButton.Visible = false;

            StartReaderConnector();
        }

        private void UnloadButton_Click(object sender, EventArgs e)
        {
            if(!this._vehicle.isBusy)
            {
                this.TaskThread = new Thread(this._vehicle.Unload);
                this.TaskThread.Start();
            }
        }

        private void ActivityTimer_Tick(object sender, EventArgs e)
        {
            string activity = this._vehicle.GetActivity();

            if(activity != "" && activity != this._prevActivity)
            {
                this._log.AddEntry(activity);
            }

            this._prevActivity = activity;
        }

        private void PositionTimer_Tick(object sender, EventArgs e)
        {
            Position currentPos = this._vehicle.GetPosition();

            if(!currentPos.Equals(this._prevPosition))
            {
                PositionLabel.Text = "X: " + currentPos.PosX + " Y: " + currentPos.PosY + " Rotation: " + currentPos.Rotation;
            }

            this._prevPosition = currentPos;
        }

        private void ManualButton_Click(object sender, EventArgs e)
        {
            //Stop listener
            this._reader.isListenerAborted = true;
        }

        private void PaperButton_Click(object sender, EventArgs e)
        {
            if(!this._vehicle.isBusy)
            {
                //Stop listener
                this._reader.isListenerAborted = true;
                this._vehicle.isBusy = true;

                SimulateTag("Paper");

                this._log.AddEntry("Simulation complete");

                Reset();
            }

        }

        private void WoodButton_Click(object sender, EventArgs e)
        {
            if(!this._vehicle.isBusy)
            {
                //Stop listener
                this._reader.isListenerAborted = true;
                this._vehicle.isBusy = true;

                SimulateTag("Wood");

                this._log.AddEntry("Simulation complete");

                Reset();
            }
        }

        private void PlasticButton_Click(object sender, EventArgs e)
        {
            if(!this._vehicle.isBusy)
            {
                //Stop listener
                this._reader.isListenerAborted = true;
                this._vehicle.isBusy = true;

                SimulateTag("Plastic");

                this._log.AddEntry("Simulation complete");

                Reset();
            }
        }

        private void MetalButton_Click(object sender, EventArgs e)
        {
            if(!this._vehicle.isBusy)
            {
                //Stop listener
                this._reader.isListenerAborted = true;
                this._vehicle.isBusy = true;

                SimulateTag("Metal");

                this._log.AddEntry("Simulation complete");

                Reset();
            } 
        }

        private void UnknownButton_Click(object sender, EventArgs e)
        {
            if(!this._vehicle.isBusy)
            {
                //Stop listener
                this._reader.isListenerAborted = true;
                this._vehicle.isBusy = true;

                SimulateTag("Unknown");

                this._log.AddEntry("Simulation complete");

                Reset();
            }
        }
    }
}

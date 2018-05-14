using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustriTekOP
{
    public partial class ConnectionHandler : Form
    {
        public static ConnectionHandler ConnectionInstance;

        private const int CS_DROPSHADOW = 0x20000;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        Vehicle vh = new Vehicle();

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        public ConnectionHandler()
        {
            ConnectionInstance = this;

            InitializeComponent();

        }

        private void ConnectionHandler_Load(object sender, EventArgs e)
        {
            Connect();
        }

        private void LoadMainForm()
        {
            //Hide Connection Form
            ShowInTaskbar = false;
            Visible = false;

            //Create the main Panel to be shown once connection is made
            var MainForm = new OperatorPanel(vh);

            //Display Main Form
            MainForm.Show();
        }

        private void Connect()
        {
            new Thread(delegate ()
            {
                //Loop until connected
                while (!vh.isConnected)
                {
                    //Get every available COM port
                    string[] ports = vh.GetComPorts();

                    if (ports.Length < 1)
                    {
                        Invoke((MethodInvoker)delegate ()
                        {
                            SubStatusLabel.Text = "NO COM PORTS DETECTED";
                        });
                    }
                    else
                    {
                        foreach (string port in ports)
                        {
                            Invoke((MethodInvoker)delegate ()
                            {
                                SubStatusLabel.Text = port + " FOUND - TRYING CONNECTION";
                            });

                            if (vh.PortMatches(port))
                            {
                                vh.isConnected = true;
                                break;
                            }
                        }

                        //Skip rest of loop if connection was made
                        if (vh.isConnected)
                        {
                            continue;
                        }

                        Invoke((MethodInvoker)delegate ()
                        {
                            SubStatusLabel.Text = "NO PORTS MATCHING VEHICLE FOUND";
                        });

                        Thread.Sleep(5000);
                    }
                }

                Invoke((MethodInvoker)delegate ()
                {
                    LoadMainForm();
                });

            }).Start();

        }

        /// <summary>
        /// Makes connection handler/loading window draggable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectionHandler_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Preloader_MouseDown(object sender, MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        private void StatusLabel_MouseDown(object sender, MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        private void SubStatusLabel_MouseDown(object sender, MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }
    }
}

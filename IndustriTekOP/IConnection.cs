using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustriTekOP
{
    interface IConnection
    {
        void MessageHandler(object sender, SerialDataReceivedEventArgs e);
        void SendMessage(string message);
        string GetMessage();
        bool PortMatches(string port);

    }
}

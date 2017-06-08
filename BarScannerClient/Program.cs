using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BarScannerClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient("192.168.0.5", 1337);
            Console.WriteLine("Connected to server...");
            NetworkStream netStream = client.GetStream();
            


        }
    }
}

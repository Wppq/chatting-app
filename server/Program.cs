using System.Net;
using System.Net.Sockets;

namespace Chat_App 
{
    public class Program 
    {
        private static List<Socket> clients = new List<Socket>();
        private static TcpListener tcpListener = new TcpListener(IPAddress.Parse("localhost"),8080);
        public static void Main()
        {
            tcpListener.Start();
            while(true)
            {
                Socket client = tcpListener.AcceptSocket();
                if(client.Connected)
                {
                    clients.Add(client);
                    Thread newThread = new Thread(() => Listeners(client));
                    newThread.Start();
                }
            }
        }

        public static void Listeners(Socket client)
        {
            Console.WriteLine("Client : " + client.RemoteEndPoint + "now connected");
        }
    }
}
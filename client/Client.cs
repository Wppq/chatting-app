using System.Text;
using System.Net.Sockets;

namespace ClientConsole
{
    public class Client
    {
        public Socket Socket { get; private set; }
        public void StartChatting()
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect("192.168.1.16", 3000);
            Thread Thread = new Thread(GetMessages);
            Thread.Start();
            while (true)
            {
                string input = Console.ReadLine();
                byte[] message = Encoding.Unicode.GetBytes(input);
                Socket.Send(message);
            }
        }
        private void GetMessages()
        {
            while (true)
            {
                byte[] data = new byte[512];
                int res = Socket.Receive(data);
                if (res > 0)
                {
                    string message = Encoding.Unicode.GetString(data);
                    Console.WriteLine(message.Trim('\0'));
                }

            }
        }
    }
}

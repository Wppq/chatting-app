using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Server
    {
        public Thread thread { get; private set; }
        public bool IsServerActive { get; private set; }
        public int UserIdCounter { get; set; }
        public int Port = 3000;
        public Socket ListeningSocket { get; private set; }
        public List<Client> LstClients { get; private set; }
        public IPEndPoint EndPoint { get; private set; }
        public static bool IsSocketConnected(Socket s)
        {
            if (!s.Connected)
                return false;

            if (s.Available == 0)
                if (s.Poll(1000, SelectMode.SelectRead))
                    return false;

            return true;
        }
        public Server()
        {
            Port = 3000;
            UserIdCounter = 0;
            IsServerActive = false;
            LstClients = new List<Client>();
        }
        private bool SendMessage(Client client, string msg)
        {
            try
            {
                client.Socket.Send(Encoding.Unicode.GetBytes(msg));
                return true;
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Can't send message to {0}", client.Username);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            return false;
        }
        private void WaitConnections()
        {
            while (IsServerActive)
            {
                Client client = new Client
                {
                    ID = UserIdCounter,
                    Username = "User" + UserIdCounter.ToString(),
                };
                try
                {
                    client.Socket = ListeningSocket.Accept();
                    client.thread = new Thread(() => ProcessMessaging(client)); //?????????
                    Console.WriteLine("{0} connected.", client.Username);
                    UserIdCounter += 1;
                    LstClients.Add(client);
                    client.thread.Start();
                }
                catch (Exception)
                {
                    Console.WriteLine("Error in waiting to connections");
                }
            }
        }
        public void ProcessMessaging(Client client)
        {
            try
            {
                SendMessage(client, "Welcome to chat app");
            }
            catch
            {
                Console.WriteLine("Error in user connecting");
                return;
            }
            while (IsServerActive)
            {
                try
                {
                    byte[] buff = new byte[512]; //256 Unicode symbols :)
                    if (!IsSocketConnected(client.Socket))
                    {
                        LstClients.Remove(client);
                        Console.WriteLine("{0} disconnected.", client.Username);
                        client.Dispose();
                        return;
                    }
                    int res = client.Socket.Receive(buff);
                    if (res > 0)
                    {
                        string response = string.Empty;
                        string strMessage = Encoding.Unicode.GetString(buff);
                        bool IsSend = false;
                        string msgto = client.Username + " > " + strMessage;
                        foreach (Client user in LstClients)
                        {
                            IsSend = SendMessage(user, msgto);
                        }
                        SendMessage(client, response);
                    }
                }
                catch (SocketException)
                {
                    LstClients.Remove(client);
                    Console.WriteLine("{0} disconnected.", client.Username);
                    client.Dispose();
                    return;
                }
            }
        }
        public void Start()
        {
               
            while (true)
            {
                if (IsServerActive) return;
                ListeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                EndPoint = new IPEndPoint(IPAddress.Any, Port);
                Console.WriteLine(EndPoint);
                ListeningSocket.Bind(EndPoint);
                ListeningSocket.Listen(5);
                Console.WriteLine("Server was started.\nWait for connections...");
                thread = new Thread(WaitConnections);
                thread.Start();
                IsServerActive = true;
            }
        }
    }
}

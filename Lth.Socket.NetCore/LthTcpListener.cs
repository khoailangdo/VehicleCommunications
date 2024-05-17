using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LthSocket.NetCore
{
    public class LthTcpListener
    {
        private static TcpListener? tcpListener { get; set; }
        private static List<TcpClient> clients = new List<TcpClient>();

        private IPAddress Ip {  get; set; }
        private int Port { get; set; }  

        public LthTcpListener(string? ip, int port)
        {
            if (IPAddress.TryParse(ip, out var IpOut))
            {
                Ip = IpOut;
            }
            else
            {
                Ip = IPAddress.Any;
            }
            Port = port;
        }

        public void Start()
        {
            tcpListener = new TcpListener(Ip, Port);
            tcpListener.Start();

            Console.WriteLine($"Server is listening on port {Port}...");

            StartAccept();
        }


        public void Stop()
        {
            tcpListener?.Stop();
        }

        private static void StartAccept()
        {
            tcpListener?.BeginAcceptTcpClient(HandleAsyncAccept, tcpListener);
        }

        private static void HandleAsyncAccept(IAsyncResult ar)
        {
            StartAccept(); // Listen for new connections again

            var listener = ar.AsyncState as TcpListener;
            var client = listener?.EndAcceptTcpClient(ar);

            if (client != null)
            {
                lock (clients)
                {
                    clients.Add(client); // Add the new client to the list
                }
                Console.WriteLine("Client connected. Total clients: " + clients.Count);

                // Handle client in a new thread
                var clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
            else
            {
                Console.WriteLine("Client isn't connected. Total clients: " + clients.Count);
            }
        }

        private static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string receivedData = Encoding.Unicode.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received: " + receivedData);

                    // Echo the data back to the client
                    stream.Write(buffer, 0, bytesRead);
                }
            }
            catch (SocketException ex){
                if (client.Connected)
                {
                    // Handle the case where the client is still connected but an error occurred.
                }
                else
                {
                    // Handle the case where the client has disconnected.
                    Console.WriteLine("Client disconnected unexpectedly: " + ex.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            finally
            {
                stream.Close();
                client.Close();
                lock (clients)
                {
                    clients.Remove(client); // Remove the client from the list
                }
                Console.WriteLine("Client disconnected. Total clients: " + clients.Count);
            }
        }

        public static bool IsSocketConnected(Socket s)
        {
            return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
        }

        public int ClientCount() { return clients.Count; }
    }
}

using JT808.Protocol.Extensions;
using System.Drawing;
using System.Net.Sockets;
using System.Text;

namespace LthSocket.NetCore
{
    public class LthTcpClient
    {
        private static TcpClient? client { get; set; }
        private static NetworkStream? clientStream { get; set; }

        private string? RemoteIp { get; set; }
        private int RemotePort { get; set; }
        public int MessageSent {  get; set; }

        public LthTcpClient(string? remoteIp, int remotePort)
        {
            MessageSent = 0;
            RemoteIp = remoteIp;
            RemotePort = remotePort;
        }

        public async Task Start()
        {
            if (RemoteIp == null) { return; }
            var client = new TcpClient() {
                ReceiveBufferSize = 1024,   
            };
            try
            {
                await client.ConnectAsync(RemoteIp, RemotePort);
                if (client.Connected)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Client is connected with {RemoteIp}:{RemotePort}...");
                    Console.ResetColor();

                    clientStream = client.GetStream();
                }
            }
            catch (SocketException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("SocketException: {0}", e);
                Console.ResetColor();
            }
            //finally
            //{
            //    // Đóng tất cả
            //    client.Close();
            //}
        }
        public void Stop()
        {
            try
            {
                if (client != null && client.Connected) client.Close();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("SocketException: {0}", e);
                Console.ResetColor();
            }
        }

        public NetworkStream? GetStream() { return clientStream; }

        public async Task SendMessageAsync(NetworkStream stream, byte[] dataBytes)
        {
            try
            {
                if (client == null) { await Start(); }
                if (stream == null) { stream = client.GetStream(); }
                // Asynchronously write the message to the stream
                await stream.WriteAsync(dataBytes, 0, dataBytes.Length);

                Console.ForegroundColor = GetRandomColor();
                Console.WriteLine("Sent data: " + dataBytes.ToHexString());
                Console.ResetColor();
                MessageSent = MessageSent + 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SendMessageAsync Exception: {ex.Message}");
            }
        }

        private ConsoleColor GetRandomColor()
        {
            ConsoleColor[] consoleColors = (ConsoleColor[])Enum.GetValues(typeof(ConsoleColor));
            ConsoleColor randomColor;

            // Generate a random color until it's not yellow
            do
            {
                randomColor = consoleColors[new Random().Next(consoleColors.Length)];
            } while (randomColor == ConsoleColor.Yellow);

            return randomColor;
        }

        public async Task<byte[]> ReceiveMessageAsync(NetworkStream stream) {
            byte[] dataBytes = Array.Empty<byte>();
            try
            {
                // Nhận dữ liệu từ máy chủ
                byte[] receivedBytes = new byte[1024];
                int bytesRead = await stream.ReadAsync(receivedBytes, 0, receivedBytes.Length);
                dataBytes = receivedBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ReceiveMessageAsync Exception: {ex.Message}");
            }
            return dataBytes;
        }
    }
}

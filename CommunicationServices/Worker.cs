using CommunicationProtocol;
using CommunicationServices.Models;
using LthSocket.NetCore;
using MessagePack;
using Microsoft.Extensions.Options;
using System.Buffers;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using JT808.Protocol;
using JT808.Protocol.Enums;
using JT808.Protocol.Extensions;
using JT808.Protocol.MessageBody;


namespace CommunicationServices
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptions<WorkserSettings> _settings;
        private LthTcpClient _tcpClient;

        public Worker(ILogger<Worker> logger, IOptions<WorkserSettings> settings, JT808Serializer jT808Serializer)
        {
            _logger = logger;
            _settings = settings;

            var msg = new string[] { "<CKVER>", "<ckbsj>" };
            _tcpClient = new LthTcpClient(_settings.Value.RemoteIp, _settings.Value.RemotePort);
            _tcpClient.Start().Wait();
            var stream = _tcpClient.GetStream();
            foreach (var key in msg)
            {
                _tcpClient.SendMessageAsync(stream, MessagePackaging(key).ToHexBytes()).Wait();
                
                var dataReceived = _tcpClient.ReceiveMessageAsync(stream);

                var trimmedDataReceived = TrimByteArray(dataReceived.Result);

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Received data: " + trimmedDataReceived.ToHexString());

                var returnPackage = JT808Serializer.Instance.Deserialize(trimmedDataReceived);
                if (returnPackage.Header.MsgId.Equals(JT808MsgId._0x8001))
                {
                    var returnPackage8001 = (JT808_0x8001) returnPackage.Bodies;
                    if (returnPackage8001.JT808PlatformResult.Equals(JT808PlatformResult.succeed))
                    {
                        ///Sending and receiving succeed
                        Console.WriteLine($"Received message: {returnPackage8001.Description}");
                    }
                }
                Console.ResetColor();

            }

            _tcpClient.Stop();

            //Deserilized();

        }

        private void Deserilized()
        {
            var hexString = "7E8300000F013306546928A291013C45585443423C434B42534A3E3E847E";
            var bytesString = hexString.ToHexBytes();
            var bytesData = JT808Serializer.Instance.Deserialize<JT808Package>(bytesString);
            
            hexString = "7E8300000F000912825866A291013C45585443423C434B42534A3E3E027E";
            bytesString = hexString.ToHexBytes();
            var bytesData2 = JT808Serializer.Instance.Deserialize<JT808Package>(bytesString);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && _tcpClient.MessageSent > 0)
            {
                _logger.LogInformation("Number of client connecting {number} at: {time}", _tcpClient.MessageSent, DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }

            Console.ReadLine(); // Keep the server running
        }

        private string Message8801Packaging(string msg)
        {
            var packageBuilder = new JT808Package();

            packageBuilder.Header = new JT808Header
            {
                MsgId = (ushort)JT808MsgId._0x8801,
                ManualMsgNum = 41617,
                TerminalPhoneNo = "13306546928",
            };

            var queryTerminalParams = new JT808_0x8801
            {
                ChannelId = 1,
                Chroma = 1,
                Contrast = 1,
                Lighting = 1,
                Resolution = 1, 
                Saturability = 1,   
                SaveFlag = 1,   
                ShootingCommand = 1,
                VideoQuality = 1,
                VideoTime = 1
            };

            packageBuilder.Bodies = queryTerminalParams;
            packageBuilder.Version = JT808Version.JTT2019;
            //packageBuilder.Begin = packageBuilder.End = 126;
            //packageBuilder.CheckCode = (byte)JT808EncryptMethod.RSA;

            // Serialize the package
            byte[] data = JT808Serializer.Instance.Serialize(packageBuilder);
            var hex = data.ToHexString(); // Output the result in hexadecimal format
            Console.WriteLine($"Prepairing a Hex message by format {packageBuilder.Bodies.ToString()} is: {hex}");
            return hex;
        }

        private string MessagePackaging(string msg)
        {
            var packageBuilder = new JT808Package();

            packageBuilder.Header = new JT808Header
            {
                MsgId = (ushort)JT808MsgId._0x8300,
                ManualMsgNum = 41617,
                TerminalPhoneNo = "13306546928",
            };

            var queryTerminalParams = new JT808_0x8300
            {
                TextType = 0,
                TextFlag = 1,
                TextInfo = msg
            };

            packageBuilder.Bodies = queryTerminalParams;
            packageBuilder.Version = JT808Version.JTT2019;
            //packageBuilder.Begin = packageBuilder.End = 126;
            //packageBuilder.CheckCode = (byte)JT808EncryptMethod.RSA;

            // Serialize the package
            byte[] data = JT808Serializer.Instance.Serialize(packageBuilder);
            var hex = data.ToHexString(); // Output the result in hexadecimal format
            Console.WriteLine($"Prepairing a Hex message by format {packageBuilder.Bodies.ToString()} is: {hex}");
            return hex;
        }

        private byte[] TrimByteArray(byte[] bytes)
        {
            for (var i = bytes.Length - 1; i >= 0; i--)
            {
                if (bytes[i] != 0)
                {
                    Array.Resize(ref bytes, i + 1);
                    break;
                }
            } 

            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgId">Increament steps by steps</param>
        /// <param name="msgNumber">Start at 0 in the order you send it.</param>
        /// <param name="message"></param>
        /// <param name="theSubContract">When the 13th bit in the message body property is 1, it means that the message body is a long message, 
        /// and the subcontracting is carried out. The specific subcontracting information is determined by the package item of the message package. 
        /// If bit 13 is 0, there is no message package wrapper field in the header.</param>
        /// <returns></returns>
        private byte[] MessageEncapsulation(ushort msgId, ushort msgNumber, string message)
        {
            string identify = "7E";
            string checkCode = "00";
            byte theSubContract = (byte)((StringToBytes(message).Length >= 140) ? 1 : 0); // =1 if is long length message.
            var messageBody = new MessageBody()
            {
                Keep = new byte[]{ 0, 0 },
                TheSubContract = theSubContract,
                DataEncryption = new byte[] { 0, 0, 1 }, // = 001 -> checksum with RSA algorithm
                MessageBodyLength = StringToBytes(StringToBytes(message).Length.ToString())
            };
            var hexMsgBody = Convert.ToHexString(MessagePackSerializer.Serialize(messageBody));
            hexMsgBody = EscapeHexString(hexMsgBody.ToUpper());

            var messageHeader = new MessageHeader()
            {
                MessageId = 512, //msgId,
                MessageBodyProperty = 15, //15 bits
                TerminalPhoneNumber = new BcdPhoneNumber
                {
                    CountryCode = 8,
                    AreaCode = 4,
                    MainNumber = 912825866
                },
                MessageNumber = 126,//msgNumber,
                MessagePackageEncapsulated = (Hex2Bytes(hexMsgBody).Length > 140) ? hexMsgBody : ""
            };

            var hexMsgHeader = Convert.ToHexString(MessagePackSerializer.Serialize(messageHeader));
            hexMsgHeader = EscapeHexString(hexMsgHeader.ToUpper());

            var hexPackageNoneCheckSum = $"{hexMsgHeader}{hexMsgBody}";
            checkCode = EscapeHexString(CalculateCheckSum(hexPackageNoneCheckSum));

            var hexPackage = $"{identify}{hexMsgHeader}{hexMsgBody}{checkCode}{identify}";

            return Hex2Bytes(hexPackage);
        }

        private string EscapeHexString(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString)) { return string.Empty; }
            if (hexString.ToUpper().Contains("7E"))
            {
                hexString = hexString.Replace("7D", "7D01", StringComparison.OrdinalIgnoreCase);
                hexString = hexString.Replace("7E", "7D02", StringComparison.OrdinalIgnoreCase);
            }

            return hexString;
        }

        private byte[] StringToBytes(string msg)
        {
            return Encoding.Unicode.GetBytes(msg);
        }

        private byte[] Hex2Bytes(string hexString)
        {
            //return Enumerable.Range(0, hexString.Length / 2).Select(x => Convert.ToByte(hexString.Substring(x * 2, 2), 16)).ToArray();
            return Encoding.Unicode.GetBytes(hexString);
        }

        private string CalculateCheckSum(string hexString)
        {
            // Convert the hexadecimal string to a byte array
            byte[] byteArray = Hex2Bytes(hexString);
            string encryptedHashHexString = string.Empty;

            // Generate an RSA key pair
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Calculate the hash (SHA-256) of the input bytes
                byte[] hashBytes;
                using (SHA256 sha256 = SHA256.Create())
                {
                    hashBytes = sha256.ComputeHash(byteArray);
                }

                // Encrypt the hash with the private key
                byte[] encryptedHash = rsa.SignHash(hashBytes, CryptoConfig.MapNameToOID("SHA256"));

                // Convert the encrypted hash to a hexadecimal string
                encryptedHashHexString = BitConverter.ToString(encryptedHash).Replace("-", "");
                //Console.WriteLine($"Checksum calculated with Hash (RSA signature): {encryptedHashHexString}");
            }
            return encryptedHashHexString;
        }
    }

}

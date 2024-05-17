using MessagePack;

namespace CommunicationProtocol
{
    [Serializable]
    [MessagePackObject]
    public class BcdPhoneNumber
    {
        [Key(0)]
        public byte CountryCode { get; set; } // Mã quốc gia (1 byte)
        [Key(1)]
        public byte AreaCode { get; set; } // Mã vùng (1 byte)
        [Key(2)]
        public uint MainNumber { get; set; } // Số điện thoại chính (4 bytes)
    }

    [Serializable]
    [MessagePackObject]
    public class MessageHeader
    {
        [Key(0)]
        public ushort MessageId {  get; set; } // The WORD
        [Key(1)]
        public ushort MessageBodyProperty {  get; set; } // The WORD
        [Key(2)]
        public BcdPhoneNumber? TerminalPhoneNumber { get; set; }
        [Key(3)]
        public ushort MessageNumber {  get; set; }
        [Key(4)]
        public string? MessagePackageEncapsulated {  get; set; }
    }

    [Serializable]
    [MessagePackObject]
    public class MessageBody
    {
        [Key(0)]
        public byte[]? Keep { get; set; } // 2 bytes
        [Key(1)]
        public byte TheSubContract { get; set; } // 1 bytes
        [Key(2)]
        public byte[]? DataEncryption { get; set; } // 3 bytes
        [Key(3)]
        public byte[]? MessageBodyLength { get; set; } // 10 bytes
    }

    [Serializable]
    [MessagePackObject]
    public class Package
    {
        [Key(0)]
        public byte IdentifyFirst { get; set; }
        [Key(1)]
        public MessageHeader? Header { get; set; }
        [Key(2)]
        public MessageBody? Body { get; set; }
        [Key(3)]
        public byte CheckCode { get; set; }
        [Key(4)]
        public byte IdentifyLast { get; set; }

    }
}

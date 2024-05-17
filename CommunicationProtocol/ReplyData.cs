
namespace CommunicationProtocol
{
    #region "Reply package"
    public class UniversalReplyBody
    {
        /// <summary>
        /// TerminalUniversal => The current number of the corresponding platform message.
        /// PlatformUniversal => The current number of the corresponding terminal message.
        /// </summary>
        public ushort AnswerSerialNumber { get; set; }
        /// <summary>
        /// TerminalUniversal => 0: success/confirmation; 1. Failure; 2. Wrong message; 3: no
        /// PlatformUniversal => The ID of the corresponding terminal message.
        /// </summary>
        public short ResponseId { get; set; }

        /// <summary>
        /// TerminalUniversal => The ID of the corresponding platform message.
        /// PlatformUniversal => The ID of the corresponding terminal message.
        /// </summary>
        public byte ResultOf { get; set; }
    }

    public class RegisteredReplyBody
    {
        /// <summary>
        ///The current number of the corresponding terminal registration message
        /// </summary>
        public ushort AnswerSerialNumber { get; set; }
        /// <summary>
        /// This field is only available after success.
        /// </summary>
        public string? AuthenticationCode { get; set; }

        /// <summary>
        /// 0: success; 1. The vehicle has been registered; 2: there is no such vehicle in the database; 
        /// 3. The terminal has been registered; 4: there is no such terminal in the database.
        /// </summary>
        public byte ResultOf { get; set; }
    }

    public class AuthenticationReplyBody
    {
        /// <summary>
        /// The terminal is reconnected and the verification code is reported.
        /// </summary>
        public string? AuthenticationCode { get; set; }
    }

    public class TerminalReplyBody
    {
        /// <summary>
        ///The current number of the corresponding terminal registration message
        /// </summary>
        public ushort AnswerSerialNumber { get; set; }
        /// <summary>
        /// This field is only available after success.
        /// </summary>
        public byte ResponseNUmber { get; set; }

        /// <summary>
        /// 0: success; 1. The vehicle has been registered; 2: there is no such vehicle in the database; 
        /// 3. The terminal has been registered; 4: there is no such terminal in the database.
        /// </summary>
        public List<ParameterItem<object>>? ParameterItemList { get; set; }
    }

    public class TerminalAttributesReplyBody
    {
        public ushort TerminalType { get; set; }
        public byte[]? ManufacturerId { get; set; }
        /// <summary>
        /// 20 bytes. The terminal model is defined by the manufacturer, and "0X00" is added when the number is insufficient.
        /// </summary>
        public byte[]? TerminalModel { get; set; }
        public byte[]? TerminalId { get; set; }
        public BcdPhoneNumber? TerminalSimCardIccId { get; set; }
        public byte TerminalHardwareVersionNumberLength { get; set; }
        public string? TerminalHardwareVersionNumber { get; set; }
        public byte TerminalFirmwareVersionNumberLength { get; set; }
        public string? TerminalFirmwareVersionNumber { get; set; }
        public byte GnssModuleProperties { get; set; }
        public byte CommunicationModuleAttribute { get; set; }
    }

    public class IssueTerminalUpgradeReplyBody
    {
        public byte UpgradeType { get; set; }
        public byte[]? ManufactorerId { get; set; }
        public byte VersionLength { get; set; }
        public string? VersionNumber { get; set; }
        public uint UpgradePackageLengthInUInt { get; set; }
        public byte UpgradePackageLengthInByte { get; set; }
    }

    public class LocationInformationQueryReplyBody
    {
        public ushort AnswerSerialNumber { get; set; }
        public LocationInformationReport? LocationInformationReportObject { get; set; }
    }

    public class TerminalNotificationUpgradeMessage
    {
        public byte UpgradeType { get; set; }
        public byte Upgrade { get; set; }
    }

    public class TerminalControlMessage
    {
        public byte CommandWork { get; set; }
        public string? CommandParamter { get; set; }

    }

    public partial class Package<T>
    {
        public ushort MessageId { get; set; }
        public T? MessageBody { get; set; }
    }

    public class TerminalUniversal : Package<UniversalReplyBody>
    {
        public TerminalUniversal() {
            MessageId = 0x0001;
        }
    }

    public class PlatformUniversal : Package<UniversalReplyBody>
    {
        public PlatformUniversal()
        {
            MessageId = 0x8001;
        }
    }

    public class TerminalHeartbeat : Package<UniversalReplyBody>
    {
        public TerminalHeartbeat()
        {
            MessageId = 0x0002;
            MessageBody = null;
        }
    }

    #endregion

    #region "Supplementary subcontract request"

    public class SupplementarySubContractRequestBody
    {
        //The message flow number corresponding to the first packet of the original message that is requested to be forwarded.
        public ushort OriginalMessageStreamNumber { get; set; }
        // N.
        public byte TotalReTransmittedPackageNumber { get; set; }
        //Retransmit package number sequence, such as "package  ID1 package ID2...Package the IDn."
        public byte[]? RetransmitPackageIdList { get; set; }
    }

    public class TerminalRegistrationRequestBody
    {
        /// <summary>
        //Mark the province where the terminal installation vehicle is located, 0 is reserved and the default value is taken by the platform.
        //The provincial ID shall be the first two of the six administrative division codes specified in GB/T 2260.
        /// </summary>
        public ushort ProvincialId { get; set; }
        /// <summary>
        /// Mark the city and county where the terminal installation vehicle is located, 0 is reserved and the default value is taken by the platform.
        /// The ID of the city and county shall adopt the code of administrative division stipulated in GB/T 2260.
        /// </summary>
        public ushort CityCountyId { get; set; }
        /// <summary>
        /// 5 bytes, terminal manufacturer code.
        /// </summary>
        public byte[]? ManufacturerId { get; set; }
        /// <summary>
        /// 20 bytes. This terminal model is defined by the manufacturer.In case of insufficient digits, "0X00" is added.
        /// </summary>
        public byte[]? TerminalType { get; set; }
        /// <summary>
        /// 7 bytes. This terminal ID is defined by the manufacturer.In case of insufficient digits, "0X00" is added.
        /// </summary>
        public byte[]? TerminalId { get; set; }
        /// <summary>
        /// The license plate color is 5.4.12 according to JT/ t415-2006.When the card is not played, the value is 0.
        /// </summary>
        public byte LicensePlateColor {  get; set; }
        /// <summary>
        /// When the color of the license plate is 0, it means the vehicle VIN; Otherwise, 
        /// it represents the motor vehicle license plate issued by the public security traffic administration department.
        /// </summary>
        public string? VehicleIdentification {  get; set; }
    }

    public class SupplementarySubContract : Package<SupplementarySubContractRequestBody>
    {
        public SupplementarySubContract()
        {
            MessageId = 0x8003;
        }
    }

    public class TerminalRegistration : Package<TerminalRegistrationRequestBody>
    {
        public TerminalRegistration()
        {
            MessageId = 0x0100;
        }
    }

    public class TerminalRegistered : Package<RegisteredReplyBody>
    {
        public TerminalRegistered() {
            MessageId = 0x8100;
        }
    }

    public class TerminalLogout : Package<RegisteredReplyBody>
    {
        public TerminalLogout()
        {
            MessageId = 0x0003;
            MessageBody = null;
        }
    }

    public class TerminalAuthentication : Package<AuthenticationReplyBody>
    {
        public TerminalAuthentication()
        {
            MessageId = 0x0102;
        }
    }

    #endregion

    #region "Set terminal parameters"
    public class SetBody
    {
        public byte TotalParameterNumber { get; set; }

        public List<ParameterItem<object>>? ParameterItemList { get; set; }

    }

    public class ParameterItem<T>
    { 
        public uint ParameterId {  get; set; }
        public byte ParameterLength {  get; set; }
        public T? ParameterValue { get; set; }

        public ParameterItem(T value)
        {
            ParameterValue = value;
        }

        public T GetParameterItem()
        {           
            if (ParameterId.Equals(0x0001)) {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0002))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0003))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0004))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0005))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0010))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(string));
            }
            else if (ParameterId.Equals(0x0011))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(string));
            }
            else if (ParameterId.Equals(0x0012))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(string));
            }
            else if (ParameterId.Equals(0x0013))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(string));
            }
            else if (ParameterId.Equals(0x0014))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(string));
            }
            else if (ParameterId.Equals(0x0015))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(string));
            }
            else if (ParameterId.Equals(0x0016))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(string));
            }
            else if (ParameterId.Equals(0x0017))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(string));
            }
            else if (ParameterId.Equals(0x0018))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0019))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0020))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0027))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0028))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0029))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0030))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0041))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(string));
            }
            else if (ParameterId.Equals(0x0042))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(string));
            }
            else if (ParameterId.Equals(0x0049))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(string));
            }
            else if (ParameterId.Equals(0x0055))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0056))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0057))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0058))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x005b))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(ushort));
            }
            else if (ParameterId.Equals(0x005c))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(ushort));
            }
            else if (ParameterId.Equals(0x0080))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0081))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(ushort));
            }
            else if (ParameterId.Equals(0x0082))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(ushort));
            }
            else if (ParameterId.Equals(0x0083))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(string));
            }
            else if (ParameterId.Equals(0x0084))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(byte));
            }
            else if (ParameterId.Equals(0x0100))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(uint));
            }
            else if (ParameterId.Equals(0x0101))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(ushort));
            }
            else if (ParameterId.Equals(0x0110))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(byte[]));
            }
            else if (ParameterId.Equals(0x8501))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(byte[]));
            }
            else if (ParameterId.Equals(0x8503))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(byte[]));
            }
            else if (ParameterId.Equals(0xf011))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(byte[]));
            }
            else if (ParameterId.Equals(0xf012))
            {
                return (T)Convert.ChangeType(ParameterId, typeof(byte[]));
            }
            else
                return (T)Convert.ChangeType(ParameterId, typeof(T));
        }

        public void SetParameterItem(T value)
        {
            ParameterValue = value;            
        }
    }

    public class TerminalParameters : Package<SetBody>
    {
        public TerminalParameters()
        {
            MessageId = 0x8103;
        }
    }

    public class QueryTerminalParameters : Package<SetBody>
    {
        public QueryTerminalParameters()
        {
            MessageId = 0x8104;
            MessageBody = null;
        }
    }

    public class QueryTerminalParameterReply : Package<TerminalReplyBody>
    {
        public QueryTerminalParameterReply()
        {
            MessageId = 0x0104;
        }
    }

    public class TerminalControl : Package<TerminalControlMessage>
    {
        public TerminalControl()
        {
            MessageId = 0x8105;
        }
    }

    public class QueryTerminalProperties : Package<SetBody>
    {
        public QueryTerminalProperties()
        {
            MessageId = 0x8107;
            MessageBody = null;
        }
    }

    public class QueryTerminalAttributes : Package<TerminalAttributesReplyBody>
    {
        public QueryTerminalAttributes()
        {
            MessageId = 0x0107;
        }
    }

    public class IssueTerminalUpgrade : Package<IssueTerminalUpgradeReplyBody>
    {
        public IssueTerminalUpgrade()
        {
            MessageId = 0x8108;
        }
    }

    public class TerminalNotificationUpgradeResult : Package<TerminalNotificationUpgradeMessage>
    {
        public TerminalNotificationUpgradeResult()
        {
            MessageId = 0x0108;
        }
    }

    #endregion

    #region "Location information report"
    public class LocationAdditionalInformation
    {
        public LocationBasicInfo? LocationBasicInformation { get; set; }

        public List<LocationAdditionalItem<object>>? LocationAdditionalItemList { get; set; }

    }

    public class LocationBasicInfo
    {
        public uint WarningMark { get; set; }
        public uint State { get; set; }
        public uint Latitude { get; set; }
        public uint Longitude { get; set; }
        public ushort Elevation { get; set; }
        public ushort Speed { get; set; }
        public ushort DirectionOf { get; set; }
        public BcdPhoneNumber? Time { get; set; }    
    }

    public class LocationAdditionalItem<T>
    {
        public byte AdditionalInformationId { get; set; }
        public byte AdditionalInformationLength { get; set; }
        public T? AdditionalInformation { get; set; }

        public LocationAdditionalItem(T value)
        {
            AdditionalInformation = value;
        }

        public T GetParameterItem()
        {
            if (AdditionalInformationId.Equals(0x01))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(uint));
            }
            else if (AdditionalInformationId.Equals(0x02))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(ushort));
            }
            else if (AdditionalInformationId.Equals(0x03))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(ushort));
            }
            else if (AdditionalInformationId.Equals(0x04))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(ushort));
            }
            else if (AdditionalInformationId.Equals(0x25))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(uint));
            }
            else if (AdditionalInformationId.Equals(0x2a))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(uint));
            }
            else if (AdditionalInformationId.Equals(0x2b))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(uint));
            }
            else if (AdditionalInformationId.Equals(0x30))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(byte));
            }
            else if (AdditionalInformationId.Equals(0x31))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(byte));
            }
            else if (AdditionalInformationId.Equals(0xe3))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(byte[]));
            }
            else if (AdditionalInformationId.Equals(0xf3))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(byte));
            }
            else if (AdditionalInformationId.Equals(0xd0))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(uint));
            }
            else if (AdditionalInformationId.Equals(0xd1))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(uint));
            }
            else if (AdditionalInformationId.Equals(0xd2))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(uint));
            }
            else if (AdditionalInformationId.Equals(0xd3))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(uint));
            }
            else if (AdditionalInformationId.Equals(0xd4))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(uint));
            }
            else if (AdditionalInformationId.Equals(0xd5))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(uint));
            }
            else if (AdditionalInformationId.Equals(0xd6))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(uint));
            }
            else if (AdditionalInformationId.Equals(0xc0))
            {
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(uint));
            }
            else
                return (T)Convert.ChangeType(AdditionalInformationId, typeof(T));
        }

        public void SetParameterItem(T value)
        {
            AdditionalInformation = value;
        }
    }

    public class LocationInformationReport : Package<List<LocationAdditionalInformation>>
    {
        public LocationInformationReport()
        {
            MessageId = 0x0200;
        }
    }

    public class LocationInformationQuery : Package<LocationAdditionalInformation>
    {
        public LocationInformationQuery()
        {
            MessageId = 0x8201;
            MessageBody = null;
        }
    }

    public class LocationInformationQueryReply : Package<LocationInformationQueryReplyBody>
    {
        public LocationInformationQueryReply() 
        {
            MessageId = 0x0201;
        }
    }

    #endregion

    #region "Temporary position tracking control"

    public class TemporaryPositionTrackingMessageBody
    {
        public ushort TimeInterval {  get; set; }
        public uint LocationTrackingExpiryDate { get; set; }
    }    

    public class TemporaryPositionTrackingControl : Package<TemporaryPositionTrackingControl>
    {
        public TemporaryPositionTrackingControl()
        {
            MessageId = 0x8202;
        }
    }
    #endregion

    #region "Data downlink passthrough"

    public class DataDownlinkPassthroughMessageBody
    {
        public byte PassthroughMessageType { get; set; }
        public byte[]? PassMessageContent { get; set; }
    }

    public class DataDownlinkPassthrough : Package<DataDownlinkPassthroughMessageBody>
    {
        public DataDownlinkPassthrough()
        {
            MessageId = 0x8900;
        }
    }
    #endregion

    #region "Data uplink passthrough"

    public class DataUplinkPassthroughMessageBody
    {
        public byte PassthroughMessageType { get; set; }
        public byte[]? PassMessageContent { get; set; }
    }

    public class DataUplinkPassthrough : Package<DataUplinkPassthroughMessageBody>
    {
        public DataUplinkPassthrough()
        {
            MessageId = 0x0900;
        }
    }

    public class TerminalCalibrationRequestUplink : Package<DataUplinkPassthroughMessageBody>
    {
        public TerminalCalibrationRequestUplink()
        {
            MessageId = 0x0F01;
            MessageBody = null;
        }
    }

    #endregion

    #region "Server calibration reply"

    public class ServerCalibrationReplyBody
    {
        public byte ResultOf { get; set; }
        public BcdPhoneNumber? Time { get; set; }
    }

    public class ServerCalibrationReply : Package<ServerCalibrationReplyBody>
    {
        public ServerCalibrationReply()
        {
            MessageId = 0x8F01;
        }
    }
    #endregion

    #region "Text message sent"

    public class TextMessageSentBody
    {
        public byte Mark { get; set; }
        public string? TextInformation { get; set; }
    }

    public class TextMessageSent : Package<TextMessageSentBody>
    {
        public TextMessageSent()
        {
            MessageId = 0x8300;
        }
    }
    #endregion

    #region "Text message uplink"

    public class TextMessageUplinkBody
    {
        //public byte Mark { get; set; }
        public string? TextInformation { get; set; }
    }

    public class TextMessageUplink : Package<TextMessageUplinkBody>
    {
        public TextMessageUplink()
        {
            MessageId = 0x0300;
        }
    }
    #endregion

    #region "Vehicle control"

    public class VehicleControlBody
    {
        //public byte Mark { get; set; }
        public byte[]? ControlTheLogo { get; set; }
    }

    public class VehicleControl : Package<VehicleControlBody>
    {
        public VehicleControl()
        {
            MessageId = 0x8500;
        }
    }
    #endregion

    #region "Vehicle control response"

    public class VehicleControlResponseBody
    {
        public ushort AnswerSerialNumber { get; set; }
        public byte[]? LocationInfoReportedToMessageBody { get; set; }
    }

    public class VehicleControlResponse : Package<VehicleControlResponseBody>
    {
        public VehicleControlResponse()
        {
            MessageId = 0x0500;
        }
    }
    #endregion

    #region "Recording function related instructions"

    public class RecordingTimeSentByServerBody
    {
        public byte RecordingTime { get; set; }
        public string? SessionId { get; set; }
    }

    public class RecordingTimeSentByServer : Package<RecordingTimeSentByServerBody>
    {
        public RecordingTimeSentByServer()
        {
            MessageId = 0x8116;
        }
    }

    public class DeviceReplyRecordingBody
    {
        public byte RecordingState { get; set;}
        public string? SessionId { get; set; }
    }

    public class DeviceReplyRecording : Package<DeviceReplyRecordingBody>
    {
        public DeviceReplyRecording()
        {
            MessageId = 0x0116;
        }
    }


    public class DeviceReportsRecordingDataBody
    {
        public byte TotalPackageNumber { get; set; }
        public byte PackageNumber { get; set; }
        public string? SessionId { get; set; }
        public BcdPhoneNumber? Time {  get; set; }
        public string? RecordingFile { get; set;}
    }

    public class DeviceReportsRecordingData : Package<DeviceReportsRecordingDataBody>
    {
        public DeviceReportsRecordingData()
        {
            MessageId = 0x0118;
        }
    }


    public class ServerSendsRecordedDataReplyBody
    {
        public byte CurrentPackageSerialNumber { get; set; }
        public string? SessionId { get; set; }
        public BcdPhoneNumber? Time { get; set; }
    }

    public class ServerSendsRecordedDataReply : Package<ServerSendsRecordedDataReplyBody>
    {
        public ServerSendsRecordedDataReply()
        {
            MessageId = 0x8118;
        }
    }


    public class DeviceRecordingDataUploadReportCompletionNotificationBody
    {
        public string? SessionId { get; set; }
    }

    public class DeviceRecordingDataUploadReportCompletionNotification : Package<DeviceRecordingDataUploadReportCompletionNotificationBody>
    {
        public DeviceRecordingDataUploadReportCompletionNotification()
        {
            MessageId = 0x0119;
        }
    }

    public class TerminalReplyRecordingCancelledBody
    {
        public byte CancelTheResult { get; set; }
        public string? SessionId { get; set; }
    }

    public class TerminalReplyRecordingCancelled : Package<TerminalReplyRecordingCancelledBody>
    {
        public TerminalReplyRecordingCancelled()
        {
            MessageId = 0x0115;
        }
    }

    #endregion

}

using System;

using Autodrome.Base;

namespace Autodrome.Navigation
{
   public class NovatelEventArgs : EventArgs
   {
      public Novatel.Message Message;
      public NovatelEventArgs(Novatel.Message message)
      {
         Message = message;
      }
   }

   public class Novatel
   {
      public enum MessageID : ushort
      {
         BestPos = 42,
         BestVel = 99,
         RTKPos = 141,
         PassCom1 = 233,
         PassCom2 = 234,
         PassCom3 = 235,
         BestXYZ = 241,
         PassXCom1 = 405,
         PassXCom2 = 406,
         PassUSB1 = 607,
         PassUSB2 = 608,
         PassUSB3 = 609,
         BslnXYZ = 686,
         PassAUX = 690,
         BestUTM = 726,
         Heading = 971,
         PassXCom3 = 795,
         MasterPos = 1051,
      }

      [FlagsAttribute]
      public enum MessageType : byte
      {
         None = 0,
         ASCII = 1 << 5,
         NMEA = 1 << 6,
         UndefinedFormat = ASCII | NMEA,
         Response = 1 << 7,
      }

      public enum TimeStatus : byte
      {
         Unknown = 20,
         Approximate = 60,
         CoarseAdjusting = 80,
         Coarse = 100,
         CoarseSteering = 120,
         FreeWheeling = 130,
         FineAdjusting = 140,
         Fine = 160,
         FineSteering = 180,
         SatTime = 200,
      }

      public enum SolutionStatus : ushort
      {
         Computed = 0,
         InsufficientObservations = 1,
         NoCovergence = 2,
         Singularity = 3,
         CovarianceTrace = 4,
         TestDistance = 5,
         ColdStart = 6,
         LimitsExceeded = 7,
         Variance = 8,
         Residuals = 9,
         DeltaPosition = 10,
         NegativeVariance = 11,
         //Reserved          = 12,
         IntegrityWarning = 13,
         //INS values        = 14-17,
         Pending = 18,
         InvalidFix = 19,
         Unauthorized = 20,
         AntennaWarning = 21,
      }

      [FlagsAttribute]
      public enum ExtraSolutionStatus : byte
      {
         None = 0,

         AdVanceRTK = 1 << 0,

         KlobucharBroadcast = 1 << 1,
         SBAS = 1 << 2,
         MultiFrequency = KlobucharBroadcast | SBAS,
         PseudorangeDifferential = 1 << 3,
         NovatelBlendedIonoValue = KlobucharBroadcast | PseudorangeDifferential,
      }

      public enum PositionType : byte
      {
         None = 0,
         FixedPosition = 1,
         FixedHeight = 2,
         DopplerVelocity = 8,
         Single = 16,
         PseudorangeDifferential = 17,
         WAAS = 18,
         Propagated = 19,
         OmniStar = 20,
         L1Float = 32,
         IonoFreeFloat = 33,
         NarrowFloat = 34,
         L1Integer = 48,
         WideInteger = 49,
         NarrowInteger = 50,
         RTKDirectINS = 51,
         INS = 52,
         INS_PseudorangeSP = 53,
         INS_PseudorangeDifferential = 54,
         INS_RTKFloat = 55,
         INS_RTKFixed = 56,
         OmniStar_HP = 64,
         OmniStar_XP = 65,
         CDGPS = 66,
      }

      public enum DatumID : byte
      {
      }

      [FlagsAttribute]
      public enum SignalMask
      {
         None = 0,
         L1 = 1 << 0,
         L2 = 1 << 1,
         L5 = 1 << 2,
         G1 = 1 << 4,
         G2 = 1 << 5,
      }

      public class MessageHeader
      {
         public Byte HeaderLength;
         public MessageID MessageID;
         public MessageType MessageType;
         public Byte PortAddress;
         public UInt16 MessageLength;
         public UInt16 Sequence;
         public Byte Idle;
         public TimeStatus TimeStatus;
         public UInt16 Week;
         public UInt32 MS;
         public UInt32 ReceiverStatus;
         public Byte reserved;
         public UInt16 FirmwareVersion;
      }

      public class Message
      {
         public MessageHeader Header;
      }

      public class PositionMessage : Message
      {
         public SolutionStatus SolutionStatus;
         public PositionType PositionType;
         public Double Latitude;
         public Double Longitude;
         public Double Height;
         public Single Undulation;
         public DatumID DatumID;
         public Single LatitudeDeviation;
         public Single LongitudeDeviation;
         public Single HeightDeviation;
         public UInt32 StationID;
         public Single DifferentialAge;
         public Single SolutionAge;
         public Byte SatelliteCount;
         public Byte SatellitesUsed;
         public Byte L1Used;
         public Byte L1L2Used;
         public Byte reserved1;
         public ExtraSolutionStatus ExtraSolutionStatus;
         public Byte reserved2;
         public SignalMask SignalMask;
      }

      public class UTMMessage : Message
      {
         public SolutionStatus SolutionStatus;
         public PositionType PositionType;
         public UInt32 ZoneNumber;
         public UInt32 ZoneLetter;
         public Double Northing;
         public Double Easting;
         public Double Height;
         public Single Undulation;
         public DatumID DatumID;
         public Single NorthingDeviation;
         public Single EastingDeviation;
         public Single HeightDeviation;
         public UInt32 StationID;
         public Single DifferentialAge;
         public Single SolutionAge;
         public Byte SatelliteCount;
         public Byte SatellitesUsed;
         public Byte L1Used;
         public Byte L1L2Used;
         public Byte reserved1;
         public ExtraSolutionStatus ExtraSolutionStatus;
         public Byte reserved2;
         public SignalMask SignalMask;
      }

      public class BaselineXYZMessage : Message
      {
         public SolutionStatus SolutionStatus;
         public PositionType PositionType;
         public Double X;
         public Double Y;
         public Double Z;
         public Single XDeviation;
         public Single YDeviation;
         public Single ZDeviation;
         public UInt32 StationID;
         public Byte SatelliteCount;
         public Byte SatellitesUsed;
         public Byte L1Used;
         public Byte L1L2Used;
         public Byte reserved1;
         public ExtraSolutionStatus ExtraSolutionStatus;
         public Byte reserved2;
         public SignalMask SignalMask;
      }

      public class XYZMessage : Message
      {
         public SolutionStatus SolutionStatus;
         public PositionType PositionType;
         public Double PositionX;
         public Double PositionY;
         public Double PositionZ;
         public Single PositionXDeviation;
         public Single PositionYDeviation;
         public Single PositionZDeviation;
         public SolutionStatus Velocity_status;
         public PositionType Velocity_type;
         public Double VelocityX;
         public Double VelocityY;
         public Double VelocityZ;
         public Single VelocityXDeviation;
         public Single VelocityYDeviation;
         public Single VelocityZDeviation;
         public UInt32 StationID;
         public Single Velocity_latency;
         public Single DifferentialAge;
         public Single SolutionAge;
         public Byte SatelliteCount;
         public Byte SatellitesUsed;
         public Byte L1Used;
         public Byte L1L2Used;
         public Byte reserved1;
         public ExtraSolutionStatus ExtraSolutionStatus;
         public Byte reserved2;
         public SignalMask SignalMask;
      }

      public class HeadingMessage : Message
      {
         public SolutionStatus SolutionStatus;
         public PositionType PositionType;
         public Single Length;
         public Single Heading;
         public Single Pitch;
         public Single reserved1;
         public Single HeadingDeviation;
         public Single PitchDeviation;
         public UInt32 StationID;
         public Byte SatelliteCount;
         public Byte SatellitesUsed;
         public Byte SatellitesAboveMask;
         public Byte SatellitesAboveMaskL2;
         public ExtraSolutionStatus ExtraSolutionStatus;
         public SignalMask SignalMask;
      }

      public class PassMessage : Message
      {
         public UInt32 Size;
         public Byte[] Bytes;
      }
   }

   public class NovatelSpliceBuffer : global::Autodrome.Base.SignatureSpliceBuffer
   {
      public NovatelSpliceBuffer(int size)
         : base(new byte[] { 0xAA, 0x44, 0x12 }, size)
      {
         PacketReceived += packetReceived;
      }

      protected override bool PacketReady(int position)
      {
         int bytes_left = Offset - position;
         return (bytes_left > 28) && (bytes_left >= PacketSize(position));
      }
      protected override bool PacketValid(int position)
      {
         int data_size = PacketSize(position) - 4;
         UInt32 crc_calculated = CalculateCRC(Data, position, data_size);
         UInt32 crc_written = BitConverter.ToUInt32(Data, position + data_size);
         return crc_calculated == crc_written;
      }
      protected override int PacketSize(int position)
      {
         int result = (Int16)Data[position + 3] + BitConverter.ToInt16(Data, position + 8) + 4;
         return result;
      }

      public event EventHandler<NovatelEventArgs> MessageReceived;

      void packetReceived(object sender, DataEventArgs args)
      {
         byte[] data = args.Data;
         int offset = args.Offset;

         Novatel.MessageHeader h = new Novatel.MessageHeader();

         h.HeaderLength = data[offset + 3];
         h.MessageID = (Novatel.MessageID)BitConverter.ToUInt16(data, offset + 4);
         h.MessageType = (Novatel.MessageType)data[offset + 6];
         h.PortAddress = data[offset + 7];
         h.MessageLength = BitConverter.ToUInt16(data, offset + 8);
         h.Sequence = BitConverter.ToUInt16(data, offset + 10);
         h.Idle = data[offset + 12];
         h.TimeStatus = (Novatel.TimeStatus)data[offset + 13];
         h.Week = BitConverter.ToUInt16(data, offset + 14);
         h.MS = BitConverter.ToUInt32(data, offset + 16);
         h.ReceiverStatus = BitConverter.ToUInt32(data, offset + 20);
         h.FirmwareVersion = BitConverter.ToUInt16(data, offset + 26);

         Novatel.Message result;

         offset += h.HeaderLength;

         switch (h.MessageID)
         {
            case Novatel.MessageID.BestPos:
            case Novatel.MessageID.MasterPos:
            case Novatel.MessageID.RTKPos:
               {
                  Novatel.PositionMessage m = new Novatel.PositionMessage();
                  m.SolutionStatus = (Novatel.SolutionStatus)BitConverter.ToUInt32(data, offset);
                  m.PositionType = (Novatel.PositionType)BitConverter.ToUInt32(data, offset + 4);
                  m.Latitude = BitConverter.ToDouble(data, offset + 8);
                  m.Longitude = BitConverter.ToDouble(data, offset + 16);
                  m.Height = BitConverter.ToDouble(data, offset + 24);
                  m.Undulation = BitConverter.ToSingle(data, offset + 32);
                  m.DatumID = (Novatel.DatumID)BitConverter.ToUInt32(data, offset + 36);
                  m.LatitudeDeviation = BitConverter.ToSingle(data, offset + 40);
                  m.LongitudeDeviation = BitConverter.ToSingle(data, offset + 44);
                  m.HeightDeviation = BitConverter.ToSingle(data, offset + 48);
                  m.StationID = BitConverter.ToUInt32(data, offset + 52);
                  m.DifferentialAge = BitConverter.ToSingle(data, offset + 56);
                  m.SolutionAge = BitConverter.ToSingle(data, offset + 60);
                  m.SatelliteCount = data[offset + 64];
                  m.SatellitesUsed = data[offset + 65];
                  m.L1Used = data[offset + 66];
                  m.L1L2Used = data[offset + 67];
                  m.ExtraSolutionStatus = (Novatel.ExtraSolutionStatus)data[offset + 69];
                  m.SignalMask = (Novatel.SignalMask)data[offset + 71];
                  result = m;
               }
               break;
            case Novatel.MessageID.BestUTM:
               {
                  Novatel.UTMMessage m = new Novatel.UTMMessage();
                  m.SolutionStatus = (Novatel.SolutionStatus)BitConverter.ToUInt32(data, offset);
                  m.PositionType = (Novatel.PositionType)BitConverter.ToUInt32(data, offset + 4);
                  m.ZoneNumber = BitConverter.ToUInt32(data, offset + 8);
                  m.ZoneLetter = BitConverter.ToUInt32(data, offset + 12);
                  m.Northing = BitConverter.ToDouble(data, offset + 16);
                  m.Easting = BitConverter.ToDouble(data, offset + 24);
                  m.Height = BitConverter.ToDouble(data, offset + 32);
                  m.Undulation = BitConverter.ToSingle(data, offset + 40);
                  m.DatumID = (Novatel.DatumID)BitConverter.ToUInt32(data, offset + 44);
                  m.NorthingDeviation = BitConverter.ToSingle(data, offset + 48);
                  m.EastingDeviation = BitConverter.ToSingle(data, offset + 52);
                  m.HeightDeviation = BitConverter.ToSingle(data, offset + 56);
                  m.StationID = BitConverter.ToUInt32(data, offset + 60);
                  m.DifferentialAge = BitConverter.ToSingle(data, offset + 64);
                  m.SolutionAge = BitConverter.ToSingle(data, offset + 68);
                  m.SatelliteCount = data[offset + 72];
                  m.SatellitesUsed = data[offset + 73];
                  m.L1Used = data[offset + 74];
                  m.L1L2Used = data[offset + 75];
                  m.ExtraSolutionStatus = (Novatel.ExtraSolutionStatus)data[offset + 77];
                  m.SignalMask = (Novatel.SignalMask)data[offset + 79];
                  result = m;
               }
               break;
            case Novatel.MessageID.Heading:
               {
                  Novatel.HeadingMessage m = new Novatel.HeadingMessage();
                  m.SolutionStatus = (Novatel.SolutionStatus)BitConverter.ToUInt32(data, offset);
                  m.PositionType = (Novatel.PositionType)BitConverter.ToUInt32(data, offset + 4);
                  m.Length = BitConverter.ToSingle(data, offset + 8);
                  m.Heading = BitConverter.ToSingle(data, offset + 12);
                  m.Pitch = BitConverter.ToSingle(data, offset + 16);
                  m.HeadingDeviation = BitConverter.ToSingle(data, offset + 24);
                  m.PitchDeviation = BitConverter.ToSingle(data, offset + 28);
                  m.StationID = BitConverter.ToUInt32(data, offset + 32);
                  m.SatelliteCount = data[offset + 36];
                  m.SatellitesUsed = data[offset + 37];
                  m.SatellitesAboveMask = data[offset + 38];
                  m.SatellitesAboveMaskL2 = data[offset + 39];
                  m.ExtraSolutionStatus = (Novatel.ExtraSolutionStatus)data[offset + 41];
                  m.SignalMask = (Novatel.SignalMask)data[offset + 43];
                  result = m;
               }
               break;
            case Novatel.MessageID.BslnXYZ:
               {
                  Novatel.BaselineXYZMessage m = new Novatel.BaselineXYZMessage();
                  m.SolutionStatus = (Novatel.SolutionStatus)BitConverter.ToUInt32(data, offset);
                  m.PositionType = (Novatel.PositionType)BitConverter.ToUInt32(data, offset + 4);
                  m.X = BitConverter.ToDouble(data, offset + 8);
                  m.Y = BitConverter.ToDouble(data, offset + 16);
                  m.Z = BitConverter.ToDouble(data, offset + 24);
                  m.XDeviation = BitConverter.ToSingle(data, offset + 32);
                  m.YDeviation = BitConverter.ToSingle(data, offset + 36);
                  m.ZDeviation = BitConverter.ToSingle(data, offset + 40);
                  m.StationID = BitConverter.ToUInt32(data, offset + 44);
                  m.SatelliteCount = data[offset + 48];
                  m.SatellitesUsed = data[offset + 49];
                  m.L1Used = data[offset + 50];
                  m.L1L2Used = data[offset + 51];
                  m.ExtraSolutionStatus = (Novatel.ExtraSolutionStatus)data[offset + 53];
                  m.SignalMask = (Novatel.SignalMask)data[offset + 55];
                  result = m;
               }
               break;
            case Novatel.MessageID.PassAUX:
            case Novatel.MessageID.PassCom1:
            case Novatel.MessageID.PassCom2:
            case Novatel.MessageID.PassCom3:
            case Novatel.MessageID.PassUSB1:
            case Novatel.MessageID.PassUSB2:
            case Novatel.MessageID.PassUSB3:
            case Novatel.MessageID.PassXCom1:
            case Novatel.MessageID.PassXCom2:
            case Novatel.MessageID.PassXCom3:
               {
                  Novatel.PassMessage m = new Novatel.PassMessage();
                  m.Size = BitConverter.ToUInt32(data, offset);
                  m.Bytes = new byte[m.Size];
                  Array.Copy(data, offset + 4, m.Bytes, 0, m.Size);
                  result = m;
               }
               break;
            default:
               {
                  result = new Novatel.Message();
               }
               break;
         }
         result.Header = h;
         if (MessageReceived != null)
         {
            MessageReceived(this, new NovatelEventArgs(result));
         }
      }
   }
}

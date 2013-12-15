using System;
using Autodrome.Base;

namespace Autodrome.Navigation
{
   public class GSOF
   {
      public enum RecordType : byte
      {
         None = 0,
         PositionTimeGPS = 1,
         PositionWGS84 = 2,
         PositionECEF = 3,
         PositionLocalDatum = 4,
         PositionLocalZone = 5,
         DeltaECEF = 6,
         TangentPlaneDelta = 7,
         Velocity = 8,
         PDOPInfo = 9,
         Clock = 10,
         PositionVCV = 11,
         PositionSigmaInfo = 12,
         SatellitesBrief = 13,
         SatellitesDetailed = 14,
         ReceiverSerialNumber = 15,
         CurrentTime = 16,
         PositionTimeUTC = 26,
         Attitude = 27,
         AllSatellitesBrief = 33,
         AllSatellitesDetailed = 34,
         ReceivedBaseInfo = 35,
         BasePosition = 41,
      }

      [FlagsAttribute]
      public enum PositionFlags : ushort
      {
         None = 0,
         NewPosition = 1 << 0,
         ClockFix = 1 << 1,
         PlaneCoordinates = 1 << 2,
         Height = 1 << 3,

         LeastSquares = 1 << 5,

         FilteredL1Pseudoranges = 1 << 7,
         Differential = 1 << 8,
         DifferentialPhase = 1 << 9,
         DifferentialFixed = 1 << 10,
         OmniStar = 1 << 11,
         Static = 1 << 12,
         NetworkRTK = 1 << 13,
         RTKLocation = 1 << 14,
         BeaconDGPS = 1 << 15,

         Mask = 0xFFAF,
      }

      [FlagsAttribute]
      public enum VelocityFlags : byte
      {
         None = 0,
         Valid = 0x0001,
         Consecutive = 0x0002,
      }

      public class Record
      {
         public RecordType Type;
         public byte Length;
      }

      public enum BaseQuality : byte
      {
         Unavailable = 0,
         Autonomous = 1,
         Differential = 2,
         FixedRTK = 3,
         FloatRTK = 4,
      }

      public class PositionTimeGPSData : Record
      {
         public Int32 Time;
         public Int16 Week;
         public Byte SatelliteUsed;
         public PositionFlags Flags;
         public Byte InitializationNumber;
      }

      public class PositionWGS84Data : Record
      {
         public Double Latitude;
         public Double Longitude;
         public Double Height;
      }

      public class TangentPlaneDelta : Record
      {
         public Double East;
         public Double North;
         public Double Height;
      }

      public class VelocityData : Record
      {
         public VelocityFlags Flags;
         public Single Velocity;
         public Single Heading;
         public Single VerticalVelocity;
      }

      public class PositionSigmaInfo : Record
      {
         public Single PositionRMS;
         public Single SigmaEast;
         public Single SigmaNorth;
         public Single CovarianceEastNorth;
         public Single SigmaHeigth;
         public Single SemiMajorAxis;
         public Single SemiMinorAxis;
         public Single Orientation;
         public Single UnitVariance;
         public Int16 EpochCount;
      }

      public class PDOPInfo : Record
      {
         public Single PDOP;
         public Single HDOP;
         public Single VDOP;
         public Single TDOP;
      }

      [FlagsAttribute]
      public enum SatelliteFlags : ushort
      {
         None = 0,
         AboveHorizon = 1 << 0,
         AssignedToChannel = 1 << 1,
         L1Tracking = 1 << 2,
         L2Tracking = 1 << 3,
         L1ReportedAtBase = 1 << 4,
         L2ReportedAtBase = 1 << 5,
         UsedInPosition = 1 << 6,
         UsedInRTK = 1 << 7,
         PCodeOnL1Tracking = 1 << 8,
         PCodeOnL2Tracking = 1 << 9,
         //GPS
         CSOnL2Tracking = 1 << 10,
         L5Tracking = 1 << 11,
         //GLONASS
         GlonassM = 1 << 10,
         GlonassK = 1 << 11,

         Mask = 0xFFF,
      }

      public enum SatelliteSystem : byte
      {
         GPS = 0,
         SBAS = 1,
         GLONASS = 2,
         OMNISTAR = 10,
      }

      public class SatelliteDetailedData
      {
         public Byte PRN;
         public SatelliteFlags Flags;
         public SByte Elevation;
         public UInt16 Azimuth;
         public Byte L1SNR;
         public Byte L2SNR;
      }

      public class SatelliteDetailed : Record
      {
         public SatelliteDetailedData[] Data;
      }

      public class AllSatelliteDetailedData
      {
         public Byte PRN;
         public SatelliteSystem System;
         public SatelliteFlags Flags;
         public SByte Elevation;
         public UInt16 Azimuth;
         public Byte L1SNR;
         public Byte L2SNR;
         public Byte ExtraSNR;
      }

      public class AllSatelliteDetailed : Record
      {
         public AllSatelliteDetailedData[] Data;
      }

      public class BasePosition : Record
      {
         public Int32 Time;
         public Int16 Week;
         public Double Latitude;
         public Double Longitude;
         public Double Height;
         public BaseQuality Quality;
      }

      public enum AttitudeCalculationMode : byte
      {
         None = 0,
         Autonomous = 1,
         RTK_Float = 2,
         RTK_Fixed = 3,
         DGPS = 4,
      }
      
      public class AttitudeInfo : Record
      {
         public Int32 Time;
         public Byte Flags;
         public Byte SatelliteCount;
         public AttitudeCalculationMode CalculationMode;
         public Byte Reserved;
         public Double Pitch;
         public Double Yaw;
         public Double Roll;
         public Double MasterSlaveRange;
         public Int16 PDOP;
         public Single PithVariance;
         public Single YawVariance;
         public Single RollVariance;
         public Single PithYawCovariance;
         public Single PitchRollCovariance;
         public Single YawRollCovariance;
         public Single MasterSlaveRangeVariance;
      }

      public static Int16 ToInt16(byte[] data, int index)
      {
         byte[] copy = new byte[2];
         Array.Copy(data, index, copy, 0, 2);
         Array.Reverse(copy);
         return BitConverter.ToInt16(copy, 0);
      }

      public static UInt16 ToUInt16(byte[] data, int index)
      {
         byte[] copy = new byte[2];
         Array.Copy(data, index, copy, 0, 2);
         Array.Reverse(copy);
         return BitConverter.ToUInt16(copy, 0);
      }

      public static Int32 ToInt32(byte[] data, int index)
      {
         byte[] copy = new byte[4];
         Array.Copy(data, index, copy, 0, 4);
         Array.Reverse(copy);
         return BitConverter.ToInt32(copy, 0);
      }

      public static Double ToDouble(byte[] data, int index)
      {
         byte[] copy = new byte[8];
         Array.Copy(data, index, copy, 0, 8);
         Array.Reverse(copy);
         return BitConverter.ToDouble(copy, 0);
      }

      public static Single ToSingle(byte[] data, int index)
      {
         byte[] copy = new byte[4];
         Array.Copy(data, index, copy, 0, 4);
         Array.Reverse(copy);
         return BitConverter.ToSingle(copy, 0);
      }

      public static Record[] ReadRecords(byte[] data, int start, int size)
      {
         int offset;
         int record_count = 0;
         for (offset = 0; offset < size; offset += data[start+offset + 1]+2)
         {
            record_count++;
         }

         Record[] result = new Record[record_count];
         offset = start;
         for (int i=0; i < record_count; i++)
         {
            result[i] = ReadRecord(data, offset);
            offset += result[i].Length+2;
         }
         return result;
      }

      public static Record ReadRecord(byte[] data, int start)
      {
         RecordType type = (RecordType)data[start];
         byte length = data[start + 1];

         Record result;
         try
         {
            switch (type)
            {
               case RecordType.PositionTimeGPS:
                  {
                     PositionTimeGPSData record = new PositionTimeGPSData();
                     record.Time = ToInt32(data, start + 2);
                     record.Week = ToInt16(data, start + 6);
                     record.SatelliteUsed = data[start + 8];
                     record.Flags = (PositionFlags)BitConverter.ToUInt16(data, start + 9) & PositionFlags.Mask;
                     record.InitializationNumber = data[start + 10];
                     result = record;
                  }
                  break;
               case RecordType.PositionWGS84:
                  {
                     PositionWGS84Data record = new PositionWGS84Data();
                     record.Latitude = ToDouble(data, start + 2);
                     record.Longitude = ToDouble(data, start + 10);
                     record.Height = ToDouble(data, start + 18);
                     result = record;
                  }
                  break;
               case RecordType.TangentPlaneDelta:
                  {
                     TangentPlaneDelta record = new TangentPlaneDelta();
                     record.East = ToDouble(data, start + 2);
                     record.North = ToDouble(data, start + 10);
                     record.Height = ToDouble(data, start + 18);
                     result = record;
                  }
                  break;
               case RecordType.PositionSigmaInfo:
                  {
                     PositionSigmaInfo record = new PositionSigmaInfo();
                     record.PositionRMS = ToSingle(data, start + 2);
                     record.SigmaEast = ToSingle(data, start + 6);
                     record.SigmaNorth = ToSingle(data, start + 10);
                     record.CovarianceEastNorth = ToSingle(data, start + 14);
                     record.SigmaHeigth = ToSingle(data, start + 18);
                     record.SemiMajorAxis = ToSingle(data, start + 22);
                     record.SemiMinorAxis = ToSingle(data, start + 26);
                     record.Orientation = ToSingle(data, start + 30);
                     record.UnitVariance = ToSingle(data, start + 34);
                     record.EpochCount = ToInt16(data, start + 38);
                     result = record;
                  }
                  break;
               case RecordType.PDOPInfo:
                  {
                     PDOPInfo record = new PDOPInfo();
                     record.PDOP = ToSingle(data, start + 2);
                     record.HDOP = ToSingle(data, start + 6);
                     record.VDOP = ToSingle(data, start + 10);
                     record.TDOP = ToSingle(data, start + 14);
                     result = record;
                  }
                  break;
               case RecordType.SatellitesDetailed:
                  {
                     SatelliteDetailed record = new SatelliteDetailed();
                     record.Data = new SatelliteDetailedData[data[start + 2]];
                     for (int i = 0; i < record.Data.Length; i++)
                     {
                        int offset = start + 3 + 8 * i;
                        record.Data[i] = new SatelliteDetailedData();
                        record.Data[i].PRN = data[offset];
                        record.Data[i].Flags = ((SatelliteFlags)BitConverter.ToUInt16(data, offset + 1)) & SatelliteFlags.Mask;
                        record.Data[i].Elevation = (SByte)data[offset + 3];
                        record.Data[i].Azimuth = ToUInt16(data, offset + 4);
                        record.Data[i].L1SNR = data[offset + 6];
                        record.Data[i].L2SNR = data[offset + 7];
                     }
                     result = record;
                  }
                  break;
               case RecordType.AllSatellitesDetailed:
                  {
                     AllSatelliteDetailed record = new AllSatelliteDetailed();
                     record.Data = new AllSatelliteDetailedData[data[start + 2]];
                     for (int i = 0; i < record.Data.Length; i++)
                     {
                        int offset = start + 3 + 10 * i;
                        record.Data[i] = new AllSatelliteDetailedData();
                        record.Data[i].PRN = data[offset];
                        record.Data[i].System = (SatelliteSystem)data[offset + 1];
                        record.Data[i].Flags = ((SatelliteFlags)BitConverter.ToUInt16(data, offset + 2)) & SatelliteFlags.Mask;
                        record.Data[i].Elevation = (SByte)data[offset + 4];
                        record.Data[i].Azimuth = ToUInt16(data, offset + 5);
                        record.Data[i].L1SNR = data[offset + 7];
                        record.Data[i].L2SNR = data[offset + 8];
                        record.Data[i].ExtraSNR = data[offset + 9];
                     }
                     result = record;
                  }
                  break;
               case RecordType.BasePosition:
                  {
                     BasePosition record = new BasePosition();
                     record.Time = ToInt32(data, start + 2);
                     record.Week = ToInt16(data, start + 6);
                     record.Latitude = ToDouble(data, start + 8);
                     record.Longitude = ToDouble(data, start + 16);
                     record.Height = ToDouble(data, start + 24);
                     record.Quality = (BaseQuality)data[start + 33];
                     result = record;
                  }
                  break;
               case RecordType.Attitude:
                  {
                     AttitudeInfo record = new AttitudeInfo();
                     record.Time = ToInt32(data, start + 2);
                     record.Flags = data[start + 6];
                     record.SatelliteCount = data[start + 7];
                     record.CalculationMode = (AttitudeCalculationMode)data[start + 8];
                     record.Reserved = data[start + 9];
                     record.Pitch = ToDouble(data, start + 10);
                     record.Yaw = ToDouble(data, start + 18);
                     record.Roll = ToDouble(data, start + 26);
                     record.MasterSlaveRange = ToDouble(data, start + 34);
                     record.PDOP = ToInt16(data, start + 42);
                     result = record;
                  }
                  break;
               default:
                  result = new Record();
                  break;
            }
         }
         catch (IndexOutOfRangeException e)
         {
            String error_str = String.Format("data length: {0}\nstart index: {1}\ntype: {2}\nlength: {3}\n---\n",
               data.Length, start, start < data.Length ? ((RecordType)data[start]).ToString() : "???", start < (data.Length - 1) ? data[start + 1].ToString() : "???");
            Util.HandleException(error_str + e);
            result = new Record();
         }
         result.Type = type;
         result.Length = length;
         return result;
      }
   }
}
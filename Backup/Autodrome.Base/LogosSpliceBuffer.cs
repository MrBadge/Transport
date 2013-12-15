using System;
using System.Collections.Generic;
using System.Text;

using Autodrome.Interfaces;

namespace Autodrome.Base
{
   public enum LogosRecordType : byte
   {
      Ping = 0,
      Display = 1,
      VehicleControl = 2,
      VehicleState = 3,
      TrafficControlState = 4,
   }

   public class LogosRecord
   {
      public LogosRecordType Type;
      public LogosRecord() {Type = LogosRecordType.Ping;}
      protected LogosRecord(LogosRecordType type){Type=type;}
   }

   public class LogosVehicleStateRecord : LogosRecord
   {
      public Int32 TickCount;
      public UInt32 Value;
      public LogosVehicleStateRecord() : base(LogosRecordType.VehicleState) { }
   }

   public class LogosTrafficControlStateRecord : LogosRecord
   {
      public Int32 TickCount;
      public UInt64 Value;
      public LogosTrafficControlStateRecord() : base(LogosRecordType.TrafficControlState) { }
   }

   public class LogosEventArgs : EventArgs
   {
      public LogosRecord Record;
      public LogosEventArgs(LogosRecord r) { Record = r; }
   }

   public class LogosSpliceBuffer : SignatureSpliceBuffer
   {
      public LogosSpliceBuffer(int size)
         : base(new byte[] { 0xAA, 0x44, 0x12 }, size)
      {
         PacketReceived += processPacket;
         init_state_codes();
      }

      public event EventHandler<LogosEventArgs> RecordReceived;

      protected override bool PacketReady(int position)
      {
         int bytes_left = Offset - position;
         return (bytes_left > 10) && (bytes_left >= PacketSize(position));
      }
      protected override bool PacketValid(int position)
      {
         int message_size = PacketSize(position) - 4;
         if ((message_size > Data.Length) || (message_size < 0))
         {
            return false;
         }
         UInt32 crc = BitConverter.ToUInt32(Data, position + message_size);
         if (crc == 0x39393939)
            return true;
         if (crc == CalculateCRC(Data, position, message_size))
         {
            return true;
         }
         return false;
         //         return crc==CalculateCRC(Data, position, message_size);
      }
      protected override int PacketSize(int position)
      {
         int result = BitConverter.ToInt16(Data, position + 3) + 10;
         return result;
      }

      //private static Dictionary<VehicleStateFlags, int> state_codes;
      struct StateCodeMask
      {
         public VehicleStateFlags Flag;
         public int Mask;
         public int Value;
         public StateCodeMask(VehicleStateFlags f, int m, bool one)
         {
            Flag = f;
            Mask = m;
            Value = one ? m : 0;
         }
      }
      private static List<StateCodeMask> state_code_masks;

      private static void init_state_codes()
      {
         //if (state_codes != null) return;
         //state_codes = new Dictionary<VehicleStateFlags, int>();
         //state_codes[VehicleStateFlags.Key1] = 1 << 0;
         //state_codes[VehicleStateFlags.Key2] = 1 << 1;
         //state_codes[VehicleStateFlags.OilPressure] = 1 << 3;
         //state_codes[VehicleStateFlags.Battery] = 1 << 5;
         //state_codes[VehicleStateFlags.LeftTurn] = 1 << 7;
         //state_codes[VehicleStateFlags.RightTurn] = 1 << 9;
         //state_codes[VehicleStateFlags.SeatBelt] = 1 << 11;
         //state_codes[VehicleStateFlags.ParkingBrake] = 1 << 13;
         //state_codes[VehicleStateFlags.Ignition] = 1 << 21;

         if (state_code_masks != null) return;
         state_code_masks = new List<StateCodeMask>();
         state_code_masks.Add(new StateCodeMask(VehicleStateFlags.Key1, 1 << 0, true));
         state_code_masks.Add(new StateCodeMask(VehicleStateFlags.Key2, 1 << 1, true));
         state_code_masks.Add(new StateCodeMask(VehicleStateFlags.OilPressure, 1 << 3, true));
         state_code_masks.Add(new StateCodeMask(VehicleStateFlags.Battery, 1 << 5, true));
         state_code_masks.Add(new StateCodeMask(VehicleStateFlags.LeftTurn, 1 << 7, false));
         state_code_masks.Add(new StateCodeMask(VehicleStateFlags.RightTurn, 1 << 9, false));
         state_code_masks.Add(new StateCodeMask(VehicleStateFlags.SeatBelt, 1 << 11, false));
         state_code_masks.Add(new StateCodeMask(VehicleStateFlags.ParkingBrake, 1 << 13, true));
         state_code_masks.Add(new StateCodeMask(VehicleStateFlags.Ignition, 1 << 21, true));
      }

      private void processPacket(object sender, DataEventArgs args)
      {
         byte[] data = args.Data;
         int offset = args.Offset;
         int count = args.Count;

         LogosRecord result;

         LogosRecordType type = (LogosRecordType)data[5];
         switch(type)
         {
            case LogosRecordType.VehicleState:
               {
                  LogosVehicleStateRecord r = new LogosVehicleStateRecord();
                  r.TickCount = BitConverter.ToInt32(data, offset + 6);
                  r.Value = BitConverter.ToUInt32(data, offset + 10);
                  result = r;
               }
               break;
            case LogosRecordType.TrafficControlState:
               {
                  LogosTrafficControlStateRecord r = new LogosTrafficControlStateRecord();
                  r.TickCount = BitConverter.ToInt32(data, offset + 6);
                  r.Value = BitConverter.ToUInt64(data, offset + 10);
                  result = r;
               }
               break;
            default:
               result = new LogosRecord();
               break;
         }
         if (RecordReceived != null) { RecordReceived(this, new LogosEventArgs(result)); }
      }
   }
}

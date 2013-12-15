using System;
using System.Collections.Generic;
using System.Text;

namespace Autodrome.Interfaces
{
   public enum VehicleMode
   {
      Disconnected = 0,
      Test = 1,
      Work = 2,
   }

   [FlagsAttribute]
   public enum VehicleStateFlags : short
   {
      None = 0,

      ParkingBrake = 1 << 0,
      SeatBelt = 1 << 1,
      LeftTurn = 1 << 2,
      RightTurn = 1 << 3,
      Battery = 1 << 4,
      OilPressure = 1 << 5,
      Ignition = 1 << 6,
      //Starter = 1<<7,
      //Light = 1<<8,
      Key1 = 1 << 9,
      Key2 = 1 << 10,
      KeyMask = Key1|Key2,
      EngineStopped = Ignition|Battery|OilPressure,
      Hazard = LeftTurn|RightTurn,
   }

   [FlagsAttribute]
   public enum VehicleNavigationFlags : short
   {
      None = 0,
      ReceiverPresent = 1<<0,
      Autonomous = 1<<1,
      Differential = 1<<2,
      RTK = 1<<3,
      RTK_Fixed = 1<<4,
      Heading = 1 << 5,
      Heading_Fixed = 1 << 6,
   }

   [FlagsAttribute]
   public enum VehicleControlFlags : byte
   {
      None = 0,

      StopEngine = 1 << 1,
      RedIndicator = 1 << 2,
      GreenIndicator = 1 << 3,
      YellowIndicator = RedIndicator | GreenIndicator,
      Sound = 1 << 4,
      Power = 1 << 5,
   }

   public enum VehicleGears : byte
   {
      Undefined = 0,
      Manual1 = 1,
      Manual2 = 2,
      Manual3 = 3,
      Manual4 = 4,
      Manual5 = 5,
      ManualNeutral = 16,
      ManualReverse = 17,
      AutoNeutral = 32,
      AutoDrive = 33,
      Auto2 = 34,
      AutoReverse = 35,
      AutoParking = 36,
   }
}

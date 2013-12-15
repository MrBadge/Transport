using System;
using System.Collections.Generic;
//using System.Text;
using System.Drawing;

namespace Autodrome.Interfaces
{
   [Serializable]
   public class StaticVehicleInfo
   {
      public Int32 ID;
      public Int32 Model_ID;
      public Int32 Serial;
      public String Register;
      public Int32[] ExamVariants;
   }

   [Serializable]
   public class DynamicVehicleInfo
   {
      public Int32 ID;
      public VehicleStateFlags State;
      public VehicleNavigationFlags NavigationFlags;
      public VehicleGears Gear;
      public VehicleControlFlags Control;
      public VehicleMode Mode;
      public Single X;
      public Single Y;
      public Single Angle;
      public Int32 CandidateID;
      public Int32 ExamSeconds;
      public Int32 ExerciseID;
      public Int32 ErrorCount;
      public ExamFlags ExamFlags;
      public Int32 ExamVariantID;
      public Single DistanceCovered;
   }

   [Serializable]
   public class ErrorInfo
   {
      public Int32 Seconds;
      public Byte Weight;
      public String Description;
   }

   [FlagsAttribute]
   public enum TrafficLightState : byte
   {
      None = 0,
      Red = 1<<0,
      Yellow = 1<<1,
      Green = 1<<2,
      RedYellow = Red|Yellow,
   }

   [FlagsAttribute]
   public enum ExamFlags : byte
   {
      None = 0,
      Active = 1 << 0,
      Autostart = 1 << 1,
   }

   [Serializable]
   public class StaticTrafficLightInfo
   {
      public Int32 ID;
      public Single X;
      public Single Y;
      public Single Angle;
   }

   [Serializable]
   public class DynamicTrafficLightInfo
   {
      public Int32 ID;
      public TrafficLightState State;
   }

   public class TrafficLightInfo
   {
      public StaticTrafficLightInfo Static;
      public DynamicTrafficLightInfo Dynamic;
   }

   [Serializable]
   public class CandidateInfo
   {
      public Int32 ID;
      public String LastName;
      public String FirstName;
      public String SecondName;
      public DateTime BirthDate;
      public Guid RBT_ID;
      private static CandidateInfo empty;
      public static CandidateInfo Empty
      {
         get
         {
            if (empty == null)
            {
               empty = new CandidateInfo();
               empty.ID = 0;
            }
            return empty;
         }
      }
   }

   public enum LineType
   {
      None = 0,
      WhiteSolid = 1,
      WhiteDash = 2,
      WhiteThick = 3,
      YellowSolid = 4,
      YellowDash = 5,
      Border = 6,
      Cones = 7,
   }

   public enum FillType
   {
      None = 0,
      Grass = 1,
   }

   [Serializable]
   public class LineInfo
   {
      public PointF[] Geometry;
      public LineType LineType;
      public FillType FillType;
   }

   [Serializable]
   public class WheelInfo
   {
      public PointF Position;
      public Single Width;
   }

   [Serializable]
   public class VehicleModelInfo
   {
      public Int32 ID;
      public PointF[] Geometry;
      public String Manufacturer;
      public String Model;
      public PointF MasterPosition;
      public PointF RoverPosition;
      public WheelInfo[] Wheels;
   }

   [Serializable]
   public class ExerciseInfo
   {
      public ICollection<ErrorInfo> Errors;
      public Int32 Number;
      public String Name;
      public Int32 StartSeconds;
   }

   [Serializable]
   public class ExamVariantInfo
   {
      public Int32 ID;
      public String Name;
   }

   [Serializable]
   public class ExamInfo
   {
      public Int32 ID;
      public String ExamVariantName;
      public String Result;
      public ICollection<ExerciseInfo> Exercises;
      public DateTime StartTime;
      public Single Duration;
//      public String VehicleCategory;
//      public String VehicleStateNumber;
//      public String VehicleModel;
      public Int32 CandidateID;
//      public String CandidateLastName;
//      public String CandidateFirstName;
//      public String CandidateSecondName;
//      public Guid CandidateRBK_ID;
//      public DateTime CandidateBirthDate;
      public Int32 VehicleID;
      public Single DistanceCovered;
   }

   [Serializable]
   public class StaticInfo
   {
      public LineInfo[] Lines;
      public VehicleModelInfo[] Models;
      public StaticVehicleInfo[] Vehicles;
      public StaticTrafficLightInfo[] TrafficLights;
      public ExamVariantInfo[] ExamVariants;
   }

   [Serializable]
   public class DynamicInfo
   {
      public DynamicVehicleInfo[] Vehicles;
      public DynamicTrafficLightInfo[] TrafficLights;
   }

   [Serializable]
   public class HistoryFrame
   {
      public Int32 ms;
      public DynamicInfo info;
   }

   [Serializable]
   public class HistoryInfo
   {
      public DateTime timeStamp;
      public StaticInfo staticInfo;
      public HistoryFrame[] frames;
   }

   public interface AutodromeServer  
   {
      StaticInfo GetStaticInfo();
      DynamicInfo GetDynamicInfo();
      HistoryInfo GetHistoryInfo(Int32 exam);
      
      ICollection<StaticVehicleInfo> GetStaticVehiclesInfo();
      ICollection<DynamicVehicleInfo> GetDynamicVehiclesInfo();
      ICollection<ErrorInfo> GetVehicleErrors(Int32 vehicle);
      String GetExerciseString(Int32 exercise);
      ICollection<StaticTrafficLightInfo> GetStaticTrafficLightsInfo();
      ICollection<DynamicTrafficLightInfo> GetDynamicTrafficLightsInfo();
      ICollection<LineInfo> GetLinesInfo();
      ICollection<VehicleModelInfo> GetModelsInfo();
      ICollection<CandidateInfo> GetCandidatesInfo(CandidateInfo filter);
      CandidateInfo GetCandidateInfo(Int32 ID);
      Int32 AddCandidateInfo(CandidateInfo info);
      ICollection<ExamInfo> GetExamsInfo(DateTime from, DateTime to);

      void StartExam(Int32 vehicleID);
      void StopExam(Int32 vehicleID);
      void SetEngineStopped(Int32 vehicleID, Boolean stopped);
      void SetAutoStart(Int32 vehicleID, Boolean autostart);
      void BindCandidateToVehicle(Int32 CandidateID, Int32 vehicleID);
      void SetExamVariantToVehicle(Int32 examVariant, Int32 vehicleID);

      void Shutdown();
   }
}

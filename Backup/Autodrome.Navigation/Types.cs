using System;
using System.Collections.Generic;
using System.Text;

namespace Autodrome.Navigation
{
   public enum SolutionType
   {
      None = 0,
      Automonous = 1,
      Differential = 2,
      FloatRTK = 3,
      FixedRTK = 4,
   }

   public class SolutionData
   {
      public Double MasterX;
      public Double MasterY;
      public Double MasterZ;
      public Single MasterSigmaX;
      public Single MasterSigmaY;
      public Single MasterSigmaZ;
      public SolutionType MasterSolutionType;
      public UInt16 MasterSatellitesCount;
      public UInt16 MasterSatellitesUsed;
      public Double RoverX;
      public Double RoverY;
      public Double RoverZ;
      public Single RoverSigmaX;
      public Single RoverSigmaY;
      public Single RoverSigmaZ;
      public SolutionType RoverSolutionType;
      public UInt16 RoverSatellitesCount;
      public UInt16 RoverSatellitesUsed;
   }
}

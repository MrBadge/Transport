using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;

using Autodrome.Interfaces;

namespace Autodrome.Base
{
   [Serializable]
   public class WheelData
   {
      public Double Width;
      public PointF Position;
   }

   [Serializable]
   public class VehicleData
   {
      public String Manufacturer;
      public String Model;
      public VehicleMode Mode;

      private PointF[] geometry;
      public PointF[] Geometry
      {
         get { return geometry; }
         set { geometry = value; }
      }

      private PointF position;
      public PointF Position { get { return position; } }
      private Single angle;
      public Single Angle { get { return angle; } }

      public void SetAntennaPositions(PointF MasterPos, PointF RoverPoint)
      {
         Double a = Math.Atan2(MasterPos.Y - RoverPoint.Y, MasterPos.X - RoverPoint.X);
         Double s = Math.Sin(a);
         Double c = Math.Cos(a);

         Double x = MasterPos.X - c * MasterOffset.X + s * MasterOffset.Y;
         Double y = MasterPos.Y - s * MasterOffset.X - c * MasterOffset.Y;

         angle = (Single)a;
         position.X = (Single)x;
         position.Y = (Single)y;
      }

      public void SetPositionAndAttitude(PointF pos, Double yaw, Double pitch, Double range)
      {
         Double a = 0.5 * Math.PI - yaw;
         Double s = Math.Sin(a);
         Double c = Math.Cos(a);
         Double l = range * Math.Cos(pitch);

         MasterPos.X = (Single)(RoverPos.X + c * l);
         MasterPos.Y = (Single)(RoverPos.Y + s * l);

         Double x = MasterPos.X - c * MasterOffset.X + s * MasterOffset.Y;
         Double y = MasterPos.Y - s * MasterOffset.X - c * MasterOffset.Y;

         angle = (Single)a;
         position.X = (Single)x;
         position.Y = (Single)y;
      }

      public WheelData[] Wheels;
      public PointF MasterPos;
      public PointF MasterDev;
      public PointF MasterOffset;
      public PointF RoverPos;
      public PointF RoverDev;
      public PointF RoverOffset;

      public VehicleData()
      {
         RoverDev.X = RoverDev.Y = MasterDev.X = MasterDev.Y = 1f;
      }
   }
}

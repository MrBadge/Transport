using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Autodrome.Base
{
   public class GeometryUtil
   {
      public static RectangleF GetBoundingRect(PointF[] points)
      {
         PointF min;
         PointF max;
         if(points == null)
         {
            throw new ArgumentNullException("points");
         }
         min = max = points[0];
         for (int i = 0; i < points.Length; i++)
         {
            if (min.X > points[i].X) min.X = points[i].X;
            if (min.Y > points[i].Y) min.Y = points[i].Y;
            if (max.X < points[i].X) max.X = points[i].X;
            if (max.Y < points[i].Y) max.Y = points[i].Y;
         }
         return new RectangleF(min.X,min.Y, max.X-min.X,max.Y-min.Y);
      }

      //public static bool Intersects(PointF[] line1, PointF[] line2)
      //{
      //   for (int i = 1; i < line1.Length; i++)
      //   {
      //      for (int j = 1; j < line2.Length; j++)
      //      {

      //      }
      //   }
      //}
   }
}

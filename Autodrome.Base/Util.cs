using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

using Autodrome.Interfaces;
using System.IO;

namespace Autodrome.Base
{
   public static class Util
   {
      public static void HandleException(Exception e)
      {
         HandleException(e.ToString());
      }
      public static void HandleException(String s, Exception e)
      {
         HandleException(s + "" + e.ToString());
      }
      public static void HandleException(String s)
      {
         //StreamWriter log = File.AppendText("Exceptions.log");
         //log.WriteLine("{0}: {1}\n", DateTime.Now, s);
         MessageBox.Show(s);
      }
   }

   public class Conversions
   {
      private const Double to_radian = Math.PI / 180.0;
      private const Double to_degrees = 180.0 / Math.PI;

      private const Double wgs84_equator_radius = 6378137;
      private const Double wgs84_polar_radius = 6356752.314245;

      private static readonly DateTime gps_time_origin = new DateTime(1980, 1, 1);
      
      static private Double reference_latitude;
      static private Double reference_longitude;

      public static Double ToRadian(Double degrees) { return to_radian * degrees; }
      public static Single ToRadian(Single degrees) { return (Single)ToRadian((Double)degrees); }
      public static Double ToDegrees(Double radian) { return to_degrees * radian; }
      public static Single ToDegrees(Single radian) { return (Single)ToDegrees((Double)radian); }

      public static DateTime ToDateTime(Int32 week, Int32 ms)
      {
         return gps_time_origin + new TimeSpan(7 * week, 0, 0, 0, ms);
      }
      public static void FromDateTime(DateTime date_time, out Int16 week, out Int32 ms)
      {
         TimeSpan span = date_time - gps_time_origin;
         week = (Int16)(span.Days / 7);
         span -= new TimeSpan(7 * week * TimeSpan.TicksPerDay);
         ms = (Int32)(span.Ticks / TimeSpan.TicksPerMillisecond);
      }
      
      public static String ByteToString(byte value)
      {
         string result = "";
         for (int i = 0x80; i > 0; i >>= 1)
         {
            result += ((i & value) == 0) ? "0" : "1";
         }
         return result;
      }

      
      public static String ClientKeyToString(UInt64 key)
      {
         Byte[] b = BitConverter.GetBytes(key);
         return b[4].ToString() + '.' + b[5].ToString() + '.' + b[6].ToString() + '.' + b[7].ToString() + ':' + ((Int32)key).ToString();
      }

      public static String AddressToString(Int32 address)
      {
         try
         {
            byte[] b = BitConverter.GetBytes(address);
            return String.Format("{0}.{1}.{2}.{3}", new object[] { b[0], b[1], b[2], b[3] });
         }
         catch
         {
            return "0.0.0.0";
         }
      }

      public static Int32 StringToAddress(String str)
      {
         Int32 address = 0;
         if (str != null)
         {
            String[] substrings = str.Split(new char[] { '.' });
            foreach (String sub in substrings)
            {
               address <<= 8;
               address += Byte.Parse(sub);
            }
         }
         return IPAddress.NetworkToHostOrder(address);
      }

      public static String GearCodeToString(Int32 code)
      {
         char[] str = new char[8];
         for (int i = 0; i < 8; i++)
         {
            str[i] += ((code & (0x80>>i)) != 0 ? '1' : '0');
         }
         return new String(str);
      }

      public static Int32 StringToGearCode(String str)
      {
         Int32 code = 0;
         for (int i = 0; i < str.Length; i++)
         {
            code <<= 1;
            switch (str[i])
            {
               case '0': break;
               case '1': code += 1; break;
               default: return 0;
            }
         }
         return code;
      }

      public static String SecondsToString(Int32 seconds) { return String.Format("{0}:{1:D2}", seconds / 60, seconds % 60); }

      public static Double ToRadian(String geodetic)
      {
         String[] splitted = geodetic.Split(new char[]{'°','\'','\"'});
         Double result = Double.Parse(splitted[0]) + Double.Parse(splitted[1]) / 60.0 + Double.Parse(splitted[2]) / 3600.0;
         result = ToRadian(result);
         switch (splitted[3].ToUpper().ToCharArray()[0])
         {
            case 'E':
            case 'N': return result;
            case 'W':
            case 'S': return -result;
            default:
               throw new Exception("Can't convert from geodetic to degrees");
         }
      }

      public static void InitWGS84ToLocal( Double latitude, Double longitude )
      {
         reference_latitude = latitude;
         reference_longitude = longitude;
      }

      public static void ToLocalPlane(Double latitude, Double longitude,
         out Single easting, out Single northing)
      {
         Double longitude_delta = longitude - reference_longitude;
         Double latitude_delta = latitude - reference_latitude;
         const Double max_delta = 0.02;

         if (Math.Min(Math.Abs(latitude_delta), Math.Abs(longitude_delta)) > max_delta)
         {
//            throw new Exception("Far from reference position");
         }
         
         Double x = wgs84_equator_radius * Math.Cos(latitude);
         Double y = wgs84_polar_radius * Math.Sin(latitude);
         Double r = Math.Sqrt(x * x + y * y);
         easting = (Single)( x * longitude_delta);
         northing = (Single)(r * latitude_delta);
         
         //easting = (Single)(latitude - ToRadian(55.651921)) * 1e7;
         //northing = (Single)(longitude - ToRadian(37.63413)) * 1e7;
      }

      public static Single[] ToLocalPlane(Double latitude, Double longitude)
      {
         Single[] result = new Single[2];
         ToLocalPlane(latitude, longitude, out result[0], out result[1]);
         return result;
      }

      public static Byte[] PointFToByteArray(PointF[] points)
      {
         if (points == null)
            return null;
         Byte[] bytes = new Byte[points.Length * 2 * sizeof(Single)];
         int offset = 0;
         for (int i=0; i < points.Length; i++)
         {
            BitConverter.GetBytes(points[i].X).CopyTo(bytes, offset); offset += sizeof(Single);
            BitConverter.GetBytes(points[i].Y).CopyTo(bytes, offset); offset += sizeof(Single);
         }
         return bytes;
      }

      public static PointF[] ByteToPointFArray(Byte[] bytes)
      {
         if (bytes == null)
            return null;
         PointF[] points = new PointF[bytes.Length / 2 / sizeof(Single)];
         int offset = 0;
         for (int i = 0; i < points.Length; i++)
         {
            points[i].X = BitConverter.ToSingle(bytes, offset); offset += sizeof(Single);
            points[i].Y = BitConverter.ToSingle(bytes, offset); offset += sizeof(Single);
         }
         return points;
      }

      public static Rectangle StringToRectangle(String str)
      {
         String[] s = str.Split(new char[] { ',' });
         if (s.Length != 4) throw new Exception("StringToRectangle conversion error");
         return new Rectangle(Int32.Parse(s[0]), Int32.Parse(s[1]), Int32.Parse(s[2]), Int32.Parse(s[3]));
      }

      //public static PointF CoordinateToPointF(Coordinate value)
      //{
      //   return new PointF((Single)value.X, (Single)value.Y);
      //}

      //public static Coordinate SphericalToOrtho(Double Yaw, Double Pitch, Double Length)
      //{
      //   double yaw = -0.5*Math.PI - Yaw;
      //   return new Coordinate(Length * Math.Cos(yaw) * Math.Cos(Pitch), Length * Math.Sin(yaw) * Math.Cos(Pitch), Length*Math.Sin(Pitch));
      //}
   }
}
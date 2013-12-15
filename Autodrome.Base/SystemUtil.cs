using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Net;


namespace Autodrome.Base
{
   public class SystemUtil
   {
      public enum ShutdownAction
      {
         None = 0,
         Logoff = 1,
         Reboot = 2,
         Shutdown = 3,
      }

      public static object[] GetLocalAddresses()
      {
         IPAddress[] local_addresses = Dns.GetHostAddresses(Dns.GetHostName());
         object[] result = new String[local_addresses.Length + 1];
         result[0] = "127.0.0.1";
         for (int i = 0; i < local_addresses.Length; i++)
         {
            result[i + 1] = local_addresses[i].ToString();
         }
         return result;
      }

      public static void Shutdown(ShutdownAction action)
      {
         String param_string = "";
         switch (action)
         {
            case ShutdownAction.Logoff: param_string = "-l"; break;
            case ShutdownAction.Reboot: param_string = "-r"; break;
            case ShutdownAction.Shutdown: param_string = "-s"; break;
            default: return;
         }
         System.Diagnostics.Process.Start("shutdown", param_string + " -f -t 0");
      }

      public static void ShutdownWMI(ShutdownAction action)
      {
         if (action == ShutdownAction.None) return;
         ManagementBaseObject mboShutdown = null;
         ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
         mcWin32.Get();

         // You can't shutdown without security privileges
         mcWin32.Scope.Options.EnablePrivileges = true;
         ManagementBaseObject mboShutdownParams =
                  mcWin32.GetMethodParameters("Win32Shutdown");

         // Flag 1 means we want to shut down the system. Use "2" to reboot.
         switch (action)
         {
            case ShutdownAction.Logoff: mboShutdownParams["Flags"] = "0"; break;
            case ShutdownAction.Reboot: mboShutdownParams["Flags"] = "2"; break;
            case ShutdownAction.Shutdown: mboShutdownParams["Flags"] = "1"; break;

            default: mboShutdownParams["Flags"] = "0"; break;
         }

         mboShutdownParams["Reserved"] = "0";
         foreach (ManagementObject manObj in mcWin32.GetInstances())
         {
            mboShutdown = manObj.InvokeMethod("Win32Shutdown",
                                           mboShutdownParams, null);
         }
      }
   }
}

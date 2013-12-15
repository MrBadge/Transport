using System;
using System.Reflection;

namespace Autodrome.Interfaces
{
    [Serializable]
    public class TestVersion : MarshalByRefObject
    {
        public string GetVersion()
        {
            Assembly assem = Assembly.GetEntryAssembly();
            AssemblyName assemName = assem.GetName();
            Version vers = assemName.Version;
            string ver = vers.ToString();
            return ver;
        }
    }

}

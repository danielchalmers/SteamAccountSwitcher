#region

using System;
using System.Deployment.Application;
using System.Reflection;

#endregion

namespace SteamAccountSwitcher
{
    internal class AssemblyInfo
    {
        public static string GetAssemblyAttribute<T>(Func<T, string> value)
            where T : Attribute
        {
            var attribute = (T) Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof (T));
            return value.Invoke(attribute);
        }

        public static string GetVersionString()
        {
            return GetVersionString(GetVersion());
        }

        public static string GetVersionStringFull()
        {
            return GetVersionStringFull(GetVersion());
        }

        public static string GetVersionString(Version version)
        {
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        public static string GetVersionStringFull(Version version)
        {
            return version.ToString();
        }

        public static Version GetVersion()
        {
            var obj = ApplicationDeployment.IsNetworkDeployed
                ? ApplicationDeployment.CurrentDeployment.
                    CurrentVersion
                : Assembly.GetExecutingAssembly().GetName().Version;
            return obj;
        }

        public static string GetCopyright()
        {
            return GetAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright);
        }

        public static string GetTitle()
        {
            return GetAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title);
        }
    }
}
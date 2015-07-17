#region

using System;
using System.Deployment.Application;
using System.Reflection;
using System.Runtime.InteropServices;

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

        public static string GetVersionString() => GetVersionString(GetVersion());
        public static string GetVersionStringFull() => GetVersionStringFull(GetVersion());
        public static string GetVersionString(Version version) => $"{version.Major}.{version.Minor}.{version.Build}";
        public static string GetVersionStringFull(Version version) => version.ToString();

        public static Version GetVersion()
            =>
                (ApplicationDeployment.IsNetworkDeployed
                    ? ApplicationDeployment.CurrentDeployment.CurrentVersion
                    : Assembly.GetExecutingAssembly().GetName().Version);

        public static string GetCopyright() => GetAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright);
        public static string GetTitle() => GetAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title);
        public static string GetDescription() => GetAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description);
        public static string GetGuid() => GetAssemblyAttribute<GuidAttribute>(a => a.Value);
    }
}
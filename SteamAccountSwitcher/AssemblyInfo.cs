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
        public static string VersionString = GetVersionString();
        public static string VersionStringFull = GetVersionStringFull();

        public static Version Version = GetVersion();

        public static string Copyright = GetCopyright();

        public static string Title = GetTitle();

        public static string Description = GetDescription();

        public static string Guid = GetGuid();

        public static string GetAssemblyAttribute<T>(Func<T, string> value)
            where T : Attribute
        {
            var attribute = (T) Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof (T));
            return value.Invoke(attribute);
        }

        private static string GetVersionString() => GetVersionString(GetVersion());
        private static string GetVersionStringFull() => GetVersionStringFull(GetVersion());
        public static string GetVersionString(Version version) => $"{version.Major}.{version.Minor}.{version.Build}";
        public static string GetVersionStringFull(Version version) => version.ToString();

        private static Version GetVersion()
            =>
                (ApplicationDeployment.IsNetworkDeployed
                    ? ApplicationDeployment.CurrentDeployment.CurrentVersion
                    : Assembly.GetExecutingAssembly().GetName().Version);

        private static string GetCopyright() => GetAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright);
        private static string GetTitle() => GetAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title);
        private static string GetDescription() => GetAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description);
        private static string GetGuid() => GetAssemblyAttribute<GuidAttribute>(a => a.Value);
    }
}
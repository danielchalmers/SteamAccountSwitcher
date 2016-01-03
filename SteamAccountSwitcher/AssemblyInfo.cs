#region

using System;
using System.Deployment.Application;
using System.Reflection;
using System.Runtime.InteropServices;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal static class AssemblyInfo
    {
        private static Version Version { get; } = ApplicationDeployment.IsNetworkDeployed
            ? ApplicationDeployment.CurrentDeployment.CurrentVersion
            : Assembly.GetExecutingAssembly().GetName().Version;

        private static string Copyright { get; } = GetAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright);
        private static string Title { get; } = GetAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title);

        public static string Description { get; } =
            GetAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description);

        public static string CustomDescription { get; } = string.Format(Resources.About, Title, Version,
            Resources.Website, Resources.GitHubIssues, Resources.DonateLink, Resources.GitHubCommits, Copyright);

        public static string Guid { get; } = GetAssemblyAttribute<GuidAttribute>(a => a.Value);

        private static string GetAssemblyAttribute<T>(Func<T, string> value)
            where T : Attribute
        {
            var attribute = (T) Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof (T));
            return value.Invoke(attribute);
        }
    }
}
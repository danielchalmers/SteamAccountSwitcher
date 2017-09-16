using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SteamAccountSwitcher
{
    public static class AssemblyInfo
    {
        public static Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version;

        public static string Copyright { get; } = GetAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright);
        public static string Title { get; } = GetAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title);

        public static string Guid { get; } = GetAssemblyAttribute<GuidAttribute>(a => a.Value);

        private static string GetAssemblyAttribute<T>(Func<T, string> value)
            where T : Attribute
        {
            var attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
            return value.Invoke(attribute);
        }
    }
}
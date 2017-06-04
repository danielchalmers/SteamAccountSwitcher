using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
            ViewLicenses = new RelayCommand(ViewLicensesExecute);
        }

        public string Title { get; } = $"About {AssemblyInfo.Title}";
        public ICommand ViewLicenses { get; }

        private void ViewLicensesExecute()
        {
            Process.Start(LicensesDirectory);
        }

        public static string Text
        {
            get
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine($"{AssemblyInfo.Title} ({AssemblyInfo.Version})");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"Website: {Resources.Website}");
                stringBuilder.AppendLine($"Source Code: {Resources.GitHubMainPage}");
                stringBuilder.AppendLine($"Changes: {Resources.GitHubCommits}");
                stringBuilder.AppendLine($"Issues: {Resources.GitHubIssues}");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Libraries:");
                foreach (var library in Libraries
                    .Select(x => $"  {x.Key}: {x.Value}"))
                {
                    stringBuilder.AppendLine(library);
                }
                stringBuilder.AppendLine();
                stringBuilder.Append(AssemblyInfo.Copyright);

                return stringBuilder.ToString();
            }
        }

        private static Dictionary<string, string> Libraries { get; } = new Dictionary<string, string>
        {
            {"Common Service Locator", "https://commonservicelocator.codeplex.com"},
            {"Extended WPF Toolkit", "https://wpftoolkit.codeplex.com"},
            {"NotifyIcon", "http://www.hardcodet.net/wpf-notifyicon"},
            {"MVVM Light", "http://galasoft.ch/mvvm"},
            {"Json.NET", "http://www.newtonsoft.com/json"}
        };

        public static string LicensesDirectory { get; } = Path.Combine(
            Path.GetDirectoryName(
                Assembly.GetExecutingAssembly()
                    .Location),
            "Resources",
            "Licenses");
    }
}

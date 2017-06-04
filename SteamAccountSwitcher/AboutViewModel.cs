using System.Text;
using GalaSoft.MvvmLight;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public class AboutViewModel : ViewModelBase
    {
        public string Title { get; } = $"About {AssemblyInfo.Title}";

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
                stringBuilder.Append(AssemblyInfo.Copyright);

                return stringBuilder.ToString();
            }
        }
    }
}

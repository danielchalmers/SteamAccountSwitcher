#region

using System.Windows.Controls;

#endregion

namespace SteamAccountSwitcher.OptionsPages
{
    /// <summary>
    ///     Interaction logic for About.xaml
    /// </summary>
    public partial class About : Page
    {
        public About()
        {
            InitializeComponent();

            txtDescription.Text = string.Format(Properties.Resources.About, AssemblyInfo.GetTitle(),
                AssemblyInfo.GetVersion(),
                Properties.Resources.Website, Properties.Resources.GitHubIssues, Properties.Resources.DonateLink,
                Properties.Resources.GitHubCommits,
                AssemblyInfo.GetCopyright());
        }
    }
}
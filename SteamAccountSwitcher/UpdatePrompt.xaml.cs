#region

using System;
using System.Windows;

#endregion

namespace SteamAccountSwitcher
{
    /// <summary>
    ///     Interaction logic for UpdatePrompt.xaml
    /// </summary>
    public partial class UpdatePrompt : Window
    {
        public enum UpdateResponse
        {
            UpdateNow,
            RemindLater,
            RemindNever
        }

        public UpdateResponse UserResponse;

        public UpdatePrompt(Version updateVersion, bool isRequired)
        {
            InitializeComponent();

            if (!isRequired)
            {
                if (updateVersion.Major != AssemblyInfo.GetVersion().Major)
                {
                    txtUpdateSubMsg.Text =
                        $"A new major release, {AssemblyInfo.GetTitle()} {updateVersion.Major}.{updateVersion.Minor} is now available!";
                }
                else if (updateVersion.Revision != AssemblyInfo.GetVersion().Revision)
                {
                    txtUpdateSubMsg.Text =
                        $"{AssemblyInfo.GetTitle()} {AssemblyInfo.GetVersionStringFull(updateVersion)} is now available!";
                }
                else
                {
                    txtUpdateSubMsg.Text =
                        $"{AssemblyInfo.GetTitle()} {AssemblyInfo.GetVersionString(updateVersion)} is now available!";
                }
                txtUpdateSubMsg.Text += "\nDo you want to update to the latest version?";
            }
            else
            {
                txtUpdateSubMsg.Text =
                    $"A mandatory update is available.\nPress \"{btnOK.Content}\" to update to the latest version.";
            }

            btnNo.Visibility = isRequired ? Visibility.Hidden : Visibility.Visible;
            btnRemindLater.Visibility = isRequired ? Visibility.Hidden : Visibility.Visible;
            new ChangelogDownloader().GetChangelog(x => { txtChangelog.Text = x; });
        }

        private void btnRemindLater_Click(object sender, RoutedEventArgs e)
        {
            UserResponse = UpdateResponse.RemindLater;
            DialogResult = true;
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            UserResponse = UpdateResponse.RemindNever;
            DialogResult = true;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            UserResponse = UpdateResponse.UpdateNow;
            DialogResult = true;
        }
    }
}
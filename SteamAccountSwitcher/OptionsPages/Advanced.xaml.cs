#region

using System;
using System.Windows.Controls;

#endregion

namespace SteamAccountSwitcher.OptionsPages
{
    /// <summary>
    ///     Interaction logic for Advanced.xaml
    /// </summary>
    public partial class Advanced : Page
    {
        public Advanced()
        {
            InitializeComponent();
        }

        private void menuItemDefaults_OnClick(object sender, EventArgs e)
        {
            SettingsHelper.ResetSettings();
        }

        private void menuItemImport_OnClick(object sender, EventArgs e)
        {
            SettingsHelper.ImportSettings();
        }

        private void menuItemExport_OnClick(object sender, EventArgs e)
        {
            SettingsHelper.ExportSettings();
        }
    }
}
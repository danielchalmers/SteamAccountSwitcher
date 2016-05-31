#region

using System;
using System.Windows.Controls;

#endregion

namespace SteamAccountSwitcher.OptionsPages
{
    /// <summary>
    ///     Interaction logic for General.xaml
    /// </summary>
    public partial class General : Page
    {
        public General()
        {
            InitializeComponent();
        }

        private void menuItemImport_OnClick(object sender, EventArgs e)
        {
            AccountDataHelper.ImportAccounts();
        }

        private void menuItemExport_OnClick(object sender, EventArgs e)
        {
            AccountDataHelper.ExportAccounts();
        }
    }
}
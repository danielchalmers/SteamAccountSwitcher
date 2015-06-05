#region

using System.ComponentModel;
using System.Windows;
using Microsoft.Win32;

#endregion

namespace SteamAccountSwitcher
{
    /// <summary>
    ///     Interaction logic for SteamPath.xaml
    /// </summary>
    public partial class SteamPath : Window
    {
        public string Path;

        public SteamPath()
        {
            InitializeComponent();
            txtPath.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog {Filter = "Steam|steam.exe|All Files|*.*" };
            if (dlg.ShowDialog() == true)
                txtPath.Text = dlg.FileName;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Path = txtPath.Text;
        }
    }
}
using System.Windows;

namespace SteamAccountSwitcher
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        private void btnOK_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
#region

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using SteamAccountSwitcher.OptionsPages;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    /// <summary>
    ///     Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private readonly List<Page> _pages;

        public Options()
        {
            InitializeComponent();

            _pages = new List<Page>();
            LoadPages();

            Settings.Default.Save();
        }

        private void LoadPages()
        {
            _pages.Add(new General());
            _pages.Add(new About());
            for (var i = 0; i < _pages.Count; i++)
                NavBar.Items.Add(new ListBoxItem
                {
                    Content = _pages[i].Title,
                    Tag = i
                });
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Reload();
            DialogResult = true;
        }

        private void NavBar_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OptionsFrame.Navigate(_pages[NavBar.SelectedIndex]);
        }
    }
}
#region

using System;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace SteamAccountSwitcher.OptionsPages
{
    /// <summary>
    ///     Interaction logic for General.xaml
    /// </summary>
    public partial class Style : Page
    {
        public Style()
        {
            InitializeComponent();
            cbTextAlignment.ItemsSource = Enum.GetValues(typeof (HorizontalAlignment));
        }
    }
}
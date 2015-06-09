#region

using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

#endregion

namespace SteamAccountSwitcher
{
    /// <summary>
    ///     Interaction logic for HexColorChooser.xaml
    /// </summary>
    public partial class HexColorChooser : Window
    {
        public Brush Color;

        public HexColorChooser(Brush oldBrush)
        {
            InitializeComponent();
            txtColor.Text = oldBrush?.ToString();
            txtColor.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                Color = (Brush)new BrushConverter().ConvertFromString(txtColor.Text);
            }
            catch
            {
                // ignored
            }
        }
    }
}
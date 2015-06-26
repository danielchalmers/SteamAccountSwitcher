#region

using System.ComponentModel;
using System.Windows;

#endregion

namespace SteamAccountSwitcher
{
    /// <summary>
    ///     Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        public string TextData;

        public InputBox(string displayData = "")
        {
            InitializeComponent();
            if (displayData != "")
            {
                txtData.Text = displayData;
                txtData.IsReadOnly = true;
                txtData.SelectAll();
                //Clipboard.SetText(displayData);
            }
            txtData.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            TextData = txtData.Text;
        }
    }
}
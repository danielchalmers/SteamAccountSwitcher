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
        public bool Cancelled;
        public string TextData;

        public InputBox(string title, string displayData = null)
        {
            InitializeComponent();
            if (displayData != null)
            {
                txtData.Text = displayData;
                txtData.IsReadOnly = true;
                txtData.SelectAll();
                //Clipboard.SetText(displayData);
                btnCancel.Visibility = Visibility.Collapsed;
                btnOK.IsCancel = true;
            }
            else
            {
                btnCancel.Visibility = Visibility.Visible;
            }
            Title = title;
            txtData.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Cancelled = true;
            DialogResult = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            TextData = txtData.Text;
        }
    }
}
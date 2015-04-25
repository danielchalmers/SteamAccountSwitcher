using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SteamAccountSwitcher
{
	class Popup
	{
		public static MessageBoxResult Show(string text, MessageBoxButton btn = MessageBoxButton.OK,
			MessageBoxImage img = MessageBoxImage.Information, MessageBoxResult defaultbtn = MessageBoxResult.OK)
		{
			return MessageBox.Show(text, "SteamAccountSwitcher", btn, img, defaultbtn);
		}
	}
}

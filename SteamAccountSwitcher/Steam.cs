#region

using System.Diagnostics;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
	internal class Steam
	{
		private static void RunCommand(string command, string username, string password)
		{
			var cmd = new ProcessStartInfo
			{
				FileName = "cmd.exe",
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardInput = true,
				UseShellExecute = false
			};
			var p = new Process {StartInfo = cmd};
			p.Start();

			using (var sw = p.StandardInput)
				if (sw.BaseStream.CanWrite)
					sw.WriteLine(command, Settings.Default.SteamPath, username, password);
		}

		public static void LogOut()
		{
			RunCommand(Scripts.Close, "", "");
		}

		public static void LogIn(string username, string password)
		{
			RunCommand(Scripts.Open, username, password);
		}
	}
}
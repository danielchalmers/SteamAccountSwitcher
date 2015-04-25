using System.Diagnostics;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
	internal class Scripts
	{
		public const string Open = "start \"\" {0} -login \"{1}\" \"{2}\"";
		public const string Close = "start \"\" {0} -shutdown";

		public static void Run(string command, Account account)
		{
			var cmd = new ProcessStartInfo
			{
				FileName = "cmd.exe",
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				RedirectStandardInput = true,
				UseShellExecute = false
			};
			var p = new Process { StartInfo = cmd };
			p.Start();

			using (var sw = p.StandardInput)
				if (sw.BaseStream.CanWrite)
					sw.WriteLine(command, Settings.Default.SteamPath, account.Username, account.Password);
		}
	}
}
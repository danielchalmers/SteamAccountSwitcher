using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gameloop.Vdf;
using H.NotifyIcon.Core;
using Microsoft.Win32;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public static class SteamClient
    {
        public static SteamAccountCollection Accounts { get; private set; } = GetAccounts();

        public static void Launch(string args = "")
        {
            var directory = FindInstallDirectory();

            Process.Start(GetExe(directory), args);
        }

        public static async Task LogIn(SteamAccount account)
        {
        }

        public static async Task Exit(CancellationToken cancellationToken)
        {
            var process = GetProcess();

            if (process == null)
                return;

            if (process.MainWindowTitle == Resources.SteamNotLoggedInTitle)
            {
                process.CloseMainWindow();
            }
            else
            {
                Launch("-shutdown");
            }

            while (true)
            {
                process.Refresh();

                if (process.HasExited)
                    return;

                if (cancellationToken.IsCancellationRequested)
                {
                    process.Kill();
                }

                await Task.Delay(Settings.Default.SteamLogoutTimeoutInterval);
            }
        }

        public static async Task Exit()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(Settings.Default.SteamLogoutTimeoutMax);

            await Exit(cts.Token);
        }

        private static Process GetProcess() =>
            Process.GetProcessesByName("steam").FirstOrDefault();

        public static bool TrySetLoginUser(string user)
        {
            try
            {
                using var registryKey = Registry.CurrentUser.OpenSubKey(Resources.SteamRegistryDirectoryPath);
                registryKey?.SetValue("AutoLoginUser", user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryResetLoginUser() => TrySetLoginUser(null);

        private static string FindInstallDirectory()
        {
            // Return the user-specified directory if it's valid.
            if (File.Exists(GetExe(Settings.Default.SteamInstallDirectory)))
                return Settings.Default.SteamInstallDirectory;

            // Otherwise check the registry.
            try
            {
                using var registryKey = Registry.CurrentUser.OpenSubKey(Resources.SteamRegistryDirectoryPath);
                return registryKey?.GetValue("SteamPath").ToString();
            }
            catch
            {
                App.TrayIcon.ShowNotification(
                    "Couldn't find the Steam folder",
                    "Please manually enter the path in options",
                    NotificationIcon.Error);

                return null;
            }
        }

        //private static string FindExePath()
        //{
        //    // Return the user-specified path if it's valid.
        //    var userSpecifiedExe = GetExe(Settings.Default.SteamInstallDirectory);
        //    if (File.Exists(userSpecifiedExe))
        //        return userSpecifiedExe;

        //    // Otherwise check the registry.
        //    try
        //    {
        //        using var registryKey = Registry.CurrentUser.OpenSubKey(Resources.SteamRegistryDirectoryPath);
        //        return registryKey?.GetValue("SteamExe").ToString();
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        private static string GetExe(string installDirectory) => Path.Combine(installDirectory, "steam.exe");

        public static SteamAccountCollection GetAccounts()
        {
            var directory = FindInstallDirectory();
            var loginUsersVdfPath = Path.Combine(directory, "config", "loginusers.vdf");
            dynamic loginUsers = VdfConvert.Deserialize(File.ReadAllText(loginUsersVdfPath));

            var accounts = new SteamAccountCollection();
            foreach (var loginUser in loginUsers.Value)
            {
                accounts.Add(new()
                {
                    //ID = loginUser.Key,
                    Name = loginUser.Value.AccountName.Value,
                    Alias = loginUser.Value.PersonaName.Value,
                });
            }

            return accounts;
        }
    }
}
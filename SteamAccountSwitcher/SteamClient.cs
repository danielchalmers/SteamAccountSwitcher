using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.NotifyIcon.Core;
using Microsoft.Win32;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
	public static class SteamClient
	{
		public static SteamAccountCollection Accounts { get; } = new();

		/// <summary>
		/// Launches Steam with optional arguments and waits for it to start up.
		/// </summary>
		/// <remarks><see href="https://developer.valvesoftware.com/wiki/Command_Line_Options#Steam" /></remarks>
		/// <param name="args">Command-line parameters to launch Steam with.</param>
		public static async Task Launch(string args = "", CancellationToken cancellationToken = default)
		{
			var directory = FindInstallDirectory();

			var process = Process.Start(GetSteamExe(directory), args);

			while (!cancellationToken.IsCancellationRequested)
			{
				process.Refresh();

				if (process.HasExited || !string.IsNullOrEmpty(process.MainWindowTitle))
					break;

				await Task.Delay(Settings.Default.SteamLogoutTimeoutInterval, cancellationToken);
			}
		}

		/// <summary>
		/// Switches account to the specified user.
		/// </summary>
		/// <remarks>This will exit Steam then relaunch.</remarks>
		/// <param name="account">The account to switch to.</param>
		public static async Task LogIn(SteamAccount account)
		{
			await Exit();

			if (!TrySetLoginUser(account.Name))
			{
				App.TrayIcon.ShowNotification(
					"Couldn't switch account automatically",
					"May not have permission to change the registry",
					NotificationIcon.Error);
			}

			await Launch(Settings.Default.SteamArguments);
		}

		/// <summary>
		/// Logs out of the current user, if one is logged in.
		/// </summary>
		/// <remarks>This will exit Steam then relaunch.</remarks>
		public static async Task LogOut()
		{
			await Exit();

			if (!TryResetLoginUser())
			{
				App.TrayIcon.ShowNotification(
					"Couldn't log out automatically",
					"Please log of out Steam and try again",
					NotificationIcon.Error);
			}

			await Launch(Settings.Default.SteamArguments);
		}

		/// <summary>
		/// Shuts down the Steam client.
		/// </summary>
		/// <param name="cancellationToken">When cancellation is requested, Steam will be forcefully closed.</param>
		public static async Task Exit(CancellationToken cancellationToken)
		{
			var process = GetProcess();

			if (process == null)
				return;

			// The login window doesn't respond to the shutdown argument and needs to be closed through the process.
			if (process.MainWindowTitle == Resources.SteamNotLoggedInTitle)
			{
				// The window title was "Steam Login" before the 2022 UI changed it to "Steam Sign In",
				// but the later UI actually does support the shutdown argument so we don't have to check for it.
				process.CloseMainWindow();
			}
			else
			{
				await Launch("-shutdown", cancellationToken);
			}

			// Wait for Steam to gracefully shut down.
			while (true)
			{
				process.Refresh();

				if (process.HasExited)
					return;

				if (cancellationToken.IsCancellationRequested)
				{
					// Grace period has ended so we'll now forcefully exit.
					process.Kill();
				}

				await Task.Delay(Settings.Default.SteamLogoutTimeoutInterval, cancellationToken);
			}
		}

		/// <summary>
		/// Shuts down the Steam client.
		/// </summary>
		/// <remarks>After a default timeout period, Steam will be forcefully closed.</remarks>
		public static async Task Exit()
		{
			var cts = new CancellationTokenSource();
			cts.CancelAfter(Settings.Default.SteamLogoutTimeoutMax);

			await Exit(cts.Token);
		}

		/// <summary>
		/// Returns the main Steam process.
		/// </summary>
		private static Process GetProcess() =>
			Process.GetProcessesByName("steam").FirstOrDefault();

		/// <summary>
		/// Sets the next user to be automatically logged in.
		/// </summary>
		/// <returns>A bool indicating the login user was successfully set.</returns>
		private static bool TrySetLoginUser(string user)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			try
			{
				using var key = Registry.CurrentUser.OpenSubKey(Resources.SteamRegistryDirectoryPath, true);
				key?.SetValue("AutoLoginUser", user);
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Clears the value that tells Steam to automatically log in to a specific user.
		/// </summary>
		private static bool TryResetLoginUser() => TrySetLoginUser(string.Empty);

		/// <summary>
		/// Locates the directory Steam is installed to.
		/// </summary>
		/// <returns>The installation directory, or <c>null</c> if it wasn't found.</returns>
		public static string FindInstallDirectory()
		{
			// Return the user-specified directory if it's valid.
			if (File.Exists(GetSteamExe(Settings.Default.SteamInstallDirectory)))
				return Settings.Default.SteamInstallDirectory;

			// Otherwise check the registry.
			try
			{
				using var key = Registry.CurrentUser.OpenSubKey(Resources.SteamRegistryDirectoryPath);
				return key?.GetValue("SteamPath").ToString();
			}
			catch
			{
				App.TrayIcon.ShowNotification(
					"Couldn't find the Steam folder",
					"Please manually enter the path in Options",
					NotificationIcon.Error);

				return null;
			}
		}

		/// <summary>
		/// Returns the main Steam executable from the given installation directory.
		/// </summary>
		private static string GetSteamExe(string installDirectory) => Path.Combine(installDirectory, "steam.exe");
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamAccountSwitcher
{
	class AccountHandler
	{
		private List<Account> _accounts;

		private AccountHandler()
		{
			_accounts = new List<Account>();
		}

		public static void LogOut()
		{
			Scripts.Run(Scripts.Close, new Account());
		}

		public static void LogIn(Account account)
		{
			Scripts.Run(Scripts.Open, account);
		}
	}
}

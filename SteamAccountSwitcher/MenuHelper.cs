#region

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace SteamAccountSwitcher
{
    internal class MenuHelper
    {
        private readonly AccountHandler _accountHandler;

        public MenuHelper(AccountHandler accountHandler)
        {
            _accountHandler = accountHandler;
        }

        private IEnumerable<object> AccountMenuItems()
        {
            var menuList = new List<object>();

            var itemProperties = new MenuItem {Header = "Edit..."};
            itemProperties.Click += delegate { _accountHandler.OpenPropeties(); };

            var itemMoveUp = new MenuItem {Header = "Move Up"};
            itemMoveUp.Click += delegate { _accountHandler.MoveUp(); };

            var itemMoveDown = new MenuItem {Header = "Move Down"};
            itemMoveDown.Click += delegate { _accountHandler.MoveDown(); };

            var itemRemove = new MenuItem {Header = "Remove..."};
            itemRemove.Click += delegate { _accountHandler.Remove(-2, true); };

            menuList.Add(itemProperties);
            menuList.Add(new Separator());
            menuList.Add(itemMoveUp);
            menuList.Add(itemMoveDown);
            menuList.Add(new Separator());
            menuList.Add(itemRemove);
            return menuList;
        }

        private IEnumerable<object> MainMenuItems()
        {
            var menuList = new List<object>();

            var itemAdd = new MenuItem {Header = "Add Account..."};
            itemAdd.Click += delegate { _accountHandler.New(); };
            var itemRefresh = new MenuItem {Header = "Refresh All..."};
            itemRefresh.Click += delegate { _accountHandler.Refresh(); };
            var itemOptions = new MenuItem {Header = "Options..."};
            itemOptions.Click += delegate { SettingsHelper.OpenOptions(_accountHandler); };
            var itemCheckUpdates = new MenuItem {Header = "Check for Updates"};
            itemCheckUpdates.Click += delegate { ClickOnceHelper.CheckForUpdates(); };
            var itemExit = new MenuItem {Header = "Exit"};
            itemExit.Click += delegate { Application.Current.Shutdown(); };

            menuList.Add(itemAdd);
            menuList.Add(new Separator());
            menuList.Add(itemRefresh);
            menuList.Add(new Separator());
            menuList.Add(itemOptions);
            menuList.Add(new Separator());
            menuList.Add(itemCheckUpdates);
            menuList.Add(new Separator());
            menuList.Add(itemExit);
            return menuList;
        }

        public ContextMenu AccountMenu()
        {
            var menu = new ContextMenu();
            foreach (var item in AccountMenuItems())
                menu.Items.Add(item);
            return menu;
        }

        public ContextMenu MainMenu()
        {
            var menu = new ContextMenu();
            foreach (var item in MainMenuItems())
                menu.Items.Add(item);
            return menu;
        }
    }
}
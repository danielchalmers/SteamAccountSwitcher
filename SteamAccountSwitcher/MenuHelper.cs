#region

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace SteamAccountSwitcher
{
    internal class MenuHelper
    {
        private IEnumerable<object> AccountMenuItems(AccountHandler accountHandler)
        {
            var menuList = new List<object>();

            var itemProperties = new MenuItem {Header = "Edit..."};
            itemProperties.Click += delegate { accountHandler.OpenPropeties(); };

            var itemMoveUp = new MenuItem {Header = "Move Up"};
            itemMoveUp.Click += delegate { accountHandler.MoveUp(); };

            var itemMoveDown = new MenuItem {Header = "Move Down"};
            itemMoveDown.Click += delegate { accountHandler.MoveDown(); };

            var itemRemove = new MenuItem {Header = "Remove..."};
            itemRemove.Click += delegate { accountHandler.Remove(-2, true); };

            menuList.Add(itemProperties);
            menuList.Add(new Separator());
            menuList.Add(itemMoveUp);
            menuList.Add(itemMoveDown);
            menuList.Add(new Separator());
            menuList.Add(itemRemove);
            return menuList;
        }

        private IEnumerable<object> MainMenuItems(AccountHandler accountHandler, MainWindow window)
        {
            var menuList = new List<object>();
            
            var itemOptions = new MenuItem {Header = "Options..."};
            itemOptions.Click += delegate { SettingsHelper.OpenOptions(accountHandler, window); };
            var itemCheckUpdates = new MenuItem {Header = "Check for Updates"};
            itemCheckUpdates.Click += delegate { ClickOnceHelper.CheckForUpdates(); };
            var itemExit = new MenuItem {Header = "Exit"};
            itemExit.Click += delegate { Application.Current.Shutdown(); };

            menuList.Add(itemOptions);
            menuList.Add(new Separator());
            menuList.Add(itemCheckUpdates);
            menuList.Add(new Separator());
            menuList.Add(itemExit);
            return menuList;
        }

        public ContextMenu AccountMenu(AccountHandler accountHandler)
        {
            var menu = new ContextMenu();
            menu.Opened += accountHandler.SetFocus;
            foreach (var item in AccountMenuItems(accountHandler))
                menu.Items.Add(item);
            return menu;
        }

        public ContextMenu MainMenu(AccountHandler accountHandler, MainWindow window)
        {
            var menu = new ContextMenu();
            foreach (var item in MainMenuItems(accountHandler, window))
                menu.Items.Add(item);
            return menu;
        }
    }
}
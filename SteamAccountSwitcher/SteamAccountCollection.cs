using System.Collections.ObjectModel;
using System.IO;
using Gameloop.Vdf;

namespace SteamAccountSwitcher
{
    public class SteamAccountCollection : ObservableCollection<SteamAccount>
    {
        public void Reload(string installDirectory)
        {
            var loginUsersVdfPath = Path.Combine(installDirectory, "config", "loginusers.vdf");
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

            Clear();

            foreach (var account in accounts)
            {
                Add(account);
            }
        }
    }
}
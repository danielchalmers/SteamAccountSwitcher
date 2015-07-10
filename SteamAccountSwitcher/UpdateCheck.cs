#region

using System;
using System.ComponentModel;
using System.Deployment.Application;

#endregion

namespace SteamAccountSwitcher
{
    internal class UpdateCheck
    {
        private Action<UpdateCheckInfo> _updateFoundAction;
        private UpdateCheckInfo _updateInfo;

        public void CheckForUpdates(Action<UpdateCheckInfo> updateFoundAction)
        {
            _updateFoundAction = updateFoundAction;
            var bw = new BackgroundWorker();
            bw.DoWork += BwOnDoWork;
            bw.RunWorkerCompleted += BwOnRunWorkerCompleted;
            bw.RunWorkerAsync();
        }

        private void BwOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            _updateFoundAction(_updateInfo);
        }

        private void BwOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            try
            {
                if (!ApplicationDeployment.IsNetworkDeployed)
                    return;
                var ad = ApplicationDeployment.CurrentDeployment;

                _updateInfo = ad.CheckForDetailedUpdate();
            }
            catch
            {
                _updateInfo = null;
            }
        }
    }
}
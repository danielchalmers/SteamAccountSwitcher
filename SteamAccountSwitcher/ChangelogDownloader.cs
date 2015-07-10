#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class ChangelogDownloader
    {
        private string _updateText;
        private Action<string> _updateTextAction;

        public void GetChangelog(Action<string> updateTextAction)
        {
            _updateTextAction = updateTextAction;
            var bw = new BackgroundWorker();
            bw.DoWork += BwOnDoWork;
            bw.RunWorkerCompleted += BwOnRunWorkerCompleted;
            bw.RunWorkerAsync();
            _updateTextAction("Downloading changelog...");
        }

        private void BwOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            _updateTextAction(_updateText);
        }

        private void BwOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            try
            {
                var str = new StringBuilder();
                using (var webClient = new WebClient())
                {
                    webClient.Headers.Add("User-Agent: Other");
                    var json =
                        webClient.DownloadString(Resources.GitHubApiCommits);
                    var jsonD = JsonConvert.DeserializeObject<List<GitHubApiCommitsRootObject>>(json);
                    var versionDetected = false;
                    foreach (var j in jsonD)
                    {
                        var commit = j.commit.message;
                        var index = commit.IndexOf("\n", StringComparison.Ordinal);
                        if (index > 0)
                            commit = commit.Substring(0, index);
                        var isVersion = Regex.Match(commit, @"^\d+(\.\d+)+$") != Match.Empty;
                        var firstVersion = (!versionDetected && isVersion);
                        if (isVersion)
                            versionDetected = true;

                        if (!versionDetected)
                            continue;
                        str.AppendLine(isVersion ? $"{(firstVersion ? "" : Environment.NewLine)}{commit}" : $" {commit}");
                    }
                    str.Append($"\nYou can view the rest of the changelog at {Resources.GitHubCommits}");
                    _updateText = str.ToString();
                }
            }
            catch
            {
                _updateText = "Changelog could not be downloaded.";
            }
        }
    }
}
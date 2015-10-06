#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
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
            bw.DoWork += Worker_DoWork;
            bw.RunWorkerCompleted += Worker_Completed;
            bw.RunWorkerAsync();
            _updateTextAction("Downloading changelog...");
        }

        private void Worker_Completed(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            _updateTextAction(_updateText);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            try
            {
                _updateText = GetFormattedChangelog();
            }
            catch
            {
                _updateText = "Changelog could not be downloaded.";
            }
        }

        private static string GetFormattedChangelog()
        {
            var str = new StringBuilder();
            var changelogData = ParseChangelogData(DownloadChangelog());

            foreach (var x in changelogData)
            {
                var changes = x.Changes.ToList();
                changes.Sort();
                str.Append($"{x.Version} ({x.PublishDate.ToString("yyyy-MM-dd")})");
                foreach (var y in changes)
                    str.Append($"\n {y}");
                str.AppendLine();
                str.AppendLine();
            }

            str.Append($"You can view the full changelog at {Resources.GitHubCommits}");
            return str.ToString();
        }

        private static IEnumerable<ChangelogVersionData> ParseChangelogData(string data)
        {
            var json = JsonConvert.DeserializeObject<List<GitHubApiCommitsRootObject>>(data);
            var versionLog = new List<ChangelogVersionData>();
            var versionCommits = new List<string>();
            Version lastVersion = null;
            var lastDateTime = DateTime.Now;
            var lastCommit = json.Last();
            foreach (var j in json)
            {
                var commit = j.commit.message;
                var index = commit.IndexOf("\n", StringComparison.Ordinal);
                if (index > 0)
                    commit = commit.Substring(0, index);
                Version version;
                if (Version.TryParse(commit, out version) || j.Equals(lastCommit))
                {
                    if (lastVersion != null)
                    {
                        versionLog.Add(new ChangelogVersionData(
                            lastVersion,
                            lastDateTime,
                            versionCommits.ToList()));
                        versionCommits.Clear();
                    }
                    lastVersion = version;
                    lastDateTime = DateTime.Parse(j.commit.committer.date);
                }
                else
                {
                    if (commit != "Minor code cleanup")
                        versionCommits.Add(commit);
                }
            }
            return versionLog;
        }

        private static string DownloadChangelog()
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("User-Agent: Other");
                var json = webClient.DownloadString(Resources.GitHubApiCommits);
                return json;
            }
        }
    }

    public class ChangelogVersionData
    {
        public ChangelogVersionData(Version version, DateTime publishDateTime, List<string> changes)
        {
            Version = version;
            PublishDate = publishDateTime;
            Changes = changes;
        }

        public Version Version { get; }
        public DateTime PublishDate { get; }
        public List<string> Changes { get; }
    }
}
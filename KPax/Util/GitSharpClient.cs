using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace KPax.Util
{
    public class GitSharpClient
    {
        private readonly string _repoPath;

        public GitSharpClient(string repoPath)
        {
            _repoPath = repoPath;
        }

        public Branch CurrentBranch()
        {
            var repo = new Repository(_repoPath);
            return repo.Head.TrackedBranch;
        }

        public string LastTag()
        {
            using (var repo = new Repository(_repoPath))
            {
                return repo.Describe(repo.Head.Tip, new DescribeOptions() { AlwaysRenderLongFormat = false, MinimumCommitIdAbbreviatedSize = 1, Strategy = DescribeStrategy.Tags }  );
            }
        }

        public string LastModify()
        {
            var repo = new Repository(_repoPath);

            string msg = repo.Head.Tip.MessageShort + " (" + repo.Head.Tip.Author.Email + ")" + " - " + repo.Head.Tip.Committer.When.ToString();
            return msg;
        }


        public string Status()
        {
            using (var repo = new Repository(_repoPath))
            {
                if (repo.RetrieveStatus().IsDirty)
                {
                    return "is dirty";
                }
                else
                {
                    return "nothing to commit";
                }
            }
        }

        public string Pull(string username, string password)
        {
            using (var repo = new Repository(_repoPath))
            {
                var options = new PullOptions();
                options.FetchOptions = new FetchOptions();
                options.FetchOptions.CredentialsProvider = new CredentialsHandler(
                    (url, usernameFromUrl, types) =>
                        new UsernamePasswordCredentials()
                        {
                            Username = username,
                            Password = password
                        });
                var mergeResults = repo.Network.Pull(
                    new Signature(username, username + "@atlantico.com.br", new DateTimeOffset(DateTime.Now)),
                    options);
                return mergeResults.Status.ToString();
            }
        }

        public bool Tag(string tagName, string username, string password)
        {
            using (var repo = new Repository(_repoPath))
            {
                Tag t = repo.ApplyTag(tagName.Trim());

                var options = new PushOptions();
                options.CredentialsProvider = new CredentialsHandler(
                    (url, usernameFromUrl, types) =>
                        new UsernamePasswordCredentials()
                        {
                            Username = username,
                            Password = password
                        });

                repo.Network.Push(repo.Network.Remotes["origin"], t.CanonicalName, options);
                return true;
            }
        }
    }
}
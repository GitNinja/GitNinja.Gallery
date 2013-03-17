using System;
//using System.Configuration;
using System.Web;
using LibGit2Sharp;

namespace GitNinja.Gallery.Web.Models
{
    public class Repo
    {
        public string Dojo { get; private set; }
        public string Name { get; private set; }
        public string Url { get; private set; }
        public Repository Repository { get; private set; }

        public Repo(string dojoName, string repoName)
        {
            if (!GitNinja.Instance.RepoExists(dojoName, repoName))
                throw new Exception(string.Format("Repo: {0}/{1} not found.", dojoName, repoName));
            Dojo = dojoName;
            Name = repoName;
            Repository = new Repository(GitNinja.GetRepositoryPath(dojoName, repoName));
            //Url = string.Format("https://{0}/{1}/{2}", ConfigurationManager.AppSettings.Get("GitHost"), Dojo, Name);
            Url = string.Format("https://{0}/git/{1}/{2}", HttpContext.Current.Request.Url.Host, Dojo, Name);
        }
    }
}
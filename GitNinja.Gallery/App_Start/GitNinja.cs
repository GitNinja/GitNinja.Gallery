using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace GitNinja.Gallery.App_Start
{
  public class GitNinja
  {
    private GitNinja() { }

    private static readonly GitNinja instance = new GitNinja();

    public static GitNinja Instance { get { return instance; } }


      public static void Moo()
      {
          
      }

    public static bool IsValidRepository(string dojoName, string repoName)
    {
      return Instance.RepoExists(dojoName, repoName);
    }

    public static string GetRepositoryPath(string dojoName, string repoName)
    {
      return Path.Combine(Home, dojoName, repoName);
    }

    public static string Home
    {
      get
      {
        var gitNinjaHome = ConfigurationManager.AppSettings.Get("GitNinjaHome");
        if (string.IsNullOrWhiteSpace(gitNinjaHome))
          gitNinjaHome = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Repositories");
        return gitNinjaHome;
      }
    }
    

    public static void Init()
    {
      CreateDirectory(Home);
    }

    public IEnumerable<string> GetDojoList()
    {
      //todo: check credentials, and return subset of dojos for which user has read access.
      return Directory.GetDirectories(Home).Select(Path.GetFileName);
    }

    public bool DojoExists(string dojoName)
    {
      return GetDojoList().Any(x => x.Equals(dojoName, StringComparison.InvariantCultureIgnoreCase));
    }

    public void CreateDojo(string dojoName)
    {
      if(!DojoExists(dojoName))
        CreateDirectory(Path.Combine(Home, dojoName));
    }

    public IEnumerable<string> GetRepoList(string dojoName)
    {
      //todo: check credentials, and return subset of repos for which user has read access.
      return Directory.GetDirectories(Path.Combine(Home, dojoName)).Select(Path.GetFileName);
    }

    public bool RepoExists(string dojoName, string repoName)
    {
      return DojoExists(dojoName) && GetRepoList(dojoName).Any(x => x.Equals(repoName, StringComparison.InvariantCultureIgnoreCase));
    }

    public void CreateRepo(string dojoName, string repoName)
    {
      if (!RepoExists(dojoName, repoName))
      {
        var repoPath = GetRepositoryPath(dojoName, repoName);
        CreateDirectory(repoPath);
        Repository.Init(repoPath, true);
      }
    }

    private static void CreateDirectory(string path)
    {
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
    }
  }
}
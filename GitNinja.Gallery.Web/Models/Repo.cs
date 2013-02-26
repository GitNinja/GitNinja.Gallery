using System;

namespace GitNinja.Gallery.Web.Models
{
  public class Repo
  {
    public string Dojo { get; private set; }
    public string Name { get; private set; }

    public Repo(string dojoName, string repoName)
    {
      if (!GitNinja.Instance.RepoExists(dojoName, repoName))
        throw new Exception(string.Format("Repo: {0}/{1} not found.", dojoName, repoName));
      Dojo = dojoName;
      Name = repoName;
    }
  }
}
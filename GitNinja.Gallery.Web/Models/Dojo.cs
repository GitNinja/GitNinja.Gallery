﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GitNinja.Gallery.Web.Models
{
  public class Dojo : List<Repo>
  {
    public string Name { get; private set; }

    public Dojo(string name)
    {
      if (!GitNinja.Instance.DojoExists(name))
        throw new Exception(string.Format("Dojo: {0} not found.", name));
      Name = name;
      AddRange(GitNinja.Instance.GetRepoList(Name).Select(repoName => new Repo(name, repoName)));
    }
  }
}
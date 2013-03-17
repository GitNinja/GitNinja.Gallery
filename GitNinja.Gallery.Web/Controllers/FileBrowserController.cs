using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GitNinja.Gallery.Web.Models;
using LibGit2Sharp;
using Filter = LibGit2Sharp.Filter;

namespace GitNinja.Gallery.Web.Controllers
{
    public class FileBrowserController : Controller
    {
        public ActionResult Lookup(string dojo, string repo, string branch, string id)
        {
            if (string.IsNullOrEmpty(branch))
            {
                branch = "master";
            }

            var repository = new Repo(dojo, repo);
            var gitRepo = repository.Repository;
            var gitBranch = gitRepo.Branches[branch];

            id = id ?? gitBranch.Tip.Tree.Id.Sha;

            var tree = gitRepo.Lookup<Tree>(id);

            if (tree == null)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            
            var nodes = tree.Select(entry => new TreeNodeModel() {Id = entry.Target.Sha, Name = entry.Name, Type = entry.Mode.ToString()}).ToList();
            
            foreach (var node in nodes)
            {
                var filter = new LibGit2Sharp.Filter() { Until = node.Id, SortBy = GitSortOptions.Reverse };

                var lastCommit = gitRepo.Commits.QueryBy(filter).FirstOrDefault();
                if (lastCommit != null)
                {

                    node.LastCommit = new CommitModel()
                        {
                            Id = lastCommit.Id.Sha,
                            Author = lastCommit.Author.Name,
                            Message = lastCommit.Message,
                            AuthoredWhen = lastCommit.Author.When,
                            CommittedWhen = lastCommit.Committer.When,
                            Comitter = lastCommit.Committer.Name
                        };
                }
            }
            
            return Json(nodes, JsonRequestBehavior.AllowGet);
        }

        public class TreeNodeModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }

            public CommitModel LastCommit { get; set; }
        }

    }

    public class CommitModel
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Comitter { get; set; }
        public string Message { get; set; }
        public DateTimeOffset AuthoredWhen { get; set; }
        public DateTimeOffset CommittedWhen { get; set; }
    }

    public enum TreeNodeType
    {
        File,
        Directory
    }
}

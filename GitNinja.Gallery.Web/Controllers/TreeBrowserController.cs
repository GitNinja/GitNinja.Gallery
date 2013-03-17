using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GitNinja.Gallery.Web.Helpers;
using GitNinja.Gallery.Web.Models;
using LibGit2Sharp;
using Filter = LibGit2Sharp.Filter;

namespace GitNinja.Gallery.Web.Controllers
{
    public class TreeBrowserController : Controller
    {
        public ActionResult Tree(string dojo, string repo, string branch = "master", string path = null)
        {
            var repository = new Repo(dojo, repo);
            var gitRepo = repository.Repository;
            var gitBranch = gitRepo.Branches[branch];

            if (gitRepo == null || gitBranch == null)
            {
                return new EmptyResult();
            }

            var tree = gitRepo.FindTreeForPath(gitBranch.Tip, path);

            if (tree == null)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

            var nodes = tree.Select(entry => new TreeViewElement() { Id = entry.Target.Sha, Name = entry.Name, Path = entry.Path, Type = entry.Mode.ToString() }).ToList();

            foreach (var node in nodes)
            {
                Commit lastCommit = gitRepo.FindLastChangingCommit(gitRepo.Head.Tip, node.Path);

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

            return PartialView("Tree", new TreeViewModel() {Dojo = dojo, Repo = repo, Branch = branch, Path = path, Elements = nodes});
        }

        public ActionResult Blob(string dojo, string repo, string branch, string path)
        {
            throw new NotImplementedException();
        }

        public class TreeViewModel
        {
            public string Dojo { get; set; }
            public string Repo { get; set; }
            public string Branch { get; set; }
            public string Path { get; set; }

            public bool IsRoot
            {
                get { return string.IsNullOrEmpty(Path); }
            }

            public IEnumerable<TreeViewElement> Elements { get; set; }

            public string ParentPath
            {
                get
                {
                    var lastSlash = Path.LastIndexOf("/");

                    if (lastSlash < 0)
                    {
                        return "";
                    }
                    return Path.Substring(0, lastSlash);
                }
            }
        }


        public class TreeViewElement
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public string Type { get; set; }

            public CommitModel LastCommit { get; set; }

            public bool IsDirectory { get { return Type == "Directory";}}
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

    }


}

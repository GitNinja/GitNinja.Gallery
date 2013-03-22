using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GitNinja.Gallery.Web.Helpers;
using GitNinja.Gallery.Web.Models;
using GitNinja.Gallery.Web.Models.View;
using LibGit2Sharp;
using PagedList;
using Blob = GitNinja.Gallery.Web.Models.View.Blob;
using Commit = GitNinja.Gallery.Web.Models.View.Commit;
using TreeEntry = GitNinja.Gallery.Web.Models.View.TreeEntry;

namespace GitNinja.Gallery.Web.Controllers
{
    public class RepositoryController : Controller
    {
        #region Repository Management
        [HttpPost]
        public ActionResult Create(string dojo, string repo)
        {
            if (string.IsNullOrWhiteSpace(repo)
                || GitNinja.Instance.RepoExists(dojo, repo))
            {
                dynamic failureModel = new ExpandoObject();
                failureModel.Dojo = dojo;
                failureModel.Repo = repo;
                return View("RepoCreationFailed", failureModel);
            };

            //TODO: Transactionally not safe, revisit
            GitNinja.Instance.CreateRepo(dojo, repo);

            return RedirectToAction("Tree", new { dojo, repo });
        }
        #endregion

        #region Commits
        [HttpGet]
        public ActionResult Commit(string dojo, string repo, string reference)
        {
            return View("Commit");
        }

        [HttpGet]
        public ActionResult Commits(string dojo, string repo, int page = 1)
        {
            var repository = new Repo(dojo, repo);
            var gitRepository = repository.Repository;

            const int pageSize = 10;

            var pagedCommits = gitRepository.Commits.Select(
                c =>
                new Commit()
                    {
                        Id = c.Sha,
                        Message = c.Message,
                        MessageShort = c.MessageShort,
                        Author = c.Author.Name,
                        AuthorMail = c.Author.Email,
                        AuthoredWhen = c.Author.When,
                        Committer = c.Committer.Name,
                        CommitterMail = c.Committer.Email,
                        CommittedWhen = c.Committer.When
                    }).ToPagedList(page, pageSize);

            var model = new CommitList(repository, pagedCommits);
            return View("Commits", model);
        }
        #endregion

        #region Trees and Blobs
        public ActionResult Tree(string dojo, string repo, string reference = "master", string path = null)
        {
            var repository = new Repo(dojo, repo);
            var gitRepository = repository.Repository;


            if (!gitRepository.Branches.Any())
            {
                return View("Tree", new Models.View.Tree(repository) { Dojo = dojo, Name = repo, Reference = reference, Path = path, Elements = new TreeEntry[0] });

            }

            var commit = gitRepository.FindCommitForReference(reference);

            var tree = gitRepository.FindTreeForPath(commit, path);

            var nodes = tree.Select(entry => new TreeEntry() { Id = entry.Target.Sha, Name = entry.Name, Path = entry.Path, Type = entry.Mode.ToString() }).ToList();

            foreach (var node in nodes)
            {
                LibGit2Sharp.Commit lastCommit = gitRepository.FindLastChangingCommit(commit, node.Path);

                if (lastCommit != null)
                {

                    node.LastCommit = new Commit()
                    {
                        Id = lastCommit.Id.Sha,
                        Author = lastCommit.Author.Name,
                        Message = lastCommit.Message,
                        AuthoredWhen = lastCommit.Author.When,
                        CommittedWhen = lastCommit.Committer.When,
                        Committer = lastCommit.Committer.Name
                    };
                }
            }

            return View("Tree", new Models.View.Tree(repository) { Dojo = dojo, Name = repo, Reference = reference, Path = path, Elements = nodes });
        }

        public ActionResult Blob(string dojo, string repo, string reference = "master", string path = null)
        {
            var repository = new Repo(dojo, repo);
            var gitRepository = repository.Repository;

            if (gitRepository == null)
            {
                return new EmptyResult();
            }

            var commit = gitRepository.FindCommitForReference(reference);

            var treeEntry = commit[path];
            var blob = treeEntry.Target as LibGit2Sharp.Blob;

            return View("Blob", new Blob(repository) { Reference = reference, Path = path, Content = blob.ContentAsUtf8(), SizeInBytes = blob.Size });
        }
        #endregion
    }
}

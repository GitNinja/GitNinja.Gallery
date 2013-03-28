using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using global::GitNinja.Core.Extensions;
using GitNinja.Gallery.Models;
using GitNinja.Gallery.Models.View;
using PagedList;

namespace GitNinja.Gallery.Controllers
{
    public class RepositoryController : Controller
    {
        #region Repository Management
        [HttpPost]
        public ActionResult Create(string dojo, string repo)
        {
            if (string.IsNullOrWhiteSpace(repo)
                || App_Start.GitNinja.Instance.RepoExists(dojo, repo))
            {
                dynamic failureModel = new ExpandoObject();
                failureModel.Dojo = dojo;
                failureModel.Repo = repo;
                return View("CreationFailed", failureModel);
            };

            //TODO: Transactionally not safe, revisit
            App_Start.GitNinja.Instance.CreateRepo(dojo, repo);

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
                new CommitViewModel()
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

            var model = new CommitsViewModel(repository, pagedCommits);
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
                return View("Tree", new TreeViewModel(repository) { Dojo = dojo, Name = repo, Reference = reference, Path = path, Elements = new TreeEntryViewModel[0] });

            }

            var commit = gitRepository.FindCommitForReference(reference);

            var tree = gitRepository.FindTreeForPath(commit, path);

            var nodes = tree.Select(entry => new TreeEntryViewModel() { Id = entry.Target.Sha, Name = entry.Name, Path = entry.Path, Type = entry.Mode.ToString() }).ToList();

            foreach (var node in nodes)
            {
                LibGit2Sharp.Commit lastCommit = gitRepository.FindLastChangingCommit(commit, node.Path);

                if (lastCommit != null)
                {

                    node.LastCommit = new CommitViewModel()
                    {
                        Id = lastCommit.Sha,
                        Message = lastCommit.Message,
                        MessageShort = lastCommit.MessageShort,
                        Author = lastCommit.Author.Name,
                        AuthorMail = lastCommit.Author.Email,
                        AuthoredWhen = lastCommit.Author.When,
                        Committer = lastCommit.Committer.Name,
                        CommitterMail = lastCommit.Committer.Email,
                        CommittedWhen = lastCommit.Committer.When
                    };
                }
            }

            return View("Tree", new TreeViewModel(repository) { Dojo = dojo, Name = repo, Reference = reference, Path = path, Elements = nodes });
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

            return View("Blob", new BlobViewModel(repository) { Reference = reference, Path = path, Content = blob.ContentAsUtf8(), SizeInBytes = blob.Size });
        }
        #endregion
    }
}

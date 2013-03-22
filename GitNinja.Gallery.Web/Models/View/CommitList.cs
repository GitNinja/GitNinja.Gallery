using System.Collections.Generic;
using PagedList;

namespace GitNinja.Gallery.Web.Models.View
{
    public class CommitList : RepositoryBasedModel
    {
        public IPagedList<Commit> Commits { get; set; }

        public CommitList(Repo repo, IPagedList<Commit> commits) : base(repo)
        {
            Commits = commits ?? new PagedList<Commit>(new Commit[0], 1, 0);
        }
    }
}
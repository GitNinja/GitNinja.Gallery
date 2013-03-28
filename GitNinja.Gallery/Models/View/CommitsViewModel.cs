using PagedList;

namespace GitNinja.Gallery.Models.View
{
    public class CommitsViewModel : RepositoryBasedViewModel
    {
        public IPagedList<CommitViewModel> Commits { get; set; }

        public CommitsViewModel(Repo repo, IPagedList<CommitViewModel> commits) : base(repo)
        {
            Commits = commits ?? new PagedList<CommitViewModel>(new CommitViewModel[0], 1, 0);
        }
    }
}
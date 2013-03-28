namespace GitNinja.Gallery.Models.View
{
    public abstract class RepositoryBasedViewModel
    {
        public string Dojo { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        protected RepositoryBasedViewModel(Repo repo)
        {
            Dojo = repo.Dojo;
            Name = repo.Name;
            Url = repo.Url;
        }
    }
}
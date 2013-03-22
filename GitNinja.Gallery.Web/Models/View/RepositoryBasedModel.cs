namespace GitNinja.Gallery.Web.Models.View
{
    public abstract class RepositoryBasedModel
    {
        public string Dojo { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        protected RepositoryBasedModel(Repo repo)
        {
            Dojo = repo.Dojo;
            Name = repo.Name;
            Url = repo.Url;
        }
    }
}
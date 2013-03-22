namespace GitNinja.Gallery.Web.Models.View
{
    public class Blob : RepositoryBasedModel
    {
        public Blob(Repo repo) : base(repo)
        {
        }

        public string Path { get; set; }
        public string FileName
        {
            get
            {
                var slashPos = Path.LastIndexOf('/');

                if (slashPos > 0)
                {
                    return Path.Substring(++slashPos);
                }
                return Path;
            }
        }
        public string Extension
        {
            get
            {
                var dotPos = FileName.LastIndexOf('.');
                if (dotPos > 0)
                {
                    return FileName.Substring(++dotPos);
                }
                return FileName;
            }
        }
        public long SizeInBytes { get; set; }
        public string Content { get; set; }

        public string ParentPath
        {
            get
            {
                var lastSlash = Path.LastIndexOf("/", System.StringComparison.Ordinal);

                if (lastSlash < 0)
                {
                    return "";
                }
                return Path.Substring(0, lastSlash);
            }
        }

        public string Reference { get; set; }
    }
}
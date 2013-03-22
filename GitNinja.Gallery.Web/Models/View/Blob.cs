namespace GitNinja.Gallery.Web.Models.View
{
    public class Blob : TreeBase
    {
        public Blob(Repo repo) : base(repo)
        {
        }

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
    }
}
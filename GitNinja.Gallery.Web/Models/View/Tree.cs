using System.Collections.Generic;

namespace GitNinja.Gallery.Web.Models.View
{
    public class Tree : RepositoryBasedModel
    {
        public Tree(Repo repo) : base(repo)
        {
        }

        public string Reference { get; set; }
        public string Path { get; set; }

        public bool IsRoot
        {
            get { return string.IsNullOrEmpty(Path); }
        }

        public IEnumerable<TreeEntry> Elements { get; set; }

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
    }
}
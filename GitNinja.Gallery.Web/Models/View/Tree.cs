using System.Collections.Generic;

namespace GitNinja.Gallery.Web.Models.View
{
    public abstract class TreeBase : RepositoryBasedModel
    {
        protected TreeBase(Repo repo) : base(repo)
        {
        }

        public string Reference { get; set; }
        public string Path { get; set; }

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

    public class Tree : TreeBase
    {
        public Tree(Repo repo) : base(repo)
        {
        }
        
        public bool IsRoot
        {
            get { return string.IsNullOrEmpty(Path); }
        }

        public IEnumerable<TreeEntry> Elements { get; set; }

    }
}
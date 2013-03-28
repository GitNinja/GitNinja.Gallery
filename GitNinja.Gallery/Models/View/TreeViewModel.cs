using System.Collections.Generic;

namespace GitNinja.Gallery.Models.View
{
    public abstract class TreeBaseViewModel : RepositoryBasedViewModel
    {
        protected TreeBaseViewModel(Repo repo) : base(repo)
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

    public class TreeViewModel : TreeBaseViewModel
    {
        public TreeViewModel(Repo repo) : base(repo)
        {
        }
        
        public bool IsRoot
        {
            get { return string.IsNullOrEmpty(Path); }
        }

        public IEnumerable<TreeEntryViewModel> Elements { get; set; }

    }
}
using System;

namespace GitNinja.Gallery.Models.View
{
    public class TreeEntryViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }

        public CommitViewModel LastCommit { get; set; }

        public bool IsDirectory { get { return Type.Equals("Directory", StringComparison.InvariantCultureIgnoreCase); } }
    }
}
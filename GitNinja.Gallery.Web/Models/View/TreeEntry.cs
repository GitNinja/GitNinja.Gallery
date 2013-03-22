using System;

namespace GitNinja.Gallery.Web.Models.View
{
    public class TreeEntry
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }

        public Commit LastCommit { get; set; }

        public bool IsDirectory { get { return Type.Equals("Directory", StringComparison.InvariantCultureIgnoreCase); } }
    }
}
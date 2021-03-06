﻿using System;

namespace GitNinja.Gallery.Models.View
{
    public class CommitViewModel
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string AuthorMail { get; set; }
        public string Committer { get; set; }
        public string CommitterMail { get; set; }
        public string MessageShort { get; set; }
        public string Message { get; set; }
        public DateTimeOffset AuthoredWhen { get; set; }
        public DateTimeOffset CommittedWhen { get; set; }
    }
}
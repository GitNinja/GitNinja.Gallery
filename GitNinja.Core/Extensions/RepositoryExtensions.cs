using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;

namespace GitNinja.Core.Extensions
{
    public static class RepositoryExtensions
    {

        /// TODO: This method is very simple as of now. It is not performing well, and cannot detect renames/deletes
        public static Commit FindLastChangingCommit(this Repository repo, Commit start, string path)
        {
            if (start == null)
            {
                start = repo.Head.Tip;
                if(start == null)
                    return null;
            }

            var tree = start.Tree;
            var fileInTree = tree[path];
            if (fileInTree == null)
            {
                return null;
            }

            if (start.ParentsCount >= 1)
            {
                var commits = new List<Commit>(){ start };
                foreach (var parent in start.Parents)
                {
                    var fileInParentTree = parent.Tree[path];

                    if (fileInParentTree == null)
                        continue;

                    if (fileInTree.Target.Sha == fileInParentTree.Target.Sha)
                    {
                        var commitPerParent = FindLastChangingCommit(repo, parent, path);
                        if (commitPerParent != null)
                        {
                            commits.Add(commitPerParent);
                        }
                    }
                }
                
                return commits.OrderBy(c => c.Committer.When)
                              .FirstOrDefault();
            }
            return null;
        }

        public static Commit FindCommitForReference(this Repository gitRepo, string reference)
        {
            // Reference might be to a tag, branch or commit
            var branch = gitRepo.Branches[reference];
            var tag = gitRepo.Tags[reference];
            var commit = gitRepo.Lookup<Commit>(reference);

            if (branch != null)
            {
                commit = branch.Tip;
            }
            else if (tag != null)
            {
                commit = tag.Target as Commit;
            }

            if (branch == null && tag == null && commit == null)
            {
                throw new ArgumentException("Reference is neither branch, nor tag, nor commit.");
            }
            return commit;
        }

        public static Tree FindTreeForPath(this Repository repo, Commit start, string path)
        {

            var tree = start.Tree[path];
            if (tree == null)
            {
                return start.Tree;
            }

            var subTree = tree.Target as Tree;
            if (subTree == null)
            {
                return start.Tree;
            }

            return subTree;
        }
    }
}
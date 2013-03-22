using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LibGit2Sharp;

namespace GitNinja.Gallery.Web.Helpers
{
    public static class RepositoryExtensions
    {

        /// TODO: This method is very simple as of now. It is not performing well, and cannot detect renames/deletes/merges
        public static Commit FindLastChangingCommit(this Repository repo, Commit start, string path)
        {
            if (start == null)
            {
                start = repo.Head.Tip;
            }

            var tree = start.Tree;
            var fileInTree = tree[path];

            Commit lastCommit = null;

            if (start.ParentsCount == 1)
            {
                var parent = start.Parents.First();
                var fileInParentTree = parent.Tree[path];

                if (fileInParentTree == null)
                {
                    return start;
                }

                if (fileInTree.Target.Sha == fileInParentTree.Target.Sha)
                {
                    return FindLastChangingCommit(repo, parent, path);
                }

                return start;
            }
            else
            {
                return lastCommit;
            }
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
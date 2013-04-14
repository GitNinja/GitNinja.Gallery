using GitNinja.Core.Extensions;
using GitNinja.Core.Tests.TestHelpers;
using LibGit2Sharp;
using NUnit.Framework;

namespace GitNinja.Core.Tests.Extensions
{
    public class RepositoryExtensionsTests
    {
        [TestFixture]
        public class FindLastChangingCommitTests : BaseTests
        {
            [Test]
            public void Returns_Null_If_Repository_Has_No_Commits()
            {
                var repo = new Repository(PathToBareRepository);

                var commit = repo.FindLastChangingCommit(null, "");

                Assert.That(commit, Is.Null);
            }

            [Test]
            public void Returns_Null_If_Path_Not_In_Tree()
            {
                var repo = new Repository(PathToSimpleRepository);

                var commit = repo.FindLastChangingCommit(null, "non-existing-directory");

                Assert.That(commit, Is.Null);
            }
            
            [Test]
            public void Returns_Latest_Commit_For_Changed_Files()
            {
                var repo = new Repository(PathToSimpleRepository);

                var commit = repo.FindLastChangingCommit(null, "first-level-directory/first-file.txt");

                /*
                 * Commit expected
                 * Sha: 59fa440f92e4f8f812fb22c227ac484a517029cd
                 * Author: tjdecke <tom.joedecke@googlemail.com>
                 * Date: Sun Apr 14 16:14:56 2013 +0100
                 * changed first-file.txt
                 */
                Assert.That(commit.Id.Sha, Is.EqualTo("59fa440f92e4f8f812fb22c227ac484a517029cd"));
            }

            [Test]
            public void Returns_Rename_Commit_For_Renamed_Files()
            {
                var repo = new Repository(PathToSimpleRepository);

                var commit = repo.FindLastChangingCommit(null, "first-level-directory/second-renamed-file.txt");

                /*
                 * Commit expected
                 * Sha: 225b729865a3d2c70db5ffecdb53e95eb870b3fe
                 * Author: tjdecke <tom.joedecke@googlemail.com>
                 * Date: Sun Apr 14 16:16:53 2013 +0100
                 * renamed second-file.txt to second-renamed-file.txt
                 */
                Assert.That(commit.Sha, Is.EqualTo("225b729865a3d2c70db5ffecdb53e95eb870b3fe"));
            }

            [Test]
            public void Returns_Tip_If_It_Was_The_Last_Change()
            {
                var repo = new Repository(PathToSimpleRepository);

                var commit = repo.FindLastChangingCommit(null, "first-level-directory/second-level-directory/third-file.txt");

                /*
                 * Commit expected
                 * Sha:  061b35ca18c50a38e368bfaf6ece0fef475ab0c8
                 * Author: tjdecke <tom.joedecke@googlemail.com>
                 * Date: Sun Apr 14 16:15:38 2013 +0100
                 * added second-level-directory and third-file.txt
                 */
                Assert.That(commit.Sha, Is.EqualTo("061b35ca18c50a38e368bfaf6ece0fef475ab0c8"));
            }
        }

    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitNinja.Core.Tests.TestHelpers
{
    public class BaseTests
    {
        static BaseTests()
        {
            CopyTestRepositories();
        }

        public BaseTests()
        {
            PathToBareRepository = "Resources/Bare";
            PathToSimpleRepository = "Resources/SimpleNoMerges";
        }

        private static void CopyTestRepositories()
        {
            string fromPath = "../../Resources";
            string toPath = "Resources";

            if (Directory.Exists(toPath))
            {
                new DirectoryInfo(toPath).Delete(true);
            }
            CopyFilesRecursively(new DirectoryInfo(fromPath), new DirectoryInfo(toPath));
        }

        public string PathToSimpleRepository { get; set; }
        public string PathToBareRepository { get; set; }

        private static readonly Dictionary<string, string> toRename = new Dictionary<string, string>
        {
            { "dot_git", ".git" },
            { "gitmodules", ".gitmodules" },
        };

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            // From http://stackoverflow.com/questions/58744/best-way-to-copy-the-entire-contents-of-a-directory-in-c/58779#58779
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                CopyFilesRecursively(dir, target.CreateSubdirectory(Rename(dir.Name)));
            }
            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, Rename(file.Name)));
            }
        }

        private static string Rename(string name)
        {
            return toRename.ContainsKey(name) ? toRename[name] : name;
        }
    }
}

using System.Collections.Generic;
using System.Text;

namespace GitNinja.Gallery.Models.View
{
    public class PathSegmentViewModel
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public bool IsLast { get; private set; }

        public static IEnumerable<PathSegmentViewModel> FromPath(string path)
        {
            var segments = new List<PathSegmentViewModel>();

            if (string.IsNullOrEmpty(path))
                return segments;

            var splitPath = path.Split('/');
            for (var i = 0; i < splitPath.Length; i++)
            {
                var segment = new PathSegmentViewModel();
                segment.Name = splitPath[i];
                
                var sb = new StringBuilder();
                for (var j = 0; j <= i; j++)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append('/');
                    }
                    sb.Append(splitPath[j]);
                }

                if (i == (splitPath.Length-1))
                {
                    segment.IsLast = true;
                }
                segment.Path = sb.ToString();

                segments.Add(segment);
            }
            return segments;
        }
    }
}
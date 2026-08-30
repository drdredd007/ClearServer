using System;
using System.IO;

namespace ClearServer.Core.Security
{
    internal static class SafePath
    {
        public static bool TryResolve(string baseDirectory, string requestUrl, out string fullPath)
        {
            fullPath = null;
            if (string.IsNullOrEmpty(requestUrl))
            {
                return false;
            }

            string baseFull = Path.GetFullPath(baseDirectory + Path.DirectorySeparatorChar);
            string relative = requestUrl.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);

            string candidate;
            try
            {
                candidate = Path.GetFullPath(Path.Combine(baseFull, relative));
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }

            if (!candidate.StartsWith(baseFull, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            fullPath = candidate;
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Services
{
    public static class DirectoryInfoExtensions
    {
        public static long GetDirectorySize(this DirectoryInfo directoryInfo, bool recursive = true)
        {
            long result = 0;
            if (directoryInfo == null || !directoryInfo.Exists)
                return result;

            foreach (var fileInfo in directoryInfo.GetFiles())
                result += fileInfo.Length;

            if (recursive)
            {
                foreach (var subDirectory in directoryInfo.GetDirectories())
                    result += GetDirectorySize(subDirectory, recursive);
            }

            return result;
        }
    }
}

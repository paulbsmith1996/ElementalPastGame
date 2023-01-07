using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Resources.Sounds
{
    public static class SoundsDirectoryLocator
    {
        private const string myRelativePath = nameof(SoundsDirectoryLocator) + ".cs";
        private static string? lazyLocation;
        public static string Location => lazyLocation ??= calculatePath();

        private static string calculatePath()
        {
            string pathName = GetSourceFilePathName();
            return pathName.Substring(0, pathName.Length - myRelativePath.Length);
        }

        public static string GetSourceFilePathName([CallerFilePath] string? callerFilePath = null) //
        => callerFilePath ?? "";
    }
}

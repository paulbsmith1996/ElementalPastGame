using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace ElementalPastGame.Resources.Textures
{
    public static class TextureDirectoryLocator
    {
        private const string myRelativePath = nameof(TextureDirectoryLocator) + ".cs";
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

using System;
using System.IO;
using System.Reflection;

namespace SimplyFast
{
    public static class AppEx
    {
        private static string _executablePath;
        private static bool? _isRunningOnMono;

        public static Version ExecutableVersion => Assembly.GetEntryAssembly().GetName().Version;

        public static string ExecutableDirectory => Path.GetDirectoryName(ExecutablePath);

        public static bool IsRunningOnMono
        {
            get
            {
                if (!_isRunningOnMono.HasValue)
                    _isRunningOnMono = Type.GetType("Mono.Runtime") != null;
                return _isRunningOnMono.Value;
            }
        }

        public static string ExecutablePath
        {
            get
            {
                if (_executablePath != null)
                    return _executablePath;
                var entryAssembly = Assembly.GetEntryAssembly();
                var uri = new Uri(entryAssembly.CodeBase);
                _executablePath = !uri.IsFile
                    ? uri.ToString()
                    : uri.LocalPath + Uri.UnescapeDataString(uri.Fragment);
                return _executablePath;
            }
        }

        public static string CombineExecutableDirectory(string fileName)
        {
            return Path.Combine(ExecutableDirectory, fileName);
        }
    }
}
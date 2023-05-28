using System.IO;
using UnityEngine;

namespace olimsko
{
    public static class BasePath
    {

        public static string PackageRootPath => GetPackageRootPath();

        public static string ConfigurationPath => BaseUtil.CombinePath(PackageRootPath, Configuration);
        public static string ConfigurationResourcePath => BaseUtil.CombinePath(PackageRootPath, $"{Configuration}/Resouces/");

        public static string PrefabsPath => BaseUtil.CombinePath(PackageRootPath, "Prefabs");
        public static string GeneratedDataPath => GetGeneratedDataPath();

        public const string Configuration = "Configuration";
        private const string PackageName = "olimsko";
        private const string DefaultGeneratedDataPath = "olimsko";
        private static string cachedPackageRootPath;

        private static string GetPackageRootPath()
        {
            cachedPackageRootPath = BaseUtil.CombinePath(Application.dataPath, PackageName);
            return cachedPackageRootPath;
        }

        private static string GetGeneratedDataPath()
        {
            var localPath = DefaultGeneratedDataPath;
            var path = BaseUtil.CombinePath(Application.dataPath, localPath);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }
}

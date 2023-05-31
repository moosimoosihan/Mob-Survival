using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace olimsko
{
    public static class BaseUtil
    {
        public static class IO
        {
            public static async UniTask<byte[]> ReadFileAsync(string filePath)
            {
                if (Application.platform == RuntimePlatform.WebGLPlayer) return File.ReadAllBytes(filePath);
                else
                {
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        var fileData = new byte[fileStream.Length];
                        await fileStream.ReadAsync(fileData, 0, (int)fileStream.Length);
                        return fileData;
                    }
                }
            }

            public static async UniTask WriteFileAsync(string filePath, byte[] fileData)
            {
                if (Application.platform == RuntimePlatform.WebGLPlayer) File.WriteAllBytes(filePath, fileData);
                else
                {
                    using (var fileStream = File.OpenWrite(filePath))
                        await fileStream.WriteAsync(fileData, 0, fileData.Length);
                }
            }

            public static async UniTask<string> ReadTextFileAsync(string filePath)
            {
                if (Application.platform == RuntimePlatform.WebGLPlayer) return File.ReadAllText(filePath);
                else
                {
                    using (var stream = File.OpenText(filePath))
                        return await stream.ReadToEndAsync();
                }
            }

            public static async UniTask WriteTextFileAsync(string filePath, string fileText)
            {
                if (Application.platform == RuntimePlatform.WebGLPlayer) File.WriteAllText(filePath, fileText);
                else
                {
                    using (var stream = File.CreateText(filePath))
                        await stream.WriteAsync(fileText);
                }
            }

            public static void DeleteFile(string filePath)
            {
                File.Delete(filePath);
            }

            public static void MoveFile(string sourceFilePath, string destFilePath)
            {
                File.Delete(destFilePath);
                File.Move(sourceFilePath, destFilePath);
            }

            public static void CreateDirectory(string path)
            {
                Directory.CreateDirectory(path);
            }

            public static void DeleteDirectory(string path, bool recursive)
            {
                Directory.Delete(path, recursive);
            }
        }

        public static class String
        {
            public static async UniTask<byte[]> ZipStringAsync(string content)
            {
                using (var output = new MemoryStream())
                {
                    using (var gzip = new DeflateStream(output, CompressionMode.Compress))
                    {
                        using (var writer = new StreamWriter(gzip, Encoding.UTF8))
                        {
                            await writer.WriteAsync(content);
                        }
                    }

                    return output.ToArray();
                }
            }

            public static async UniTask<string> UnzipStringAsync(byte[] content)
            {
                using (var inputStream = new MemoryStream(content))
                {
                    using (var gzip = new DeflateStream(inputStream, CompressionMode.Decompress))
                    {
                        using (var reader = new StreamReader(gzip, Encoding.UTF8))
                        {
                            return await reader.ReadToEndAsync();
                        }
                    }
                }
            }
        }

        public static Texture2D ToTexture2D(this RenderTexture renderTexture)
        {
            var texture2d = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            texture2d.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2d.Apply();
            return texture2d;
        }

        public static void DestroyOrImmediate(UnityEngine.Object obj)
        {
            if (!IsValid(obj)) return;

            if (Application.isPlaying)
                UnityEngine.Object.Destroy(obj);
            else UnityEngine.Object.DestroyImmediate(obj);
        }

        public static bool IsValid(object obj)
        {
            if (obj is UnityEngine.Object unityObject)
                return unityObject != null && unityObject;
            else return false;
        }

        public static string GetAfter(this string content, string matchString, StringComparison comp = StringComparison.Ordinal)
        {
            if (content.Contains(matchString))
            {
                var startIndex = content.LastIndexOf(matchString, comp) + matchString.Length;
                if (content.Length <= startIndex) return string.Empty;
                return content.Substring(startIndex);
            }
            else return null;
        }

        public static string GetAfterFirst(this string content, string matchString, StringComparison comp = StringComparison.Ordinal)
        {
            if (content.Contains(matchString))
            {
                var startIndex = content.IndexOf(matchString, comp) + matchString.Length;
                if (content.Length <= startIndex) return string.Empty;
                return content.Substring(startIndex);
            }
            else return null;
        }

        public static string GetBefore(this string content, string matchString, StringComparison comp = StringComparison.Ordinal)
        {
            if (content.Contains(matchString))
            {
                var endIndex = content.IndexOf(matchString, comp);
                return content.Substring(0, endIndex);
            }
            else return null;
        }

        public static string GetBeforeLast(this string content, string matchString, StringComparison comp = StringComparison.Ordinal)
        {
            if (content.Contains(matchString))
            {
                var endIndex = content.LastIndexOf(matchString, comp);
                return content.Substring(0, endIndex);
            }
            else return null;
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property, IEqualityComparer<TKey> propertyComparer = null)
        {
            var comparer = new GeneralPropertyComparer<T, TKey>(property, propertyComparer);
            return items.Distinct(comparer);
        }

        public static string AbsoluteToAssetPath(string absolutePath)
        {
            absolutePath = absolutePath.Replace("\\", "/");
            if (!absolutePath.StartsWithFast(Application.dataPath)) return null;
            return "Assets" + absolutePath.Replace(Application.dataPath, string.Empty);
        }

        public static string CombinePath(params string[] paths)
        {
            return Path.Combine(paths).Replace("\\", "/");
        }

        public static bool StartsWithFast(this string content, string match)
        {
            return content.StartsWith(match, StringComparison.Ordinal);
        }

        public static bool EqualsFast(this string content, string comparedString)
        {
            return content.Equals(comparedString, StringComparison.Ordinal);
        }

        public static IReadOnlyCollection<Assembly> GetDomainAssemblies(bool excludeDynamic = true, bool excludeSystem = false, bool excludeUnity = false)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(a =>
                (!excludeDynamic || !a.IsDynamic) &&
                (!excludeSystem || !a.GlobalAssemblyCache && !a.FullName.StartsWithFast("System") && !a.FullName.StartsWithFast("mscorlib") && !a.FullName.StartsWithFast("netstandard")) &&
                (!excludeUnity || !a.FullName.StartsWithFast("UnityEditor") && !a.FullName.StartsWithFast("UnityEngine") && !a.FullName.StartsWithFast("Unity.") &&
                    !a.FullName.StartsWithFast("nunit.") && !a.FullName.StartsWithFast("ExCSS.") && !a.FullName.StartsWithFast("UniTask.") &&
                    !a.FullName.StartsWithFast("UniRx.") && !a.FullName.StartsWithFast("JetBrains.") && !a.FullName.StartsWithFast("Newtonsoft."))
            ).ToArray();
        }

        public static string InsertCamel(this string source, char insert = ' ', bool preserveAcronyms = true)
        {
            if (string.IsNullOrWhiteSpace(source) || source.Length < 2)
                return source;

            bool IsUpperOrNumber(char ch) => char.IsUpper(ch) || char.IsNumber(ch);

            var builder = new StringBuilder(source.Length * 2);

            builder.Append(source[0]);
            for (int i = 1; i < source.Length; i++)
            {
                if (IsUpperOrNumber(source[i]))
                {
                    if (source[i - 1] != insert && !IsUpperOrNumber(source[i - 1]) || preserveAcronyms && IsUpperOrNumber(source[i - 1]) && i < source.Length - 1 && !IsUpperOrNumber(source[i + 1]))
                        builder.Append(insert);
                }
                builder.Append(source[i]);
            }

            return builder.ToString();
        }

        public static void ForEachDescendant(this GameObject gameObject, Action<GameObject> action, bool invokeOnSelf = true)
        {
            if (invokeOnSelf) action?.Invoke(gameObject);
            foreach (Transform childTransform in gameObject.transform)
                ForEachDescendant(childTransform.gameObject, action, true);
        }

        public static bool IsPointOverUI(GameObject go)
        {
            if (!Application.isPlaying) return false;
            bool isOver = false;
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> listRayResult = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, listRayResult);
            for (int i = 0; i < listRayResult.Count; i++)
            {
                if (listRayResult[i].gameObject.name == go.name)
                {
                    isOver = true;
                    break;
                }
            }

            return isOver;
        }
        public static bool IsIndexValid<T>(this T[] array, int index)
        {
            return array.Length > 0 && index >= 0 && index < array.Length;
        }

        public static bool IsIndexValid<T>(this List<T> list, int index)
        {
            return list.Count > 0 && index >= 0 && index < list.Count;
        }

        public static bool IsIndexValid<T>(this IReadOnlyCollection<T> list, int index)
        {
            return list.Count > 0 && index >= 0 && index < list.Count;
        }

        public static bool TryInvariantInt(string str, out int result)
        {
            return int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        public static bool TryInvariantFloat(string str, out float result)
        {
            return float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }

        public static int? AsInvariantInt(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;
            var succeeded = TryInvariantInt(str, out var result);
            return succeeded ? (int?)result : null;
        }

        public static float? AsInvariantFloat(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;
            var succeeded = TryInvariantFloat(str, out var result);
            return succeeded ? (float?)result : null;
        }

        public static IList<T> TopologicalOrder<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies, bool warnCyclic = true)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
                Visit(item);

            return sorted;

            void Visit(T item)
            {
                var alreadyVisited = visited.TryGetValue(item, out var inProcess);

                if (alreadyVisited)
                {
                    if (inProcess && warnCyclic)
                        Debug.LogWarning($"Cyclic dependency found while performing topological ordering of {typeof(T).Name}.");
                }
                else
                {
                    visited[item] = true;

                    var dependencies = getDependencies(item);
                    if (dependencies != null)
                    {
                        foreach (var dependency in dependencies)
                            Visit(dependency);
                    }

                    visited[item] = false;
                    sorted.Add(item);
                }
            }
        }
    }

    public class GeneralPropertyComparer<T, TKey> : IEqualityComparer<T>
    {
        private Func<T, TKey> property;
        private IEqualityComparer<TKey> propertyComparer;

        public GeneralPropertyComparer(Func<T, TKey> property, IEqualityComparer<TKey> propertyComparer = null)
        {
            this.property = property;
            this.propertyComparer = propertyComparer;
        }

        public bool Equals(T first, T second)
        {
            var firstProperty = property.Invoke(first);
            var secondProperty = property.Invoke(second);
            if (propertyComparer != null) return propertyComparer.Equals(firstProperty, secondProperty);
            if (firstProperty == null && secondProperty == null) return true;
            else if (firstProperty == null ^ secondProperty == null) return false;
            else return firstProperty.Equals(secondProperty);
        }

        public int GetHashCode(T obj)
        {
            var prop = property.Invoke(obj);
            if (propertyComparer != null) return propertyComparer.GetHashCode(prop);
            return prop == null ? 0 : prop.GetHashCode();
        }
    }




}

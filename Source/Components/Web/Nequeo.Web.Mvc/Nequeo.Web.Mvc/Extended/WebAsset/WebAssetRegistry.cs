/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;

using Nequeo.Extension;
using Nequeo.Threading.Extension;
using Nequeo.Collections.Extension;
using Nequeo.Web.Mvc.Extended.Factory;

namespace Nequeo.Web.Mvc.Extended.WebAsset
{
    /// <summary>
    /// Defines basic building blocks of Global storage for web assets.
    /// </summary>
    public interface IWebAssetRegistry : IWebAssetLocator
    {
        /// <summary>
        /// Stores the specified asset group.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="assetGroup">The asset group.</param>
        /// <returns></returns>
        string Store(string contentType, WebAssetItemGroup assetGroup);

        /// <summary>
        /// Retrieves the web asset by specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        WebAsset Retrieve(string id);
    }

    /// <summary>
    /// The default web asset registry.
    /// </summary>
    public class WebAssetRegistry : IWebAssetRegistry
    {
        private static readonly Regex urlRegEx =
            new Regex(@"url\s*\((\""|\')?(?<path>[^)]+)?(\""|\')?\)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);

        private static readonly ReaderWriterLockSlim syncLock = new ReaderWriterLockSlim();

        private readonly bool isInDebugMode;
        private readonly ICacheManager cacheManager;
        private readonly IWebAssetLocator assetLocator;
        private readonly IUrlResolver urlResolver;
        private readonly IPathResolver pathResolver;
        private readonly IVirtualPathProvider virtualPathProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAssetRegistry"/> class.
        /// </summary>
        /// <param name="isInDebugMode">if set to <c>true</c> [is in debug mode].</param>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="assetLocator">The asset locator.</param>
        /// <param name="urlResolver">The URL resolver.</param>
        /// <param name="pathResolver">The path resolver.</param>
        /// <param name="virtualPathProvider">The virtual path provider.</param>
        public WebAssetRegistry(bool isInDebugMode, ICacheManager cacheManager, IWebAssetLocator assetLocator, 
            IUrlResolver urlResolver, IPathResolver pathResolver, IVirtualPathProvider virtualPathProvider)
        {
            this.isInDebugMode = isInDebugMode;
            this.cacheManager = cacheManager;
            this.assetLocator = assetLocator;
            this.urlResolver = urlResolver;
            this.pathResolver = pathResolver;
            this.virtualPathProvider = virtualPathProvider;
        }

        /// <summary>
        /// Stores the specified asset group.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="assetGroup">The asset group.</param>
        /// <returns></returns>
        public string Store(string contentType, WebAssetItemGroup assetGroup)
        {
            MergedAsset mergedAsset = CreateMergedAssetWith(contentType, assetGroup);
            string id = assetGroup.IsShared ? assetGroup.Name : CreateIdFrom(mergedAsset);

            EnsureAsset(mergedAsset, id);

            return id;
        }

        /// <summary>
        /// Retrieves the web asset by specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public WebAsset Retrieve(string id)
        {
            MergedAsset mergedAsset = CreateMergedAssetFromConfiguration(id) ?? CreateMergedAssetFromUrl(id);
            WebAssetHolder assetHolder = EnsureAsset(mergedAsset, id);

            return new WebAsset(assetHolder.Asset.ContentType, assetHolder.Asset.Version, assetHolder.Asset.Compress, assetHolder.Asset.CacheDurationInDays, assetHolder.Content);
        }

        /// <summary>
        /// Returns the correct virtual path based upon the debug mode and version.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public string Locate(string virtualPath, string version)
        {
            return assetLocator.Locate(virtualPath, version);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static MergedAsset CreateMergedAssetFromConfiguration(string id)
        {
            WebAssetItemGroup assetGroup = SharedGroup.FindScriptGroup(id);
            string contentType = "application/x-javascript";

            if (assetGroup == null)
            {
                assetGroup = SharedGroup.FindStyleSheetGroup(id);
                contentType = "text/css";
            }

            return (assetGroup != null) ? CreateMergedAssetWith(contentType, assetGroup) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static MergedAsset CreateMergedAssetFromUrl(string id)
        {
            string decompressed = Decompress(Decode(id));
            MergedAsset mergedAsset = Deserialize(decompressed);

            return mergedAsset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mergedAsset"></param>
        /// <returns></returns>
        private static string CreateIdFrom(MergedAsset mergedAsset)
        {
            string serialized = Serialize(mergedAsset);
            string id = Encode(Compress(serialized));

            return id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="assetGroup"></param>
        /// <returns></returns>
        private static MergedAsset CreateMergedAssetWith(string contentType, WebAssetItemGroup assetGroup)
        {
            Func<string, string> getDirectory = path => path.Substring(2, path.LastIndexOf("/", StringComparison.Ordinal) - 2);
            Func<string, string> getFile = path => path.Substring(path.LastIndexOf("/", StringComparison.Ordinal) + 1);

            MergedAsset asset = new MergedAsset
            {
                ContentType = contentType,
                Version = assetGroup.Version,
                Compress = assetGroup.Compress,
                CacheDurationInDays = assetGroup.CacheDurationInDays
            };

            IEnumerable<string> directories = assetGroup.Items.Select(item => getDirectory(item.Source)).Distinct(StringComparer.OrdinalIgnoreCase);

            directories.Each(directory => asset.Directories.Add(new MergedAssetDirectory { Path = directory }));

            for (int i = 0; i < assetGroup.Items.Count; i++)
            {
                string item = assetGroup.Items[i].Source;
                string directory = getDirectory(item);
                string file = getFile(item);

                MergedAssetDirectory assetDirectory = asset.Directories.Single(d => d.Path.IsCaseInsensitiveEqual(directory));

                assetDirectory.Files.Add(new MergedAssetFile { Order = i, Name = file });
            }

            return asset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mergedAsset"></param>
        /// <returns></returns>
        private static string Serialize(MergedAsset mergedAsset)
        {
            JavaScriptSerializer serializer = CreateSerializer();

            string json = serializer.Serialize(mergedAsset);

            return json;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static MergedAsset Deserialize(string json)
        {
            JavaScriptSerializer serializer = CreateSerializer();

            MergedAsset mergedAsset = serializer.Deserialize<MergedAsset>(json);

            return mergedAsset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static string Encode(string target)
        {
            return target.Replace("/", "_").Replace("+", "-");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static string Decode(string target)
        {
            return target.Replace("-", "+").Replace("_", "/");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static string Compress(string target)
        {
            return target.Compress();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static string Decompress(string target)
        {
            return target.Decompress();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static JavaScriptSerializer CreateSerializer()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            serializer.RegisterConverters(new JavaScriptConverter[] 
            { 
                new MergedAssetJsonConverter(), 
                new MergedAssetDirectoryJsonConverter(), 
                new MergedAssetFileJsonConverter() });

            return serializer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private WebAssetHolder EnsureAsset(MergedAsset asset, string id)
        {
            string key = "{0}:{1}".FormatWith(GetType().AssemblyQualifiedName, id);
            WebAssetHolder assetHolder;

            using (syncLock.ReadAndWrite())
            {
                assetHolder = GetWebAssetHolder(key);

                if (assetHolder == null)
                {
                    using (syncLock.Write())
                    {
                        assetHolder = GetWebAssetHolder(key);

                        if (assetHolder == null)
                        {
                            List<string> physicalPaths = new List<string>();
                            StringBuilder contentBuilder = new StringBuilder();

                            var files = asset.Directories
                                             .SelectMany(d => d.Files.Select(f => new { Directory = d, File = f }))
                                             .OrderBy(f => f.File.Order);

                            foreach (var pair in files)
                            {
                                string path = "~/" + pair.Directory.Path + "/" + pair.File.Name;

                                string virtualPath = assetLocator.Locate(path, asset.Version);
                                string fileContent = virtualPathProvider.ReadAllText(virtualPath);

                                if (string.Compare(asset.ContentType, "text/css", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    string baseDiretory = virtualPathProvider.GetDirectory(virtualPath);

                                    fileContent = ReplaceImagePath(baseDiretory, asset.Version, fileContent);
                                }

                                contentBuilder.AppendLine(fileContent);

                                physicalPaths.Add(pathResolver.Resolve(virtualPath));
                            }

                            assetHolder = new WebAssetHolder { Asset = asset, Content = contentBuilder.ToString() };
                            cacheManager.Insert(key, assetHolder, null, physicalPaths.ToArray());
                        }
                    }
                }
            }

            return assetHolder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private WebAssetHolder GetWebAssetHolder(string key)
        {
            return isInDebugMode ? null : cacheManager.GetItem(key) as WebAssetHolder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseDiretory"></param>
        /// <param name="version"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private string ReplaceImagePath(string baseDiretory, string version, string content)
        {
            baseDiretory = AppendVersionNo(baseDiretory, version);

            content = urlRegEx.Replace(
                content,
                new MatchEvaluator(
                    match =>
                    {
                        //string virtualPath = assetLocator.Locate(path, asset.Version);

                        string path = match.Groups["path"].Value.Trim("'\"".ToCharArray());

                        if (!path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                        {
                            path = virtualPathProvider.CombinePaths(baseDiretory, path);

                            return "url('{0}')".FormatWith(urlResolver.Resolve(path));
                        }

                        return "url('{0}')".FormatWith(path);
                    }));

            return content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private string AppendVersionNo(string virtualPath, string version)
        {
            Func<string, string> fixPath = path =>
            {
                if (!path.EndsWith(version + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                {
                    string newPath = virtualPathProvider.CombinePaths(path, version);

                    if (virtualPathProvider.DirectoryExists(newPath))
                    {
                        return newPath;
                    }
                }

                return path;
            };

            if (!string.IsNullOrEmpty(version))
            {
                virtualPath = fixPath(virtualPath);
            }

            return virtualPath;
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        private sealed class WebAssetHolder
        {
            /// <summary>
            /// 
            /// </summary>
            public MergedAsset Asset
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string Content
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        private sealed class MergedAsset
        {
            /// <summary>
            /// 
            /// </summary>
            public MergedAsset()
            {
                Directories = new List<MergedAssetDirectory>();
            }

            /// <summary>
            /// 
            /// </summary>
            public string ContentType
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string Version
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public bool Compress
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public float CacheDurationInDays
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public IList<MergedAssetDirectory> Directories
            {
                get;
                private set;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        private sealed class MergedAssetDirectory
        {
            /// <summary>
            /// 
            /// </summary>
            public MergedAssetDirectory()
            {
                Files = new List<MergedAssetFile>();
            }

            /// <summary>
            /// 
            /// </summary>
            public string Path
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public IList<MergedAssetFile> Files
            {
                get;
                private set;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        private sealed class MergedAssetFile
        {
            /// <summary>
            /// 
            /// </summary>
            public int Order
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string Name
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class MergedAssetJsonConverter : JavaScriptConverter
        {
            /// <summary>
            /// 
            /// </summary>
            public override IEnumerable<Type> SupportedTypes
            {
                get
                {
                    yield return typeof(MergedAsset);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="serializer"></param>
            /// <returns></returns>
            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                MergedAsset mergedAsset = (MergedAsset)obj;

                IDictionary<string, object> dictionary = new Dictionary<string, object>
                                                             {
                                                                 { "ct", mergedAsset.ContentType },
                                                                 { "v", mergedAsset.Version },
                                                                 { "c", mergedAsset.Compress },
                                                                 { "cd", mergedAsset.CacheDurationInDays },
                                                                 { "d", mergedAsset.Directories }
                                                             };

                return dictionary;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dictionary"></param>
            /// <param name="type"></param>
            /// <param name="serializer"></param>
            /// <returns></returns>
            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                MergedAsset mergedAsset = new MergedAsset
                {
                    ContentType = serializer.ConvertToType<string>(dictionary["ct"]),
                    Version = serializer.ConvertToType<string>(dictionary["v"]),
                    Compress = serializer.ConvertToType<bool>(dictionary["c"]),
                    CacheDurationInDays = serializer.ConvertToType<float>(dictionary["cd"])
                };

                mergedAsset.Directories.AddRange(serializer.ConvertToType<IList<MergedAssetDirectory>>(dictionary["d"]));

                return mergedAsset;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class MergedAssetDirectoryJsonConverter : JavaScriptConverter
        {
            /// <summary>
            /// 
            /// </summary>
            public override IEnumerable<Type> SupportedTypes
            {
                get
                {
                    yield return typeof(MergedAssetDirectory);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="serializer"></param>
            /// <returns></returns>
            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                MergedAssetDirectory mergedAssetDirectory = (MergedAssetDirectory)obj;

                IDictionary<string, object> dictionary = new Dictionary<string, object>
                                                             {
                                                                 { "p", mergedAssetDirectory.Path },
                                                                 { "f", mergedAssetDirectory.Files }
                                                             };

                return dictionary;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dictionary"></param>
            /// <param name="type"></param>
            /// <param name="serializer"></param>
            /// <returns></returns>
            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                MergedAssetDirectory mergedAssetDirectory = new MergedAssetDirectory
                {
                    Path = serializer.ConvertToType<string>(dictionary["p"])
                };

                mergedAssetDirectory.Files.AddRange(serializer.ConvertToType<IList<MergedAssetFile>>(dictionary["f"]));

                return mergedAssetDirectory;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class MergedAssetFileJsonConverter : JavaScriptConverter
        {
            /// <summary>
            /// 
            /// </summary>
            public override IEnumerable<Type> SupportedTypes
            {
                get
                {
                    yield return typeof(MergedAssetFile);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="serializer"></param>
            /// <returns></returns>
            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                MergedAssetFile mergedAssetFile = (MergedAssetFile)obj;

                IDictionary<string, object> dictionary = new Dictionary<string, object>
                                                             {
                                                                 { "o", mergedAssetFile.Order },
                                                                 { "n", mergedAssetFile.Name }
                                                             };

                return dictionary;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dictionary"></param>
            /// <param name="type"></param>
            /// <param name="serializer"></param>
            /// <returns></returns>
            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                MergedAssetFile mergedAssetFile = new MergedAssetFile
                {
                    Order = serializer.ConvertToType<int>(dictionary["o"]),
                    Name = serializer.ConvertToType<string>(dictionary["n"])
                };

                return mergedAssetFile;
            }
        }
    }
}

using System.Collections;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;

namespace DevelopEngine
{
    public class LoadAssetBundle : MonoBehaviour
    {
        #region 单例模式
        private static LoadAssetBundle m_Instance;
        public static LoadAssetBundle Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = FindObjectOfType(typeof(LoadAssetBundle)) as LoadAssetBundle;
                if (m_Instance == null)
                {
                    m_Instance = new GameObject("Singlton of " + typeof(LoadAssetBundle).Name, typeof(LoadAssetBundle)).GetComponent<LoadAssetBundle>();
                }
                return m_Instance;
            }
        }
        #endregion

        #region 读取路径
        /// <summary>
        /// 本地文件存放路径
        /// </summary>
        public string LocalPath(string assetBundleName)
        {
            return Application.dataPath + "/AssetBundles/" + assetBundleName;
        }

        /// <summary>
        /// 服务器文件存放路径
        /// </summary>
        public string ServerPath(string assetBundleName)
        {
            //根据不同服务器，服务器端地址需要做修改
            return "http://localhost/AssetBundles/" + assetBundleName;
        }
        #endregion

        #region 加载依赖
        /// <summary>
        ///本地同步加载依赖
        /// </summary>
        public void LoadManifestFromFile(string assetBundleName)
        {
            string manifestPath = Application.dataPath + "/AssetBundles/AssetBundles";
            AssetBundle manifestAB = AssetBundle.LoadFromFile(manifestPath);
            AssetBundleManifest manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            string[] strs = manifest.GetAllDependencies(assetBundleName);
            foreach (string name in strs)
            {
                AssetBundle.LoadFromFile(LocalPath(name));
            }
        }

        /// <summary>
        /// 从内存中同步加载依赖
        /// </summary>
        /// <param name="assetBundleName"></param>
        public void LoadManifestFromMemory(string assetBundleName)
        {
            string manifestPath = Application.dataPath + "/AssetBundles/AssetBundles";
            AssetBundle manifestAB = AssetBundle.LoadFromMemory(File.ReadAllBytes(manifestPath));
            AssetBundleManifest manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            string[] strs = manifest.GetAllDependencies(assetBundleName);
            foreach (string name in strs)
            {
                AssetBundle.LoadFromMemory(File.ReadAllBytes(LocalPath(name)));
            }
        }

        /// <summary>
        /// 从内存中异步加载依赖
        /// </summary>
        public IEnumerator LoadManifestFromMemoryAsync(string assetBundleName)
        {
            string manifestPath = Application.dataPath + "/AssetBundles/AssetBundles";
            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(manifestPath));
            yield return request;

            AssetBundle manifestAB = request.assetBundle;
            AssetBundleManifest manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            string[] strs = manifest.GetAllDependencies(assetBundleName);
            foreach (string name in strs)
            {
                AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(LocalPath(name)));
            }
        }

        /// <summary>
        ///  WWW加载方式，可从本地上加载，也可以从服务器上加载依赖（废弃）
        /// </summary>
        public IEnumerator LoadManifestFromCacheOrDownload(bool isLocal, int version, string assetBundleName)
        {
            //while (!Caching.ready)
            //    yield return null;

            WWW wwwAB = null;
            if (!isLocal)
            {
                string assetBundlePath = ServerPath("AssetBundles");
                wwwAB = WWW.LoadFromCacheOrDownload(assetBundlePath, version);
            }
            else
            {
                //本地加载的话，路径地址前缀必须为file:// or file:///
                string assetBundlePath = LocalPath("AssetBundles");
                wwwAB = WWW.LoadFromCacheOrDownload("file://" + assetBundlePath, version);
            }
            yield return wwwAB;
            if (!string.IsNullOrEmpty(wwwAB.error))
            {
                Debug.Log(wwwAB.error);
                yield break;
            }
            AssetBundle manifestAB = wwwAB.assetBundle;
            AssetBundleManifest manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            string[] strs = manifest.GetAllDependencies(assetBundleName);
            foreach (string name in strs)
            {
                if (!isLocal)
                {
                    WWW.LoadFromCacheOrDownload(name, version);
                }
                else
                {
                    WWW.LoadFromCacheOrDownload("file://" + name, version);
                }
                //yield return wwwDependence;
                //if (!string.IsNullOrEmpty(wwwDependence.error))
                //{
                //    Debug.Log(wwwDependence.error);
                //    yield break;
                //}
                //AssetBundle abDependence = wwwDependence.assetBundle;
            }
        }

        /// <summary>
        /// 本地或服务器加载依赖
        /// </summary>
        public IEnumerator UnityManifestWebRequest(bool isLocal, string assetBundleName)
        {
            UnityEngine.Networking.UnityWebRequest manifestReq = null;
            if (!isLocal)
            {
                string assetBundlePath = ServerPath("AssetBundles");
                manifestReq = UnityEngine.Networking.UnityWebRequest.GetAssetBundle(assetBundlePath);
            }
            else
            {
                string assetBundlePath = LocalPath("AssetBundles");
                manifestReq = UnityEngine.Networking.UnityWebRequest.GetAssetBundle("file://" + assetBundlePath);
            }
            yield return manifestReq.Send();

            AssetBundle manifestAB = DownloadHandlerAssetBundle.GetContent(manifestReq);
            AssetBundleManifest manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            string[] strs = manifest.GetAllDependencies(assetBundleName);
            foreach (string name in strs)
            {
                UnityEngine.Networking.UnityWebRequest dependenceReq = null;
                if (!isLocal)
                {
                    dependenceReq = UnityEngine.Networking.UnityWebRequest.GetAssetBundle(ServerPath(name));
                }
                else
                {
                    dependenceReq = UnityEngine.Networking.UnityWebRequest.GetAssetBundle(LocalPath(name));
                }
                yield return dependenceReq.Send();
                AssetBundle dependenceAB = DownloadHandlerAssetBundle.GetContent(dependenceReq);
            }
        }
        #endregion

        #region 加载对象
        /// <summary>
        /// 本地同步加载
        /// </summary>
        public UnityEngine.Object LoadFromFile(bool hasDependence, string assetBundleName, string objName)
        {
            string assetBundlePath = LocalPath(assetBundleName);
            AssetBundle ab = AssetBundle.LoadFromFile(assetBundlePath);

            //if (!string.IsNullOrEmpty(dependenceName))
            //{
            //    string dependencePath = LocalPath(dependenceName);
            //    AssetBundle abDependence = AssetBundle.LoadFromFile(dependencePath);
            //}

            if(hasDependence)
                LoadManifestFromFile(assetBundleName);

            return ab.LoadAsset<UnityEngine.Object>(objName);
        }

        /// <summary>
        /// 本地同步加载
        /// </summary>
        //public UnityEngine.Object LoadFromFile(string assetBundleName, string objName)
        //{
        //    return LoadFromFile(assetBundleName, null, objName);
        //}

        /// <summary>
        /// 从内存中同步加载
        /// </summary>
        public UnityEngine.Object LoadFromMemory(bool hasDependence, string assetBundleName, string objName)
        {
            string assetBundlePath = LocalPath(assetBundleName);
            AssetBundle ab = AssetBundle.LoadFromMemory(File.ReadAllBytes(assetBundlePath));

            //if (!string.IsNullOrEmpty(dependenceName))
            //{
            //    string dependencePath = LocalPath(dependenceName);
            //    AssetBundle abDependence = AssetBundle.LoadFromMemory(File.ReadAllBytes(dependencePath));
            //}
            if(hasDependence)
                LoadManifestFromMemory(assetBundleName);

            return ab.LoadAsset<UnityEngine.Object>(objName);
        }

        /// <summary>
        /// 从内存中同步加载
        /// </summary>
        //public UnityEngine.Object LoadFromMemory(string assetBundleName, string objName)
        //{
        //    return LoadFromMemory(assetBundleName, null, objName);
        //}

        /// <summary>
        /// 从内存中异步加载
        /// </summary>
        public IEnumerator LoadFromMemoryAsync(bool hasDependence, string assetBundleName, string objName, Action<UnityEngine.Object> handler)
        {
            if (hasDependence)
            {
                string manifestPath = Application.dataPath + "/AssetBundles/AssetBundles";
                AssetBundleCreateRequest manifestReq = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(manifestPath));
                yield return manifestReq;

                AssetBundle manifestAB = manifestReq.assetBundle;
                AssetBundleManifest manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

                string[] strs = manifest.GetAllDependencies(assetBundleName);
                foreach (string name in strs)
                {
                    AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(LocalPath(name)));
                }
            }

            string assetBundlePath = LocalPath(assetBundleName);
            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(assetBundlePath));
            yield return request;
            AssetBundle ab = request.assetBundle;

            //if (!string.IsNullOrEmpty(dependenceName))
            //{
            //    string dependencePath = LocalPath(dependenceName);
            //    AssetBundleCreateRequest dependenceRequest = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(dependencePath));
            //    yield return dependenceRequest;
            //    AssetBundle abDependence = dependenceRequest.assetBundle;
            //}

            var go = ab.LoadAsset<UnityEngine.Object>(objName);
            if (handler != null)
                handler(go);
        }

        /// <summary>
        /// 从内存中异步加载
        /// </summary>
        //public void LoadFromMemoryAsync(string assetBundleName, string objName, Action<UnityEngine.Object> handler)
        //{
        //    StartCoroutine(LoadFromMemoryAsync(assetBundleName, null, objName, handler));
        //}

        /// <summary>
        /// WWW加载方式，可从本地上加载，也可以从服务器上加载（废弃）
        /// </summary>
        public IEnumerator LoadFromCacheOrDownload(bool isLocal, bool hasDependence, string assetBundleName, int version, string objName, Action<UnityEngine.Object> handler)
        {
            while (!Caching.ready)
                yield return null;

            if(hasDependence)
            {
                WWW wwwAB = null;
                if (!isLocal)
                {
                    string assetBundlePath = ServerPath("AssetBundles");
                    wwwAB = WWW.LoadFromCacheOrDownload(assetBundlePath, version);
                }
                else
                {
                    //本地加载的话，路径地址前缀必须为file:// or file:///
                    string assetBundlePath = LocalPath("AssetBundles");
                    wwwAB = WWW.LoadFromCacheOrDownload("file://" + assetBundlePath, version);
                }
                yield return wwwAB;
                if (!string.IsNullOrEmpty(wwwAB.error))
                {
                    Debug.Log(wwwAB.error);
                    yield break;
                }
                AssetBundle manifestAB = wwwAB.assetBundle;
                AssetBundleManifest manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

                string[] strs = manifest.GetAllDependencies(assetBundleName);
                foreach (string name in strs)
                {
                    //WWW wwwDependence = null;
                    if (!isLocal)
                    {
                        WWW.LoadFromCacheOrDownload(name, version);
                    }
                    else
                    {
                        WWW.LoadFromCacheOrDownload("file://" + name, version);
                    }
                }
                //    yield return wwwDependence;
                //    if (!string.IsNullOrEmpty(wwwDependence.error))
                //    {
                //        Debug.Log(wwwDependence.error);
                //        yield break;
                //    }
                //    AssetBundle abDependence = wwwDependence.assetBundle;
               
            }

            WWW www = null;
            if (!isLocal)
            {
                string assetBundlePath = ServerPath(assetBundleName);
                www = WWW.LoadFromCacheOrDownload(assetBundlePath, version);
            }
            else
            {
                //本地加载的话，路径地址前缀必须为file:// or file:///
                string assetBundlePath = LocalPath(assetBundleName);
                www = WWW.LoadFromCacheOrDownload("file://" + assetBundlePath, version);
            }
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.error);
                yield break;
            }
            AssetBundle ab = www.assetBundle;

            //if (!string.IsNullOrEmpty(dependenceName))
            //{
            //    WWW wwwDp = null;
            //    if (!isLocal)
            //    {
            //        string dependencePath = ServerPath(dependenceName);
            //        wwwDp = WWW.LoadFromCacheOrDownload(dependencePath, version);
            //    }
            //    else
            //    {
            //        string dependencePath = LocalPath(dependenceName);
            //        wwwDp = WWW.LoadFromCacheOrDownload("file://" + dependencePath, version);
            //    }
            //    yield return wwwDp;
            //    if (!string.IsNullOrEmpty(wwwDp.error))
            //    {
            //        Debug.Log(wwwDp.error);
            //        yield break;
            //    }
            //    AssetBundle abDependence = wwwDp.assetBundle;
            //}

            var go = ab.LoadAsset<UnityEngine.Object>(objName);
            if (handler != null)
                handler(go);
        }

        /// <summary>
        /// WWW加载方式，可从本地上加载，也可以从服务器上加载（废弃）
        /// </summary>
        //public void LoadFromCacheOrDownload(bool isLocal, string assetBundleName, int version, string objName, Action<UnityEngine.Object> handler)
        //{
        //    StartCoroutine(LoadFromCacheOrDownload(isLocal, assetBundleName, version, null, objName, handler));
        //}

        /// <summary>
        /// 本地或服务器加载
        /// </summary>
        public IEnumerator UnityWebRequest(bool isLocal, bool hasDependence, string assetBundleName, string objName, Action<UnityEngine.Object> handler)
        {
            if (hasDependence)
            {
                UnityEngine.Networking.UnityWebRequest manifestReq = null;
                if (!isLocal)
                {
                    string assetBundlePath = ServerPath("AssetBundles");
                    manifestReq = UnityEngine.Networking.UnityWebRequest.GetAssetBundle(assetBundlePath);
                }
                else
                {
                    string assetBundlePath = LocalPath("AssetBundles");
                    manifestReq = UnityEngine.Networking.UnityWebRequest.GetAssetBundle("file://" + assetBundlePath);
                }
                yield return manifestReq.Send();

                AssetBundle manifestAB = DownloadHandlerAssetBundle.GetContent(manifestReq);
                AssetBundleManifest manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

                string[] strs = manifest.GetAllDependencies(assetBundleName);
                foreach (string name in strs)
                {
                    UnityEngine.Networking.UnityWebRequest dependenceReq = null;
                    if (!isLocal)
                    {
                        dependenceReq = UnityEngine.Networking.UnityWebRequest.GetAssetBundle(ServerPath(name));
                    }
                    else
                    {
                        dependenceReq = UnityEngine.Networking.UnityWebRequest.GetAssetBundle(LocalPath(name));
                    }
                    yield return dependenceReq.Send();
                    AssetBundle dependenceAB = DownloadHandlerAssetBundle.GetContent(dependenceReq);
                }
            }

            UnityEngine.Networking.UnityWebRequest request = null;
            if (!isLocal)
            {
                string assetBundlePath = ServerPath(assetBundleName); 
                request = UnityEngine.Networking.UnityWebRequest.GetAssetBundle(assetBundlePath);
                yield return request.Send();
            }
            else
            {
                string assetBundlePath = LocalPath(assetBundleName); 
                request = UnityEngine.Networking.UnityWebRequest.GetAssetBundle("file://" + assetBundlePath);
                yield return request.Send();
            }
            
            AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request);
            //AssetBundle ab = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;

            //if (!string.IsNullOrEmpty(dependenceName))
            //{
            //    UnityEngine.Networking.UnityWebRequest requestDP = null;
            //    if (!isLocal)
            //    {
            //        string dependencePath = ServerPath(dependenceName); 
            //        requestDP = UnityEngine.Networking.UnityWebRequest.GetAssetBundle(dependencePath);
            //        yield return requestDP.Send();
            //    }
            //    else
            //    {
            //        string dependencePath = LocalPath(dependenceName); 
            //        requestDP = UnityEngine.Networking.UnityWebRequest.GetAssetBundle("file://" + dependencePath);
            //        yield return requestDP.Send();
            //    }

            //    AssetBundle abDependence = DownloadHandlerAssetBundle.GetContent(requestDP);
            //    //AssetBundle abDependence = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
            //}

            var go = ab.LoadAsset<UnityEngine.Object>(objName);
            if (handler != null)
                handler(go);
        }

        /// <summary>
        /// 服务器加载
        /// </summary>
        //public void UnityWebRequest(bool isLocal, string assetBundleName, string objName, Action<UnityEngine.Object> handler)
        //{
        //    StartCoroutine(UnityWebRequest(isLocal, assetBundleName, null, objName, handler));
        //}
        #endregion
    }
}

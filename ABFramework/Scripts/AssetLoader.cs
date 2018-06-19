using System.Collections;
using UnityEngine;
using System;

namespace ABFramework
{
    /// <summary>
    /// AssetBundle资源加载类
    ///     1：管理与加载指定AB资源。
    ///     2：加载具有“缓存功能”的资源，带选用参数。
    ///     3：卸载、释放AB资源。
    ///     4：查看当前AB资源。
    /// </summary>
	public class AssetLoader : IDisposable
    {

        private AssetBundle currentAssetBundle;
        private Hashtable hashTable;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ab">给定WWW加载的AssetBundle实例</param>
        public AssetLoader(AssetBundle ab)
        {
            if(ab != null)
            {
                currentAssetBundle = ab;
                hashTable = new Hashtable();
            }
            else
            {
                Debug.Log(GetType() + " /构造函数/ 参数为空, 请检查!");
            }
        }

        /// <summary>
        /// 加载当前包中指定的资源
        /// </summary>
        /// <param name="assetName">资源的名称</param>
        /// <param name="isCache">是否开启缓存</param>
        public UnityEngine.Object LoadAsset(string assetName, bool isCache = false)
        {
            return LoadResource<UnityEngine.Object>(assetName, isCache);
        }

        /// <summary>
        ///  加载当前AB包的资源，带缓存
        /// </summary>
        /// <param name="assetName">资源的名称</param>
        /// <param name="isCache">是否需要缓存处理</param>
        /// <returns></returns>
        private T LoadResource<T>(string assetName, bool isCache) where T: UnityEngine.Object
        {
            //是否缓存集合已经存在
            if (hashTable.Contains(assetName))
                return hashTable[assetName] as T;

            //缓存集合中不存在的话，进行加载
            T resource = currentAssetBundle.LoadAsset<T>(assetName);
            if (resource != null && isCache)
            {
                hashTable.Add(assetName, resource);
            }
            else if (resource == null)
            {
                Debug.LogError(GetType() + "/LoadResource<T>()/ 中resource为空，请检查！");
            }
            return resource;
        }

        /// <summary>
        /// 卸载指定的资源
        /// </summary>
        public bool UnloadAsset(UnityEngine.Object asset)
        {
            if (asset != null) 
            {
                Resources.UnloadAsset(asset);
                return true;
            }
            Debug.Log(GetType() + "/UnloadAsset()/ 参数为空，请检查！");
            return false;
        }

        /// <summary>
        /// 释放当前AssetBundle内存镜像资源
        /// </summary>
        public void Dispose()
        {
            currentAssetBundle.Unload(false);
        }

        /// <summary>
        /// 释放当前AssetBundle内存镜像资源和内存资源
        /// </summary>
        public void DisposeAll()
        {
            currentAssetBundle.Unload(true);
        }

        /// <summary>
        /// 查询当前AssetBundle中包含的所有资源
        /// </summary>
        public string[] RetrievalAllAssetNames()
        {
            return currentAssetBundle.GetAllAssetNames();
        }
    }
}

using System.Collections;
using UnityEngine;
using System;

namespace ABFramework
{
    /// <summary>
    /// WWW加载AssetBundle
    /// </summary>
	public class SingleABLoader : IDisposable
    {
        //引用类：资源加载类
        private AssetLoader _AssetLoader;
        //委托：
        private LoadCompleteHandler _LoadCompleteHandler;
        //AssetBundle 名称
        private string label;
        //AssetBundle 下载路径
        private string downloadPath;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">AB包名</param>
        public SingleABLoader(string label, LoadCompleteHandler _LoadCompleteHandler)
        {
            this._AssetLoader = null;
            this.label = label;
            //委托初始化
            this._LoadCompleteHandler = _LoadCompleteHandler;
            //AB包下载路径初始化
            this.downloadPath = PathTool.GetWWWPath() + "/" + label;
        }

        /// <summary>
        /// 加载AssetBundle资源包
        /// </summary>
        public IEnumerator LoadAssetBundle()
        {
            using (WWW www = new WWW(downloadPath))
            {
                yield return www;
                //如果ab包下载完成
                if (www.progress >= 1)
                {
                    //获取AssetBundle的实例
                    AssetBundle ab = www.assetBundle;
                    if(ab != null)
                    {
                        //实例化引用类
                        _AssetLoader = new AssetLoader(ab);
                        if (_LoadCompleteHandler != null)
                        {
                            _LoadCompleteHandler(label);
                        }
                    }
                    else
                    { 
                        Debug.LogError(GetType() + "/LoadAssetBundle()/ WWW下载出错，请检查！AssetBundle URL:" + downloadPath + "\n错误信息：" + www.error);
                    }
                }
            }
        }

        /// <summary>
        /// 加载（AB包内）资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="isCache">是否需要缓存</param>
        /// <returns></returns>
        public UnityEngine.Object LoadAsset(string assetName, bool isCache)
        {
            if (_AssetLoader != null)
            {
                return _AssetLoader.LoadAsset(assetName, isCache);
            }
            Debug.LogError(GetType() + "/LoadAsset()/ 参数assetLoader为空，请检查！");
            return null;
        }

        /// <summary>
        /// 卸载（AB包中）资源
        /// </summary>
        /// <param name="asset"></param>
        public void UnloadAsset(UnityEngine.Object asset)
        {
            if (_AssetLoader != null)
            {
                _AssetLoader.UnloadAsset(asset);
            }
            else
            {
                Debug.LogError(GetType() + "/UnloadAsset()/ 中_AssetLoader为空，请检查！");
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_AssetLoader != null)
            {
                _AssetLoader.Dispose();
                _AssetLoader = null;
            }
            else
            {
                Debug.LogError(GetType() + "/Dispose()/ 中_AssetLoader为空，请检查！");
            }
        }

        /// <summary>
        /// 释放当前AssetBundle资源包并且卸载所有资源
        /// </summary>
        public void DisposeAll()
        {
            if (_AssetLoader != null)
            {
                _AssetLoader.DisposeAll();
                _AssetLoader = null;
            }
            else
            {
                Debug.LogError(GetType() + "/DisposeAll()/ 中_AssetLoader为空，请检查！");
            }
        }

        /// <summary>
        /// 查询当前AssetBundle包中所有的资源
        /// </summary>
        /// <returns></returns>
        public string[] RetrievalAllAssetNames()
        {
            if (_AssetLoader != null)
            {
                return _AssetLoader.RetrievalAllAssetNames();
            }
            Debug.LogError(GetType() + "/RetrievelAllAssetNames()/ 中_AssetLoader为空，请检查！");
            return null;
        }
    }
}

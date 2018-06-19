using System.Collections;
using UnityEngine;
using System;

namespace ABFramework
{
    /// <summary>
    /// 辅助类：读取AssetBundle依赖关系文件(Window.Manifest)
    /// </summary>
    public class ManifestLoader : IDisposable
    {
        //单例模式
        private static ManifestLoader instance;
        public static ManifestLoader Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ManifestLoader();
                }
                return instance;
            }
        }

        //Manifest（清单文件）系统类
        private AssetBundleManifest manifest;
        //Manifest（清单文件）的路径
        private string manifestPath;
        //读取Manifest（清单文件）的AssetBundle
        private AssetBundle ab;
        //是否Manifest加载完成
        private bool isLoaded;
        public bool IsLoaded { get { return isLoaded; } }

        private ManifestLoader()
        {
            //确定Manifest WWW下载路径
            manifestPath = PathTool.GetWWWPath() +"/" + PathTool.GetPlatformName();
            manifest = null;
            ab = null;
            isLoaded = false;
        }

        /// <summary>
        /// 加载Manifest
        /// </summary>
        public IEnumerator LoadManifestFile()
        {
            using (WWW www = new WWW(manifestPath))
            {
                yield return www;
                if (www.progress >= 1)
                {
                    //加载完成，获取AssetBundle实例
                    AssetBundle ab = www.assetBundle;
                    if (ab != null)
                    {
                        this.ab = ab;
                        //读取Manifest资源
                        manifest = this.ab.LoadAsset(Define.Manifest) as AssetBundleManifest;
                        isLoaded = true;
                    }
                    else
                    {
                        Debug.LogError(GetType() + "/LoadManifestFile/ WWW下载出错，请检查！manifestPath:" + manifestPath + "\n错误信息:" + www.error);
                    }
                }
            }
        }

        /// <summary>
        /// 获取Manifest
        /// </summary>
        public AssetBundleManifest GetManifest()
        {
            if(isLoaded)
            {
                if (manifest != null)
                {
                    return manifest;
                }
                else
                {
                    Debug.LogError(GetType() + "/GetManifest/ manifest为空，请检查！");
                }
            }
            else
            {
                Debug.LogError(GetType() + "/GetManifest/ isLoaded为false，manifeset未加载完成，请检查！");
            }
            return null;
        }

        /// <summary>
        /// 获取Manifest中所有依赖项
        /// </summary>
        public string[] RetrievalDependences(string label)
        {
            if (manifest != null && !string.IsNullOrEmpty(label)) 
            {
                return manifest.GetAllDependencies(label);
            }
            return null;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (ab != null)
            {
                ab.Unload(true);
            }
        }
    }
}

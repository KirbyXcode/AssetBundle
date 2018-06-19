using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABFramework
{
    /// <summary>
    /// 框架主流程（第4层）：所有“场景”AssetBundle的管理。
    ///     功能: 1、读取“Manifest 清单文件“，缓存本脚本。
    ///           2、以“场景”为单位，管理整个项目中所有的AssetBundle包。
    /// </summary>
	public class AssetBundleManager : MonoBehaviour
	{
        //本类实例
        private static AssetBundleManager instance;
        //场景集合
        private Dictionary<string, MultiABManager> allScenesDict = new Dictionary<string, MultiABManager>();
        //AssetBundle（清单文件）
        private AssetBundleManifest manifest = null;

        //单例模式
        public static AssetBundleManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new GameObject("AssetBundleManager").AddComponent<AssetBundleManager>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            //加载清单文件
            StartCoroutine(ManifestLoader.Instance.LoadManifestFile());
        }

        /// <summary>
        /// 下载AssetBundle指定包
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="label">AB包名</param>
        /// <param name="_LoadCompleteHandler">委托：调用是否完成</param>
        public IEnumerator LoadAssetBundlePackage(string sceneName, string label, LoadCompleteHandler _LoadCompleteHandler)
        {
            //参数检查
            if(string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(label))
            {
                Debug.LogError(GetType() + "/LoadAssetBundlePackage/ sceneName or label is null, 请检查!");
                yield return null;
            }

            //等待Manifest清单文件加载完成
            while(!ManifestLoader.Instance.IsLoaded)
            {
                yield return null;
            }
            manifest = ManifestLoader.Instance.GetManifest();

            if(manifest == null)
            {
                Debug.LogError(GetType() + "/LoadAssetBundlePackage/ manifest is null, 请检查!");
                yield return null;
            }

            //把当前场景加入集合中
            MultiABManager multiABMgr = null;
            if (!allScenesDict.ContainsKey(sceneName))
            {
                multiABMgr = new MultiABManager(sceneName, label, _LoadCompleteHandler);
                allScenesDict.Add(sceneName, multiABMgr);
            }

            //调用下一层（“多包管理类”）
            multiABMgr = allScenesDict[sceneName];
            if(multiABMgr == null)
            {
                Debug.LogError(GetType() + "/LoadAssetBundlePackage()/ mutiABMgr is null, 请检查！");
            }
            //调用“多包管理类”的加载指定AB包
            yield return multiABMgr.LoadAssetBundle(label);
        }

        /// <summary>
        /// 加载（AB包中）资源
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="label">AB包名</param>
        /// <param name="assetName">资源名称</param>
        /// <param name="isCache">是否缓存</param>
        public UnityEngine.Object LoadAsset(string  sceneName, string label, string assetName, bool isCache)
        {
            if(allScenesDict.ContainsKey(sceneName))
            {
                MultiABManager multiABMgr = allScenesDict[sceneName];
                return multiABMgr.LoadAsset(label, assetName, isCache);
            }

            Debug.LogError(GetType() + "/LoadAsset()/ sceneName is null, 请检查! sceneName=" + sceneName);
            return null;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        public void DisposeAllAssets(string sceneName)
        {
            if (allScenesDict.ContainsKey(sceneName))
            {
                MultiABManager multiABMgr = allScenesDict[sceneName];
                multiABMgr.DisposeAllAssets();
            }
            else
            {
                Debug.LogError(GetType() + "/DisposeAllAssets()/ sceneName is null, 无法加载AB包中资源, 请检查! sceneName=" + sceneName);
            }
        }
    }
}

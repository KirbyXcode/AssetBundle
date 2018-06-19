using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABFramework
{
    /// <summary>
    /// 一个场景中多个AssetBundle管理
    ///     功能：1）获取AB包之间的依赖与引用关系
    ///           2）管理AssetBundle包之间的自动连锁（递归）加载机制
    ///         
    /// </summary>
    public class MultiABManager
    {
        //（下层）引用类：单个AB包加载实现类
        private SingleABLoader loader;
        //“AB包实现类”缓存集合（作用：缓存AB包，防止重复加载，即：“AB包缓存集合”）
        private Dictionary<string, SingleABLoader> loaderCacheDict;
        //当前场景（调试使用）
        private string currentSceneName;
        //当前AssetBundle名称
        private string label;
        //AB包与对应依赖关系集合
        private Dictionary<string, Relation> relationDict;
        //委托：所有AB包加载完成
        private LoadCompleteHandler _LoadCompleteHandler;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="label">AB包名</param>
        /// <param name="_LoadCompleteHandler">（委托）是否调用完成</param>
        public MultiABManager(string sceneName, string label, LoadCompleteHandler _LoadCompleteHandler)
        {
            loaderCacheDict = new Dictionary<string, SingleABLoader>();
            relationDict = new Dictionary<string, Relation>();

            this.currentSceneName = sceneName;
            this.label = label;
            this._LoadCompleteHandler = _LoadCompleteHandler;
        }

        /// <summary>
        /// 完成指定AB包调用
        /// </summary>
        private void LoadComplete(string label)
        {
            if(label.Equals(this.label))
            {
                if (_LoadCompleteHandler != null)
                {
                    _LoadCompleteHandler(label);
                }
            }
        }

        /// <summary>
        /// 加载AB包
        /// </summary>
        public IEnumerator LoadAssetBundle(string label)
        {
            //AB包关系的建立
            if(!relationDict.ContainsKey(label))
            {
                Relation relation = new Relation(label);
                relationDict.Add(label, relation);
            }
            Relation currentRelation = relationDict[label];

            //得到指定AB包所有的依赖关系（查询Manifest清单文件）
            string[] dependences = ManifestLoader.Instance.RetrievalDependences(label);
            for (int i = 0; i < dependences.Length; i++)
            {
                //添加“依赖”项
                currentRelation.AddDependence(dependences[i]);
                //添加“引用”项(使用协程递归调用)
                yield return LoadReference(dependences[i], label);
            }

            //真正加载AB包
            if(loaderCacheDict.ContainsKey(label))
            {
                yield return loaderCacheDict[label].LoadAssetBundle();
            }
            else
            {
                loader = new SingleABLoader(label, LoadComplete);
                loaderCacheDict.Add(label, loader);
                yield return loader.LoadAssetBundle();
            }
        }

        /// <summary>
        /// 加载引用AB包
        /// </summary>
        /// <param name="label">AB包名</param>
        /// <param name="refLabel">被引用的AB包名</param>
        /// <returns></returns>
        private IEnumerator LoadReference(string label, string refLabel)
        {
            Relation relation = null;

            //如果AB包已经加载
            if (relationDict.ContainsKey(label))
            {
                relation = relationDict[label];
                //添加AB包引用关系（被依赖关系）
                relation.AddReference(refLabel);
            }
            else
            {
                relation = new Relation(label);
                relation.AddReference(refLabel);
                relationDict.Add(label, relation);

                //开始加载依赖的包（递归调用）
                yield return LoadAssetBundle(label);
            }
        }

        /// <summary>
        /// 加载（AB包中）资源
        /// </summary>
        /// <param name="label">AB包名</param>
        /// <param name="assetName">资源名称</param>
        /// <param name="isCache">是否缓存</param>
        /// <returns></returns>
        public UnityEngine.Object LoadAsset(string label, string assetName, bool isCache)
        {
            foreach (string mLabel in loaderCacheDict.Keys)
            {
                if (mLabel == label)
                {
                    return loaderCacheDict[mLabel].LoadAsset(assetName, isCache);
                }
            }
            Debug.LogError(GetType() + "/LoadAsset()/ 找不到AssetBundle包，无法加载资源，请检查！label =" + label + " assetName=" + assetName);
            return null;
        }

        /// <summary>
        /// 释放本场景中所有的资源（建议仅在场景转换中使用）
        /// </summary>
        public void DisposeAllAssets()
        {
            try
            {
                //逐一释放所有加载过的AssetBundle包中的资源
                foreach (SingleABLoader loader in loaderCacheDict.Values)
                {
                    loader.DisposeAll();
                }
            }
            finally
            {
                loaderCacheDict.Clear();
                loaderCacheDict = null;

                //释放其他对象占用资源
                relationDict.Clear();
                relationDict = null;
                currentSceneName = null;
                label = null;
                _LoadCompleteHandler = null;

                //卸载没有使用到的资源
                Resources.UnloadUnusedAssets();
                
                //立即执行垃圾回收
                System.GC.Collect();
            }
        }
    }
}

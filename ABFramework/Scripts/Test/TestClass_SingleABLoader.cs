using UnityEngine;

namespace ABFramework
{
    /// <summary>
    /// 验证（测试）“SingleABLoader”类
    /// </summary>
    public class TestClass_SingleABLoader : MonoBehaviour
    {
        #region 复杂（有依赖包）预设的加载

        private SingleABLoader _SingleABLoader = null;
        //依赖AB包名
        private string dependLabel1 = "scene_1/textures.ab";
        private string dependLabel2 = "scene_1/materials.ab";
        //obj AB包名
        private string label = "scene_1/prefabs.ab";
        //资源名称
        private string assetName = "Capsule.prefab";

        private void Start()
        {
            SingleABLoader _DependLoader = new SingleABLoader(dependLabel1, LoadDepend1Complete);
            StartCoroutine(_DependLoader.LoadAssetBundle());
        }

        /// <summary>
        /// 依赖回调函数1
        /// </summary>
        private void LoadDepend1Complete(string label)
        {
            Debug.Log("依赖包1（贴图包）加载完毕，加载依赖包2（材质包）");
            SingleABLoader _DependLoader = new SingleABLoader(dependLabel2, LoadDepend2Complete);
            StartCoroutine(_DependLoader.LoadAssetBundle());
        }

        /// <summary>
        /// 依赖回调函数2
        /// </summary>
        private void LoadDepend2Complete(string label)
        {
            Debug.Log("依赖包2（材质包）加载完毕，开始正式加载预设包");
            _SingleABLoader = new SingleABLoader(this.label, LoadComplete);
            StartCoroutine(_SingleABLoader.LoadAssetBundle());
        }

        /// <summary>
        /// 回调函数
        /// </summary>
        private void LoadComplete(string label)
        {
            //加载AB包中资源
            UnityEngine.Object obj = _SingleABLoader.LoadAsset(assetName, false);
            Instantiate(obj);

            //查询包中的资源
            string[] strArray = _SingleABLoader.RetrievalAllAssetNames();
            foreach (string str in strArray)
            {
                Debug.Log(str);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("释放镜像内存资源");
                _SingleABLoader.Dispose();

                //Debug.Log("释放镜像内存资源与内存资源");
                //_SingleABLoader.DisposeAll();
            }
        }

        #endregion

        #region 简单（无依赖包）预设的加载

        //private SingleABLoader _SingleABLoader;
        ////AB包名
        //private string label = "scene_1/prefabs.ab";
        ////AB包中资源名称
        //private string assetName = "Capsule.prefab";

        //private void Start()
        //{
        //    _SingleABLoader = new SingleABLoader(label, LoadComplete);
        //    StartCoroutine(_SingleABLoader.LoadAssetBundle());
        //}

        ///// <summary>
        ///// 回调函数
        ///// </summary>
        //private void LoadComplete(string label)
        //{
        //    //加载AB包中资源
        //    UnityEngine.Object obj = _SingleABLoader.LoadAsset(assetName, false);
        //    Instantiate(obj);
        //}

        #endregion
    }
}

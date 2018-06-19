using UnityEngine;

namespace ABFramework
{
    /// <summary>
    /// 框架整体验证（测试）脚本
    /// </summary>
	public class TestClass_ABFramework : MonoBehaviour
	{
        //场景名称
        private string sceneName = "scene_1";
        //AB包名
        private string label = "scene_1/prefabs.ab";
        //资源名称
        private string assetName = "Capsule.prefab";

        private void Start()
        {
            Debug.Log(GetType() + " 开始ABFramework框架测试");
            //调用AB包（连锁智能调用AB包【集合】)
            StartCoroutine(AssetBundleManager.Instance.LoadAssetBundlePackage(sceneName, label, LoadAllABComplete));
        }

        /// <summary>
        /// 回掉函数：所有的AB包都已经加载完毕后自动调用
        /// </summary>
        /// <param name="name"></param>
        private void LoadAllABComplete(string label)
        {
            Debug.Log(GetType() + " 所有AB包都已加载完毕");
            UnityEngine.Object obj = null;

            obj = AssetBundleManager.Instance.LoadAsset(sceneName, label, assetName, false);

            if (obj != null)
            {
                Instantiate(obj);
            }
        }

        private void Update()
        {
            //销毁场景资源
            if(Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log(GetType() + " 测试销毁资源");
                AssetBundleManager.Instance.DisposeAllAssets(sceneName);
            }
        }
    }
}

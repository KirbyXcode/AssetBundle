using UnityEditor;
using System.IO;

namespace ABFramework
{
    /// <summary>
    ///删除指定目录下，所有的AssetBundles文件
    /// </summary>
	public class DeleteAssetBundle
	{
        [MenuItem("AssetBundle/Delete AssetBundles")]
		public static void DeleteAllAssetBundles()
        {
            string path = PathTool.GetBuildABPath();

            if(!string.IsNullOrEmpty(path))
            {
                //第二个参数表示可以删除非空目录
                Directory.Delete(path, true);

                //去除删除警告
                //File.Delete(path + ".meta");

                //刷新
                AssetDatabase.Refresh();
            }
        }
	}
}

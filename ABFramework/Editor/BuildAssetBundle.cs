using UnityEditor;
using System.IO;

namespace ABFramework
{
    /// <summary>
    /// 打包生成所有AssetBundles
    /// </summary>
	public class BuildAssetBundle
	{
        [MenuItem("AssetBundle/Build AssetBundles")]
		public static void BuildAllAssetBundles()
        {
            string path = PathTool.GetBuildABPath();

            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        }
	}
}

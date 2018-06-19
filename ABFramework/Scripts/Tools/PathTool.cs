using UnityEngine;

namespace ABFramework
{
    /// <summary>
    /// 路径工具类，包含本框架中所有的路径常量和路径方法
    /// </summary>
	public class PathTool
	{
        public const string AB_Root = "AB_Res";

        /// <summary>
        /// 获取AB资源的输入目录（路径）
        /// </summary>
        public static string GetABRootPath()
        {
            return Application.dataPath + "/" + AB_Root;
        }

        /// <summary>
        /// 获取AB资源的输出（目录）路径
        /// </summary>
        public static string GetBuildABPath()
        {
            return GetPlatformPath() + "/" + GetPlatformName();
        }

        /// <summary>
        /// 获取平台的路径
        /// </summary>
        public static string GetPlatformPath()
        {
            string platformPath = string.Empty;

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    platformPath = Application.streamingAssetsPath;
                    break;
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.Android:
                    platformPath = Application.persistentDataPath;
                    break;
            }

            return platformPath;
        }

        /// <summary>
        /// 获取平台的名称
        /// </summary>
        public static string GetPlatformName()
        {
            string platformName = string.Empty;

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    platformName = "Windows";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    platformName = "IPhone";
                    break;
                case RuntimePlatform.Android:
                    platformName = "Android";
                    break;
            }
            return platformName;
        }

        /// <summary>
        /// 获取WWW协议下载（AB包）路径
        /// </summary>
        public static string GetWWWPath()
        {
            string wwwPath = string.Empty;

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    wwwPath = "file://" + GetBuildABPath();
                    break;
                case RuntimePlatform.IPhonePlayer:
                    //wwwPath = GetBuildABPath();
                    wwwPath = Application.dataPath + "/Raw/" + GetPlatformName();
                    break;
                case RuntimePlatform.Android:
                    wwwPath = "jar:file://" + GetBuildABPath();
                    break;
            }
            return wwwPath;
        }
	}
}

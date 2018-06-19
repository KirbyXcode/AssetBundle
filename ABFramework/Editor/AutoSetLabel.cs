/*
    开发思路：
        1：定义需要打包资源的文件夹根目录
        2：遍历每个“场景”文件夹（目录）
            A：遍历本场景目录下所有目录或文件。
               如果是目录，则继续“递归”访问里面的文件，直到定位到文件。
            B：找到文件，则使用AssetImporter类，标记“标签”与“后缀名”。
*/

using UnityEngine;
using UnityEditor;
using System.IO;

namespace ABFramework
{
    /// <summary>
    /// 自动给资源文件添加标签
    /// </summary>
	public class AutoSetLabel
	{
        /// <summary>
        /// 设置AB标签
        /// </summary>
        [MenuItem("AssetBundle/Set AssetBundle Labels")]
        public static void SetABLabel()
        {
            //需要给AB做标签的根目录
            string labelRoot = string.Empty;
            //目录信息（场景目录信息数组，表示所有的根目录下场景目录）
            DirectoryInfo[] scenesInfo = null;

            //清空无用AB标签
            AssetDatabase.RemoveUnusedAssetBundleNames();
            //需要打包资源文件夹的根目录
            labelRoot = PathTool.GetABRootPath();

            //获取AB_Res下的所有文件夹（目录）
            DirectoryInfo rootInfo = new DirectoryInfo(labelRoot);
            scenesInfo = rootInfo.GetDirectories();

            //遍历每个“场景”文件夹（目录）
            for (int i = 0; i < scenesInfo.Length; i++)
            {
                //A：遍历本场景目录下所有目录或文件。
                //   如果是目录，则继续“递归”访问里面的文件，直到定位到文件。
                FolderOrFile(scenesInfo[i], scenesInfo[i].Name);
            }

            //刷新
            AssetDatabase.Refresh();
            //提示信息，设置AB标签完成。
            Debug.Log("AssetBundel 本次操作设置AB标签完成！");
        }

        /// <summary>
        /// 递归判断是否目录与文件，修改AssetBundle的标签
        /// </summary>
        /// <param name="fileInfo">当前文件信息（文件信息与目录信息可以相互转换）</param>
        /// <param name="path">当前场景名称</param>
        private static void FolderOrFile(FileSystemInfo fileSysInfo, string sceneName)
        {
            if(!fileSysInfo.Exists)
            {
                Debug.LogError("文件或者目录名称：" + fileSysInfo + "不存在，请检查");
                return;
            }

            DirectoryInfo dirInfo = fileSysInfo as DirectoryInfo;

            //得到当前目录下一级的文件信息集合
            FileSystemInfo[] filesInfo = dirInfo.GetFileSystemInfos();

            for (int i = 0; i < filesInfo.Length; i++)
            {
                FileInfo file = filesInfo[i] as FileInfo;
                //文件类型
                if (file != null)
                {
                    //修改次文件的AssetBundle标签
                    SetFileABLabel(file, sceneName);
                }
                //目录类型
                else
                {
                    FolderOrFile(filesInfo[i], sceneName);
                }
            }
        }

        /// <summary>
        /// 对指定的文件设置“标签”
        /// </summary>
        /// <param name="file">文件信息(包含文件绝对路径)</param>
        /// <param name="sceneName">场景名称</param>
        private static void SetFileABLabel(FileInfo file, string sceneName)
        {

            //AssetBundle包名（标签名）
            string label = string.Empty;

            //文件路径（相对路径）
            string filePath = string.Empty;

            //参数检查，后缀名".meta"文件不做处理
            if (file.Extension == ".meta") return;

            //得到标签名称
            label = GetLabelName(file, sceneName);

            //获取资源文件的相对路径
            int index = file.FullName.IndexOf("Assets");
            filePath = file.FullName.Substring(index);

            //给资源文件设置AB标签以及后缀
            AssetImporter importer = AssetImporter.GetAtPath(filePath);
            importer.assetBundleName = label;

            //如果是场景文件
            if(file.Extension  == ".unity")
            {
                importer.assetBundleVariant = "u3d";
            }
            else
            {
                importer.assetBundleVariant = "ab";
            }
        }

        /// <summary>
        /// 获取标签名称
        /// </summary>
        /// <param name="file">文件信息</param>
        /// <param name="sceneName">场景名称</param>
        /// AB标签名称形成规则：
        ///     标签名称 = “所在二级目录名称（场景名称)” + “三级目录名称”
        ///     例如：Scene_1/Texture
        /// <returns></returns>
        private static string GetLabelName(FileInfo file, string sceneName)
        {
            string label = string.Empty;

            //Win路径
            string winPath = file.FullName;
            //Unity路径
            string unityPath = winPath.Replace("\\", "/");
            //定位“场景名称”后面字符位置
            int index = unityPath.IndexOf(sceneName) + sceneName.Length;
            //标签中“类型名称”所在区域
            string resType = unityPath.Substring(index + 1);
            if(resType.Contains("/"))
            {
                //素材标签名称：Scene_1/Texture
                string[] stringArray = resType.Split('/');
                label = sceneName + "/" + stringArray[0];
            }
            else
            {
                //场景标签名称：Scene_1/Scene_1
                label = sceneName + "/" + sceneName;
            }
            return label;
        }
    }
}

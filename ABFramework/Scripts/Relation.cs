using System.Collections.Generic;

namespace ABFramework
{
    /// <summary>
    /// 工具辅助类：
    ///     1：存储指定AB包的所有依赖关系包
    ///     2：存储指定AB包所有的引用关系包
    /// </summary>
    public class Relation
    {
        //当前AssetBundle名称
        private string label;
        //本包所有的依赖包集合
        private List<string> allDependenceList;
        //本包所有的引用包集合
        private List<string> allReferenceList;

        public Relation(string label)
        {
            if (!string.IsNullOrEmpty(label))
            {
                this.label = label;
            }
            allDependenceList = new List<string>();
            allReferenceList = new List<string>();
        }

        /// <summary>
        /// 增加依赖关系
        /// </summary>
        /// <param name="label">AB包名</param>
        public void AddDependence(string label)
        {
            if(!allDependenceList.Contains(label))
            {
                allDependenceList.Add(label);
            }
        }

        /// <summary>
        /// 移除依赖关系
        /// </summary>
        /// <param name="label">AB包名</param>
        /// <returns>
        /// 返回True: 此AssetBundle没有依赖项
        /// 返回False: 此AssetBundle还有其他依赖项
        /// </returns>
        public bool RemoveDependence(string label)
        {
            if (allDependenceList.Contains(label)) 
            {
                allDependenceList.Remove(label);
            }

            if (allDependenceList.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获得所有依赖关系
        /// </summary>
        public List<string> GetAllDependence()
        {
            return allDependenceList;
        }


        /// <summary>
        /// 增加引用关系
        /// </summary>
        /// <param name="label">AB包名</param>
        public void AddReference(string label)
        {
            if (!allReferenceList.Contains(label))
            {
                allReferenceList.Add(label);
            }
        }

        /// <summary>
        /// 移除引用关系
        /// </summary>
        /// <param name="label">AB包名</param>
        /// <returns>
        /// 返回True: 此AssetBundle没有引用项
        /// 返回False: 此AssetBundle还有其他引用项
        /// </returns>
        public bool RemoveReference(string label)
        {
            if (allReferenceList.Contains(label))
            {
                allReferenceList.Remove(label);
            }

            if (allReferenceList.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获得所有引用关系
        /// </summary>
        public List<string> GetAllReference()
        {
            return allReferenceList;
        }
    }
}

using System;
using System.Collections.Generic;

namespace HayypCard.Utils
{
    public static class ListExtension
    {
        /// <summary>
        /// 只有当List中不存时才添加
        /// </summary>
        public static void AddIf<T>(this List<T> list,T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// 将数据拷贝到目标集合，注意会先清空目标集合中的元素再进行拷贝
        /// </summary>
        public static void CopyTo<T>(this List<T> list,List<T> target)
        {
            target.Clear(); 
            for (int i = 0; i < list.Count; i++)
            {
                target.Add(list[i]);
            }
        }

        /// <summary>
        /// 洗牌，使用Fisher-Yates洗牌算法打乱数组元素顺序
        /// </summary>
        public static void Shuffle<T>(this List<T> list)
        {
            Random rand = new Random();

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rand.Next(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}

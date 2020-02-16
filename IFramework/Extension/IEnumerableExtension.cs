using System;
using System.Collections.Generic;
using System.Linq;

namespace IFramework
{
    /// <summary>
    /// 集合静态扩展
    /// </summary>
    public static partial class IEnumerableExtension
    {
        /// <summary>
        /// 遍历集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            if (action == null) throw new ArgumentException();
            foreach (var item in self)
            {
                action(item);
            }
            return self;
        }
        /// <summary>
        /// 遍历集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T[] ForEach<T>(this T[] self, Action<T> action)
        {
            Array.ForEach(self, action);
            return self;
        }
        /// <summary>
        /// 遍历集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T[] ForEach<T>(this T[] self, Action<int, T> action)
        {
            for (var i = 0; i < self.Length; i++)
            {
                action(i, self[i]);
            }
            return self;

        }
        /// <summary>
        /// 反向遍历集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static List<T> ReverseForEach<T>(this List<T> self, Action<T> action)
        {
            if (action == null) throw new ArgumentException();

            for (var i = self.Count - 1; i >= 0; --i)
                action(self[i]);
            return self;
        }
        /// <summary>
        /// 遍历集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this List<T> self, Action<int, T> action)
        {
            for (var i = 0; i < self.Count; i++)
            {
                action(i, self[i]);
            }
        }
        /// <summary>
        /// 拷贝本集合数据到另一个集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">源集合</param>
        /// <param name="to">目标集合</param>
        /// <param name="begin">开始下表</param>
        /// <param name="end">结束下标</param>
        public static void CopyTo<T>(this List<T> self, List<T> to, int begin = 0, int end = -1)
        {
            if (begin < 0) begin = 0;
            var endIndex = Math.Min(self.Count, to.Count) - 1;
            if (end != -1 && end < endIndex) endIndex = end;
            for (var i = begin; i < end; i++)
            {
                to[i] = self[i];
            }
        }
        /// <summary>
        /// 移除集合第一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static T Dequeue<T>(this List<T> self)
        {
            if (self == null || self.Count <= 0) return default(T);
            T t = self[0];
            self.Remove(t);
            return t;
        }
        /// <summary>
        /// Add
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="t"></param>
        public static void Enqueue<T>(this List<T> self, T t)
        {
            if (self == null) throw new Exception("Null List");
            self.Add(t);
        }
        /// <summary>
        /// Add
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="t"></param>
        public static void Push<T>(this List<T> self, T t)
        {
            if (self == null) throw new Exception("Null List");
            self.Add(t);
        }
        /// <summary>
        /// 移除最后一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static T Pop<T>(this List<T> self)
        {
            if (self == null || self.Count <= 0) return default(T);
            T t = self[self.Count - 1];
            self.Remove(t);
            return t;
        }
        /// <summary>
        /// 查看第一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T QueuePeek<T>(this List<T> list)
        {
            if (list == null || list.Count <= 0) return default(T);
            return list[0];
        }
        /// <summary>
        /// 查看最后一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T StackPeek<T>(this List<T> list)
        {
            if (list == null || list.Count <= 0) return default(T);
            return list[list.Count - 1];
        }

        /// <summary>
        /// 混合字典
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="dictionaries"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, params Dictionary<TKey, TValue>[] dictionaries)
        {
            return dictionaries.Aggregate(dictionary,
                (current, self) => current.Union(self).ToDictionary(kv => kv.Key, kv => kv.Value));
        }
        /// <summary>
        /// 遍历字典
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        public static void ForEach<K, V>(this Dictionary<K, V> self, Action<K, V> action)
        {
            var dictE = self.GetEnumerator();
            while (dictE.MoveNext())
            {
                var current = dictE.Current;
                action(current.Key, current.Value);
            }

            dictE.Dispose();
        }
        /// <summary>
        /// 添加字段到字典
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="self"></param>
        /// <param name="addInDict"></param>
        /// <param name="isOverride"></param>
        public static void AddRange<K, V>(this Dictionary<K, V> self, Dictionary<K, V> addInDict, bool isOverride = false)
        {
            var dictE = addInDict.GetEnumerator();

            while (dictE.MoveNext())
            {
                var current = dictE.Current;
                if (self.ContainsKey(current.Key))
                {
                    if (isOverride)
                        self[current.Key] = current.Value;
                    continue;
                }

                self.Add(current.Key, current.Value);
            }

            dictE.Dispose();
        }
    }

}

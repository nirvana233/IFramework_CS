using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFramework
{
    /// <summary>
    /// 可观测List
    /// </summary>
    /// <typeparam name="T">Object</typeparam>
    public class ObservableList<T> : Unit, IEnumerable<T>, IList<T>
    {
        private Action<int, T> onItemAdded;
        private Action<int, int, T> onItemMoved;
        private Action<int, T> onItemRemoved;
        private Action<int, T, T> onItemReplaced;
        private Action onItemCleared;
        private Lazy<List<T>> _value = new Lazy<List<T>>(() => { return new List<T>(); });
        private List<T> value { get { return _value.Value; } }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return value[index]; }
            set { SetItem(index, value); }
        }

        /// <summary>
        /// 注册 添加一个元素
        /// </summary>
        /// <param name="action">void fun(int index, T item)</param>
        public void SubscribeAddItem(Action<int, T> action)
        {
            onItemAdded += action;
        }
        /// <summary>
        /// 移除 添加一个元素
        /// </summary>
        /// <param name="action">void fun(int index, T item)</param>
        public void UnSubscribeAddItem(Action<int, T> action)
        {
            onItemAdded -= action;
        }
        /// <summary>
        /// 注册 移动一个元素
        /// </summary>
        /// <param name="action">void fun(int oldIndex,int newIndex,T item)</param>
        public void SubscribeMoveItem(Action<int, int, T> action)
        {
            onItemMoved += action;
        }
        /// <summary>
        /// 移除 移动一个元素
        /// </summary>
        /// <param name="action">void fun(int oldIndex,int newIndex,T item)</param>
        public void UnSubscribeMoveItem(Action<int, int, T> action)
        {
            onItemMoved -= action;
        }
        /// <summary>
        /// 注册 移除一个元素
        /// </summary>
        /// <param name="action">void fun(int index,T item)</param>
        public void SubscribeRemoveItem(Action<int, T> action)
        {
            onItemRemoved += action;
        }
        /// <summary>
        /// 移除 移除一个元素
        /// </summary>
        /// <param name="action">void fun(int index,T item)</param>
        public void UnSubscribeRemoveItem(Action<int, T> action)
        {
            onItemRemoved -= action;
        }
        /// <summary>
        /// 注册 替换一个元素
        /// </summary>
        /// <param name="action">void fun(int index,T oldItem,T newItem)</param>
        public void SubscribeReplaceItem(Action<int, T, T> action)
        {
            onItemReplaced += action;
        }
        /// <summary>
        /// 移除 替换一个元素
        /// </summary>
        /// <param name="action">void fun(int index,T oldItem,T newItem)</param>
        public void UnSubscribeReplaceItem(Action<int, T, T> action)
        {
            onItemReplaced -= action;
        }
        /// <summary>
        /// 注册 清空元素
        /// </summary>
        /// <param name="action">void fun()</param>
        public void SubscribeClearItem(Action action)
        {
            onItemCleared += action;
        }
        /// <summary>
        /// 移除 清空元素
        /// </summary>
        /// <param name="action">void fun()</param>
        public void UnSubscribeClearItem(Action action)
        {
            onItemCleared -= action;
        }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count
        {
            get { return value.Count; }
        }

        /// <summary>
        /// 只读
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 将对象添加到List末尾
        /// </summary>
        /// <param name="item">要添加到List末尾的对象</param>
        public void Add(T item)
        {
            value.Add(item);
            onItemAdded?.Invoke(value.Count - 1, item);
        }

        /// <summary>
        /// 移除List中的所有元素
        /// </summary>
        public void Clear()
        {
            value.Clear();
            onItemCleared?.Invoke();
        }

        /// <summary>
        /// 确定某元素是否在List中
        /// </summary>
        /// <param name="item">要在List中定位的对象</param>
        /// <returns>如果存在返回true,否则为false</returns>
        public bool Contains(T item)
        {
            return value.Contains(item);
        }

        /// <summary>
        /// 从目标数组的指定索引处开始将整个List复制到兼容的一维Array
        /// </summary>
        /// <param name="array">从List中复制的元素的目标一维Array</param>
        /// <param name="arrayIndex">array中从零开始的索引，从此处开始复制</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            value.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 返回循环访问List的IEnumerator
        /// </summary>
        /// <returns>List的泛型枚举器</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return value.GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问List的IEnumerator
        /// </summary>
        /// <returns>List的泛型枚举器</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return value.GetEnumerator();
        }

        /// <summary>
        /// 搜索指定的对象，并返回整个List中第一个匹配项的从零开始的索引
        /// </summary>
        /// <param name="item">要在List中定位的对象</param>
        /// <returns>List中第一个匹配项的从零开始的索引,如果未匹配到则为-1</returns>
        public int IndexOf(T item)
        {
            return value.IndexOf(item);
        }

        /// <summary>
        /// 将元素插入List的指定索引处
        /// </summary>
        /// <param name="index">插入 item 的从零开始的索引</param>
        /// <param name="item">要插入的对象</param>
        public void Insert(int index, T item)
        {
            value.Insert(index, item);
            onItemAdded?.Invoke(index, item);
        }

        /// <summary>
        /// 将指定索引处的项移至列表中的新位置
        /// </summary>
        /// <param name="oldIndex">指定要移动的项的位置的从零开始的索引</param>
        /// <param name="newIndex">当前状态下指定项的新位置的从零开始的索引</param>
        public void Move(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || newIndex < 0 || oldIndex == newIndex) return;

            T item = value[oldIndex];
            if (oldIndex > newIndex)
            {
                value.RemoveAt(oldIndex);
                value.Insert(newIndex, item);
            }
            else
            {
                value.Insert(newIndex, item);
                value.RemoveAt(oldIndex);
            }


            onItemMoved?.Invoke(oldIndex, newIndex, item);
        }

        /// <summary>
        /// 从List中移除特定对象的第一个匹配项
        /// </summary>
        /// <param name="item">要删除的对象</param>
        /// <returns>成功移除返回true，否则为false；如果未找到也返回false</returns>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            var result = value.Remove(item);
            if (result)
            {
                onItemRemoved?.Invoke(index, item);
            }
            return result;
        }

        /// <summary>
        /// 移除List的指定索引处的元素
        /// </summary>
        /// <param name="index">要移除的元素的从零开始的索引</param>
        public void RemoveAt(int index)
        {
            T item = value[index];
            value.RemoveAt(index);
            onItemRemoved?.Invoke(index, item);
        }

        /// <summary>
        /// 替换指定索引处的元素
        /// </summary>
        /// <param name="index">待替换元素的从零开始的索引</param>
        /// <param name="item">位于指定索引处的元素的新值</param>
        public void SetItem(int index, T item)
        {
            var oldItem = value[index];
            value[index] = item;
            onItemReplaced?.Invoke(index, oldItem, item);
        }

        /// <summary>
        /// 列表对象释放时调用（继承自Unit）
        /// </summary>
        protected override void OnDispose()
        {
            value.Clear();
            onItemAdded -= onItemAdded;
            onItemMoved -= onItemMoved;
            onItemRemoved -= onItemRemoved;
            onItemReplaced -= onItemReplaced;
            onItemCleared -= onItemCleared;
        }
    }

}

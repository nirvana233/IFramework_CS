using System.Collections.Generic;
using System;

namespace IFramework.Serialization.DataTable
{
    /// <summary>
    /// 数据读取器
    /// </summary>
    public interface IDataReader:IDisposable
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> Get<T>();
    }
}
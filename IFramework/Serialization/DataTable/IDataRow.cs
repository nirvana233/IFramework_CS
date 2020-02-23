/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace IFramework.Serialization.DataTable
{
    /// <summary>
    /// 数据行
    /// </summary>
    public interface IDataRow
    {
        /// <summary>
        /// 读取一行
        /// </summary>
        /// <param name="val">行String</param>
        /// <param name="headNames">标题行</param>
        /// <returns></returns>
        List<DataColumn> ReadLine(string val, List<string> headNames);
        /// <summary>
        /// 读取标题行
        /// </summary>
        /// <param name="val">行String</param>
        /// <returns></returns>
        List<string> ReadHeadLine(string val);
        /// <summary>
        /// 写入一行
        /// </summary>
        /// <param name="cols">写入的信息</param>
        /// <returns></returns>
        string WriteLine(List<DataColumn> cols);
        /// <summary>
        /// 写入标题行
        /// </summary>
        /// <param name="headNames">写入的标题</param>
        /// <returns></returns>
        string WriteHeadLine(List<string> headNames);
    }
    /// <summary>
    /// 数据行
    /// </summary>
    public class DataRow : IDataRow
    {
        /// <summary>
        /// 读取一行
        /// </summary>
        /// <param name="val">行String</param>
        /// <param name="headNames">标题行</param>
        /// <returns></returns>
        public List<DataColumn> ReadLine(string val, List<string> headNames)
        {
            List<string> strVals = SpilitRow(val);
            if (strVals.Count != headNames.Count) throw new Exception("Read Err Count Is different");

            List<DataColumn> cols = new List<DataColumn>();
            for (int i = 0; i < headNames.Count; i++)
            {
                cols.Add(new DataColumn()
                {
                    StrValue = strVals[i],
                    HeadLineName = headNames[i]
                });
            }
            return cols;
        }
        /// <summary>
        /// 切割一行
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected virtual List<string> SpilitRow(string val)
        {
            var list = val.Split(',').ToList();
            list.RemoveAt(list.Count - 1);
            return list;
        }
        /// <summary>
        /// 读取标题行
        /// </summary>
        /// <param name="val">行String</param>
        /// <returns></returns>
        public virtual List<string> ReadHeadLine(string val)
        {
            List<string> headNames = val.Split(',').ToList();
            if (string.IsNullOrEmpty(headNames.Last()))
            {
                headNames.RemoveAt(headNames.Count - 1);
            }
            return headNames;
        }
        /// <summary>
        /// 写入一行
        /// </summary>
        /// <param name="cols">写入的信息</param>
        /// <returns></returns>
        public virtual string WriteLine(List<DataColumn> cols)
        {
            string result = string.Empty;
            cols.ForEach((index, c) => {
                string val = c.StrValue;
                val = val.Replace("\"", "\"\"");
                if (val.Contains(",") || val.Contains("\"") || val.Contains('\r') || val.Contains('\n'))
                    val = string.Format("\"{0}\"", val);
                if (index == cols.Count - 1) result = result.Append(val).Append(",");
                else result = result.Append(val).Append(",");
            });
            return result;
        }
        /// <summary>
        /// 写入标题行
        /// </summary>
        /// <param name="headNames">写入的标题</param>
        /// <returns></returns>
        public virtual string WriteHeadLine(List<string> headNames)
        {
            string result = string.Empty;
            headNames.ForEach((index, val) => {
                val = val.Replace("\"", "\"\"");
                if (val.Contains(",") || val.Contains("\"") || val.Contains('\r') || val.Contains('\n'))
                    val = string.Format("\"{0}\"", val);
                if (index == headNames.Count - 1) result = result.Append(val).Append(",");
                else result = result.Append(val).Append(",");
            });
            return result;
        }
    }
}

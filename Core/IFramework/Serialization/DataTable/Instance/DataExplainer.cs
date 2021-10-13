﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IFramework.Serialization.DataTable
{
    /// <summary>
    /// string 解释器
    /// </summary>
    [VersionUpdate(1,"数据转化")]
    [VersionUpdate(10,"处理序列化过程中存在 , \" 的情况")]
    [ScriptVersion(10)]
    [RequireAttribute(typeof(StringConvert))]
    public class DataExplainer : IDataExplainer
    {
        private string dot;
        private string quotes;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="dot">替换逗号的字符</param>
        /// <param name="quotes">替换双引号的字符</param>
        public DataExplainer(string dot= "◞", string quotes= "◜")
        {
            this.dot = dot;
            this.quotes = quotes;
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T CreatInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }
        /// <summary>
        /// 根据格子反序列化一个实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cols"></param>
        /// <param name="membersDic">需要反序列化的成员</param>
        /// <returns></returns>
        public T CreatInstance<T>(List<DataColumn> cols, Dictionary<MemberInfo, string> membersDic)
        {
            T t = CreatInstance<T>();
            foreach (var pair in membersDic)
            {
                MemberInfo m = pair.Key;
                string csvName = pair.Value;
                DataColumn column;
                if (m.IsDefined(typeof(DataColumnIndexAttribute), false))
                {
                    DataColumnIndexAttribute attr = m.GetCustomAttributes(typeof(DataColumnIndexAttribute), false)[0] as DataColumnIndexAttribute;
                    if (attr.index >= cols.Count)
                        throw new Exception(string.Format("index {0} is too Large Then colums  {1}", attr.index, cols.Count));
                    column = cols[attr.index];
                }
                else
                {
                    column = cols.Find((c) => { return c.headLineName == csvName; });
                }
                string str = FitterCsv_CreatInstance(column.strValue);
                if (m is PropertyInfo)
                {
                    PropertyInfo info = m as PropertyInfo;
                    object obj = default(object);
                    if (StringConvert.TryConvert(str, info.PropertyType, ref obj))
                        info.SetValue(t, obj, null);
                    else
                        throw new Exception(string.Format("Convert Err Type {0} Name {1} Value {2}", typeof(T), csvName, column.strValue));
                }
                else
                {
                    FieldInfo info = m as FieldInfo;
                    object obj = default(object);
                    if (StringConvert.TryConvert(str, info.FieldType, ref obj))
                        info.SetValue(t, obj);
                    else
                        throw new Exception(string.Format("Convert Err Type {0} Name {1} Value {2}", typeof(T), csvName, column.strValue));
                }
            }
            return t;
        }
        /// <summary>
        /// 根据 具体类型 获取单个数据格子数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="membersDic">需要序列化的成员</param>
        /// <returns></returns>
        public List<DataColumn> GetColumns<T>(T t, Dictionary<MemberInfo, string> membersDic)
        {
            List<DataColumn> columns = new List<DataColumn>();
            foreach (var member in membersDic)
            {
                string val = string.Empty;
                MemberInfo m = member.Key;
                if (m is PropertyInfo)
                {
                    PropertyInfo info = m as PropertyInfo;
                    val = StringConvert.ConvertToString(info.GetValue(t, null), info.PropertyType);
                }
                else
                {
                    FieldInfo info = m as FieldInfo;
                    val = StringConvert.ConvertToString(info.GetValue(t), info.FieldType);
                }
                columns.Add(new DataColumn()
                {
                    headLineName = member.Value,
                    //StrValue = val
                    strValue = FitterCsv_GetColum(val)

                });
            }
            return columns;
        }


        private string FitterCsv_GetColum(string source)
        {
            return source.Replace(",", dot).Replace("\"", quotes);
        }
        private string FitterCsv_CreatInstance(string source)
        {
            return source.Replace( dot,",").Replace(quotes, "\"");
        }
    }
}

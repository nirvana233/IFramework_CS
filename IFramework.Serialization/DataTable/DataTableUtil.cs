using System.IO;

namespace IFramework.Serialization.DataTable
{
    /// <summary>
    /// DataTable
    /// </summary>
    public static class DataTableUtil
    {
        /// <summary>
        /// 创建数据读取器
        /// </summary>
        /// <param name="streamReader"></param>
        /// <param name="rowReader"></param>
        /// <param name="explainer"></param>
        /// <returns></returns>
        public static IDataReader CreateReader(TextReader streamReader, IDataRow rowReader, IDataExplainer explainer)
        {
            return new DataReader(streamReader, rowReader, explainer);
        }
        /// <summary>
        /// 创建数据读取器
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rowReader"></param>
        /// <param name="explainer"></param>
        /// <returns></returns>
        public static IDataReader CreateReader(string text, IDataRow rowReader, IDataExplainer explainer)
        {
            return new DataReader(text, rowReader, explainer);
        }
        /// <summary>
        /// 创建数据写入器
        /// </summary>
        /// <param name="streamWriter"></param>
        /// <param name="rowWriter"></param>
        /// <param name="explainer"></param>
        /// <returns></returns>
        public static IDataWriter CreateWriter(TextWriter streamWriter, IDataRow rowWriter, IDataExplainer explainer)
        {
            return new DataWriter( streamWriter,  rowWriter,  explainer);
        }
    }
}

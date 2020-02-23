namespace IFramework
{

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public interface ILoger
    {
        void Log(LogType logType, object messages ,params object[] paras);
        void LogFormat(LogType logType, string format, object messages ,params object[] paras);
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}

namespace IFramework
{
    public interface ILoger
    {
        void Log(LogType logType, object messages ,params object[] paras);
        void LogFormat(LogType logType, string format, object messages ,params object[] paras);
    }
}

using System;

namespace IFramework
{
    public class CSLogger : ILoger
    {
        public void Log(LogType logType, object message, params object[] paras)
        {
            Console.WriteLine(string.Format("[{0}] {1}:\t\t{2} {3}", DateTime.Now.ToString(), logType, message, paras));
        }

        public void LogFormat(LogType logType, string format, object message, params object[] paras)
        {
            Console.WriteLine(string.Format("[{0}] {1}:\t\t{2}", DateTime.Now.ToString(), logType, string.Format(format, message, paras)));
        }
    }

}

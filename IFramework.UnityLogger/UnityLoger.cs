

namespace IFramework
{
    public class UnityLoger : ILoger
    {
        public void Log(LogType logType, object message, params object[] paras)
        {
            switch (logType)
            {
                case LogType.Error:
                    UnityEngine.Debug.LogError(message);
                    break;
                case LogType.Warning:
                    UnityEngine.Debug.LogWarning(message);
                    break;
                case LogType.Log:
                    UnityEngine.Debug.Log(message);
                    break;
            }
        }

        public void LogFormat(LogType logType, string format, object message, params object[] paras)
        {
            switch (logType)
            {
                case LogType.Error:
                    UnityEngine.Debug.LogErrorFormat(message as UnityEngine.Object, format, paras);
                    break;
                case LogType.Warning:
                    UnityEngine.Debug.LogWarningFormat(message as UnityEngine.Object, format, paras);
                    break;
                case LogType.Log:
                    UnityEngine.Debug.LogFormat(message as UnityEngine.Object, format, paras);
                    break;
            }
        }
    }

}

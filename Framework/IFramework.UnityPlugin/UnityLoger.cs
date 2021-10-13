

using System;

namespace IFramework
{
    public class UnityLoger : ILoger
    {
        public void Error(object messages, params object[] paras)
        {
            UnityEngine.Debug.LogError(messages);
        }
        public void Exception(Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }
        public void Log(object messages, params object[] paras)
        {
            UnityEngine.Debug.Log(messages);
        }
        public void Warn(object messages, params object[] paras)
        {
            UnityEngine.Debug.LogWarning(messages);
        }
    }

}



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
            UnityEngine.Debug.LogError(string.Format("{0}\n{1}",ex.Message ,ex.StackTrace));
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

namespace IFramework
{
    public class Log
    {
        public static int LogLevel = 0;
        public static int WarnningLevel = 0;
        public static int ErrLevel = 0;
        public static bool Enable = true;
        public static bool LogEnable = true;
        public static bool WarnningEnable = true;
        public static bool ErrEnable = true;
        public static ILoger loger { get; set; }
        static Log()
        {
            loger = new CSLogger();
        }

        public static void L(object message, int lev = 0, params object[] paras)
        {
            if (!Enable) return;
            if (!LogEnable) return;
            if (LogLevel > lev) return;
            loger.Log(LogType.Default, message, paras);
        }
        public static void W(object message, int lev = 50, params object[] paras)
        {
            if (!Enable) return;
            if (!WarnningEnable) return;
            if (WarnningLevel > lev) return;
            loger.Log(LogType.Warning, message, paras);
        }
        public static void E(object message, int lev = 100, params object[] paras)
        {
            if (!Enable) return;
            if (!ErrEnable) return;

            if (ErrLevel > lev) return;
            loger.Log(LogType.Error, message, paras);
        }

        public static void LF(object message, string format, int lev = 0, params object[] paras)
        {
            if (!Enable) return;
            if (!LogEnable) return;
            if (LogLevel > lev) return;
            loger.LogFormat(LogType.Default, format, message, paras);
        }
        public static void WF(object message, string format, int lev = 50, params object[] paras)
        {
            if (!Enable) return;
            if (!WarnningEnable) return;
            if (WarnningLevel > lev) return;
            loger.LogFormat(LogType.Warning, format, message, paras);
        }
        public static void EF(object message, string format, int lev = 100, params object[] paras)
        {
            if (!Enable) return;
            if (!ErrEnable) return;
            if (ErrLevel > lev) return;
            loger.LogFormat(LogType.Error, format, message, paras);
        }

    }
}

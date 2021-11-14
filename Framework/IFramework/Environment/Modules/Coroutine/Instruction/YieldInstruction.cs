namespace IFramework.Modules.Coroutine
{
    /// <summary>
    /// 所有等待类的基类
    /// </summary>
    public abstract class YieldInstruction
    {

        /// <summary>
        /// 是否结束
        /// </summary>
        public virtual bool isDone
        {
            get
            {
                return IsCompelete();
            }
            internal set { }
        }

        /// <summary>
        /// 是否结束
        /// </summary>
        /// <returns></returns>
        protected abstract bool IsCompelete();
    }
}

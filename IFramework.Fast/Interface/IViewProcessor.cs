namespace IFramework.Fast
{
    /// <summary>
    /// 界面逻辑处理
    /// </summary>
    public interface IViewProcessor: IProcessor {
        /// <summary>
        /// 获取数据处理
        /// </summary>
        /// <typeparam name="TModelProcessor"></typeparam>
        /// <returns></returns>
        TModelProcessor GetModelProcessor<TModelProcessor>() where TModelProcessor : class, IModelProcessor;
        /// <summary>
        /// 获取跟数据处理
        /// </summary>
        /// <typeparam name="TModelProcessor"></typeparam>
        /// <returns></returns>
        TModelProcessor GetRootModelProcessor<TModelProcessor>() where TModelProcessor : class, IModelProcessor;
    }
}

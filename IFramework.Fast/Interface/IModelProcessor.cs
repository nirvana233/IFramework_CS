namespace IFramework.Fast
{
    /// <summary>
    /// 数据逻辑处理
    /// </summary>
    public interface IModelProcessor: IProcessor {

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        TModel GetModel<TModel>() where TModel : class, IModel;
        /// <summary>
        /// 获取跟数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        TModel GetRootModel<TModel>() where TModel : class, IModel;

    }
}

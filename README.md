# IFramework_CS
Simple  C# Tools

## 整体结构
* IFramework
  * Framework（总入口）
    * FrameworkEnvironment（环境）
      * RecyclableObjectPool （通用对象池）
      * IFrameworkContainer（ioc容器）
      * FrameworkModules
        * FsmModule（状态机）
        * TimerModule（计时器）
        * LoomModule（线程反馈）
        * CoroutineModule（高仿携程）
        * MessageModule（消息转发器）
        * FrameworkAppModule（用户自己扩展）
        * PoolModule（对象池）
        * MVVMModule（mvvm管理器）
        * ECSModule
  * NodeAction（节点事件【依赖高仿携程】）
  * Packet（自定义数据协议）
  * Net（TCP，UDP，WS）
  * Serialization（string版本【主要用于csv】，二进制版本【支持简单的数据结构】）
  * Singleton（通用单例）


## Manual
1、 https://www.jianshu.com/nb/41390681
2、 https://www.jianshu.com/p/7ded8b8bde18



``` csharp
while(true)
    Console.Write("Thanks For EveryOne Who Used It Once !")
```

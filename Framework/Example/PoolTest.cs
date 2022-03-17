using IFramework;
using System;

namespace Example
{
    public class PoolTest : Test
    {
        public interface IObject { }
        public class Obj_A : IObject { }
        public class Obj_B : IObject { }
        public class Obj_C : IObject { }
        public class Human
        {
            public readonly int age;
            public readonly string name;

            public Human(int age, string name)
            {
                this.age = age;
                this.name = name;
            }

            public void Say()
            {
                Log.L($"我的名字叫{name},{age}岁");
            }
        }

        /// <summary>
        /// 自动管理对象池例子
        /// </summary>
        private void ActivatorPoolExample()
        {
            Log.L("");
            Log.L("————这个例子讲述如何使用自动管理的对象池————");
            Log.L("创建一个自动管理的存放Obj_A的对象池");
            ActivatorCreatePool<Obj_A> pool = new ActivatorCreatePool<Obj_A>();

            Log.L("用对象池的Get方法获取一个实例");
            Log.L($"当前池中的对象数量为{pool.count}");
            var _obj = pool.Get();
            Log.L($"拿出的实例类型为 {_obj.GetType()}");
            Log.L("用对象池的Set方法将实例放回对象池");
            pool.Set(_obj);
            Log.L($"当前池中的对象数量为{pool.count} \n");


            Log.L("可以带参数创建一个自动管理的对象池");
            Log.L("这个参数会作为构造函数的参数使用");
            Log.L("传入年龄和姓名创建一个管理Human对象的对象池");
            ActivatorCreatePool<Human> pool_c = new ActivatorCreatePool<Human>(33, "吉良吉影");
            var human = pool_c.Get();
            Log.L("调用对象的Say方法：");
            human.Say();
        }

        /// <summary>
        /// 自定义一个自己管理生命周期的对象池
        /// </summary>
        private class MyPool : ObjectPool<Obj_A>
        {
            protected override Obj_A CreatNew(IEventArgs arg)
            {
                Log.L("*对象池：创建了一个实例");
                return new Obj_A();
            }

            protected override void OnCreate(Obj_A t, IEventArgs arg)
            {
                Log.L("*对象池：实例正在被创建");
                base.OnCreate(t, arg);
            }

            protected override void OnGet(Obj_A t, IEventArgs arg)
            {
                Log.L("*对象池：正在获取实例");
                base.OnGet(t, arg);
            }

            protected override bool OnSet(Obj_A t, IEventArgs arg)
            {
                Log.L("*对象池：正在回收实例");
                return base.OnSet(t, arg);
            }

            protected override void OnClear(Obj_A t, IEventArgs arg)
            {
                Log.L("*对象池：正在清除池子里的实例");
                base.OnClear(t, arg);
            }

            protected override void OnDispose()
            {
                Log.L("*对象池：池子正在被释放");
                base.OnDispose();
            }
        }

        /// <summary>
        /// 自己管理生命周期的对象池例子
        /// </summary>
        private void CustomPoolExample()
        {
            Log.L("");
            Log.L("————这个例子讲述如何使用自己管理生命周期的对象池————");
            Log.L("创建一个自己管理生命周期的对象池");
            MyPool pool = new MyPool();
            Log.L("从对象池中取一个对象");
            var _obj = pool.Get();

            Log.L($"取出的对象类型为 {_obj.GetType()}\n");

            Log.L("将对象归还到对象池");
            pool.Set(_obj);

            Log.L("再从对象池中取一个对象");
            _obj = pool.Get();
        }

        private class MutiPool : BaseTypePool<IObject> { }

        private class MyPool2 : ObjectPool<Obj_B>
        {
            protected override Obj_B CreatNew(IEventArgs arg)
            {
                Console.WriteLine("----------------------------");
                return new Obj_B();
            }
        }

        /// <summary>
        /// 基类对象池例子
        /// </summary>
        private void BaseTypePoolExample()
        {
            Log.L("");
            Log.L("————这个例子讲述如何使用基类对象池————");
            Log.L("创建一个继承BaseTypePool的基类对象池");
            MutiPool pool = new MutiPool();
            Log.L("从池中获取一个Obj_A类型的父类对象");
            IObject _obj = pool.Get<Obj_A>();
            Log.L($"取出的实例类型为{_obj.GetType()}");
            Log.L("一样使用Set方法将实例放回对象池");
            pool.Set(_obj);

            Log.L("从池中获取一个Obj_B类型的父类对象");
            _obj = pool.Get(typeof(Obj_B));
            Log.L($"取出的实例类型为{_obj.GetType()}");
            Log.L("使用Set方法将实例放回对象池 \n");
            pool.Set(_obj);

            Log.L("使用SetPool方法可以将父类对象池放入基类对象池中");
            pool.SetPool(new MyPool2());
        }

        /// <summary>
        /// 全局对象池
        /// </summary>
        private void GlobalPoolExample()
        {
            Log.L("");
            Log.L("————这个例子讲述如何使用全局对象池————");
            Log.L("从全局对象池中获取一个长度为10的Human数组");
            var arr = Framework.GlobalAllocateArray<Human>(10);
            Log.L("将这个数组的第一个元素赋值为一个新的Human对象");
            arr[0] = new Human(33, "吉良吉影");
            arr[0].Say();
            Log.L("然后将这个数组回收掉");
            arr.GlobalRecyle();

            Log.L("重新从全局对象池中获取一个长度为10的Human数组");
            arr = Framework.GlobalAllocateArray<Human>(10);
            Log.L("调用数组第一个元素的Say方法");
            arr[0].Say();
            Log.L("可以发现拿到的数组对象还是之前那个，并且数据没有清空");
            Log.L("因此使用的时候需要注意在回收的时候或者获取的时候清空数据");
            Log.L("防止出现意外情况\n");

            Log.L("当然，如果取长度不为10的Human数组，得到的是另一个对象");
            arr.GlobalRecyle();
            arr = Framework.GlobalAllocateArray<Human>(11);
            Log.L(arr[0] == null);
        }

        protected override void Start()
        {
            ActivatorPoolExample();
            CustomPoolExample();
            BaseTypePoolExample();
            GlobalPoolExample();
        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}

using IFramework.Modules.Coroutine;
using System;
using System.Collections;

namespace IFramework.NodeAction
{
    /// <summary>
    /// 静态扩展
    /// </summary>
    [ScriptVersion(65)]
    [VersionUpdateAttribute(55, "面向接口")]
    [VersionUpdateAttribute(60, "增加节点类型")]
    [VersionUpdateAttribute(65, "减少迭代器使用")]

    public static class ActionNodeExtension
    {
        private class NodeIterator : RecyclableObject
        {
            private ActionNode _node;
            public void Config(IActionNode node)
            {
                this._node = node as ActionNode;
                this.env.BindUpdate(MoveNext);
                SetDataDirty();
            }
            private void MoveNext()
            {
                if (recyled) return;
                bool bo = _node.MoveNext();
                if (!bo)
                {
                    Recyle();
                    this.env.UnBindUpdate(MoveNext);
                }
            }

            protected override void OnDataReset()
            {
                this._node = null;
            }
        }

        /// <summary>
        /// 获取节点运行的 迭代器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerator ActionEnumerator<T>(this T self) where T : IActionNode
        {
            while ((self as ActionNode).MoveNext())
            {
                yield return null;
            }
        }
        /// <summary>
        /// 运行于 ICoroutineModule 上
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="moudle"></param>
        /// <returns></returns>
        public static T Run<T>(this T self, ICoroutineModule moudle) where T : IActionNode
        {
            moudle.StartCoroutine(self.ActionEnumerator());
            return self;
        }


        /// <summary>
        /// 直接运行于环境默认的上
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static T Run<T>(this T self) where T : IActionNode
        {
            NodeIterator it = NodeIterator.Allocate<NodeIterator>(self.env);
            it.Config(self);
            return self;
        }

        private static T Allocate<T>(FrameworkEnvironment env) where T : ActionNode
        {
            T t = RecyclableObject.Allocate<T>(env);
            while (t.disposed)
                t = RecyclableObject.Allocate<T>(env);
            return t;
        }
        private static T Allocate<T>(EnvironmentType envType) where T : ActionNode
        {
            T t = RecyclableObject.Allocate<T>(envType);
            while (t.disposed)
                t = RecyclableObject.Allocate<T>(envType);
            return t;
        }


        /// <summary>
        /// 开启顺序节点
        /// </summary>
        /// <param name="self"></param>
        /// <param name="envType"></param>
        /// <param name="autoRecyle"></param>
        /// <returns></returns>
        [RequireAttribute(typeof(FrameworkEnvironment))]
        public static ISequenceNode Sequence(this object self, EnvironmentType envType, bool autoRecyle = true)
        {
            SequenceNode node = Allocate<SequenceNode>(envType);
            node.Config(autoRecyle);
            return node;
        }
        /// <summary>
        /// 开启顺序节点
        /// </summary>
        /// <param name="self"></param>
        /// <param name="env"></param>
        /// <param name="autoRecyle"></param>
        /// <returns></returns>
        [RequireAttribute(typeof(FrameworkEnvironment))]
        public static ISequenceNode Sequence(this object self, FrameworkEnvironment env, bool autoRecyle = true)
        {
            SequenceNode node = Allocate<SequenceNode>(env);
            node.Config(autoRecyle);
            return node;
        }




        /// <summary>
        /// 结束回调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T OnCompelete<T>(this T self, Action<T> action) where T : IActionNode
        {
            self.OnCompelete(() => { action(self); });
            return self;
        }
        /// <summary>
        /// 开始回调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T OnBegin<T>(this T self, Action<T> action) where T : IActionNode
        {
            return self.OnBegin(() => { action(self); });
        }
        /// <summary>
        /// 被回收回调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T OnRecyle<T>(this T self, Action<T> action) where T : IActionNode
        {
            return self.OnRecyle(() => { action(self); });
        }
        /// <summary>
        /// 结束回调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T OnCompelete<T>(this T self, Action action) where T : IActionNode
        {
            (self as ActionNode).onCompelete += action;
            return self;
        }
        /// <summary>
        /// 开始回调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T OnBegin<T>(this T self, Action action) where T : IActionNode
        {
            (self as ActionNode).onBegin += action;
            return self;
        }
        /// <summary>
        /// 被回收回调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T OnRecyle<T>(this T self, Action action) where T : IActionNode
        {
            (self as ActionNode).onRecyle += action;
            return self;
        }




        /// <summary>
        /// 开启一个时间节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="time"></param>
        /// <param name="autoRecyle"></param>
        /// <returns></returns>
        public static T TimeSpan<T>(this T self, TimeSpan time, bool autoRecyle = false) where T : IContainerNode
        {
            TimeSpanNode node = Allocate<TimeSpanNode>(self.env);
            node.Config(time, autoRecyle);
            (self as ContainerNode).Append(node);
            return self;
        }
        /// <summary>
        /// 开启一个条件结束节点,直到条件成立
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="condition"></param>
        /// <param name="autoRecyle"></param>
        /// <returns></returns>
        public static T Until<T>(this T self, Func<bool> condition, bool autoRecyle = false) where T : IContainerNode
        {
            UntilNode node = Allocate<UntilNode>(self.env);
            node.Config(condition, autoRecyle);
            (self as ContainerNode).Append(node);
            return self;
        }
        /// <summary>
        /// 开启一个条件运行节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="condition"></param>
        /// <param name="body"></param>
        /// <param name="autoRecyle"></param>
        /// <returns></returns>
        public static T While<T>(this T self, Func<bool> condition, Action body, bool autoRecyle = false) where T : IContainerNode
        {
            WhileNode node = Allocate<WhileNode>(self.env);
            node.Config(condition, body, autoRecyle);
            (self as ContainerNode).Append(node);
            return self;
        }
        /// <summary>
        /// 开启一个事件节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="body"></param>
        /// <param name="autoRecyle"></param>
        /// <returns></returns>
        public static T Event<T>(this T self, Action body, bool autoRecyle = false) where T : IContainerNode
        {
            EventNode node = Allocate<EventNode>(self.env);
            node.Config(body, autoRecyle);
            (self as ContainerNode).Append(node);
            return self;
        }
        /// <summary>
        /// 开启一个 IF 节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="condition"></param>
        /// <param name="body"></param>
        /// <param name="autoRecyle"></param>
        /// <returns></returns>
        public static T Condition<T>(this T self, Func<bool> condition, Action body, bool autoRecyle = false) where T : IContainerNode
        {
            ConditionNode node = Allocate<ConditionNode>(self.env);
            node.Config(condition, body, autoRecyle);
            (self as ContainerNode).Append(node);
            return self;
        }
        /// <summary>
        /// 开启一个帧节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="frame"></param>
        /// <param name="autoRecyle"></param>
        /// <returns></returns>
        public static T Frame<T>(this T self, int frame, bool autoRecyle = false) where T : IContainerNode
        {
            FrameNode node = Allocate<FrameNode>(self.env);
            node.Config(frame, autoRecyle);
            (self as ContainerNode).Append(node);
            return self;
        }
        /// <summary>
        /// 开启一个do while节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="condition"></param>
        /// <param name="body"></param>
        /// <param name="autoRecyle"></param>
        /// <returns></returns>
        public static T DoWhile<T>(this T self, Func<bool> condition, Action body, bool autoRecyle = false) where T : IContainerNode
        {
            DoWhileNode node = Allocate<DoWhileNode>(self.env);
            node.Config(condition, body, autoRecyle);
            (self as ContainerNode).Append(node);
            return self;
        }
        /// <summary>
        /// 开启一个 for 节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="initializer"></param>
        /// <param name="condition"></param>
        /// <param name="iterator"></param>
        /// <param name="body"></param>
        /// <param name="autoRecyle"></param>
        /// <returns></returns>
        public static T For<T>(this T self, Action initializer, Func<bool> condition, Action iterator, Action body, bool autoRecyle=false) where T : IContainerNode
        {
            ForNode node = Allocate<ForNode>(self.env);
            node.Config(initializer, condition, iterator, body, autoRecyle);
            (self as ContainerNode).Append(node);
            return self;
        }

        /// <summary>
        /// 开启一个重复运行节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <param name="repeat"></param>
        /// <param name="autoRecyle"></param>
        /// <returns></returns>
        public static T Repeat<T>(this T self, Action<IRepeatNode> action, int repeat = 1, bool autoRecyle = false) where T : IContainerNode
        {
            RepeatNode node = Allocate<RepeatNode>(self.env);
            node.Config(repeat, autoRecyle);
            if (action != null)
                action(node);
            (self as ContainerNode).Append(node);
            return self;
        }
        /// <summary>
        /// 开启一个 顺序运行节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <param name="autoRecyle"></param>
        /// <returns></returns>
        public static T Sequence<T>(this T self, Action<ISequenceNode> action, bool autoRecyle = false) where T : IContainerNode
        {
            SequenceNode node = Allocate<SequenceNode>(self.env);
            node.Config(autoRecyle);
            if (action != null)
                action(node);
            (self as ContainerNode).Append(node);
            return self;
        }
        /// <summary>
        /// 开启一个并行运行节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <param name="autoRecyle"></param>
        /// <returns></returns>
        public static T Spawn<T>(this T self, Action<ISpawnNode> action, bool autoRecyle = false) where T : IContainerNode
        {
            SpawnNode node = Allocate<SpawnNode>(self.env);
            node.Config(autoRecyle);
            if (action != null)
                action(node);
            (self as ContainerNode).Append(node);
            return self;
        }


        /// <summary>
        /// 获取容器节点中的最后一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T LastNode<T>(this T self, Action<IActionNode> action) where T : IContainerNode
        {
            IActionNode last = self.last;
            if (last == null)
            {
                Log.E("There is nothing  in this node");
            }
            else
            {
                if (action != null)
                    action(last);
            }
            return self;
        }
        /// <summary>
        /// 获取容器节点中的最后一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T LastNode<T,T1>(this T self, Action<T1> action) where T : IContainerNode where T1:IActionNode
        {
            IActionNode last = self.last;
            if (last == null)
            {
                Log.E("There is nothing  in this node");
            }
            else
            {
                if (action != null)
                    action((T1)last);
            }
            return self;
        }

    }

}

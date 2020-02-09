using IFramework.Modules.Coroutine;
using System;
using System.Collections;

namespace IFramework.Modules.NodeAction
{
    public static class IActionNodeExtension
    {
        public static SequenceNode Sequence(this object self, bool autoRecyle = true)
        {
            SequenceNode node = RecyclableObject.Allocate<SequenceNode>();
            node.Config(autoRecyle);
            return node;
        }
        public static IEnumerator IActionEnumerator<T>(this T self) where T : IActionNode
        {
            while (self.MoveNext())
            {
                yield return null;
            }
        }


        public static T Run<T>(this T self, CoroutineModule moudle) where T : IActionNode
        {
            moudle.StartCoroutine(self.IActionEnumerator());
            return self;
        }

        public static T OnCompelete<T>(this T self, Action<T> action) where T : IActionNode
        {
            self.OnCompelete(() => { action(self); });
            return self;
        }
        public static T OnBegin<T>(this T self, Action<T> action) where T : IActionNode
        {
            return self.OnBegin(() => { action(self); });
        }
        public static T OnDispose<T>(this T self, Action<T> action) where T : IActionNode
        {
            return self.OnDispose(() => { action(self); });
        }
        public static T OnRecyle<T>(this T self, Action<T> action) where T : IActionNode
        {
            return self.OnRecyle(() => { action(self); });
        }
        public static T OnCompelete<T>(this T self, Action action) where T : IActionNode
        {
            self.onCompelete += action;
            return self;
        }
        public static T OnBegin<T>(this T self, Action action) where T : IActionNode
        {
            self.onBegin += action;
            return self;
        }
        public static T OnDispose<T>(this T self, Action action) where T : IActionNode
        {
            self.onDispose += action;
            return self;
        }
        public static T OnRecyle<T>(this T self, Action action) where T : IActionNode
        {
            self.onRecyle += action;
            return self;
        }

        public static T TimeSpan<T>(this T self, TimeSpan timeSpan, bool autoRecyle = false) where T : ContainerNode
        {
            TimeSpanNode node = RecyclableObject.Allocate<TimeSpanNode>();
            node.Config(timeSpan, autoRecyle);
            self.Append(node);
            return self;
        }
        public static T Until<T>(this T self, Func<bool> func, bool autoRecyle = false) where T : ContainerNode
        {
            UntilNode node = UntilNode.Allocate<UntilNode>();
            node.Config(func, autoRecyle);
            self.Append(node);
            return self;
        }
        public static T Event<T>(this T self, Action action, bool autoRecyle = false) where T : ContainerNode
        {
            EventNode node = EventNode.Allocate<EventNode>();
            node.Config(action, autoRecyle);
            self.Append(node);
            return self;
        }
        public static T Repeat<T>(this T self, Action<RepeatNode> action, int repeat = 1, bool autoRecyle = false) where T : ContainerNode
        {
            RepeatNode node = RepeatNode.Allocate<RepeatNode>();
            node.Config(repeat, autoRecyle);
            if (action != null)
                action(node);
            self.Append(node);
            return self;
        }

        public static T Sequence<T>(this T self, Action<SequenceNode> action, bool autoRecyle = false) where T : ContainerNode
        {
            SequenceNode node = SequenceNode.Allocate<SequenceNode>();
            node.Config(autoRecyle);
            if (action != null)
                action(node);
            self.Append(node);
            return self;
        }
        public static T Spawn<T>(this T self, Action<SpawnNode> action, bool autoRecyle = false) where T : ContainerNode
        {
            SpawnNode node = SpawnNode.Allocate<SpawnNode>();
            node.Config(autoRecyle);
            if (action != null)
                action(node);
            self.Append(node);
            return self;
        }

        private static RepeatNode Node(this RepeatNode self, ActionNode node)
        {
            self.node = node;
            return self;
        }
        public static RepeatNode TimeSpan(this RepeatNode self, TimeSpan timeSpan, bool autoRecyle = false)
        {
            TimeSpanNode node = TimeSpanNode.Allocate<TimeSpanNode>();
            node.Config(timeSpan, autoRecyle);
            return self.Node(node);
        }
        public static RepeatNode Until(this RepeatNode self, Func<bool> func, bool autoRecyle = false)
        {
            UntilNode node = UntilNode.Allocate<UntilNode>();
            node.Config(func, autoRecyle);
            return self.Node(node);
        }
        public static RepeatNode Event(this RepeatNode self, Action func, bool autoRecyle = false)
        {
            EventNode node = EventNode.Allocate<EventNode>();
            node.Config(func, autoRecyle);
            return self.Node(node);
        }

        public static RepeatNode Repeat(this RepeatNode self, Action<RepeatNode> action, int repeat, bool autoRecyle = false)
        {
            RepeatNode node = RepeatNode.Allocate<RepeatNode>();
            node.Config(repeat, autoRecyle);
            if (action != null)
                action(node);
            self.Node(node);
            return self;
        }
        public static RepeatNode Sequence(this RepeatNode self, Action<SequenceNode> action, bool autoRecyle = false)
        {
            SequenceNode node = SequenceNode.Allocate<SequenceNode>();
            node.Config(autoRecyle);
            if (action != null)
                action(node);
            self.Node(node);
            return self;
        }
        public static RepeatNode Spawn(this RepeatNode self, Action<SpawnNode> action, bool autoRecyle = false)
        {
            SpawnNode node = SpawnNode.Allocate<SpawnNode>();
            node.Config(autoRecyle);
            if (action != null)
                action(node);
            self.Node(node);
            return self;
        }
    }

}

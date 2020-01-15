using IFramework.Moudles.Coroutine;
using System;
using System.Collections;

namespace IFramework.Moudles.NodeAction
{
    public static class IActionNodeExtension
    {
        public static SequenceNode Sequence(this object self, bool autoDispose = true)
        {
            SequenceNode node = SequenceNode.Allocate(autoDispose);
            return node;
        }
        public static IEnumerator IActionEnumerator<T>(this T self) where T : IActionNode
        {
            while (self.MoveNext())
            {
                yield return null;
            }
        }


        public static T Run<T>(this T self, CoroutineMoudle moudle) where T : IActionNode
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

        public static T TimeSpan<T>(this T self, TimeSpan timeSpan, bool autoDispose = false) where T : ContainerNode
        {
            self.Append(TimeSpanNode.Allocate(timeSpan, autoDispose));
            return self;
        }
        public static T Until<T>(this T self, Func<bool> func, bool autoDispose = false) where T : ContainerNode
        {
            self.Append(UntilNode.Allocate(func, autoDispose));
            return self;
        }
        public static T Event<T>(this T self, Action action, bool autoDispose = false) where T : ContainerNode
        {
            EventNode node = EventNode.Allocate(action, autoDispose);
            self.Append(node);
            return self;
        }
        public static T Repeat<T>(this T self, Action<RepeatNode> action, int repeat = 1, bool autoDispose = false) where T : ContainerNode
        {
            RepeatNode node = RepeatNode.Allocate(repeat, autoDispose);
            if (action != null)
                action(node);
            self.Append(node);
            return self;
        }

        public static T Sequence<T>(this T self, Action<SequenceNode> action, bool autoDispose = false) where T : ContainerNode
        {
            SequenceNode node = SequenceNode.Allocate(autoDispose);
            if (action != null)
                action(node);
            self.Append(node);
            return self;
        }
        public static T Spawn<T>(this T self, Action<SpawnNode> action, bool autoDispose = false) where T : ContainerNode
        {
            SpawnNode node = SpawnNode.Allocate(autoDispose);
            if (action != null)
                action(node);
            self.Append(node);
            return self;
        }

        private static RepeatNode Node(this RepeatNode self, IActionNode node)
        {
            self.node = node;
            return self;
        }
        public static RepeatNode TimeSpan(this RepeatNode self, TimeSpan timeSpan, bool autoDispose = false)
        {
            return self.Node(TimeSpanNode.Allocate(timeSpan, autoDispose));
        }
        public static RepeatNode Until(this RepeatNode self, Func<bool> func, bool autoDispose = false)
        {
            return self.Node(UntilNode.Allocate(func, autoDispose));
        }
        public static RepeatNode Event(this RepeatNode self, Action func, bool autoDispose = false)
        {
            return self.Node(EventNode.Allocate(func, autoDispose));
        }

        public static RepeatNode Repeat(this RepeatNode self, Action<RepeatNode> action, int repeat, bool autoDispose = false)
        {
            RepeatNode node = RepeatNode.Allocate(repeat, autoDispose);
            if (action != null)
                action(node);
            self.Node(node);
            return self;
        }
        public static RepeatNode Sequence(this RepeatNode self, Action<SequenceNode> action, bool autoDispose = false)
        {
            SequenceNode node = SequenceNode.Allocate(autoDispose);
            if (action != null)
                action(node);
            self.Node(node);
            return self;
        }
        public static RepeatNode Spawn(this RepeatNode self, Action<SpawnNode> action, bool autoDispose = false)
        {
            SpawnNode node = SpawnNode.Allocate(autoDispose);
            if (action != null)
                action(node);
            self.Node(node);
            return self;
        }
    }

}

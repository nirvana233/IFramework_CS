﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace IFramework
{
    class ObservableDictionary<TKey,TValue>:Unit, IDictionary<TKey, TValue>
    {
        public event Action<TKey> OnAdded;
        public event Action<TKey, TValue> OnItemValueChanged;
        public event Action<KeyValuePair<TKey, TValue>> OnRemoved;
        public event Action OnClear;

        Dictionary<TKey, TValue> Value { get; set; }


        public ICollection<TKey> Keys
        {
            get { return Value.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return Value.Values; }
        }

        public int Count
        {
            get { return Value.Count; }
        }

        public TValue this[TKey key]
        {
            get { return Value[key]; }
            set
            {
                if (Value.ContainsKey(key))
                {
                    Value[key] = value;
                    OnItemValueChanged?.Invoke(key, value);
                }
                else
                {
                    Value[key] = value;
                    OnAdded?.Invoke(key);
                }
            }
        }

        public ObservableDictionary()
        {
            Value = new Dictionary<TKey, TValue>();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Add(TKey key, TValue value)
        {
            Value.Add(key, value);
            OnAdded?.Invoke(key);
        }

        public bool ContainsKey(TKey key)
        {
            return Value.ContainsKey(key);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            var value = Value[key];
            var result = Value.Remove(key);
            if (result)
                OnRemoved?.Invoke(new KeyValuePair<TKey, TValue>(key, value));
            return result;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Value.TryGetValue(key, out value);
        }

        public void Clear()
        {
            Value.Clear();
            OnClear?.Invoke();
        }

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        protected override void OnDispose()
        {
            Value.Clear();
        }
    }
}

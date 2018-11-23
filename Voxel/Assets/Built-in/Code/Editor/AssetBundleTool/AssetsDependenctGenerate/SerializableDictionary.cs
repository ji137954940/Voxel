using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Tgame.AssetBundle
{

    /// <summary>
    /// 支持序列化的字典类型
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        public List<TKey> _serializedKeys = new List<TKey>();

        public List<TValue> _serializedValues = new List<TValue>();

        public int _serializedCount;

        public bool _isSerializing;

        protected Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public int Count
        {
            get { return dictionary.Count; }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
        }

        internal Dictionary<TKey, TValue> GetDic()
        {
            return dictionary;
        }

        public void OnBeforeSerialize()
        {
            _isSerializing = true;

            _serializedKeys.Clear();
            _serializedKeys.Capacity = Count;
            _serializedValues.Clear();
            _serializedValues.Capacity = Count;

            foreach (var item in dictionary)
            {
                _serializedKeys.Add(item.Key);
                _serializedValues.Add(item.Value);
            }

            _serializedCount = Count;
        }

        private int index;
        public void OnAfterDeserialize()
        {
            dictionary.Clear();

            //Debug.Log(_serializedKeys.Count + " " + _serializedValues.Count + " " + _serializedCount + " HashCode:" + this.GetHashCode());

            if (_serializedCount != _serializedKeys.Count)
            {
                throw new SerializationException(string.Format("{0} failed to serialize.", typeof(TKey).Name));
            }
            if (_serializedCount != _serializedValues.Count)
            {
                throw new SerializationException(string.Format("{0} failed to serialize.", typeof(TValue).Name));
            }

            for (var i = 0; i < _serializedCount; ++i)
            {
                Add(_serializedKeys[i], _serializedValues[i]);
            }

            _serializedKeys.Clear();
            _serializedValues.Clear();

            //Debug.Log(_isSerializing + " " + Thread.CurrentThread.ManagedThreadId);

            _isSerializing = false;
        }
    }
}

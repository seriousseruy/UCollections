using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Extensions {
    public static class DictionaryExtentions {
        public static void Add<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict,
                                             IEnumerable<TValue> list,
                                             Func<TValue, TKey> keySelector) {
            if (dict == null)
                return;

            foreach (var value in list) {
                dict.AddOrUpdate(keySelector(value), value, (key, val) => value);
            }
        }

        public static void AddFromList<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict,
                                                     IEnumerable<TValue> list,
                                                     Func<TValue, TKey> keySelector) {
            if (dict == null)
                return;

            foreach (var value in list) {
                dict.AddOrUpdate(keySelector(value), value, (key, val) => value);
            }
        }

        public static void Update<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, Action<TValue> action) {
            if (dict == null)
                return;

            TValue value;
            if (dict.TryGetValue(key, out value))
                action(value);
        }

        public static TRValue Get<TKey, TValue, TRValue>(this Dictionary<TKey, TValue> dict, TKey key) {
            return dict.ContainsKey(key)
                       ? dict[key].Parse<TRValue>()
                       : default(TRValue);
        }

        public static T Get<T>(this Dictionary<string, string> dict, string key) {
            return dict.Get<string, string, T>(key);
        }

        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) {
            return dict.ContainsKey(key)
                       ? dict[key]
                       : default(TValue);
        }

        public static TValue Get<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key)
        {
            return dict.ContainsKey(key)
                       ? dict[key]
                       : default(TValue);
        }

        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> valueFunc) {
            return dict.ContainsKey(key)
                       ? dict[key]
                       : valueFunc();
        }

        public static TValue Get<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, Func<TValue> valueFunc)
        {
            return dict.ContainsKey(key)
                       ? dict[key]
                       : valueFunc();
        }

        public static bool AddNew<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) {
            if (dict.ContainsKey(key))
                return false;

            dict.Add(key, value);
            return true;
        }

        public static TValue AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) {
            if (dict.ContainsKey(key))
                dict[key] = value;
            else
                dict.Add(key, value);
            return value;
        }

        public static TValue AddOrGet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> valueFunc) {
            if (!dict.ContainsKey(key))
                dict.Add(key, valueFunc());

            return dict[key];
        }
    }
}
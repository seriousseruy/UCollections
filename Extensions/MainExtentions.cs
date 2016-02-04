using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Extensions {
    public static class MainExtentions {
        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> PropertyDictionary =
            new ConcurrentDictionary<Type, List<PropertyInfo>>();

        public static IEnumerable<PropertyInfo> GetProperties(Type type) {
            List<PropertyInfo> properties;
            if (PropertyDictionary.TryGetValue(type, out properties))
                return properties;
          
            properties = type.GetProperties()
                             .ToList();
            PropertyDictionary.TryAdd(type, properties);
            return properties;
        }

        public static bool In<T>(this T source, params T[] list) {
            return source != null && list.Contains(source);
        }

        public static bool NotIn<T>(this T source, params T[] list) {
            return (source != null && list.Contains(source)) == false;
        }

        public static bool Between<T>(this T actual, T lower, T upper) where T : IComparable<T> {
            return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) < 0;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            foreach (var element in source) {
                action(element);
            }
        }

        public static bool EqualsAll<T>(this IList<T> list) {
            if (list == null || list.Count < 2)
                return true;
            var first = list[0];
            for (var i = 1; i < list.Count; i++) {
                if (!first.Equals(list[i]))
                    return false;
            }
            return true;
        }

        public static bool EqualsAll<T>(params T[] list) {
            if (list == null || list.Length < 2)
                return true;
            var first = list[0];
            for (var i = 1; i < list.Length; i++) {
                if (!first.Equals(list[i]))
                    return false;
            }
            return true;
        }

        public static decimal GetMedian(this IEnumerable<int> source) {
            if (source == null)
                return 0;

            var temp = source.ToArray();
            Array.Sort(temp);

            if (temp.Length == 0)
                return 0;

            if (temp.Length%2 == 0)
                return (temp[temp.Length/2 - 1] + temp[temp.Length/2])/2m;
            return temp[temp.Length/2];
        }

        public static T Merge<T>(this T source, T newValue) where T : class {
            if (source == null)
                return null;

            if (newValue == null)
                return source;

            foreach (var info in GetProperties(source.GetType())) {
                if (info.CanWrite)
                    info.SetValue(source, info.GetValue(newValue));
            }
            return source;
        }

        public static T CloneR<T>(this T source) where T : class {
            if (source == null)
                return default(T);

            var result = Activator.CreateInstance<T>();

            foreach (var info in typeof (T).GetProperties()) {
                info.SetValue(result, info.GetValue(source));
            }

            return result;
        }

        public static T CloneJ<T>(this T source) where T : class {
            if (source == null)
                return default(T);

            var json = JsonConvert.SerializeObject(source);
            var result = JsonConvert.DeserializeObject<T>(json);

            return result;
        }

        /// <summary>
        /// Dequeue all items from queue and Invoke action for each item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="action">action to apply for each item</param>
        /// <param name="errorHandler"></param>
        public static void DequeueEach<T>(this ConcurrentQueue<T> queue, Action<T> action, Action<Exception> errorHandler) {
            if (queue != null && action != null && !queue.IsEmpty) {
                T item;
                while (queue.TryDequeue(out item)) {
                    try {
                        action(item);
                    }
                    catch (Exception ex) {
                        errorHandler?.Invoke(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Convert object to JSON string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="indented"></param>
        /// <returns>JSON string, if object is null return string.empty</returns>
        public static string ToJson<T>(this T obj, bool indented = false) {
            var result = string.Empty;

            if (obj != null) {
                var formatting = indented
                    ? Formatting.Indented
                    : Formatting.None;

               result = JsonConvert.SerializeObject(obj, formatting);
            }

            return result;
        }

        /// <summary>
        /// Convert object to indented JSON string, analog of ToJson(true)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>indented JSON string, if object is null return string.empty </returns>
        public static string ToJsonIndented<T>(this T obj)
        {
            return ToJson(obj, true);
        }

    }
}
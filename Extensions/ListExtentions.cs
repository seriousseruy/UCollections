using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Extensions {
    public static class ListExtentions {
        private const string MergeIdPattern = "{0}{1}{2}";

        public static List<List<int>> Split(this List<int> list, int width) {
            var result = new List<List<int>>();

            var numberOfLists = (list.Count/width) + 1;

            for (var i = 0; i < numberOfLists; i++) {
                result.Add(list.Skip(i*width)
                               .Take(width)
                               .ToList());
            }

            return result;
        }

        public static void AddRange<T>(this BindingList<T> collection, IEnumerable<T> items) {
            if (collection != null && items != null) {
                foreach (var item in items) {
                    collection.Add(item);
                }
            }
        }

        public static void AddClearRange<T>(this List<T> collection, IEnumerable<T> items) {
            if (collection != null) {
                collection.Clear();
                collection.AddRange(items);
            }
        }

        public static bool IsEmpty<T>(this IEnumerable<T> list) { return list == null || list.Any() == false; }

        public static List<T> ToListSafe<T>(this IEnumerable<T> list) {
            return list?.ToList() ?? new List<T>();
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> list) { return !IsEmpty(list); }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Action<T> act) {
            var enumerable = collection as IList<T>;
            if (enumerable != null) {
                foreach (var item in enumerable) {
                    act(item);
                }
                return enumerable;
            }

            return collection;
        }

        public static bool EqualString(this List<string> sourse, List<string> data) {
            return sourse.Count == data.Count && sourse.All(data.Contains);
        }

        public static List<string> Shrink(this List<string> sourse,
                                          string condition) {
            return sourse.Where(cond => cond.StartsWith(condition))
                         .ToList();
        }

        public static void AddIfNew<T>(this IList<T> list,
                                       T newItem) {
            if (list != null && !list.Contains(newItem)) {
                list.Add(newItem);
            }
        }

        public static T AddNew<T>(this IList<T> list,
                                  T newItem) {
            if (list != null) {
                list.Add(newItem);
                return newItem;
            }

            return default(T);
        }

        public static void AddItems<T>(this IList<T> collection,
                                       IEnumerable<T> items) {
            if (collection != null && items != null) {
                foreach (var item in items) {
                    collection.Add(item);
                }
            }
        }

        public static List<T> Clone<T>(this List<T> source) { return source.ToList(); }

        public static void CloneTo<T>(this List<T> source,
                                      List<T> target) {
            target.Clear();
            target.AddRange(source);
        }

        public static void CopyTo<T>(this List<T> source,
                                     List<T> target) { target.AddRange(source); }

        public static List<T> Merge<T>(this List<T> first,
                                       params List<T>[] items) {
            var result = new List<T>();
            first.CopyTo(result);
            foreach (var source in items) {
                source.CopyTo(result);
            }
            return result;
        }

        public static string Join<T>(this IList<T> source,
                                     string delimeter = ",",
                                     string valueDelimeter = "=") {
            if (source == null) {
                return string.Empty;
            }

            if (source.Count == 0) {
                return string.Empty;
            }

            var type = typeof (T);
            if (type == typeof (int) || type == typeof (long) || type == typeof (string)) {
                return source.JoinGeneric(delimeter);
            }

            if (type == typeof (bool)) {
                return ((IList<bool>) source).JoinBool(delimeter);
            }

            throw new Exception("invalig data type");
        }

        private static string JoinBool(this IEnumerable<bool> source,
                                       string delimeter = ",") {
            var ret = source.Aggregate(string.Empty,
                (current,
                 id) => string.Format(MergeIdPattern,
                     current,
                     delimeter,
                     id
                         ? '1'
                         : '0'));
            return ret.Substring(delimeter.Length);
        }

        private static string JoinGeneric<T>(this IEnumerable<T> source,
                                             string delimeter = ",") {
            if (delimeter == null) {
                delimeter = string.Empty;
            }

            return source == null
                ? string.Empty
                : string.Join(delimeter, source);
        }

        public static List<T> GetDistinct<T>(this List<T> sourse) {
            return sourse.Distinct()
                         .ToList();
        }

        public static List<T> DistinctBy<T, TKey>(this List<T> source, Func<T, TKey> keySelector) {
            var seenKeys = new HashSet<TKey>();
            return source == null || keySelector == null
                ? null
                : source.Where(element => seenKeys.Add(keySelector(element)))
                        .ToList();
        }

        public static string ItemOrDefault(this List<string> sourse,
                                           int index) {
            if (index < 0 || index >= sourse.Count) {
                return string.Empty;
            }
            return sourse[index];
        }
    }
}
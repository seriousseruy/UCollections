using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Extensions;

namespace UCollections {
    public class KeyObservableCollection<TKey, TValue> : ObservableCollection<TValue> where TValue : class {
        private readonly string _keyPropertyName;

        public KeyObservableCollection(string keyPropertyName) { _keyPropertyName = keyPropertyName; }

        public KeyObservableCollection() { }

        private PropertyInfo KeyProperty {
            get {
                return MainExtentions.GetProperties(typeof (TValue))
                                     .FirstOrDefault(n => n.Name == _keyPropertyName);
            }
        }

        public TKey GetKey(TValue value) {
            var keyProperty = KeyProperty?.GetValue(value);
            if (keyProperty != null) {
                var key = (TKey) keyProperty;
                return key;
            }
            throw new NullReferenceException("Key property is null");
        }

        public TValue AddOrMerge(TValue value) {
            var key = GetKey(value);
            if (ContainsKey(key)) {
                TValue sourceValue;
                if (TryGetValue(key, out sourceValue)) {
                    if (sourceValue != null) {
                        sourceValue.Merge(value);
                        return sourceValue;
                    }
                }
            } else {
                Add(value);
            }
            return value;
        }

        private void RemoveMissedList(ICollection<TValue> checkList) {
            try {
                if (checkList.Count > 0) {
                    var deleteList = new List<TValue>();

                    foreach (var value in Items) {
                        var key = GetKey(value);
                        var isContains = false;

                        foreach (var checkValue in checkList) {
                            var checkKey = GetKey(checkValue);

                            if (checkKey.Equals(key)) {
                                isContains = true;
                            }
                        }

                        if (!isContains) {
                            deleteList.Add(value);
                        }
                    }

                    foreach (var delValue in deleteList) {
                        Remove(delValue);
                    }
                } else {
                    Clear();
                }
            } catch (Exception exception) {
                throw new InvalidOperationException(exception.Message);
            }
        }

        public bool ContainsKey(TKey key) {
            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }
            TValue objValue;
            return TryGetValue(key, out objValue);
        }

        private bool TryGetValue(TKey key, out TValue objValue) {
            objValue = this.FirstOrDefault(value => key.Equals(GetKey(value)));
            return objValue != null;
        }

        public void UpdateItemsByList(ICollection<TValue> updeteCollection) {
            if (updeteCollection.Any()) {
                foreach (var updValue in updeteCollection) {
                    AddOrMerge(updValue);
                }

                RemoveMissedList(updeteCollection);
            }
        }

        public void AddRange(IEnumerable<TValue> newItems) {
            if (newItems != null) {
                foreach (var newItem in newItems) {
                    Add(newItem);
                }
            }
        }
    }
}
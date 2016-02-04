using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Extensions;

namespace UCollections {
    public class ObservableConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>, INotifyCollectionChanged,
                                                                INotifyPropertyChanged
        where TValue : class, INotifyPropertyChanged {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs changeAction) {
            var eh = CollectionChanged;
            eh?.Invoke(this, changeAction);
        }

        private void ValueOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) {
            var eh = PropertyChanged;
            eh?.Invoke(sender, null);
        }

        public new void Clear() {
            foreach (var value in Values) {
                value.PropertyChanged -= ValueOnPropertyChanged;
            }
            base.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new TValue AddOrUpdate(TKey key,
                                      Func<TKey, TValue> addValueFactory,
                                      Func<TKey, TValue, TValue> updateValueFactory) {
            var isUpdated = false;
            var oldValue = default(TValue);

            var value = base.AddOrUpdate(key,
                                         addValueFactory,
                                         (k, v) => {
                                             isUpdated = true;
                                             oldValue = v;
                                             return updateValueFactory(k, v);
                                         });
            if (!isUpdated)
                value.PropertyChanged += ValueOnPropertyChanged;

            OnCollectionChanged(isUpdated
                                    ? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue)
                                    : new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            return value;
        }

        public new TValue AddOrUpdate(TKey key,
                                      TValue addValue,
                                      Func<TKey, TValue, TValue> updateValueFactory) {
            var isUpdated = false;
            var oldValue = default(TValue);

            var value = base.AddOrUpdate(key,
                                         addValue,
                                         (k, v) => {
                                             isUpdated = true;
                                             oldValue = v;
                                             return updateValueFactory(k, v);
                                         });
            if (!isUpdated)
                value.PropertyChanged += ValueOnPropertyChanged;

            OnCollectionChanged(isUpdated
                                    ? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue)
                                    : new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            return value;
        }

        public TValue AddOrMerge(TKey key,
                                 TValue addValue) {
            var isMarged = false;
            var oldValue = default(TValue);

            var value = base.AddOrUpdate(key,
                                         addValue,
                                         (k, v) => {
                                             isMarged = true;
                                             oldValue = v;
                                             return v.Merge(addValue);
                                         });
            if (!isMarged)
                value.PropertyChanged += ValueOnPropertyChanged;

            OnCollectionChanged(isMarged
                                    ? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue)
                                    : new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            return value;
        }

        public new TValue GetOrAdd(TKey key, Func<TKey, TValue> addValueFactory) {
            var isAdded = false;

            var value = base.GetOrAdd(key,
                                      k => {
                                          isAdded = true;
                                          return addValueFactory(k);
                                      });

            if (!isAdded)
                return value;

            value.PropertyChanged += ValueOnPropertyChanged;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));

            return value;
        }

        public new TValue GetOrAdd(TKey key, TValue value) {
            return GetOrAdd(key, k => value);
        }

        public new bool TryAdd(TKey key, TValue value) {
            var tryAdd = base.TryAdd(key, value);

            if (!tryAdd)
                return false;

            value.PropertyChanged += ValueOnPropertyChanged;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));

            return true;
        }

        public new bool TryRemove(TKey key, out TValue value) {
            var tryRemove = base.TryRemove(key, out value);

            if (!tryRemove)
                return false;

            value.PropertyChanged -= ValueOnPropertyChanged;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));

            return true;
        }

        public new bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue) {
            var tryUpdate = base.TryUpdate(key, newValue, comparisonValue);

            if (!tryUpdate)
                return false;

            comparisonValue.PropertyChanged -= ValueOnPropertyChanged;
            newValue.PropertyChanged -= ValueOnPropertyChanged;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, comparisonValue));

            return true;
        }

        #region Constructors

        public ObservableConcurrentDictionary() {}

        public ObservableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : base(collection) {}

        public ObservableConcurrentDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer) {}

        public ObservableConcurrentDictionary(int concurrencyLevel, int capacity)
            : base(concurrencyLevel, capacity) {}

        public ObservableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : base(collection, comparer) {}

        public ObservableConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
            : base(concurrencyLevel, capacity, comparer) {}

        public ObservableConcurrentDictionary(int concurrencyLevel,
                                              IEnumerable<KeyValuePair<TKey, TValue>> collection,
                                              IEqualityComparer<TKey> comparer)
            : base(concurrencyLevel, collection, comparer) {}

        #endregion
    }
}
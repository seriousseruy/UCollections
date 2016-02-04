using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace UCollections {
    public class RangeObservableCollection<T> : ObservableCollection<T> {
        private bool _suppressNotification;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
            if (!_suppressNotification)
                base.OnCollectionChanged(e);
        }

        public void AddClearRange(IEnumerable<T> list) {
            _suppressNotification = true;

            Clear();

            if (list == null) {
                _suppressNotification = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                return;
            }

            AddRange(list);
        }

        public void AddRange(IEnumerable<T> list) {
            if (list == null)
                return;

            _suppressNotification = true;
            try {
                foreach (var item in list) {
                    Add(item);
                }
            }
            catch (Exception ex) {
                throw ex;
            }

            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void SortBy(string order) {
            var prop = typeof (T).GetProperty(order);
            if (prop == null)
                return;

            var items = Items.OrderBy(i => prop.GetValue(i))
                             .ToList();
            AddClearRange(items);
        }

        public void RemoveRange(IEnumerable<T> list) {
            if (list == null)
                return;

            _suppressNotification = true;

            foreach (var item in list) {
                Remove(item);
            }
            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace RemodelHelper
{
    public class IdentifiableTable<TValue> : IReadOnlyDictionary<int, TValue> where TValue : class, IIdentifiable
    {

        private readonly IDictionary<int, TValue> _dictionary;

        public TValue this[int key] => this._dictionary.ContainsKey(key) ? this._dictionary[key] : null;


        public IdentifiableTable() : this(new List<TValue>()) { }

        public IdentifiableTable(IEnumerable<TValue> source)
        {
            this._dictionary = source.ToDictionary(x => x.Id);
        }


        internal void Add(TValue value)
        {
            if (!this._dictionary.ContainsKey(value.Id))
                this._dictionary.Add(value.Id, value);
        }

        internal void Remove(TValue value)
        {
            this._dictionary.Remove(value.Id);
        }

        internal void Remove(int id)
        {
            this._dictionary.Remove(id);
        }


        #region IReadOnlyDictionary<TK, TV> members

        public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
        {
            return this._dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count => this._dictionary.Count;

        public bool ContainsKey(int key)
        {
            return this._dictionary.ContainsKey(key);
        }

        public bool TryGetValue(int key, out TValue value)
        {
            return this._dictionary.TryGetValue(key, out value);
        }

        public IEnumerable<int> Keys => this._dictionary.Keys;

        public IEnumerable<TValue> Values => this._dictionary.Values;

        #endregion
    }
}

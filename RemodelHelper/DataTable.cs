using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Grabacr07.KanColleWrapper.Models;

namespace RemodelHelper
{
    [DataContract]
    public class DataTable<TValue> : IReadOnlyDictionary<int, TValue> where TValue : class, IIdentifiable
    {
        [DataMember]
        private IDictionary<int, TValue> Dictionary { get; }

        public TValue this[int key] => this.Dictionary.ContainsKey(key) ? this.Dictionary[key] : null;


        internal DataTable() : this(new List<TValue>()) { }

        internal DataTable(IEnumerable<TValue> source)
        {
            this.Dictionary = source.ToDictionary(x => x.Id);
        }


        internal void Add(TValue value)
        {
            this.Dictionary.Add(value.Id, value);
        }

        internal void Remove(TValue value)
        {
            this.Dictionary.Remove(value.Id);
        }

        internal void Remove(int id)
        {
            this.Dictionary.Remove(id);
        }


        #region IReadOnlyDictionary<TK, TV> members

        public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
        {
            return this.Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count => this.Dictionary.Count;

        public bool ContainsKey(int key)
        {
            return this.Dictionary.ContainsKey(key);
        }

        public bool TryGetValue(int key, out TValue value)
        {
            return this.Dictionary.TryGetValue(key, out value);
        }

        public IEnumerable<int> Keys => this.Dictionary.Keys;

        public IEnumerable<TValue> Values => this.Dictionary.Values;

        #endregion
    }
}

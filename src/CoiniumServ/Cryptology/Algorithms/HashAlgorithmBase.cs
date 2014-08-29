using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using CoiniumServ.Pools;
using Newtonsoft.Json;

namespace CoiniumServ.Cryptology.Algorithms
{
    public class HashAlgorithmBase : IHashAlgorithm
    {
        private readonly IList<IPool> _storage;

        public HashAlgorithmBase()
        {
            _storage = new List<IPool>(); // initialize the pool storage.
        }

        public virtual uint Multiplier { get; protected set; }
        public virtual byte[] Hash(byte[] input, dynamic config)
        {
            throw new NotImplementedException();
        }

        public void AssignPools(IEnumerable<IPool> pools)
        {
            foreach (var pool in pools)
            {
                _storage.Add(pool);
            }
        }

        public IQueryable<IPool> SearchFor(Expression<Func<IPool, bool>> predicate)
        {
            return _storage.AsQueryable().Where(predicate);
        }

        public IEnumerable<IPool> GetAll()
        {
            return _storage;
        }

        public IQueryable<IPool> GetAllAsQueryable()
        {
            return _storage.AsQueryable();
        }

        public IReadOnlyCollection<IPool> GetAllAsReadOnly()
        {
            return new ReadOnlyCollection<IPool>(_storage);
        }

        public int Count { get { return _storage.Count; } }

        public IEnumerator<IPool> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int MinerCount { get; private set; }
        public ulong Hashrate { get; private set; }

        public string ServiceResponse { get; private set; }

        public void Recache()
        {
            MinerCount = 0;
            Hashrate = 0;

            foreach (var pool in _storage)
            {
                MinerCount += pool.MinerManager.Count;
                Hashrate += pool.Hashrate;
            }

            // cache the json-service response
            ServiceResponse = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
    }
}

namespace KGGEEP
{
    using RocksDbSharp;

    using System.Collections.Generic;
    using System.Text;

    internal sealed class RocksDbStore : IKeyValueStorage
    {
        private const string DefaultColumnFamily = "default";
        private RocksDb db;
        private ColumnFamilyHandle columnFamily;

        public RocksDbStore(string dir)
        {
            var options = new DbOptions()
                .SetCreateIfMissing(true)
                .SetCreateMissingColumnFamilies(true)
                ;

            var columnFamilies = new ColumnFamilies
            {
                { DefaultColumnFamily, new ColumnFamilyOptions() },
            };

            this.db = RocksDb.Open(options, dir, columnFamilies);
            this.columnFamily = this.db.GetColumnFamily(DefaultColumnFamily);
        }

        public void Put(string key, string value)
        {
            this.db.Put(key, value, this.columnFamily, encoding: Encoding.UTF8);
        }

        public bool TryGet(string key, out string value)
        {
            value = this.db.Get(key, this.columnFamily, encoding: Encoding.UTF8);
            return value != null;
        }

        public IReadOnlyList<string> Gets(string key)
        {
            using var snapshot = this.db.CreateSnapshot();
            var options = new ReadOptions().SetSnapshot(snapshot);
            using var iter = this.db.NewIterator(this.columnFamily, options);

            var prefix = Encoding.UTF8.GetBytes(key);
            var results = new List<string>();
            iter.Seek(prefix);

            while (iter.Valid())
            {
                results.Add(iter.StringValue());
                iter.Next();
            }

            return results;
        }

        public void Delete(string key)
        {
            this.db.Remove(key);
        }

        public void DeleteAll(string key)
        {
            using var iter = this.db.NewIterator(this.columnFamily);

            var bytes = Encoding.UTF8.GetBytes(key);
            iter.Seek(bytes);

            using var batch = new WriteBatch();
            while (iter.Valid())
            {
                var targetKey = iter.Key();
                byte[] copy = (byte[])targetKey.Clone();
                batch.Delete(copy, this.columnFamily);
                iter.Next();
            }

            this.db.Write(batch);
        }

        public void Dispose()
        {
            this.db?.Dispose();
            this.columnFamily = null;
            this.db = null;
        }
    }
}
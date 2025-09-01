namespace KGGEEP
{
    using System.Collections.Generic;

    internal sealed class MemoryStorage : IKeyValueStorage
    {
        private readonly Dictionary<string, string> caches = new();

        public void Put(string key, string value) => this.caches[key] = value ?? string.Empty;
        public bool TryGet(string key, out string value) => this.caches.TryGetValue(key, out value);
        public void Delete(string key) => this.caches.Remove(key);
        public void Dispose() => this.caches.Clear();
    }
}
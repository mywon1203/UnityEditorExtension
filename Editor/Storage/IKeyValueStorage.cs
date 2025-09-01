namespace KGGEEP
{
    using System;

    internal interface IKeyValueStorage : IDisposable
    {
        void Put(string key, string value);
        bool TryGet(string key, out string value);
        void Delete(string key);
    }
}
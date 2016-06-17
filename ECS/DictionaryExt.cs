using System.Collections.Generic;

namespace ECS
{
    public static class DictionaryExt
    {
        public static TValue GetOrAddNew<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key)
            where TValue : class, new()
        {
            TValue value;
            if (!self.TryGetValue(key, out value))
            {
                value = new TValue();
                self.Add(key, value);
            }

            return value;
        }
    }
}

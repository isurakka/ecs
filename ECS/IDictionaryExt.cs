using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public static class IDictionaryExt
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

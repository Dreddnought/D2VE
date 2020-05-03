using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2VE
{
    public class StatInfo
    {
        public StatInfo(string name) { Name = name; }
        public string Name { get; }
    }
    public class StatCache
    {
        public StatCache() { }
        public StatInfo GetStatInfo(string hash)
        {
            StatInfo statInfo;
            if (!_cache.TryGetValue(hash, out statInfo))
            {
                // Look it up.
                dynamic item = D2VE.Request("Destiny2/Manifest/DestinyInventoryItemDefinition/" + hash);
            }
            return statInfo;
        }
        private StatInfo Convert(dynamic item)
        {
            return new StatInfo(item.Name);
        }
        private Dictionary<string, StatInfo> _cache = new Dictionary<string, StatInfo>();
    }
}

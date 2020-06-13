using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace D2VE
{
    public class PowerCapCache
    {
        private bool _dirty;
        public PowerCapCache() { }
        public long GetPowerCapValue(long powerCapHash)
        {
            long powerCap;
            if (!_cache.TryGetValue(powerCapHash, out powerCap))  // Look it up
            {
                _cache[powerCapHash] = powerCap = D2VE.Request("Destiny2/Manifest/DestinyPowerCapDefinition/"
                    + powerCapHash.ToString()).powerCap.Value;
                _dirty = true;
            }
            return powerCap;
        }
        public void Save()
        {
            if (!_dirty)  // No changes
                return;
            try
            {
                string cache = JsonConvert.SerializeObject(_cache, Formatting.Indented);
                Persister.Save("PowerCapCache", cache);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to save power cap cache: " + x.Message);
            }
        }
        public void Load()
        {
            try
            {
                string cache = Persister.Load("PowerCapCache");
                if (!string.IsNullOrWhiteSpace(cache))
                    _cache = JsonConvert.DeserializeObject<Dictionary<long, long>>(cache);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to load power cap cache: " + x.Message);
            }
            _dirty = false;
        }
        private Dictionary<long, long> _cache = new Dictionary<long, long>();
    }
}

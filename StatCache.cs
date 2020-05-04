using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace D2VE
{
    public class StatCache
    {
        private bool _dirty;
        public StatCache() { }
        public string GetStatName(long statHash)
        {
            string name;
            if (!_cache.TryGetValue(statHash, out name))  // Look it up
            {
                _cache[statHash] = name = ConvertValue.StatName(D2VE.Request("Destiny2/Manifest/DestinyStatDefinition/"
                    + statHash.ToString()).displayProperties.name.Value);
                _dirty = true;
            }
            return name;
        }
        public void Save()
        {
            if (!_dirty)  // No changes
                return;
            try
            {
                string cache = JsonConvert.SerializeObject(_cache, Formatting.Indented);
                Persister.Save("StatCache", cache);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to load stat cache: " + x.Message);
            }
        }
        public void Load()
        {
            try
            {
                string cache = Persister.Load("StatCache");
                if (!string.IsNullOrWhiteSpace(cache))
                    _cache = JsonConvert.DeserializeObject<Dictionary<long, string>>(cache);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to load stat cache: " + x.Message);
            }
            _dirty = false;
        }
        private Dictionary<long, string> _cache = new Dictionary<long, string>();
    }
}

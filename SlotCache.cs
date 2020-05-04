using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace D2VE
{
    public class SlotCache
    {
        private bool _dirty;
        public SlotCache() { }
        public string GetSlotName(long slotHash)
        {
            string name;
            if (!_cache.TryGetValue(slotHash, out name))  // Look it up
            {
                _cache[slotHash] = name = D2VE.Request("Destiny2/Manifest/DestinyEquipmentSlotDefinition/"
                    + slotHash.ToString()).displayProperties.name.Value.Replace(" Weapons", "");
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
                Persister.Save("SlotCache", cache);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to load slot cache: " + x.Message);
            }
        }
        public void Load()
        {
            try
            {
                string cache = Persister.Load("SlotCache");
                if (!string.IsNullOrWhiteSpace(cache))
                    _cache = JsonConvert.DeserializeObject<Dictionary<long, string>>(cache);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to load slot cache: " + x.Message);
            }
            _dirty = false;
        }
        private Dictionary<long, string> _cache = new Dictionary<long, string>();
    }
}

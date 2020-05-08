using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace D2VE
{
    public class Perk
    {
        public Perk(string name) { Name = name; }
        public string Name { get; }
        public override string ToString() { return Name; }
    }
    public class PerkCache
    {
        private bool _dirty;
        public PerkCache() { }
        public Perk GetPerk(long perkHash)
        {
            Perk perk = null;
            if (!_cache.TryGetValue(perkHash, out perk))  // Look it up
            {
                dynamic item = D2VE.Request("Destiny2/Manifest/DestinySandboxPerkDefinition/" + perkHash.ToString());
                if (item?.displayProperties?.name != null)
                    perk = new Perk(item.displayProperties.name.Value);
                _cache[perkHash] = perk;
                _dirty = true;
            }
            return perk;
        }
        public void Save()
        {
            if (!_dirty)  // No changes
                return;
            try
            {
                string cache = JsonConvert.SerializeObject(_cache, Formatting.Indented);
                Persister.Save("PerkCache", cache);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to save perk cache: " + x.Message);
            }
        }
        public void Load()
        {
            try
            {
                string cache = Persister.Load("PerkCache");
                if (!string.IsNullOrWhiteSpace(cache))
                    _cache = JsonConvert.DeserializeObject<Dictionary<long, Perk>>(cache);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to load perk cache: " + x.Message);
            }
            _dirty = false;
        }
        private Dictionary<long, Perk> _cache = new Dictionary<long, Perk>();
    }
}

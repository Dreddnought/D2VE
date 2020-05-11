using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace D2VE
{
    public class Plug
    {
        public Plug(string name, string plugType, SortedDictionary<string, long> stats) { Name = name; PlugType = plugType; Stats = stats; }
        public string Name { get; }
        public string PlugType { get; }
        public SortedDictionary<string, long> Stats { get; }
        public override string ToString()
        {
            if (Stats.Count == 0)
                return Name;
            return Name + " (" + string.Join("; ", Stats.Keys.Zip(Stats.Values, (k, v) =>
                ConvertValue.StatSortedName(k) + (v > 0L ? " +" : " ") + v.ToString())) + ")";
        }
    }
    public class PlugCache
    {
        private bool _dirty;
        public PlugCache() { }
        public Plug GetPlug(long plugHash)
        {
            Plug plug = null;
            if (!_cache.TryGetValue(plugHash, out plug))  // Look it up
            {
                dynamic item = D2VE.Request("Destiny2/Manifest/DestinyInventoryItemDefinition/" + plugHash.ToString());
                string plugName = item?.displayProperties?.name;
                if (!string.IsNullOrWhiteSpace(plugName))
                {
                    string type = ConvertValue.ItemSubType(item.itemSubType.Value);
                    if (type == "None")  // We expect None or Mask, Shader or Ornament which we ignore
                    {
                        string plugType = ConvertValue.PlugTypeName(item.itemTypeDisplayName.Value);
                        if (plugType != "Temper Effect")  // Obsidian Radiance on forge weapons
                        {
                            SortedDictionary<string, long> stats = new SortedDictionary<string, long>();
                            dynamic investmentStats = item["investmentStats"];
                            if (investmentStats != null)
                                foreach (var investmentStat in investmentStats)
                                {
                                    long statHash = investmentStat.statTypeHash.Value;
                                    string statName = D2VE.StatCache.GetStatName(statHash);
                                    if (string.IsNullOrWhiteSpace(statName) || 
                                        statName.Contains(" Cost") || statName.Contains(" Energy Capacity"))
                                        continue;  // Not one we're interested in
                                    long value = investmentStat.value.Value;
                                    stats[statName] = value;
                                }
                            plug = new Plug(plugName, plugType, stats);
                        }
                    }
                }
                _cache[plugHash] = plug;
                _dirty = true;
            }
            return plug;
        }
        public void Save()
        {
            if (!_dirty)  // No changes
                return;
            try
            {
                string cache = JsonConvert.SerializeObject(_cache, Formatting.Indented);
                Persister.Save("PlugCache", cache);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to save plug cache: " + x.Message);
            }
        }
        public void Load()
        {
            try
            {
                string cache = Persister.Load("PlugCache");
                if (!string.IsNullOrWhiteSpace(cache))
                    _cache = JsonConvert.DeserializeObject<Dictionary<long, Plug>>(cache);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to load plug cache: " + x.Message);
            }
            _dirty = false;
        }
        private Dictionary<long, Plug> _cache = new Dictionary<long, Plug>();
    }
}

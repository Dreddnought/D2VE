using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace D2VE
{
    public class ItemInfo
    {
        public ItemInfo(string name, string itemType, string season)
        {
            Name = name;
            ItemType = itemType;
            Season = season;
        }
        public string Name { get; }
        public string ItemType { get; }
        public string Season { get; }
        public override string ToString() { return Name + " (" + ItemType + ")"; }
    }
    public class ItemCache
    {
        private bool _dirty;
        public ItemCache() { }
        public ItemInfo GetItemInfo(string itemHash)
        {
            ItemInfo itemInfo;
            if (!_cache.TryGetValue(itemHash, out itemInfo))  // Look it up
            {
                _cache[itemHash] = itemInfo =
                    Convert(D2VE.Request("Destiny2/Manifest/DestinyInventoryItemDefinition/" + itemHash));
                _dirty = true;
            }
            return itemInfo;
        }
        public void Save()
        {
            if (!_dirty)  // No changes
                return;
            try
            {
                string cache = JsonConvert.SerializeObject(_cache, Formatting.Indented);
                Persister.Save("ItemCache", cache);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to load item cache: " + x.Message);
            }
        }
        public void Load()
        {
            try
            {
                string cache = Persister.Load("ItemCache");
                if (!string.IsNullOrWhiteSpace(cache))
                    _cache = JsonConvert.DeserializeObject<Dictionary<string, ItemInfo>>(cache);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to load item cache: " + x.Message);
            }
            _dirty = false;
        }
        private ItemInfo Convert(dynamic definition)
        {
            ItemInfo itemInfo;
            long itemType = definition.itemType.Value;
            if (itemType != 2L && itemType != 3L)  // 2 = weapon, 3 = armor
                itemInfo = null;
            else
                itemInfo = new ItemInfo(
                    definition.itemTypeDisplayName.Value,
                    definition.displayProperties.name.Value,
                    definition.seasonHash?.Value.ToString());
            return itemInfo;
        }
        private Dictionary<string, ItemInfo> _cache = new Dictionary<string, ItemInfo>();
    }
}

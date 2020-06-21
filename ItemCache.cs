using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace D2VE
{
    public class ItemInfo
    {
        public ItemInfo(string name, string tierType, string itemCategory, string itemType, string slot, string energyType,
            string season, string classType, Dictionary<string, long> stats, List<long> powerCaps)
        {
            Name = name;
            TierType = tierType;
            ItemCategory = itemCategory;
            ItemType = itemType;
            Slot = slot;
            EnergyType = energyType;
            Season = season;
            ClassType = classType;
            Stats = stats;
            PowerCaps = powerCaps;
        }
        public string Name { get; }
        public string TierType { get; }
        public string ItemCategory { get; }
        public string ItemType { get; }
        public string Slot { get; }
        public string EnergyType { get; }
        public string Season { get; }
        public string ClassType { get; }
        public Dictionary<string, long> Stats { get; }
        public List<long> PowerCaps { get; }
        public override string ToString() { return Name; }
    }
    public class ItemCache
    {
        private bool _dirty;
        public ItemCache() { }
        public ItemInfo GetItemInfo(long itemHash)
        {
            ItemInfo itemInfo;
            if (!_cache.TryGetValue(itemHash, out itemInfo))  // Look it up
            {
                _cache[itemHash] = itemInfo =
                    Convert(D2VE.Request("Destiny2/Manifest/DestinyInventoryItemDefinition/" + itemHash.ToString()));
                _dirty = true;
            }
            return itemInfo;
        }
        // For items in the season pass where we haven't got the hash.
        public ItemInfo GetItemInfo(string name)
        {
            foreach (var kvp in _cache)
                if (kvp.Value?.Name == name)
                    return kvp.Value;
            return null;
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
                Console.WriteLine("Failed to save item cache: " + x.Message);
            }
        }
        public void Load()
        {
            try
            {
                string cache = Persister.Load("ItemCache");
                if (!string.IsNullOrWhiteSpace(cache))
                    _cache = JsonConvert.DeserializeObject<Dictionary<long, ItemInfo>>(cache);
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
            if (itemType != 2L && itemType != 3L)  // 2 = armor, 3 = weapon
                itemInfo = null;
            else
            {
                string itemCategory = itemType == 2L ? "Armor" : "Weapon";
                // Items have some base stats (some hidden).  These may be overridden at the instance level.
                Dictionary<string, long> stats = new Dictionary<string, long>();
                foreach (var stat in definition.stats.stats)
                {
                    long value = stat.Value["value"].Value;
                    if (value == 0L)  // Some stats seem deprecated and have a value of zero
                        continue;
                    long statHash = stat.Value["statHash"].Value;
                    string statName = D2VE.StatCache.GetStatName(statHash);
                    stats[statName] = value;
                }
                string tierType = definition.inventory.tierTypeName.Value;
                string slot = "";
                string energyType = "";
                string season = "";
                if (itemType == 2L)  // Armor, energyType is at instance level
                {
                    slot = D2VE.SlotCache.GetSlotName(definition.equippingBlock.equipmentSlotTypeHash.Value);
                    // Get the season.
                    // First find the socket indexes for the ARMOR MODS category, 590099826.
                    int lastIndex = -1;
                    foreach (dynamic socketCategory in definition.sockets.socketCategories)
                        if (socketCategory.socketCategoryHash == 590099826L)
                        {
                            lastIndex = (int)socketCategory.socketIndexes.Last.Value;
                            break;
                        }
                    if (lastIndex != -1)
                    {
                        long socketTypeHash = definition.sockets.socketEntries[lastIndex].socketTypeHash.Value;
                        season = D2VE.SeasonCache.GetSeasonName(socketTypeHash);
                    }
                }
                else  // Weapon
                {
                    energyType = ConvertValue.DamageType(definition.defaultDamageType?.Value);
                    slot = D2VE.SlotCache.GetSlotName(definition.equippingBlock.equipmentSlotTypeHash.Value);
                }
                // PowerCaps
                List<long> powerCaps = new List<long>();
                foreach (dynamic version in definition.quality.versions)
                    powerCaps.Add(D2VE.PowerCapCache.GetPowerCapValue(version.powerCapHash.Value));
                itemInfo = new ItemInfo(
                   definition.displayProperties.name.Value,
                   tierType,
                   itemCategory,
                   definition.itemTypeDisplayName.Value,
                   slot,
                   energyType,
                   season,
                   ConvertValue.ClassType(definition.classType?.Value ?? 0L),
                   stats,
                   powerCaps);
            }
            return itemInfo;
        }
        private Dictionary<long, ItemInfo> _cache = new Dictionary<long, ItemInfo>();
    }
}

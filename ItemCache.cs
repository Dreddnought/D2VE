using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace D2VE
{
    public class ItemInfo
    {
        public ItemInfo(string name, string itemCategory, string itemType, string slot, string energyType, string season,
            string classType, Dictionary<string, long> stats)
        {
            Name = name;
            ItemCategory = itemCategory;
            ItemType = itemType;
            Slot = slot;
            EnergyType = energyType;
            Season = season;
            ClassType = classType;
            Stats = stats;
        }
        public string Name { get; }
        public string ItemCategory { get; }
        public string ItemType { get; }
        public string Slot { get; }
        public string EnergyType { get; }
        public string Season { get; }
        public string ClassType { get; }
        public Dictionary<string, long> Stats { get; }
        public override string ToString()
        {
            return Name + " (" + ItemType + ") " + Slot + " " + EnergyType + " " + Season + " " + ClassType;
        }
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
                string slot = "";
                string energyType = "";
                if (itemType == 2L)  // Armor, energyType is at instance level
                    slot = D2VE.SlotCache.GetSlotName(definition.equippingBlock.equipmentSlotTypeHash.Value);
                else  // Weapon
                {
                    energyType = ConvertValue.DamageType(definition.defaultDamageType?.Value);
                    slot = D2VE.SlotCache.GetSlotName(definition.equippingBlock.equipmentSlotTypeHash.Value);
                }
                itemInfo = new ItemInfo(
                   definition.itemTypeDisplayName.Value,
                   itemCategory,
                   definition.displayProperties.name.Value,
                   slot,
                   energyType,
                   ConvertValue.Season(definition.seasonHash?.Value ?? 0L),
                   ConvertValue.ClassType(definition.classType?.Value ?? 0L),
                   stats);
            }
            return itemInfo;
        }
        private Dictionary<long, ItemInfo> _cache = new Dictionary<long, ItemInfo>();
    }
}

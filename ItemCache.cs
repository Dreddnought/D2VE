using Newtonsoft.Json;

namespace D2VE;

public class ItemInfo
{
    public ItemInfo(string name, string tierType, string itemCategory, string itemType, string slot,
        string energyType, string classType, string artifice, Dictionary<string, long> stats)
    {
        Name = name;
        TierType = tierType;
        ItemCategory = itemCategory;
        ItemType = itemType;
        Slot = slot;
        EnergyType = energyType;
        ClassType = classType;
        Artifice = artifice;
        Stats = stats;
    }
    public string Name { get; }
    public string TierType { get; }
    public string ItemCategory { get; }
    public string ItemType { get; }
    public string Slot { get; }
    public string EnergyType { get; }
    public string EnergyCapacity { get; }
    public string ClassType { get; }
    public string Artifice { get; }
    public Dictionary<string, long> Stats { get; }
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
    public ItemInfo GetItemInfo(string name, long itemHash)
    {
        // If we know the hash use it.
        if (itemHash != 0)
            return GetItemInfo(itemHash);
        // Otherwise look it up by name and hope we've already got one.
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
            string artifice = "FALSE";
            if (itemType == 2L)  // Armor
            {
                slot = D2VE.SlotCache.GetSlotName(definition.equippingBlock.equipmentSlotTypeHash.Value);
                foreach (dynamic socketEntry in definition.sockets.socketEntries)
                    if (socketEntry.singleInitialItemHash.Value == 3727270518L)
                    {
                        artifice = "TRUE";
                        break;
                    }
                    else if (socketEntry.singleInitialItemHash.Value == 0  
                        && socketEntry.socketTypeHash == "965959289")
                    {
                        artifice = "MAYBE";
                        break;
                    }
            }
            else  // Weapon
            {
                energyType = ConvertValue.DamageType(definition.defaultDamageType?.Value);
                slot = D2VE.SlotCache.GetSlotName(definition.equippingBlock.equipmentSlotTypeHash.Value);
            }
            itemInfo = new ItemInfo(
               definition.displayProperties.name.Value,
               tierType,
               itemCategory,
               definition.itemTypeDisplayName.Value,
               slot,
               energyType,
               ConvertValue.ClassType(definition.classType?.Value ?? 0L),
               artifice,
               stats);
        }
        return itemInfo;
    }
    private Dictionary<long, ItemInfo> _cache = new Dictionary<long, ItemInfo>();
}

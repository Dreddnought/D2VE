namespace D2VE;

public class ItemInstance
{
    public ItemInstance(string itemInstanceId, ItemInfo itemInfo, long power,
        string energyType, long energyCapacity, string masterwork,
        SortedDictionary<string, long> stats, SortedDictionary<string, Plug> plugs)
    {
        ItemInstanceId = itemInstanceId;
        ItemInfo = itemInfo;
        Power = power;
        EnergyType = energyType;
        EnergyCapacity = energyCapacity;
        Masterwork = masterwork;
        Stats = stats;
        Plugs = plugs;
    }
    public string ItemInstanceId { get; }
    public ItemInfo ItemInfo { get; }
    public string Name { get { return ItemInfo.Name; } }
    public long Power { get; }
    public string ItemCategory { get { return ItemInfo.ItemCategory; } }
    public string TierType { get { return ItemInfo.TierType; } }
    public string ItemType { get { return ItemInfo.ItemType; } }
    public string Slot { get { return ItemInfo.Slot; } }
    public string ClassType { get { return ItemInfo.ClassType; } }
    public string Artifice { get { return ItemInfo.Artifice; } }
    public string EnergyType { get; }
    public long EnergyCapacity { get; }
    public string Masterwork { get; }
    public SortedDictionary<string, long> Stats { get; }
    public SortedDictionary<string, Plug> Plugs { get; }
    public override string ToString()
    {
        return ItemInstanceId + " " + Name + " " + Power.ToString()
            + " (" + ItemType + ") " + Slot + " " + (string.IsNullOrWhiteSpace(EnergyType) ? "" : EnergyType) + " "
            + EnergyCapacity.ToString() + " " + TierType
            + (string.IsNullOrWhiteSpace(ClassType) ? "" : " [" + ClassType + "]")
            + (Artifice == "FALSE" ? "" : " Artifice")
            + "\r\n  " + string.Join("\r\n  ",
            Stats.Keys.Zip(Stats.Values, (k, v) => ConvertValue.StatSortedName(k) + " = " + v))
            + "\r\n  " + string.Join("\r\n  ",
            Plugs.Keys.Zip(Plugs.Values, (k, v) => k + " = " + v));
    }
}

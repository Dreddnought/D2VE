namespace D2VE;

public class Armor
{
    public Armor(string itemInstanceId, string name, string classType, string tierType, string itemType,
        bool artifice, long energyCapacity,
        long mobility, long resilience, long recovery, long discipline, long intellect, long strength)
    {
        ItemInstanceId = itemInstanceId;
        Name = name;
        ClassType = classType;
        TierType = tierType;
        ItemType = itemType;
        Artifice = artifice;
        EnergyCapacity = energyCapacity;
        Mobility = mobility;
        Resilience = resilience;
        Recovery = recovery;
        Discipline = discipline;
        Intellect = intellect;
        Strength = strength;
        BaseStats = mobility + resilience + recovery + discipline + intellect + strength;
        Mrr = mobility + resilience + recovery;
        Dis = discipline + intellect + strength;
        RI = recovery + intellect;
        RD = recovery + discipline;
        Id = Name + " " + (artifice ? BaseStats - 3 : BaseStats).ToString() + "(" + RI.ToString() + ")="
            + Mrr.ToString() + "+" + Dis.ToString()
            + " (" + (Name.EndsWith("(mob)") ? Mobility - 3 : Mobility).ToString()
            + "-" + (Name.EndsWith("(res)") ? Resilience - 3 : Resilience).ToString()
            + "-" + (Name.EndsWith("(rec)") ? Recovery - 3 : Recovery).ToString()
            + "-" + (Name.EndsWith("(dis)") ? Discipline - 3 : Discipline).ToString()
            + "-" + (Name.EndsWith("(int)") ? Intellect - 3 : Intellect).ToString()
            + "-" + (Name.EndsWith("(str)") ? Strength - 3 : Strength).ToString() + ")";
    }
    public static List<Armor> LegendaryClassItems = new List<Armor>()
    {
        //new Armor("", "Artifice Bond (mob)", "Warlock", "Legendary", "Class Item", true, 0, 3, 0, 0, 0, 0, 0),
        new Armor("", "Artifice Bond (res)", "Warlock", "Legendary", "Class Item", true, 0, 0, 3, 0, 0, 0, 0),
        new Armor("", "Artifice Bond (rec)", "Warlock", "Legendary", "Class Item", true, 0, 0, 0, 3, 0, 0, 0),
        new Armor("", "Artifice Bond (dis)", "Warlock", "Legendary", "Class Item", true, 0, 0, 0, 0, 3, 0, 0),
        new Armor("", "Artifice Bond (int)", "Warlock", "Legendary", "Class Item", true, 0, 0, 0, 0, 0, 3, 0),
        new Armor("", "Artifice Bond (str)", "Warlock", "Legendary", "Class Item", true, 0, 0, 0, 0, 0, 0, 3),
        new Armor("", "Artifice Cloak (mob)", "Hunter", "Legendary", "Class Item", true, 0, 3, 0, 0, 0, 0, 0),
        new Armor("", "Artifice Cloak (res)", "Hunter", "Legendary", "Class Item", true, 0, 0, 3, 0, 0, 0, 0),
        new Armor("", "Artifice Cloak (rec)", "Hunter", "Legendary", "Class Item", true, 0, 0, 0, 3, 0, 0, 0),
        new Armor("", "Artifice Cloak (dis)", "Hunter", "Legendary", "Class Item", true, 0, 0, 0, 0, 3, 0, 0),
        new Armor("", "Artifice Cloak (int)", "Hunter", "Legendary", "Class Item", true, 0, 0, 0, 0, 0, 3, 0),
        new Armor("", "Artifice Cloak (str)", "Hunter", "Legendary", "Class Item", true, 0, 0, 0, 0, 0, 0, 3),
        //new Armor("", "Artifice Mark (mob)", "Titan", "Legendary", "Class Item", true, 0, 3, 0, 0, 0, 0, 0),
        new Armor("", "Artifice Mark (res)", "Titan", "Legendary", "Class Item", true, 0, 0, 3, 0, 0, 0, 0),
        new Armor("", "Artifice Mark (rec)", "Titan", "Legendary", "Class Item", true, 0, 0, 0, 3, 0, 0, 0),
        new Armor("", "Artifice Mark (dis)", "Titan", "Legendary", "Class Item", true, 0, 0, 0, 0, 3, 0, 0),
        new Armor("", "Artifice Mark (int)", "Titan", "Legendary", "Class Item", true, 0, 0, 0, 0, 0, 3, 0),
        new Armor("", "Artifice Mark (str)", "Titan", "Legendary", "Class Item", true, 0, 0, 0, 0, 0, 0, 3)
    };
    public string ItemInstanceId { get; }
    public string Name { get; }
    public string ClassType { get; }
    public string TierType { get; }
    public string ItemType { get; }
    public bool Artifice { get; }
    public long EnergyCapacity { get; }
    public long Mobility { get; }
    public long Resilience { get; }
    public long Recovery { get; }
    public long Discipline { get; }
    public long Intellect { get; }
    public long Strength { get; }
    public long BaseStats { get; }
    public long Mrr { get; }
    public long Dis { get; }
    public long RI { get; }
    public long RD { get; }
    public string Id { get; }
    public override string ToString() { return Id; }
}

public class ArmorCalculator
{
    public ArmorCalculator(string classType, string exotic,
        long mobilityMod = 0L, long resilienceMod = 0L, long recoveryMod = 0L,
        long disciplineMod = 0L, long intellectMod = 0L, long strengthMod = 0L)
    {
        string mods = mobilityMod.ToString() + "," + resilienceMod.ToString() + "," + recoveryMod.ToString()
            + "," + disciplineMod.ToString() + "," + intellectMod.ToString() + "," + strengthMod.ToString();
        Name = classType + " - " + exotic.Replace("/", "") + (mods == "0,0,0,0,0,0" ? "" : " - " + mods);
        ClassType = classType;
        Exotic = exotic;
        MobilityMod = mobilityMod;
        ResilienceMod = resilienceMod;
        RecoveryMod = recoveryMod;
        DisciplineMod = disciplineMod;
        IntellectMod = intellectMod;
        StrengthMod = strengthMod;
    }
    /// <summary>Name for the calculation.</summary>
    public string Name { get; }
    /// <summary>Class type (Warlock, Hunter or Titan).</summary>
    public string ClassType { get; }
    /// <summary>Exotic name to base the build on.</summary>
    public string Exotic { get; }
    /// <summary>Expected mobility mod value.</summary>
    public long MobilityMod { get; }
    /// <summary>Expected resilence mod value. (Optional)</summary>
    public long ResilienceMod { get; }
    public long RecoveryMod { get; }
    public long DisciplineMod { get; }
    public long IntellectMod { get; }
    public long StrengthMod { get; }
    public Category Calculate(List<Armor> armorItems)
    {
        Category category = new Category(Name);
        category.ColumnIndex("DIM Query");
        category.ColumnIndex("Name");
        category.ColumnIndex("Helmet");
        category.ColumnIndex("Gauntlets");
        category.ColumnIndex("Chest Armor");
        category.ColumnIndex("Leg Armor");
        category.ColumnIndex("Class Item");
        category.ColumnIndex("Mobility");
        category.ColumnIndex("Resilience");
        category.ColumnIndex("Recovery");
        category.ColumnIndex("Discipline");
        category.ColumnIndex("Intellect");
        category.ColumnIndex("Strength");
        category.ColumnIndex("Res+Dis");
        category.ColumnIndex("RR");
        category.ColumnIndex("RD");
        category.ColumnIndex("DI");
        category.ColumnIndex("RRI");
        category.ColumnIndex("RRD");
        category.ColumnIndex("RRS");
        category.ColumnIndex("MRD");
        category.ColumnIndex("Usage");
        category.ColumnIndex("Artifice");
        category.ColumnIndex("Wastage");
        category.ColumnIndex("LowestBaseStats");
        category.ColumnIndex("Mob");
        category.ColumnIndex("Res");
        category.ColumnIndex("Rec");
        category.ColumnIndex("Dis");
        category.ColumnIndex("Int");
        category.ColumnIndex("Str");
        category.ColumnIndex("MRR");
        category.ColumnIndex("DIS");
        category.ColumnIndex("Solstice Rekindled");
        category.ColumnIndex("Exotic");
        category.ColumnIndex("ExoticType");
        category.ColumnIndex("Masterworked");
        category.ColumnIndex("Exotic Masterworked");
        category.ColumnIndex("Head Masterworked");
        category.ColumnIndex("Arms Masterworked");
        category.ColumnIndex("Chest Masterworked");
        category.ColumnIndex("Leg Masterworked");
        category.ColumnIndex("Class Item Masterworked");
        // First find the exotic.
        List<Armor> exotic = armorItems.Where(a => a.Name.StartsWith(Exotic)).ToList();
        if (exotic.Count == 0)  // we don't have one!
            return category;
        string exoticType = exotic[0].ItemType;
        List<Armor> helmets = exoticType == "Helmet" ? exotic :
            armorItems.Where(a => a.ItemType == "Helmet" && a.TierType != "Exotic").ToList();
        List<Armor> gauntlets = exoticType == "Gauntlets" ? exotic :
            armorItems.Where(a => a.ItemType == "Gauntlets" && a.TierType != "Exotic").ToList();
        List<Armor> chestArmor = exoticType == "Chest Armor" ? exotic :
            armorItems.Where(a => a.ItemType == "Chest Armor" && a.TierType != "Exotic").ToList();
        List<Armor> legArmor = exoticType == "Leg Armor" ? exotic :
            armorItems.Where(a => a.ItemType == "Leg Armor" && a.TierType != "Exotic").ToList();
        List<Armor> classItems = exoticType == "Class Item" ? exotic : Armor.LegendaryClassItems;
        foreach (Armor head in helmets)
            if (head.ClassType == ClassType)
                foreach (Armor arm in gauntlets)
                    if (arm.ClassType == ClassType)
                        foreach (Armor chest in chestArmor)
                            if (chest.ClassType == ClassType)
                                foreach (Armor leg in legArmor)
                                    if (leg.ClassType == ClassType)
                                        foreach (Armor classItem in classItems)
                                            if (classItem.ClassType == ClassType)
                                            {
                                                object[] row = Calculate(category, head, arm, chest, leg,
                                                    classItem, exoticType, _minimumUsage);
                                                if (row != null)
                                                    category.Rows.Add(row);
                                            }
        return category;
    }
    private object[] Calculate(Category category, Armor head, Armor arm, Armor chest, Armor leg, Armor classItem,
        string exoticType, int minimumUsage)
    {
        // Calculate a result for this combination.
        long mobility = 10 + MobilityMod + head.Mobility + arm.Mobility + chest.Mobility + leg.Mobility + classItem.Mobility;
        long resilience = 10 + ResilienceMod + head.Resilience + arm.Resilience + chest.Resilience + leg.Resilience + classItem.Resilience
            + (exoticType == "Chest Armor" ? 0 : 1);  // Solstice (Rekindled) chest ornament gives +1 Resilience
        long recovery = 10 + RecoveryMod + head.Recovery + arm.Recovery + chest.Recovery + leg.Recovery + classItem.Recovery;
        long discipline = 10 + DisciplineMod + head.Discipline + arm.Discipline + chest.Discipline + leg.Discipline + classItem.Discipline;
        long intellect = 10 + IntellectMod + head.Intellect + arm.Intellect + chest.Intellect + leg.Intellect + classItem.Intellect;
        long strength = 10 + StrengthMod + head.Strength + arm.Strength + chest.Strength + leg.Strength + classItem.Strength;
        // Calculate effective values (divide by 10).
        long mobi = Math.Min(10L, mobility / 10);
        long resi = Math.Min(10L, resilience / 10);
        long reco = Math.Min(10L, recovery / 10);
        long disc = Math.Min(10L, discipline / 10);
        long inte = Math.Min(10L, intellect / 10);
        long stre = Math.Min(10L, strength / 10);
        long usage = (mobi + resi + reco + disc + inte + stre) * 10;
        if (usage < minimumUsage)
            return null;
        // Skip mobility 3+ items unless it's for a hunter or mobility based exotic.
        if (ClassType != "Hunter"
            && !category.Name.Contains("Wings of Sacred Dawn")
            && !category.Name.Contains("Lion Rampant")
            && mobi > 3)
            return null;
        bool artifice = (exoticType == "Helmet" || head.Artifice)
            && (exoticType == "Gauntlets" || arm.Artifice)
            && (exoticType == "Chest Armor" || chest.Artifice)
            && (exoticType == "Leg Armor" || leg.Artifice)
            && (exoticType == "Class Item" || classItem.Artifice);
        long wastage = mobility + resilience + recovery + discipline + intellect + strength - usage;
        long lowestBaseStats = Math.Min(Math.Min(head.BaseStats, arm.BaseStats), Math.Min(chest.BaseStats, leg.BaseStats));
        long mrr = mobi + resi + reco;
        long dis = disc + inte + stre;
        bool masterworked = head.EnergyCapacity == 10L && arm.EnergyCapacity == 10L
            && chest.EnergyCapacity == 10L && leg.EnergyCapacity == 10L;
        bool exoticMasterworked = exoticType == "Helmet" && head.EnergyCapacity == 10L
            || exoticType == "Gauntlets" && arm.EnergyCapacity == 10L
            || exoticType == "Chest Armor" && chest.EnergyCapacity == 10L
            || exoticType == "Leg Armor" && leg.EnergyCapacity == 10L
            || exoticType == "Class Item" && classItem.EnergyCapacity == 10L;
        object[] row = new object[category.ColumnNames.Count];
        row[category.ColumnIndex("DIM Query")] =
            $"id:'{head.ItemInstanceId}' or id:'{arm.ItemInstanceId}' or id:'{chest.ItemInstanceId}' or id:'{leg.ItemInstanceId}'"
            + (classItem.TierType == "Legendary" ? "" : " or id:'{classItem.ItemInstanceId}'");
        row[category.ColumnIndex("Name")] =
            head.Id + "/" + arm.Id + "/" + chest.Id + "/" + leg.Id + "/" + classItem.Id;
        row[category.ColumnIndex("Helmet")] = head.Id;
        row[category.ColumnIndex("Gauntlets")] = arm.Id;
        row[category.ColumnIndex("Chest Armor")] = chest.Id;
        row[category.ColumnIndex("Leg Armor")] = leg.Id;
        row[category.ColumnIndex("Class Item")] = classItem.Id;
        row[category.ColumnIndex("Mobility")] = mobi;
        row[category.ColumnIndex("Resilience")] = resi;
        row[category.ColumnIndex("Recovery")] = reco;
        row[category.ColumnIndex("Discipline")] = disc;
        row[category.ColumnIndex("Intellect")] = inte;
        row[category.ColumnIndex("Strength")] = stre;
        row[category.ColumnIndex("Usage")] = usage;
        row[category.ColumnIndex("Res+Dis")] = resi + disc;
        row[category.ColumnIndex("RR")] = resi + reco;
        row[category.ColumnIndex("RD")] = resi + disc;
        row[category.ColumnIndex("DI")] = disc + inte;
        row[category.ColumnIndex("RRD")] = resi + reco + disc;
        row[category.ColumnIndex("RRS")] = resi + reco + stre;
        row[category.ColumnIndex("MRD")] = resi + mobi + disc;
        row[category.ColumnIndex("RRI")] = resi + reco + inte;
        row[category.ColumnIndex("Artifice")] = artifice;
        row[category.ColumnIndex("Wastage")] = wastage;
        row[category.ColumnIndex("LowestBaseStats")] = lowestBaseStats;
        row[category.ColumnIndex("Mob")] = mobility;
        row[category.ColumnIndex("Res")] = resilience;
        row[category.ColumnIndex("Rec")] = recovery;
        row[category.ColumnIndex("Dis")] = discipline;
        row[category.ColumnIndex("Int")] = intellect;
        row[category.ColumnIndex("Str")] = strength;
        row[category.ColumnIndex("MRR")] = mrr;
        row[category.ColumnIndex("DIS")] = dis;
        row[category.ColumnIndex("Solstice Rekindled")] = exoticType != "Chest Armor" && resilience % 10 == 0;
        row[category.ColumnIndex("Exotic")] = Exotic;
        row[category.ColumnIndex("ExoticType")] = exoticType;
        row[category.ColumnIndex("Masterworked")] = masterworked;
        row[category.ColumnIndex("Exotic Masterworked")] = exoticMasterworked;
        row[category.ColumnIndex("Head Masterworked")] = head.EnergyCapacity == 10L;
        row[category.ColumnIndex("Arms Masterworked")] = arm.EnergyCapacity == 10L;
        row[category.ColumnIndex("Chest Masterworked")] = chest.EnergyCapacity == 10L;
        row[category.ColumnIndex("Leg Masterworked")] = leg.EnergyCapacity == 10L;
        row[category.ColumnIndex("Class Item Masterworked")] = leg.EnergyCapacity == 10L;
        return row;
    }
    private const int _minimumUsage = 320;
}

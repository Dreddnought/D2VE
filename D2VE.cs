//#define TEST_OUTPUT
//#define ARMOR_ONLY
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace D2VE
{
    class D2VE
    {
        private const string OAuthUrl = "https://www.bungie.net/en/OAuth/Authorize?client_id=32695&response_type=code";
        private const string AccessTokenUrl = "https://www.bungie.net/Platform/App/OAuth/token/";
        private const string PlatformUrl = "https://www.bungie.net/Platform/";
        private const string ApplicationRegistrationUrl = "https://www.bungie.net/en/Application";
        private static List<string> _warlockExotics = new List<string>();
        private static List<string> _hunterExotics = new List<string>();
        private static List<string> _titanExotics = new List<string>();
        private static HttpClient _httpClient;
        private static string _jsonMediaType = "application/json";
#if !TEST_OUTPUT
        private static string _formMediaType = "application/x-www-form-urlencoded";
#endif
        private static MediaTypeWithQualityHeaderValue _jsonAcceptHeader
            = new MediaTypeWithQualityHeaderValue(_jsonMediaType);
        private static TimeSpan _timeout = new TimeSpan(0, 1, 0);  // 1 minute
        private static string _apiKey;
        public static ItemCache ItemCache { get; } = new ItemCache();
        public static PlugCache PlugCache { get; } = new PlugCache();
        public static StatCache StatCache { get; } = new StatCache();
        public static SlotCache SlotCache { get; } = new SlotCache();
        public static SeasonCache SeasonCache { get; } = new SeasonCache();
        public static PowerCapCache PowerCapCache { get; } = new PowerCapCache();
        static void Main(string[] args)
        {
            _apiKey = Persister.Load("ApiKey");
            if (string.IsNullOrEmpty(_apiKey))
            {
                Console.WriteLine("ApiKey not defined");
                return;
            }
#if TEST_OUTPUT
            Console.WriteLine("Using cached result");
#else
            // For now we have a manual step to get the code.  Go to OAuthUrl in a browser and copy the returned code
            // from the redirect address (to automate this I would need a proper website).  Once you have the code,
            // pass it on the command line.  We can query for the access token using this code, but only once.  So
            // we're going to persist it in the registry.  The access token is good for one hour.  Once it expires
            // we'll try the passed code again.  This will need to be updated.
            string accessToken = Persister.Load("AccessToken");
            dynamic membershipsForCurrentUser = null;
            try
            {
                // Make a test query using this accessToken.  If it fails we'll need to request a new one.
                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    membershipsForCurrentUser = Request("User/GetMembershipsForCurrentUser/");
                    if (membershipsForCurrentUser == null)
                        accessToken = null;
                    Client.DefaultRequestHeaders.Authorization = null;
                }
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Get the code returned from " + OAuthUrl
                            + " (after the \"code=\") and enter it on the command line");
                        return;
                    }
                    StringContent code = new StringContent("client_id=32695&grant_type=authorization_code&code=" + args[0],
                        Encoding.UTF8, _formMediaType);
                    HttpResponseMessage accessResponseMessage = Client.PostAsync(AccessTokenUrl, code).Result;
                    if (accessResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("Get the code returned from " + OAuthUrl
                            + " (after the \"code=\") and enter it on the command line");
                        return;
                    }
                    string accessResponseString = accessResponseMessage.Content.ReadAsStringAsync().Result;
                    dynamic accessResponse = JsonConvert.DeserializeObject(accessResponseString);
                    accessToken = accessResponse["access_token"]?.Value;
                    Persister.Save("AccessToken", accessToken);
                }
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to get access token: " + x.Message);
                return;
            }
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
#endif
            ItemCache.Load();
            PlugCache.Load();
            StatCache.Load();
            SlotCache.Load();
            SeasonCache.Load();
            PowerCapCache.Load();
            List<Membership> memberships = new List<Membership>();
#if TEST_OUTPUT
            memberships.Add(new Membership("DrEdd_Nought", "2", "4611686018434882493"));
#else
            // Get membership type and id for the current user.  We may have already got this earlier when testing the
            // cached access token.
            if (membershipsForCurrentUser == null)
                membershipsForCurrentUser = Request("User/GetMembershipsForCurrentUser/");
            dynamic destinyMemberships = membershipsForCurrentUser["destinyMemberships"];
            foreach (var destinyMembership in destinyMemberships)
                memberships.Add(new Membership(
                    destinyMembership.displayName.Value,
                    destinyMembership.membershipType.Value.ToString(),
                    destinyMembership.membershipId.Value));
#endif
            foreach (Membership membership in memberships)
            {
                List<ItemInstance> instances = null;
#if TEST_OUTPUT
                instances = Load(membership.DisplayName);
#else
                instances = GetItemInstances(membership);
#endif
                if (instances == null)
                    continue;
                // Convert the data to spreadsheet form.
                Dictionary<string, Category> data = new Dictionary<string, Category>();
                WeaponsAndArmor(instances, data);
                ArmorCalculations(instances, data);
                // Find all non-masterworked Warlock armor that isn't in the PVE, PVP or 320 sheets.
                List<string> candidates = new List<string>();
                foreach (ItemInstance i in instances)
                    if (i.ClassType == "Warlock" && i.ItemCategory == "Armor" && i.EnergyCapacity != 10L)
                    {
                        long mobility = i.Stats["1"];
                        long resilience = i.Stats["2"];
                        long recovery = i.Stats["3"];
                        long discipline = i.Stats["4"];
                        long intellect = i.Stats["5"];
                        long strength = i.Stats["6"];
                        long baseStats = mobility + resilience + recovery + discipline + intellect + strength;
                        long mrr = mobility + resilience + recovery;
                        long dis = discipline + intellect + strength;
                        long ri = recovery + intellect;
                        string id = i.Name + " " + baseStats.ToString() + "(" + ri.ToString() + ")="
                            + mrr.ToString() + "+" + dis.ToString()
                            + " (" + mobility.ToString() + "-" + resilience.ToString() + "-" + recovery.ToString()
                            + "-" + discipline.ToString() + "-" + intellect.ToString() + "-" + strength.ToString() + ")";
                        if (!candidates.Contains(id))
                            candidates.Add(id);
                    }
                string[] sheets = new string[] { "320", "PVE", "PVP" };
                foreach (string sheet in sheets)
                {
                    Category category = data[sheet];
                    foreach (object[] row in category.Rows)
                    {
                        candidates.Remove((string)row[category.ColumnIndex("Helmet")]);
                        candidates.Remove((string)row[category.ColumnIndex("Gauntlets")]);
                        candidates.Remove((string)row[category.ColumnIndex("Chest Armor")]);
                        candidates.Remove((string)row[category.ColumnIndex("Leg Armor")]);
                    }
                }
                foreach (string unwantedArmor in candidates)
                    Console.WriteLine("Delete " + unwantedArmor);
                // Create an output spreadsheet.     
                OutputContext outputContext = new OutputContext()
                {
                    SpreadsheetName = membership.DisplayName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    Folder = @"C:\Temp\Destiny"
                };
                IOutput output = new CsvOutput();
                output.Create(outputContext, data);
            }
            ItemCache.Save();
            PlugCache.Save();
            StatCache.Save();
            SlotCache.Save();
            SeasonCache.Save();
            PowerCapCache.Save();
            //Console.Read();
        }
        private static void WeaponsAndArmor(List<ItemInstance> instances, Dictionary<string, Category> data)
        {
            // First pass.  Determine all column names except stats which we want on the end.
            foreach (ItemInstance itemInstance in instances)
            {
                Category category = null;
                if (!data.TryGetValue(itemInstance.ItemCategory, out category))
                {
                    data[itemInstance.ItemCategory] = category = new Category(itemInstance.ItemCategory);
                    category.ColumnIndex("Name");
                    //category.ColumnIndex("ItemInstanceId");
                    category.ColumnIndex("ItemType");
                    category.ColumnIndex("Power");
                    category.ColumnIndex("PowerCap");
                    category.ColumnIndex("TierType");
                    category.ColumnIndex("Slot");
                    category.ColumnIndex("ClassType");
                    category.ColumnIndex("EnergyType");
                    if (itemInstance.ItemCategory == "Armor")
                    {
                        category.ColumnIndex("Masterworked");
                        category.ColumnIndex("Artifice");
                    }
                    else
                    {
                        category.ColumnIndex("Intrinsic");
                        category.ColumnIndex("Barrel/Sight");
                        category.ColumnIndex("Magazine/Battery");
                        category.ColumnIndex("Perk1");
                        category.ColumnIndex("Perk2");
                        category.ColumnIndex("Masterwork");
                        category.ColumnIndex("Impact x RPM");
                    }
                }
                if (itemInstance.ItemCategory == "Weapon")  // In case we missed any
                    foreach (string plugTypeName in itemInstance.Plugs.Keys)
                        category.ColumnIndex(plugTypeName);
            }
            // Second pass.  Add all column names for stats.
            foreach (ItemInstance itemInstance in instances)
            {
                Category category = data[itemInstance.ItemCategory];
                foreach (string statName in itemInstance.Stats.Keys)
                    category.ColumnIndex(ConvertValue.StatSortedName(statName));
            }
            // Third pass.  Add rows.
            foreach (ItemInstance itemInstance in instances)
            {
                Category category = data[itemInstance.ItemCategory];
                object[] row = new object[category.ColumnNames.Count];
                row[category.ColumnIndex("Name")] = itemInstance.Name;
                //row[category.ColumnIndex("ItemInstanceId")] = itemInstance.ItemInstanceId;
                row[category.ColumnIndex("ItemType")] = itemInstance.ItemType;
                row[category.ColumnIndex("Power")] = itemInstance.Power;
                row[category.ColumnIndex("PowerCap")] = ConvertValue.PowerCap(itemInstance.PowerCap);  // Replace 999990 with ""
                row[category.ColumnIndex("TierType")] = itemInstance.TierType;
                row[category.ColumnIndex("Slot")] = itemInstance.Slot;
                row[category.ColumnIndex("EnergyType")] = itemInstance.EnergyType;
                row[category.ColumnIndex("ClassType")] = itemInstance.ClassType;
                if (itemInstance.ItemCategory == "Armor")
                {
                    row[category.ColumnIndex("Masterworked")] = itemInstance.EnergyCapacity == 10L;
                    row[category.ColumnIndex("Artifice")] = itemInstance.Artifice;
                }
                else
                {
                    row[category.ColumnIndex("Masterwork")] = itemInstance.Masterwork;
                    if (itemInstance.Stats.ContainsKey("Impact") && itemInstance.Stats.ContainsKey("Rounds Per Minute"))
                        row[category.ColumnIndex("Impact x RPM")] = itemInstance.Stats["Impact"] * itemInstance.Stats["Rounds Per Minute"];
                }
                if (itemInstance.ItemCategory == "Weapon")
                    foreach (var kvp in itemInstance.Plugs)
                        row[category.ColumnIndex(kvp.Key)] = kvp.Value;
                foreach (var kvp in itemInstance.Stats)
                {
                    int index = category.ColumnIndex(ConvertValue.StatSortedName(kvp.Key));
                    row[index] = kvp.Value;
                }
                category.Rows.Add(row);
            }
        }
        private static void ArmorCalculations(List<ItemInstance> instances, Dictionary<string, Category> data)
        {
            // Armor calculation.
            List<Armor> armor = instances.Where(i => i.ItemCategory == "Armor" && i.TierType != "Rare" &&
                !i.ItemType.StartsWith("Warlock") && !i.ItemType.StartsWith("Hunter") && !i.ItemType.StartsWith("Titan"))
                .Select(i => new Armor(i.Name, i.ClassType, i.TierType, i.ItemType, i.EnergyType,
                    i.Artifice != "FALSE", i.EnergyCapacity,
                    i.PowerCap, i.Stats["1"], i.Stats["2"], i.Stats["3"], i.Stats["4"], i.Stats["5"], i.Stats["6"]))
                .ToList();
            foreach (string exotic in _warlockExotics)
                AddArmorCalculation(data, armor, new ArmorCalculator("Warlock", exotic));
            foreach (string exotic in _hunterExotics)
                AddArmorCalculation(data, armor, new ArmorCalculator("Hunter", exotic));
            foreach (string exotic in _titanExotics)
                AddArmorCalculation(data, armor, new ArmorCalculator("Titan", exotic));
        }
        private static void AddArmorCalculation(Dictionary<string, Category> data, List<Armor> armor,
            ArmorCalculator armorCalculator)
        {
            try
            {
                Category category = armorCalculator.Calculate(armor);
                data[category.Name] = category;
                // Make file of all 320 armor.
                if (!data.ContainsKey("320"))
                {
                    data["320"] = new Category("320");
                    data["320"].ColumnNames.AddRange(category.ColumnNames);
                }
                data["320"].Rows.AddRange(category.Rows.Where(row =>
                    (long)row[category.ColumnIndex("Usage")] == 320L));
                // Make file of all fully masterworked sets.
                if (!data.ContainsKey("Masterworked"))
                {
                    data["Masterworked"] = new Category("Masterworked");
                    data["Masterworked"].ColumnNames.AddRange(category.ColumnNames);
                }
                data["Masterworked"].Rows.AddRange(category.Rows.Where(row =>
                    (bool)row[category.ColumnIndex("Masterworked")] == true));
                // Make file of all perfect PVE 310s  3 4 9 7 6 2
                if (!data.ContainsKey("PVE"))
                {
                    data["PVE"] = new Category("PVE");
                    data["PVE"].ColumnNames.AddRange(category.ColumnNames);
                }
                data["PVE"].Rows.AddRange(category.Rows.Where(row =>
                    //row[category.ColumnIndex("Artifice")] == "TRUE" &&
                    (long)row[category.ColumnIndex("Usage")] >= 300L &&
                    (long)row[category.ColumnIndex("Mobility")] >= 1L &&
                    (long)row[category.ColumnIndex("Resilience")] >= 7L &&
                    (long)row[category.ColumnIndex("Recovery")] >= 6L &&
                    (long)row[category.ColumnIndex("Discipline")] >= 7L &&
                    (long)row[category.ColumnIndex("Intellect")] >= 1L &&
                    (long)row[category.ColumnIndex("Strength")] >= 1L &&
                    (long)row[category.ColumnIndex("RRD")] >= 22L));
                // Make file of all perfect PVP 310s  3 4 9 2 9 4
                if (!data.ContainsKey("PVP"))
                {
                    data["PVP"] = new Category("PVP");
                    data["PVP"].ColumnNames.AddRange(category.ColumnNames);
                }
                data["PVP"].Rows.AddRange(category.Rows.Where(row =>
                    (long)row[category.ColumnIndex("Usage")] >= 310L &&
                    (long)row[category.ColumnIndex("Mobility")] >= 3L &&
                    (long)row[category.ColumnIndex("Resilience")] >= 4L &&
                    (long)row[category.ColumnIndex("Recovery")] >= 9L &&
                    (long)row[category.ColumnIndex("Discipline")] >= 2L &&
                    (long)row[category.ColumnIndex("Intellect")] >= 8L &&
                    (long)row[category.ColumnIndex("Strength")] >= 3L));
            }
            catch (Exception x)
            {
                Console.WriteLine("Exception in armor calculation: " + x.ToString());
            }
        }
        private static List<ItemInstance> GetItemInstances(Membership membership)
        {
            List<ItemInstance> instances = new List<ItemInstance>();
            // Get all inventories in their vault and characters.
            dynamic inventories =
                Request("Destiny2/" + membership.Type + "/Profile/" + membership.Id + "/?components=102,201,205");
            if (inventories == null)
                return null;
            // Items equipped on the character.
            foreach (var characters in inventories["characterEquipment"]["data"])
                foreach (var character in characters)
                    foreach (var item in character["items"])
                        ProcessItem(membership, instances, item);
            // Items on the character but not equipped.  Includes postmaster items.
            foreach (var characters in inventories["characterInventories"]["data"])
                foreach (var character in characters)
                    foreach (var item in character["items"])
                        ProcessItem(membership, instances, item);
            // Items in the vault.
            foreach (var item in inventories["profileInventory"]["data"]["items"])
                ProcessItem(membership, instances, item);
            // Add unclaimed season pass armor.
            if (membership.DisplayName == "DrEdd_Nought")
            {
            }
            Save(membership.DisplayName, instances);  // Save result for dev purposes (testing output)
            return instances;
        }
        // Assume the item can be found by name because we already have one.
        // TODO: Do this properly.  In particular we need the real itemInstanceId.
        private static void AddSeasonPassArmor(List<ItemInstance> instances, string name, string energyType,
            long hash, SortedDictionary<string, long> stats)
        {
            ItemInfo itemInfo = ItemCache.GetItemInfo(name, hash);
            if (itemInfo == null)
            {
                Console.WriteLine("Could not find " + name);
                return;
            }
            ItemInstance itemInstance = new ItemInstance(null, itemInfo, 0, 1410, energyType, 0, "", stats,
                new SortedDictionary<string, Plug>());
            instances.Add(itemInstance);
            Console.WriteLine(itemInstance.ToString());
        }
        private static void ProcessItem(Membership membership, List<ItemInstance> instances, dynamic item)
        {
            try
            {
                int traitNumber = 0;
                long itemHash = item.itemHash.Value;
                // Get the definition of the item.
                ItemInfo itemInfo = ItemCache.GetItemInfo(itemHash);
                if (itemInfo == null)  // Not a weapon or armor
                    return;
                SortedDictionary<string, long> stats = new SortedDictionary<string, long>(itemInfo.Stats);
                SortedDictionary<string, Plug> plugs = new SortedDictionary<string, Plug>();
                // Now get the specific stats and perks for the item.
                string itemInstanceId = item.itemInstanceId.Value;
                string request = "Destiny2/" + membership.Type + "/Profile/" + membership.Id + "/Item/" + itemInstanceId
                    + "?components=300,304,305,306,308,309,310";
                dynamic instance = Request(request);
                long power = instance.instance.data.primaryStat.value.Value;
                long powerCap = itemInfo.PowerCaps[itemInfo.PowerCaps.Count <= (int)item.versionNumber.Value ? 0
                    : (int)item.versionNumber.Value];
                string masterwork = "";
                string energyType = itemInfo.EnergyType;
                long energyCapacity = 0L;
                if (itemInfo.ItemCategory == "Armor")  // Armor, energyType is at instance level
                {
                    if (instance.instance.data.energy == null)  // Armor 1.0 ignore
                        return;
                    if (itemInfo.TierType == "Exotic")
                    {
                        if (itemInfo.ClassType == "Warlock" && !_warlockExotics.Contains(itemInfo.Name))
                            _warlockExotics.Add(itemInfo.Name);
                        else if (itemInfo.ClassType == "Hunter" && !_hunterExotics.Contains(itemInfo.Name))
                            _hunterExotics.Add(itemInfo.Name);
                        else if (itemInfo.ClassType == "Titan" && !_titanExotics.Contains(itemInfo.Name))
                            _titanExotics.Add(itemInfo.Name);
                    }
                    energyType = ConvertValue.EnergyType(instance.instance.data.energy.energyType.Value);
                    energyCapacity = instance.instance.data.energy.energyCapacity.Value;
                }
#if ARMOR_ONLY
                else
                    return;
#endif
                foreach (var stat in instance["stats"]["data"]["stats"])
                {
                    long statHash = stat.Value["statHash"].Value;
                    long value = stat.Value["value"].Value;
                    string statName = StatCache.GetStatName(statHash);
                    stats[statName] = value;  // May override item level stats
                }
                dynamic socketData = instance["sockets"]["data"];
                if (socketData != null)
                    foreach (var socketInstance in socketData["sockets"])
                    {
                        long plugHash = socketInstance.plugHash?.Value ?? 0L;
                        if (plugHash == 0L)  // No info on it so can ignore
                            continue;
                        if (!socketInstance.isEnabled.Value)  // Not enabled
                            continue;
                        Plug plug = PlugCache.GetPlug(plugHash);
                        if (plug == null)
                            continue;
                        if (itemInfo.ItemCategory == "Armor")  // Remove stat values from plugs
                        {
                            foreach (var stat in plug.Stats)
                            {
                                long value;
                                if (stats.TryGetValue(stat.Key, out value))
                                    if (value == 0 && stat.Value < 0)  // E.g., Protective Light (plugHash 3523075120L)
                                    {
                                        if (itemInfo.ItemType == "Warlock Bond"
                                            || itemInfo.ItemType == "Hunter Cloak"
                                            || itemInfo.ItemType == "Titan Mark")
                                            stats[stat.Key] = 2;
                                        else if (!(stat.Key == "6" && 
                                            (itemInstanceId == "6917529197610536488"
                                            || itemInstanceId == "6917529396441144390"
                                            || itemInstanceId == "6917529186941930616"
                                            || itemInstanceId == "6917529209922408391"
                                            || itemInstanceId == "6917529369993566692"
                                            || itemInstanceId == "6917529196627182734"
                                            || itemInstanceId == "6917529200048653470"
                                            || itemInstanceId == "6917529418067559390"
                                            || itemInstanceId == "6917529330988751513")))
                                        {
                                            Console.WriteLine("XXXX " + itemInfo.Name + " " + itemInstanceId + " "
                                                + ConvertValue.StatSortedName(stat.Key) + " " + stats["1"] + " "
                                                + stats["2"] + " " + stats["3"] + " " + stats["4"] + " "
                                                + stats["5"] + " " + stats["6"]);
                                            stats[stat.Key] = 2;  // Can't get original value so assume worst case (2)
                                        }
                                    }
                                    else
                                        stats[stat.Key] = value - stat.Value;
                            }
                            // Fix some specific armor pieces where Protective Light screws us up.
                            if (itemInstanceId == "6917529197610536488")
                                stats["6"] = 6;
                            else if (itemInstanceId == "6917529396441144390"
                                || itemInstanceId == "6917529186941930616")
                                stats["6"] = 7;
                            else if (itemInstanceId == "6917529209922408391"
                                || itemInstanceId == "6917529369993566692"
                                || itemInstanceId == "6917529196627182734"
                                || itemInstanceId == "6917529200048653470"
                                || itemInstanceId == "6917529418067559390"            
                                || itemInstanceId == "6917529330988751513")
                                stats["6"] = 2;
                            stats["MRR"] = stats["1"] + stats["2"] + stats["3"];
                            stats["DIS"] = stats["4"] + stats["5"] + stats["6"];
                            stats["RD"] = stats["3"] + stats["4"];
                            stats["Total"] = stats["MRR"] + stats["DIS"];
                        }
                        else
                        {
                            foreach (var stat in plug.Stats)  // We'll leave equipped plugs except we want to max out the masterwork
                                if (plug.Name == "Masterwork")  // Full stats are already applied so we just need the name
                                {
                                    if (masterwork == "")  // Some have additional but usually make them worse so just take the first
                                        masterwork = stat.Key;
                                }
                                else if (plug.Name.StartsWith("Tier "))  // Stat value is one more than has actually been applied
                                {
                                    if (masterwork == "")  // Some have additional but usually make them worse so just take the first
                                        masterwork = stat.Key;
                                    // These are awkward because the mapping to milliseconds is not linear.  For now I'm going to ignore
                                    // it.
                                    if (stat.Key == "Charge Time" || stat.Key == "Draw Time")
                                        break;
                                    long value;
                                    if (stats.TryGetValue(stat.Key, out value))
                                        if (stat.Value > 0)
                                            stats[stat.Key] = value - stat.Value + 10;
                                        else
                                            stats[stat.Key] = value - stat.Value - 11;
                                }
                            string plugType = plug.PlugType;
                            if (string.IsNullOrWhiteSpace(plugType) || plugType == "Weapon Mod" || plugType == "Memento")
                                continue;
                            if (plugType == "Trait" || plugType == "Enhanced Trait")
                                plugType = "Perk" + (++traitNumber).ToString();
                            if (plugType == "Enhanced Intrinsic")
                                plugType = "Intrinsic";
                            if (plugType == "Haft")
                                plugType = "Barrel/Sight";
                            // Cerberus has three barrel plugs.  Ikelos weapons also have more than one barrel plug (seems like a bug).
                            // We'll just ignore the extra barrels because they are not particularly interesting.
                            System.Diagnostics.Debug.Assert(plugType == "Barrel/Sight" || !plugs.ContainsKey(plugType));
                            if (!plugs.ContainsKey(plugType))
                                plugs[plugType] = plug;
                        }
                    }
                ItemInstance itemInstance =
                    new ItemInstance(itemInstanceId, itemInfo, power, powerCap, energyType, energyCapacity,
                        masterwork, stats, plugs);
                instances.Add(itemInstance);
                Console.WriteLine(itemInstance.ToString());
            }
            catch (Exception x)
            {
                Console.WriteLine("Error processing item : " + x.Message);
            }
        }
        public static dynamic Request(string path)
        {
            // Note: Bungie's API sometimes requires the path to end in / or it doesn't work.  If there are parameters, there
            // sometimes needs to be a / before the ?.  But it is not consistent so I will not try to deal with it.  The calls
            // will need to pass the correct string.  If it works in Postman but not here then it's probably due to this.
            try
            {
                Console.WriteLine(PlatformUrl + path);
                HttpResponseMessage response = Client.GetAsync(PlatformUrl + path).Result;
                string content = response.Content.ReadAsStringAsync().Result;
                dynamic item = JsonConvert.DeserializeObject(content);
                return item["Response"];
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed request for " + path + ": " + x.Message);
                return null;
            }
        }
        private static HttpClient Client
        {
            get
            {
                if (_httpClient == null)
                {
                    HttpClientHandler httpClientHandler = new HttpClientHandler()
                    {
                        AllowAutoRedirect = true,
                        AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
                    };
                    _httpClient = new HttpClient(httpClientHandler) { Timeout = _timeout };
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_jsonMediaType));
                    _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
                    _httpClient.DefaultRequestHeaders.ConnectionClose = false;
                }
                return _httpClient;
            }
        }
        private static void Save(string membershipDisplayName, List<ItemInstance> items)
        {
            try
            {
                string result = JsonConvert.SerializeObject(items, Formatting.Indented);
                Persister.Save("Result-" + membershipDisplayName, result);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to save result: " + x.Message);
            }
        }
        private static List<ItemInstance> Load(string membershipDisplayName)
        {
            try
            {
                string result = Persister.Load("Result-" + membershipDisplayName);
                if (!string.IsNullOrWhiteSpace(result))
                    return JsonConvert.DeserializeObject<List<ItemInstance>>(result);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to load result: " + x.Message);
            }
            return null;
        }
    }
}

/*
TODO
Option to exclude armor pieces from consideration (if they are designated for PVP say).
Add unclaimed season pass armor pieces.
 */

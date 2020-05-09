using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace D2VE
{
    public class SocketTypeCache
    {
        private bool _dirty;
        public SocketTypeCache() { }
        private const string SeasonPrefix = "enhancements.season_";
        public string GetSocketTypeName(long socketTypeHash)
        {
            string name;
            if (!_cache.TryGetValue(socketTypeHash, out name))  // Look it up
            {
                StringBuilder season = new StringBuilder(128);
                dynamic def = D2VE.Request("Destiny2/Manifest/DestinySocketTypeDefinition/" + socketTypeHash.ToString());
                // We are only interested in determining the season of a piece of armor given the socket type of the last armor
                // mod socket.  The plugWhitelist for the socket type lists the seasons whose mods can be applied.  For exotics
                // there is no seasonal mod slot so this will be some other socket type.
                foreach (dynamic category in def.plugWhitelist)
                    if (category.categoryIdentifier.Value.StartsWith(SeasonPrefix))
                        season.Append(ConvertValue.Season(category.categoryIdentifier.Value.Substring(SeasonPrefix.Length)) + "/");
                if (season.Length > 0)
                    season.Remove(season.Length - 1, 1);  // Remove trailing slash   
                name = season.ToString();
                int firstSlash = name.IndexOf('/');
                if (firstSlash != -1)
                {
                    int lastSlash = name.LastIndexOf('/');
                    if (firstSlash == lastSlash)  // Outlaw/Forge => Outlaw
                        name = name.Substring(0, firstSlash);
                    else  // Three seasons mods can be applied.  Take the middle one.
                        name = name.Substring(firstSlash + 1, lastSlash - firstSlash - 1);
                }
                _cache[socketTypeHash] = name;
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
                Persister.Save("SocketTypeCache", cache);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to save socketType cache: " + x.Message);
            }
        }
        public void Load()
        {
            try
            {
                string cache = Persister.Load("SocketTypeCache");
                if (!string.IsNullOrWhiteSpace(cache))
                    _cache = JsonConvert.DeserializeObject<Dictionary<long, string>>(cache);
            }
            catch (Exception x)
            {
                Console.WriteLine("Failed to load socketType cache: " + x.Message);
            }
            _dirty = false;
        }
        private Dictionary<long, string> _cache = new Dictionary<long, string>();
    }
}

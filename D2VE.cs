using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace D2VE
{
    class D2VE
    {
        private const string OAuthUrl =
            "https://www.bungie.net/en/OAuth/Authorize?client_id=32695&response_type=code";
        private const string AccessTokenUrl = "https://www.bungie.net/Platform/App/OAuth/token/";
        private const string PlatformUrl = "https://www.bungie.net/Platform/";
        private const string ApplicationRegistrationUrl = "https://www.bungie.net/en/Application";
        private static HttpClient _httpClient;
        private static string _jsonMediaType = "application/json";
        private static string _formMediaType = "application/x-www-form-urlencoded";
        private static MediaTypeWithQualityHeaderValue _jsonAcceptHeader
            = new MediaTypeWithQualityHeaderValue(_jsonMediaType);
        private static TimeSpan _timeout = new TimeSpan(0, 1, 0);  // 1 minute
        private static StatCache _statCache = new StatCache();
        private static ItemCache _itemCache = new ItemCache();
        private static string _apiKey;
        static void Main(string[] args)
        {
            _apiKey = Persister.Load("ApiKey");
            if (string.IsNullOrEmpty(_apiKey))
            {
                Console.WriteLine("ApiKey not defined");
                return;
            }
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
            _itemCache.Load();
            // Get membership type and id for the current user.  We may have already got this earlier when testing the
            // cached access token.
            if (membershipsForCurrentUser == null)
                membershipsForCurrentUser = Request("User/GetMembershipsForCurrentUser/");
            dynamic destinyMemberships = membershipsForCurrentUser["destinyMemberships"];
            foreach (var destinyMembership in destinyMemberships)
            {
                Membership membership = new Membership(
                    destinyMembership["displayName"].Value,
                    destinyMembership["membershipType"].Value.ToString(),
                    destinyMembership["membershipId"].Value);
                // Get all bucket slots in their vault and character.


                dynamic inventories =
                    Request("Destiny2/" + membership.Type + "/Profile/" + membership.Id + "/?components=102,201,205");
                // Items equipped on the character.
                foreach (var characters in inventories["characterEquipment"]["data"])
                    foreach (var character in characters)
                        foreach (var item in character["items"])
                            ProcessItem(membership, item);
                // Items on the character but not equipped.  Includes postmaster items.
                foreach (var characters in inventories["characterInventories"]["data"])
                    foreach (var character in characters)
                        foreach (var item in character["items"])
                            ProcessItem(membership, item);
                // Items in the vault.
                foreach (var item in inventories["profileInventory"]["data"]["items"])
                    ProcessItem(membership, item);

                _itemCache.Save();
                // Create an output spreadsheet.

            }
            Console.ReadKey();
        }
        private static void ProcessItem(Membership membership, dynamic item)
        {
            try
            {
                string itemHash = item.itemHash.Value.ToString();
                // Get the definition of the item.
                ItemInfo itemInfo = _itemCache.GetItemInfo(itemHash);
                if (itemInfo == null)  // Not a weapon or armor
                    return;

                string itemInstanceId = item.itemInstanceId.Value;
                //dynamic instance = Request("Destiny2/" + membership.Type + "/Profile/" + membership.Id + "/Item/"
                //      + itemInstanceId + "?components=302,304");

                Console.WriteLine(itemInfo.ToString() + " " + itemHash + " " + itemInstanceId);
            }
            catch (Exception x)
            {
                Console.WriteLine("Error processing item : " + x.Message);
            }
        }
        public static dynamic Request(string path)
        {
            // Note; Bungie's API sometimes requires the path to end in / or it doesn't work.  If there are parameters, there
            // sometimes needs to be a / before the ?.  But it is not consistent so I will not try to deal with it.  The calls
            // will need to pass the correct string.  If it works in Postman but not here then it's probably due to this.
            try
            {
                //Console.WriteLine(PlatformUrl + path);
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
    }
}

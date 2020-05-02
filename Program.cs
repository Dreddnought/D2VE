using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using RestSharp;

namespace D2VE
{
    class Program
    {
        private const string ApiKey = "dadd80e0b85c4683a2ad68ebd4822678";
        private const string OAuthUrl =
            "https://www.bungie.net/en/OAuth/Authorize?client_id=32695&response_type=code";
        private const string AccessTokenUrl = "https://www.bungie.net/Platform/App/OAuth/token/";
        private static HttpClient _httpClient;
        private static string _jsonMediaType = "application/json";
        private static string _formMediaType = "application/x-www-form-urlencoded";
        private static MediaTypeWithQualityHeaderValue _jsonAcceptHeader
            = new MediaTypeWithQualityHeaderValue(_jsonMediaType);
        private static TimeSpan _timeout = new TimeSpan(0, 1, 0);  // 1 minute
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Get the code returned from " + OAuthUrl
                    + " (after the \"code=\") and enter it on the command line");
                return;
            }
            StringContent code = new StringContent("client_id=32695&grant_type=authorization_code&code=" + args[0],
                Encoding.UTF8, _formMediaType);

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 |
            // SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;


            Client.DefaultRequestHeaders.Add("X-API-Key", ApiKey);
            HttpResponseMessage accessResponseMessage = Client.PostAsync(AccessTokenUrl, code).Result;
            if (accessResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Get the code returned from " + OAuthUrl
                    + " (after the \"code=\") and enter it on the command line");
               // return;
            }
            string accessResponseString = accessResponseMessage.Content.ReadAsStringAsync().Result;
            dynamic accessResponse = JsonConvert.DeserializeObject(accessResponseString);
            string accessToken = accessResponse["access_token"]?.Value;

            accessToken = @"COuPAhKGAgAg9i5UgA5ETpb2rXISD7qEC2I6cFosPkRgmgZCX3jk6DngAAAAzkKjUHAzATtJPet47Ii3CgrZkXhwZ0DShU0UGoy+yTN9l0OXxqRk2u53tVTlXVuy7g+SabGVLbJpzvhui1wUUfKlCS1otgEASW+lDOFU62STpcg8aNhVYEUAXJFsz/D52OIRTIpRE+nXLorIFRCSsYaT2ot3pHG51s3wU4vvRm2XGFvgg25UsDicOOcJDsAYTMWUkzIJH4bpuUEwxJGSIuDP8+BIBmKwoyxEBClFINWiuwFUhKWeI8vvalxVg1SQHm/A7J66kXd6p10RJPIH0BMg4k38vHwjKdqiu9ZhKVI=";

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            ////  Client.DefaultRequestHeaders.Add("X-CSRF", "bungled=5303931530811214944; Path=/; Domain=www.bungie.net; Expires=Tue, 02 May 2023 14:32:08 GMT;");
            HttpResponseMessage response2 =
                Client.GetAsync("https://www.bungie.net/Platform/User/GetMembershipsForCurrentUser/").Result;

            string content2 = response2.Content.ReadAsStringAsync().Result;
            dynamic item = Newtonsoft.Json.JsonConvert.DeserializeObject(content2);


            //var client = new RestClient("https://www.bungie.net/Platform/User/GetCurrentBungieNetUser/");
            //client.Timeout = -1;
            //var request = new RestRequest(Method.GET);
            //request.AddHeader("X-API-Key", "dadd80e0b85c4683a2ad68ebd4822678");
            //request.AddHeader("Authorization", @"Bearer COuPAhKGAgAg9i5UgA5ETpb2rXISD7qEC2I6cFosPkRgmgZCX3jk6DngAAAAzkKjUHAzATtJPet47Ii3CgrZkXhwZ0DShU0UGoy+yTN9l0OXxqRk2u53tVTlXVuy7g+SabGVLbJpzvhui1wUUfKlCS1otgEASW+lDOFU62STpcg8aNhVYEUAXJFsz/D52OIRTIpRE+nXLorIFRCSsYaT2ot3pHG51s3wU4vvRm2XGFvgg25UsDicOOcJDsAYTMWUkzIJH4bpuUEwxJGSIuDP8+BIBmKwoyxEBClFINWiuwFUhKWeI8vvalxVg1SQHm/A7J66kXd6p10RJPIH0BMg4k38vHwjKdqiu9ZhKVI=");
            ////request.AddHeader("Cookie", "__cfduid=dd28de21ee8aebddc8862afdb3f5425091588428665; bungled=6169023077096577652; bungledid=B+HDXCxGTEdNpKp5BlNG4iuQvcHVqO7XCAAA; Q6dA7j3mn3WPBQVV6Vru5CbQXv0q+I9ddZfGro+PognXQwjW=v1YNlRgw@@m5S; bunglefrogblastventcore=1588431350");
            //IRestResponse response = client.Execute(request);
            //Console.WriteLine(response.Content);

            //HttpResponseMessage response = Client.GetAsync("https://www.bungie.net/platform/Destiny/Manifest/InventoryItem/1274330687/").Result;
            //string content = response.Content.ReadAsStringAsync().Result;
            //dynamic item = Newtonsoft.Json.JsonConvert.DeserializeObject(content);

            //Console.WriteLine(item.Response.data.inventoryItem.itemName); //Gjallarhorn

            Console.ReadKey();
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
                        AutomaticDecompression =
                        DecompressionMethods.Deflate | DecompressionMethods.GZip
                    };
                    _httpClient = new HttpClient(httpClientHandler) { Timeout = _timeout };
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    _httpClient.DefaultRequestHeaders.ConnectionClose = false;
                }
                return _httpClient;
            }
        }
    }
}

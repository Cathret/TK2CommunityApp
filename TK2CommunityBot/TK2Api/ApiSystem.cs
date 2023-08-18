using Newtonsoft.Json.Linq;

namespace TK2Bot.API
{
    public static partial class ApiSystem
    {
        private struct AuthInfoData
        {
            public string Token { get; internal init; }
            public TimeSpan ExpirationDate { get; internal init; }
        }
        
        private static readonly string API_LOGIN = Environment.GetEnvironmentVariable("TK2_API_LOGIN")!;
        private static readonly string API_KEY = Environment.GetEnvironmentVariable("TK2_API_KEY")!;
        private static AuthInfoData AuthInfo { get; set; }

        private static readonly HttpClient HTTP_CLIENT = new()
        {
            BaseAddress = new Uri("https://the-karters-community.com/api/"),
        };

        public static async Task TryAuthentificate()
        {
            // Setup Headers which will be used for all requests
            HTTP_CLIENT.DefaultRequestHeaders.Add("x-api-credentials", $"{API_LOGIN}@{API_KEY}");
            
            // Make request to login for 1 day and get json from response
            string contentAsString = await HTTP_CLIENT.GetStringAsync("login");
            
            // We have our json, so we check if it succeeded, and if it did we will set AuthInfo to correct Token and ExpirationDate
            dynamic json = JObject.Parse(contentAsString);
            if (json.status == "success")
            {
                AuthInfo = new AuthInfoData()
                {
                    Token          = json.authorization.token,
                    //ExpirationDate = DateTimeOffset.Parse(json.authorization.expires_at.timestamp.ToString()),
                };

                Console.WriteLine("[INFO] Retrieved Authentication Data");

                HTTP_CLIENT.DefaultRequestHeaders.Clear();
                HTTP_CLIENT.DefaultRequestHeaders.Add("authorization", $"Bearer {AuthInfo.Token}");
                HTTP_CLIENT.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            }
            else
            {
                await Console.Error.WriteLineAsync("[ERROR] Could not retrieve Authentication Data");
            }
        }

        private struct ApiGetResponse
        {
            public bool IsSuccess { get; internal init; }
            public dynamic JsonContent { get; internal init; }
        }
        
        private static async Task<ApiGetResponse> ExecuteGetRequest(string _requestUri)
        {
            string contentAsString = await HTTP_CLIENT.GetStringAsync(_requestUri);
            
            dynamic contentAsJson = JObject.Parse(contentAsString);

            return new ApiGetResponse()
            {
                IsSuccess = contentAsJson.status.ToString().Equals("success"),
                JsonContent = contentAsJson,
            };
        }

        private static string GetLocationFilterOptions(ELocation _location)
        {
            if (_location == ELocation.NO_FILTER)
                return string.Empty;

            string filterName = LocationUtils.GetLocationTypeStr(_location);
            string alias = LocationUtils.GetAlias(_location);
            return $"?{filterName}={alias}";
        }
    }
}
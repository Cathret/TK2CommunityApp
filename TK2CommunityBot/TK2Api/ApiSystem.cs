using System.Net.Http.Headers;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TK2Bot.API
{
    public static class ApiSystem
    {
        private struct AuthInfoData
        {
            public string Token { get; internal set; }
            public TimeSpan ExpirationDate { get; internal set; }
        }
        
        private static readonly string API_LOGIN = Environment.GetEnvironmentVariable("TK2_API_LOGIN")!;
        private static readonly string API_KEY = Environment.GetEnvironmentVariable("TK2_API_KEY")!;
        private static AuthInfoData AuthInfo { get; set; }

        private static readonly HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://the-karters-community.com/api/"),
        };

        public static async Task TryAuthentificate()
        {
            // Setup Headers which will be used for all requests
            httpClient.DefaultRequestHeaders.Add("x-api-credentials", $"{API_LOGIN}@{API_KEY}");
            
            // Make request to login for 1 day and get json from response
            string contentAsString = await httpClient.GetStringAsync("login");
            
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

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {AuthInfo.Token}");
                httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            }
            else
            {
                await Console.Error.WriteLineAsync("[ERROR] Could not retrieve Authentication Data");
            }
        }
        
        private const UInt32 WR_PLACEMENT = 1;

        public static PlayerInfo GetPlayerInfoFromId(ApiPlayerId _playerId)
        {
            // TODO: Request API to get Player's info from its PlayerId
            return new PlayerInfo()
            {
                PlayerName = "Arca",
                ProfileUrl = "https://the-karters-community.com/player/415"
            };
        }
        
        public static async Task<PlayerTrackTime> GetWorldRecordForTrack(ETrackId _trackId)
        {
            string mapSlug = MapTranslator.GetSlugFromTrackId(_trackId);

            string contentAsString = await httpClient.GetStringAsync($"track/{mapSlug}/world-record");
            Console.WriteLine($"HTTP String Content: {contentAsString}");
            
            dynamic contentAsJson = JObject.Parse(contentAsString);

            //dynamic contentAsJson = JsonConvert.DeserializeObject(contentAsString)!;
            PlayerInfo wrHolderInfo = new PlayerInfo()
            {
                PlayerName = contentAsJson.data.record.player.name,
                ProfileUrl = contentAsJson.data.record.player.profile_url,
                AvatarUrl  = contentAsJson.data.record.player.avatar_url, 
            };
            Console.WriteLine($"WrHolder: {wrHolderInfo}");

            TrackInfo trackInfo = new TrackInfo()
            {
                TrackId        = _trackId,
                Slug           = mapSlug,
                MapName        = contentAsJson.data.track.name,
                ImageUrl       = contentAsJson.data.track.image_url,
                LeaderboardUrl = contentAsJson.data.track.leaderboard_url,
            };
            Console.WriteLine($"TrackInfo: {trackInfo}");

            PlayerTrackTime wrTrackTime = new PlayerTrackTime()
            {
                PlayerInfo = wrHolderInfo,
                TrackInfo  = trackInfo,
                RunTime    = TimeSpan.FromMilliseconds(double.Parse(contentAsJson.data.record.score.ToString())),
                Placement  = WR_PLACEMENT,
            };
            Console.WriteLine($"WrTrackTime: {wrTrackTime}");

            return wrTrackTime;
        }
            
        public static TrackTimes GetTimesForTrack(ETrackId _trackId, UInt32 _lowestPlacementIncluded, UInt32 _highestPlacementIncluded)
        {
            // TODO: Request API to get times for TrackID and convert into our type
            PlayerTrackTime[] allTrackTimes = new PlayerTrackTime[]
            {
                new PlayerTrackTime()
                {
                    RunTime     = TimeSpan.FromMilliseconds(65000),
                    Placement   = 1,
                },
                
                new PlayerTrackTime()
                {
                    RunTime     = TimeSpan.FromMilliseconds(95000),
                    Placement   = 2,
                },
                
                new PlayerTrackTime()
                {
                    RunTime     = TimeSpan.FromMilliseconds(100000),
                    Placement   = 3,
                },
            };

            return new TrackTimes() { PlayerTrackTimes = allTrackTimes };
        }
    }
}
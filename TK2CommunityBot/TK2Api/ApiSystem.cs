using System.Diagnostics;
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

        public static async Task<FullPlayerInfo> GetFullPlayerInfoFromName(string _playerName)
        {
            string contentAsString = await httpClient.GetStringAsync($"player?search={_playerName}");

            dynamic contentAsJson = JObject.Parse(contentAsString);
            Console.WriteLine($"PlayerInfo: {contentAsJson.ToString()}");

            PlayerInfo playerInfo = new PlayerInfo()
            {
                PlayerName = contentAsJson.data.player.name,
                ProfileUrl = contentAsJson.data.player.profile_url,
                AvatarUrl  = contentAsJson.data.player.avatar_url,
            };

            PlayerStats playerStats = new PlayerStats()
            {
                PosWorldwide = contentAsJson.data.player.global.positions.worldwide,
                PosContinent = contentAsJson.data.player.global.positions.continent,
                PosCountry   = contentAsJson.data.player.global.positions.country,
                Points       = contentAsJson.data.player.global.points
            };

            ContinentInfo continentInfo = new ContinentInfo()
            {
                Name     = contentAsJson.data.player.continent.name,
                Alias    = contentAsJson.data.player.continent.alpha2,
                ImageUrl = contentAsJson.data.player.continent.image_url,
            };

            CountryInfo countryInfo = new CountryInfo()
            {
                Name     = contentAsJson.data.player.country.name,
                Alias    = contentAsJson.data.player.country.alpha2,
                ImageUrl = contentAsJson.data.player.country.image_url,
            };

            List<PlayerTrackTime> playerTrackTimes = new List<PlayerTrackTime>();
            foreach (dynamic oneRecord in contentAsJson.data.records)
            {
                playerTrackTimes.Add(new PlayerTrackTime()
                    {
                        PlayerInfo = playerInfo,
                        PlayerStats = new PlayerStats()
                        {
                            PosWorldwide = oneRecord.data.position.worldwide,
                            PosContinent = oneRecord.data.position.continent,
                            PosCountry   = oneRecord.data.position.country,
                            Points       = oneRecord.data.points
                        },
                        TrackInfo = new TrackInfo()
                        {
                            MapName        = oneRecord.track.name,
                            ImageUrl       = oneRecord.track.image_url,
                            LeaderboardUrl = oneRecord.track.leaderboard_url
                        },
                        RunTime = TimeSpan.FromMilliseconds(double.Parse(oneRecord.score.ToString())),
                    }
                );
            }

            PlayerRecords playerRecords = new PlayerRecords()
            {
                PlayerTrackTimes = playerTrackTimes.ToArray(),
            };
            
            FullPlayerInfo fullPlayerInfo = new FullPlayerInfo()
            {
                PlayerInfo = playerInfo,
                PlayerStats = playerStats,
                ContinentInfo = continentInfo,
                CountryInfo = countryInfo,
                PlayerRecords = playerRecords,
            };

            return fullPlayerInfo;
        }
        
        public static async Task<PlayerTrackTime> GetWorldRecordForTrack(ETrackId _trackId)
        {
            string mapSlug = MapTranslator.GetSlugFromTrackId(_trackId);

            string contentAsString = await httpClient.GetStringAsync($"track/{mapSlug}/world-record");
            
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
                MapName        = contentAsJson.data.track.name,
                ImageUrl       = contentAsJson.data.track.image_url,
                LeaderboardUrl = contentAsJson.data.track.leaderboard_url,
            };
            Console.WriteLine($"TrackInfo: {trackInfo}");

            PlayerTrackTime wrTrackTime = new PlayerTrackTime()
            {
                PlayerInfo  = wrHolderInfo,
                TrackInfo   = trackInfo,
                RunTime     = TimeSpan.FromMilliseconds(double.Parse(contentAsJson.data.record.score.ToString())),
                PlayerStats = new PlayerStats(),
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
                },
                
                new PlayerTrackTime()
                {
                    RunTime     = TimeSpan.FromMilliseconds(95000),
                },
                
                new PlayerTrackTime()
                {
                    RunTime     = TimeSpan.FromMilliseconds(100000),
                },
            };

            return new TrackTimes() { PlayerTrackTimes = allTrackTimes };
        }
    }
}
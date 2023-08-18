using Newtonsoft.Json.Linq;

namespace TK2Bot.API
{
    public static class ApiSystem
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
        
        public static async Task<FullPlayerInfo> GetFullPlayerInfoFromName(string _playerName)
        {
            string contentAsString = await HTTP_CLIENT.GetStringAsync($"player?search={_playerName}");

            dynamic contentAsJson = JObject.Parse(contentAsString);

            if (contentAsJson.status == "error")
            {
                return new FullPlayerInfo() { IsValid = false };
            }

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
                            PosWorldwide = oneRecord.data.positions.worldwide,
                            PosContinent = oneRecord.data.positions.continent,
                            PosCountry   = oneRecord.data.positions.country,
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
                PlayerInfo    = playerInfo,
                PlayerStats   = playerStats,
                ContinentInfo = continentInfo,
                CountryInfo   = countryInfo,
                PlayerRecords = playerRecords,
                IsValid       = true,
            };

            return fullPlayerInfo;
        }
        
        public static async Task<PlayerTrackTime> GetWorldRecordForTrack(ETrackId _trackId, ELocation _location)
        {
            string mapSlug = MapTranslator.GetSlugFromTrackId(_trackId);

            string requestUri = $"track/{mapSlug}/world-record{GetLocationFilterOptions(_location)}";

            string contentAsString = await HTTP_CLIENT.GetStringAsync(requestUri);
            
            dynamic contentAsJson = JObject.Parse(contentAsString);

            PlayerInfo wrHolderInfo = new PlayerInfo()
            {
                PlayerName = contentAsJson.data.record.player.name,
                ProfileUrl = contentAsJson.data.record.player.profile_url,
                AvatarUrl  = contentAsJson.data.record.player.avatar_url, 
            };

            TrackInfo trackInfo = new TrackInfo()
            {
                MapName        = contentAsJson.data.track.name,
                ImageUrl       = contentAsJson.data.track.image_url,
                LeaderboardUrl = contentAsJson.data.track.leaderboard_url,
            };

            PlayerTrackTime wrTrackTime = new PlayerTrackTime()
            {
                PlayerInfo  = wrHolderInfo,
                TrackInfo   = trackInfo,
                RunTime     = TimeSpan.FromMilliseconds(double.Parse(contentAsJson.data.record.score.ToString())),
                PlayerStats = new PlayerStats(),
            };

            return wrTrackTime;
        }
            
        public static async Task<Leaderboards> GetTrackLeaderboards(ETrackId _trackId, ELocation _location)
        {
            string mapSlug = MapTranslator.GetSlugFromTrackId(_trackId);

            string requestUri = $"track/{mapSlug}/leaderboard{GetLocationFilterOptions(_location)}";

            string contentAsString = await HTTP_CLIENT.GetStringAsync(requestUri);
            
            dynamic contentAsJson = JObject.Parse(contentAsString);
            
            List<LeaderboardRecord> allLeaderboardRecords = new List<LeaderboardRecord>();
            foreach (dynamic oneRecord in contentAsJson.data.records)
            {
                allLeaderboardRecords.Add(new LeaderboardRecord()
                    {
                        PlayerInfo = new PlayerInfo()
                        {
                            PlayerName = oneRecord.player.name,
                            ProfileUrl = oneRecord.player.profile_url,
                            AvatarUrl  = oneRecord.player.avatar_url
                        },
                        CountryInfo = new CountryInfo()
                        {
                            Name     = oneRecord.player.country.name,
                            Alias    = oneRecord.player.country.alpha2,
                            ImageUrl = oneRecord.player.country.image_url,
                        },
                        ContinentInfo = new ContinentInfo()
                        {
                            Name     = oneRecord.player.continent.name,
                            Alias    = oneRecord.player.continent.alpha2,
                            ImageUrl = oneRecord.player.continent.image_url,
                        },
                        PlayerStats = new PlayerStats()
                        {
                            PosWorldwide = oneRecord.data.positions.worldwide,
                            PosContinent = oneRecord.data.positions.continent,
                            PosCountry   = oneRecord.data.positions.country,
                            Points       = oneRecord.data.points
                        },
                        RunTime  = TimeSpan.FromMilliseconds(double.Parse(oneRecord.score.ToString())),
                    }
                );
            }

            Leaderboards leaderboards = new Leaderboards()
            {
                TrackInfo = new TrackInfo()
                {
                    MapName        = contentAsJson.data.track.name,
                    ImageUrl       = contentAsJson.data.track.image_url,
                    LeaderboardUrl = contentAsJson.data.track.leaderboard_url,
                },
                LeaderboardRecords = allLeaderboardRecords.ToArray(),
            };

            return leaderboards;
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
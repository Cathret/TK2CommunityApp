using Newtonsoft.Json.Linq;

namespace TK2Bot.API
{
    public static partial class ApiSystem
    {
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
    }
}

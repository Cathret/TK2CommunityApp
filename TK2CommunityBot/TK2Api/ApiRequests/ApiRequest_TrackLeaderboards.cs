using Newtonsoft.Json.Linq;

namespace TK2Bot.API
{
    public static partial class ApiSystem
    {
        public static async Task<TrackLeaderboards> GetTrackLeaderboards(ETrackId _trackId, ELocation _location)
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

            TrackLeaderboards trackLeaderboards = new TrackLeaderboards()
            {
                TrackInfo = new TrackInfo()
                {
                    MapName        = contentAsJson.data.track.name,
                    ImageUrl       = contentAsJson.data.track.image_url,
                    LeaderboardUrl = contentAsJson.data.track.leaderboard_url,
                },
                LeaderboardRecords = allLeaderboardRecords.ToArray(),
            };

            return trackLeaderboards;
        }
    }
}
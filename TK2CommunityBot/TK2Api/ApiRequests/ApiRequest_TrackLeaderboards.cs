using Newtonsoft.Json.Linq;

namespace TK2Bot.API
{
    public static partial class ApiSystem
    {
        public static async Task<TrackLeaderboard> GetTrackLeaderboard(ETrackId _trackId, ELocation _location)
        {
            string mapSlug = MapTranslator.GetSlugFromTrackId(_trackId);

            string requestUri = $"track/{mapSlug}/leaderboard{GetLocationFilterOptions(_location)}";

            ApiGetResponse getResponse = await ExecuteGetRequest(requestUri);
            if (getResponse.IsSuccess == false)
            {
                return new TrackLeaderboard()
                {
                    LeaderboardRecords = Array.Empty<TrackLeaderboardEntry>()
                };
            }
            
            dynamic contentAsJson = getResponse.JsonContent;
            
            List<TrackLeaderboardEntry> allLeaderboardRecords = new List<TrackLeaderboardEntry>();
            foreach (dynamic oneRecord in contentAsJson.data.records)
            {
                allLeaderboardRecords.Add(new TrackLeaderboardEntry()
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

            TrackLeaderboard trackLeaderboard = new TrackLeaderboard()
            {
                TrackInfo = new TrackInfo()
                {
                    MapName        = contentAsJson.data.track.name,
                    ImageUrl       = contentAsJson.data.track.image_url,
                    LeaderboardUrl = contentAsJson.data.track.leaderboard_url,
                },
                LeaderboardRecords = allLeaderboardRecords.ToArray(),
            };

            return trackLeaderboard;
        }
    }
}
using Newtonsoft.Json.Linq;

namespace TK2Bot.API
{
    public static partial class ApiSystem
    {
        public static async Task<GlobalLeaderboards> GetGlobalLeaderboards(ELocation _location)
        {
            string requestUri = $"leaderboard{GetLocationFilterOptions(_location)}";

            ApiGetResponse getResponse = await ExecuteGetRequest(requestUri);
            if (getResponse.IsSuccess == false)
            {
                return new GlobalLeaderboards()
                {
                    LeaderboardEntries = Array.Empty<GlobalLeaderboardEntry>()
                };
            }
            
            dynamic contentAsJson = getResponse.JsonContent;
            
            List<GlobalLeaderboardEntry> allLeaderboardEntries = new List<GlobalLeaderboardEntry>();
            foreach (dynamic oneEntry in contentAsJson.data.records)
            {
                allLeaderboardEntries.Add(new GlobalLeaderboardEntry()
                    {
                        PlayerInfo = new PlayerInfo()
                        {
                            PlayerName = oneEntry.player.name,
                            ProfileUrl = oneEntry.player.profile_url,
                            AvatarUrl  = oneEntry.player.avatar_url
                        },
                        CountryInfo = new CountryInfo()
                        {
                            Name     = oneEntry.player.country.name,
                            Alias    = oneEntry.player.country.alpha2,
                            ImageUrl = oneEntry.player.country.image_url,
                        },
                        ContinentInfo = new ContinentInfo()
                        {
                            Name     = oneEntry.player.continent.name,
                            Alias    = oneEntry.player.continent.alpha2,
                            ImageUrl = oneEntry.player.continent.image_url,
                        },
                        PlayerStats = new PlayerStats()
                        {
                            PosWorldwide = oneEntry.data.positions.worldwide,
                            PosContinent = oneEntry.data.positions.continent,
                            PosCountry   = oneEntry.data.positions.country,
                            Points       = oneEntry.data.points
                        },
                    }
                );
            }

            GlobalLeaderboards globalLeaderboards = new GlobalLeaderboards()
            {
                LeaderboardEntries = allLeaderboardEntries.ToArray(),
            };

            return globalLeaderboards;
        }
    }
}
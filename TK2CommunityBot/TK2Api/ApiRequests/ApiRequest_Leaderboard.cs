using Newtonsoft.Json.Linq;

namespace TK2Bot.API
{
    public static partial class ApiSystem
    {
        public static async Task<GlobalLeaderboard> GetGlobalLeaderboard(ELocation _location)
        {
            string requestUri = $"leaderboard{GetLocationFilterOptions(_location)}";

            ApiGetResponse getResponse = await ExecuteGetRequest(requestUri);
            if (getResponse.IsSuccess == false)
            {
                return new GlobalLeaderboard()
                {
                    LeaderboardEntries = Array.Empty<GlobalLeaderboardEntry>()
                };
            }
            
            dynamic contentAsJson = getResponse.JsonContent;
            
            List<GlobalLeaderboardEntry> allLeaderboardEntries = new List<GlobalLeaderboardEntry>();
            foreach (dynamic oneEntry in contentAsJson.data.players)
            {
                allLeaderboardEntries.Add(new GlobalLeaderboardEntry()
                    {
                        PlayerInfo = new PlayerInfo()
                        {
                            PlayerName = oneEntry.name,
                            ProfileUrl = oneEntry.profile_url,
                            AvatarUrl  = oneEntry.avatar_url
                        },
                        CountryInfo = new CountryInfo()
                        {
                            Name     = oneEntry.country.name,
                            Alias    = oneEntry.country.alpha2,
                            ImageUrl = oneEntry.country.image_url,
                        },
                        ContinentInfo = new ContinentInfo()
                        {
                            Name     = oneEntry.continent.name,
                            Alias    = oneEntry.continent.alpha2,
                            ImageUrl = oneEntry.continent.image_url,
                        },
                        PlayerStats = new PlayerStats()
                        {
                            PosWorldwide = oneEntry.global.positions.worldwide,
                            PosContinent = oneEntry.global.positions.continent,
                            PosCountry   = oneEntry.global.positions.country,
                            Points       = oneEntry.global.points
                        },
                    }
                );
            }

            dynamic jsonPagination = contentAsJson.data.pagination;
            PaginationInformation paginationInfo = new PaginationInformation()
            {
                ItemsCount      = jsonPagination.items_count,
                ItemsTotalCount = jsonPagination.items_total_count,
                Page            = jsonPagination.page,
                LastPage        = jsonPagination.last_page
            };

            return new GlobalLeaderboard()
            {
                LeaderboardEntries = allLeaderboardEntries.ToArray(),
                PaginationInfo = paginationInfo
            };;
        }
    }
}
using Newtonsoft.Json.Linq;

namespace TK2Bot.API
{
    public static partial class ApiSystem
    {
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
    }
}

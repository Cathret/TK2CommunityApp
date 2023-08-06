namespace TK2Bot.API
{
    public static class ApiSystem
    {
        private const UInt32 WR_PLACEMENT = 1;

        public static PlayerInfo GetPlayerInfoFromId(ApiPlayerId _playerId)
        {
            // TODO: Request API to get Player's info from its PlayerId
            return new PlayerInfo()
            {
                ApiPlayerId = _playerId,
                KartersId = "415",
                PlayerName = "Arca",
            };
        }
        
        public static PlayerTrackTime GetWorldRecordForTrack(ETrackId _trackId)
        {
            TrackTimes times = GetTimesForTrack(_trackId, WR_PLACEMENT, WR_PLACEMENT);
            PlayerTrackTime wrPlayerTrackTime = times.PlayerTrackTimes[0];
            return wrPlayerTrackTime;
        }
            
        public static TrackTimes GetTimesForTrack(ETrackId _trackId, UInt32 _lowestPlacementIncluded, UInt32 _highestPlacementIncluded)
        {
            // TODO: Request API to get times for TrackID and convert into our type
            PlayerTrackTime[] allTrackTimes = new PlayerTrackTime[]
            {
                new PlayerTrackTime()
                {
                    ApiPlayerId = new ApiPlayerId() { WebPlayerId = 1 },
                    RunTime     = TimeSpan.FromMilliseconds(90000),
                    Placement   = 1,
                },
                
                new PlayerTrackTime()
                {
                    ApiPlayerId = new ApiPlayerId() { WebPlayerId = 8 },
                    RunTime     = TimeSpan.FromMilliseconds(95000),
                    Placement   = 2,
                },
                
                new PlayerTrackTime()
                {
                    ApiPlayerId = new ApiPlayerId() { WebPlayerId = 4 },
                    RunTime     = TimeSpan.FromMilliseconds(100000),
                    Placement   = 3,
                },
            };

            return new TrackTimes() { PlayerTrackTimes = allTrackTimes };
        }

        public static TrackInfo GetTrackInfoFromId(ETrackId _trackId)
        {
            // TODO: Request API to get track Info for TrackID
            return new TrackInfo()
            {
                TrackId = _trackId,
                WebMapShortUrl = "woodsy-lane",
            };
        }
    }
}
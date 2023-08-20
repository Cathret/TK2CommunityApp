namespace TK2Bot.API
{
    public class ApiCacheManager
    {
        private static readonly DateTime BASE_TIME = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        
        private readonly Dictionary<string, dynamic> m_cacheMap = new Dictionary<string, dynamic>();
        
        private DateTime m_nextUpdateTime = DateTime.UtcNow;

        public bool CheckIfNeedRefresh()
        {
            return m_nextUpdateTime <= DateTime.UtcNow;
        }

        public void ClearCache()
        {
            m_cacheMap.Clear();
        }

        public bool HasCachedValue(string _uri)
        {
            return m_cacheMap.ContainsKey(_uri);
        }
        
        public dynamic? GetCachedJson(string _uri)
        {
            return m_cacheMap.TryGetValue(_uri, out dynamic? jsonContent) ? jsonContent : null;
        }
        
        public void SetCachedJson(string _uri, dynamic _jsonContent)
        {
            if (CheckIfNeedRefresh())
            {
                m_nextUpdateTime = BASE_TIME.AddSeconds((double)_jsonContent.next_update);
            }
            
            m_cacheMap[_uri] = _jsonContent;
        }
    }
}
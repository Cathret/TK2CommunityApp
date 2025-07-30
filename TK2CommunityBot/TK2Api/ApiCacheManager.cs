namespace TK2Bot.API
{
    using CachedPair = KeyValuePair<DateTime, dynamic>;

    public class ApiCacheManager
    {
        private readonly Dictionary<string, CachedPair> m_cacheMap = new();

        public static bool CheckIfNeedRefresh(CachedPair _cachedJson)
        {
            return _cachedJson.Key <= DateTime.UtcNow;
        }

        public void ClearCache()
        {
            m_cacheMap.Clear();
        }

        public bool TryGetCachedValue(string _uri, out dynamic? _outCachedJson)
        {
            if (m_cacheMap.TryGetValue(_uri, out CachedPair cachedJson))
            {
                if (CheckIfNeedRefresh(cachedJson))
                {
                    _outCachedJson = null;
                    return false;
                }
                else
                {
                    _outCachedJson = cachedJson.Value;
                    return true;
                }
            }

            _outCachedJson = null;
            return false;
        }

        public void SetCachedJson(string _uri, dynamic _jsonContent)
        {
            m_cacheMap[_uri] = new CachedPair(ApiSystem.BaseUtcTime.AddSeconds((double)_jsonContent.next_update), _jsonContent);
        }
    }
}

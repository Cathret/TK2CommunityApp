namespace TK2Bot.API
{
    public class ApiCacheManager
    {
        private Dictionary<string, dynamic> m_cacheMap = new Dictionary<string, dynamic>();

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
            m_cacheMap[_uri] = _jsonContent;
        }
    }
}
using Newtonsoft.Json;

namespace KeyMaster_MVC.Repository
{
    public static class SessionExtensions
    {
        public static void Setjson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T Getjson<T>(this ISession session, string key)
        {
            var sessionData = session.GetString(key);
            return sessionData == null ? default(T) : JsonConvert.DeserializeObject<T>(sessionData);
        }
    }
}

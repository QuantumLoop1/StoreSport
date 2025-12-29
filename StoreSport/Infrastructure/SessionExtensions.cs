using System.Text.Json;
using System.Text.Json.Serialization;
using StoreSport.Models;

namespace StoreSport.Infrastructure
{
    public static class SessionExtensions
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles, 
          WriteIndented = false
        };

     public static void SetJson(this ISession session, string key, object value)
        {
  session.SetString(key, JsonSerializer.Serialize(value, _options));
        }

   public static T? GetJson<T>(this ISession session, string key)
        {
            var sessionData = session.GetString(key);
    return sessionData == null ? default(T) : JsonSerializer.Deserialize<T>(sessionData, _options);
        }
    }
}

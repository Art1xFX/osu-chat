using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Osu
{
    public class Api
    {
        
        #region ~Public Methods~

        public static async Task<User> GetUserAsync(string k, object u, int? event_days = null)
        {
            var request = CreateRequestGetUser(k, u, event_days);
            var response = await request.GetResponseAsync();
            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                return ParseGetUser(JArray.Parse(await new StreamReader(response.GetResponseStream()).ReadToEndAsync()));
            }
        }

        private static HttpWebRequest CreateRequestGetUser(string k, object u, int? event_days = null)
        {
            string requestUri = string.Format("https://osu.ppy.sh/api/get_user?k={0}", k);

            if (u is int)
                requestUri += string.Format("&u={0}&type={1}", u, "id");
            else if (u is string)
                requestUri += string.Format("&u={0}&type={1}", u, "string");
            if (event_days != null)
                requestUri += string.Format("&event_days={0}", event_days.Value);

            var request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.Method = "GET";

            return request;
        }

        private static User ParseGetUser(JArray json)
        {
            return new User()
            {
                UserId = (int?)json[0]["user_id"],
                Username = (string)json[0]["username"],
                
            };
        }


        
        #endregion

    }
}

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;

namespace ClearServer.Core.Requester
{
    public static class DataConverter
    {
        public static Dictionary<string, object> DataDeserialize(string content)
        {
            Dictionary<string, object> keyValuePairs;
            try
            {
                //Geting values from json
                keyValuePairs = new Dictionary<string, object>();
                keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                return keyValuePairs;
            }
            catch { }
            try
            {
                //geting values from queryString
                keyValuePairs = new Dictionary<string, object>();
                var query = HttpUtility.ParseQueryString(content);

                foreach (var parse in query.AllKeys)
                {
                    keyValuePairs.Add(parse, query.Get(parse));
                }
                return keyValuePairs;


            }
            catch { }
            return null;
        }
    }
}

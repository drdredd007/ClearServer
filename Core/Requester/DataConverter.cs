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
            return null;
        }
    }
}

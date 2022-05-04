using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LUIS.Get.Intent
{
    public class Endpoint
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId">LuisAppID</param>
        /// <param name="endpointKey">Subscription KEY</param>
        /// <param name="textRequest">Text to send to LUIS</param>

        public async Task<string> Obtein(string appId, string endpointKey, string textRequest)
        {
            string result = string.Empty;
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", endpointKey);
            queryString["q"] = textRequest;

            //Default values of the request
            queryString["timezoneOffset"] = "0";
            queryString["verbose"] = "false";
            queryString["spellCheck"] = "false";
            queryString["staging"] = "false";

            var endpointUri = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/" + appId + "?" + queryString;
            var response = await client.GetAsync(endpointUri);

            var strResponseContent = await response.Content.ReadAsStringAsync();
            if (strResponseContent != null)
                result = strResponseContent.ToString();
            return result;
        }
    }
}

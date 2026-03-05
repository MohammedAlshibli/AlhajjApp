using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;

namespace Pligrimage.Web.Dto
{
    public class JundToken
    {
        public string GetsecurityToken()
        {

            var url = "https://jundauth/connect/token";
            var oauthClient = new RestClient(url);

            var request = new RestRequest()
            {
                Method = Method.POST
            };



            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("scope", "Jund");
            request.AddParameter("client_id", "ITS_Apps");
            request.AddParameter("client_secret", "ITS@123");
            request.AddParameter("grant_type", "client_credentials");
            var tResponse = oauthClient.Execute(request);
            var responseJson = tResponse.Content;
            var securityToken = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson)["access_token"].ToString();

            return securityToken;

        }
    }
}

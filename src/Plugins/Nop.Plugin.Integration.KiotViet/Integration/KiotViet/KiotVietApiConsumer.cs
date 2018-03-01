using Newtonsoft.Json;
using Nop.Core.Extensions;
using Nop.Plugin.Integration.KiotViet.Integration.KiotViet.Entities;
using RestSharp;
using System.Net;

namespace Nop.Plugin.Integration.KiotViet.Integration.KiotViet
{
    public class KiotVietApiConsumer
    {
        protected RestClient Client;

        public KiotVietApiConsumer()
        {
            Client = new RestClient("https://public.kiotapi.com");
        }

        private ApiResponse<T> HandleResponse<T>(IRestResponse response) where T : new()
        {
            var apiResponse = new ApiResponse<T>()
            {
                Status = response.StatusCode
            };
            if (response.StatusCode.IsIn(HttpStatusCode.OK, HttpStatusCode.Created))
            {
                apiResponse.Resource = JsonConvert.DeserializeObject<T>(response.Content);
            }
            else
            {
                apiResponse.Error = new ApiError(response.Content);
            }

            return apiResponse;
        }

        public string GetToken()
        {
            var tokenClient = new RestClient("https://id.kiotviet.vn/");
            var request = new RestRequest("connect/token", Method.POST);
            request.AddParameter("client_id", "8a80ab27-b951-4ad1-879b-c898065f0c1d");
            request.AddParameter("client_secret", "F3462C9643299B121CD4486DCAC620CDDCC03122");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scopes", "PublicApi.Access");

            var response = tokenClient.Execute(request);
            var apiResponse = HandleResponse<GetTokenResponse>(response);

            return apiResponse.Resource?.access_token ?? string.Empty;
        }
    }
}

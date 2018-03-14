using System.Collections.Generic;
using System.Linq;
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
        private string _token;
        public KiotVietApiConsumer()
        {
            Client = new RestClient(KiotVietConstant.UrlApiRootPublic);
            _token = GetToken();
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
            var tokenClient = new RestClient(KiotVietConstant.UrlApiRootToken);
            var request = new RestRequest(KiotVietConstant.UrlApiGetToken, Method.POST);
            request.AddParameter("client_id", "8a80ab27-b951-4ad1-879b-c898065f0c1d");
            request.AddParameter("client_secret", "F3462C9643299B121CD4486DCAC620CDDCC03122");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scopes", "PublicApi.Access");

            var response = tokenClient.Execute(request);
            var apiResponse = HandleResponse<GetTokenResponse>(response);

            return apiResponse.Resource?.access_token ?? string.Empty;
        }

        public List<KVCategory> GetKVCategories()
        {
            if (string.IsNullOrEmpty(_token))
            {
                return null;
            }
            var request = new RestRequest(KiotVietConstant.UrlApiGetCategory, Method.GET);
            request.AddHeader("Retailer", "bisonkiotvietcom");
            request.AddHeader("Authorization", $"Bearer {_token}");
            var response = Client.Execute(request);
            var apiResponse = HandleResponse<GetCategoryResponse>(response);
            if (apiResponse.Status == HttpStatusCode.Unauthorized)
            {
                _token = GetToken();
            }
            return apiResponse.Resource.data;
        }

        public List<KVProduct> GetAllProducts()
        {
            var currentItemId = 0;
            var pageSize = 100;
            var allowContinue = true;
            if (string.IsNullOrEmpty(_token))
            {
                return null;
            }
            List<KVProduct> kvProducts = new List<KVProduct>();
            while (allowContinue)
            {
                var request = new RestRequest(string.Format(KiotVietConstant.UrlApiGetProduct, currentItemId,pageSize), Method.GET);
                request.AddHeader("Retailer", "bisonkiotvietcom");
                request.AddHeader("Authorization", $"Bearer {_token}");
                var response = Client.Execute(request);
                var apiResponse = HandleResponse<GetProductResponse>(response);
                if (apiResponse.Status == HttpStatusCode.Unauthorized)
                {
                    _token = GetToken();
                }
                else
                {
                    var dataResult = apiResponse.Resource.data;
                    kvProducts.AddRange(dataResult);
                    if (dataResult.Count < pageSize)
                    {
                        allowContinue = false;
                    }
                    else
                    {
                        currentItemId = (kvProducts.Count - 1);
                    }
                }
            }
            return kvProducts;
        }

        public List<KVProduct> GetProductsByCategoryId(int categoryId)
        {
            if (string.IsNullOrEmpty(_token))
            {
                return null;
            }
            var request = new RestRequest(string.Format(KiotVietConstant.UrlApiGetProduct,categoryId), Method.GET);
            request.AddHeader("Retailer", "bisonkiotvietcom");
            request.AddHeader("Authorization", $"Bearer {_token}");
            var response = Client.Execute(request);
            var apiResponse = HandleResponse<GetProductResponse>(response);
            if (apiResponse.Status == HttpStatusCode.Unauthorized)
            {
                _token = GetToken();
            }
            return apiResponse.Resource.data;
        }


    }
}

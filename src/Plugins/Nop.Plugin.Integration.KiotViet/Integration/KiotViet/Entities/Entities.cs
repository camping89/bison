using System.Net;

namespace Nop.Plugin.Integration.KiotViet.Integration.KiotViet.Entities
{
    public class ApiResponse<T>
    {
        public HttpStatusCode Status { get; set; }
        public T Resource { get; set; }
        public ApiError Error { get; set; }
    }

    public class ApiError
    {
        public string error { get; set; }

        public ApiError(string error)
        {
            this.error = error;
        }
    }

    public class GetTokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }
}

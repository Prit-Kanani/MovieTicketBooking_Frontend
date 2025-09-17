using System.Net.Http.Headers;
using Movie_management_system.Helper;
using Movie_management_system.DTOs;
using Newtonsoft.Json;

namespace Movie_management_system
{
    public class ApiHelper
    {
        private readonly HttpClient _client;

        public ApiHelper()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://localhost:port/api/");
            if (!string.IsNullOrEmpty(TokenManager.Token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenManager.Token);
            }
        }
    }

}

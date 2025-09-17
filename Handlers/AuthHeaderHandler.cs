using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Movie_management_system.Helper; // TokenManager

namespace Movie_management_system.Handlers
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Only add if token exists
            var token = TokenManager.Token;
            if (!string.IsNullOrEmpty(token))
            {
                // Overwrite existing header (safer)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}

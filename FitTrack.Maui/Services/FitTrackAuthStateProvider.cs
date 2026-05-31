using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace FitTrack.Maui.Services
{
    public class FitTrackAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ApiClient _apiClient;

        public FitTrackAuthStateProvider(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _apiClient.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return Anonymous();

                var claims = ParseJwtClaims(token);
                var identity = new ClaimsIdentity(claims, "jwt");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                return Anonymous();
            }
        }

        public async Task NotifyUserAuthenticated(string token)
        {
            await _apiClient.SaveTokenAsync(token);
            var claims = ParseJwtClaims(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var principal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }

        public void NotifyUserLoggedOut()
        {
            _apiClient.ClearToken();
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymous()));
        }

        private static AuthenticationState Anonymous()
            => new(new ClaimsPrincipal(new ClaimsIdentity()));

        private static IEnumerable<Claim> ParseJwtClaims(string token)
        {
            var segments = token.Split('.');
            if (segments.Length < 2)
                return Enumerable.Empty<Claim>();

            var payload = segments[1];
            var padded = payload + new string('=', (4 - payload.Length % 4) % 4);
            var bytes = Convert.FromBase64String(padded);
            var json = Encoding.UTF8.GetString(bytes);
            var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            if (parsed == null)
                return Enumerable.Empty<Claim>();

            var claims = new List<Claim>();
            foreach (var kvp in parsed)
            {
                if (kvp.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in kvp.Value.EnumerateArray())
                        claims.Add(new Claim(kvp.Key, item.ValueKind == JsonValueKind.String ? item.GetString()! : item.ToString()));
                }
                else
                {
                    claims.Add(new Claim(kvp.Key, kvp.Value.ValueKind == JsonValueKind.String ? kvp.Value.GetString()! : kvp.Value.ToString()));
                }
            }
            return claims;
        }
    }
}


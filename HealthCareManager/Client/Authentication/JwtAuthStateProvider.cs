using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealthCareManager.Client.Authentication
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        public string _userName = "Unknown User";
        public JwtAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await GetTokenAsync();
            var claims = ParseClaimsFromJwt(token);
            if (claims is object)
            {
                string? name = claims.FirstOrDefault(x => x.Type == "FullName")?.Value;
                if (name is object)
                    _userName = name;
            }

            var identity = string.IsNullOrEmpty(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(claims, "jwt");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task SetTokenAsync(string? token, DateTime expiry = default)
        {
            if (token == null)
            {
                await _localStorage.RemoveItemAsync("authToken");
                await _localStorage.RemoveItemAsync("authTokenExpiry");
            }
            else
            {
                await _localStorage.SetItemAsStringAsync("authToken", token);
                await _localStorage.SetItemAsync("authTokenExpiry", expiry);
            }

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task<string?> GetTokenAsync()
        {
            var expiry = await _localStorage.GetItemAsync<DateTime>("authTokenExpiry");

            if (expiry != default)
            {
                if (DateTime.Parse(expiry.ToString()) > DateTime.UtcNow)
                {
                    return await _localStorage.GetItemAsStringAsync("authToken");
                }
                else
                {
                    await SetTokenAsync(null);
                }
            }
            return null;
        }

        public string GetUserName() => _userName;

        private static IEnumerable<Claim>? ParseClaimsFromJwt(string? jwt)
        {
            if(jwt is null)
                return null;

            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs?.Select(kvp => new Claim(kvp.Key, (kvp.Value).ToString() ?? ""));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}

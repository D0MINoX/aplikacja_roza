using MauiApp1.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace MauiApp1.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private const string TokenKey = "jwt_token";

        public string Token { get; private set; }
        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var loginData = new { username = username, password = password };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginData);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                Token = result.Token;
                await SecureStorage.Default.SetAsync(TokenKey, Token);
                return true;
            }
            return false;
        }
        public async Task<bool> CheckAndSetTokenAsync()
        {
            var token = await SecureStorage.Default.GetAsync(TokenKey);
            if (!string.IsNullOrEmpty(token))
            {
                Token = token;
                return true;
            }
            return false;
        }

        public async Task<bool> RegisterAsync(string name, string surname, string username, string password,int? parish)
        {
            var registerData = new { username = username, password = password, name = name, surname = surname,parish=parish };
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/register", registerData);
                if (!response.IsSuccessStatusCode)
                {
              
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"API ERROR: {response.StatusCode} - {errorContent}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd rejestracji: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> Logout()
        {
            try
            {
                Token = null;
                SecureStorage.Default.Remove(TokenKey);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> CanUserSendSmsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Token)) return false;

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(Token);
                var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);

                if (int.Parse(roleClaim?.Value ?? "5") > 2) return false;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var response = await _httpClient.GetAsync("api/Auth/CheckSmsPermission");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PermissionResponse>();
                    return result?.canSend ?? false;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public class PermissionResponse { public bool canSend { get; set; } }
    }
}


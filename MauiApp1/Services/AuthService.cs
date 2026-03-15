using MauiApp1.Models;
using System.Diagnostics;
using System.Net.Http.Json;

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

        public async Task<bool> RegisterAsync(string name, string surname, string username, string password)
        {
            var registerData = new { username = username, password = password, name = name, surname = surname };
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/register", registerData);
                if (!response.IsSuccessStatusCode)
                {
                    // ODCZYTAJ TREŚĆ BŁĘDU Z API
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
    }
}


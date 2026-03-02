using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using MauiApp1.Models;

namespace MauiApp1
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

        // --- WYLOGOWANIE ---
        public async Task<bool> Logout()
        {
            try
            {
                Token = null;
                SecureStorage.Default.Remove(TokenKey);
                return true;

            }
        catch(Exception ex)
            {
                return false;
            }
        }
    }
}


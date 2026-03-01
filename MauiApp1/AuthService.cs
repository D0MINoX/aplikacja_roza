using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace MauiApp1
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var loginData = new { username = username, password = password };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginData);
            return response.IsSuccessStatusCode;
        }
    }
}

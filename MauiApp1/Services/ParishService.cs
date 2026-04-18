using MauiApp1.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace MauiApp1.Services
{
    public class ParishService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        public ParishService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }
        public async Task<(bool isSuccess, List<Parish> Data, string ErrorMessage)> AllParish()
        {
            string url = $"api/Parish/getAllParish";
            try
            {
                if (string.IsNullOrEmpty(_authService.Token)) return (false, null, "Błąd dostępu");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<Parish>>();
                    return (true, data, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, null, errorContent ?? $"Błąd serwera: {response.StatusCode}");
                }


            }
            catch (Exception ex)
            {
                return (false, null, $"Błąd połączenia: {ex.Message}");
            }
        }
        public async Task<(bool isSuccess, Parish Data, string ErrorMessage)> GetUserParish(int UserId)
        {
            string url = $"api/Parish/getUserParish/{UserId}";
            try
            {
                if (string.IsNullOrEmpty(_authService.Token)) return (false, null, "Błąd dostępu");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<Parish>();
                    return (true, data, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, null, errorContent ?? $"Błąd serwera: {response.StatusCode}");
                }


            }
            catch (Exception ex)
            {
                return (false, null, $"Błąd połączenia: {ex.Message}");
            }
        }
    }
}

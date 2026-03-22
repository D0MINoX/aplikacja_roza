using MauiApp1.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace MauiApp1.Services
{
    public class ParishService
    {
        private readonly HttpClient _httpClient;
        public ParishService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<(bool isSuccess, List<Parish> Data, string ErrorMessage)> AllParish()
        {
            string url = $"api/Parish/getAllParish";
            try
            {
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

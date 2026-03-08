
using MauiApp1.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;

namespace MauiApp1
{
    public class RosaryService
    {
        private readonly HttpClient _httpClient;
        public RosaryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<RosaryInfo>> GetUserRosariesAsync(int userId)
        {
            try
            {
                // Adres Twojego API
                string url = $"api/Rosaries/user/{userId}/rosaries";

                // Pobranie i automatyczna deserializacja JSON do listy obiektów
                var response = await _httpClient.GetFromJsonAsync<List<RosaryInfo>>(url);

                return response ?? new List<RosaryInfo>();
            }
            catch (Exception ex)
            {
                return new List<RosaryInfo>();
            }
        }
     
        public async Task<List<RosaryInfo>> GetAllRosariesAsync()
        {
            try
            {
                // Adres Twojego API
                string url = $"api/Rosaries/rosaries";

                
                var response = await _httpClient.GetFromJsonAsync<List<RosaryInfo>>(url);

                return response ?? new List<RosaryInfo>();
            }
            catch (Exception ex)
            {
                return new List<RosaryInfo>();
            }
        }
        public async Task<bool> JoinRosaryAsync(int UserId,int RosaryId)
        {
            string url = $"api/Rosaries/JoinRosary";
            var requestData = new { UserId = UserId, RosaryId = RosaryId };
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, requestData);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"API ERROR: {response.StatusCode} - {errorContent}");
                    return false;
                }
                return true;

            }catch (Exception ex)
            {
                Debug.WriteLine($"Błąd rejestracji: {ex.Message}");
                return false;
            }
        }
    }
}

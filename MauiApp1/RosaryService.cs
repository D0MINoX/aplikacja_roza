
using MauiApp1.Models;
using System;
using System.Collections.Generic;
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
    }
}

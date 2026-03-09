using MauiApp1.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace MauiApp1.Services
{
    public class AdminService
    {
        
        private readonly HttpClient _httpClient;
        public AdminService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool isSuccess, List<AdminUserView> Data, string ErrorMessage)> AdminUsers(int rosaryId)
        {
            string url = $"api/Admin/{rosaryId}/usersShow";
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<AdminUserView>>();
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

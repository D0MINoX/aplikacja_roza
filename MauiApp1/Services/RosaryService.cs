
using MauiApp1.Models;
using System.Net.Http.Json;

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
        public async Task<(bool IsSuccess, string ErrorMessage)> JoinRosaryAsync(int UserId, int RosaryId)
        {
            string url = $"api/Rosaries/JoinRosary";
            var requestData = new { UserId = UserId, RosaryId = RosaryId };
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, requestData);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, errorContent ?? "Nieznany błąd serwera");
                }
                return (true, null);

            }
            catch (Exception ex)
            {
                return (false, $"Błąd sieci: {ex.Message}");
            }
        }
        public async Task<string> GetNameAsync(int RosaryId)
        {
            try
            {
                string url = $"api/Rosaries/rosary/{RosaryId}/Name";


                var response = await _httpClient.GetStringAsync(url);

                return response;
            }
            catch (Exception ex)
            {
                return "error";
            }
        }
    }
}

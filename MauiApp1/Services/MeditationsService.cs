using MauiApp1.Models;
using MauiApp1.Services;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MauiApp1
{
    public class MeditationsService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        public MeditationsService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }
        public async Task<LocalMeditation> GetMeditationData(int date, string title)
        {


            try
            {
               
                var meditation = await _httpClient.GetFromJsonAsync<List<LocalMeditation>>(
                    $"api/meditations/search?date={date}&title={title}");

                return meditation?.FirstOrDefault();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"### BŁĄD API: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Obsługa błędów sieciowych/deserializacji
                Debug.WriteLine($"### BŁĄD KRYTYCZNY: {ex.Message}");
                return null;
            }
        }

        public async Task<List<LocalMeditation>> GetAllMeditationsForMystery(string title)
        {
            try
            {
                string url = $"api/meditations/search?title={Uri.EscapeDataString(title)}";
              
                var meditations = await _httpClient.GetFromJsonAsync<List<LocalMeditation>>(url);
                return meditations ?? new List<LocalMeditation>();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"### BŁĄD API: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Obsługa błędów sieciowych/deserializacji
                Debug.WriteLine($"### BŁĄD KRYTYCZNY: {ex.Message}");
                return null;
            }
        }
    }
}


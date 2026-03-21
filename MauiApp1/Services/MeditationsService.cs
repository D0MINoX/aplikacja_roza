using MauiApp1.Models;
using System.Net.Http.Json;
using System.Diagnostics;
namespace MauiApp1
{
    public class MeditationsService
    {
        private readonly HttpClient _httpClient;
        public MeditationsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<MeditationInfo> GetMeditationData(int date, string title)
        {


            try
            {
                // Używamy GetFromJsonAsync, który automatycznie zamieni JSON na obiekt C#
                var meditation = await _httpClient.GetFromJsonAsync<MeditationInfo>(
                    $"api/meditations/search?date={date}&title={title}");

                return meditation;
            }
            catch (HttpRequestException ex)
            {
                // Obsługa błędów HTTP (np. 404, 500)
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

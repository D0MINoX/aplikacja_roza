using MauiApp1.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MauiApp1.Services
{
    public class ErrorService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public ErrorService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        // Metoda do zgłaszania błędów
        public async Task<(bool isSuccess, string ErrorMessage)> SubmitErrorAsync(ErrorReport errorReport)
        {
            try
            {

                var response = await _httpClient.PostAsJsonAsync("api/error/submit", errorReport);

                if (response.IsSuccessStatusCode)
                {
                    return (true, null); // Sukces - brak danych o błędzie
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, errorContent ?? "Błąd serwera przy przesyłaniu zgłoszenia.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas zgłaszania błędu: {ex.Message}");
                return (false, $"Błąd połączenia: {ex.Message}");
            }
        }

        // Pobieranie wszystkich zgłoszeń błędów
        public async Task<(bool isSuccess, List<ErrorReport> Data, string ErrorMessage)> GetAllErrorsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_authService.Token))
                    return (false, null, "Brak dostępu.");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);

                var response = await _httpClient.GetAsync("api/error/all");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<ErrorReport>>();
                    return (true, data, null);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, null, errorContent ?? "Błąd serwera przy pobieraniu zgłoszeń.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd pobierania zgłoszeń błędów: {ex.Message}");
                return (false, null, $"Błąd połączenia: {ex.Message}");
            }
        }

        // Pobieranie szczegółów zgłoszenia po ID
        public async Task<(bool isSuccess, ErrorReport Data, string ErrorMessage)> GetErrorDetailsAsync(int id)
        {
            try
            {
                if (string.IsNullOrEmpty(_authService.Token))
                    return (false, null, "Brak dostępu.");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);

                var response = await _httpClient.GetAsync($"api/error/details/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ErrorReport>();
                    return (true, data, null);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, null, errorContent ?? "Błąd serwera przy pobieraniu szczegółów zgłoszenia.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas pobierania szczegółów zgłoszenia: {ex.Message}");
                return (false, null, $"Błąd połączenia: {ex.Message}");
            }
        }

        // Aktualizacja statusu zgłoszenia
        public async Task<(bool isSuccess, string ErrorMessage)> UpdateErrorStatusAsync(int id, string status)
        {
            try
            {
                if (string.IsNullOrEmpty(_authService.Token))
                    return (false, "Brak dostępu.");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);

                var query = $"api/error/update-status/{id}?status={status}";

                var response = await _httpClient.PutAsync(query, null);

                if (response.IsSuccessStatusCode)
                {
                    return (true, null); // Sukces - brak błędów
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, errorContent ?? "Błąd serwera podczas aktualizacji zgłoszenia.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas aktualizacji statusu zgłoszenia: {ex.Message}");
                return (false, $"Błąd połączenia: {ex.Message}");
            }
        }
    }
}
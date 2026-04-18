using MauiApp1.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;


namespace MauiApp1.Services
{
    public class MessagesService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        public MessagesService( HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }
        public async Task<(bool isSuccess, List<RosaryMessage> Data, string ErrorMessage)> GetMessagesAsync(int rosaryId)
        {
            string url = $"api/Messages/getMessages/{rosaryId}";
            try
            {
                if (string.IsNullOrEmpty(_authService.Token)) return (false, null, "Błąd dostępu");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<RosaryMessage>>();
                    return (true,data,null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return  (false,null,  errorContent??$"Błąd serwera: {response.StatusCode}");
                }


            }
            catch (Exception ex)
            {
                return (false, null, $"Błąd połączenia: {ex.Message}");
            }
        }
        public async Task<bool> NewMessageAsync(RosaryMessage message)
        {
            try
            {
                string url = $"api/Messages/newMessage";
                if (string.IsNullOrEmpty(_authService.Token)) return false;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
                var response = await _httpClient.PostAsJsonAsync(url, message);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd rejestracji: {ex.Message}");
                return false;
            }
        }
        public async Task<List<string>> getExternalNumbers(int rosaryId)
        {
            try
            {
                if (string.IsNullOrEmpty(_authService.Token)) return new List<string>();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
                var phones = await _httpClient.GetFromJsonAsync<List<string>>($"api/Messages/GetExternalNumbers/{rosaryId}");

                return phones ?? new List<string>();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Błąd sieciowy: {ex.Message}");
                return new List<string>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd deserializacji: {ex.Message}");
                return new List<string>();
            }
        }
        public async Task<List<ExternalNumber>> getAdminExternalNumbers(int rosaryId)
        {
            try
            {
                //weryfikacja admin dodać
                if (string.IsNullOrEmpty(_authService.Token)) return new List<ExternalNumber>();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
                var phones = await _httpClient.GetFromJsonAsync<List<ExternalNumber>>($"api/Messages/GetAdminExternalNumbers/{rosaryId}");

                return phones ?? new List<ExternalNumber>();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Błąd sieciowy: {ex.Message}");
                return new List<ExternalNumber>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd deserializacji: {ex.Message}");
                return new List<ExternalNumber>();
            }
        }
    }
}

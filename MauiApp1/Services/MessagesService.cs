using MauiApp1.Models;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;


namespace MauiApp1.Services
{
    public class MessagesService
    {
        private readonly HttpClient _httpClient;
        public MessagesService( HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<(bool isSuccess, List<RosaryMessage> Data, string ErrorMessage)> GetMessagesAsync(int rosaryId)
        {
            string url = $"api/Messages/getMessages/{rosaryId}";
            try
            {
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

namespace MauiApp1
{
    public class MeditationsService
    {
        private readonly HttpClient _httpClient;
        public MeditationsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> GetOnlyDescription(DateTime date, string title)
        {
            string dateStr = date.ToString("yyyy-MM-dd");
         
            try
            {
                // Budujemy pełny URL z parametrami
                var response = await _httpClient.GetAsync($"api/meditations/search?date={dateStr}&title={title}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                // Jeśli API zwróciło błąd (np. 404), warto wiedzieć dlaczego
                return $"Błąd serwera: {response.StatusCode}";
            }
            catch (Exception ex)
            {
                return $"Błąd połączenia: {ex.Message}";
            }
        }
    }
}

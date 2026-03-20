namespace MauiApp1
{
    public class MeditationsService
    {
        private readonly HttpClient _httpClient;
        public MeditationsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> GetOnlyDescription(int date, string title)
        {


            try
            {
                // Budujemy pełny URL z parametrami
                var response = await _httpClient.GetAsync($"api/meditations/search?date={date}&title={title}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                // Jeśli API zwróciło błąd (np. 404), warto wiedzieć dlaczego
                return $"Błąd serwera: {response.StatusCode}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"### BŁĄD SIECI: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"### INNER: {ex.InnerException.Message}");
                }
                return $"Błąd połączenia: {ex.Message}";
            }
        }
    }
}

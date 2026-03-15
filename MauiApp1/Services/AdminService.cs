using MauiApp1.Models;
using System.Diagnostics;
using System.Net.Http.Json;

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

        public async Task<(bool isSuccess, string ErrorMessage)> VerifyUser(int userId, int rosaryId)
        {
            string url = $"api/Admin/{userId}/Authorization/{rosaryId}";
            try
            {
                var response = await _httpClient.PutAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return (true, content);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, errorContent ?? $"Błąd serwera: {response.StatusCode}");
                }


            }
            catch (Exception ex)
            {
                return (false, $"Błąd połączenia: {ex.Message}");
            }
        }
        public async Task<(bool isSuccess, string ErrorMessage)> DeleteUser(int userId, int rosaryId)
        {
            string url = $"api/Admin/delete-membership/{userId}/{rosaryId}";
            try
            {
                var response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return (true, content);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, errorContent ?? $"Błąd serwera: {response.StatusCode}");
                }


            }
            catch (Exception ex)
            {
                return (false, $"Błąd połączenia: {ex.Message}");
            }
        }

        public async Task<(bool isSuccess, List<AdminUserView> Data, string ErrorMessage)> AdminZelators()
        {
            string url = $"api/Admin/zelatorsShow";
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
        public async Task<bool> RegisterAsync(string name, string parish, int zelatorId)
        {
            var registerData = new { name = name, parish = parish,zelatorsId = zelatorId };
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Admin/AddRosary", registerData);
                if (!response.IsSuccessStatusCode)
                {
                    // ODCZYTAJ TREŚĆ BŁĘDU Z API
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"API ERROR: {response.StatusCode} - {errorContent}");
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
        public async Task<bool> ModifyMeditationAsync(string Title, string Content, int Date)
        {
            var ModifyData = new { Title = Title,Content = Content,Date = Date };
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/Admin/ModifyMeditation",ModifyData);
                if (!response.IsSuccessStatusCode)
                {
                 
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"API ERROR: {response.StatusCode} - {errorContent}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd Zmiany rozważania: {ex.Message}");
                return false;
            }
        }

        public async Task<(bool isSuccess, List<AdminUserView> Data, string ErrorMessage)> UsersPrivilagiesShow(int UserRole)
        {
            string url = $"api/Admin/usersShow/{UserRole}";
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

        public async Task<bool> UpdateRole(int userId, int userRole)
        {
            var ModifyData = new { Id = userId,Role = userRole  };
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/Admin/UpdateRole", ModifyData);
                if (!response.IsSuccessStatusCode)
                {

                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"API ERROR: {response.StatusCode} - {errorContent}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd Zmiany rozważania: {ex.Message}");
                return false;
            }
        }
    }
}

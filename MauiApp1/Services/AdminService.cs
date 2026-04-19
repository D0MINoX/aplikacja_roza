using MauiApp1.Models;
using Newtonsoft.Json.Serialization;
using System.Buffers.Text;
using System.Diagnostics;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MauiApp1.Services
{
    public class AdminService
    {

        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        public AdminService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<(bool isSuccess, List<AdminUserView> Data, string ErrorMessage)> AdminUsers(int rosaryId)
        {
            string url = $"api/Admin/{rosaryId}/usersShow";
            try
            {
                if (string.IsNullOrEmpty(_authService.Token)) return (false, null, "Błąd dostępu");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
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
                if (string.IsNullOrEmpty(_authService.Token)) return (false, "Błąd dostępu");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
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
                if (string.IsNullOrEmpty(_authService.Token)) return (false, "Błąd dostępu");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
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
                if (string.IsNullOrEmpty(_authService.Token)) return (false,null, "Błąd dostępu");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
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
        public async Task<bool> RegisterAsync(string name, int parish, int zelatorId)
        {
            var registerData = new { name = name, parish = parish,zelatorsId = zelatorId };
            try
            {
                if (string.IsNullOrEmpty(_authService.Token)) return false;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
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
        public async Task<bool> ModifyMeditationAsync(string Title, string Content, int Date,string link)
        {
            var ModifyData = new { Title = Title,Content = Content,Date = Date,Link = link };
            try
            {
                if (string.IsNullOrEmpty(_authService.Token)) return false;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
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
                if (string.IsNullOrEmpty(_authService.Token)) return (false, null, $"Błąd serwera: odmowa dostępu");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
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

       

        public async Task<(bool isSuccess, List<AdminUserView> Data, string ErrorMessage)> AdminMainZelators()
        {
            string url = $"api/Admin/MainZelatorsShow";
            try
            {
                if (string.IsNullOrEmpty(_authService.Token)) return (false, null, $"Błąd serwera: odmowa dostępu");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
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
        public async Task<bool> AddParish(string name, int zelatorId)
        {
            var registerData = new { name = name, zelatorsId = zelatorId };
            try
            {
                if (string.IsNullOrEmpty(_authService.Token)) return false;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
                var response = await _httpClient.PostAsJsonAsync("api/Admin/AddParish", registerData);
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

        public async Task<bool> UpdateUserPermissions(int userId, int userRole, bool userCanSendSMS)
        {
            var ModifyData = new { Id = userId, Role = userRole,CanSendSMS=userCanSendSMS };
            try
            {
                if (string.IsNullOrEmpty(_authService.Token)) return false;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
                var response = await _httpClient.PutAsJsonAsync("api/Admin/UpdatePermissions", ModifyData);
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
                Debug.WriteLine($"Błąd Zmiany uprawnień: {ex.Message}");
                return false;
            }
        }
         public async Task<(bool isSuccess, List<UserConsent> Data, string ErrorMessage)> AdminConsentsShow()
         {
            string url = $"api/Admin/ConsentsShow";
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<UserConsent>>();
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

        public async Task<(bool isSuccess, string ErrorMessage)> DeleteExternalNumber(int userId, int rosaryId)
        {
            string url = $"api/Admin/deleteExternalNumber/{userId}/{rosaryId}";
            try
            {
                if (string.IsNullOrEmpty(_authService.Token)) return (false, $"Błąd serwera: odmowa dostępu");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
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

        public async Task<bool> RegisterExternalMemberAsync(string? name, string? surname, string phone, int rosaryId,string publicIp)
        {
            var request = new
            {
                FirstName = name,
                LastName = surname,
                PhoneNumber = phone,
                RosaryId = rosaryId,
                UserIp=publicIp
            };
            if (string.IsNullOrEmpty(_authService.Token)) return false;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
            var response = await _httpClient.PostAsJsonAsync($"api/Admin/AddExternalMember", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateExternalMember(int userId,string? name, string? surname, string phoneNumber)
        {
            var ModifyData = new {Id = userId, Name = name, Surname = surname, PhoneNumber=phoneNumber };
            try
            {
                if (string.IsNullOrEmpty(_authService.Token)) return false;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
                var response = await _httpClient.PutAsJsonAsync("api/Admin/UpdateExternalMember", ModifyData);
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
                Debug.WriteLine($"Błąd Zmiany uprawnień: {ex.Message}");
                return false;
            }
        }
        public async Task<(bool isSuccess, string ErrorMessage)> deleteUser(int userId)
        {
            var Data = new { Id = userId };
            try
            {
                if (string.IsNullOrEmpty(_authService.Token)) return (false,"Brak dostępu");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
                var request = new HttpRequestMessage(HttpMethod.Delete, "api/Admin/deleteUser")
                {
                    Content = JsonContent.Create(Data)
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return (true,null);
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, errorContent ?? $"Błąd serwera: {response.StatusCode}");
               
            }
            catch (Exception ex)
            {
                return (false, $"Błąd połączenia: {ex.Message}");
            }
        }
    }
}

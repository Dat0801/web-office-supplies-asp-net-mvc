using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VanPhongPham.ModelViews;

namespace VanPhongPham.APIService
{
    public class ApiService : IDisposable
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://vanphongpham-001-site1.ltempurl.com")
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Đăng nhập
        public async Task<bool> LoginAsync(UserLoginModelView user)
        {
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/auth/auth_account", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(json);
                var accessToken = jsonResponse?.data?.accessToken;
                var refreshToken = jsonResponse.data?.refreshToken;
                var expiresIn = jsonResponse.data?.expiresIn;
                SaveToken(accessToken, refreshToken, expiresIn);
                return true;
            }
            else
                return false;
        }

        // Đăng ký
        public async Task<bool> RegisterAsync(UserRegisterModelView model)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
                return false;
        }

        // Lưu JWT Token vào session
        private void SaveToken(string accessToken, string refreshToken, dynamic expiresIn)
        {
            System.Web.HttpContext.Current.Session["JWTToken"] = accessToken;
            System.Web.HttpContext.Current.Session["RefreshToken"] = refreshToken;
            System.Web.HttpContext.Current.Session["ExpiresIn"] = expiresIn;
        }
        private async Task EnsureTokenIsValid()
        {
            var token = System.Web.HttpContext.Current.Session["JWTToken"] as string;
            var expiresIn = DateTime.Parse(System.Web.HttpContext.Current.Session["ExpiresIn"] as string);
            
            if (string.IsNullOrEmpty(token) || DateTime.UtcNow >= expiresIn)
            {
                await RefreshTokenAsync();
            }
        }        
        private async void SetTokenHeader()
        {
            await EnsureTokenIsValid();
            var token = System.Web.HttpContext.Current.Session["JWTToken"] as string;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        public async Task<string> RefreshTokenAsync()
        {
            var refreshToken = System.Web.HttpContext.Current.Session["RefreshToken"] as string;

            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new Exception("Không tìm thấy refresh token.");
            }

            var content = new StringContent(JsonConvert.SerializeObject(new { refreshToken }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/auth/refresh_token", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(json);
                var accessToken = jsonResponse.data?.accessToken;
                var newRefreshToken = jsonResponse.data?.refreshToken;
                var expiresIn = jsonResponse.data?.expiresIn;

                // Lưu lại token mới
                SaveToken(accessToken, refreshToken, expiresIn);
                return accessToken;
            }

            throw new Exception("Làm mới token thất bại.");
        }

        // GET Method
        public async Task<T> GetAsync<T>(string apiUrl)
        {
            SetTokenHeader();
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        // POST Method
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string apiUrl, TRequest data)
        {
            SetTokenHeader();
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, content);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(json);
        }

        // PUT Method
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string apiUrl, TRequest data)
        {
            SetTokenHeader();
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(apiUrl, content);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(json);
        }

        // DELETE Method
        public async Task DeleteAsync(string apiUrl)
        {
            SetTokenHeader();
            var response = await _httpClient.DeleteAsync(apiUrl);
            response.EnsureSuccessStatusCode();
        }

        // Dispose method
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}

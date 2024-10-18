using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VanPhongPham.ModelViews;

namespace VanPhongPham.APIService
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();
            // Thay thế URL của API bên dưới bằng URL của Web API của bạn
            _httpClient.BaseAddress = new Uri("https://254b-171-243-49-91.ngrok-free.app");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Đăng nhập
        public async Task<string> LoginAsync(string email, string password)
        {
            var loginData = new
            {
                Email = email,
                Password = password
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);
                // Lưu JWT token nếu cần
                SaveToken(tokenResponse.Token);
                //return tokenResponse.Token;
            }

            throw new Exception("Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.");
        }

        // Đăng ký
        public async Task<bool> RegisterAsync(UserRegisterModelView model)
        {
            UserRegisterModelView registerData = new UserRegisterModelView
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password                
            };

            var content = new StringContent(JsonConvert.SerializeObject(registerData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            throw new Exception("Đăng ký thất bại. Vui lòng kiểm tra lại thông tin.");
        }

        // Lưu JWT Token trong bộ nhớ hoặc session
        private void SaveToken(string token)
        {
            // Ví dụ: lưu vào session (nếu đang sử dụng ASP.NET MVC)
            System.Web.HttpContext.Current.Session["JWTToken"] = token;

            // Nếu lưu ở nơi khác (cookie, local storage, v.v.), tùy theo yêu cầu của bạn
        }

        // Thiết lập JWT Token cho các lần gọi tiếp theo
        private void SetTokenHeader()
        {
            var token = System.Web.HttpContext.Current.Session["JWTToken"] as string;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
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

        // Mô hình phản hồi của Token
        private class TokenResponse
        {
            public string Token { get; set; }
        }
    }
}

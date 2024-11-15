using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace VanPhongPham.Services
{
    public class GHNService
    {
        private readonly string _apiKey;

        public GHNService()
        {
            _apiKey = FirebaseService.GetApiKey();
        }

        public async Task<(bool isDelivered, DateTime? finishDate)> IsOrderDeliveredAsync(string orderCode)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Tạo một HttpRequestMessage với phương thức GET và URL yêu cầu
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/detail?order_code={orderCode}");

                    // Thêm header Token vào yêu cầu
                    requestMessage.Headers.Add("Token", _apiKey);

                    // Gửi yêu cầu và nhận phản hồi
                    var response = await client.SendAsync(requestMessage);

                    // Kiểm tra xem yêu cầu có thành công không
                    if (!response.IsSuccessStatusCode)
                    {
                        // Nếu yêu cầu không thành công, log thông tin lỗi
                        string errorContent = await response.Content.ReadAsStringAsync();
                        return (false, null);
                    }

                    // Đọc nội dung phản hồi
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(content);

                    // Kiểm tra và lấy finish_date nếu có
                    if (result?.data?.status == "delivered")
                    {
                        string finishDateString = result?.data?.finish_date?.ToString();

                        // Chuyển đổi từ định dạng ISO 8601 sang DateTime
                        DateTime finishDate;

                        if (DateTime.TryParse(finishDateString, out finishDate))
                        {
                            // Nếu là UTC, chuyển thành giờ địa phương nếu cần
                            finishDate = finishDate.ToLocalTime();

                            return (true, finishDate);
                        }
                        else
                        {
                            // Nếu không chuyển đổi được, log thông báo lỗi
                            Console.WriteLine($"Failed to parse finish date: {finishDateString}");
                        }
                    }

                    // Nếu không có trạng thái "delivered", trả về false
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có exception
                Console.WriteLine($"Exception: {ex.Message}");
                return (false, null);
            }
        }
    }
}
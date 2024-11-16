using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;

namespace VanPhongPham.Services
{
    public class FirebaseService
    {
        private static FirebaseApp firebaseApp;
        private static string apiKey;

        // Khởi tạo Firebase App
        public static void Initialize()
        {
            if (firebaseApp == null)
            {
                firebaseApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(HttpContext.Current.Server.MapPath("~/App_Data/firebase-config.json"))
                });
            }

            // Lấy API Key từ cấu hình
            apiKey = ConfigurationManager.AppSettings["FirebaseApiKey"];
        }

        // Phương thức lấy API Key
        public static string GetApiKey()
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("API Key chưa được cấu hình.");
            }

            return apiKey;
        }

        // Phương thức xác minh token của người dùng
        public static async Task<FirebaseToken> VerifyTokenAsync(string token)
        {
            return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        }
    }
}
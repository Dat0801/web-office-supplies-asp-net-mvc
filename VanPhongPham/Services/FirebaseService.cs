using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace VanPhongPham.Services
{
    public class FirebaseService
    {
        private static FirebaseApp firebaseApp;
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
        }

        // Phương thức xác minh token của người dùng
        public static async Task<FirebaseToken> VerifyTokenAsync(string token)
        {
            return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        }
    }
}
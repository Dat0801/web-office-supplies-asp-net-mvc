using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using System.Data.Linq;
using FirebaseAdmin.Auth.Hash;
using System.Data.SqlTypes;

namespace VanPhongPham.Models
{
    public class UserRepository
    {
        private readonly DB_VanPhongPhamDataContext _context;
        public UserRepository()
        {
            _context = new DB_VanPhongPhamDataContext();
        }
        public List<user> GetAllUsers()
        {
            //DataLoadOptions options = new DataLoadOptions();
            //options.LoadWith<user>(u => u.addresses);
            //_context.LoadOptions = options;
            return _context.users.Where(s => s.status == true).ToList();
        }
        public List<user> GetDeletedUsers()
        {
            //DataLoadOptions options = new DataLoadOptions();
            //options.LoadWith<user>(u => u.addresses);
            //_context.LoadOptions = options;
            return _context.users.Where(s => s.status == false).ToList();
        }
        public user GetUserById(string id)
        {
            //DataLoadOptions options = new DataLoadOptions();
            //options.LoadWith<user>(u => u.addresses);
            //_context.LoadOptions = options;
            return _context.users.FirstOrDefault(x => x.user_id == id);
        }
        public List<user> SearchUser(string search_str)
        {
            //DataLoadOptions options = new DataLoadOptions();
            //options.LoadWith<user>(u => u.addresses);
            //_context.LoadOptions = options;
            return _context.users
                .Where(p => p.status == true &&
                           (p.username.Contains(search_str) ||
                            p.user_id.Contains(search_str) ||
                            p.full_name.Contains(search_str) ||                            
                            p.email.Contains(search_str)))
                .ToList();
        }
        public List<user> SearchDeletedUser(string search_str)
        {
            return _context.users
                .Where(p => p.status == false &&
                           (p.username.Contains(search_str) ||
                            p.user_id.Contains(search_str) ||
                            p.full_name.Contains(search_str) ||
                            p.email.Contains(search_str)))
                .ToList();
        }

        public bool AddUser(user sup)
        {
            try
            {
                user existUser = _context.users.FirstOrDefault(u => u.username == sup.username || u.user_id == sup.user_id);
                if(sup.user_id == null)
                {
                    sup.user_id = GenerateUserId();
                }
                if(existUser != null)
                {
                    return false;
                }
                sup.status = true;                
                sup.password = HashPasswordMD5(sup.password);
                _context.users.InsertOnSubmit(sup);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
        public bool UpdateUser(user sup)
        {
            user userToUpdate = GetUserById(sup.user_id);
            if(userToUpdate == null)
            {
                return false;
            }
            try
            {
                userToUpdate.username = sup.username;
                userToUpdate.password = sup.password;
                userToUpdate.full_name = sup.full_name;
                userToUpdate.addresses = sup.addresses;
                userToUpdate.email = sup.email;
                userToUpdate.dob = sup.dob;
                userToUpdate.gender = sup.gender;                
                userToUpdate.avt_url = sup.avt_url;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public bool DeleteUsers(List<string> ids)
        {
            try
            {
                var usersToDelete = _context.users.Where(s => ids.Contains(s.user_id)).ToList();
                usersToDelete.ForEach(s => s.status = false);

                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool RecoverUsers(List<string> ids)
        {
            try
            {
                var usersToRecover = _context.users.Where(s => ids.Contains(s.user_id)).ToList();
                usersToRecover.ForEach(s => s.status = true);

                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public string GenerateUserId()
        {
            user us = _context.users.Where(u => u.status == true).ToList().LastOrDefault();
            if(us == null)
            {
                return "USER001";
            }
            else
            { 
                int num = int.Parse(us.user_id.Substring(4)) + 1;
                string user_id = "USER";
                if (num < 10)
                    user_id = "USER00";
                else if (num < 100)
                    user_id = "USER0";
                user_id += num;
                return user_id;
            }
        }
        public user CheckLoginAdmin(string username, string password)
        {
            user_role adminRole = _context.user_roles.FirstOrDefault(r => r.role_id == 2);
            string hashedPassword = HashPasswordMD5(password);
            return _context.users.FirstOrDefault(u => u.username == username && u.password == hashedPassword && u.user_id == adminRole.user_id);
        }

        private string HashPasswordMD5(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                // Chuyển mảng byte sang chuỗi hexadecimal
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // Chuyển đổi mỗi byte thành 2 ký tự hexadecimal
                }
                return sb.ToString();
            }
        }

    }
}
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
        public List<RoleViewModel> GetAllRoles()
        {
            return (from r in _context.roles
                    where r.role_id != 2
                    select new RoleViewModel
                    {
                        RoleId = r.role_id,
                        RoleName = r.role_name
                    }).ToList();
        }

        public List<UserViewModel> GetAllUsersWithAddresses()
        {
            var usersWithAddresses = (from u in _context.users
                                      where u.status == true
                                      let userRoles = (from ur in _context.user_roles
                                                       where ur.user_id == u.user_id && ur.role_id != 2
                                                       select ur.role_id).Any()
                                      where userRoles
                                      select new UserViewModel
                                      {
                                          UserId = u.user_id,
                                          FullName = u.full_name,
                                          Username = u.username,
                                          Email = u.email,
                                          Gender = u.gender,
                                          Dob = u.dob,
                                          Status = (bool)u.status,
                                          AvatarUrl = u.avt_url,
                                          Addresses = (from a in _context.addresses
                                                       where a.user_id == u.user_id
                                                       select new AddressViewModel
                                                       {
                                                           AddressId = a.address_id,
                                                           UserId = a.user_id,
                                                           FullName = a.full_name,
                                                           PhoneNumber = a.phone_number,
                                                           AddressLine = a.address_line,
                                                           Ward = a.ward,
                                                           District = a.district,
                                                           Province = a.province,
                                                           isDefault = (bool)a.isDefault
                                                       }).ToList(),
                                          Roles = (from ur in _context.user_roles
                                                   join r in _context.roles on ur.role_id equals r.role_id
                                                   where ur.user_id == u.user_id && ur.role_id != 2
                                                   select r.role_name).ToList()
                                      }).ToList();

            return usersWithAddresses;
        }


        public List<UserViewModel> GetDeletedUsers()
        {
            return (from u in _context.users
                    where u.status == false
                    let userRoles = (from ur in _context.user_roles
                                     where ur.user_id == u.user_id && ur.role_id != 2
                                     select ur.role_id).Any()
                    where userRoles
                    select new UserViewModel
                    {
                        UserId = u.user_id,
                        FullName = u.full_name,
                        Username = u.username,
                        Email = u.email,
                        Gender = u.gender,
                        Dob = u.dob,
                        Status = (bool)u.status,
                        AvatarUrl = u.avt_url,
                        Addresses = (from a in _context.addresses
                                     where a.user_id == u.user_id
                                     select new AddressViewModel
                                     {
                                         AddressId = a.address_id,
                                         UserId = a.user_id,
                                         FullName = a.full_name,
                                         PhoneNumber = a.phone_number,
                                         AddressLine = a.address_line,
                                         Ward = a.ward,
                                         District = a.district,
                                         Province = a.province,
                                         isDefault = (bool)a.isDefault
                                     }).ToList(),
                        Roles = (from ur in _context.user_roles
                                 join r in _context.roles on ur.role_id equals r.role_id
                                 where ur.user_id == u.user_id && ur.role_id != 2
                                 select r.role_name).ToList()
                    }).ToList();
        }

        public UserViewModel GetUserById(string id)
        {
            return (from u in _context.users
                    where u.user_id == id
                    let userRoles = (from ur in _context.user_roles
                                     where ur.user_id == u.user_id && ur.role_id != 2
                                     select ur.role_id).Any()
                    where userRoles
                    select new UserViewModel
                    {
                        UserId = u.user_id,
                        FullName = u.full_name,
                        Username = u.username,
                        Password = u.password,
                        Email = u.email,
                        Gender = u.gender,
                        Dob = u.dob,
                        Status = (bool)u.status,
                        AvatarUrl = u.avt_url,
                        Addresses = (from a in _context.addresses
                                     where a.user_id == u.user_id
                                     select new AddressViewModel
                                     {
                                         AddressId = a.address_id,
                                         UserId = a.user_id,
                                         FullName = a.full_name,
                                         PhoneNumber = a.phone_number,
                                         AddressLine = a.address_line,
                                         Ward = a.ward,
                                         District = a.district,
                                         Province = a.province,
                                         isDefault = (bool)a.isDefault
                                     }).ToList(),
                        Roles = (from ur in _context.user_roles
                                 join r in _context.roles on ur.role_id equals r.role_id
                                 where ur.user_id == u.user_id && ur.role_id != 2
                                 select r.role_name).ToList()
                    }).FirstOrDefault();
        }

        // Tìm kiếm người dùng theo chuỗi tìm kiếm
        public List<UserViewModel> SearchUser(string search_str)
        {
            return (from u in _context.users
                    where u.status == true &&
                          (u.username.Contains(search_str) ||
                           u.user_id.Contains(search_str) ||
                           u.full_name.Contains(search_str) ||
                           u.email.Contains(search_str))
                    let userRoles = (from ur in _context.user_roles
                                     where ur.user_id == u.user_id && ur.role_id != 2
                                     select ur.role_id).Any()
                    where userRoles
                    select new UserViewModel
                    {
                        UserId = u.user_id,
                        FullName = u.full_name,
                        Username = u.username,
                        Email = u.email,
                        Gender = u.gender,
                        Dob = u.dob,
                        Status = (bool)u.status,
                        AvatarUrl = u.avt_url,
                        Addresses = (from a in _context.addresses
                                     where a.user_id == u.user_id
                                     select new AddressViewModel
                                     {
                                         AddressId = a.address_id,
                                         UserId = a.user_id,
                                         FullName = a.full_name,
                                         PhoneNumber = a.phone_number,
                                         AddressLine = a.address_line,
                                         Ward = a.ward,
                                         District = a.district,
                                         Province = a.province,
                                         isDefault = (bool)a.isDefault
                                     }).ToList(),
                        Roles = (from ur in _context.user_roles
                                 join r in _context.roles on ur.role_id equals r.role_id
                                 where ur.user_id == u.user_id && ur.role_id != 2
                                 select r.role_name).ToList()
                    }).ToList();
        }
        public List<UserViewModel> SearchDeletedUser(string search_str)
        {
            return (from u in _context.users
                    where u.status == false &&
                          (u.username.Contains(search_str) ||
                           u.user_id.Contains(search_str) ||
                           u.full_name.Contains(search_str) ||
                           u.email.Contains(search_str))
                    let userRoles = (from ur in _context.user_roles
                                     where ur.user_id == u.user_id && ur.role_id != 2
                                     select ur.role_id).Any()
                    where userRoles
                    select new UserViewModel
                    {
                        UserId = u.user_id,
                        FullName = u.full_name,
                        Username = u.username,
                        Email = u.email,
                        Gender = u.gender,
                        Dob = u.dob,
                        Status = (bool)u.status,
                        AvatarUrl = u.avt_url,
                        Addresses = (from a in _context.addresses
                                     where a.user_id == u.user_id
                                     select new AddressViewModel
                                     {
                                         AddressId = a.address_id,
                                         UserId = a.user_id,
                                         FullName = a.full_name,
                                         PhoneNumber = a.phone_number,
                                         AddressLine = a.address_line,
                                         Ward = a.ward,
                                         District = a.district,
                                         Province = a.province,
                                         isDefault = (bool)a.isDefault
                                     }).ToList(),
                        Roles = (from ur in _context.user_roles
                                 join r in _context.roles on ur.role_id equals r.role_id
                                 where ur.user_id == u.user_id && ur.role_id != 2
                                 select r.role_name).ToList()
                    }).ToList();
        }

        // Thêm người dùng mới
        public bool AddUser(UserViewModel userViewModel, List<int> roleIds)
        {
            try
            {                
                var existingUser = _context.users.FirstOrDefault(u => u.username == userViewModel.Username || u.user_id == userViewModel.UserId);
                if (existingUser != null)
                {
                    return false;
                }

                var newUser = new user
                {
                    user_id = userViewModel.UserId ?? GenerateUserId(),
                    username = userViewModel.Username,
                    password = HashPasswordMD5(userViewModel.Password),
                    full_name = userViewModel.FullName,
                    email = userViewModel.Email,
                    gender = userViewModel.Gender,
                    dob = userViewModel.Dob,
                    status = true,
                    avt_url = userViewModel.AvatarUrl
                };

                _context.users.InsertOnSubmit(newUser);
               
                foreach (var roleId in roleIds)
                {
                    var userRole = new user_role
                    {
                        user_id = newUser.user_id,
                        role_id = roleId
                    };
                    _context.user_roles.InsertOnSubmit(userRole);
                }
                
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        // Cập nhật người dùng
        public bool UpdateUser(UserViewModel userViewModel, List<int> roleIds)
        {
            var modelPassword = userViewModel.Password;
            var userToUpdate = _context.users.FirstOrDefault(u => u.user_id == userViewModel.UserId);
            if (userToUpdate == null)
            {
                return false;
            }
            if(modelPassword == null || modelPassword == "" || modelPassword == userToUpdate.password)
            {
                userViewModel.Password = userToUpdate.password;
            }
            else
            {
                userViewModel.Password = HashPasswordMD5(userViewModel.Password);
            }
            try
            {                
                userToUpdate.username = userViewModel.Username;
                userToUpdate.password = userViewModel.Password;
                userToUpdate.full_name = userViewModel.FullName;
                userToUpdate.email = userViewModel.Email;
                userToUpdate.dob = userViewModel.Dob;
                userToUpdate.gender = userViewModel.Gender;
                userToUpdate.avt_url = userViewModel.AvatarUrl;
                
                var existingRoles = _context.user_roles.Where(ur => ur.user_id == userToUpdate.user_id);
                _context.user_roles.DeleteAllOnSubmit(existingRoles);
                
                foreach (var roleId in roleIds)
                {
                    var userRole = new user_role
                    {
                        user_id = userToUpdate.user_id,
                        role_id = roleId
                    };
                    _context.user_roles.InsertOnSubmit(userRole);
                }
                
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
            if (us == null)
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
            string hashedPassword = HashPasswordMD5(password);
            return _context.users.FirstOrDefault(u => u.username == username && u.password == hashedPassword && u.status == true);
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
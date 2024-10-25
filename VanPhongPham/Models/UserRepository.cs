using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
            return _context.users.ToList();
        }
        public user GetUserById(int id)
        {
            return _context.users.FirstOrDefault(x => x.user_id == id);
        }
        public void AddUser(user user)
        {
            _context.users.InsertOnSubmit(user);
            _context.SubmitChanges();
        }
        public void UpdateUser(user user)
        {
            user userToUpdate = GetUserById(user.user_id);
            userToUpdate.username = user.username;
            userToUpdate.password = user.password;
            userToUpdate.email = user.email;
            userToUpdate.phone_number = user.phone_number;
            userToUpdate.addresses = user.addresses;
            userToUpdate.user_roles = user.user_roles;
            _context.SubmitChanges();
        }
        public void DeleteUser(int id)
        {
            user userToDelete = GetUserById(id);
            _context.users.DeleteOnSubmit(userToDelete);
            _context.SubmitChanges();
        }
    }
}
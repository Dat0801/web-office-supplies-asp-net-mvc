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
        public user GetUserById(string id)
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
            
            _context.SubmitChanges();
        }
        public void DeleteUser(string id)
        {
            user userToDelete = GetUserById(id);
            _context.users.DeleteOnSubmit(userToDelete);
            _context.SubmitChanges();
        }
        public user CheckLoginAdmin(string username, string password)
        {
            return _context.users.FirstOrDefault(u => u.username == username && u.password == password);
        }
    }
}
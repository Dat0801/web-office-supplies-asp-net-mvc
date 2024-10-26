using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanPhongPham.Models
{
    public class ProductRepository
    {
        private readonly DB_VanPhongPhamDataContext _context;

        public ProductRepository()
        {
            _context = new DB_VanPhongPhamDataContext();
        }

        public List<product> GetAllProduct()
        {
            return _context.products.ToList();
        }

        public List<category> GetAllCategory()
        {
            return _context.categories.ToList();
        }

        public string GenerateCategoryId()
        {
            category category = _context.categories.ToList().LastOrDefault();
            int num = int.Parse(category.category_id.Substring(3)) + 1;
            string category_id = "CAT";
            if (num < 10)
                category_id = "CAT00";
            else if (num < 100)
                category_id = "CAT0";
            category_id += num;
            return category_id;
        }

        public bool AddCategory(category category)
        {
            try
            {
                _context.categories.InsertOnSubmit(category);
                _context.SubmitChanges();
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteCategory(string pcategory_id)
        {
            category category = _context.categories.FirstOrDefault(cat => cat.category_id == pcategory_id);
            try
            {
                _context.categories.DeleteOnSubmit(category);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateCategory(category pcategory)
        {
            category category = _context.categories.FirstOrDefault(cat => cat.category_id == pcategory.category_id);
            try
            {
                category.category_name = pcategory.category_name;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public category GetCategory(string category_id)
        {
            return _context.categories.FirstOrDefault(cat => cat.category_id == category_id);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
        //public List<product> GetAllProductsWithImages()
        //{            
        //    var productsWithImages = from p in _context.products
        //                                where p.status == true
        //                                select new product
        //                                {
        //                                    product_id = p.product_id,
        //                                    product_name= p.product_name,
        //                                    description = p.description,
        //                                    purchase_price= p.purchase_price,
        //                                    price = p.price,
        //                                    promotion_price= p.promotion_price,
        //                                    stock_quantity = p.stock_quantity,
        //                                    images = p.images.ToList(),
        //                                };

        //    return productsWithImages.ToList();
            
        //}
        public List<category> GetAllCategory()
        {
            return _context.categories.Where(c => c.status == true).ToList();
        }

        public string GenerateProductId()
        {
            product product = _context.products.ToList().LastOrDefault();
            int num = int.Parse(product.product_id.Substring(3)) + 1;
            string product_id = "PRO";
            if (num < 10)
                product_id = "PRO00";
            else if (num < 100)
                product_id = "PRO0";
            product_id += num;
            return product_id;
        }

        public List<product> SearchProduct(string search_str)
        {
            return _context.products
                .Where(p => p.status == true &&
                            (p.product_name.Contains(search_str) ||
                            p.product_id.Contains(search_str) ||
                            p.category.category_name.Contains(search_str)))   
                .ToList();
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
            }
            catch (Exception ex)
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

        public List<category> SearchCategory(string search_str)
        {
            return _context.categories
                .Where(c => c.status == true &&
                            (c.category_name.Contains(search_str) ||
                            c.category_id.Contains(search_str)))
                .ToList();
        }
    }
}
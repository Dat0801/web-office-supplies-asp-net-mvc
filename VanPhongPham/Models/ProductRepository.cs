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

        public List<product> GetProducts()
        {
            return _context.products.Where(p => p.status == true).ToList();
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

        public bool AddProduct(product product)
        {
            try
            {
                _context.products.InsertOnSubmit(product);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteProduct(string p_product_id)
        {
            product product = _context.products.FirstOrDefault(p => p.product_id == p_product_id);
            try
            {
                product.status = false;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateProduct(product p_product)
        {
            product product = _context.products.FirstOrDefault(p => p.product_id == p_product.product_id);
            try
            {
                product.product_name = p_product.product_name;
                product.category_id = p_product.category_id;
                product.description = p_product.description;
                product.price_coefficient = p_product.price_coefficient;
                product.status = p_product.status;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<product> SearchProduct(string search_str)
        {
            return _context.products
                .Where(p => p.product_name.Contains(search_str) ||
                            p.product_id.Contains(search_str) ||
                            p.category.category_name.Contains(search_str))   
                .ToList();
        }

        public string GenerateImageId()
        {
            image image = _context.images.ToList().LastOrDefault();
            int num = 1;
            if(image != null)
            {
                num = int.Parse(image.image_id.Substring(3)) + 1;
            }
            string image_id = "IMG";
            if (num < 10)
                image_id = "IMG00";
            else if (num < 100)
                image_id = "IMG0";
            image_id += num;
            return image_id;
        }

        public bool AddImages(image image)
        {
            try
            {
                _context.images.InsertOnSubmit(image);
                _context.SubmitChanges();
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<category> GetCategories()
        {
            return _context.categories.ToList();
        }

        public category GetCategory(string category_id)
        {
            return _context.categories.FirstOrDefault(cat => cat.category_id == category_id);
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

        public List<category> SearchCategory(string search_str)
        {
            return _context.categories
                .Where(c => c.category_name.Contains(search_str) ||
                            c.category_id.Contains(search_str))
                .ToList();
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

        public List<attribute> GetAttributes()
        {
            return _context.attributes.ToList();
        }

        public bool AddAttribute(attribute attribute)
        {
            try
            {
                _context.attributes.InsertOnSubmit(attribute);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public string GenerateAttributeId()
        {
            attribute attribute = _context.attributes.ToList().LastOrDefault();
            int num = int.Parse(attribute.attribute_id.Substring(3)) + 1;
            string attribute_id = "ATT";
            if (num < 10)
                attribute_id = "ATT00";
            else if (num < 100)
                attribute_id = "ATT0";
            attribute_id += num;
            return attribute_id;
        }

        public List<attribute_value> GetAttributeValues()
        {
            return _context.attribute_values.ToList();
        }

        public bool AddAttributeValue(attribute_value attribute_Value)
        {
            try
            {
                _context.attribute_values.InsertOnSubmit(attribute_Value);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateAttributeValue(attribute_value p_attribute_Value)
        {
            attribute_value attribute_Value = _context.attribute_values.FirstOrDefault(att => att.attribute_value_id == p_attribute_Value.attribute_value_id);
            try
            {
                attribute_Value.attribute_id = p_attribute_Value.attribute_id;
                attribute_Value.value = p_attribute_Value.value;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteAttributeValue(string attribute_value_id)
        {
            attribute_value attribute_Value = _context.attribute_values.FirstOrDefault(att => att.attribute_value_id == attribute_value_id);

            try
            {
                _context.attribute_values.DeleteOnSubmit(attribute_Value);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public string GenerateAttributeValueId()
        {
            attribute_value attribute_Value = _context.attribute_values.ToList().LastOrDefault();
            int num = int.Parse(attribute_Value.attribute_value_id.Substring(3)) + 1;
            string attribute_value_id = "VAL";
            if (num < 10)
                attribute_value_id = "VAL00";
            else if (num < 100)
                attribute_value_id = "VAL0";
            attribute_value_id += num;
            return attribute_value_id;
        }

        public bool AddProductAttributeValue(product_attribute_value product_Attribute_Value)
        {
            try
            {
                _context.product_attribute_values.InsertOnSubmit(product_Attribute_Value);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
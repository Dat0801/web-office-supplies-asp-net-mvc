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

        public List<product> GetProducts()
        {
            return _context.products.Where(p => p.status == true).ToList();
        }

        public product GetProduct(string product_id)
        {
            return _context.products.FirstOrDefault(p => p.status == true && p.product_id == product_id);
        }

        public List<product> GetRecycleProducts()
        {
            return _context.products.Where(p => p.status == false).ToList();
        }

        public bool RecycleProduct(string product_id)
        {
            product product = _context.products.FirstOrDefault(p => p.product_id == product_id);
            try
            {
                product.status = true;
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

        public bool AddProduct(product product)
        {
            try
            {
                if (product.created_at == null)
                {
                    product.created_at = DateTime.Now;
                }

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

        /*public string GenerateImageId()
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
        }*/

        public image GetMainImageByProductId(string product_id)
        {
            return _context.images.FirstOrDefault(img => img.product_id == product_id && img.is_primary == true);
        }

        public List<image> GetAdditionalImageByProductId(string product_id)
        {
            return _context.images.Where(img => img.product_id == product_id && img.is_primary == false).ToList();
        }

        public image GetMainImageByProductUrl(string url)
        {
            return _context.images.FirstOrDefault(img => img.image_url == url);
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

        public bool UpdateImage(image pimage)
        {
            image image = _context.images.FirstOrDefault(img => img.product_id == pimage.product_id);
            try
            {
                image.image_url = pimage.image_url;
                image.is_primary = pimage.is_primary;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteImage(string image_url)
        {
            image image = _context.images.FirstOrDefault(img => img.image_url == image_url);
            try
            {
                _context.images.DeleteOnSubmit(image);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateAdditionalImages(string productId, List<string> newImageUrls)
        {
            try
            {
                // Lấy danh sách các hình ảnh phụ hiện tại của sản phẩm
                var existingImages = GetAdditionalImageByProductId(productId);

                // Xóa hình ảnh phụ không còn trong danh sách mới
                foreach (var image in existingImages.ToList())
                {
                    if (!newImageUrls.Contains(image.image_url))
                    {
                        DeleteImage(image.image_url);
                    }
                }

                // Thêm hình ảnh mới vào danh sách
                foreach (var newImageUrl in newImageUrls)
                {
                    if (!existingImages.Any(img => img.image_url == newImageUrl))
                    {
                        // Nếu hình ảnh mới chưa tồn tại trong danh sách, thêm vào
                        image additionalImage = new image
                        {
                            image_url = newImageUrl,
                            is_primary = false,
                            product_id = productId

                        };
                        AddImages(additionalImage);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
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

        public List<category> GetCategories()
        {
            return _context.categories.ToList();
        }

        public category GetCategory(string category_id)
        {
            return _context.categories.FirstOrDefault(cat => cat.category_id == category_id);
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

        public List<attribute> GetAttributes()
        {
            return _context.attributes.ToList();
        }

        public attribute GetAttribute(string attribute_id)
        {
            return _context.attributes.FirstOrDefault(att => att.attribute_id == attribute_id);
        }

        public List<attribute> SearchAttribute(string search_str)
        {
            return _context.attributes
                .Where(c => c.attribute_name.Contains(search_str) ||
                            c.attribute_id.Contains(search_str))
                .ToList();
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

        public bool UpdateAttribute(attribute p_attribute)
        {
            attribute attribute = _context.attributes.FirstOrDefault(att => att.attribute_id == p_attribute.attribute_id);
            try
            {
                attribute.attribute_id = p_attribute.attribute_id;
                attribute.attribute_name = p_attribute.attribute_name;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteAttribute(string pattribute_id)
        {
            attribute attribute = _context.attributes.FirstOrDefault(att => att.attribute_id == pattribute_id);
            try
            {
                _context.attributes.DeleteOnSubmit(attribute);
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<attribute_value> GetAttributeValues()
        {
            return _context.attribute_values.ToList();
        }

        public attribute_value GetAttributeValue(string attribute_value_id)
        {
            return _context.attribute_values.FirstOrDefault(att_val => att_val.attribute_value_id == attribute_value_id);
        }

        public List<attribute_value> GetAttributeValueByProduct(string product_id)
        {
            var attributeValueIds = _context.product_attribute_values
                .Where(pav => pav.product_id == product_id)
                .Select(pav => pav.attribute_value_id)
                .ToList();
            var attributeValues = _context.attribute_values
                .Where(av => attributeValueIds.Contains(av.attribute_value_id))
                .ToList();
            return attributeValues;
        }

        public List<attribute_value> SearchAttributeValue(string search_str)
        {
            return _context.attribute_values
                .Where(c => c.value.Contains(search_str) ||
                            c.attribute_value_id.Contains(search_str))
                .ToList();
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
                attribute_Value.attribute_value_id = p_attribute_Value.attribute_value_id;
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

        public List<product_attribute_value> GetProductAttributeValueByProductId(string productId)
        {
            return _context.product_attribute_values.Where(att => att.product_id == productId).ToList();
        }

        public bool UpdateProductAttributeValue(string productId, List<string> newAttributeValueIds)
        {
            try
            {
                // Lấy danh sách các `attribute_value_id` hiện tại của sản phẩm
                var existingAttributeValues = GetProductAttributeValueByProductId(productId);

                // Xóa các `attribute_value_id` không còn trong danh sách mới
                foreach (var attributeValue in existingAttributeValues)
                {
                    if (!newAttributeValueIds.Contains(attributeValue.attribute_value_id))
                    {
                        _context.product_attribute_values.DeleteOnSubmit(attributeValue);
                    }
                }

                // Thêm `attribute_value_id` mới vào nếu chưa có trong danh sách
                foreach (var newAttributeValueId in newAttributeValueIds)
                {
                    if (!existingAttributeValues.Any(att => att.attribute_value_id == newAttributeValueId))
                    {
                        _context.product_attribute_values.InsertOnSubmit(
                            new product_attribute_value
                            {
                                product_id = productId,
                                attribute_value_id = newAttributeValueId
                            });
                    }
                }
                // Lưu các thay đổi
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
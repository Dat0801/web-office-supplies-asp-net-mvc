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
            return _context.products.Where(p => p.status == true && p.category.status == true).ToList();
        }

        public product GetProduct(string product_id)
        {
            return _context.products.FirstOrDefault(p => p.status == true && p.product_id == product_id);
        }

        public List<product> GetProductByCategoryName(string category_name)
        {
            return _context.products.Where(p => p.category.category_name == category_name && p.status == true).ToList();
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
                product.price = string.IsNullOrEmpty(product.price.ToString()) ? 0 : product.price;
                product.purchase_price = string.IsNullOrEmpty(product.purchase_price.ToString()) ? 0 : product.purchase_price;
                product.stock_quantity = string.IsNullOrEmpty(product.stock_quantity.ToString()) ? 0 : product.stock_quantity;
                product.created_at = string.IsNullOrEmpty(product.created_at.ToString()) ? DateTime.Now : product.created_at;

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
            }
            catch (Exception ex)
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
                var existingImages = GetAdditionalImageByProductId(productId);

                foreach (var image in existingImages.ToList())
                {
                    if (!newImageUrls.Contains(image.image_url))
                    {
                        DeleteImage(image.image_url);
                    }
                }

                foreach (var newImageUrl in newImageUrls)
                {
                    if (!existingImages.Any(img => img.image_url == newImageUrl))
                    {
                        image additionalImage = new image
                        {
                            image_url = newImageUrl,
                            is_primary = false,
                            product_id = productId

                        };
                        //AddImages(additionalImage);
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
            return _context.categories.Where(c => c.status == true).ToList();
        }

        public category GetCategory(string category_id)
        {
            return _context.categories.FirstOrDefault(cat => cat.category_id == category_id);
        }

        public category GetCategoryByName(string category_name)
        {
            return _context.categories.FirstOrDefault(cat => cat.category_name == category_name);
        }

        public List<category> SearchCategory(string search_str)
        {
            return _context.categories
                .Where(c => c.category_name.Contains(search_str) ||
                            c.category_id.Contains(search_str))
                .ToList();
        }

        public List<category> GetRecycleCategories()
        {
            return _context.categories.Where(c => c.status == false).ToList();
        }

        public bool RecycleCategory(string category_id)
        {
            category category = _context.categories.FirstOrDefault(c => c.category_id == category_id);
            try
            {
                category.status = true;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public category GetRecycleCategory(string category_name)
        {
            return _context.categories.FirstOrDefault(c => c.category_name == category_name && c.status == false);
        }

        public bool AddCategory(category category)
        {
            try
            {
                category.status = (category.status == null) ? true : category.status;
                category.created_at = (category.created_at == null) ? DateTime.Now : category.created_at;
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
                category.status = false;
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
            return _context.attributes.Where(att => att.status == true).ToList();
        }

        public attribute GetAttribute(string attribute_id)
        {
            return _context.attributes.FirstOrDefault(att => att.attribute_id == attribute_id);
        }

        public attribute GetAttributeByName(string attribute_name)
        {
            return _context.attributes.FirstOrDefault(att => att.attribute_name == attribute_name);
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
                attribute.status = (attribute.status == null) ? true : attribute.status;
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
                attribute.status = false;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<attribute> GetRecycleAttributes()
        {
            return _context.attributes.Where(c => c.status == false).ToList();
        }

        public bool RecycleAttribute(string attribute_id)
        {
            attribute attribute = _context.attributes.FirstOrDefault(att => att.attribute_id == attribute_id);
            try
            {
                attribute.status = true;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public attribute GetRecycleAttribute(string attribute_name)
        {
            return _context.attributes.FirstOrDefault(att => att.attribute_name == attribute_name && att.status == false);
        }

        public List<attribute_value> GetAttributeValues()
        {
            return _context.attribute_values.Where(att => att.status == true && att.attribute.status == true).ToList();
        }

        public attribute_value GetAttributeValue(string attribute_value_id)
        {
            return _context.attribute_values.FirstOrDefault(att_val => att_val.attribute_value_id == attribute_value_id);
        }

        public attribute_value GetAttributeValueByValue(string value)
        {
            return _context.attribute_values.FirstOrDefault(att => att.value == value);
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
                attribute_Value.status = (attribute_Value.status == null) ? true : attribute_Value.status;
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
                attribute_Value.status = false;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<attribute_value> GetRecycleAttributeValues()
        {
            return _context.attribute_values.Where(att => att.status == false).ToList();
        }

        public bool RecycleAttributeValue(string attribute_value_id)
        {
            attribute_value attribute_Value = _context.attribute_values.FirstOrDefault(att => att.attribute_value_id == attribute_value_id);
            try
            {
                attribute_Value.status = true;
                _context.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public attribute_value GetRecycleAttributeValue(string value)
        {
            return _context.attribute_values.FirstOrDefault(att => att.value == value && att.status == false);
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
            return _context.product_attribute_values.Where(att => att.product_id == productId && att.attribute_value.status == true).ToList();
        }

        public bool UpdateProductAttributeValue(string productId, List<string> newAttributeValueIds)
        {
            try
            {
                var existingAttributeValues = GetProductAttributeValueByProductId(productId);

                foreach (var attributeValue in existingAttributeValues)
                {
                    if (!newAttributeValueIds.Contains(attributeValue.attribute_value_id))
                    {
                        _context.product_attribute_values.DeleteOnSubmit(attributeValue);
                    }
                }

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
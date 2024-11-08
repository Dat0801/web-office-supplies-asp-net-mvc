using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;
using PagedList;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VanPhongPham.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductRepository productRepository;
        public ProductController()
        {
            productRepository = new ProductRepository();
        }
        public ActionResult Index(int? page, string search_str)
        {
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            List<product> listProduct;
            if (search_str != null)
            {
                listProduct = productRepository.SearchProduct(search_str);
                ViewBag.search_str = search_str;
            }
            else
            {
                listProduct = productRepository.GetProducts();
            }
            return View(listProduct.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            ViewBag.product_id = productRepository.GenerateProductId();
            ViewBag.categories = productRepository.GetCategories();
            ViewBag.attributes = productRepository.GetAttributes();
            var attributeValues = productRepository.GetAttributeValues()
                                                    .Select(av => new AttributeValueDTO
                                                    {
                                                        Attribute_value_id = av.attribute_value_id,
                                                        Value = av.value,
                                                        Attribute_id = av.attribute_id
                                                    }).ToList();
            ViewBag.attribute_values = attributeValues;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public Task<JsonResult> AddProduct(product product, string[] attribute_value_id, string mainImageUrl, string additionalImageUrlsJson)
        {
            try
            {
                bool result = productRepository.AddProduct(product);
                List<string> additionalImageUrls = JsonConvert.DeserializeObject<List<string>>(additionalImageUrlsJson);
                image mainImage = new image
                {
                    image_url = mainImageUrl,
                    is_primary = true,
                    product_id = product.product_id
                };
                productRepository.AddImages(mainImage);
                foreach (var imageUrl in additionalImageUrls)
                {
                    image additionalImage = new image
                    {
                        image_url = imageUrl,
                        is_primary = false,
                        product_id = product.product_id

                    };
                    productRepository.AddImages(additionalImage);
                }
                foreach (var value_id in attribute_value_id)
                {
                    product_attribute_value product_Attribute_Value = new product_attribute_value
                    {
                        product_id = product.product_id,
                        attribute_value_id = value_id,
                        status = true
                    };
                    productRepository.AddProductAttributeValue(product_Attribute_Value);
                }
                return Task.FromResult(Json(new { success = true }));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(new { success = false, message = ex.Message }));
            }
        }

        public ActionResult UpdateProduct(string product_id)
        {
            product product = productRepository.GetProduct(product_id);
            ViewBag.categories = productRepository.GetCategories();
            ViewBag.attributes = productRepository.GetAttributes();
            var attributeValuesForProduct = productRepository.GetAttributeValueByProduct(product_id)
                                                                .Select(av => new AttributeValueDTO
                                                                {
                                                                    Attribute_value_id = av.attribute_value_id,
                                                                    Value = av.value,
                                                                    Attribute_id = av.attribute_id
                                                                }).ToList();
            ViewBag.attributeValuesForProduct = attributeValuesForProduct;
            var attributeValues = productRepository.GetAttributeValues()
                                                    .Select(av => new AttributeValueDTO
                                                    {
                                                        Attribute_value_id = av.attribute_value_id,
                                                        Value = av.value,
                                                        Attribute_id = av.attribute_id
                                                    }).ToList();
            ViewBag.attribute_values = attributeValues;
            var attributeIds = attributeValuesForProduct.Select(av => av.Attribute_id).Distinct();
            var attributesForProduct = productRepository.GetAttributes()
                                                        .Where(attr => attributeIds.Contains(attr.attribute_id))
                                                        .ToList();
            ViewBag.attributesForProduct = attributesForProduct;
            ViewBag.mainImage = productRepository.GetMainImageByProductId(product_id);
            ViewBag.additionalImages = productRepository.GetAdditionalImageByProductId(product_id);
            return View(product);
        }

        [HttpPost]
        [ValidateInput(false)]
        public Task<JsonResult> UpdateProduct(product product, string[] attribute_value_id, string mainImageUrl, string additionalImageUrlsJson)
        {
            try
            {
                bool result = productRepository.UpdateProduct(product);

                List<string> additionalImageUrls = JsonConvert.DeserializeObject<List<string>>(additionalImageUrlsJson);

                List<string> attributeValueIdsList = attribute_value_id.ToList();

                image mainImage = new image
                {
                    image_url = mainImageUrl,
                    is_primary = true,
                    product_id = product.product_id
                };

                productRepository.UpdateImage(mainImage);

                productRepository.UpdateAdditionalImages(product.product_id, additionalImageUrls);

                productRepository.UpdateProductAttributeValue(product.product_id, attributeValueIdsList);

                return Task.FromResult(Json(new { success = true }));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(new { success = false, message = ex.Message }));
            }
        }

        public ActionResult DeleteProduct(string product_id)
        {
            productRepository.DeleteProduct(product_id);
            return RedirectToAction("Index");
        }

        public ActionResult RecycleProductIndex()
        {
            List<product> listProduct = productRepository.GetRecycleProducts();
            return View(listProduct);
        }

        public ActionResult RecycleProduct(string product_id)
        {
            productRepository.RecycleProduct(product_id);
            return RedirectToAction("Index");
        }

        public ActionResult Category(int? page, string category_id, string search_str)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            List<category> listCategory;
            if (category_id != null)
            {
                category category = productRepository.GetCategory(category_id);
                ViewBag.category = category;
            }
            if (search_str != null)
            {
                listCategory = productRepository.SearchCategory(search_str);
                ViewBag.search_str = search_str;
            }
            else
            {
                listCategory = productRepository.GetCategories();
            }
            ViewBag.category_id = productRepository.GenerateCategoryId();
            return View(listCategory.ToPagedList(pageNumber, pageSize));
        }


        [HttpPost]
        public ActionResult AddCategory(string action, category category)
        {
            if (action == "add")
            {
                productRepository.AddCategory(category);
            }
            else
            {
                productRepository.UpdateCategory(category);
            }
            return RedirectToAction("Category", "/Product");
        }


        [HttpPost]
        public JsonResult AddCategoryAJAX(string categoryName)
        {
            category category = new category
            {
                category_id = productRepository.GenerateCategoryId(),
                category_name = categoryName
            };
            bool result = productRepository.AddCategory(category);
            if (result)
            {
                return Json(new { success = true, newCategoryId = category.category_id });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        public ActionResult DeleteCategory(string category_id)
        {
            productRepository.DeleteCategory(category_id);
            return RedirectToAction("Category");
        }

        public ActionResult Attribute(int? page, string attribute_id, string search_str)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            List<attribute> listAttribute;
            if (attribute_id != null)
            {
                attribute attribute = productRepository.GetAttribute(attribute_id);
                ViewBag.attribute = attribute;
            }
            if (search_str != null)
            {
                listAttribute = productRepository.SearchAttribute(search_str);
                ViewBag.search_str = search_str;
            }
            else
            {
                listAttribute = productRepository.GetAttributes();
            }
            ViewBag.attribute_id = productRepository.GenerateAttributeId();
            return View(listAttribute.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult AddAttribute(string action, attribute attribute)
        {
            if (action == "add")
            {
                productRepository.AddAttribute(attribute);
            }
            else
            {
                productRepository.UpdateAttribute(attribute);
            }
            return RedirectToAction("Attribute", "/Product");
        }

        [HttpPost]
        public JsonResult AddAttributeAJAX(string attributeName)
        {
            attribute attribute = new attribute
            {
                attribute_id = productRepository.GenerateAttributeId(),
                attribute_name = attributeName
            };
            bool result = productRepository.AddAttribute(attribute);
            if (result)
            {
                return Json(new { success = true, newAttributeId = attribute.attribute_id });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        public ActionResult DeleteAttribute(string attribute_id)
        {
            productRepository.DeleteAttribute(attribute_id);
            return RedirectToAction("Attribute");
        }

        public ActionResult AttributeValue(int? page, string attribute_value_id, string search_str)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            List<attribute_value> listAttributeValue;
            ViewBag.attributes = productRepository.GetAttributes();
            if (attribute_value_id != null)
            {
                attribute_value attribute_Value = productRepository.GetAttributeValue(attribute_value_id);
                ViewBag.attribute_value = attribute_Value;
            }
            if (search_str != null)
            {
                listAttributeValue = productRepository.SearchAttributeValue(search_str);
                ViewBag.search_str = search_str;
            }
            else
            {
                listAttributeValue = productRepository.GetAttributeValues();
            }
            ViewBag.attribute_value_id = productRepository.GenerateAttributeValueId();
            return View(listAttributeValue.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public JsonResult AddAttributeValueAJAX(string attribute_id, string value)
        {
            attribute_value attribute_Value = new attribute_value
            {
                attribute_value_id = productRepository.GenerateAttributeValueId(),
                attribute_id = attribute_id,
                value = value
            };
            bool result = productRepository.AddAttributeValue(attribute_Value);
            if (result)
            {
                return Json(new { success = true, newAttributeValueId = attribute_Value.attribute_value_id });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult AddAttributeValue(string action, attribute_value attribute_Value)
        {
            if (action == "add")
            {
                productRepository.AddAttributeValue(attribute_Value);
            }
            else
            {
                productRepository.UpdateAttributeValue(attribute_Value);
            }
            return RedirectToAction("AttributeValue", "/Product");
        }

        public ActionResult DeleteAttributeValue(string attribute_value_id)
        {
            productRepository.DeleteAttributeValue(attribute_value_id);
            return RedirectToAction("AttributeValue");
        }
    }
}
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
        public ActionResult Index(string search_str)
        {
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
            return View(listProduct);
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
                    image_id = productRepository.GenerateImageId(),
                    image_url = mainImageUrl,
                    is_primary = true,
                    product_id = product.product_id
                };
                productRepository.AddImages(mainImage);
                foreach (var imageUrl in additionalImageUrls)
                {
                    image additionalImage = new image
                    {
                        image_id = productRepository.GenerateImageId(),
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

        public ActionResult DeleteProduct(string product_id)
        {
            productRepository.DeleteProduct(product_id);
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

        [HttpPost]
        public JsonResult AddAttribute(string attributeName)
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

        [HttpPost]
        public JsonResult AddAttributeValue(string attribute_id, string value)
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
        public JsonResult UpdateAttributeValue(string attributeValueId, string attributeValue, string attributeId)
        {
            attribute_value attributeValueToUpdate = new attribute_value
            {
                attribute_value_id = attributeValueId,
                attribute_id = attributeId,
                value = attributeValue
            };

            bool result = productRepository.UpdateAttributeValue(attributeValueToUpdate);

            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public JsonResult DeleteAttributeValue(string attributeValueId)
        {
            bool result = productRepository.DeleteAttributeValue(attributeValueId);

            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
    }
}
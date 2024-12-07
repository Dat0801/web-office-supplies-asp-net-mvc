using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanPhongPham.Models;
using PagedList;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Drawing;
using VanPhongPham.Services;
using VanPhongPham.Areas.Admin.Filter;

namespace VanPhongPham.Areas.Admin.Controllers
{
    [Admin]
    public class ProductController : Controller
    {
        private readonly ProductRepository productRepository;
        private readonly ExcelReportService _excelReportService;

        public ProductController()
        {
            productRepository = new ProductRepository();
            _excelReportService = new ExcelReportService();
        }
        public ActionResult Index(int? page, string category_name, string search_str)
        {
            var message = TempData["Message"];
            var messageType = TempData["MessageType"];
            if (message != null)
            {
                ViewBag.Message = message;
                ViewBag.MessageType = messageType;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            List<product> listProduct;
            if (search_str != null)
            {
                listProduct = productRepository.SearchProduct(search_str);
                ViewBag.search_str = search_str;
            }
            else if (category_name != null)
            {
                listProduct = productRepository.GetProductByCategoryName(category_name);
                ViewBag.category_name = category_name;
            }
            else
            {
                listProduct = productRepository.GetProducts();
            }
            ViewBag.mainImages = productRepository.GetMainImages();
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
                                                    })
                                                    .OrderBy(av => av.Attribute_id)
                                                    .ThenBy(av => AttributeValueDTO.GetSortKey(av.Value))
                                                    .ToList();
            ViewBag.attribute_values = attributeValues;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public Task<JsonResult> AddProduct(product product, string[] attribute_value_id, string mainImageUrl, string additionalImageUrlsJson)
        {
            try
            {
                var ProductNameExist = productRepository.GetProductByName(product.product_name);
                var ProductNameRecycleExist = productRepository.GetRecycleProduct(product.product_name);

                if (ProductNameExist != null)
                {
                    return Task.FromResult(Json(new { success = false, message = "Tên sản phẩm đã tồn tại! Vui lòng nhập tên khác!" }));
                }

                if (ProductNameRecycleExist != null)
                {
                    return Task.FromResult(Json(new { success = false, message = "Sản phẩm có tên này đã bị xóa! Hãy khôi phục sản phẩm ở trang phục hồi!" }));
                }
                bool result = productRepository.AddProduct(product);
                if(!result)
                {
                    return Task.FromResult(Json(new { success = false, message = "Thêm thất bại" }));
                }
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
                if(attribute_value_id != null)
                {
                    foreach (var value_id in attribute_value_id)
                    {
                        product_attribute_value product_Attribute_Value = new product_attribute_value
                        {
                            product_id = product.product_id,
                            attribute_value_id = value_id,
                        };
                        productRepository.AddProductAttributeValue(product_Attribute_Value);
                    }
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
                                                    })
                                                    .OrderBy(av => av.Attribute_id)
                                                    .ThenBy(av => AttributeValueDTO.GetSortKey(av.Value))
                                                    .ToList();
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

                List<string> attributeValueIdsList = attribute_value_id?.ToList() ?? new List<string>();

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
            var result = productRepository.DeleteProduct(product_id);
            if (result)
            {
                TempData["Message"] = "Xóa sản phẩm thành công!";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "Xóa sản phẩm thất bại!";
                TempData["MessageType"] = "danger";
            }
            return RedirectToAction("Index");
        }

        public ActionResult RecycleProductIndex()
        {
            List<product> listProduct = productRepository.GetRecycleProducts();
            ViewBag.mainImages = productRepository.GetMainImages();
            return View(listProduct);
        }

        public ActionResult RecycleProduct(string product_id)
        {
            var result = productRepository.RecycleProduct(product_id);
            if (result)
            {
                TempData["Message"] = "Khôi phục sản phẩm thành công!";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "Khôi phục sản phẩm thất bại!";
                TempData["MessageType"] = "danger";
            }
            return RedirectToAction("Index");
        }

        public ActionResult ProductExportToExcel()
        {
            var data = productRepository.GetProducts();
            var userName = ((user)Session["Admin"]).full_name;
            var fileContent = _excelReportService.GenerateProductReport(data, userName);
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "danh_sach_san_pham.xlsx");
        }

        public ActionResult Category(int? page, string category_id, string search_str)
        {
            var message = TempData["Message"];
            var messageType = TempData["MessageType"];
            if (message != null)
            {
                ViewBag.Message = message;
                ViewBag.MessageType = messageType;
            }
            int pageSize = 10;
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
                var existCategory = productRepository.GetCategoryByName(category.category_name);
                if (existCategory != null)
                {
                    TempData["Message"] = "Tên danh mục đã tồn tại! Vui lòng thêm tên mới hoặc kiểm tra phần khôi phục!";
                    TempData["MessageType"] = "danger";
                }
                else
                {
                    var result = productRepository.AddCategory(category);
                    if (result)
                    {
                        TempData["Message"] = "Thêm danh mục thành công!";
                        TempData["MessageType"] = "success";
                    }
                    else
                    {
                        TempData["Message"] = "Thêm danh mục thất bại!";
                        TempData["MessageType"] = "danger";
                    }
                }
            }
            else
            {
                var result = productRepository.UpdateCategory(category);
                if (result)
                {
                    TempData["Message"] = "Cập nhật danh mục thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Cập nhật danh mục thất bại!";
                    TempData["MessageType"] = "success";
                }
            }
            return RedirectToAction("Category", "/Product");
        }


        [HttpPost]
        public JsonResult AddCategoryAJAX(string categoryName)
        {
            category category = new category
            {
                category_id = productRepository.GenerateCategoryId(),
                category_name = categoryName,
            };

            var existCategory = productRepository.GetRecycleCategory(categoryName);

            if (existCategory != null)
            {
                return Json(new { success = false, message = $"Danh mục {categoryName} đã tồn tại và đã bị xóa. Bạn có muốn khôi phục lại danh mục này?", existCategoryId = existCategory.category_id });
            }
            else
            {
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
        }

        public ActionResult DeleteCategory(string category_id)
        {
            var result = productRepository.DeleteCategory(category_id);
            if (result)
            {
                TempData["Message"] = "Xóa danh mục thành công!";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "Xóa danh mục thất bại!";
                TempData["MessageType"] = "danger";
            }

            return RedirectToAction("Category");
        }

        public ActionResult RecycleCategoryIndex()
        {
            List<category> listCategory = productRepository.GetRecycleCategories();
            return View(listCategory);
        }

        public ActionResult RecycleCategory(string category_id)
        {
            var result = productRepository.RecycleCategory(category_id);
            if (result)
            {
                TempData["Message"] = "Khôi phục danh mục thành công!";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "Khôi phục danh mục thất bại!";
                TempData["MessageType"] = "danger";
            }
            return RedirectToAction("Category");
        }

        [HttpPost]
        public JsonResult RecycleCategoryAJAX(string category_id)
        {
            bool result = productRepository.RecycleCategory(category_id);
            var category = productRepository.GetCategory(category_id);
            if (result)
            {
                return Json(new { success = true, e_category_id = category.category_id, e_category_name = category.category_name });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        public ActionResult Attribute(int? page, string attribute_id, string search_str)
        {
            var message = TempData["Message"];
            var messageType = TempData["MessageType"];
            if (message != null)
            {
                ViewBag.Message = message;
                ViewBag.MessageType = messageType;
            }
            int pageSize = 10;
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
                var existAttribute = productRepository.GetAttributeByName(attribute.attribute_name);
                if (existAttribute != null)
                {
                    TempData["Message"] = "Tên thuộc tính đã tồn tại! Vui lòng thêm tên mới hoặc kiểm tra phần khôi phục!";
                    TempData["MessageType"] = "danger";
                }
                else
                {
                    var result = productRepository.AddAttribute(attribute);
                    if (result)
                    {
                        TempData["Message"] = "Thêm thuộc tính thành công!";
                        TempData["MessageType"] = "success";
                    }
                    else
                    {
                        TempData["Message"] = "Thêm thuộc tính thất bại!";
                        TempData["MessageType"] = "danger";
                    }
                }
            }
            else
            {
                var result = productRepository.UpdateAttribute(attribute);
                if (result)
                {
                    TempData["Message"] = "Cập nhật thuộc tính thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Cập nhật thuộc tính thất bại!";
                    TempData["MessageType"] = "danger";
                }
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
            var existAttribute = productRepository.GetRecycleAttribute(attributeName);

            if (existAttribute != null)
            {
                return Json(new { success = false, message = $"Thuộc tính {attributeName} đã tồn tại và đã bị xóa. Bạn có muốn khôi phục lại thuộc tính này?", existAttributeId = existAttribute.attribute_id });
            }
            else
            {
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
        }

        public ActionResult DeleteAttribute(string attribute_id)
        {
            var result = productRepository.DeleteAttribute(attribute_id);
            if (result)
            {
                TempData["Message"] = "Xóa thuộc tính thành công!";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "Xóa thuộc tính thất bại!";
                TempData["MessageType"] = "danger";
            }
            return RedirectToAction("Attribute");
        }

        public ActionResult RecycleAttributeIndex()
        {
            List<attribute> listAttribute = productRepository.GetRecycleAttributes();
            return View(listAttribute);
        }

        public ActionResult RecycleAttribute(string attribute_id)
        {
            var result = productRepository.RecycleAttribute(attribute_id);
            if (result)
            {
                TempData["Message"] = "Khôi phục thuộc tính thành công!";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "Khôi phục thuộc tính thất bại!";
                TempData["MessageType"] = "danger";
            }
            return RedirectToAction("Attribute");
        }

        [HttpPost]
        public JsonResult RecycleAttributeAJAX(string attribute_id)
        {
            bool result = productRepository.RecycleAttribute(attribute_id);
            var attribute = productRepository.GetAttribute(attribute_id);
            if (result)
            {
                return Json(new { success = true, e_attribute_id = attribute.attribute_id, e_attribute_name = attribute.attribute_name });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        public ActionResult AttributeValue(int? page, string attribute_value_id, string search_str)
        {
            var message = TempData["Message"];
            var messageType = TempData["MessageType"];
            if (message != null)
            {
                ViewBag.Message = message;
                ViewBag.MessageType = messageType;
            }
            int pageSize = 10;
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
            var existAttributeValue = productRepository.GetRecycleAttributeValue(value);

            if (existAttributeValue != null)
            {
                return Json(new { success = false, message = $"Giá trị {value} đã tồn tại và đã bị xóa. Bạn có muốn khôi phục lại giá trị này?", existAttributeValueId = existAttributeValue.attribute_value_id });
            }
            else
            {
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
        }

        [HttpPost]
        public ActionResult AddAttributeValue(string action, attribute_value attribute_Value)
        {
            if (action == "add")
            {
                var existValue = productRepository.GetAttributeValueByValue(attribute_Value.value);
                if (existValue != null)
                {
                    TempData["Message"] = "Giá trị đã tồn tại! Vui lòng thêm giá trị mới hoặc kiểm tra phần khôi phục!";
                    TempData["MessageType"] = "danger";
                }
                else
                {
                    var result = productRepository.AddAttributeValue(attribute_Value);
                    if (result)
                    {
                        TempData["Message"] = "Thêm giá trị thành công!";
                        TempData["MessageType"] = "success";
                    }
                    else
                    {
                        TempData["Message"] = "Thêm giá trị thất bại!";
                        TempData["MessageType"] = "danger";
                    }
                }
            }
            else
            {
                var result = productRepository.UpdateAttributeValue(attribute_Value);
                if (result)
                {
                    TempData["Message"] = "Cập nhật giá trị thành công!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Cập nhật giá trị thất bại!";
                    TempData["MessageType"] = "danger";
                }
            }
            return RedirectToAction("AttributeValue", "/Product");
        }

        public ActionResult DeleteAttributeValue(string attribute_value_id)
        {
            var result = productRepository.DeleteAttributeValue(attribute_value_id);
            if (result)
            {
                TempData["Message"] = "Xóa giá trị thành công!";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "Xóa giá trị thất bại!";
                TempData["MessageType"] = "danger";
            }
            return RedirectToAction("AttributeValue");
        }

        public ActionResult RecycleAttributeValueIndex()
        {
            List<attribute_value> listAttributeValue = productRepository.GetRecycleAttributeValues();
            return View(listAttributeValue);
        }

        public ActionResult RecycleAttributeValue(string attribute_value_id)
        {
            var result = productRepository.RecycleAttributeValue(attribute_value_id);
            if (result)
            {
                TempData["Message"] = "Khôi phục giá trị thành công!";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "Khôi phục giá trị thất bại!";
                TempData["MessageType"] = "danger";
            }
            return RedirectToAction("Attribute");
        }

        [HttpPost]
        public JsonResult RecycleAttributeValueAJAX(string attribute_value_id)
        {
            bool result = productRepository.RecycleAttributeValue(attribute_value_id);
            var attribute_value = productRepository.GetAttributeValue(attribute_value_id);
            if (result)
            {
                return Json(new { success = true, e_attribute_value_id = attribute_value.attribute_value_id, e_value = attribute_value.value, e_attribute_id = attribute_value.attribute.attribute_id });
            }
            else
            {
                return Json(new { success = false });
            }
        }
    }
}
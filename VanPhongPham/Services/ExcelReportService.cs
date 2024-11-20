using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using VanPhongPham.Models;
using System.Threading.Tasks;

namespace VanPhongPham.Services
{
    public class ExcelReportService
    {
        private readonly PurchaseOrderRepository purchaseOrderRepository;
        public ExcelReportService()
        {
            purchaseOrderRepository = new PurchaseOrderRepository();
        }
        public byte[] GenerateProductReport(IEnumerable<product> data, string userName)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Danh sách sản phẩm");

                worksheet.Cells[1, 1].Value = $"Ngày lập: {DateTime.Now.ToString("dd/MM/yyyy")}";
                worksheet.Cells[1, 1, 1, 7].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 12;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                worksheet.Cells[2, 1].Value = $"Người lập báo cáo: {userName}";
                worksheet.Cells[2, 1, 2, 7].Merge = true;
                worksheet.Cells[2, 1].Style.Font.Bold = true;
                worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[3, 1].Value = "Danh sách sản phẩm";
                worksheet.Cells[3, 1, 3, 7].Merge = true;
                worksheet.Cells[3, 1].Style.Font.Bold = true;
                worksheet.Cells[3, 1].Style.Font.Size = 14;
                worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[3, 1].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);  // Background color for the header

                worksheet.Cells[4, 1].Value = "Mã sản phẩm";
                worksheet.Cells[4, 2].Value = "Tên sản phẩm";
                worksheet.Cells[4, 3].Value = "Loại sản phẩm";
                worksheet.Cells[4, 4].Value = "Giá nhập";
                worksheet.Cells[4, 5].Value = "Giá bán";
                worksheet.Cells[4, 6].Value = "Số lượng tồn";
                worksheet.Cells[4, 7].Value = "Số lượng bán";

                worksheet.Cells[4, 1, 4, 7].Style.Font.Bold = true;
                worksheet.Cells[4, 1, 4, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[4, 1, 4, 7].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                worksheet.Cells[4, 1, 4, 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[4, 1, 4, 7].Style.Fill.BackgroundColor.SetColor(Color.LightGray); 

                int row = 5;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.product_id;
                    worksheet.Cells[row, 2].Value = item.product_name;
                    worksheet.Cells[row, 3].Value = item.category.category_name;
                    worksheet.Cells[row, 4].Value = item.purchase_price;
                    worksheet.Cells[row, 5].Value = item.price;
                    worksheet.Cells[row, 6].Value = item.stock_quantity;
                    worksheet.Cells[row, 7].Value = item.sold;

                    worksheet.Cells[row, 4].Style.Numberformat.Format = "#,##0₫"; 
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0₫"; 

                    if (row % 2 == 0)
                    {
                        using (var range = worksheet.Cells[row, 1, row, 7])
                        {
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.Beige); 
                        }
                    }
                    else
                    {
                        using (var range = worksheet.Cells[row, 1, row, 7])
                        {
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightYellow); 
                        }
                    }

                  
                    worksheet.Cells[row, 1, row, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 1, row, 7].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }

        public byte[] GeneratePurchaseOrderReport(IEnumerable<purchase_order> data, string userName)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Danh sách phiếu đặt hàng");

                // Tiêu đề báo cáo
                worksheet.Cells[1, 1].Value = "BÁO CÁO DANH SÁCH PHIẾU ĐẶT HÀNG";
                worksheet.Cells[1, 1, 1, 7].Merge = true; // Gộp ô tiêu đề
                worksheet.Cells[1, 1].Style.Font.Bold = true; // In đậm
                worksheet.Cells[1, 1].Style.Font.Size = 14; // Cỡ chữ lớn hơn
                worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Căn giữa

                // Ngày lập báo cáo
                worksheet.Cells[2, 1].Value = "Ngày lập báo cáo:";
                worksheet.Cells[2, 2].Value = DateTime.Now.ToString("dd/MM/yyyy");

                // Thêm thông tin về người lập báo cáo (nếu cần)
                worksheet.Cells[3, 1].Value = "Người lập báo cáo:";
                worksheet.Cells[3, 2].Value = userName; // Cập nhật tên người lập nếu có

                // Tiêu đề cột cho danh sách phiếu đặt hàng
                worksheet.Cells[5, 1].Value = "Mã phiếu đặt";
                worksheet.Cells[5, 2].Value = "Ngày đặt";
                worksheet.Cells[5, 3].Value = "Tên nhà cung cấp";
                worksheet.Cells[5, 4].Value = "Người đặt";
                worksheet.Cells[5, 5].Value = "Số lượng hàng hóa";
                worksheet.Cells[5, 6].Value = "Trạng thái";
                worksheet.Cells[5, 7].Value = "Tổng tiền";  // Thêm cột tổng tiền ngay cạnh cột Trạng thái

                // Định dạng tiêu đề cột
                using (var range = worksheet.Cells[5, 1, 5, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray); // Màu nền tiêu đề
                }

                int row = 6; // Dữ liệu bắt đầu từ dòng 6
                foreach (var orderData in data)
                {
                    double? totalOrderPrice = 0; // Khởi tạo biến tổng tiền cho phiếu đặt hàng

                    // Thêm thông tin phiếu đặt hàng
                    worksheet.Cells[row, 1].Value = orderData.purchase_order_id;
                    worksheet.Cells[row, 2].Value = orderData.created_at.Value.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 3].Value = orderData.supplier.supplier_name;
                    worksheet.Cells[row, 4].Value = orderData.user.full_name;
                    worksheet.Cells[row, 5].Value = orderData.item_count;
                    worksheet.Cells[row, 6].Value = orderData.status;
                    int rowOrderData = row;
                    // Tô đậm và căn giữa dòng dữ liệu phiếu đặt hàng
                    using (var range = worksheet.Cells[row, 1, row, 7])
                    {
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.LightYellow); // Màu nền của dữ liệu phiếu đặt
                    }

                    // Thêm chú thích "Danh sách sản phẩm" cho mỗi phiếu
                    row++;
                    worksheet.Cells[row, 1].Value = "Danh sách sản phẩm:";
                    worksheet.Cells[row, 1, row, 7].Merge = true; // Gộp ô tiêu đề
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    // Thiết lập tiêu đề cột cho danh sách sản phẩm
                    row++;
                    worksheet.Cells[row, 1].Value = "Mã sản phẩm";
                    worksheet.Cells[row, 2].Value = "Tên sản phẩm";
                    worksheet.Cells[row, 3].Value = "Đơn giá";
                    worksheet.Cells[row, 4].Value = "Số lượng đặt";
                    worksheet.Cells[row, 5].Value = "Số lượng nhận";
                    worksheet.Cells[row, 6].Value = "Số lượng còn lại";
                    worksheet.Cells[row, 7].Value = "Tổng tiền";

                    // Định dạng tiêu đề sản phẩm
                    using (var range = worksheet.Cells[row, 1, row, 7])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue); // Màu nền tiêu đề sản phẩm
                    }

                    var orderDetailList = purchaseOrderRepository.GetPurchaseOrderDetails(orderData.purchase_order_id);
                    row++; // Dữ liệu sản phẩm bắt đầu từ dòng kế tiếp
                    foreach (var order_Detail in orderDetailList)
                    {
                        // Thêm thông tin sản phẩm
                        worksheet.Cells[row, 1].Value = order_Detail.product.product_id;
                        worksheet.Cells[row, 2].Value = order_Detail.product.product_name;
                        worksheet.Cells[row, 3].Value = order_Detail.price;
                        worksheet.Cells[row, 4].Value = order_Detail.quantity;
                        worksheet.Cells[row, 5].Value = order_Detail.quantity_received;
                        worksheet.Cells[row, 6].Value = order_Detail.quantity - order_Detail.quantity_received;
                        worksheet.Cells[row, 7].Value = order_Detail.quantity * order_Detail.price;

                        // Cộng dồn tổng tiền cho phiếu đặt
                        totalOrderPrice += order_Detail.quantity * order_Detail.price;

                        // Định dạng tiền theo VND cho cột "Đơn giá" và "Tổng tiền"
                        worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0\"đ\"";  // Đơn giá
                        worksheet.Cells[row, 7].Style.Numberformat.Format = "#,##0\"đ\"";  // Tổng tiền

                        // Căn giữa dòng dữ liệu sản phẩm
                        using (var range = worksheet.Cells[row, 1, row, 7])
                        {
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.Beige); // Màu nền cho dữ liệu sản phẩm
                        }
                        row++;
                    }

                    // Hiển thị tổng tiền cho phiếu đặt hàng dưới cột "Tổng tiền" của mỗi phiếu
                    worksheet.Cells[rowOrderData, 7].Value = totalOrderPrice;
                    worksheet.Cells[rowOrderData, 7].Style.Numberformat.Format = "#,##0\"đ\""; // Định dạng tiền

                    row++; // Dòng trống giữa các phiếu
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns(); // Tự động điều chỉnh kích thước cột

                return package.GetAsByteArray();
            }
        }
    }
}
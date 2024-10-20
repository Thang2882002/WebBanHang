using BanHang.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace BanHang.Areas.Admin.Controllers
{
	public class HomeController : Controller
	{
		private BanHangContext db = new BanHangContext();

		// GET: Admin/Home
		public ActionResult Index()
		{
			if (Session["Tendangnhap1"] == null)
			{
				return RedirectToAction("Index", "Login");
			}

			// Lấy các thông tin thống kê
			decimal totalRevenue = (decimal)db.Orders.Sum(o => o.TotalAmount);
			int totalQuantitySold = (int)db.Order_Details.Sum(od => od.Quantity);
			decimal totalProfit = (decimal)db.Order_Details.Sum(od => (od.Price - od.Product.PriceOrigin) * od.Quantity);
			int totalQuantityInStock = (int)(db.Products.Sum(p => p.Quantity) - totalQuantitySold);

			// Truyền các giá trị này vào view thông qua ViewBag
			ViewBag.TotalRevenue = totalRevenue;
			ViewBag.TotalQuantitySold = totalQuantitySold;
			ViewBag.TotalProfit = totalProfit;
			ViewBag.TotalQuantityInStock = totalQuantityInStock;

			return View();
		}

		[HttpPost]
		public ActionResult SearchOrders(string startDate, string endDate)
		{
			// Chuyển đổi startDate và endDate sang kiểu DateTime
			DateTime startDateTime = DateTime.Parse(startDate);
			DateTime endDateTime = DateTime.Parse(endDate).AddDays(1).AddSeconds(-1); // Đảm bảo lấy hết đến cuối ngày

			// Lấy danh sách đơn hàng trong khoảng thời gian đã chọn
			var ordersInPeriod = db.Orders.Where(o => o.CreatedDate >= startDateTime && o.CreatedDate <= endDateTime).ToList();

			return PartialView("_OrderListPartial", ordersInPeriod);
		}

		public ActionResult ExportToExcel(string startDate, string endDate)
		{
			// Chuyển đổi startDate và endDate sang kiểu DateTime
			DateTime startDateTime = DateTime.Parse(startDate);
			DateTime endDateTime = DateTime.Parse(endDate).AddDays(1).AddSeconds(-1); // Đảm bảo lấy hết đến cuối ngày

			// Lấy danh sách đơn hàng từ cơ sở dữ liệu trong khoảng thời gian đã chọn
			var orders = db.Orders
				.Where(o => o.CreatedDate >= startDateTime && o.CreatedDate <= endDateTime &&o.Status == true)
				.ToList();

			// Tạo một DataTable để lưu dữ liệu của các đơn hàng
			DataTable dt = new DataTable("Orders");
			dt.Columns.AddRange(new DataColumn[9] {
				new DataColumn("Mã Đơn Hàng", typeof(string)),
				new DataColumn("Khách Hàng", typeof(string)),
				new DataColumn("Số Điện Thoại", typeof(string)),
				new DataColumn("Địa Chỉ", typeof(string)),
				new DataColumn("Tổng Tiền", typeof(decimal)),
				new DataColumn("Thanh Toán", typeof(string)),
				new DataColumn("Ngày Tạo", typeof(string)), // Sử dụng kiểu dữ liệu string để định dạng ngày
                new DataColumn("Đơn vị vận chuyển", typeof(string)),
				new DataColumn("Email", typeof(string))
			});

			// Thêm dữ liệu của các đơn hàng vào DataTable
			foreach (var order in orders)
			{
				dt.Rows.Add(
					order.OrderCode,
					order.CustomerName,
					order.Phone,
					order.Address,
					order.TotalAmount,
					(bool)order.StatusPayment ? "Đã thanh toán" : "Chưa thanh toán",
					order.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"), // Định dạng ngày tạo theo yêu cầu
					order.Shipping,
					order.Email
				);
			}

			// Tạo một MemoryStream để lưu trữ dữ liệu Excel
			using (MemoryStream ms = new MemoryStream())
			{
				using (ExcelPackage package = new ExcelPackage(ms))
				{
					// Tạo một ExcelWorksheet
					ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Orders");

					// Tạo range cho tiêu đề "HÓA ĐƠN CHI TIẾT"
					ExcelRange headerRange = worksheet.Cells["A1:I1"];
					headerRange.Merge = true;
					headerRange.Value = "DANH SÁCH HÓA ĐƠN";
					headerRange.Style.Font.Bold = true;
					headerRange.Style.Font.Color.SetColor(System.Drawing.Color.Black); // Đặt màu chữ là màu xanh lam
					headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
					headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White); // Đặt màu nền là xanh lam
					headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa tiêu đề

					// Tạo tiêu đề cho các cột và đặt định dạng cho chúng
					string[] columnTitles = { "Mã Đơn Hàng", "Khách Hàng", "Số Điện Thoại", "Địa Chỉ", "Tổng Tiền", "Thanh Toán", "Ngày Tạo", "Đơn vị vận chuyển", "Email" };
					for (int i = 0; i < columnTitles.Length; i++)
					{
						ExcelRange columnTitleRange = worksheet.Cells[2, i + 1];
						columnTitleRange.Value = columnTitles[i];
						columnTitleRange.Style.Font.Bold = true;
						columnTitleRange.Style.Font.Color.SetColor(System.Drawing.Color.Black); // Đặt màu chữ là đen
						columnTitleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
						columnTitleRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue); // Đặt màu nền là xanh lam
					}

					// Đổ dữ liệu từ DataTable vào ExcelWorksheet, bắt đầu từ dòng thứ 3 (sau tiêu đề)
					worksheet.Cells["A3"].LoadFromDataTable(dt, false);

					// Tự động điều chỉnh độ rộng của các cột để vừa với nội dung
					worksheet.Cells.AutoFitColumns();

					// Thêm border cho bảng
					using (var range = worksheet.Cells[headerRange.Start.Row, headerRange.Start.Column, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column])
					{
						range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
						range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
						range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
						range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
					}

					// Lưu trữ ExcelPackage vào MemoryStream
					package.Save();
				}

				// Xuất file Excel
				return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ListOrders.xlsx");
			}
		}
	}
}

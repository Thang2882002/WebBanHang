using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BanHang.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using OfficeOpenXml;
using PagedList;

namespace BanHang.Areas.Admin.Controllers
{
	public class OrdersController : Controller
	{
		private BanHangContext db = new BanHangContext();

		// GET: Admin/Orders
		public ActionResult Index(string searchTitle, int? page)
		{
			if (Session["Tendangnhap1"] == null)
			{
				return RedirectToAction("Index", "Login");
			}
			var news = db.Orders.AsQueryable();

			if (!string.IsNullOrEmpty(searchTitle))
			{
				news = news.Where(n => n.OrderCode.Contains(searchTitle));
			}

			int pageSize = 3; // Số lượng tin tức trên mỗi trang
			int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là 1 nếu không có giá trị

			return View(news.OrderByDescending(o => o.CreatedDate).ToPagedList(pageNumber, pageSize));
		}
		public ActionResult ExportToExcel()
		{
			if (Session["Tendangnhap1"] == null)
			{
				return RedirectToAction("Index", "Login");
			}

			var orders = db.Orders.ToList(); // Fetch the data from the database

			// Generate Excel file
			using (var package = new ExcelPackage())
			{
				var worksheet = package.Workbook.Worksheets.Add("Danh sách Đơn Hàng");
				// Add headers
				worksheet.Cells[1, 1].Value = "Mã Đơn Hàng";
				worksheet.Cells[1, 2].Value = "Khách Hàng";
				worksheet.Cells[1, 3].Value = "Số Điện Thoại";
				worksheet.Cells[1, 4].Value = "Địa Chỉ";
				worksheet.Cells[1, 5].Value = "Tổng Tiền";
				worksheet.Cells[1, 6].Value = "Thanh Toán";
				worksheet.Cells[1, 7].Value = "Ngày Tạo";
				worksheet.Cells[1, 8].Value = "Email";

				// Fill in data
				int row = 2;
				foreach (var order in orders)
				{
					string formattedDate = order.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
					worksheet.Cells[row, 1].Value = order.OrderCode;
					worksheet.Cells[row, 2].Value = order.CustomerName;
					worksheet.Cells[row, 3].Value = order.Phone;
					worksheet.Cells[row, 4].Value = order.Address;
					worksheet.Cells[row, 5].Value = order.TotalAmount;
					worksheet.Cells[row, 6].Value = (bool)order.StatusPayment ? "Đã thanh toán" : "Chưa thanh toán";
					worksheet.Cells[row, 7].Value = formattedDate;
					worksheet.Cells[row, 8].Value = order.Email;

					row++;
				}

				// Auto fit columns
				worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

				// Return the Excel file
				var fileStream = new MemoryStream();
				package.SaveAs(fileStream);
				fileStream.Position = 0;
				return File(fileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Danh_sach_Don_Hang.xlsx");
			}
		}

		// GET: Admin/Orders/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Order order = db.Orders.Find(id);
			if (order == null)
			{
				return HttpNotFound();
			}
			return View(order);
		}

		// GET: Admin/Orders/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: Admin/Orders/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "OrderID,OrderCode,CustomerName,Phone,Address,TotalAmount,Status,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Shipping,TypePayment,Email,GiaoHang,NhanHang,StatusPayment,Username")] Order order)
		{
			if (ModelState.IsValid)
			{
				db.Orders.Add(order);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(order);
		}

		// GET: Admin/Orders/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Order order = db.Orders.Find(id);
			if (order == null)
			{
				return HttpNotFound();
			}
			return View(order);
		}
		private string GetShippingOption(int? shipping)
		{
			if (shipping.HasValue)
			{
				switch (shipping.Value)
				{
					case 1:
						return "Giao Hàng Nhanh";
					case 2:
						return "Giao Hàng Tiết Kiệm";
					case 3:
						return "J&T Express";
					default:
						return "";
				}
			}
			return "";
		}
		// POST: Admin/Orders/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "OrderID, Status, GiaoHang, NhanHang, StatusPayment,Shipping")] Order order)
		{
			if (ModelState.IsValid)
			{
				// Lấy thông tin đơn hàng từ cơ sở dữ liệu
				var existingOrder = db.Orders.Find(order.OrderID);

				// Cập nhật các trường Status, GiaoHang, NhanHang, StatusPayment
				existingOrder.Status = order.Status;
				existingOrder.GiaoHang = order.GiaoHang;
				existingOrder.NhanHang = order.NhanHang;
				existingOrder.StatusPayment = order.StatusPayment;

				// Cập nhật thông tin ModifiedBy và ModifiedDate
				existingOrder.ModifiedBy = "Tên của người sửa đổi"; // Thay bằng tên của người dùng hoặc tên người đăng nhập
				existingOrder.ModifiedDate = DateTime.Now;
				existingOrder.Shipping = order.Shipping;

				// Lưu các thay đổi vào cơ sở dữ liệu
				db.Entry(existingOrder).State = EntityState.Modified;
				db.SaveChanges();

				// Chuyển hướng người dùng đến trang Index
				return RedirectToAction("Index");
			}
			return View(order);
		}

		// GET: Admin/Orders/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Order order = db.Orders.Find(id);
			if (order == null)
			{
				return HttpNotFound();
			}
			return View(order);
		}

		// POST: Admin/Orders/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Order order = db.Orders.Find(id);
			db.Orders.Remove(order);
			db.SaveChanges();
			return RedirectToAction("Index");
		}
		public ActionResult Partial_SanPham(int id)
		{
			var items = db.Order_Details.Where(x => x.OrderID == id).ToList();
			return PartialView(items);
		}


		public string GenerateInvoicePDFWithQR(int orderId)
		{
			string folderPath = Server.MapPath("~/Invoices");
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			// Tạo tài liệu PDF mới
			Document document = new Document();

			// Đường dẫn tới tệp PDF
			string filePath = Server.MapPath("~/Invoices/Invoice_" + orderId + ".pdf");

			// Thiết lập font Unicode cho tài liệu
			BaseFont bf = BaseFont.CreateFont(Server.MapPath("~/Content/Arial.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
			Font font = new Font(bf, 12, Font.NORMAL);

			// Thêm thông tin chi tiết đơn hàng
			Order order = db.Orders.Include("Order_Details").SingleOrDefault(x => x.OrderID == orderId);
			if (order != null)
			{
				// Tạo mã QR với thông tin đơn hàng
				string qrCodeData = $"Mã hóa đơn: {order.OrderCode},\n Tên khách hàng: {order.CustomerName}, \n Số điện thoại: {order.Phone},\n Tổng tiền: {order.TotalAmount}";
				qrCodeData += ", Chi tiết sản phẩm:";
				foreach (var item in order.Order_Details)
				{
					qrCodeData += $"\n Tên sản phẩm - {item.Product.Name} - \n Số lượng: {item.Quantity} - \n Size: {item.Size}";
				}

				BarcodeQRCode qrCode = new BarcodeQRCode(qrCodeData, 100, 100, null);
				Image qrCodeImage = qrCode.GetImage();

				// Tạo hóa đơn PDF
				PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
				document.Open();

				// Tạo font mới với các thuộc tính in đậm và kích thước lớn hơn
				Font titleFont = new Font(bf, 16, Font.BOLD);

				// Tạo đối tượng Paragraph với font mới
				Paragraph title = new Paragraph("SHOP TTQ - Hóa Đơn Chi Tiết", titleFont);
				title.Alignment = Element.ALIGN_CENTER;
				document.Add(title);

				// Thêm các thông tin chi tiết
				// Thêm các thông tin chi tiết
				Paragraph orderCodeParagraph = new Paragraph("Mã hóa đơn: " + order.OrderCode, font);
				orderCodeParagraph.Alignment = Element.ALIGN_CENTER;
				document.Add(orderCodeParagraph);

				Paragraph customerNameParagraph = new Paragraph("Tên khách hàng: " + order.CustomerName, font);
				customerNameParagraph.Alignment = Element.ALIGN_CENTER;
				document.Add(customerNameParagraph);

				Paragraph phoneParagraph = new Paragraph("Số điện thoại: " + order.Phone, font);
				phoneParagraph.Alignment = Element.ALIGN_CENTER;
				document.Add(phoneParagraph);

				Paragraph productDetailTitleParagraph = new Paragraph("Tên sản phẩm - Số lượng - Size", font);
				productDetailTitleParagraph.Alignment = Element.ALIGN_CENTER;
				document.Add(productDetailTitleParagraph);

				foreach (var item in order.Order_Details)
				{
					Paragraph productDetailParagraph = new Paragraph(item.Product.Name + " - " + item.Quantity + " - " + item.Size, font);
					productDetailParagraph.Alignment = Element.ALIGN_CENTER;
					document.Add(productDetailParagraph);
				}

				Paragraph totalAmountParagraph = new Paragraph("Tổng tiền: " + order.TotalAmount + " VNĐ", font);
				totalAmountParagraph.Alignment = Element.ALIGN_CENTER;
				document.Add(totalAmountParagraph);


				// Chèn mã QR vào tài liệu PDF
				qrCodeImage.Alignment = Image.ALIGN_CENTER;
				document.Add(qrCodeImage);
			}
			else
			{
				document.Add(new Paragraph("Không tìm thấy thông tin đơn hàng", font));
			}

			document.Close();

			// Trả về đường dẫn tới tệp PDF
			return filePath;
		}



		public ActionResult GenerateAndDownloadInvoiceWithQR(int orderId)
		{
			string filePath = GenerateInvoicePDFWithQR(orderId);
			byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
			string fileName = Path.GetFileName(filePath);
			return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}

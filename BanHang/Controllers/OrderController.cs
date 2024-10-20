using BanHang.Models;
using BanHang.Models.Payments;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BanHang.Controllers
{
	public class OrderController : Controller
	{
		// GET: Order
		private BanHangContext db = new BanHangContext();

		public ActionResult VnpayReturn()
		{
			if (Request.QueryString.Count > 0)
			{
				string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
				var vnpayData = Request.QueryString;
				VnPayLibrary vnpay = new VnPayLibrary();

				foreach (string s in vnpayData)
				{
					//get all querystring data
					if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
					{
						vnpay.AddResponseData(s, vnpayData[s]);
					}
				}
				string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
				long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
				string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
				string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
				String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
				String TerminalID = Request.QueryString["vnp_TmnCode"];
				long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
				String bankCode = Request.QueryString["vnp_BankCode"];

				bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
				if (checkSignature)
				{
					if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
					{
						var itemOrder = db.Orders.FirstOrDefault(x => x.OrderCode == orderCode);
						if (itemOrder != null)
						{
							itemOrder.StatusPayment = true;//đã thanh toán
							db.Orders.Attach(itemOrder);
							db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
							db.SaveChanges();
						}
						//Thanh toan thanh cong
						ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
						//log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
					}
					else
					{
						//Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
						ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
						//log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
					}
					//displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
					//displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
					//displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
					ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
					//displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
				}
			}
			//var a = UrlPayment(0, "DH3574");
			return View();
		}

		// GET: /Order/Create
		public ActionResult Create()
		{
			if (Session["Tendangnhap"] == null)
			{
				return RedirectToAction("Index", "CustomerLogin");
			}
			// Lấy thông tin giỏ hàng từ Session
			ShoppingCart cart = (ShoppingCart)Session["Cart"];
			// Kiểm tra giỏ hàng không rỗng và có ít nhất 1 sản phẩm
			if (cart != null && cart.Items.Any())
			{
				// Hiển thị view để khách hàng nhập thông tin đặt hàng
				return View();
			}
			// Nếu giỏ hàng rỗng, chuyển hướng đến trang chủ hoặc trang thông báo giỏ hàng trống
			return RedirectToAction("Index", "Home");
		}

		// POST: /Order/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(OrderViewModel model)
		{


			if (ModelState.IsValid)
			{
				Random rd = new Random();
				// Lấy thông tin giỏ hàng từ Session
				ShoppingCart cart = (ShoppingCart)Session["Cart"];
				// Tạo đơn hàng mới và gán thông tin từ model
				Order order = new Order
				{
					CustomerName = model.CustomerName,
					Phone = model.Phone,
					Address = model.Address,
					Email = model.Email,
					TypePayment = model.TypePayment,
					TotalAmount = cart.GetTotalPrice(),

					Status = true, // Trạng thái chưa thanh toán
					GiaoHang = false,
					NhanHang = false,
					StatusPayment = false,
					CreatedDate = DateTime.Now,
					ModifiedDate = DateTime.Now,
					OrderCode = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9),
					Username = (string)Session["Tendangnhap"],

				};

				// Lưu đơn hàng vào cơ sở dữ liệu
				db.Orders.Add(order);
				db.SaveChanges();

				// Lấy ID của đơn hàng vừa tạo
				int orderId = order.OrderID;

				// Duyệt qua từng sản phẩm trong giỏ hàng và lưu vào bảng chi tiết đơn hàng
				foreach (var item in cart.Items)
				{
					var product = db.Products.FirstOrDefault(p => p.IDProduct == item.ProductId);
					if (product != null)
					{
						// Giảm số lượng sản phẩm trong kho bằng số lượng sản phẩm trong giỏ hàng
						product.Quantity -= item.Quantity;
						// Cập nhật thông tin sản phẩm trong cơ sở dữ liệu
						db.Entry(product).State = EntityState.Modified;
					}
					Order_Details orderDetail = new Order_Details
					{
						OrderID = orderId,
						IDProduct = item.ProductId,
						Price = item.Price,
						Quantity = item.Quantity,
						Size = item.Size
					};
					db.Order_Details.Add(orderDetail);
				}
				db.SaveChanges();
				//send mail cho khachs hang
				var strSanPham = "";
				var thanhtien = decimal.Zero;
				var TongTien = decimal.Zero;
				foreach (var sp in cart.Items)
				{
					strSanPham += "<tr>";
					strSanPham += "<td>" + sp.ProductName + "</td>";
					strSanPham += "<td>" + sp.Quantity + "</td>";
					strSanPham += "<td>" + BanHang.Common.Common.FormatNumber(sp.TotalPrice, 0) + "</td>";
					strSanPham += "</tr>";
					thanhtien += sp.Price * sp.Quantity;
				}
				TongTien = thanhtien;
				string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
				contentCustomer = contentCustomer.Replace("{{MaDon}}", order.OrderCode);
				contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
				contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
				contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.CustomerName);
				contentCustomer = contentCustomer.Replace("{{Phone}}", order.Phone);
				contentCustomer = contentCustomer.Replace("{{Email}}", model.Email);
				contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.Address);
				contentCustomer = contentCustomer.Replace("{{ThanhTien}}", BanHang.Common.Common.FormatNumber(thanhtien, 0));
				contentCustomer = contentCustomer.Replace("{{TongTien}}", BanHang.Common.Common.FormatNumber(TongTien, 0));
				BanHang.Common.Common.SendMail("ShopOnline", "Đơn hàng #" + order.OrderCode, contentCustomer.ToString(), order.Email);

				string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
				contentAdmin = contentAdmin.Replace("{{MaDon}}", order.OrderCode);
				contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
				contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
				contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", order.CustomerName);
				contentAdmin = contentAdmin.Replace("{{Phone}}", order.Phone);
				contentAdmin = contentAdmin.Replace("{{Email}}", model.Email);
				contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.Address);
				contentAdmin = contentAdmin.Replace("{{ThanhTien}}", BanHang.Common.Common.FormatNumber(thanhtien, 0));
				contentAdmin = contentAdmin.Replace("{{TongTien}}", BanHang.Common.Common.FormatNumber(TongTien, 0));
				BanHang.Common.Common.SendMail("ShopOnline", "Đơn hàng mới #" + order.OrderCode, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);
				cart.ClearCart();

				// Xóa thông tin giỏ hàng từ Session sau khi đã đặt hàng thành công
				Session["Cart"] = null;
				//Session["Order"] = order;
				if (model.TypePayment == 2)
				{
					var url = UrlPayment(model.TypePaymentVN, order.OrderCode);
					return Redirect(url);
				}
				// Hiển thị thông báo đặt hàng thành công và chuyển hướng về trang chủ
				//TempData["SuccessMessage"] = "Đặt hàng thành công!";

				return RedirectToAction("Success");
			}

			// Nếu model không hợp lệ, hiển thị lại form đặt hàng để khách hàng nhập lại thông tin
			return View(model);
		}

		public ActionResult Success()
		{
			return View();
		}
		// Action để hiển thị đơn hàng đã được đặt của khách hàng
		public ActionResult MyOrders()
		{
			// Lấy tên đăng nhập của khách hàng từ session
			string tendangnhap = (string)Session["Tendangnhap"];

			// Kiểm tra xem người dùng đã đăng nhập chưa
			if (string.IsNullOrEmpty(tendangnhap))
			{
				// Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
				return RedirectToAction("Index", "CustomerLogin");
			}

			// Lấy thông tin đơn hàng từ cơ sở dữ liệu dựa trên tên đăng nhập
			var orders = db.Orders
				.Where(o => o.Username == tendangnhap).OrderByDescending(o => o.CreatedDate)
				.ToList();

			return View(orders);
		}
		public ActionResult PendingOrders()
		{
			string tendangnhap = (string)Session["Tendangnhap"];

			if (string.IsNullOrEmpty(tendangnhap))
			{
				return RedirectToAction("Index", "CustomerLogin");
			}

			var pendingOrders = db.Orders
				.Where(o => o.Username == tendangnhap && o.Status == true && o.GiaoHang == false).OrderByDescending(o => o.CreatedDate)
				.ToList();

			return View(pendingOrders);
		}

		public ActionResult DeliveryStatus()
		{
			// Lấy tên đăng nhập của khách hàng từ session
			string tendangnhap = (string)Session["Tendangnhap"];

			if (string.IsNullOrEmpty(tendangnhap))
			{
				return RedirectToAction("Index", "CustomerLogin");
			}

			// Truy vấn các đơn hàng có trạng thái đang vận chuyển và chưa được giao hàng của khách hàng
			var pendingDeliveryOrders = db.Orders
				.Where(o => o.Username == tendangnhap && o.Status == true && o.GiaoHang == true && o.NhanHang == false).OrderByDescending(o => o.CreatedDate)
				.ToList();

			return View("DeliveryStatus", pendingDeliveryOrders);
		}
		public ActionResult Partial_SanPham(int id)
		{
			var items = db.Order_Details.Where(x => x.OrderID == id).ToList();
			return PartialView(items);
		}
		public ActionResult Partial_SanPhamShipping(int id)
		{
			var items = db.Order_Details.Where(x => x.OrderID == id).ToList();
			return PartialView(items);
		}
		[HttpPost]
		public ActionResult CancelOrder(int id)
		{
			var order = db.Orders.FirstOrDefault(o => o.OrderID == id);
			if (order != null)
			{
				// Xử lý hủy đơn hàng ở đây, ví dụ: đặt trạng thái Status của đơn hàng thành "Đã hủy"
				order.Status = false;
				db.SaveChanges();
				return Json(new { success = true });
			}
			else
			{
				return Json(new { success = false });
			}
		}
		[HttpPost]
		public ActionResult ReceiveOrder(int id)
		{
			var order = db.Orders.FirstOrDefault(o => o.OrderID == id);
			if (order != null)
			{
				// Xử lý cập nhật trạng thái nhận hàng ở đây, ví dụ: đặt trạng thái NhanHang của đơn hàng thành "true"
				order.NhanHang = true;
				db.SaveChanges();
				return Json(new { success = true });
			}
			else
			{
				return Json(new { success = false });
			}
		}
		public ActionResult ReceiveOrders()
		{
			// Lấy tên đăng nhập của khách hàng từ session
			string tendangnhap = (string)Session["Tendangnhap"];

			if (string.IsNullOrEmpty(tendangnhap))
			{
				return RedirectToAction("Index", "CustomerLogin");
			}

			// Truy vấn các đơn hàng có trạng thái đang vận chuyển và chưa được giao hàng của khách hàng
			var pendingDeliveryOrders = db.Orders
				.Where(o => o.Username == tendangnhap && o.Status == true && o.GiaoHang == true && o.NhanHang == true).OrderByDescending(o => o.CreatedDate)
				.ToList();

			return View("ReceiveOrders", pendingDeliveryOrders);
		}
		public ActionResult CancelledOrders()
		{
			string username = (string)Session["Tendangnhap"];
			var cancelledOrders = db.Orders
				.Where(o => o.Username == username && o.Status == false).OrderByDescending(o => o.CreatedDate) // Đơn hàng đã bị hủy
				.ToList();
			return View(cancelledOrders);
		}


		#region Thanh toán vnpay
		public string UrlPayment(int TypePaymentVN, string orderCode)
		{
			var urlPayment = "";
			var order = db.Orders.FirstOrDefault(x => x.OrderCode == orderCode);
			//Get Config Info
			string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
			string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
			string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
			string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

			//Build URL for VNPAY
			VnPayLibrary vnpay = new VnPayLibrary();
			var Price = (long)order.TotalAmount * 100;
			vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
			vnpay.AddRequestData("vnp_Command", "pay");
			vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
			vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
			if (TypePaymentVN == 1)
			{
				vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
			}
			else if (TypePaymentVN == 2)
			{
				vnpay.AddRequestData("vnp_BankCode", "VNBANK");
			}
			else if (TypePaymentVN == 3)
			{
				vnpay.AddRequestData("vnp_BankCode", "INTCARD");
			}

			vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
			vnpay.AddRequestData("vnp_CurrCode", "VND");
			vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
			vnpay.AddRequestData("vnp_Locale", "vn");
			vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.OrderCode);
			vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

			vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
			vnpay.AddRequestData("vnp_TxnRef", order.OrderCode); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

			//Add Params of 2.1.0 Version
			//Billing

			urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
			//log.InfoFormat("VNPAY URL: {0}", paymentUrl);
			return urlPayment;
		}
		#endregion


		// Các action khác cho việc hiển thị và quản lý đơn hàng (xem chi tiết, cập nhật trạng thái, v.v.) có thể được thêm vào sau này
	}
}

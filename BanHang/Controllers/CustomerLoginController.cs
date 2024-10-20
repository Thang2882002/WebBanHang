using BanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Controllers
{
    public class CustomerLoginController : Controller
    {
		// GET: Login
		private BanHangContext db = new BanHangContext();
		// GET: Admin/Login
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Dangxuat()
		{
			// Đặt các session cần xóa thành null
			Session["Tendangnhap"] = null;
			Session["TenNguoiDung"] = null;

			// Chuyển hướng người dùng đến trang chủ
			return RedirectToAction("Index", "Home");
		}
		public static string HashPassword(string password)
		{
			using (SHA256 sha256Hash = SHA256.Create())
			{
				byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					builder.Append(bytes[i].ToString("x2"));
				}

				return builder.ToString();
			}
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index(Login model)
		{
			if (ModelState.IsValid)
			{
				if (!string.IsNullOrEmpty(model.Password))
				{
					string pass = HashPassword(model.Password);
					var user = db.Users.FirstOrDefault(u => u.UserName == model.Username && u.Password == pass);
					if (user != null)
					{
						if (user.Status == true)
						{
							if (user.IDRole == 2)
							{
								TempData["dangnhapthanhcong"] = true;

								int userID = user.UserID;// Lấy ID của người dùng sau khi xác thực
								Session["UserID"] = userID;
								Session["Tendangnhap"] = user.UserName;
								Session["TenNguoiDung"] = user.Name;
								return RedirectToAction("Index", "Home");
							}
						}
						else
						{
							// Đăng nhập thất bại - tài khoản không hoạt động
							ModelState.AddModelError("", "Tài khoản này hiện tại không hoạt động.");
							return View(model);
						}
					}
					else
					{
						// Đăng nhập thất bại - sai tên đăng nhập hoặc mật khẩu
						ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không chính xác.");
						return View(model);
					}
				}
				else
				{
					// Đăng nhập thất bại - không nhập mật khẩu
					ModelState.AddModelError("", "Vui lòng nhập mật khẩu.");
					return View(model);
				}
			}

			return View(model);
		}

		// GET: Admin/Users/Create
		public ActionResult Register()
		{

			return View();
		}

		// POST: Admin/Users/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Register([Bind(Include = "UserID,UserName,Password,Name,Phone,Address,Email,IDRole,Status")] User user)
		{
			
			if (ModelState.IsValid)
			{
				if (db.Users.Any(c => c.UserName == user.UserName) )
				{
					if (db.Users.Any(c => c.UserName == user.UserName))
					{
						ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại trong cơ sở dữ liệu.");
					}
					
					return View(user);
				}
				user.Status = true;
				user.IDRole = 2;
				user.Password = HashPassword(user.Password);
				db.Users.Add(user);
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(user);
		}
		[HttpPost]
		public ActionResult CheckLoggedIn()
		{
			bool isLoggedIn = Session["Tendangnhap"] != null;
			return Json(new { isLoggedIn = isLoggedIn }, JsonRequestBehavior.AllowGet);
		}
		public ActionResult Doimatkhau()
		{
			

			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Doimatkhau(Updatepassword model)
		{
			if (ModelState.IsValid)
			{
				string tendn = (string)Session["Tendangnhap"];
				var existingUser = db.Users.FirstOrDefault(u => u.UserName == tendn);

				if (existingUser != null && existingUser.Password == HashPassword(model.CurrentPassword))
				{
					if (model.NewPassword != model.ConfirmPassword)
					{
						ModelState.AddModelError("", "Mật khẩu mới và xác nhận mật khẩu mới không khớp.");
						return View(model);
					}

					existingUser.Password = HashPassword(model.NewPassword);
					db.SaveChanges();
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError("", "Mật khẩu hiện tại không đúng.");
					return View(model);
				}
			}

			return View(model);
		}
		public ActionResult Info()
		{
			if (Session["Tendangnhap"] == null)
			{
				return RedirectToAction("Index", "CustomerLogin");
			}

			string tendn = (string)Session["Tendangnhap"];
			var customer = db.Users.FirstOrDefault(u => u.UserName == tendn);
			if (customer == null)
			{
				return HttpNotFound(); // Hoặc chuyển hướng đến một trang lỗi khác
			}

			return View(customer);
		}

		// GET: CustomerLogin/Edit
		public ActionResult Edit()
		{
			// Kiểm tra xem người dùng đã đăng nhập hay chưa
			if (Session["Tendangnhap"] == null)
			{
				return RedirectToAction("Index", "CustomerLogin"); // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
			}

			// Lấy thông tin người dùng từ session
			string tendn = (string)Session["Tendangnhap"];
			var user = db.Users.FirstOrDefault(u => u.UserName == tendn);

			// Kiểm tra xem người dùng có tồn tại không
			if (user == null)
			{
				return HttpNotFound(); // Nếu không tìm thấy, trả về trang lỗi
			}

			// Hiển thị form sửa thông tin tài khoản với dữ liệu của người dùng hiện tại
			return View(user);
		}

		// POST: CustomerLogin/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(User model)
		{
			// Kiểm tra xem người dùng đã đăng nhập hay chưa
			if (Session["Tendangnhap"] == null)
			{
				return RedirectToAction("Index", "CustomerLogin"); // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
			}

			// Lấy thông tin người dùng từ session
			string tendn = (string)Session["Tendangnhap"];
			var user = db.Users.FirstOrDefault(u => u.UserName == tendn);

			// Kiểm tra xem người dùng có tồn tại không
			if (user == null)
			{
				return HttpNotFound(); // Nếu không tìm thấy, trả về trang lỗi
			}

			// Cập nhật thông tin tài khoản với dữ liệu mới từ form sửa
			user.Name = model.Name;
			user.Phone = model.Phone;
			user.Address = model.Address;
			user.Email = model.Email;

			// Lưu thay đổi vào cơ sở dữ liệu
			db.SaveChanges();

			// Chuyển hướng đến trang thông tin tài khoản sau khi sửa thành công
			return RedirectToAction("Info", "CustomerLogin");
		}



		public ActionResult GetCustomerInfo()
		{
			if (Session["Tendangnhap"] != null)
			{
				string tendn = (string)Session["Tendangnhap"];
				var customer = db.Users.FirstOrDefault(u => u.UserName == tendn);
				if (customer != null)
				{
					return Json(new
					{
						Name = customer.Name,
						Phone = customer.Phone,
						Address = customer.Address,
						Email = customer.Email
					}, JsonRequestBehavior.AllowGet);
				}
			}
			return Json(null); // Trả về null nếu không tìm thấy thông tin khách hàng
		}


	}
}
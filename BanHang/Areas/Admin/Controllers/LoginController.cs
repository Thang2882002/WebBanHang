using BanHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        private BanHangContext db = new BanHangContext();
		public ActionResult Dangxuat()
		{
			// Đặt các session cần xóa thành null
			Session["Tendangnhap1"] = null;
			Session["TenNguoiDung1"] = null;

			// Chuyển hướng người dùng đến trang chủ
			return RedirectToAction("Index", "Home");
		}
		// GET: Admin/Login
		public ActionResult Index()
        {
			TempData["dangnhapthanhcong"] = true;
			return View();
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
			if (ModelState.IsValid) // Kiểm tra xem ModelState có hợp lệ không
			{
				if (!string.IsNullOrEmpty(model.Password)) // Kiểm tra trống mật khẩu
				{
					string pass = HashPassword(model.Password);
					var user = db.Users.FirstOrDefault(u => u.UserName == model.Username && u.Password == pass);
					if (user != null)
					{
						if (user.Status == true)
						{
							if (user.IDRole == 1)
							{
								//Admin
								TempData["DangNhapThanhCong1"] = true;
								
								int adminID = user.UserID; // Lấy ID của admin sau khi xác thực
								Session["AdminID"] = adminID;
								Session["Tendangnhap1"] = user.UserName;
								Session["TenNguoiDung1"] = user.Name;
								return RedirectToAction("Index", "Home");
							}
							
							
						}
						else
						{

							ModelState.AddModelError("", "Tài khoản này hiện tại không hoạt động.");
							return View(model);
						}
					}
					else
					{
						ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không chính xác.");
						return View(model);
					}
				}
				else
				{
					ModelState.AddModelError("", "Vui lòng nhập mật khẩu.");
					return View(model);
				}
			}


			return View(model);
		}
	}
}
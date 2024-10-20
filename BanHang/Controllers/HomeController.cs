using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
		[HttpPost]
		public ActionResult RegisterForPromotion(string email)
		{
			try
			{
				// Gửi mail thông báo khuyến mãi
				string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send3.html"));
				// Thay thế placeholder {{email}} trong nội dung email bằng địa chỉ email người dùng nhập
				content = content.Replace("{{email}}", email);
				BanHang.Common.Common.SendMail("ShopOnline", "Thông báo khuyến mãi mới", content, email);

				// Trả về kết quả thành công
				return Json(new { success = true });
			}
			catch (Exception ex)
			{
				// Trả về kết quả thất bại nếu có lỗi xảy ra
				return Json(new { success = false, message = ex.Message });
			}
		}
		public ActionResult FooteWeb()
		{
			return View();
		}
	}
}
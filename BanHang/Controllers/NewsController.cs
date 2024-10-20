using BanHang.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Controllers
{
    public class NewsController : Controller
    {
        private BanHangContext db = new BanHangContext();
		// GET: News
		public ActionResult Index(int? page)
		{
			var pageSize = 4;
			if (page == null)
			{
				page = 1;
			}
			// Retrieve only active news articles
			IEnumerable<News> items = db.Post.Where(x => x.IsActive == true)
											  .OrderByDescending(x => x.CreatedDate);
			var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
			items = items.ToPagedList(pageIndex, pageSize);
			ViewBag.PageSize = pageSize;
			ViewBag.Page = page;
			return View(items);
		}
		public ActionResult Detail(int id)
		{
			var item = db.Post.Find(id);
			return View(item);
		}
		public ActionResult Partial_News_Home()
		{
			var items = db.Post.Take(3).ToList();
			return PartialView(items);
		}
	}
}
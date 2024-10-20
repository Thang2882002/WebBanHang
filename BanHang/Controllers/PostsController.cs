using BanHang.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Controllers
{
    public class PostsController : Controller
    {
		private BanHangContext db = new BanHangContext();
		// GET: Posts
		public ActionResult Index(int? page)
		{
			var pageSize = 4;
			if (page == null)
			{
				page = 1;
			}
			IEnumerable<Post> items = db.Posts.Where(x => x.IsActive == true).OrderByDescending(x => x.CreatedDate);
			var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
			items = items.ToPagedList(pageIndex, pageSize);
			ViewBag.PageSize = pageSize;
			ViewBag.Page = page;
			return View(items);
		}
		public ActionResult Detail(string alias)
		{
			var item = db.Posts.FirstOrDefault(x => x.Alias == alias);
			return View(item);
		}

	}
}
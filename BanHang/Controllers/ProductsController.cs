using BanHang.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Controllers
{
    public class ProductsController : Controller
    {
        private BanHangContext db = new BanHangContext();
		// GET: Products
		public ActionResult Index(string searchTitle, int? page)
		{
			
			var news = db.Products.AsQueryable();

			if (!string.IsNullOrEmpty(searchTitle))
			{
				news = news.Where(n => n.Name.Contains(searchTitle));
			}

			int pageSize = 8; // Số lượng tin tức trên mỗi trang
			int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là 1 nếu không có giá trị

			return View(news.OrderBy(n => n.IDProduct).ToPagedList(pageNumber, pageSize));
		}
		public ActionResult Partial_ItemsByCateId()
		{
			var items = db.Products.Take(10).ToList();
			return PartialView(items);
		}
		public ActionResult Partial_ProductSales()
		{
			var items = db.Products.Take(12).ToList();
			return PartialView(items);
		}
		//public ActionResult Detail(string alias, int id)
		//{
		//	var item = db.Products.Find(id);
		//	if (item != null)
		//	{
		//		db.Products.Attach(item);
		//		item.ViewCount = item.ViewCount + 1;
		//		db.Entry(item).Property(x => x.ViewCount).IsModified = true;
		//		db.SaveChanges();
		//	}

		//	return View(item);
		//}
		public ActionResult ProductCategory(string alias, int id, string searchTitle, int? page)
		{
			var items = db.Products.Where(x => x.ProductCategoryID == id);

			// Tìm kiếm theo tiêu đề nếu có từ khóa tìm kiếm được cung cấp
			if (!string.IsNullOrEmpty(searchTitle))
			{
				items = items.Where(n => n.Name.Contains(searchTitle));
			}

			int pageSize = 8; // Số lượng sản phẩm trên mỗi trang
			int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là 1 nếu không có giá trị

			// Sắp xếp và phân trang danh sách sản phẩm
			var pagedItems = items.OrderBy(n => n.IDProduct).ToPagedList(pageNumber, pageSize);

			var cate = db.ProductCategories.Find(id);
			if (cate != null)
			{
				ViewBag.CateName = cate.Name;
			}

			ViewBag.CateId = id;
			ViewBag.SearchTitle = searchTitle;

			return View(pagedItems);
		}
		public ActionResult Details(int? id)
		{
			var item = db.Products.Find(id);
			//if (item != null)
			//{
			//	db.Products.Attach(item);
			//	item.ViewCount = item.ViewCount + 1;
			//	db.Entry(item).Property(x => x.ViewCount).IsModified = true;
			//	db.SaveChanges();
			//}

			return View(item);
		}
		

	}
}
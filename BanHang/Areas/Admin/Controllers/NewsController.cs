using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BanHang.Models;
using PagedList.Mvc;
using PagedList;
namespace BanHang.Areas.Admin.Controllers
{
    public class NewsController : Controller
    {
		private BanHangContext db = new BanHangContext();

		// GET: Admin/News
		public ActionResult Index(string searchTitle, int? page)
		{
			if (Session["Tendangnhap1"] == null)
			{
				return RedirectToAction("Index", "Login");
			}
			var news = db.Post.AsQueryable();

			if (!string.IsNullOrEmpty(searchTitle))
			{
				news = news.Where(n => n.Title.Contains(searchTitle));
			}

			int pageSize = 4; // Số lượng tin tức trên mỗi trang
			int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là 1 nếu không có giá trị

			return View(news.OrderBy(n => n.IDNews).ToPagedList(pageNumber, pageSize));
		}




		// GET: Admin/News/Details/5
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.Post.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // GET: Admin/News/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Title");
            return View();
        }

        // POST: Admin/News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDNews,Title,Descriptiom,Detail,Image,CategoryID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Alias,IsActive")] News news)
        {
			if (ModelState.IsValid)
			{
				if (db.Categories.Any(c => c.Title == news.Title) )
				{
					if (db.Categories.Any(c => c.Title == news.Title))
					{
						ModelState.AddModelError("Title", "Tiêu đề đã tồn tại trong cơ sở dữ liệu.");
					}
					
					return View(news);
				}
				// Set CreatedDate to current datetime
				news.CreatedDate = DateTime.Now;

				// Set CategoryId to a specific value (e.g., 3)
				news.CategoryID = 10;

				// Set ModifiedDate to current datetime
				news.ModifiedDate = DateTime.Now;

				// Set Alias using FilterChar method
				news.Alias = BanHang.Models.Common.Filter.FilterChar(news.Title);

				db.Post.Add(news);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Title", news.CategoryID);
			return View(news);
		}

        // GET: Admin/News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.Post.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Title", news.CategoryID);
            return View(news);
        }

        // POST: Admin/News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDNews,Title,Descriptiom,Detail,Image,CategoryID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Alias,IsActive")] News news)
        {
			if (ModelState.IsValid)
			{
				var existingNews = db.Post.Find(news.IDNews);
				if (existingNews != null)
				{
					// Gán các giá trị CreatedDate và CreatedBy của tin tức đã lưu trong cơ sở dữ liệu cho tin tức category
					news.CreatedDate = existingNews.CreatedDate;
					news.CreatedBy = existingNews.CreatedBy;
                    news.CategoryID = existingNews.CategoryID;

					// Thêm dòng này để cập nhật giá trị ModifiedDate
					news.ModifiedDate = DateTime.Now;

					// Thêm dòng này để tạo alias mới từ tiêu đề đã chỉnh sửa
					news.Alias = BanHang.Models.Common.Filter.FilterChar(news.Title);

					// Cập nhật trạng thái của tin tức trong cơ sở dữ liệu
					db.Entry(existingNews).CurrentValues.SetValues(news);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
				else
				{
					return HttpNotFound();
				}
			}
			ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Title", news.CategoryID);
			return View(news);

		}

        // GET: Admin/News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.Post.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: Admin/News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.Post.Find(id);
            db.Post.Remove(news);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
		// POST: Admin/News/IsActive
		[HttpPost]
		public ActionResult IsActive(int id)
		{
			var newsItem = db.Post.Find(id);
			if (newsItem != null)
			{
				newsItem.IsActive = !newsItem.IsActive; // Đảo ngược trạng thái
				db.SaveChanges();
				return Json(new { success = true, isActive = newsItem.IsActive });
			}
			return Json(new { success = false });
		}
		// POST: Admin/News/DeleteSelected
		[HttpPost]
		public ActionResult DeleteSelected(List<int> ids)
		{
			if (ids != null && ids.Any())
			{
				try
				{
					var newsToDelete = db.Post.Where(n => ids.Contains(n.IDNews)).ToList();
					db.Post.RemoveRange(newsToDelete);
					db.SaveChanges();
					return Json(new { success = true });
				}
				catch (Exception)
				{
					// Xử lý lỗi nếu có
					return Json(new { success = false });
				}
			}
			return Json(new { success = false });
		}



	}
}

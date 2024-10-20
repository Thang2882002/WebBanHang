using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

using BanHang.Models;
using PagedList;

namespace BanHang.Areas.Admin.Controllers
{
    public class PostsController : Controller
    {
        private BanHangContext db = new BanHangContext();

		// GET: Admin/Posts
		public ActionResult Index(string searchTitle, int? page)
		{
			if (Session["Tendangnhap1"] == null)
			{
				return RedirectToAction("Index", "Login");
			}
			var post = db.Posts.AsQueryable();

			if (!string.IsNullOrEmpty(searchTitle))
			{
				post = post.Where(n => n.Title.Contains(searchTitle));
			}

			int pageSize = 4; // Số lượng tin tức trên mỗi trang
			int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là 1 nếu không có giá trị

			return View(post.OrderBy(n => n.IDPosts).ToPagedList(pageNumber, pageSize));
		}

		// GET: Admin/Posts/Details/5
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Admin/Posts/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Title");
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDPosts,Title,Descriptiom,Detail,Image,CategoryID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Alias,IsActive")] Post post)
        {
			if (ModelState.IsValid)
			{
				if (db.Posts.Any(p => p.Title == post.Title))
				{
					ModelState.AddModelError("Title", "Tiêu đề đã tồn tại trong cơ sở dữ liệu.");
					ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Title", post.CategoryID);
					return View(post);
				}

				post.CreatedDate = DateTime.Now;
				post.ModifiedDate = DateTime.Now;
				post.Alias = BanHang.Models.Common.Filter.FilterChar(post.Title);
				post.CategoryID = 9; // Đặt CategoryID với giá trị cụ thể, ví dụ: 10

				db.Posts.Add(post);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Title", post.CategoryID);
			return View(post);
		}

        // GET: Admin/Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Title", post.CategoryID);
            return View(post);
        }

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDPosts,Title,Descriptiom,Detail,Image,CategoryID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Alias,IsActive")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Title", post.CategoryID);
            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
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
			var newsItem = db.Posts.Find(id);
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
					var newsToDelete = db.Posts.Where(n => ids.Contains(n.IDPosts)).ToList();
					db.Posts.RemoveRange(newsToDelete);
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BanHang.Models;

namespace BanHang.Areas.Admin.Controllers
{
    public class CategoriesController : Controller
    {
        private BanHangContext db = new BanHangContext();

        // GET: Admin/Categories
        public ActionResult Index()
        {
			if (Session["Tendangnhap1"] == null)
			{
				return RedirectToAction("Index", "Login");
			}
			return View(db.Categories.ToList());
        }

        // GET: Admin/Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Admin/Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,Title,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Alias,Position")] Category category)
        {
            if (ModelState.IsValid)
            {
				if (db.Categories.Any(c => c.Title == category.Title) || db.Categories.Any(c => c.Position == category.Position))
				{
					if (db.Categories.Any(c => c.Title == category.Title))
					{
						ModelState.AddModelError("Title", "Tiêu đề đã tồn tại trong cơ sở dữ liệu.");
					}
					if (db.Categories.Any(c => c.Position == category.Position))
					{
						ModelState.AddModelError("Position", "Vị trí đã tồn tại trong cơ sở dữ liệu.");
					}
					return View(category);
				}
				category.CreatedDate = DateTime.Now;
				category.ModifiedDate = DateTime.Now;
                category.CreatedBy = (string)Session["Tendangnhap1"];
				category.Alias = BanHang.Models.Common.Filter.FilterChar(category.Title);
				db.Categories.Add(category);
				db.SaveChanges();
				TempData["Themthanhcong"] = true;
				return RedirectToAction("Index");
			}

            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,Title,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Alias,Position")] Category category)
        {
			if (ModelState.IsValid)
			{
				var existingCategory = db.Categories.Find(category.CategoryID);
				if (existingCategory != null)
				{
					// Gán các giá trị CreatedDate và CreatedBy của đối tượng đã lưu trong cơ sở dữ liệu cho đối tượng category
					category.CreatedDate = existingCategory.CreatedDate;
					category.CreatedBy = existingCategory.CreatedBy;

					// Thêm dòng này để cập nhật giá trị ModifiedDate
					category.ModifiedDate = DateTime.Now;
					// Thêm dòng này để tạo alias mới từ tiêu đề đã chỉnh sửa
					category.Alias = BanHang.Models.Common.Filter.FilterChar(category.Title);

					db.Entry(existingCategory).CurrentValues.SetValues(category);

					db.SaveChanges();
                    TempData["Suathanhcong"] = true;
					return RedirectToAction("Index");
				}
				else
				{
					return HttpNotFound();
				}
			}
			return View(category);
		}

        // GET: Admin/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            TempData["Xoathanhcong"] = true;

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
    }
}

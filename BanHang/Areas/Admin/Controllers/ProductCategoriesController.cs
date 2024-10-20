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
    public class ProductCategoriesController : Controller
    {
        private BanHangContext db = new BanHangContext();

        // GET: Admin/ProductCategories
        public ActionResult Index()
        {
			if (Session["Tendangnhap1"] == null)
			{
				return RedirectToAction("Index", "Login");
			}
			return View(db.ProductCategories.ToList());
        }

        // GET: Admin/ProductCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productCategory = db.ProductCategories.Find(id);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            return View(productCategory);
        }

        // GET: Admin/ProductCategories/Create
        public ActionResult Create()
        {
            return View();
        }

		// POST: Admin/ProductCategories/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ProductCategoryID,Name,SeoTitle,Image,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Alias")] ProductCategory productCategory)
		{
			if (ModelState.IsValid)
			{
				// Kiểm tra xem Name đã tồn tại trong cơ sở dữ liệu chưa
				if (db.ProductCategories.Any(c => c.Name == productCategory.Name))
				{
					ModelState.AddModelError("Name", "Tên danh mục đã tồn tại trong cơ sở dữ liệu.");
					return View(productCategory);
				}

				productCategory.CreatedDate = DateTime.Now;
				productCategory.ModifiedDate = DateTime.Now;
				productCategory.Alias = BanHang.Models.Common.Filter.FilterChar(productCategory.Name);

				db.ProductCategories.Add(productCategory);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(productCategory);
		}


		// GET: Admin/ProductCategories/Edit/5
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productCategory = db.ProductCategories.Find(id);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            return View(productCategory);
        }

		// POST: Admin/ProductCategories/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ProductCategoryID,Name,SeoTitle,Image,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Alias")] ProductCategory productCategory)
		{
			if (ModelState.IsValid)
			{
				// Lấy thông tin danh mục sản phẩm từ cơ sở dữ liệu
				var existingCategory = db.ProductCategories.Find(productCategory.ProductCategoryID);
				if (existingCategory != null)
				{
					// Gán giá trị CreatedBy và CreatedDate từ danh mục sản phẩm đã lưu trong cơ sở dữ liệu cho đối tượng productCategory
					productCategory.CreatedBy = existingCategory.CreatedBy;
					productCategory.CreatedDate = existingCategory.CreatedDate;

					// Thêm dòng này để cập nhật giá trị ModifiedDate
					productCategory.ModifiedDate = DateTime.Now;

					// Tạo Alias mới từ tên danh mục sản phẩm đã chỉnh sửa
					productCategory.Alias = BanHang.Models.Common.Filter.FilterChar(productCategory.Name);

					// Cập nhật thông tin danh mục sản phẩm trong cơ sở dữ liệu
					db.Entry(existingCategory).CurrentValues.SetValues(productCategory);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
				else
				{
					return HttpNotFound();
				}
			}
			return View(productCategory);
		}


		// GET: Admin/ProductCategories/Delete/5
		public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productCategory = db.ProductCategories.Find(id);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            return View(productCategory);
        }

        // POST: Admin/ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductCategory productCategory = db.ProductCategories.Find(id);
            db.ProductCategories.Remove(productCategory);
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
    }
}

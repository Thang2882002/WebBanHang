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
    public class ProductImagesController : Controller
    {
        private BanHangContext db = new BanHangContext();

		// GET: Admin/ProductImages
		// GET: Admin/ProductImages
		public ActionResult Index(int? page)
		{
			if (Session["Tendangnhap1"] == null)
			{
				return RedirectToAction("Index", "Login");
			}
			int pageSize = 4; // Số mục trên mỗi trang
			int pageNumber = (page ?? 1); // Trang hiện tại (mặc định là trang 1)
			var productImages = db.ProductImages.Include(p => p.Product)
												 .OrderBy(p => p.ProductImageID); 
			return View(productImages.ToPagedList(pageNumber, pageSize));
		}


		// GET: Admin/ProductImages/Details/5
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            return View(productImage);
        }

        // GET: Admin/ProductImages/Create
        public ActionResult Create()
        {
            ViewBag.IDProduct = new SelectList(db.Products, "IDProduct", "Name");
            return View();
        }

        // POST: Admin/ProductImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductImageID,IDProduct,Image,IsDefault")] ProductImage productImage)
        {
            if (ModelState.IsValid)
            {
                db.ProductImages.Add(productImage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDProduct = new SelectList(db.Products, "IDProduct", "Name", productImage.IDProduct);
            return View(productImage);
        }

        // GET: Admin/ProductImages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDProduct = new SelectList(db.Products, "IDProduct", "Name", productImage.IDProduct);
            return View(productImage);
        }

        // POST: Admin/ProductImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductImageID,IDProduct,Image,IsDefault")] ProductImage productImage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDProduct = new SelectList(db.Products, "IDProduct", "Name", productImage.IDProduct);
            return View(productImage);
        }

        // GET: Admin/ProductImages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            return View(productImage);
        }

        // POST: Admin/ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductImage productImage = db.ProductImages.Find(id);
            db.ProductImages.Remove(productImage);
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
		public ActionResult IsDefault(int id)
		{
			var newsItem = db.ProductImages.Find(id);
			if (newsItem != null)
			{
				newsItem.IsDefault = !newsItem.IsDefault; // Đảo ngược trạng thái
				db.SaveChanges();
				return Json(new { success = true, IsDefault = newsItem.IsDefault });
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
					var newsToDelete = db.ProductImages.Where(n => ids.Contains(n.ProductImageID)).ToList();
					db.ProductImages.RemoveRange(newsToDelete);
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

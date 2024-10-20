using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BanHang.Models;
using PagedList;

namespace BanHang.Areas.Admin.Controllers
{
	public class ProductsController : Controller
	{
		private BanHangContext db = new BanHangContext();

		// GET: Admin/Products
		public ActionResult Index(string searchTitle, int? page)
		{
			if (Session["Tendangnhap1"] == null)
			{
				return RedirectToAction("Index", "Login");
			}
			var news = db.Products.AsQueryable();

			if (!string.IsNullOrEmpty(searchTitle))
			{
				news = news.Where(n => n.Name.Contains(searchTitle));
			}

			int pageSize = 4; // Số lượng tin tức trên mỗi trang
			int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là 1 nếu không có giá trị

			return View(news.OrderBy(n => n.IDProduct).ToPagedList(pageNumber, pageSize));
		}

		// GET: Admin/Products/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Product product = db.Products.Find(id);
			if (product == null)
			{
				return HttpNotFound();
			}
			return View(product);
		}

		// GET: Admin/Products/Create
		public ActionResult Create()
		{
			ViewBag.ProductCategoryID = new SelectList(db.ProductCategories, "ProductCategoryID", "Name");
			return View();
		}

		// POST: Admin/Products/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "IDProduct,Name,SeoTitle,Description,Detail,Image,Price,PriceSale,PriceOrigin,Size,Quantity,IsHome,IsSale,IsActive,IsHot,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ProductCategoryID,Alias")] Product product)
		{
			if (ModelState.IsValid)
			{
				// Kiểm tra xem tên sản phẩm đã tồn tại trong cơ sở dữ liệu chưa
				if (db.Products.Any(p => p.Name == product.Name))
				{
					ModelState.AddModelError("Name", "Tên sản phẩm đã tồn tại trong cơ sở dữ liệu.");
					ViewBag.ProductCategoryID = new SelectList(db.ProductCategories, "ProductCategoryID", "Name", product.ProductCategoryID);
					return View(product);
				}

				// Set CreatedDate to current datetime
				product.CreatedDate = DateTime.Now;

				// Set ModifiedDate to current datetime
				product.ModifiedDate = DateTime.Now;

				// Set Alias using FilterChar method
				product.Alias = BanHang.Models.Common.Filter.FilterChar(product.Name);

				db.Products.Add(product);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewBag.ProductCategoryID = new SelectList(db.ProductCategories, "ProductCategoryID", "Name", product.ProductCategoryID);
			return View(product);
		}

		// GET: Admin/Products/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Product product = db.Products.Find(id);
			if (product == null)
			{
				return HttpNotFound();
			}
			ViewBag.ProductCategoryID = new SelectList(db.ProductCategories, "ProductCategoryID", "Name", product.ProductCategoryID);
			return View(product);
		}

		// POST: Admin/Products/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "IDProduct,Name,SeoTitle,Description,Detail,Image,Price,PriceSale,PriceOrigin,Size,Quantity,IsHome,IsSale,IsActive,IsHot,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ProductCategoryID,Alias")] Product product)
		{
			if (ModelState.IsValid)
			{
				var existingProduct = db.Products.Find(product.IDProduct);
				if (existingProduct != null)
				{
					product.CreatedDate = existingProduct.CreatedDate;
					product.CreatedBy = existingProduct.CreatedBy;
					product.ModifiedDate = DateTime.Now; // Cập nhật ModifiedDate mới

					product.Alias = BanHang.Models.Common.Filter.FilterChar(product.Name); // Tạo alias mới từ tên sản phẩm đã chỉnh sửa
					db.Entry(existingProduct).CurrentValues.SetValues(product);
					// Lưu các thay đổi vào cơ sở dữ liệu
					db.SaveChanges();
					return RedirectToAction("Index");
				}
				else
				{
					return HttpNotFound();
				}
			}
			ViewBag.ProductCategoryID = new SelectList(db.ProductCategories, "ProductCategoryID", "Name", product.ProductCategoryID);
			return View(product);
		}

		// GET: Admin/Products/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Product product = db.Products.Find(id);
			if (product == null)
			{
				return HttpNotFound();
			}
			return View(product);
		}

		// POST: Admin/Products/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Product product = db.Products.Find(id);
			db.Products.Remove(product);
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

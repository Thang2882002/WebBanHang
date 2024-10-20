using BanHang.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHang.Controllers
{
	public class ShoppingCartController : Controller
	{
		private BanHangContext db = new BanHangContext();
		// GET: ShoppingCart
		public ActionResult Index()
		{
			ShoppingCart cart = (ShoppingCart)Session["Cart"];
			if (cart != null && cart.Items.Any())
			{
				ViewBag.CheckCart = cart;
			}
			return View();
		}

		public ActionResult Checkout()
		{
			return RedirectToAction("Create", "Order");
		}


		public ActionResult Partial_Item_Cart()
		{
			ShoppingCart cart = (ShoppingCart)Session["Cart"];
			if (cart != null && cart.Items.Any())
			{
				return PartialView(cart.Items);
			}
			return PartialView();
		}

		public ActionResult Partial_Item_ThanhToan()
		{
			ShoppingCart cart = (ShoppingCart)Session["Cart"];
			if (cart != null && cart.Items.Any())
			{
				return PartialView(cart.Items);
			}
			return PartialView();
		}
		public ActionResult ShowCount()
		{
			ShoppingCart cart = (ShoppingCart)Session["Cart"];
			if (cart != null)
			{
				return Json(new { Count = cart.Items.Count }, JsonRequestBehavior.AllowGet);
			}
			return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Partial_CheckOut()
		{
			return PartialView();
		}

		[HttpPost]
		public ActionResult AddToCart(int id, int quantity, string size)
		{
			var code = new { Success = false, msg = "", code = -1, Count = 0 };
			var db = new BanHangContext();
			var checkProduct = db.Products.FirstOrDefault(x => x.IDProduct == id);
			if (checkProduct != null)
			{
				ShoppingCart cart = (ShoppingCart)Session["Cart"];
				if (cart == null)
				{
					cart = new ShoppingCart();
				}
				ShoppingCartItem item = new ShoppingCartItem
				{
					ProductId = checkProduct.IDProduct,
					ProductName = checkProduct.Name,
					ProductImg = checkProduct.Image,
					CategoryName = checkProduct.ProductCategory.Name,
					Alias = checkProduct.Alias,
					Quantity = quantity,
					Size = size
				};
				if (checkProduct.ProductImages.FirstOrDefault(x => x.IsDefault == true) != null)
				{
					item.ProductImg = checkProduct.ProductImages.FirstOrDefault(x => x.IsDefault==true).Image;
				}

				// Kiểm tra và gán giá trị cho item.Price
				if (checkProduct.PriceSale > 0)
				{
					item.Price = (decimal)checkProduct.PriceSale;
				}
				else
				{
					item.Price = (decimal)(checkProduct.Price ?? 0);
				}

				item.TotalPrice = item.Quantity * item.Price;
				cart.AddToCart(item, quantity);
				Session["Cart"] = cart;
				code = new { Success = true, msg = "Thêm sản phẩm vào giỏ hàng thành công!", code = 1, Count = cart.Items.Count };
			}

			return Json(code);
		}

		[HttpPost]
		public ActionResult Update(int id, int quantity)
		{
			ShoppingCart cart = (ShoppingCart)Session["Cart"];
			if (cart != null)
			{
				cart.UpdateQuantity(id, quantity);
				return Json(new { Success = true });
			}
			return Json(new { Success = false });
		}
		[HttpPost]
		public ActionResult Delete(int id)
		{
			var code = new { Success = false, msg = "", code = -1, Count = 0 };

			ShoppingCart cart = (ShoppingCart)Session["Cart"];
			if (cart != null)
			{
				var checkProduct = cart.Items.FirstOrDefault(x => x.ProductId == id);
				if (checkProduct != null)
				{
					cart.Remove(id);
					code = new { Success = true, msg = "", code = 1, Count = cart.Items.Count };
				}
			}
			return Json(code);
		}



		[HttpPost]
		public ActionResult DeleteAll()
		{
			ShoppingCart cart = (ShoppingCart)Session["Cart"];
			if (cart != null)
			{
				cart.ClearCart();
				return Json(new { Success = true });
			}
			return Json(new { Success = false });
		}
	}
}
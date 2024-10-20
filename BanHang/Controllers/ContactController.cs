using BanHang.Models;
using System;
using System.Data.Entity.Infrastructure;
using System.Web.Mvc;

namespace BanHang.Controllers
{
	public class ContactController : Controller
	{
		private BanHangContext db = new BanHangContext();

		// GET: Contact
		public ActionResult Index()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index([Bind(Include = "IDContact,Name,Email,Phone,Message,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate")] Contact contact)
		{
			if (ModelState.IsValid)
			{
				contact.CreatedDate = DateTime.Now;
				contact.ModifiedDate = DateTime.Now;
				db.Contacts.Add(contact);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(contact);
		}

	}
}

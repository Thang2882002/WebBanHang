using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace BanHang.Models
{
	public partial class BanHangContext : DbContext
	{
		public BanHangContext()
			: base("name=BanHangContext")
		{
		}

		public virtual DbSet<Category> Categories { get; set; }
		public virtual DbSet<Contact> Contacts { get; set; }
		
		public virtual DbSet<News> Post { get; set; }
		public virtual DbSet<Order> Orders { get; set; }
		public virtual DbSet<Order_Details> Order_Details { get; set; }
		public virtual DbSet<Product> Products { get; set; }
		public virtual DbSet<ProductCategory> ProductCategories { get; set; }
		public virtual DbSet<ProductImage> ProductImages { get; set; }
		public virtual DbSet<Role> Roles { get; set; }
		public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
		public virtual DbSet<User> Users { get; set; }
		public virtual DbSet<Post> Posts { get; set; }
		

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			

			modelBuilder.Entity<Order>()
				.Property(e => e.OrderCode)
				.IsUnicode(false);

			modelBuilder.Entity<Order>()
				.Property(e => e.Phone)
				.IsUnicode(false);

			modelBuilder.Entity<Order>()
				.Property(e => e.Email)
				.IsUnicode(false);

			modelBuilder.Entity<Order>()
				.HasMany(e => e.Order_Details)
				.WithRequired(e => e.Order)
				.WillCascadeOnDelete(true);

			modelBuilder.Entity<Product>()
				.Property(e => e.SeoTitle)
				.IsUnicode(false);

			modelBuilder.Entity<Product>()
				.Property(e => e.Size)
				.IsUnicode(false);

			modelBuilder.Entity<Product>()
				.HasMany(e => e.Order_Details)
				.WithRequired(e => e.Product)
				.WillCascadeOnDelete(true);

			modelBuilder.Entity<ProductCategory>()
				.Property(e => e.SeoTitle)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.UserName)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.Password)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.Phone)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.Email)
				.IsUnicode(false);

			
		}
	}
}

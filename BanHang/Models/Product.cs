namespace BanHang.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Entity.Spatial;
	using System.Web.Mvc;

	[Table("Product")]
	public partial class Product
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Product()
		{
			
			Order_Details = new HashSet<Order_Details>();
			ProductImages = new HashSet<ProductImage>();
		}

		[Key]
		[DisplayName("ID Sản Phẩm")]
		public int IDProduct { get; set; }

		[StringLength(50)]
		[DisplayName("Tên")]
		[Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
		public string Name { get; set; }

		[StringLength(255)]
		[DisplayName("Tiêu đề SEO")]
		public string SeoTitle { get; set; }

		[DisplayName("Mô tả")]
		[AllowHtml]
		public string Description { get; set; }
		[AllowHtml]

		[DisplayName("Chi tiết")]
		public string Detail { get; set; }

		[DisplayName("Hình ảnh")]
		public string Image { get; set; }

		[DisplayName("Giá")]
		
		public decimal? Price { get; set; }

		[DisplayName("Giá Khuyến Mãi")]
		
		public decimal? PriceSale { get; set; }
		[DisplayName("Giá gốc")]
		
		public decimal? PriceOrigin { get; set; }

		[StringLength(10)]
		[DisplayName("Kích thước")]
		public string Size { get; set; }

		[DisplayName("Số lượng")]
		public int? Quantity { get; set; }

		[DisplayName("Hiển thị trang chủ")]
		public bool? IsHome { get; set; }

		[DisplayName("Sản phẩm khuyến mãi")]
		public bool? IsSale { get; set; }

		[DisplayName("Hoạt động")]
		public bool? IsActive { get; set; }

		[DisplayName("Sản phẩm nổi bật")]
		public bool? IsHot { get; set; }

		[StringLength(50)]
		[DisplayName("Người tạo")]
		public string CreatedBy { get; set; }

		[DisplayName("Ngày tạo")]
		public DateTime? CreatedDate { get; set; }

		[StringLength(50)]
		[DisplayName("Người sửa đổi")]
		public string ModifiedBy { get; set; }

		[DisplayName("Ngày sửa đổi")]
		public DateTime? ModifiedDate { get; set; }

		[DisplayName("ID Danh mục sản phẩm")]
		public int? ProductCategoryID { get; set; }

		[DisplayName("Alias")]
		public string Alias { get; set; }

		

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Order_Details> Order_Details { get; set; }

		public virtual ProductCategory ProductCategory { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<ProductImage> ProductImages { get; set; }
	}
}

namespace BanHang.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Entity.Spatial;

	[Table("ProductCategory")]
	public partial class ProductCategory
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public ProductCategory()
		{
			Products = new HashSet<Product>();
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[DisplayName("Mã Danh Mục Sản Phẩm")]
		public int ProductCategoryID { get; set; }

		[StringLength(50)]
		[DisplayName("Tên")]
		public string Name { get; set; }

		[StringLength(255)]
		[DisplayName("Tiêu đề SEO")]
		public string SeoTitle { get; set; }
		[DisplayName("Hình ảnh")]
		public string Image { get; set; }

		[StringLength(50)]
		[DisplayName("Người Tạo")]
		public string CreatedBy { get; set; }

		[DisplayName("Ngày Tạo")]
		public DateTime? CreatedDate { get; set; }

		[StringLength(50)]
		[DisplayName("Người Sửa Đổi")]
		public string ModifiedBy { get; set; }

		[DisplayName("Ngày Sửa Đổi")]
		public DateTime? ModifiedDate { get; set; }

		[DisplayName("Đường Dẫn")]
		public string Alias { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Product> Products { get; set; }
	}
}

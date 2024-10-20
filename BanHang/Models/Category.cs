namespace BanHang.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Entity.Spatial;

	[Table("Category")]
	public partial class Category
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Category()
		{
			News = new HashSet<News>();
			Posts = new HashSet<Post>();
		}

		public int CategoryID { get; set; }

		[Display(Name = "Tiêu đề")]
		[StringLength(50)]
		[Required(ErrorMessage = "Tiêu đề bắt buộc phải nhập ")]
		public string Title { get; set; }

		[Display(Name = "Mô tả")]
		public string Description { get; set; }

		[Display(Name = "Người tạo")]
		[StringLength(50)]
		public string CreatedBy { get; set; }

		[Display(Name = "Ngày tạo")]
		public DateTime? CreatedDate { get; set; }

		[Display(Name = "Người sửa đổi")]
		[StringLength(50)]
		public string ModifiedBy { get; set; }

		[Display(Name = "Ngày sửa đổi")]
		public DateTime? ModifiedDate { get; set; }

		[Display(Name = "Alias")]
		public string Alias { get; set; }

		[Display(Name = "Vị trí")]
		public int? Position { get; set; } // Thuộc tính Vị trí

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<News> News { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Post> Posts { get; set; }
	}
}

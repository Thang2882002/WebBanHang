using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace BanHang.Models
{
	public partial class Post
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int IDPosts { get; set; }

		[DisplayName("Tiêu đề")]
		[StringLength(250)]
		[Required(ErrorMessage = "Tiêu đề bắt buộc phải nhập ")]
		public string Title { get; set; }

		[DisplayName("Mô tả")]
		public string Descriptiom { get; set; }

		[DisplayName("Chi tiết")]
		[AllowHtml]
		public string Detail { get; set; }

		[DisplayName("Ảnh")]
		public string Image { get; set; }

		[DisplayName("ID Danh mục")]
		public int? CategoryID { get; set; }

		[DisplayName("Người tạo")]
		[StringLength(50)]
		public string CreatedBy { get; set; }

		[DisplayName("Ngày tạo")]
		public DateTime? CreatedDate { get; set; }

		[DisplayName("Người sửa đổi")]
		[StringLength(50)]
		public string ModifiedBy { get; set; }

		[DisplayName("Ngày sửa đổi")]
		public DateTime? ModifiedDate { get; set; }

		[DisplayName("Alias")]
		public string Alias { get; set; }

		[DisplayName("Trạng Thái")]
		public bool? IsActive { get; set; }

		public virtual Category Category { get; set; }
	}
}

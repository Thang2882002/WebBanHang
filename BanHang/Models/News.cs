using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BanHang.Models
{
	public partial class News
	{
		[Key]
		public int IDNews { get; set; }

		[StringLength(250)]
		[DisplayName("Tiêu đề")]
		[Required(ErrorMessage = "Tiêu đề bắt buộc phải nhập ")]
		public string Title { get; set; }

		[DisplayName("Mô tả")]
		public string Descriptiom { get; set; }

		[DisplayName("Chi tiết")]
		[AllowHtml]
		public string Detail { get; set; }

		[DisplayName("Hình ảnh")]
		public string Image { get; set; }

		[DisplayName("ID Danh mục")]
		public int? CategoryID { get; set; }

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

		[DisplayName("Alias")]
		public string Alias { get; set; }

		[DisplayName("Trạng thái hoạt động")]
		public bool? IsActive { get; set; }

		public virtual Category Category { get; set; }
	}
}

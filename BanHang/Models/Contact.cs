using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHang.Models
{
	[Table("Contact")]
	public partial class Contact
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int IDContact { get; set; }
		[Display(Name = "Tên Khách Hàng")]
		[StringLength(50)]
		public string Name { get; set; }

		[Display(Name = "Email")]
		[StringLength(150)]
		[EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
		public string Email { get; set; }

		[Display(Name = "Điện thoại")]
		[StringLength(10)]
		[RegularExpression(@"^\d{10}$", ErrorMessage = "Định dạng số điện thoại không hợp lệ.")]
		public string Phone { get; set; }
		[Display(Name = "Tin Nhắn")]

		[StringLength(4000)]
		public string Message { get; set; }

		[Display(Name = "Người Tạo")]

		[StringLength(50)]
		public string CreatedBy { get; set; }
		[Display(Name = "Ngày Tạo")]

		public DateTime? CreatedDate { get; set; }
		[Display(Name = "Người Chỉnh Sửa")]

		[StringLength(50)]
		public string ModifiedBy { get; set; }
		[Display(Name = "Ngày Chỉnh Sửa")]

		public DateTime? ModifiedDate { get; set; }


	}
}

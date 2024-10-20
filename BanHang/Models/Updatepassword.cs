using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BanHang.Models
{
	public class Updatepassword
	{
		[Display(Name = "Mật khẩu hiện tại")]
		[Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
		public string CurrentPassword { get; set; }

		[Display(Name = "Mật khẩu mới")]
		[Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
		[MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
		public string NewPassword { get; set; }

		[Display(Name = "Xác nhận mật khẩu mới")]
		[Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới.")]
		[Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không khớp.")]
		public string ConfirmPassword { get; set; }
	}
}
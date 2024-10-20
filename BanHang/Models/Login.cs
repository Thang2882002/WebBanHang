using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BanHang.Models
{
	public class Login
	{
		[Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
		[Display(Name = "Tên đăng nhập")]
		public string Username { get; set; }
		[Display(Name = "Mật khẩu")]

		[Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
		public string Password { get; set; }
	}
}
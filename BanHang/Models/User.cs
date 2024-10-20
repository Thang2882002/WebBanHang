namespace BanHang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            
            
        }

        public int UserID { get; set; }

		[Display(Name = "Tên đăng nhập")]
		[Required(ErrorMessage = "Vui lòng nhập tên người dùng.")]
		[StringLength(50)]
		public string UserName { get; set; }
		[Display(Name = "Mật khẩu")]
		[Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]

		public string Password { get; set; }

		[Display(Name = "Họ tên")]
		[StringLength(50)]
		public string Name { get; set; }

		[Display(Name = "Điện thoại")]
		[StringLength(10)]
		[RegularExpression(@"^\d{10}$", ErrorMessage = "Định dạng số điện thoại không hợp lệ.")]
		public string Phone { get; set; }

		[Display(Name = "Địa chỉ")]
		[StringLength(150)]
		public string Address { get; set; }

		[Display(Name = "Email")]
		[StringLength(50)]
		[EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
		public string Email { get; set; }

        public int? IDRole { get; set; }

        public bool? Status { get; set; }

        

        

        public virtual Role Role { get; set; }
    }
}

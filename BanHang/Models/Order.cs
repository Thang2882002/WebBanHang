namespace BanHang.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Entity.Spatial;

	[Table("Order")]
	public partial class Order
	{
		internal Random rd;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Order()
		{
			Order_Details = new HashSet<Order_Details>();
		}

		public int OrderID { get; set; }
		[Display(Name = "Mã Đơn Hàng")]

		[StringLength(10)]
		public string OrderCode { get; set; }
		[Display(Name = "Tên Khách Hàng")]

		public string CustomerName { get; set; }

		[Display(Name = "Điện thoại")]
		[StringLength(10)]
		[RegularExpression(@"^\d{10}$", ErrorMessage = "Định dạng số điện thoại không hợp lệ.")]
		public string Phone { get; set; }
		[Display(Name = "Đia Chỉ")]

		public string Address { get; set; }
		[Display(Name = "Tổng Tiền")]

		public decimal? TotalAmount { get; set; }

		[Display(Name = "Trạng Thái")]
		public bool? Status { get; set; }

		[StringLength(50)]
		[Display(Name = "Người Tạo")]
		public string CreatedBy { get; set; }
		[Display(Name = "Ngày Tạo")]
		public DateTime CreatedDate { get; set; }

		[StringLength(50)]
		[Display(Name = "Người Chỉnh Sửa")]
		public string ModifiedBy { get; set; }
		[Display(Name = "Ngày Chỉnh Sửa")]
		public DateTime? ModifiedDate { get; set; }
		[Display(Name = "Đơn Vị Vận Chuyển")]
		public string Shipping { get; set; }
		[Display(Name = "Phương thức thanh toán")]
		public int? TypePayment { get; set; }

		[Display(Name = "Email")]
		[StringLength(150)]
		[EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
		public string Email { get; set; }
		[Display(Name = "Vận Chuyển")]
		public bool? GiaoHang { get; set; }
		[Display(Name = "Nhận Hàng")]
		public bool? NhanHang { get; set; }
		[Display(Name = "Trạng Thái Thanh Toán")]
		public bool? StatusPayment { get; set; }
		[Display(Name = "Tên Đăng Nhập")]
		public string Username { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Order_Details> Order_Details { get; set; }
	}
}

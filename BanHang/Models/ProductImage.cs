namespace BanHang.Models
{
    using System;
    using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductImage")]
    public partial class ProductImage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductImageID { get; set; }
		[DisplayName("ID sản phẩm")]
		public int? IDProduct { get; set; }
		[DisplayName("Hình Ảnh")]
		public string Image { get; set; }
		[DisplayName("Trạng Thái")]
		public bool? IsDefault { get; set; }

        public virtual Product Product { get; set; }
    }
}

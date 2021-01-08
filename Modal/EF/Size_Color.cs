namespace Modal.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Size_Color
    {
        [Key]
        public int id_size_color { get; set; }

        public int? id_sanpham { get; set; }

        [StringLength(5)]
        public string size { get; set; }

        [StringLength(10)]
        public string color { get; set; }

        public int? soluong { get; set; }

        public virtual SanPham SanPham { get; set; }
    }
}

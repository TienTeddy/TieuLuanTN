namespace Modal.EF
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Context_ : DbContext
    {
        public Context_()
            : base("name=Context_")
        {
        }

        public virtual DbSet<HoaDon> HoaDons { get; set; }
        public virtual DbSet<HoaDonCT> HoaDonCTs { get; set; }
        public virtual DbSet<LoaiSanPham> LoaiSanPhams { get; set; }
        public virtual DbSet<NguoiBan> NguoiBans { get; set; }
        public virtual DbSet<NguoiMua> NguoiMuas { get; set; }
        public virtual DbSet<PhieuThanhToan> PhieuThanhToans { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SanPham> SanPhams { get; set; }
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }
        public virtual DbSet<TheTich> TheTichs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HoaDon>()
                .Property(e => e.mahd)
                .IsUnicode(false);

            modelBuilder.Entity<LoaiSanPham>()
                .Property(e => e.hinhanh)
                .IsUnicode(false);

            modelBuilder.Entity<LoaiSanPham>()
                .HasMany(e => e.SanPham)
                .WithOptional(e => e.LoaiSanPham)
                .WillCascadeOnDelete();

            modelBuilder.Entity<NguoiBan>()
                .Property(e => e.taikhoanng)
                .IsUnicode(false);

            modelBuilder.Entity<NguoiMua>()
                .HasMany(e => e.HoaDon)
                .WithOptional(e => e.NguoiMua)
                .WillCascadeOnDelete();

            modelBuilder.Entity<SanPham>()
                .Property(e => e.barcode)
                .IsUnicode(false);

            modelBuilder.Entity<SanPham>()
                .Property(e => e.hinhanh1)
                .IsUnicode(false);

            modelBuilder.Entity<SanPham>()
                .Property(e => e.hinhanh2)
                .IsUnicode(false);

            modelBuilder.Entity<SanPham>()
                .Property(e => e.hinhanh3)
                .IsUnicode(false);

            modelBuilder.Entity<SanPham>()
                .Property(e => e.hinhanh4)
                .IsUnicode(false);

            modelBuilder.Entity<SanPham>()
                .Property(e => e.size)
                .IsUnicode(false);

            modelBuilder.Entity<SanPham>()
                .HasMany(e => e.TheTich)
                .WithOptional(e => e.SanPham)
                .WillCascadeOnDelete();

            modelBuilder.Entity<TaiKhoan>()
                .Property(e => e.username)
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoan>()
                .Property(e => e.password)
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoan>()
                .Property(e => e.password_old)
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoan>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoan>()
                .HasMany(e => e.NguoiBan)
                .WithOptional(e => e.TaiKhoan)
                .WillCascadeOnDelete();

            modelBuilder.Entity<TaiKhoan>()
                .HasMany(e => e.NguoiMua)
                .WithOptional(e => e.TaiKhoan)
                .WillCascadeOnDelete();
        }
    }
}

create database pingpings
go

use pingpings 
go

--create table NhaSanXuat --vd 1 nhà sản xuất tự đăng thì sao? cách 1 là tạo 1 nsx 'Tư Nhân'
--(
--	id_nhasx int identity primary key,
--	tennsx nvarchar(50),
--	thongtin nvarchar(max),
--	hinhanh varchar(max)
--) --
--go

create table LoaiSanPham
(
	id_loaisp int identity primary key,
	--id_nhasx int,
	tenloai nvarchar(50),
	tenngan nvarchar(50),
	thongtin nvarchar(max),
	hinhanh varchar(max),
	xeploai nvarchar(100) check(xeploai in('Trending Item','Hot Item','Onsale','Best Saller','Top Viewed')) default 'Hot Item',
	theloai nvarchar(20)
)
alter table SanPham
add  xeploai nvarchar(100) check(xeploai in('Trending Item','Hot Item','Onsale','Best Saller','Top Viewed')) default 'Hot Item'
create table SanPham
(
	id_sanpham int identity primary key,
	tensp nvarchar(200),
	id_loaisp int,
	tenngan nvarchar(50),
	soluong int,
	dongia float,
	giasale float,
	trangthai nvarchar(5) check(trangthai in(N'CŨ',N'MỚI')) default N'MỚI',
	hienthi nvarchar(5) check(hienthi in(N'ẨN',N'HIỆN')) default N'HIỆN',
	barcode varchar(100),
	tinhtrang nvarchar(10) check(tinhtrang in(N'CÒN HÀNG ',N'SẮP HẾT',N'HẾT HÀNG',N'TỒN KHO ĐÃ LÂU',N'HÀNG LỖI')),
	thongtin nvarchar(max),
	hinhanh1 varchar(max),
	hinhanh2 varchar(max),
	hinhanh3 varchar(max),
	hinhanh4 varchar(max),
	size varchar(2)

	CONSTRAINT FK_SanPham_LoaiSanPham FOREIGN KEY (id_loaisp) REFERENCES LoaiSanPham (id_loaisp) ON DELETE CASCADE,
	--id_danhgia int,
	--id_thetich int
	--id_binhluan int
)
--create table DanhGia
--(
--	id_danhgia int identity primary key,
--	id_sanpham int,
--	danhgia nvarchar(11) check(danhgia in (N'TỆ',N'TRUNG BÌNH',N'TỐT',N'RẤT TỐT')),
--	thoigian datetime,
--	id_nguoimua int
--)
create table TheTich
(
	id_thetich int identity primary key,
	id_sanpham int unique, --duy nhất
	chieucao float,
	chieurong float,
	chieudai float,
	cannang float
	CONSTRAINT FK_TheTich_SanPham FOREIGN KEY (id_sanpham) REFERENCES SanPham (id_sanpham) ON DELETE CASCADE,
)
--create table BinhLuan
--(
--	id_binhluan int identity primary key,
--	id_sanpham int,
--	noidung nvarchar(2500),
--	hienthi nvarchar(5) check(hienthi in(N'ẨN',N'HIỆN')) default N'HIỆN',
--	id_nguoimua int, --ko lien kết
--	id_nguoiban int  --ko liên kết
--)

create table TaiKhoan
(
	id_taikhoan int identity primary key,
	username varchar(250),
	hoten nvarchar(150),
	password varchar(250),
	password_old varchar(250), -- default=password
	email varchar(250),
	loaitk bit default 1, -- 1=người mua, 0=người bán, 0=người quản lý
	--tgchuyen datetime --update trở thành người bán hàng
)

create table NguoiMua
(
	id_nguoimua int identity primary key,
	id_taikhoan int,
	phone int,
	street nvarchar(100),
	ward nvarchar(100),
	district nvarchar(100),
	province nvarchar(100),
	CONSTRAINT FK_NguoiMua_TaiKhoan FOREIGN KEY (id_taikhoan) REFERENCES TaiKhoan (id_taikhoan) ON DELETE CASCADE,
)
create table NguoiBan
(
	id_nguoiban int identity primary key,
	id_taikhoan int,
	taikhoanng varchar(100),
	nganhang nvarchar(100),
	phone int,
	street nvarchar(100),
	ward nvarchar(100),
	district nvarchar(100),
	province nvarchar(100),
	CONSTRAINT FK_NguoiBan_TaiKhoan FOREIGN KEY (id_taikhoan) REFERENCES TaiKhoan (id_taikhoan) ON DELETE CASCADE,
)

create table HoaDon
(
	id_hoadon int identity primary key,
	id_sanpham int,
	id_loaisp int,
	id_nguoimua int,
	mahd varchar(100),
	tonggia float,
	thoigian datetime,
	hinhthuctt nvarchar(100),
	soluong int,
	freeship float,
	trangthai nvarchar(50) check(trangthai in(N'Chưa Thanh Toán',N'Đã Thanh Toán')) default N'Chưa Thanh Toán',
	CONSTRAINT FK_HoaDon_NguoiMua FOREIGN KEY (id_nguoimua) REFERENCES NguoiMua (id_nguoimua) ON DELETE CASCADE,
)

create table HoaDonCT
(
	id_hoadonct int identity primary key,
	id_hoadon int,
	id_sanpham int,
	dongia float,
	thoigian datetime, --hdct add đồng thời sau hd
	soluong int,
	trangthai nvarchar(50) check(trangthai in(N'Chưa Thanh Toán',N'Đã Thanh Toán')) default N'Chưa Thanh Toán',
	size varchar(5), --chỉ lấy size
	color nvarchar(10), --chỉ lấy màu
	CONSTRAINT FK_HoaDonCT_HoaDon FOREIGN KEY (id_hoadon) REFERENCES HoaDon (id_hoadon) ON DELETE CASCADE,
	CONSTRAINT FK_HoaDonCT_SanPham FOREIGN KEY (id_sanpham) REFERENCES SanPham (id_sanpham) ON DELETE CASCADE,
)
alter table HoaDonCT
add id_size int check(trangthai in(N'Chưa Thanh Toán',N'Đã Thanh Toán')) default N'Chưa Thanh Toán',
create table PhieuThanhToan
(
	id_phieutt int identity primary key,
	id_hoadon int,
	tensp nvarchar(250),
	soluong int,
	dongia float,
	thoigian datetime,
	trangthai nvarchar(50) check(trangthai in(N'Chưa Thanh Toán',N'Đã Thanh Toán')) default N'Đã Thanh Toán'
)

create table Sale
(
	id_sale int identity primary key,
	id_sanpham int,
	thoigianbd datetime default GETDATE(),
	thoigiankt datetime,
	sale int, --%
	thoigianc datetime,
	trangthai nvarchar(50) check(trangthai in(N'Hoạt Động',N'Ngưng Hoạt Động')) default N'Hoạt Động'
	CONSTRAINT FK_Sale_SanPham FOREIGN KEY (id_sanpham) REFERENCES SanPham (id_sanpham)
)
go
create table Size
(
	id_size int identity primary key,
	id_sanpham int,
	size varchar(5),
	soluong int
	CONSTRAINT FK_Size_SanPham FOREIGN KEY (id_sanpham) REFERENCES SanPham (id_sanpham)
)
create table Color
(
	id_color int identity primary key,
	id_size int,
	color nvarchar(10),
	soluong int
	CONSTRAINT FK_Color_Size FOREIGN KEY (id_size) REFERENCES Size (id_size)
)

-- xử lý sp trùng nhau trong hoadonct
--CREATE TRIGGER trg_hdct_sp ON HoaDonCT AFTER INSERT AS 
--BEGIN
--	UPDATE HoaDonCT
--	SET soluong = soluong + (
--		SELECT soluong
--		FROM inserted
--		WHERE id_sanpham = HoaDonCT.id_sanpham
--		), thoigian=(
--		SELECT thoigian
--		FROM inserted
--		WHERE id_sanpham = HoaDonCT.id_sanpham
--		)
--	FROM HoaDonCT
--	JOIN inserted ON HoaDonCT.id_sanpham = inserted.id_sanpham
--END
--GO
select *from SanPham
select *from Size
select *from HoaDon
select *from HoaDonCT

delete from HoaDon
delete from HoaDonCT 
# HỆ THỐNG QUẢN LÝ SINH VIÊN VÀ LỚP HỌC

Hệ thống quản lý sinh viên, giảng viên, môn học, lớp học, lịch học, đăng ký môn và điểm số được xây dựng bằng **ASP.NET Core 8 MVC**.

## 🚀 CÔNG NGHỆ SỬ DỤNG

- **Framework**: ASP.NET Core 8 MVC
- **Authentication**: Cookie-based Authentication
- **Authorization**: Role-based (Admin, Lecturer, Student)
- **Database**: Fake Database (List<T> static) - Không sử dụng SQL Server/EF Core
- **UI**: Razor Views + Bootstrap 5 + Bootstrap Icons
- **Animation**: CSS Animation & Transition

## 📁 CẤU TRÚC DỰ ÁN

```
WebApplication1/
├── Areas/
│   ├── Admin/
│   │   ├── Controllers/
│   │   │   ├── DashboardController.cs
│   │   │   ├── StudentController.cs
│   │   │   ├── EnrollmentController.cs
│   │   │   └── ...
│   │   └── Views/
│   ├── Lecturer/
│   │   ├── Controllers/
│   │   └── Views/
│   └── Student/
│       ├── Controllers/
│       └── Views/
├── Controllers/
│   ├── HomeController.cs
│   └── AccountController.cs
├── Data/
│   └── FakeDatabase.cs
├── Middleware/
│   └── AuthMiddleware.cs
├── Models/
│   ├── User.cs
│   ├── Student.cs
│   ├── Lecturer.cs
│   ├── Subject.cs
│   ├── CourseClass.cs
│   ├── Schedule.cs
│   ├── Enrollment.cs
│   ├── Grade.cs
│   ├── Attendance.cs
│   └── Notification.cs
├── Services/
│   ├── AuthService.cs
│   ├── UserService.cs
│   ├── StudentService.cs
│   ├── LecturerService.cs
│   ├── SubjectService.cs
│   ├── CourseClassService.cs
│   ├── ScheduleService.cs
│   ├── EnrollmentService.cs
│   ├── GradeService.cs
│   └── NotificationService.cs
├── ViewModels/
│   ├── AuthViewModels.cs
│   ├── StudentViewModels.cs
│   ├── LecturerViewModels.cs
│   └── ...
└── Views/
    ├── Shared/
    │   ├── _PublicLayout.cshtml
    │   └── _DashboardLayout.cshtml
    ├── Home/
    └── Account/
```

## 👥 VAI TRÒ HỆ THỐNG

### 1. **Admin** (Quản trị viên)
- Quản lý tài khoản người dùng
- Quản lý sinh viên, giảng viên
- Quản lý môn học, lớp học phần
- Quản lý lịch học
- Duyệt/từ chối đăng ký môn học
- Quản lý điểm số

### 2. **Lecturer** (Giảng viên)
- Xem danh sách lớp đang dạy
- Xem danh sách sinh viên trong lớp
- Điểm danh sinh viên
- Nhập điểm (chuyên cần, giữa kỳ, cuối kỳ)
- Xem lịch giảng dạy
- Gửi thông báo cho lớp

### 3. **Student** (Sinh viên)
- Xem hồ sơ cá nhân
- Đăng ký môn học
- Xem thời khóa biểu
- Xem điểm số và GPA
- Xem thông báo
- Xem danh sách lớp đang học

## 🔐 TÀI KHOẢN DEMO

### Admin
- **Email**: `admin@university.edu.vn`
- **Password**: `admin123`

### Giảng viên
- **Email**: `nguyenvana@university.edu.vn`
- **Password**: `lecturer123`

- **Email**: `tranthib@university.edu.vn`
- **Password**: `lecturer123`

### Sinh viên
- **Email**: `phamvand@student.edu.vn`
- **Password**: `student123`

- **Email**: `hoangthie@student.edu.vn`
- **Password**: `student123`

## 🎯 CHỨC NĂNG CHÍNH

### Authentication & Authorization
- ✅ Đăng nhập với Email & Password
- ✅ Đăng ký tài khoản mới (mặc định role Student)
- ✅ Quên mật khẩu (reset về 123456)
- ✅ Đổi mật khẩu
- ✅ Cookie-based Authentication
- ✅ Role-based Authorization Middleware
- ✅ Tự động redirect theo role sau khi đăng nhập

### Admin Module
- ✅ Dashboard tổng quan hệ thống
- ✅ Quản lý User (CRUD, đổi role, khóa/mở tài khoản)
- ✅ Quản lý Student (CRUD, gán lớp hành chính)
- ✅ Quản lý Lecturer (CRUD)
- ✅ Quản lý Subject (CRUD, môn tiên quyết)
- ✅ Quản lý CourseClass (CRUD, gán giảng viên, phòng học)
- ✅ Quản lý Schedule (CRUD, tạo/sửa/xóa lịch học)
- ✅ Quản lý Enrollment (duyệt/từ chối đăng ký)
- ✅ Quản lý Grade (xem điểm theo lớp/sinh viên)

### Lecturer Module
- ✅ Dashboard giảng viên
- ✅ Xem danh sách lớp đang dạy
- ✅ Xem chi tiết lớp học và danh sách sinh viên
- ✅ Xem danh sách sinh viên trong lớp
- ✅ Nhập điểm (chuyên cần 10%, giữa kỳ 30%, cuối kỳ 60%)
- ✅ Tự động tính điểm tổng kết và điểm chữ (A, B+, B, C+, C, D+, D, F)
- ✅ Xem lịch giảng dạy
- ✅ Gửi thông báo cho sinh viên trong lớp

### Student Module
- ✅ Dashboard sinh viên với thống kê
- ✅ Xem hồ sơ cá nhân
- ✅ Đăng ký môn học
  - Kiểm tra môn tiên quyết
  - Kiểm tra lớp đầy
  - Kiểm tra trùng lịch
  - Trạng thái: Pending → Admin duyệt → Approved
- ✅ Xem thời khóa biểu theo học kỳ
- ✅ Xem bảng điểm chi tiết
- ✅ Xem GPA (tính theo tín chỉ)
- ✅ Hủy đăng ký (khi còn Pending)

### Notification System
- ✅ Icon chuông trên header với số thông báo chưa đọc
- ✅ Dropdown hiển thị 5 thông báo gần nhất
- ✅ Tự động tạo thông báo khi:
  - Đăng ký môn thành công/thất bại
  - Lịch học thay đổi
  - Có điểm mới
  - Giảng viên gửi thông báo lớp
- ✅ Hiển thị thời gian tương đối (vừa xong, 5 phút trước...)

## 🎨 GIAO DIỆN

### Public Pages (Chưa đăng nhập)
- **Layout**: `_PublicLayout.cshtml`
- **Đặc điểm**:
  - Header đơn giản
  - Footer
  - Background gradient đẹp mắt
  - Không có sidebar
  - 2 Card lớn: Đăng nhập & Đăng ký
  - Animation: fade in, scale on hover

### Dashboard Pages (Sau khi đăng nhập)
- **Layout**: `_DashboardLayout.cshtml`
- **Đặc điểm**:
  - Sidebar bên trái (thay đổi theo role)
  - Header với notification bell & user profile
  - Main content area
  - Sidebar có animation collapse/hover
  - Cards với shadow & hover effect
  - Tables responsive với hover effect
  - Alert messages với slide down animation

### Theme & Colors
- **Primary Color**: Gradient #667eea → #764ba2
- **Font**: Segoe UI
- **Icons**: Bootstrap Icons
- **Buttons**: Gradient background với shadow on hover
- **Cards**: Border-radius 12px, shadow, transform on hover

## 📊 FAKE DATABASE

Hệ thống sử dụng **FakeDatabase** với dữ liệu mẫu:
- **9 Users**: 1 Admin, 3 Lecturers, 5 Students
- **5 Students**: SV001-SV005
- **3 Lecturers**: GV001-GV003
- **2 Administrative Classes**: CNTT-K17A, QTKD-K18A
- **6 Subjects**: IT001-IT004, BA001-BA002
- **6 Course Classes**: Học kỳ HK1-2024
- **6+ Schedules**: Lịch học theo thứ và ca
- **8 Enrollments**: Với các trạng thái khác nhau
- **4 Grades**: Một số có điểm, một số chưa
- **4 Attendances**: Bản ghi điểm danh mẫu
- **5 Notifications**: Thông báo mẫu

## 🏃 CÁCH CHẠY DỰ ÁN

### Yêu cầu
- .NET 8 SDK
- Visual Studio 2022 hoặc VS Code
- Trình duyệt web hiện đại

### Các bước
1. **Clone/Download dự án**
2. **Mở solution trong Visual Studio**
3. **Restore packages** (tự động)
4. **Build solution**: `Ctrl + Shift + B`
5. **Run**: `F5` hoặc `Ctrl + F5`
6. **Truy cập**: `https://localhost:xxxxx`

### Luồng test
1. Vào trang chủ → Click "Đăng nhập"
2. Đăng nhập với tài khoản demo
3. Hệ thống tự động redirect theo role:
   - Admin → `/Admin/Dashboard`
   - Lecturer → `/Lecturer/Dashboard`
   - Student → `/Student/Dashboard`

## 📝 LƯU Ý

### Authentication Flow
- Chưa đăng nhập: Chỉ truy cập được `/`, `/Account/Login`, `/Account/Register`
- Đã đăng nhập: Tự động redirect về dashboard tương ứng
- Truy cập sai quyền: HTTP 403 Forbidden

### Grade Calculation
- **Attendance**: 10%
- **Midterm**: 30%
- **Final**: 60%
- **Letter Grade**:
  - A: ≥ 8.5
  - B+: ≥ 8.0
  - B: ≥ 7.0
  - C+: ≥ 6.5
  - C: ≥ 5.5
  - D+: ≥ 5.0
  - D: ≥ 4.0
  - F: < 4.0

### GPA Calculation
- Tính theo công thức: `GPA = Σ(Điểm × Tín chỉ) / Σ(Tín chỉ)`
- Chỉ tính các môn đã có điểm tổng kết
- Hiển thị với 2 chữ số thập phân

## 🎓 MỤC ĐÍCH

Dự án này được xây dựng phục vụ mục đích **HỌC TẬP**, demo đầy đủ các tính năng của một hệ thống quản lý sinh viên thực tế với:
- Kiến trúc MVC chuẩn
- Clean code, dễ đọc, dễ bảo trì
- UI/UX đẹp mắt, chuyên nghiệp
- Animation & Transition mượt mà
- Business logic đầy đủ
- Role-based Authorization

## 📞 HỖ TRỢ

Nếu có bất kỳ câu hỏi nào về dự án, vui lòng tạo issue hoặc liên hệ.

---

**Copyright © 2024 - Student Management System**

# ?? H??NG D?N KH?C PH?C L?I & C?I THI?N

## ? ?Ã KH?C PH?C

### 1. **L?i 404 Not Found khi ??ng nh?p**
**Nguyên nhân**: Thi?u Views cho các Controller

**?ã t?o thêm:**
- ? `Areas/Admin/Views/Student/Index.cshtml`
- ? `Areas/Admin/Views/Enrollment/Index.cshtml`
- ? `Areas/Lecturer/Views/CourseClass/Index.cshtml`
- ? `Areas/Lecturer/Views/Schedule/Index.cshtml`
- ? `Areas/Lecturer/Controllers/ScheduleController.cs`
- ? `Areas/Student/Views/Schedule/Index.cshtml`

### 2. **Footer không sát d??i cùng**
**?ã fix:**
- Layout s? d?ng Flexbox v?i `display: flex; flex-direction: column`
- Content area có `flex: 1` và `min-height: calc(100vh - 150px)`
- Main content có `height: 100%`

### 3. **L?i chính t? trong UI**
?ã ???c xem xét và code s? d?ng ti?ng Vi?t chu?n.

---

## ?? H??NG D?N S? D?NG SAU KHI FIX

### B??c 1: Build l?i project
```bash
dotnet build
```

### B??c 2: Ch?y ?ng d?ng
```bash
dotnet run
```
ho?c nh?n `F5` trong Visual Studio

### B??c 3: Truy c?p và test
1. M? trình duy?t: `https://localhost:xxxxx`
2. Test ??ng nh?p v?i t?ng role:

#### Test Admin:
```
Email: admin@university.edu.vn
Password: admin123
```
- Sau khi ??ng nh?p ? Redirect ??n `/Admin/Dashboard`
- Click sidebar menu ?? test các ch?c n?ng:
  - ? Dashboard
  - ? Qu?n lý sinh viên
  - ? Qu?n lý ??ng ký (duy?t/t? ch?i)

#### Test Lecturer:
```
Email: nguyenvana@university.edu.vn
Password: lecturer123
```
- Sau khi ??ng nh?p ? Redirect ??n `/Lecturer/Dashboard`
- Test các ch?c n?ng:
  - ? Xem l?p h?c ph?n
  - ? Xem l?ch gi?ng d?y
  - ? Nh?p ?i?m (click vào l?p ? Nh?p ?i?m)

#### Test Student:
```
Email: phamvand@student.edu.vn
Password: student123
```
- Sau khi ??ng nh?p ? Redirect ??n `/Student/Dashboard`
- Test các ch?c n?ng:
  - ? Dashboard v?i th?ng kê
  - ? ??ng ký môn h?c
  - ? Xem th?i khóa bi?u
  - ? Xem b?ng ?i?m

---

## ?? CÁC CONTROLLER VÀ VIEW CÒN THI?U (N?U C?N M? R?NG)

### Admin Area - Views còn thi?u:
```
Areas/Admin/Views/
??? User/
?   ??? Index.cshtml           (C?n t?o)
?   ??? Create.cshtml          (C?n t?o)
?   ??? Edit.cshtml            (C?n t?o)
??? Student/
?   ??? Create.cshtml          (C?n t?o)
?   ??? Edit.cshtml            (C?n t?o)
?   ??? Details.cshtml         (C?n t?o)
??? Lecturer/
?   ??? Index.cshtml           (C?n t?o)
?   ??? Create.cshtml          (C?n t?o)
?   ??? Edit.cshtml            (C?n t?o)
??? Subject/
?   ??? Index.cshtml           (C?n t?o)
?   ??? Create.cshtml          (C?n t?o)
?   ??? Edit.cshtml            (C?n t?o)
??? CourseClass/
?   ??? Index.cshtml           (C?n t?o)
?   ??? Create.cshtml          (C?n t?o)
?   ??? Details.cshtml         (C?n t?o)
??? Schedule/
    ??? Index.cshtml           (C?n t?o)
    ??? Create.cshtml          (C?n t?o)
```

### Lecturer Area - Views còn thi?u:
```
Areas/Lecturer/Views/
??? CourseClass/
?   ??? Details.cshtml         (C?n t?o)
??? Grade/
    ??? CourseClass.cshtml     (C?n t?o)
```

---

## ?? CÁCH T?O NHANH CÁC CONTROLLER/VIEW CÒN THI?U

### Ví d? t?o UserController cho Admin:

```csharp
// Areas/Admin/Controllers/UserController.cs
[Area("Admin")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    public IActionResult Index()
    {
        var users = _userService.GetAll();
        return View(users);
    }

    // CRUD methods...
}
```

### Pattern cho View Index:
```html
@model List<WebApplication1.ViewModels.UserListViewModel>
@{
    ViewData["Title"] = "Qu?n lý ng??i dùng";
    ViewData["PageTitle"] = "Qu?n lý ng??i dùng";
    Layout = "~/Views/Shared/_DashboardLayout.cshtml";
}

@section SidebarMenu {
    <!-- Copy sidebar menu t? Dashboard/Index.cshtml -->
}

<!-- Content -->
<div class="card">
    <div class="card-header bg-white">
        <h5>Danh sách ng??i dùng</h5>
    </div>
    <div class="card-body">
        <!-- Table ho?c content -->
    </div>
</div>
```

---

## ?? UI/UX IMPROVEMENTS ?Ã TH?C HI?N

### 1. Layout Improvements:
- ? Footer luôn ? d??i cùng (flexbox)
- ? Sidebar fixed v?i scroll khi n?i dung dài
- ? Content area có min-height ?? ??y footer xu?ng

### 2. Animation & Transitions:
- ? Card hover effects
- ? Button hover v?i shadow
- ? Sidebar menu hover v?i translateX
- ? Alert slide down animation
- ? Table row hover

### 3. Responsive Design:
- ? Bootstrap 5 responsive classes
- ? Table responsive wrapper
- ? Mobile-friendly navigation

---

## ?? KI?M TRA CH?C N?NG

### Checklist Admin:
- [x] Dashboard hi?n th? th?ng kê
- [x] Xem danh sách sinh viên
- [x] Xem danh sách ??ng ký
- [x] Duy?t/T? ch?i ??ng ký
- [ ] CRUD User (Views ch?a có)
- [ ] CRUD Subject (Views ch?a có)
- [ ] CRUD CourseClass (Views ch?a có)

### Checklist Lecturer:
- [x] Dashboard
- [x] Xem danh sách l?p
- [x] Xem l?ch gi?ng d?y
- [ ] Chi ti?t l?p h?c (View ch?a có)
- [ ] Nh?p ?i?m (View ch?a có)

### Checklist Student:
- [x] Dashboard v?i th?ng kê
- [x] ??ng ký môn h?c
- [x] Xem th?i khóa bi?u
- [x] Xem b?ng ?i?m
- [x] H?y ??ng ký môn

---

## ?? G?I Ý M? R?NG

### 1. Thêm tính n?ng Export:
- Export danh sách sinh viên ra Excel
- Export b?ng ?i?m ra PDF

### 2. Thêm tính n?ng Search/Filter:
- Tìm ki?m sinh viên theo tên, mã SV
- L?c ??ng ký theo h?c k?
- L?c l?p theo tr?ng thái

### 3. Thêm Dashboard Charts:
- Bi?u ?? th?ng kê s? sinh viên theo khoa
- Bi?u ?? phân b? ?i?m
- Bi?u ?? t? l? ??ng ký môn h?c

### 4. Notification Real-time:
- SignalR ?? push notification real-time
- Email notification khi có ?i?m m?i

---

## ?? H? TR?

N?u g?p l?i:
1. Check Console trong trình duy?t (F12)
2. Check Output window trong Visual Studio
3. ??m b?o ?ã build thành công
4. Clear cache trình duy?t (Ctrl + Shift + Delete)

**Các l?i th??ng g?p:**
- 404 Not Found ? View ch?a ???c t?o
- 403 Forbidden ? Không có quy?n truy c?p
- 500 Server Error ? Check stack trace trong Output

---

**H? th?ng ?ã hoàn thi?n các ch?c n?ng c? b?n và s?n sàng demo!** ??

# C?P NH?T CH?C NÃNG GI?NG VIÊN

## ?? T?NG QUAN

Ð? b? sung ð?y ð? các ch?c nãng c?n thi?u cho module Gi?ng viên (Lecturer), ð?c bi?t là:
- ? View nh?p ði?m cho l?p h?c
- ? View xem chi ti?t l?p h?c và danh sách sinh viên
- ? **Menu "Manage Grades" trong sidebar** ? M?I
- ? Trang danh sách l?p ð? ch?n l?p nh?p ði?m
- ? C?p nh?t Controller ð? l?y ð?y ð? thông tin môn h?c

## ?? CÁC FILE Ð? T?O M?I

### 1. **Areas/Lecturer/Views/Grade/Index.cshtml** ? M?I
**M?c ðích**: Trang danh sách l?p ð? ch?n l?p c?n nh?p ði?m

**Tính nãng**:
- Hi?n th? t?t c? l?p gi?ng viên ðang d?y d?ng grid
- M?i l?p hi?n th?:
  - Tên môn h?c, m? l?p
  - H?c k?, ph?ng h?c
  - S? lý?ng sinh viên
  - Tr?ng thái l?p
- Button "Enter/Update Grades" trên m?i l?p
- Alert hý?ng d?n s? d?ng

**Lu?ng s? d?ng**:
```
Sidebar "Manage Grades" ? Danh sách l?p ? Ch?n l?p ? Nh?p ði?m
```

### 2. **Areas/Lecturer/Views/Grade/CourseClass.cshtml**
**M?c ðích**: View ð? gi?ng viên nh?p ði?m cho sinh viên trong l?p

**Tính nãng**:
- Hi?n th? danh sách sinh viên trong l?p d?ng b?ng
- Nh?p ði?m chuyên c?n (10%), gi?a k? (30%), cu?i k? (60%)
- T? ð?ng tính ði?m t?ng k?t khi nh?p ði?m
- T? ð?ng tính ði?m ch? (A, B+, B, C+, C, D+, D, F)
- JavaScript real-time tính toán khi thay ð?i ði?m
- Validation cho ði?m t? 0-10
- Hi?n th? công th?c tính ði?m r? ràng
- Form submit ð? lýu hàng lo?t ði?m

**UI/UX**:
- Badge màu s?c theo ði?m ch? (A = xanh lá, F = ð?, etc.)
- Table responsive v?i hover effect
- Alert thông báo công th?c tính ði?m
- Back button v? danh sách l?p

### 3. **Areas/Lecturer/Views/CourseClass/Details.cshtml**
**M?c ðích**: View ð? xem chi ti?t l?p h?c ph?n

**Tính nãng**:
- Hi?n th? thông tin ð?y ð? v? l?p h?c:
  - M? l?p, môn h?c, s? tín ch?
  - Gi?ng viên ph? trách
  - H?c k?, ph?ng h?c
  - S? lý?ng sinh viên (v?i progress bar)
  - Tr?ng thái l?p
- Hi?n th? l?ch h?c c?a l?p theo t?ng bu?i
- Danh sách sinh viên kèm ði?m:
  - M? sinh viên, h? tên, email
  - Tr?ng thái ðãng k? (Approved/Pending)
  - Ði?m s? và ði?m ch? (n?u có)
- Button nhanh ð? nh?p ði?m

**UI/UX**:
- Layout 2 c?t: Thông tin l?p bên trái, danh sách sinh viên bên ph?i
- Badge màu s?c theo tr?ng thái
- Card l?ch h?c riêng bi?t v?i icon calendar
- Table sinh viên responsive

## ?? CÁC FILE Ð? C?P NH?T

### 1. **Areas/Lecturer/Controllers/GradeController.cs**
**Thay ð?i**:
- ? Thêm `ISubjectService` vào constructor
- ? Thêm action `Index()` ð? hi?n th? danh sách l?p ? M?I
- ? C?p nh?t method `CourseClass()` ð? l?y thông tin môn h?c
- ? Populate ð?y ð? `SubjectName` vào ViewModel

**Code m?i**:
```csharp
// GET: Lecturer/Grade/Index
public IActionResult Index()
{
    var userIdClaim = User.FindFirst("UserId");
    if (userIdClaim == null)
    {
        return RedirectToAction("Login", "Account");
    }

    var userId = int.Parse(userIdClaim.Value);
    var lecturer = _lecturerService.GetByUserId(userId);

    if (lecturer == null)
    {
        return NotFound();
    }

    var classes = _courseClassService.GetByLecturerId(lecturer.Id);
    return View(classes);
}
```

### 2. **T?t c? Views c?a Lecturer**
**C?p nh?t sidebar menu ? các file**:
- ? `Areas/Lecturer/Views/Dashboard/Index.cshtml`
- ? `Areas/Lecturer/Views/CourseClass/Index.cshtml`
- ? `Areas/Lecturer/Views/CourseClass/Details.cshtml`
- ? `Areas/Lecturer/Views/Grade/Index.cshtml`
- ? `Areas/Lecturer/Views/Grade/CourseClass.cshtml`
- ? `Areas/Lecturer/Views/Schedule/Index.cshtml`

**Sidebar menu m?i**:
```razor
@section SidebarMenu {
    <li class="nav-item">
        <a class="nav-link" asp-area="Lecturer" asp-controller="Dashboard" asp-action="Index">
            <i class="bi bi-house-door"></i>Home
        </a>
    </li>
    <li class="nav-item">
        <a class="nav-link" asp-area="Lecturer" asp-controller="CourseClass" asp-action="Index">
            <i class="bi bi-journal-text"></i>Course Classes
        </a>
    </li>
    <li class="nav-item">
        <a class="nav-link" asp-area="Lecturer" asp-controller="Grade" asp-action="Index">
            <i class="bi bi-pencil-square"></i>Manage Grades  ? M?I
        </a>
    </li>
    <li class="nav-item">
        <a class="nav-link" asp-area="Lecturer" asp-controller="Schedule" asp-action="Index">
            <i class="bi bi-calendar3"></i>Teaching Schedule
        </a>
    </li>
}
```

### 3. **README.md**
**C?p nh?t**:
- B? sung d?ng "? Xem chi ti?t l?p h?c và danh sách sinh viên" trong Lecturer Module
- Xác nh?n ð?y ð? các tính nãng ð? hoàn thành

## ?? SIDEBAR MENU GI?NG VIÊN - HOÀN CH?NH

```
???????????????????????????????
? ?? QLSV System              ?
???????????????????????????????
? ?? Home                     ? /Lecturer/Dashboard
? ?? Course Classes           ? /Lecturer/CourseClass/Index
? ?? Manage Grades  ?        ? /Lecturer/Grade/Index  ? M?I
? ?? Teaching Schedule        ? /Lecturer/Schedule/Index
???????????????????????????????
```

**Menu luôn hi?n th? ? t?t c? trang c?a Lecturer!**

## ?? LU?NG S? D?NG

### Cách 1: Qua Menu "Manage Grades" (? M?I - KHUY?N NGH?)
```
1. Click "Manage Grades" trên sidebar
   ? /Lecturer/Grade/Index
   ? Hi?n th? danh sách T?T C? l?p ðang d?y

2. Ch?n l?p c?n nh?p ði?m
   ? Click "Enter/Update Grades"
   ? /Lecturer/Grade/CourseClass/{id}

3. Nh?p ði?m cho sinh viên
   ? Ði?n ði?m vào form
   ? Click "Save Grades"
   ? T? ð?ng t?o notification cho sinh viên
```

### Cách 2: Qua "Course Classes"
```
1. Click "Course Classes" trên sidebar
   ? /Lecturer/CourseClass/Index

2. Ch?n l?p:
   - Click "View Details" ? Xem chi ti?t
   - Click "Enter Grades" ? Nh?p ði?m tr?c ti?p

3. Nh?p ði?m
```

### Cách 3: Qua Dashboard
```
1. Dashboard ? Quick Actions
2. Click "View Course Classes"
3. Ch?n l?p và nh?p ði?m
```

## ?? ÐI?M N?I B?T

### 1. Menu C? Ð?nh ?
- **Manage Grades** gi? ðây luôn hi?n th? trên sidebar
- Không c?n vào Course Classes m?i th?y
- Truy c?p nhanh ch? v?i 1 click
- Active state r? ràng

### 2. Trang Ch?n L?p
- Hi?n th? grid ð?p m?t
- Thông tin ð?y ð? v? t?ng l?p
- Button r? ràng "Enter/Update Grades"
- Responsive trên m?i thi?t b?

### 3. Real-time Grade Calculation
JavaScript t? ð?ng tính toán khi nh?p ði?m:
```javascript
// Auto-calculate total and letter grade when input changes
document.querySelectorAll('input[type="number"]').forEach(input => {
    input.addEventListener('input', function() {
        // Tính total = attendance * 0.1 + midterm * 0.3 + final * 0.6
        // Xác ð?nh letter grade
        // C?p nh?t UI ngay l?p t?c
    });
});
```

### 4. Color-coded Grades
Badge màu s?c tr?c quan:
- **A**: Xanh lá (#10b981) - Xu?t s?c
- **B+, B**: Xanh dýõng (#3b82f6) - Khá gi?i
- **C+, C**: Cam (#f59e0b) - Trung b?nh
- **D+, D**: Xám (#6b7280) - Y?u
- **F**: Ð? (#ef4444) - Không ð?t

### 5. Progress Bar Enrollment
Thanh progress bar hi?n th? t? l? sinh viên ðãng k?:
```razor
<div class="progress" style="height: 25px;">
    <div class="progress-bar" role="progressbar" 
         style="width: @((Model.CurrentStudents * 100.0 / Model.MaxStudents).ToString("F0"))%">
        @Model.CurrentStudents / @Model.MaxStudents
    </div>
</div>
```

## ? KI?M TRA HOÀN T?T

### Checklist Gi?ng viên:
- [x] Dashboard hi?n th? th?ng kê
- [x] Xem danh sách l?p ðang d?y
- [x] Xem chi ti?t l?p h?c và danh sách sinh viên
- [x] Xem l?ch gi?ng d?y
- [x] **Menu "Manage Grades" c? ð?nh trên sidebar** ? M?I
- [x] **Trang danh sách l?p ð? ch?n l?p nh?p ði?m** ? M?I
- [x] Nh?p ði?m (chuyên c?n, gi?a k?, cu?i k?)
- [x] T? ð?ng tính ði?m t?ng k?t và ði?m ch?
- [x] Validation ði?m 0-10
- [x] Lýu ði?m hàng lo?t
- [x] T?o notification cho sinh viên khi có ði?m m?i

## ?? DEMO TÀI KHO?N

### Gi?ng viên 1:
```
Email: nguyenvana@university.edu.vn
Password: lecturer123
```

**L?p ðang d?y**:
- IT101-01: Nh?p môn l?p tr?nh (IT001)
- IT102-01: C?u trúc d? li?u (IT002)

### Gi?ng viên 2:
```
Email: tranthib@university.edu.vn
Password: lecturer123
```

**L?p ðang d?y**:
- IT201-01: Cõ s? d? li?u (IT003)

## ?? HÝ?NG D?N TEST

### Test 1: Truy c?p qua Menu "Manage Grades" ? M?I
1. Ðãng nh?p v?i tài kho?n gi?ng viên
2. Click "Manage Grades" trên sidebar
3. Xem danh sách t?t c? l?p
4. Click "Enter/Update Grades" trên m?t l?p
5. Nh?p ði?m cho sinh viên
6. Click "Save Grades"
7. Ki?m tra thông báo thành công

### Test 2: Nh?p ði?m
1. Vào "Manage Grades"
2. Ch?n m?t l?p
3. Nh?p ði?m cho sinh viên (0-10)
4. Quan sát Total Score và Letter Grade t? ð?ng c?p nh?t
5. Click "Save Grades"
6. Ki?m tra thông báo thành công

### Test 3: Xem chi ti?t l?p
1. Vào "Course Classes"
2. Click "View Details" trên m?t l?p
3. Ki?m tra thông tin l?p hi?n th? ð?y ð?
4. Xem l?ch h?c c?a l?p
5. Xem danh sách sinh viên và ði?m (n?u có)
6. Click "Enter Grades" ð? nh?y sang trang nh?p ði?m

### Test 4: Real-time Calculation
1. Vào trang nh?p ði?m
2. Nh?p ði?m t?ng ph?n (Attendance, Midterm, Final)
3. Quan sát Total Score t? ð?ng tính
4. Quan sát Letter Grade t? ð?ng ð?i màu
5. Th? nh?p ði?m khác nhau ð? xem k?t qu?

### Test 5: Ki?m tra Sidebar ? M?I
1. Ðãng nh?p gi?ng viên
2. Ki?m tra sidebar ? m?i trang:
   - Dashboard
   - Course Classes
   - Manage Grades
   - Teaching Schedule
3. Xác nh?n menu "Manage Grades" luôn hi?n th?
4. Click menu ð? chuy?n trang
5. Ki?m tra active state ðúng

## ?? NOTIFICATION FLOW

Khi gi?ng viên lýu ði?m:
1. `GradeService.UpdateGrade()` ðý?c g?i
2. Tính ði?m t?ng k?t và ði?m ch?
3. G?i `NotificationService.CreateNotification()`
4. T?o notification cho sinh viên:
   - Title: "Có ði?m m?i"
   - Message: "Ði?m s? c?a b?n ð? ðý?c c?p nh?t..."
   - Link: "/Student/Grades"
5. Sinh viên s? th?y thông báo ? icon chuông trên header

## ?? K?T LU?N

Module Gi?ng viên ð? ðý?c hoàn thi?n **100%** v?i ð?y ð? các tính nãng:
- ? Xem thông tin l?p h?c
- ? Xem danh sách sinh viên
- ? **Menu "Manage Grades" c? ð?nh trên sidebar** ? HOÀN THÀNH
- ? **Trang danh sách l?p ð? ch?n l?p** ? HOÀN THÀNH
- ? Nh?p và qu?n l? ði?m
- ? Xem l?ch gi?ng d?y
- ? UI/UX chuyên nghi?p v?i animation
- ? Real-time calculation
- ? Notification system
- ? B?o m?t và ki?m tra quy?n

**Gi?ng viên gi? có th? truy c?p ch?c nãng nh?p ði?m ngay t? sidebar!** ??

---

## ?? SCREENSHOTS (Mô t?)

### 1. Sidebar v?i Menu "Manage Grades"
```
???????????????????????
? ?? Home            ?
? ?? Course Classes  ?
? ?? Manage Grades ? ? ? M?I
? ?? Teaching Sched. ?
???????????????????????
```

### 2. Trang Manage Grades (Danh sách l?p)
```
+-----------------------------------------------+
| Select a Class to Manage Grades               |
+-----------------------------------------------+
| ?? Please select a class below to enter...   |
+-----------------------------------------------+
| [Card 1]              [Card 2]                |
| IT101-01             | IT102-01              |
| Nh?p môn l?p tr?nh   | C?u trúc d? li?u      |
| [Enter/Update Grades]| [Enter/Update Grades] |
+-----------------------------------------------+
```

### 3. Trang Nh?p Ði?m
```
+-----------------------------------------------+
| Enter Grades - IT101-01                       |
+-----------------------------------------------+
| ?? Grading Formula: Attendance (10%) + ...   |
+-----------------------------------------------+
| STT | Code  | Name      | Attend | Mid | Fin |
|  1  | SV001 | Ph?m Vãn D|   8.0  | 7.5 | 8.0 |
|  2  | SV002 | Hoàng Th? E|  9.0  | 8.0 | 8.5 |
+-----------------------------------------------+
| [Save Grades] [Cancel]                        |
+-----------------------------------------------+
```

**H? th?ng ð? s?n sàng ð? demo ð?y ð? 100%!** ??

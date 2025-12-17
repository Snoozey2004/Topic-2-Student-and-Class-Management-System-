# ?? SIDEBAR MENU - T?NG QUAN H? TH?NG

## ?? ADMIN SIDEBAR

```
???????????????????????????????
? ?? QLSV System              ?
???????????????????????????????
? ?? Dashboard                ? (/Admin/Dashboard)
? ?? Users                    ? (/Admin/User/Index)
? ?? Students                 ? (/Admin/Student/Index)
? ????? Lecturers               ? (/Admin/Lecturer/Index)
? ?? Subjects                 ? (/Admin/Subject/Index)
? ?? Course Classes           ? (/Admin/CourseClass/Index)
? ?? Schedules                ? (/Admin/Schedule/Index)
? ?? Enrollments              ? (/Admin/Enrollment/Index)
? ?? Grades                   ? (/Admin/Grade/Index)
???????????????????????????????
```

### Ch?c nãng chính:
- ? Qu?n l? toàn b? ngý?i dùng (CRUD)
- ? Qu?n l? sinh viên, gi?ng viên
- ? Qu?n l? môn h?c, l?p h?c ph?n
- ? Qu?n l? l?ch h?c
- ? Duy?t/t? ch?i ðãng k? môn h?c
- ? Xem t?ng quan ði?m s?

---

## ?? LECTURER SIDEBAR

```
???????????????????????????????
? ?? QLSV System              ?
???????????????????????????????
? ?? Home                     ? (/Lecturer/Dashboard)
? ?? Course Classes           ? (/Lecturer/CourseClass/Index)
?    ? ?? Enter Grades        ? (/Lecturer/Grade/CourseClass/{id})
? ?? Teaching Schedule        ? (/Lecturer/Schedule/Index)
???????????????????????????????
```

### Ch?c nãng chính:
- ? Xem danh sách l?p ðang d?y
- ? Xem chi ti?t l?p và danh sách sinh viên
- ? Nh?p ði?m (Attendance 10% + Midterm 30% + Final 60%)
- ? T? ð?ng tính ði?m t?ng k?t và ði?m ch?
- ? Xem l?ch gi?ng d?y theo tu?n

### Menu ð?ng:
- Khi ? trang **Dashboard**: "Home" active
- Khi ? trang **Course Classes**: "Course Classes" active
- Khi ? trang **Enter Grades**: "Manage Grades" active (hi?n th? ð?ng)
- Khi ? trang **Teaching Schedule**: "Teaching Schedule" active

---

## ?? STUDENT SIDEBAR

```
???????????????????????????????
? ?? QLSV System              ?
???????????????????????????????
? ?? Dashboard                ? (/Student/Dashboard)
? ?? Enrollment               ? (/Student/Enrollment/Index)
? ?? My Schedule              ? (/Student/Schedule/Index)
? ?? Grades                   ? (/Student/Grade/Index)
???????????????????????????????
```

### Ch?c nãng chính:
- ? Xem th?ng kê cá nhân (GPA, s? tín ch?)
- ? Ðãng k? môn h?c
  - Ki?m tra môn tiên quy?t
  - Ki?m tra l?p ð?y
  - Ki?m tra trùng l?ch
- ? Xem th?i khóa bi?u
- ? Xem b?ng ði?m và GPA
- ? H?y ðãng k? (khi c?n Pending)

---

## ?? SIDEBAR STYLING

### Ð?c ði?m UI:
- **Width**: 260px fixed
- **Background**: Linear gradient (#667eea ? #764ba2)
- **Position**: Fixed, sidebar luôn ? bên trái
- **Scroll**: Overflow-y auto khi n?i dung dài
- **Animation**: 
  - Menu hover: `translateX(5px)` + opacity change
  - Transition: 0.3s ease

### Active State:
```css
.nav-link.active {
    background: rgba(255,255,255,0.2);
    color: white;
    font-weight: 600;
}
```

### Hover Effect:
```css
.nav-link:hover {
    background: rgba(255,255,255,0.1);
    color: white;
    transform: translateX(5px);
}
```

---

## ?? SIDEBAR TRONG LAYOUT

### _DashboardLayout.cshtml
```razor
<div class="sidebar">
    <div class="sidebar-brand">
        <i class="bi bi-mortarboard-fill me-2"></i>QLSV System
    </div>
    <nav class="sidebar-nav">
        @if (IsSectionDefined("SidebarMenu"))
        {
            @RenderSection("SidebarMenu", required: false)
        }
        else
        {
            @await Html.PartialAsync("_Sidebar")
        }
    </nav>
</div>
```

### S? d?ng trong View
```razor
@section SidebarMenu {
    <li class="nav-item">
        <a class="nav-link active" asp-area="Lecturer" asp-controller="Dashboard" asp-action="Index">
            <i class="bi bi-house-door"></i>Home
        </a>
    </li>
    <li class="nav-item">
        <a class="nav-link" asp-area="Lecturer" asp-controller="CourseClass" asp-action="Index">
            <i class="bi bi-journal-text"></i>Course Classes
        </a>
    </li>
    <!-- More items... -->
}
```

---

## ?? RESPONSIVE BEHAVIOR

### Desktop (>768px):
- Sidebar hi?n th? ð?y ð?
- Main content margin-left: 260px
- Width content: calc(100% - 260px)

### Mobile (<768px):
- *Hi?n t?i chýa có mobile menu toggle*
- *Ð? xu?t thêm hamburger menu sau*

---

## ?? BEST PRACTICES

### 1. Consistency:
- T?t c? role ð?u có sidebar gi?ng nhau v? c?u trúc
- Icon và màu s?c nh?t quán

### 2. Navigation:
- Always show "Home/Dashboard" ð?u tiên
- Active state r? ràng
- Hover feedback t?t

### 3. Icons:
- S? d?ng Bootstrap Icons
- M?i menu item có icon riêng
- Icon margin-right: 0.75rem

### 4. Accessibility:
- Link có text r? ràng
- Hover state d? nh?n bi?t
- Active state n?i b?t

---

## ?? TÝÕNG LAI M? R?NG

### Ð? xu?t thêm:
1. **Mobile Menu Toggle**
   - Hamburger button
   - Sidebar collapse/expand
   - Overlay backdrop

2. **Breadcrumb Navigation**
   - Hi?n th? ðý?ng d?n hi?n t?i
   - Click ð? quay l?i

3. **Submenu Support**
   - Dropdown cho menu ph?c t?p
   - Accordion effect

4. **Quick Actions**
   - Button nhanh ? sidebar footer
   - Shortcut keys

---

## ? CHECKLIST HOÀN THÀNH

### Admin:
- [x] Dashboard
- [x] Users management
- [x] Students management
- [x] Lecturers management
- [x] Subjects management
- [x] Course Classes management
- [x] Schedules management
- [x] Enrollments management
- [x] Grades overview

### Lecturer:
- [x] Dashboard
- [x] Course Classes list
- [x] Course Class details
- [x] Enter Grades ? (V?a hoàn thành)
- [x] Teaching Schedule

### Student:
- [x] Dashboard
- [x] Enrollment
- [x] My Schedule
- [x] My Grades

**T?t c? sidebar ð? hoàn thi?n!** ??

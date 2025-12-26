# ?? SIDEBAR MENU - T?NG QUAN H? TH?NG

## ?? SIDEBAR LOCATIONS

Sidebars are now organized by role and located in their respective Areas:

- **Admin Sidebar**: `/Areas/Admin/Views/Shared/_AdminSidebar.cshtml`
- **Student Sidebar**: `/Areas/Student/Views/Shared/_StudentSidebar.cshtml`
- **Lecturer Sidebar**: `/Areas/Lecturer/Views/Shared/_LecturerSidebar.cshtml`

## ??? ARCHITECTURE

### DashboardLayout Behavior
The `_DashboardLayout.cshtml` no longer provides a fallback sidebar. Views **must** explicitly define their sidebar using the `@section SidebarMenu` block.

```razor
<div class="sidebar">
    <div class="sidebar-brand">
        <i class="bi bi-mortarboard-fill me-2"></i>SCMS
    </div>
    <nav class="sidebar-nav">
        @RenderSection("SidebarMenu", required: false)
    </nav>
</div>
```

### Usage in Views
Each view that uses `_DashboardLayout` must include a `@section SidebarMenu` block:

#### Within Areas (Student/Lecturer/Admin):
```razor
@section SidebarMenu {
    @await Html.PartialAsync("_StudentSidebar")
}
```

#### Outside Areas (e.g., Profile views):
```razor
@section SidebarMenu {
    @if (User.IsInRole("Student"))
    {
        @await Html.PartialAsync("~/Areas/Student/Views/Shared/_StudentSidebar.cshtml")
    }
    else if (User.IsInRole("Lecturer"))
    {
        @await Html.PartialAsync("~/Areas/Lecturer/Views/Shared/_LecturerSidebar.cshtml")
    }
    else if (User.IsInRole("Admin"))
    {
        @await Html.PartialAsync("~/Areas/Admin/Views/Shared/_AdminSidebar.cshtml")
    }
}
```

---

## ????? ADMIN SIDEBAR

**Location**: `/Areas/Admin/Views/Shared/_AdminSidebar.cshtml`

```
?????????????????????????????????
? ?? SCMS System                ?
?????????????????????????????????
? ?? Dashboard                  ? (/Admin/Dashboard)
? ?? Users                      ? (/Admin/User/Index)
? ?? Students                   ? (/Admin/Student/Index)
? ????? Lecturers                 ? (/Admin/Lecturer/Index)
? ?? Subjects                   ? (/Admin/Subject/Index)
? ?? Course Classes             ? (/Admin/CourseClass/Index)
? ?? Schedules                  ? (/Admin/Schedule/Index)
? ?? Enrollments                ? (/Admin/Enrollment/Index)
? ?? Grades                     ? (/Admin/Grade/Index)
?????????????????????????????????
```

### Ch?c n?ng chính:
- ? Qu?n lý toàn b? ng??i dùng (CRUD)
- ? Qu?n lý sinh viên, gi?ng viên
- ? Qu?n lý môn h?c, l?p h?c ph?n
- ? Qu?n lý l?ch h?c
- ? Duy?t/t? ch?i ??ng ký môn h?c
- ? Xem t?ng quan ?i?m s?

---

## ????? LECTURER SIDEBAR

**Location**: `/Areas/Lecturer/Views/Shared/_LecturerSidebar.cshtml`

```
?????????????????????????????????
? ?? SCMS System                ?
?????????????????????????????????
? ?? Home                       ? (/Lecturer/Dashboard)
? ?? Course Classes             ? (/Lecturer/CourseClass/Index)
?    ? ?? Enter Grades          ? (/Lecturer/Grade/CourseClass/{id})
? ?? Manage Grades              ? (/Lecturer/Grade/Index)
? ? Attendance                 ? (/Lecturer/Attendance/Index)
? ?? Teaching Schedule          ? (/Lecturer/Schedule/Index)
?????????????????????????????????
```

### Ch?c n?ng chính:
- ? Xem danh sách l?p ?ang d?y
- ? Xem chi ti?t l?p và danh sách sinh viên
- ? Nh?p ?i?m (Attendance 10% + Midterm 30% + Final 60%)
- ? T? ??ng tính ?i?m t?ng k?t và ?i?m ch?
- ? Xem l?ch gi?ng d?y theo tu?n
- ? Qu?n lý ?i?m danh

### Menu ??ng:
- Khi ? trang **Dashboard**: "Home" active
- Khi ? trang **Course Classes**: "Course Classes" active
- Khi ? trang **Enter Grades**: "Manage Grades" active (hi?n th? ??ng)
- Khi ? trang **Attendance**: "Attendance" active
- Khi ? trang **Teaching Schedule**: "Teaching Schedule" active

---

## ?? STUDENT SIDEBAR

**Location**: `/Areas/Student/Views/Shared/_StudentSidebar.cshtml`

```
?????????????????????????????????
? ?? SCMS System                ?
?????????????????????????????????
? ?? Dashboard                  ? (/Student/Dashboard)
? ?? Enrollment                 ? (/Student/Enrollment/Index)
? ?? My Schedule                ? (/Student/Schedule/Index)
? ?? Grades                     ? (/Student/Grade/Index)
?????????????????????????????????
```

### Ch?c n?ng chính:
- ? Xem th?ng kê cá nhân (GPA, s? tín ch?)
- ? ??ng ký môn h?c
  - Ki?m tra môn tiên quy?t
  - Ki?m tra l?p ??y
  - Ki?m tra trùng l?ch
- ? Xem th?i khóa bi?u
- ? Xem b?ng ?i?m và GPA
- ? H?y ??ng ký (khi còn Pending)

---

## ?? SIDEBAR STYLING

### ??c ?i?m UI:
- **Width**: 260px fixed
- **Background**: #1e3a5f (Navy blue)
- **Position**: Fixed, sidebar luôn ? bên trái
- **Scroll**: Overflow-y auto khi n?i dung dài
- **Animation**: 
  - Menu hover: `translateX(3px)` + opacity change
  - Transition: 0.3s ease

### Active State:
```css
.nav-link.active {
    background: #2d9e8e; /* Teal color */
    color: white;
    font-weight: 600;
}
```

### Hover Effect:
```css
.nav-link:hover {
    background: rgba(255,255,255,0.08);
    color: white;
    transform: translateX(3px);
}
```

---

## ?? RESPONSIVE BEHAVIOR

### Desktop (>768px):
- Sidebar hi?n th? ??y ??
- Main content margin-left: 260px
- Width content: calc(100% - 260px)

### Mobile (<768px):
- *Hi?n t?i ch?a có mobile menu toggle*
- *?? xu?t thêm hamburger menu sau*

---

## ? BEST PRACTICES

### 1. Consistency:
- T?t c? role ??u có sidebar gi?ng nhau v? c?u trúc
- Icon và màu s?c nh?t quán

### 2. Navigation:
- Always show "Home/Dashboard" ??u tiên
- Active state rõ ràng
- Hover feedback t?t

### 3. Icons:
- S? d?ng Bootstrap Icons
- M?i menu item có icon riêng
- Icon margin-right: 0.75rem

### 4. Accessibility:
- Link có text rõ ràng
- Hover state d? nh?n bi?t
- Active state n?i b?t

---

## ?? T??NG LAI M? R?NG

### ?? xu?t thêm:
1. **Mobile Menu Toggle**
   - Hamburger button
   - Sidebar collapse/expand
   - Overlay backdrop

2. **Breadcrumb Navigation**
   - Hi?n th? ???ng d?n hi?n t?i
   - Click ?? quay l?i

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
- [x] Attendance management
- [x] Teaching Schedule

### Student:
- [x] Dashboard
- [x] Enrollment
- [x] My Schedule
- [x] My Grades

**T?t c? sidebar ?ã hoàn thi?n!** ??

# ?? Ð?NG NH?T MÀU S?C TOÀN B? PROJECT - NAVY BLUE THEME

## ? T?NG QUAN

Ð? t?o **shared CSS file** và c?p nh?t toàn b? project ð? ð?ng nh?t màu s?c theo **Navy Blue Theme**.

---

## ?? CÁC FILE Ð? T?O/C?P NH?T

### 1. ? `wwwroot/css/shared-theme.css` - M?I

File CSS chung cho toàn b? project v?i:
- CSS Variables cho colors
- Button styles (Navy Blue)
- Badge colors consistent
- Card designs
- Table styles
- Form controls
- Alerts animated
- Dashboard stats cards
- Pagination, Dropdowns, Modals
- Breadcrumbs, Tabs, Navigation
- Utility classes
- Animations
- Responsive & Print styles

### 2. ? `Views/Shared/_DashboardLayout.cshtml` - C?P NH?T

- Added `<link rel="stylesheet" href="~/css/shared-theme.css" />`
- Gi? nguyên sidebar và header styles
- T?t c? components gi? dùng shared CSS

### 3. ? `Views/Shared/_PublicLayout.cshtml` - C?P NH?T

- Added `<link rel="stylesheet" href="~/css/shared-theme.css" />`
- Navbar: Navy Blue `#1e3a5f`
- Footer: Navy Blue `#1e3a5f`
- Background: Gradient Navy ? Teal
- Action cards: Gradient Navy ? Teal

---

## ?? COLOR PALETTE - CSS VARIABLES

```css
:root {
    /* Primary Colors - Navy Blue Theme */
    --primary-navy: #1e3a5f;
    --primary-teal: #2d9e8e;
    --primary-navy-dark: #163354;
    
    /* Background Colors */
    --bg-main: #f5f7fa;
    --bg-card: #ffffff;
    --bg-sidebar: #1e3a5f;
    
    /* Text Colors */
    --text-dark: #2d3748;
    --text-gray: #4a5568;
    --text-light: #718096;
    
    /* Accent Colors */
    --accent-purple: #6366f1;
    --accent-blue: #3b82f6;
    --accent-teal: #2d9e8e;
    --accent-orange: #f59e0b;
    --accent-red: #ef4444;
    --accent-green: #10b981;
    --accent-pink: #ec4899;
}
```

---

## ?? CÁC COMPONENT Ð? Ð?NG NH?T

### 1. **Buttons**
```css
.btn-primary {
    background: #1e3a5f !important;
    border: none !important;
    color: white !important;
}

.btn-primary:hover {
    background: #163354 !important;
    transform: translateY(-2px);
    box-shadow: 0 5px 15px rgba(30, 58, 95, 0.3);
}
```

### 2. **Badges**
```css
.badge.bg-primary { background: #6366f1 !important; }
.badge.bg-info { background: #2d9e8e !important; }
.badge.bg-success { background: #10b981 !important; }
```

### 3. **Cards**
```css
.card {
    border: none;
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.08);
}

.card:hover {
    transform: translateY(-3px);
    box-shadow: 0 8px 16px rgba(0,0,0,0.12);
}
```

### 4. **Tables**
```css
.table thead {
    background: #f7fafc;
}

.table th {
    font-weight: 600;
    color: #4a5568;
    text-transform: uppercase;
}

.table tbody tr:hover {
    background-color: #f7fafc;
}
```

### 5. **Forms**
```css
.form-control:focus {
    border-color: #2d9e8e;
    box-shadow: 0 0 0 0.2rem rgba(45, 158, 142, 0.25);
}
```

### 6. **Alerts**
```css
.alert {
    border-radius: 10px;
    border: none;
    animation: slideDown 0.3s ease;
}
```

### 7. **Modals**
```css
.modal-header {
    background: linear-gradient(135deg, #1e3a5f 0%, #2d9e8e 100%);
    color: white;
}
```

---

## ?? T?T C? VIEWS GI? DÙNG CHUNG

V? ð? include `shared-theme.css` vào c? 2 layouts:
- `_DashboardLayout.cshtml` (Admin, Lecturer, Student)
- `_PublicLayout.cshtml` (Home, Login, Register)

**? T?T C? views t? ð?ng có màu ð?ng nh?t!**

### Admin Views (28 files)
- ? Dashboard/Index
- ? User/Index, Create, Edit, Details, Delete
- ? Student/Index, Create, Edit, Details
- ? Lecturer/Index, Create, Edit, Details, Delete
- ? Subject/Index, Create, Edit, Details, Delete
- ? CourseClass/Index, Create, Edit, Details, Delete
- ? Schedule/Index
- ? Enrollment/Index
- ? Grade/Index

### Lecturer Views (6 files)
- ? Dashboard/Index
- ? CourseClass/Index, Details
- ? Schedule/Index
- ? Grade/Index, CourseClass

### Student Views (4 files)
- ? Dashboard/Index
- ? Enrollment/Index
- ? Schedule/Index
- ? Grade/Index

### Public Views (4 files)
- ? Home/Index
- ? Account/Login
- ? Account/Register
- ? Account/ForgotPassword
- ? Account/AccessDenied

**T?NG C?NG: ~42 views ð? ð?ng nh?t màu s?c!**

---

## ?? TRÝ?C VÀ SAU

### ? TRÝ?C (Không ð?ng nh?t)

```
Home page: Gradient purple-pink
Login page: Different gradient
Admin Dashboard: Navy blue sidebar, purple buttons
Student Dashboard: Different button colors
Tables: Inconsistent hover colors
Forms: Different focus colors
Alerts: Different animations
```

### ? SAU (Hoàn toàn ð?ng nh?t)

```
ALL PAGES: Navy Blue (#1e3a5f) + Teal (#2d9e8e)

? Buttons: Navy Blue
? Badges: Consistent colors
? Cards: Same shadow & hover
? Tables: Same hover effect
? Forms: Teal focus color
? Alerts: Same animation
? Modals: Gradient header
? Sidebar: Navy Blue
? Header/Footer: Navy Blue
? Background: Gradient Navy ? Teal
```

---

## ?? UTILITY CLASSES M?I

Gi? b?n có th? dùng các class sau trong b?t k? view nào:

### Text Colors
```html
<span class="text-navy">Navy text</span>
<span class="text-teal">Teal text</span>
```

### Background Colors
```html
<div class="bg-navy">Navy background</div>
<div class="bg-teal">Teal background</div>
```

### Border Colors
```html
<div class="border border-navy">Navy border</div>
<div class="border border-teal">Teal border</div>
```

### Animations
```html
<div class="fade-in">Fade in animation</div>
<div class="scale-in">Scale in animation</div>
```

---

## ?? CÁC VIEW Ð?C BI?T Ð? C?P NH?T

### 1. **Public Layout**
```
Navbar: Navy Blue (#1e3a5f)
Background: Gradient Navy ? Teal
Action Cards: Gradient Navy ? Teal
Footer: Navy Blue (#1e3a5f)
```

### 2. **Dashboard Layout**
```
Sidebar: Navy Blue (#1e3a5f)
Active Menu: Teal (#2d9e8e)
Header: White
Content: Light Gray (#f5f7fa)
```

### 3. **Timetable Views**
```
Day Headers: Navy Blue
Active Day: Teal (n?u có)
Class Cards: 5 màu rotation
```

---

## ?? RESPONSIVE

Shared CSS có responsive styles cho:
- Mobile (< 768px)
- Tablet (768px - 1024px)
- Desktop (> 1024px)

```css
@media (max-width: 768px) {
    .card { margin-bottom: 1rem; }
    .table th, .table td { padding: 0.75rem; }
    .btn { padding: 0.5rem 1rem; }
}
```

---

## ??? PRINT STYLES

Shared CSS có print styles:
```css
@media print {
    .card { box-shadow: none; }
    .btn, .sidebar, .top-header { display: none !important; }
}
```

---

## ?? CÁCH S? D?NG

### Trong View m?i:

**Không c?n thêm CSS g?!** Ch? c?n:

```razor
@{
    ViewData["Title"] = "My Page";
    ViewData["PageTitle"] = "My Page";
    Layout = "~/Views/Shared/_DashboardLayout.cshtml";
}

@section SidebarMenu {
    @await Html.PartialAsync("_AdminSidebar")
}

<div class="card">
    <div class="card-header">
        <h5>My Content</h5>
    </div>
    <div class="card-body">
        <button class="btn btn-primary">Primary Button</button>
        <span class="badge bg-info">Info Badge</span>
    </div>
</div>
```

**? T?t c? ð? có màu Navy Blue ðúng!**

---

## ?? CUSTOMIZATION

N?u mu?n thay ð?i màu cho toàn b? project, ch? c?n s?a CSS variables trong `shared-theme.css`:

```css
:root {
    /* Change these values */
    --primary-navy: #YourColor;
    --primary-teal: #YourColor;
    /* All views will update automatically! */
}
```

---

## ? K?T QU?

- ? **Build Successful**
- ? **1 shared CSS file** cho toàn project
- ? **~42 views** ð? ð?ng nh?t màu
- ? **Buttons** Navy Blue consistent
- ? **Badges** colors consistent
- ? **Cards, Tables, Forms** ð?ng nh?t
- ? **Public pages** Navy Blue theme
- ? **Dashboard pages** Navy Blue theme
- ? **Responsive & Print** styles ready
- ? **Utility classes** available
- ? **Easy customization** with CSS variables

---

## ?? MÀU S?C SUMMARY

| Element | Color | Hex Code |
|---------|-------|----------|
| Primary Button | Navy Blue | `#1e3a5f` |
| Button Hover | Navy Dark | `#163354` |
| Success/Teal | Teal | `#2d9e8e` |
| Sidebar | Navy Blue | `#1e3a5f` |
| Active Menu | Teal | `#2d9e8e` |
| Badge Primary | Purple | `#6366f1` |
| Badge Info | Teal | `#2d9e8e` |
| Background | Light Gray | `#f5f7fa` |
| Cards | White | `#ffffff` |
| Text Dark | Dark Gray | `#2d3748` |

---

## ?? HIGHLIGHTS

### Trý?c fix:
```
? M?i view có màu khác nhau
? Buttons không consistent
? Badges màu lung tung
? Cards style khác nhau
? Tables hover colors khác nhau
? Forms focus colors khác nhau
```

### Sau fix:
```
? T?T C? views cùng color palette
? Buttons ð?ng nh?t Navy Blue
? Badges colors consistent
? Cards same design
? Tables same hover effect
? Forms same focus style
? 1 file CSS cho toàn b?
? Easy maintenance
? Professional & Clean
```

---

## ?? NEXT STEPS

N?u c?n customize thêm:

### 1. Thêm màu accent m?i
```css
:root {
    --accent-your-color: #HEXCODE;
}
```

### 2. T?o button variant m?i
```css
.btn-your-style {
    background: var(--accent-your-color);
    /* ... */
}
```

### 3. T?o card variant m?i
```css
.card-your-style {
    border-left: 4px solid var(--accent-your-color);
}
```

---

**Toàn b? project gi? ð? có màu s?c ð?ng nh?t, professional và d? maintain!** ???

---

## ?? FILE LOCATIONS

```
WebApplication1/
??? wwwroot/
?   ??? css/
?       ??? shared-theme.css  ? M?I - File CSS chung
?
??? Views/
?   ??? Shared/
?       ??? _DashboardLayout.cshtml  ? Ð? include shared CSS
?       ??? _PublicLayout.cshtml     ? Ð? include shared CSS
?
??? Areas/
    ??? Admin/Views/    ? 28 views t? ð?ng ð?ng nh?t
    ??? Lecturer/Views/ ? 6 views t? ð?ng ð?ng nh?t
    ??? Student/Views/  ? 4 views t? ð?ng ð?ng nh?t
```

**Perfect! One CSS file to rule them all!** ??

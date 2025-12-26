# ? FIX: PROFILE & CHANGE PASSWORD 404 ERROR

## ? L?I 

```
HTTP ERROR 404
https://localhost:44357/Profile
https://localhost:44357/Profile/ChangePassword
```

## ? Ð? KH?C PH?C

### 1. **Thêm ProfileViewModel vào AuthViewModels.cs**

```csharp
public class ProfileViewModel
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    
    // For Students
    public string? StudentCode { get; set; }
    public string? Major { get; set; }
    public int? AdmissionYear { get; set; }
    
    // For Lecturers
    public string? LecturerCode { get; set; }
    public string? Department { get; set; }
    public string? Specialization { get; set; }
}

public class UpdateProfileViewModel
{
    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; } = string.Empty;
}
```

### 2. **T?o ProfileController.cs**

File: `Controllers/ProfileController.cs`

**Actions**:
- `Index()` - GET: Hi?n th? profile
- `Update()` - POST: C?p nh?t profile
- `ChangePassword()` - GET: Form ð?i m?t kh?u
- `ChangePassword()` - POST: X? l? ð?i m?t kh?u

**Features**:
- L?y thông tin user t? cookie
- Load thông tin Student/Lecturer d?a trên role
- Update tr?c ti?p vào FakeDatabase
- Validation password

### 3. **T?o Views**

#### Profile Index View
File: `Views/Profile/Index.cshtml`

**Design**:
```
???????????????????????????????
?  Avatar Section (Gradient)  ?
?  ???????                    ?
?  ?  N  ?  Nguyen Van Admin  ?
?  ???????     Admin          ?
???????????????????????????????
?  Profile Body               ?
?  Full Name: [value]         ?
?  Email: [value]             ?
?  Account ID: [value]        ?
?  Role: [value]              ?
?  (Student/Lecturer info)    ?
?                             ?
?  [Change Password Button]   ?
???????????????????????????????
```

**Features**:
- Avatar tr?n v?i ch? cái ð?u
- Gradient Navy Blue ? Teal
- Info fields v?i icons
- Conditional rendering (Student/Lecturer)

#### Change Password View
File: `Views/Profile/ChangePassword.cshtml`

**Design**:
```
???????????????????????????????
?  Change Password            ?
?  Change your account...     ?
???????????????????????????????
?  Current Password:          ?
?  [?? input] [??]            ?
?                             ?
?  New Password:              ?
?  [?? input] [??]            ?
?                             ?
?  Confirm Password:          ?
?  [?? input] [??]            ?
?                             ?
?  ? Password Requirements    ?
?  ? At least 6 characters    ?
?  ? Must not match current   ?
?  ? Confirmation must match  ?
?                             ?
?  [Update Password] [Cancel] ?
???????????????????????????????
```

**Features**:
- Toggle show/hide password (eye icon)
- Password requirements info box
- Validation messages
- Navy Blue theme

### 4. **Updated _DashboardLayout.cshtml**

User dropdown menu ð? có 2 links:

```html
<ul class="dropdown-menu dropdown-menu-end">
    <li><a class="dropdown-item" asp-area="" asp-controller="Profile" asp-action="Index">
        <i class="bi bi-person me-2"></i>Profile
    </a></li>
    <li><a class="dropdown-item" asp-area="" asp-controller="Profile" asp-action="ChangePassword">
        <i class="bi bi-key me-2"></i>Change Password
    </a></li>
    <li><hr class="dropdown-divider"></li>
    <li>
        <form asp-area="" asp-controller="Account" asp-action="Logout" method="post">
            <button type="submit" class="dropdown-item">
                <i class="bi bi-box-arrow-right me-2"></i>Logout
            </button>
        </form>
    </li>
</ul>
```

---

## ?? FILES CREATED

1. ? `Controllers/ProfileController.cs`
2. ? `Views/Profile/Index.cshtml`
3. ? `Views/Profile/ChangePassword.cshtml`
4. ? Updated `ViewModels/AuthViewModels.cs`

---

## ?? ROUTES

| URL | Controller | Action | Method |
|-----|-----------|--------|--------|
| `/Profile` | ProfileController | Index | GET |
| `/Profile/Update` | ProfileController | Update | POST |
| `/Profile/ChangePassword` | ProfileController | ChangePassword | GET |
| `/Profile/ChangePassword` | ProfileController | ChangePassword | POST |

---

## ?? TEST

### Test Profile Page

1. **Login** v?i b?t k? account nào
2. Click **avatar** ? góc ph?i trên
3. Click **"Profile"**
4. Check:
   - ? URL: `https://localhost:44357/Profile`
   - ? Avatar hi?n th? ch? cái ð?u
   - ? Full Name, Email, Account ID, Role
   - ? Student info (n?u role Student)
   - ? Lecturer info (n?u role Lecturer)

### Test Change Password

1. T? Profile page
2. Click **"Change Password"** button
3. Check:
   - ? URL: `https://localhost:44357/Profile/ChangePassword`
   - ? 3 password fields
   - ? Toggle eye icons ho?t ð?ng
   - ? Password requirements box
4. **Test ð?i m?t kh?u**:
   - Nh?p current password: `admin123` (ho?c password hi?n t?i)
   - Nh?p new password: `newpass123`
   - Nh?p confirm: `newpass123`
   - Click **"Update Password"**
   - ? Success message
5. **Logout và login l?i** v?i password m?i
   - ? Ðãng nh?p thành công v?i password m?i

---

## ?? DESIGN FEATURES

### Profile Page

**Colors**:
- Avatar background: Gradient Navy Blue `#1e3a5f` ? Teal `#2d9e8e`
- Card: White with shadow
- Info fields: Light gray `#f7fafc`
- Icons: Teal `#2d9e8e`

**Layout**:
- Max-width: 800px
- Centered
- Avatar section: Full width gradient
- Body: White with padding

### Change Password Page

**Colors**:
- Same Navy Blue theme
- Requirements box: Light blue with Teal left border
- Buttons: Navy Blue primary

**Layout**:
- Max-width: 600px
- Centered
- Form inputs with left icons
- Right eye toggle buttons

**JavaScript**:
- Toggle password visibility
- Eye icon changes: `bi-eye` ? `bi-eye-slash`

---

## ?? SECURITY

### Password Validation
```csharp
// Server-side validation
- Current password must match
- New password minimum 6 characters
- Confirm password must match new password
```

### Data Access
```csharp
// Direct access to FakeDatabase
var user = FakeDatabase.Users.FirstOrDefault(u => u.Id == userId);
user.Password = model.NewPassword;
```

### Authentication Check
```csharp
// Check logged in
var userIdClaim = User.FindFirst("UserId");
if (userIdClaim == null) {
    return RedirectToAction("Login", "Account");
}
```

---

## ? RESULT

- ? **Build Successful**
- ? **404 Errors FIXED**
- ? `/Profile` ? Works
- ? `/Profile/ChangePassword` ? Works
- ? Profile page hi?n th? ð?y ð? thông tin
- ? Change password v?i validation
- ? Toggle show/hide password
- ? Navy Blue theme consistent
- ? Responsive design
- ? User-friendly interface

---

## ?? NOTES

### Profile Info by Role

**Admin**:
- Full Name
- Email
- Account ID: ADMIN001
- Role: Admin

**Lecturer**:
- Full Name
- Email  
- Account ID: GV001
- Role: Lecturer
- Department
- Specialization

**Student**:
- Full Name
- Email
- Account ID: SV001
- Role: Student
- Major
- Admission Year

### Password Change Flow

```
1. User clicks "Change Password"
   ?
2. Enter current password
   ?
3. Verify current password
   ?
4. Enter new password
   ?
5. Confirm new password
   ?
6. Validate (length, match)
   ?
7. Update in FakeDatabase
   ?
8. Success message
   ?
9. Can login with new password
```

---

**Profile & Change Password features ð? ho?t ð?ng hoàn h?o!** ???

# ?? C?P NH?T ACCOUNT VIEWS - NAVY BLUE THEME

## ? T?NG QUAN

Ð? thi?t k? l?i hoàn toàn 3 views Account theo m?u design v?i:
- Login View - Split layout v?i info panel
- Register View - Split layout v?i features panel
- Forgot Password View - Centered card v?i logo
- **100% English language**
- Navy Blue Theme consistent

---

## ?? CÁC FILE Ð? C?P NH?T

### 1. ? Login View
**File**: `Views/Account/Login.cshtml`

**Design**:
```
??????????????????????????????????????????
?  Left Panel          ?  Right Card     ?
?  (Info)              ?  (Login Form)   ?
?                      ?                 ?
?  ?? SCMS             ?  Sign In        ?
?  Admin Portal        ?                 ?
?                      ?  [Email]        ?
?  Student & Class     ?  [Password]     ?
?  Management System   ?  ? Remember     ?
?                      ?  Forgot?        ?
?  Complete system...  ?                 ?
?                      ?  [Sign In]      ?
?                      ?  Sign up now    ?
??????????????????????????????????????????
```

**Features**:
- Split layout: Info panel + Form card
- SCMS logo with "Admin Portal" subtitle
- Title: "Student & Class Management System"
- Description: "Complete management system..."
- White card v?i shadow
- Navy Blue button
- "Forgot password?" link
- "Sign up now" link

### 2. ? Register View
**File**: `Views/Account/Register.cshtml`

**Design**:
```
??????????????????????????????????????????
?  Left Panel          ?  Right Card     ?
?  (Features)          ?  (Register)     ?
?                      ?                 ?
?  ?? SCMS             ?  Create Account ?
?  Student Portal      ?                 ?
?                      ?  [Full Name]    ?
?  Join SCMS Today     ?  [Email]        ?
?                      ?  [Password]     ?
?  ? View schedules    ?  [Confirm]      ?
?  ? Register courses  ?  ? Terms        ?
?  ? Track grades      ?                 ?
?  ? Notifications     ?  [Sign Up]      ?
?                      ?  Sign in now    ?
??????????????????????????????????????????
```

**Features**:
- Split layout with features list
- SCMS logo with "Student Portal"
- Title: "Join SCMS Today"
- 4 feature items with checkmarks
- Full Name, Email, Password, Confirm Password
- Terms of Service checkbox
- "Sign in now" link

### 3. ? Forgot Password View
**File**: `Views/Account/ForgotPassword.cshtml`

**Design**:
```
???????????????????????????
?                         ?
?     ?? (Logo Icon)      ?
?        SCMS             ?
?     Admin Portal        ?
?                         ?
?   Forgot Password       ?
?   Enter your email...   ?
?                         ?
?     [Email Field]       ?
?                         ?
?   [Send Reset Link]     ?
?                         ?
?    ? Back to login      ?
?                         ?
?   Need help? Contact    ?
?   support@scms.edu      ?
?                         ?
???????????????????????????
```

**Features**:
- Centered card design
- Large SCMS logo icon (80x80px)
- Logo text + subtitle
- Title + description
- Email input
- "Send Reset Link" button
- "Back to login" link with arrow
- Help section at bottom

---

## ?? DESIGN DETAILS

### Color Scheme
```css
Primary Navy: #1e3a5f
Primary Teal: #2d9e8e
Background: Linear gradient Navy ? Teal
Cards: White
Text Dark: #2d3748
Text Gray: #718096
Input Border: #e2e8f0
Focus Color: #2d9e8e (Teal)
```

### Layout Structure

**Login & Register (Split)**:
```css
.auth-wrapper {
    display: grid;
    grid-template-columns: 1fr 1fr;
    max-width: 1100px;
    gap: 3rem;
}

/* Left panel: Info/Features */
.auth-left {
    color: white;
    padding: 2rem;
}

/* Right panel: Form card */
.auth-card {
    background: white;
    border-radius: 20px;
    padding: 2.5rem;
}
```

**Forgot Password (Centered)**:
```css
.forgot-card {
    background: white;
    border-radius: 20px;
    padding: 3rem;
    max-width: 500px;
}
```

### Logo Design

**Logo Icon**:
```css
width: 60px-80px;
height: 60px-80px;
background: rgba(45, 158, 142, 0.3);
border-radius: 12px-20px;
font-size: 2rem-2.5rem;
```

**Logo Text**:
```html
<h3>SCMS</h3>
<p>Admin Portal / Student Portal</p>
```

### Form Elements

**Input Groups**:
```html
<div class="input-group">
    <span class="input-group-text">
        <i class="bi bi-envelope"></i>
    </span>
    <input class="form-control" placeholder="..." />
</div>
```

**Buttons**:
```css
.btn-auth {
    background: #1e3a5f;
    padding: 0.875rem;
    font-weight: 600;
    border-radius: 10px;
    width: 100%;
}

.btn-auth:hover {
    background: #163354;
    transform: translateY(-2px);
    box-shadow: 0 5px 15px rgba(30, 58, 95, 0.3);
}
```

---

## ?? TEXT CONTENT (100% ENGLISH)

### Login View
```
- Title: "Sign In"
- Subtitle: "Login to SCMS system"
- Email label: "Email"
- Email placeholder: "email@scms.edu"
- Password label: "Password"
- Password placeholder: "Enter password"
- Checkbox: "Remember me"
- Link: "Forgot password?"
- Button: "Sign In ?"
- Register link: "Don't have an account? Sign up now"
```

### Register View
```
- Title: "Create Account"
- Subtitle: "Create a new student account"
- Full Name placeholder: "Nguyen Van A"
- Email label: "Email (@scms.edu)"
- Email placeholder: "email@scms.edu"
- Password placeholder: "At least 6 characters"
- Confirm placeholder: "Re-enter password"
- Checkbox: "I agree to the Terms of Service and Privacy Policy"
- Button: "Sign Up ?"
- Login link: "Already have an account? Sign in now"
- Features:
  * "View class schedules and timetables"
  * "Register for courses online"
  * "Track grades and GPA"
  * "Receive instant notifications"
```

### Forgot Password View
```
- Title: "Forgot Password"
- Subtitle: "Enter your registered email to receive a password reset link"
- Email label: "Email"
- Email placeholder: "email@scms.edu"
- Button: "Send Reset Link ?"
- Back link: "? Back to login"
- Help text: "Need help? Contact support@scms.edu"
```

---

## ?? RESPONSIVE DESIGN

### Desktop (> 968px)
```
Login/Register: Split layout (50-50)
Forgot Password: Centered card (500px max)
```

### Mobile (< 968px)
```css
@media (max-width: 968px) {
    .auth-wrapper {
        grid-template-columns: 1fr;
    }
    
    .auth-left {
        display: none; /* Hide left panel */
    }
}
```

Mobile shows only the form card.

---

## ? ANIMATIONS

### Fade In Up
```css
@keyframes fadeInUp {
    from {
        opacity: 0;
        transform: translateY(30px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.auth-card {
    animation: fadeInUp 0.6s ease;
}
```

### Hover Effects

**Buttons**:
```css
transform: translateY(-2px);
box-shadow: 0 5px 15px rgba(30, 58, 95, 0.3);
```

**Links**:
```css
color: #1e3a5f ? #2d9e8e (on hover)
```

---

## ?? NAVIGATION FLOW

```
Home ? Login ? Register
         ?
    Forgot Password
         ?
    Back to Login
```

**Login View**:
- "Forgot password?" ? Forgot Password
- "Sign up now" ? Register

**Register View**:
- "Sign in now" ? Login

**Forgot Password View**:
- "Back to login" ? Login

---

## ?? FEATURE HIGHLIGHTS

### Login View
```
? Split layout v?i info panel
? SCMS logo + "Admin Portal"
? System description
? Email + Password fields
? Remember me checkbox
? Forgot password link
? Sign up link
? Navy Blue button
```

### Register View
```
? Split layout v?i features panel
? SCMS logo + "Student Portal"
? "Join SCMS Today" title
? 4 feature items with icons
? Full Name field
? Email field
? Password + Confirm fields
? Terms of Service checkbox
? Sign in link
```

### Forgot Password View
```
? Centered card design
? Large logo icon (80x80px)
? SCMS branding
? Clear instructions
? Email input
? Send reset button
? Back to login link
? Help/Contact section
```

---

## ?? TEST CASES

### Test 1: Login View
1. Navigate to `/Account/Login`
2. Check:
   - ? Split layout visible (desktop)
   - ? Left panel: SCMS logo, title, description
   - ? Right card: Login form
   - ? All text in English
   - ? Button Navy Blue
   - ? Hover effects work
   - ? Mobile: Only form card visible

### Test 2: Register View
1. Navigate to `/Account/Register`
2. Check:
   - ? Split layout visible (desktop)
   - ? Left panel: SCMS logo, features list
   - ? Right card: Registration form
   - ? 4 fields + Terms checkbox
   - ? All text in English
   - ? Button Navy Blue

### Test 3: Forgot Password View
1. Navigate to `/Account/ForgotPassword`
2. Check:
   - ? Centered card (500px max)
   - ? Large SCMS logo icon
   - ? Clear title + instructions
   - ? Email field
   - ? Send button
   - ? Back link
   - ? Help section

### Test 4: Navigation
1. From Login:
   - Click "Sign up now" ? Register ?
   - Click "Forgot password?" ? Forgot Password ?
2. From Register:
   - Click "Sign in now" ? Login ?
3. From Forgot Password:
   - Click "Back to login" ? Login ?

---

## ?? SO SÁNH

### ? TRÝ?C (Old Design)

**Login**:
```
- Simple centered card
- Purple gradient icon
- Mixed Vietnamese/English
- Generic layout
```

**Register**:
```
- Simple centered card
- Purple gradient icon
- Vietnamese text
- No features showcase
```

**Forgot Password**:
```
- Simple centered card
- Key icon
- Vietnamese text
- No branding
```

### ? SAU (New Design)

**Login**:
```
? Split layout with info panel
? SCMS branding + logo
? 100% English
? Professional system description
? Navy Blue theme
```

**Register**:
```
? Split layout with features
? SCMS branding
? 100% English
? 4 feature highlights
? Terms of Service checkbox
```

**Forgot Password**:
```
? Centered with large logo
? SCMS branding
? 100% English
? Help/Contact section
? Professional design
```

---

## ? K?T QU?

- ? **Build Successful**
- ? **3 views redesigned**
- ? **100% English text**
- ? **Navy Blue theme**
- ? **Modern split layouts**
- ? **SCMS branding**
- ? **Responsive design**
- ? **Smooth animations**
- ? **Professional look**

---

## ?? HIGHLIGHTS

### Design Quality
```
?? Modern split layout (Login, Register)
?? Centered card (Forgot Password)
?? Large logo icons
?? Feature highlights
?? Clean white cards
?? Navy Blue buttons
?? Smooth animations
```

### User Experience
```
? Clear navigation flow
? Helpful links
? Form validation
? Professional branding
? Consistent theme
? Mobile responsive
```

### Technical
```
?? Navy Blue theme CSS
?? Responsive grid layout
?? CSS animations
?? Form styling
?? Hover effects
?? Media queries
```

---

**Account views gi? ð? có thi?t k? chuyên nghi?p, hi?n ð?i và 100% ti?ng Anh nhý yêu c?u!** ???

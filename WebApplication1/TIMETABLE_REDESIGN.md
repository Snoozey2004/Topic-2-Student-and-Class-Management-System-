# ?? THI?T K? L?I TIMETABLE - Ð?C & Ð?P

## ? T?NG QUAN

Ð? thi?t k? l?i hoàn toàn b?ng th?i khóa bi?u (timetable) cho c? 3 role v?i layout m?i:
- **Hàng ngang**: Các th? trong tu?n (Monday ? Friday)
- **C?t d?c**: Các ti?t h?c (Period 1-3, Period 4-6, Period 7-9)
- Màu s?c theo Navy Blue Theme m?i
- Responsive và interactive

---

## ?? LAYOUT M?I

### C?u trúc Grid

```
???????????????????????????????????????????????????????????????????
?   Period    ? Monday  ? Tuesday ? Wednesday ? Thursday ? Friday ?
???????????????????????????????????????????????????????????????????
? Period 1-3  ? [Class] ? [Empty] ?  [Class]  ? [Class]  ?[Empty] ?
? 07:00-09:30 ?         ?         ?           ?          ?        ?
???????????????????????????????????????????????????????????????????
? Period 4-6  ? [Empty] ? [Class] ?  [Empty]  ? [Empty]  ?[Class] ?
? 09:45-12:15 ?         ?         ?           ?          ?        ?
???????????????????????????????????????????????????????????????????
? Period 7-9  ? [Class] ? [Class] ?  [Class]  ? [Empty]  ?[Empty] ?
? 13:00-15:30 ?         ?         ?           ?          ?        ?
???????????????????????????????????????????????????????????????????
```

---

## ?? CÁC FILE Ð? C?P NH?T

### 1. ? Student Schedule
**File**: `Areas/Student/Views/Schedule/Index.cshtml`

**Tính nãng**:
- Timetable grid v?i days ngang, periods d?c
- Class cards v?i border màu s?c theo ch? ð?
- Hi?n th?: Class Code, Subject Name, Lecturer, Room, Time
- Hover effect: lift up + shadow
- Legend màu phân bi?t bu?i h?c
- Responsive mobile-friendly

**Màu s?c**:
- Header: Gradient Navy Blue ? Teal
- Day headers: Navy Blue (#1e3a5f)
- Class cards: 5 màu rotation (Teal, Blue, Purple, Orange, Red)
- Empty cells: Light gray (#fafbfc)

### 2. ? Lecturer Schedule
**File**: `Areas/Lecturer/Views/Schedule/Index.cshtml`

**Tính nãng**:
- Layout gi?ng Student nhýng không hi?n th? Lecturer name
- Thêm **Summary Stats** phía trên:
  - Total Classes
  - Teaching Days
- Stats cards v?i gradient background
- Same grid layout và color scheme

### 3. ? Admin Schedule
**File**: `Areas/Admin/Views/Schedule/Index.cshtml`

**Tính nãng**:
- **Card Grid Layout** thay v? table
- Filter tabs theo ngày (All, Monday, Tuesday, ...)
- Schedule cards v?i:
  - Border màu theo ngày
  - Class code prominent
  - Day badge
  - Period, Room info v?i icons
  - Edit/Delete buttons
- Empty state v?i icon l?n
- JavaScript filter animation

---

## ?? DESIGN DETAILS

### Header Bar
```css
Background: Linear gradient Navy Blue ? Teal
Color: White
Padding: 1.5rem
Border radius: Top only (12px 12px 0 0)
```

### Day Headers (Horizontal)
```css
Background: #1e3a5f (Navy Blue)
Color: White
Font weight: 600
Text align: Center
Min height: 60px
```

### Period Labels (Vertical)
```css
Background: #f7fafc (Light gray)
Color: #4a5568
Font weight: 600
Display: Period name + Time range
Example: 
  Period 1-3
  07:00 - 09:30
```

### Class Cards
```css
Border-left: 4px solid [Color]
Background: Gradient with opacity
Padding: 0.75rem
Border radius: 8px
Transition: transform, shadow
```

**Hover Effect**:
```css
transform: translateY(-2px)
box-shadow: 0 4px 12px rgba(45, 158, 142, 0.2)
```

### Color Scheme cho Classes
| Color | Hex | Usage |
|-------|-----|-------|
| Teal | `#2d9e8e` | Class 1 |
| Blue | `#3b82f6` | Class 2 |
| Purple | `#8b5cf6` | Class 3 |
| Orange | `#f59e0b` | Class 4 |
| Red | `#ef4444` | Class 5 |

Colors rotate theo th? t? class trong data.

---

## ?? RESPONSIVE DESIGN

### Desktop (> 768px)
- Grid: 6 columns (1 for periods + 5 for days)
- Full card padding
- All info visible

### Mobile (< 768px)
```css
Grid columns: Narrower
Period column: 100px (t? 120px)
Font sizes: Smaller
Card padding: Reduced
Min-width: 800px with horizontal scroll
```

---

## ?? TÍNH NÃNG N?I B?T

### 1. **Grid Layout v?i CSS Grid**
```css
display: grid;
grid-template-columns: 120px repeat(5, 1fr);
gap: 1px;
background: #e2e8f0; /* Gap color */
```

### 2. **Color-coded Classes**
M?i class t? ð?ng ðý?c gán 1 trong 5 màu:
```csharp
var color = (colorIndex % 5) + 1;
<div class="class-card color-@color">
```

### 3. **Empty Cell Detection**
```csharp
<div class="schedule-cell @(!hasSchedule ? "empty" : "")">
```

Empty cells có background khác ð? d? nh?n bi?t.

### 4. **Legend**
```html
<div class="legend">
    <div class="legend-item">
        <div class="legend-color" style="background: #2d9e8e;"></div>
        <span>Morning Classes</span>
    </div>
    ...
</div>
```

### 5. **Hover Interactions**
- Class cards: Lift + shadow
- Empty cells: No interaction
- Smooth transitions (0.3s ease)

---

## ?? ADMIN SCHEDULE - CARD VIEW

Admin có layout khác bi?t:

### Card Grid Layout
```css
display: grid;
grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
gap: 1.5rem;
```

### Schedule Card Structure
```
???????????????????????????????
? IT101-01        [Monday]   ? ? Header
???????????????????????????????
? ?? Period 1-3              ?
?    07:00 - 09:30            ?
?                             ?
? ?? Room A101               ?
?                             ?
? [Edit] [Delete]             ? ? Actions
???????????????????????????????
```

### Filter Tabs
```html
<div class="filter-tabs">
    <button class="filter-tab active">All Days</button>
    <button class="filter-tab">Monday</button>
    <button class="filter-tab">Tuesday</button>
    ...
</div>
```

**JavaScript Filter**:
```javascript
function filterByDay(day) {
    cards.forEach(card => {
        card.style.display = 
            (day === 'all' || card.dataset.day === day) 
            ? 'block' : 'none';
    });
}
```

---

## ?? SO SÁNH TRÝ?C VÀ SAU

### ? TRÝ?C (Table c?)

**Student/Lecturer**:
```
- Th? n?m d?c (rows)
- Periods n?m ngang (columns)
- Table cells v?i background colors
- Khó ð?c trên mobile
- Không có hover effects
```

**Admin**:
```
- Simple table 4 columns
- No visual hierarchy
- No filtering
- Plain text, no icons
```

### ? SAU (Grid m?i)

**Student/Lecturer**:
```
- Th? n?m NGANG (nhý l?ch th?t) ?
- Periods n?m D?C (d? ð?c) ?
- Modern card-based design
- Color-coded classes
- Smooth animations
- Icon-based info
- Responsive
```

**Admin**:
```
- Card grid layout
- Filter by day
- Visual color coding
- Icon-based details
- Edit/Delete actions
- Empty state design
- Fade animations
```

---

## ?? COLOR PALETTE

### Timetable Colors
```css
/* Headers */
Navy Blue: #1e3a5f
Teal Gradient: #2d9e8e

/* Class Cards */
Teal:   #2d9e8e  (Morning classes)
Blue:   #3b82f6  (Core subjects)
Purple: #8b5cf6  (Afternoon classes)
Orange: #f59e0b  (Special classes)
Red:    #ef4444  (Evening classes)

/* Backgrounds */
Empty cells: #fafbfc
Table BG: white
Grid gaps: #e2e8f0

/* Text */
Headings: #2d3748
Body: #4a5568
Meta: #718096
```

---

## ?? TEST CASES

### Test 1: Student View
1. Login student: `phamvand@student.edu.vn` / `student123`
2. Navigate to "My Schedule"
3. Check:
   - ? Days horizontal (Monday ? Friday)
   - ? Periods vertical (Period 1-3, 4-6, 7-9)
   - ? Class cards colorful
   - ? Hover effect works
   - ? Legend visible

### Test 2: Lecturer View
1. Login lecturer: `nguyenvana@university.edu.vn` / `lecturer123`
2. Navigate to "Teaching Schedule"
3. Check:
   - ? Summary stats show correct numbers
   - ? Same grid layout as student
   - ? Classes assigned to lecturer only
   - ? No lecturer name displayed (obvious)

### Test 3: Admin View
1. Login admin: `admin@university.edu.vn` / `admin123`
2. Navigate to "Schedule Management"
3. Check:
   - ? Card grid layout
   - ? Filter tabs work
   - ? Click "Monday" ? Only Monday cards show
   - ? Fade animation smooth
   - ? Edit/Delete buttons present

### Test 4: Responsive
1. Open any schedule view
2. Resize browser to mobile width
3. Check:
   - ? Horizontal scroll appears
   - ? Cards still readable
   - ? All info visible

---

## ?? CUSTOMIZATION TIPS

### Thay ð?i màu s?c classes:
```css
/* In <style> section */
.class-card.color-1 { border-left-color: #YourColor; }
.class-card.color-1 { 
    background: linear-gradient(135deg, 
        rgba(YourR, YourG, YourB, 0.05) 0%, 
        rgba(YourR, YourG, YourB, 0.02) 100%); 
}
```

### Thay ð?i s? c?t (thêm Saturday):
```csharp
var days = new[] { 
    "Monday", "Tuesday", "Wednesday", 
    "Thursday", "Friday", "Saturday"  // ? Add
};
```

```css
grid-template-columns: 120px repeat(6, 1fr); /* 5 ? 6 */
```

### Thêm periods m?i:
```csharp
var periods = new[] {
    new { Name = "Period 1-3", Time = "07:00 - 09:30" },
    new { Name = "Period 4-6", Time = "09:45 - 12:15" },
    new { Name = "Period 7-9", Time = "13:00 - 15:30" },
    new { Name = "Period 10-12", Time = "15:45 - 18:15" } // ? Add
};
```

---

## ? K?T QU?

- ? **Build Successful**
- ? **Th? n?m NGANG - Periods n?m D?C** (nhý yêu c?u)
- ? **Modern card-based design**
- ? **Navy Blue theme consistent**
- ? **Responsive mobile-friendly**
- ? **Smooth animations**
- ? **Color-coded classes**
- ? **Interactive hover effects**

---

## ?? HIGHLIGHTS

### Student Schedule
```
?? Weekly Timetable
?? Clear view of all classes
?? Color-coded by subject
?? Mobile responsive
```

### Lecturer Schedule
```
?? Summary statistics
?? Teaching load visualization
?? Same beautiful layout
?? Quick access to class details
```

### Admin Schedule
```
?? Card-based management
?? Filter by day
?? Edit/Delete actions
?? Empty state design
```

---

## ?? FUTURE ENHANCEMENTS

Có th? thêm trong týõng lai:

1. **Print View**
   - CSS print stylesheet
   - Optimized for A4 paper

2. **Export to PDF/Image**
   - html2canvas library
   - Download timetable

3. **Drag & Drop** (Admin)
   - Move classes between slots
   - Visual scheduling

4. **Conflict Detection**
   - Highlight overlapping classes
   - Warning alerts

5. **Dark Mode**
   - Toggle theme
   - Different color palette

---

**Timetable gi? ð? ð?p và professional nhý các h? th?ng qu?n l? hi?n ð?i!** ???

---

## ?? VISUAL PREVIEW (Text)

### Grid Structure
```
        Mon     Tue     Wed     Thu     Fri
Period1 [??]   [  ]    [??]   [??]   [  ]
Period2 [  ]    [??]   [  ]    [  ]    [??]
Period3 [??]   [??]   [??]   [  ]    [  ]

Legend: ?? Teal  ?? Blue  ?? Purple  ?? Orange  ?? Red
```

**Perfect! Ðúng nhý b?n yêu c?u!** ?

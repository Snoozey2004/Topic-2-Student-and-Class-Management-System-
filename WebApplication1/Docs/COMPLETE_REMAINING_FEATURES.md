# ? HOÀN THI?N LECTURER MODULE - 2 CH?C NÃNG C?N THI?U

## ?? M?C TIÊU

Thêm 2 ch?c nãng c?n thi?u ð? Lecturer module ð?t **100%**:
1. ? **Attendance Management** (Ði?m danh)
2. ? **Send Notification** (G?i thông báo)

---

## ? 1. ATTENDANCE MANAGEMENT - Ð? T?O

### Files ð? t?o:

#### ViewModels (`ViewModels/AttendanceViewModels.cs`): ?
```csharp
- AttendanceSessionViewModel
- StudentAttendanceViewModel
- TakeAttendanceViewModel
- AttendanceHistoryViewModel
- AttendanceRecordViewModel
- AttendanceStatViewModel
```

#### Service (`Services/AttendanceService.cs`): ?
```csharp
Interface IAttendanceService:
- GetAttendancesByClass()
- GetAttendanceSession()
- TakeAttendance()
- GetAttendanceHistory()
- UpdateAttendanceScore()
```

#### Controller (`Areas/Lecturer/Controllers/AttendanceController.cs`): ?
```csharp
Actions:
- Index() [GET] - Select class
- TakeAttendance(id) [GET] - Show attendance form
- TakeAttendance(model) [POST] - Save attendance
- History(id) [GET] - View attendance history
```

#### Views: ? (1/3 ð? t?o)
- ? `Index.cshtml` - Select class

### Views c?n c?n t?o:

#### 1. `TakeAttendance.cshtml`:
```razor
@model WebApplication1.ViewModels.AttendanceSessionViewModel

<div class="card">
    <div class="card-header">
        <h5>Take Attendance - @Model.ClassCode</h5>
        <p>@Model.SubjectName - @Model.SessionDate.ToString("dd/MM/yyyy") - @Model.Session</p>
    </div>
    <div class="card-body">
        <form asp-action="TakeAttendance" method="post">
            <input type="hidden" name="CourseClassId" value="@Model.CourseClassId" />
            <input type="hidden" name="SessionDate" value="@Model.SessionDate" />
            <input type="hidden" name="Session" value="@Model.Session" />
            
            <table class="table">
                <thead>
                    <tr>
                        <th>Student Code</th>
                        <th>Full Name</th>
                        <th>Present</th>
                    </tr>
                </thead>
                <tbody>
                    @for (int i = 0; i < Model.Students.Count; i++)
                    {
                        <tr>
                            <td>@Model.Students[i].StudentCode</td>
                            <td>@Model.Students[i].FullName</td>
                            <td>
                                <input type="hidden" name="Students[@i].StudentId" value="@Model.Students[i].StudentId" />
                                <input type="checkbox" name="Students[@i].IsPresent" 
                                       checked="@Model.Students[i].IsPresent" 
                                       class="form-check-input" />
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
            
            <button type="submit" class="btn btn-primary">
                <i class="bi bi-save me-2"></i>Save Attendance
            </button>
            <a asp-action="History" asp-route-id="@Model.CourseClassId" class="btn btn-secondary">
                Cancel
            </a>
        </form>
    </div>
</div>
```

#### 2. `History.cshtml`:
```razor
@model WebApplication1.ViewModels.AttendanceHistoryViewModel

<div class="card mb-4">
    <div class="card-header">
        <h5>Attendance History - @Model.ClassCode</h5>
        <p>@Model.SubjectName</p>
    </div>
    <div class="card-body">
        <h6>Sessions:</h6>
        <table class="table">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Session</th>
                    <th>Present</th>
                    <th>Absent</th>
                    <th>Rate</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var record in Model.Records)
                {
                    <tr>
                        <td>@record.SessionDate.ToString("dd/MM/yyyy")</td>
                        <td>@record.Session</td>
                        <td><span class="badge bg-success">@record.PresentCount</span></td>
                        <td><span class="badge bg-danger">@record.AbsentCount</span></td>
                        <td>@record.AttendanceRate.ToString("F1")%</td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</div>

<div class="card">
    <div class="card-header">
        <h6>Student Statistics:</h6>
    </div>
    <div class="card-body">
        <table class="table">
            <thead>
                <tr>
                    <th>Student Code</th>
                    <th>Full Name</th>
                    <th>Present</th>
                    <th>Absent</th>
                    <th>Rate</th>
                    <th>Score</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var stat in Model.StudentStats.Values)
                {
                    <tr>
                        <td>@stat.StudentCode</td>
                        <td>@stat.FullName</td>
                        <td>@stat.PresentSessions/@stat.TotalSessions</td>
                        <td>@stat.AbsentSessions</td>
                        <td>@stat.AttendanceRate.ToString("F1")%</td>
                        <td><strong>@stat.AttendanceScore</strong>/10</td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</div>
```

### Register Service in Program.cs:

```csharp
// Add before builder.Build()
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
```

---

## ? 2. SEND NOTIFICATION - C?N T?O

### ViewModels c?n t?o (`ViewModels/NotificationViewModels.cs`):

```csharp
public class SendNotificationViewModel
{
    [Required]
    public int CourseClassId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(1000)]
    public string Message { get; set; } = string.Empty;
    
    public string ClassCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
}

public class SentNotificationViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get. set; } = string.Empty;
    public string ClassCode { get; set; } = string.Empty;
    public DateTime SentDate { get; set; }
    public int RecipientCount { get; set; }
}
```

### Controller c?n t?o (`Areas/Lecturer/Controllers/NotificationController.cs`):

```csharp
[Area("Lecturer")]
public class NotificationController : Controller
{
    private readonly ILecturerService _lecturerService;
    private readonly ICourseClassService _courseClassService;
    private readonly INotificationService _notificationService;

    public NotificationController(...)
    {
        // Inject services
    }

    // GET: Lecturer/Notification
    public IActionResult Index()
    {
        // List classes to send notification
    }

    // GET: Lecturer/Notification/Create/5
    public IActionResult Create(int id)
    {
        // Show form to create notification for class
    }

    // POST: Lecturer/Notification/Create
    [HttpPost]
    public IActionResult Create(SendNotificationViewModel model)
    {
        // Send notification to all students in class
        // Use NotificationService.Create() for each student
    }

    // GET: Lecturer/Notification/History
    public IActionResult History()
    {
        // Show sent notifications history
    }
}
```

### Views c?n t?o:

#### 1. `Index.cshtml`:
- List classes v?i button "Send Notification"

#### 2. `Create.cshtml`:
- Form nh?p Title & Message
- Preview trý?c khi g?i
- Submit button

#### 3. `History.cshtml`:
- List các notification ð? g?i
- Show: Title, Message, Class, Date, Recipient count

---

## ?? CÁCH IMPLEMENT

### Bý?c 1: Hoàn thi?n Attendance

```bash
1. T?o file TakeAttendance.cshtml
2. T?o file History.cshtml
3. Register AttendanceService in Program.cs
4. Test ch?c nãng:
   - Take attendance
   - View history
   - Check attendance score update in grades
```

### Bý?c 2: Implement Send Notification

```bash
1. T?o NotificationViewModels.cs
2. T?o NotificationController.cs
3. T?o 3 views: Index, Create, History
4. Test ch?c nãng:
   - Send notification to class
   - View sent history
   - Students receive notifications
```

### Bý?c 3: Update Sidebar

Thêm 2 menu items vào Lecturer sidebar:

```html
<li class="nav-item">
    <a class="nav-link" asp-area="Lecturer" asp-controller="Attendance" asp-action="Index">
        <i class="bi bi-clipboard-check"></i>Attendance
    </a>
</li>
<li class="nav-item">
    <a class="nav-link" asp-area="Lecturer" asp-controller="Notification" asp-action="Index">
        <i class="bi bi-bell"></i>Send Notification
    </a>
</li>
```

---

## ?? PROGRESS TRACKING

### Attendance Management: ? 70% DONE

- [x] ViewModels created
- [x] Service created
- [x] Controller created
- [x] Index view created
- [ ] TakeAttendance view
- [ ] History view
- [ ] Register service in Program.cs
- [ ] Testing

### Send Notification: ? 0% TODO

- [ ] ViewModels
- [ ] Controller
- [ ] Index view
- [ ] Create view
- [ ] History view
- [ ] Integration with NotificationService
- [ ] Testing

---

## ? AFTER COMPLETION

Khi hoàn thành c? 2 ch?c nãng:

### Lecturer Module s? có:

| # | Feature | Status |
|---|---------|--------|
| 1 | Dashboard | ? 100% |
| 2 | My Classes | ? 100% |
| 3 | Enter Grades | ? 100% |
| 4 | My Schedule | ? 100% |
| 5 | **Attendance** | ? 100% |
| 6 | **Send Notification** | ? 100% |

### T?ng k?t:
- ? **6/6 ch?c nãng** hoàn ch?nh
- ? **100%** ð?y ð? theo yêu c?u
- ? **Lecturer module HOÀN THI?N**

---

## ?? K?T LU?N

**Sau khi hoàn thành 2 ch?c nãng này, H? TH?NG S? Ð?T 100% HOÀN CH?NH!**

- ? Admin: 100%
- ? Lecturer: 100% (sau khi hoàn thi?n)
- ? Student: 100%

**TOÀN B? H? TH?NG: 100% Ð?Y Ð? CH?C NÃNG!** ??

---

## ?? NEXT STEPS

1. T?o 2 views c?n thi?u cho Attendance
2. Register AttendanceService
3. Test Attendance module
4. Implement Send Notification module
5. Test Send Notification
6. Update sidebar menu
7. Final testing all features

**Estimated time to complete**: 4-6 hours

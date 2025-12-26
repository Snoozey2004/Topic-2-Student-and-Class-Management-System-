# ?? From Hardcode to ViewBag - Migration Guide

## T?ng quan

Document này h??ng d?n cách chuy?n t? **hardcode data trong View** sang **dynamic data qua ViewBag** mà **KHÔNG thay ??i Model/Database**.

---

## ?? M?c ?ích

- ? Lo?i b? hardcode trong Views
- ? Centralize data logic vào Service layer
- ? D? dàng chuy?n sang Database th?t sau này
- ? Gi? nguyên Model/Entity classes

---

## ??? Ki?n trúc Before & After

### BEFORE (Hardcode)

```
???????????????????????????????????????????????????????
? View.cshtml                                         ?
? ???????????????????????????????????????????????   ?
? ? <select>                                     ?   ?
? ?   <option value="HK1-2024">HK1-2024</option> ? ??
? ?   <option value="HK2-2024">HK2-2024</option> ?   ?
? ? </select>                                    ?   ?
? ???????????????????????????????????????????????   ?
???????????????????????????????????????????????????????
   ?
   ??? Data hardcode ngay trong View
       ? Khó maintain
       ? Khó test
       ? Duplicate code n?u nhi?u view c?n data gi?ng nhau
```

### AFTER (ViewBag + Service)

```
????????????????    ????????????????    ????????????????    ????????????????
? FakeDatabase ??????   Service    ??????  Controller  ??????    View      ?
?              ?    ?              ?    ?              ?    ?              ?
? CourseClass  ?    ? Semester     ?    ? ViewBag.     ?    ? @ViewBag.    ?
? .Semester    ?    ? Service      ?    ? Semesters    ?    ? Semesters    ?
????????????????    ????????????????    ????????????????    ????????????????
                          ?
                          ? Query database
                          ? Business logic
                          ? Data transformation
                          ?
                    ? Single source of truth
                    ? Testable
                    ? Reusable
```

---

## ?? Implementation Steps

### Step 1: T?o Service (Data Provider)

```csharp
// WebApplication1/Services/SemesterService.cs

public interface ISemesterService
{
    string GetCurrentSemester();
    List<string> GetAllSemesters();
}

public class SemesterService : ISemesterService
{
    public string GetCurrentSemester()
    {
        // L?y t? FakeDatabase (ho?c DB th?t sau này)
        return FakeDatabase.CourseClasses
            .Where(c => c.Status == CourseClassStatus.Open || 
                       c.Status == CourseClassStatus.InProgress)
            .OrderByDescending(c => c.Semester)
            .FirstOrDefault()?.Semester ?? "HK1-2024";
    }

    public List<string> GetAllSemesters()
    {
        return FakeDatabase.CourseClasses
            .Select(c => c.Semester)
            .Distinct()
            .OrderByDescending(s => s)
            .ToList();
    }
}
```

**? KHÔNG c?n thay ??i Model! Ch? query existing data.**

### Step 2: Register Service trong Program.cs

```csharp
// WebApplication1/Program.cs

builder.Services.AddScoped<ISemesterService, SemesterService>();
```

### Step 3: Inject Service vào Controller

```csharp
// WebApplication1/Areas/Student/Controllers/EnrollmentController.cs

public class EnrollmentController : Controller
{
    private readonly ISemesterService _semesterService; // ? Inject service

    public EnrollmentController(
        IStudentService studentService, 
        IEnrollmentService enrollmentService,
        ISemesterService semesterService) // ? Add parameter
    {
        _studentService = studentService;
        _enrollmentService = enrollmentService;
        _semesterService = semesterService; // ? Assign
    }

    public IActionResult Index(string? semester = null)
    {
        // Get data from service
        var currentSemester = semester ?? _semesterService.GetCurrentSemester();
        var allSemesters = _semesterService.GetAllSemesters();

        // Pass to View via ViewBag
        ViewBag.Semesters = new SelectList(allSemesters);
        ViewBag.CurrentSemester = currentSemester;

        // ... rest of code
        return View(viewModel);
    }
}
```

**? Controller không hardcode, ch? l?y t? Service và pass cho View.**

### Step 4: Update View ?? dùng ViewBag

**BEFORE:**
```razor
<!-- ? Hardcode -->
<select class="portal-select">
    <option value="">All Semesters</option>
    <option value="HK1-2024" selected>HK1-2024</option>
    <option value="HK2-2024">HK2-2024</option>
</select>
```

**AFTER:**
```razor
@using Microsoft.AspNetCore.Mvc.Rendering
@{
    var semesters = ViewBag.Semesters as SelectList;
    var currentSemester = ViewBag.CurrentSemester as string;
}

<!-- ? Dynamic from ViewBag -->
<form method="get" id="filterForm">
    <select name="semester" class="portal-select" 
            onchange="document.getElementById('filterForm').submit();">
        <option value="">All Semesters</option>
        @if (semesters != null)
        {
            @foreach (var sem in semesters)
            {
                <option value="@sem.Value" 
                        selected="@(sem.Value == currentSemester)">
                    @sem.Text
                </option>
            }
        }
    </select>
</form>
```

**? View ch? render data, không quy?t ??nh data t? ?âu.**

---

## ?? Migration Checklist

### Files ?ã migrate:

- [x] **Services/SemesterService.cs** - Service m?i
- [x] **Program.cs** - Register service
- [x] **Student/EnrollmentController.cs** - Inject & ViewBag
- [x] **Student/EnrollmentView** - Dùng ViewBag
- [x] **Lecturer/AttendanceController.cs** - Inject & ViewBag
- [x] **Lecturer/AttendanceView** - Dùng ViewBag
- [x] **Student/GradeController.cs** - Inject & ViewBag
- [x] **Student/GradeView** - Dùng ViewBag

### Model/Database:
- [ ] **KHÔNG thay ??i** Model classes
- [ ] **KHÔNG thay ??i** Database schema
- [ ] **KHÔNG thay ??i** Entity properties

---

## ?? ViewBag vs ViewData vs TempData

| Feature | ViewBag | ViewData | TempData |
|---------|---------|----------|----------|
| Type | `dynamic` | `Dictionary` | `Dictionary` |
| Syntax | `ViewBag.Name` | `ViewData["Name"]` | `TempData["Name"]` |
| Lifetime | Current request | Current request | Current + Next request |
| Use case | Controller ? View | Controller ? View | Redirect scenarios |

### T?i sao dùng ViewBag?

```csharp
// ViewBag - Clean & simple
ViewBag.Semesters = allSemesters;
@ViewBag.Semesters

// ViewData - C?n cast
ViewData["Semesters"] = allSemesters;
@((List<string>)ViewData["Semesters"])
```

---

## ?? Best Practices

### ? DO:

1. **Service layer x? lý data logic**
   ```csharp
   public List<string> GetAllSemesters()
   {
       return _database.CourseClasses
           .Select(c => c.Semester)
           .Distinct()
           .OrderByDescending(s => s)
           .ToList();
   }
   ```

2. **Controller ch? orchestrate**
   ```csharp
   var data = _service.GetData();
   ViewBag.Data = data;
   ```

3. **View ch? render**
   ```razor
   @foreach (var item in ViewBag.Data)
   {
       <div>@item</div>
   }
   ```

### ? DON'T:

1. **Hardcode trong View**
   ```razor
   <!-- ? BAD -->
   <option value="HK1-2024">HK1-2024</option>
   ```

2. **Business logic trong Controller**
   ```csharp
   // ? BAD
   var semesters = _dbContext.CourseClasses
       .Select(c => c.Semester)
       .Distinct()
       .ToList(); // ? Logic này nên ? Service!
   ```

3. **Database access trong View**
   ```razor
   <!-- ? NEVER DO THIS -->
   @inject ApplicationDbContext _db
   @foreach (var item in _db.Items.ToList())
   ```

---

## ?? Khi migrate sang Database th?t

### Ch? c?n s?a Service, gi? nguyên Controller & View!

**SemesterService.cs:**
```csharp
public class SemesterService : ISemesterService
{
    private readonly ApplicationDbContext _context; // ? Add DbContext

    public SemesterService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<string> GetAllSemesters()
    {
        // BEFORE: FakeDatabase.CourseClasses
        // AFTER:  _context.CourseClasses
        return _context.CourseClasses
            .Select(c => c.Semester)
            .Distinct()
            .OrderByDescending(s => s)
            .ToList();
    }
}
```

**Program.cs:**
```csharp
// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Service v?n gi? nguyên
builder.Services.AddScoped<ISemesterService, SemesterService>();
```

**Controller & View:** ? **KHÔNG C?N S?A GÌ!**

---

## ?? Data Flow Diagram

```
?????????????????????????????????????????????????????????????????
?                    REQUEST LIFECYCLE                          ?
?????????????????????????????????????????????????????????????????
?                                                               ?
?  1. User Request                                              ?
?     GET /Student/Enrollment?semester=HK1-2024                 ?
?            ?                                                  ?
?            ?                                                  ?
?  2. Controller receives request                               ?
?     ???????????????????????????????????????????             ?
?     ? EnrollmentController.Index(semester)     ?             ?
?     ?                                          ?             ?
?     ? • Inject ISemesterService                ?             ?
?     ? • Call _semesterService.GetAllSemesters()?             ?
?     ? • Populate ViewBag                       ?             ?
?     ???????????????????????????????????????????             ?
?            ?                                                  ?
?            ?                                                  ?
?  3. Service queries data                                      ?
?     ???????????????????????????????????????????             ?
?     ? SemesterService.GetAllSemesters()        ?             ?
?     ?                                          ?             ?
?     ? • Query FakeDatabase (or real DB)       ?             ?
?     ? • Apply business logic                  ?             ?
?     ? • Return List<string>                   ?             ?
?     ???????????????????????????????????????????             ?
?            ?                                                  ?
?            ?                                                  ?
?  4. Controller passes to View                                 ?
?     ViewBag.Semesters = semesters;                           ?
?     ViewBag.CurrentSemester = currentSemester;               ?
?            ?                                                  ?
?            ?                                                  ?
?  5. View renders HTML                                         ?
?     ???????????????????????????????????????????             ?
?     ? @foreach (var sem in ViewBag.Semesters)  ?             ?
?     ? {                                        ?             ?
?     ?   <option value="@sem">@sem</option>     ?             ?
?     ? }                                        ?             ?
?     ???????????????????????????????????????????             ?
?            ?                                                  ?
?            ?                                                  ?
?  6. Response sent to browser                                  ?
?     <select>                                                 ?
?       <option value="HK1-2024">HK1-2024</option>             ?
?       <option value="HK2-2024">HK2-2024</option>             ?
?     </select>                                                ?
?                                                               ?
?????????????????????????????????????????????????????????????????
```

---

## ?? Testing Guide

### Unit Test Service
```csharp
[Fact]
public void GetAllSemesters_ReturnsDistinctSemesters()
{
    // Arrange
    var service = new SemesterService();
    
    // Act
    var result = service.GetAllSemesters();
    
    // Assert
    Assert.NotNull(result);
    Assert.True(result.Count > 0);
    Assert.Equal(result.Distinct().Count(), result.Count);
}
```

### Integration Test Controller
```csharp
[Fact]
public void Index_PopulatesViewBag()
{
    // Arrange
    var controller = new EnrollmentController(
        _studentService, 
        _enrollmentService, 
        _semesterService);
    
    // Act
    var result = controller.Index() as ViewResult;
    
    // Assert
    Assert.NotNull(result.ViewData["Semesters"]);
    Assert.NotNull(result.ViewData["CurrentSemester"]);
}
```

---

## ?? Benefits Summary

| Before (Hardcode) | After (ViewBag + Service) |
|-------------------|---------------------------|
| ? Data trong View | ? Data t? Service |
| ? Duplicate code | ? Reusable service |
| ? Khó test | ? Testable |
| ? Tight coupling | ? Loose coupling |
| ? Khó maintain | ? Easy to maintain |
| ? Không th? mock | ? Injectable/Mockable |

---

## ?? Conclusion

Migration t? hardcode sang ViewBag + Service:

1. ? **KHÔNG thay ??i Model/Database**
2. ? **Separation of Concerns**
3. ? **Testable & Maintainable**
4. ? **D? migrate sang DB th?t**
5. ? **Follow Clean Architecture**

### Final Architecture:
```
View (Presentation) 
  ? ViewBag
Controller (Orchestration)
  ? Inject
Service (Business Logic)
  ? Query
Data Layer (FakeDatabase ? Real DB)
```

---

*Document version: 1.0*  
*Last updated: December 2024*  
*Author: Student Management System Team*

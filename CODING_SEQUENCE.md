# 🚀 Trình Tự Code ASP.NET Core Web API — Từ Project Trống

> Hướng dẫn này mô tả **đúng thứ tự** xây dựng project `NewRentalCarManagerAPI` từ đầu.  
> Kiến trúc: **Layered Architecture** — Enums → Domain → Models → Infrastructure → Application → Controllers

---

## 📦 Bước 0: Tạo Project & Cài NuGet Packages

```bash
dotnet new webapi -n NewRentalCarManagerAPI
cd NewRentalCarManagerAPI
```

### Packages cần cài:
```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.IdentityModel.Tokens
dotnet add package BCrypt.Net-Next
dotnet add package Swashbuckle.AspNetCore
```

---

## 🔢 Bước 1: Enums (`Enums/AppEnums.cs`)

**Tại sao làm đầu tiên?** Enums là kiểu dữ liệu cơ bản, không phụ thuộc vào bất kỳ thứ gì khác. Models và DbContext sẽ dùng chúng.

```
Enums/
└── AppEnums.cs        ← BookingStatus, CarStatus, FuelType, PaymentStatus, ...
```

**Nội dung:** Định nghĩa tất cả `enum` dùng trong hệ thống. Dùng `[PgName("...")]` nếu map với PostgreSQL native enum.

---

## 🏗️ Bước 2: Common / Shared (`Common/`)

**Tại sao làm sớm?** `ApiResult<T>` là wrapper response dùng ở tất cả Controllers — cần có trước khi viết bất kỳ controller nào.

```
Common/
└── ApiResult.cs       ← Generic response wrapper { Success, Message, Data }
```

---

## 📐 Bước 3: Domain Interfaces (`Domain/Interfaces/`)

**Tại sao?** Định nghĩa **contract** (hợp đồng) trước khi implement. Giúp tách biệt tầng Application khỏi Infrastructure.

```
Domain/Interfaces/
├── IRepository.cs         ← GetByIdAsync, GetAllAsync, Query, AddAsync, Remove
├── IUnitOfWork.cs         ← Tập hợp tất cả IRepository<T> + SaveChangesAsync
├── IPasswordHasher.cs     ← Hash, Verify
├── ITokenService.cs       ← GenerateAccessToken, GenerateRefreshToken
├── ISlugService.cs        ← GenerateSlug
└── IEmailService.cs       ← SendEmailAsync (nếu có)
```

**Quy tắc:** Chỉ viết `interface`, KHÔNG implement ở đây.

---

## 🗃️ Bước 4: Models / Entities (`Models/`)

**Tại sao?** Models là trung tâm của hệ thống — DbContext, Repository, Service đều xoay quanh chúng.

### Thứ tự tạo Models (theo dependency):

```
Models/
├── 4.1  Role.cs                    ← Không FK đến đâu
├── 4.2  Permission.cs              ← Không FK đến đâu
├── 4.3  User.cs                    ← FK → Role
├── 4.4  RefreshToken.cs            ← FK → User
├── 4.5  OtpToken.cs                ← FK → User
├── 4.6  ExternalLogin.cs           ← FK → User
├── 4.7  ApiKey.cs                  ← FK → User
├── 4.8  Location.cs                ← Không FK đến đâu
├── 4.9  CarBrand.cs                ← Không FK đến đâu
├── 4.10 CarModel.cs                ← FK → CarBrand
├── 4.11 Car.cs                     ← FK → User(Owner), CarModel, Location
├── 4.12 CarPricing.cs              ← FK → Car
├── 4.13 CarAvailabilityBlock.cs    ← FK → Car
├── 4.14 Promotion.cs               ← Không FK đến đâu
├── 4.15 Booking.cs                 ← FK → Car, User, Location, Promotion
├── 4.16 Review.cs                  ← FK → Booking, User, Car
├── 4.17 Transaction.cs             ← FK → Booking, User
├── 4.18 DamageReport.cs            ← FK → Booking, Car
├── 4.19 Penalty.cs                 ← FK → Booking, User
├── 4.20 OwnerPayout.cs             ← FK → User(Owner), Booking
└── 4.21 NotificationLog.cs         ← FK → User
```

> **Nguyên tắc:** Model nào không phụ thuộc vào model khác thì tạo trước.

---

## 🗄️ Bước 5: DbContext (`Models/AppDbContext.cs`)

**Tại sao?** Sau khi có Models, cần DbContext để kết nối EF Core.

```
Models/
└── AppDbContext.cs    ← DbSet<T> cho tất cả Models + cấu hình Fluent API
```

**Nội dung cần viết:**
- `DbSet<T>` cho mỗi entity
- `OnModelCreating`: cấu hình schema, table name, enum mapping, foreign key, index

---

## 🔄 Bước 6: Migration

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> Chỉ chạy sau khi DbContext và Models hoàn chỉnh.

---

## 🔧 Bước 7: Infrastructure — Persistence (`Infrastructure/Persistence/`)

**Tại sao?** Implement các interface đã định nghĩa ở Bước 3.

```
Infrastructure/Persistence/
├── Repository.cs      ← Implement IRepository<T> dùng DbContext + DbSet
├── UnitOfWork.cs      ← Implement IUnitOfWork, lazy-init từng Repository
└── AppDbSeeder.cs     ← Seed dữ liệu mặc định (Roles, Permissions)
```

### Thứ tự:
1. `Repository.cs` — implement generic CRUD
2. `UnitOfWork.cs` — gom tất cả repository, expose `SaveChangesAsync()`
3. `AppDbSeeder.cs` — seed roles & permissions khi app khởi động

---

## ⚙️ Bước 8: Infrastructure — Services (`Infrastructure/Services/`)

Implement các service infrastructure (không liên quan đến business logic):

```
Infrastructure/Services/
├── BcryptPasswordHasher.cs    ← Implement IPasswordHasher dùng BCrypt
├── JwtTokenService.cs         ← Implement ITokenService — tạo JWT/Refresh token
├── SlugService.cs             ← Implement ISlugService
└── EmailService.cs            ← Implement IEmailService dùng SMTP
```

---

## 🔐 Bước 9: Infrastructure — Authorization (`Infrastructure/Authorization/`)

```
Infrastructure/Authorization/
├── PermissionRequirement.cs             ← IAuthorizationRequirement
├── HasPermissionAttribute.cs            ← Custom [HasPermission("resource","action")]
└── PermissionAuthorizationHandler.cs    ← IAuthorizationHandler — kiểm tra permission từ JWT claims
```

---

## 🧩 Bước 10: Middleware (`Middleware/`)

```
Middleware/
├── ExceptionMiddleware.cs          ← Global error handling, trả về ApiResult
└── UnitOfWorkCommitMiddleware.cs   ← Tự động SaveChangesAsync sau POST/PUT/PATCH/DELETE 2xx
```

---

## 📋 Bước 11: Application — DTOs trước

Với mỗi feature, tạo **DTOs trước**, sau đó mới viết Service:

```
Application/Features/{Feature}/
└── {Feature}Dtos.cs    ← Request DTO (CreateXxx, UpdateXxx) + Response DTO (XxxDto)
```

### Thứ tự các feature theo dependency:
```
11.1  Auth          ← LoginDto, RegisterDto, TokenDto
11.2  Users         ← UserDto, CreateUserDto, UpdateUserDto
11.3  Fleet         ← CarBrandDto, CarModelDto, LocationDto, CarDto, CarPricingDto, ...
11.4  Bookings      ← BookingDto, CreateBookingDto, ...
11.5  Ops           ← DamageReportDto, PenaltyDto
11.6  Payments      ← TransactionDto, OwnerPayoutDto
```

---

## 🧠 Bước 12: Application — Services

Sau khi có DTOs, implement business logic:

```
Application/Features/{Feature}/
└── {Feature}Service.cs    ← interface I{X}Service + class {X}Service
```

### Thứ tự:
```
12.1  AuthService           ← Register, Login, RefreshToken, Logout, ChangePassword
12.2  UserService           ← CRUD User, GetProfile
12.3  CarBrandService       ← CRUD CarBrand
12.4  CarModelService       ← CRUD CarModel
12.5  LocationService       ← CRUD Location
12.6  CarService            ← CRUD Car (SaveChanges trước khi re-query GetById)
12.7  CarPricingService     ← CRUD CarPricing
12.8  CarAvailabilityBlockService
12.9  PromotionService
12.10 BookingService        ← CreateBooking, ConfirmBooking, CancelBooking, ...
12.11 ReviewService
12.12 DamageReportService
12.13 PenaltyService
12.14 TransactionService
12.15 OwnerPayoutService
```

> ⚠️ **Lưu ý:** Service nào sau `CreateAsync` cần re-query entity kèm navigation properties thì phải gọi `await _uow.SaveChangesAsync()` trước `GetByIdAsync()`.

---

## 🌐 Bước 13: Controllers (`Controllers/`)

Sau khi có Service, viết Controller. Mỗi Controller chỉ gọi Service, không chứa business logic.

```
Controllers/
├── AuthController.cs
├── UsersController.cs
├── CarBrandsController.cs
├── CarModelsController.cs
├── LocationsController.cs
├── CarsController.cs
├── CarPricingsController.cs
├── CarAvailabilityBlocksController.cs
├── PromotionsController.cs
├── BookingsController.cs
├── ReviewsController.cs
├── PenaltiesController.cs
├── OwnerPayoutsController.cs
├── PermissionsController.cs
└── TransactionsController.cs (nếu có)
```

**Pattern chuẩn:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class XxxController : ControllerBase
{
    // [AllowAnonymous] cho GET public
    // [HasPermission("resource", "action")] cho write endpoints
    // Trả về ApiResult<T>
}
```

---

## 🚀 Bước 14: Program.cs (Wiring Everything Together)

Sau khi có tất cả, đăng ký DI và middleware pipeline:

```csharp
// Thứ tự đăng ký:
1. DbContext (Npgsql + enum mapping)
2. IUnitOfWork
3. Infrastructure Services (IPasswordHasher, ITokenService, ISlugService, IEmailService)
4. Authorization (IAuthorizationHandler, AddAuthorization)
5. Application Services (IAuthService, ICarService, ...)
6. Authentication (JWT Bearer)
7. CORS
8. Controllers + Swagger

// Middleware pipeline:
app.SeedDefaultRolesAndPermissions();
app.UseCors();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger() / UseSwaggerUI() [chỉ Development]
app.UseHttpsRedirection() [chỉ Production]
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UnitOfWorkCommitMiddleware>();
app.MapControllers();
```

---

## 📁 Cấu Trúc Thư Mục Cuối Cùng

```
NewRentalCarManagerAPI/
├── Common/
│   └── ApiResult.cs
├── Enums/
│   └── AppEnums.cs
├── Domain/
│   └── Interfaces/
│       ├── IRepository.cs
│       ├── IUnitOfWork.cs
│       ├── IPasswordHasher.cs
│       ├── ITokenService.cs
│       └── ISlugService.cs
├── Models/
│   ├── AppDbContext.cs
│   ├── User.cs, Role.cs, Permission.cs, ...
│   └── Car.cs, Booking.cs, Transaction.cs, ...
├── Infrastructure/
│   ├── Persistence/
│   │   ├── Repository.cs
│   │   ├── UnitOfWork.cs
│   │   └── AppDbSeeder.cs
│   ├── Services/
│   │   ├── BcryptPasswordHasher.cs
│   │   ├── JwtTokenService.cs
│   │   ├── SlugService.cs
│   │   └── EmailService.cs
│   └── Authorization/
│       ├── PermissionRequirement.cs
│       ├── HasPermissionAttribute.cs
│       └── PermissionAuthorizationHandler.cs
├── Application/
│   └── Features/
│       ├── Auth/
│       │   ├── AuthDtos.cs
│       │   └── AuthService.cs
│       ├── Fleet/
│       │   ├── FleetDtos.cs
│       │   └── FleetService.cs
│       ├── Bookings/
│       ├── Ops/
│       ├── Payments/
│       └── Users/
├── Controllers/
│   └── *.cs
├── Middleware/
│   ├── ExceptionMiddleware.cs
│   └── UnitOfWorkCommitMiddleware.cs
├── Migrations/
├── appsettings.json
└── Program.cs
```

---

## ✅ Checklist Tóm Tắt

| # | Bước | Mục tiêu |
|---|------|-----------|
| 0 | Setup Project + NuGet | Khởi tạo project, cài packages |
| 1 | Enums | Định nghĩa tất cả enum |
| 2 | Common | `ApiResult<T>` wrapper |
| 3 | Domain Interfaces | Contract cho Repository, UoW, Services |
| 4 | Models | Entities theo thứ tự dependency |
| 5 | DbContext | Kết nối EF Core, cấu hình schema |
| 6 | Migration | Tạo và chạy migration |
| 7 | Infrastructure/Persistence | Repository, UnitOfWork, Seeder |
| 8 | Infrastructure/Services | PasswordHasher, JwtToken, Email, Slug |
| 9 | Infrastructure/Authorization | Permission handler & attribute |
| 10 | Middleware | Exception handler, UoW commit |
| 11 | Application DTOs | Request/Response DTO cho từng feature |
| 12 | Application Services | Business logic |
| 13 | Controllers | HTTP endpoints |
| 14 | Program.cs | DI registration + middleware pipeline |

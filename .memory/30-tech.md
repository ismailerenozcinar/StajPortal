# StajPortal - Technology Landscape

## Technology Stack

### Backend
| Teknoloji | Versiyon | Kullanım |
|-----------|----------|----------|
| .NET | 8.0 | Runtime |
| ASP.NET Core | 8.0 | Web Framework |
| Entity Framework Core | 8.0.* | ORM |
| ASP.NET Core Identity | 8.0.* | Authentication |
| JWT Bearer | 8.0.* | API Authentication |

### Database
| Teknoloji | Versiyon | Kullanım |
|-----------|----------|----------|
| SQL Server | - | Primary Database |
| EF Core SqlServer | 8.0.* | Database Provider |

### Frontend
| Teknoloji | Kullanım |
|-----------|----------|
| Razor Views | Server-side rendering |
| Bootstrap | CSS Framework |
| jQuery | JavaScript utilities |
| jQuery Validation | Form validation |

### API Documentation
| Teknoloji | Versiyon | Kullanım |
|-----------|----------|----------|
| Swashbuckle.AspNetCore | 6.5.* | Swagger/OpenAPI |

## Package References
```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.*" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.*" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.*" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.*" />
```

## Development Environment

### Prerequisites
- Visual Studio 2022 veya VS Code
- .NET 8.0 SDK
- SQL Server (LocalDB veya Express)

### Project Structure
```
StajPortal/
├── Controllers/
│   ├── Api/
│   │   ├── AuthController.cs
│   │   └── JobsApiController.cs
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── CompanyController.cs
│   ├── HomeController.cs
│   ├── JobsController.cs
│   └── StudentController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Migrations/
│   ├── 20251117074622_InitialCreate.cs
│   └── ApplicationDbContextModelSnapshot.cs
├── Models/
│   ├── DTOs/
│   ├── Entities/
│   └── ViewModels/
├── Services/
│   ├── IJwtTokenService.cs
│   └── JwtTokenService.cs
├── Views/
│   ├── Account/
│   ├── Admin/
│   ├── Company/
│   ├── Home/
│   ├── Jobs/
│   ├── Shared/
│   └── Student/
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── lib/
├── Program.cs
├── appsettings.json
└── StajPortal.csproj
```

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=StajPortal;..."
  },
  "Jwt": {
    "Key": "...",
    "Issuer": "StajPortal",
    "Audience": "StajPortalAPI"
  }
}
```

### Identity Configuration
```csharp
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequireUppercase = true;
options.Password.RequireNonAlphanumeric = false;
options.Password.RequiredLength = 6;

options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
options.Lockout.MaxFailedAccessAttempts = 5;

options.User.RequireUniqueEmail = true;
```

### Cookie Configuration
```csharp
options.LoginPath = "/Account/Login";
options.LogoutPath = "/Account/Logout";
options.AccessDeniedPath = "/Account/AccessDenied";
options.ExpireTimeSpan = TimeSpan.FromDays(7);
options.SlidingExpiration = true;
```

### JWT Configuration
```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = "...",
    ValidAudience = "...",
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ClockSkew = TimeSpan.Zero
};
```

## Build & Deployment

### Development
```bash
dotnet restore
dotnet ef database update
dotnet run
```

### Production
```bash
dotnet publish -c Release
```

## API Endpoints

### Auth API (`/api/auth`)
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| POST | `/api/auth/login` | Giriş yap, JWT token al |
| POST | `/api/auth/register` | Kayıt ol |

### Jobs API (`/api/jobs`)
| Method | Endpoint | Açıklama | Auth |
|--------|----------|----------|------|
| GET | `/api/jobs` | İlan listesi | - |
| GET | `/api/jobs/{id}` | İlan detay | - |
| POST | `/api/jobs` | İlan oluştur | Company |
| POST | `/api/jobs/{id}/apply` | Başvur | Student |

### Swagger UI
- URL: `/swagger`
- Sadece Development modunda aktif

## Tool Chain

### IDE
- Visual Studio 2022
- VS Code + C# Dev Kit

### Database Tools
- SQL Server Management Studio
- Azure Data Studio

### Testing
- (Henüz test altyapısı yok)

### Version Control
- Git

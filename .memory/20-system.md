# StajPortal - System Architecture

## System Overview

```mermaid
graph TB
    subgraph Client["Client Layer"]
        Browser[Web Browser]
        API_Client[API Client / Mobile]
    end

    subgraph Web["Web Application Layer"]
        MVC[ASP.NET Core MVC]
        API[REST API Controllers]
        Swagger[Swagger UI]
    end

    subgraph Services["Service Layer"]
        Identity[ASP.NET Core Identity]
        JWT[JWT Token Service]
    end

    subgraph Data["Data Layer"]
        EF[Entity Framework Core]
        DbContext[ApplicationDbContext]
    end

    subgraph Database["Database"]
        SQLServer[(SQL Server)]
    end

    Browser --> MVC
    API_Client --> API
    API --> Swagger
    
    MVC --> Identity
    MVC --> EF
    API --> JWT
    API --> EF
    
    Identity --> DbContext
    JWT --> DbContext
    EF --> DbContext
    DbContext --> SQLServer
```

## Component Breakdown

### Controllers (MVC)
| Controller | Dosya | İşlev | Durum |
|------------|-------|-------|-------|
| HomeController | `/Controllers/HomeController.cs` | Anasayfa, Privacy | ✅ |
| AccountController | `/Controllers/AccountController.cs` | Kayıt, Giriş, Çıkış | ✅ |
| StudentController | `/Controllers/StudentController.cs` | Öğrenci dashboard, profil, başvurular | ✅ |
| CompanyController | `/Controllers/CompanyController.cs` | Firma dashboard, ilanlar, başvurular | ✅ |
| AdminController | `/Controllers/AdminController.cs` | Admin dashboard, onaylar | ✅ |
| JobsController | `/Controllers/JobsController.cs` | İlan listeleme, detay | ✅ |

### API Controllers
| Controller | Dosya | Endpoint | Durum |
|------------|-------|----------|-------|
| AuthController | `/Controllers/Api/AuthController.cs` | `/api/auth/*` | ✅ |
| JobsApiController | `/Controllers/Api/JobsApiController.cs` | `/api/jobs/*` | ✅ |

### Models

#### Entities
| Entity | Dosya | Açıklama |
|--------|-------|----------|
| ApplicationUser | `/Models/Entities/ApplicationUser.cs` | Identity User + Role, FullName |
| StudentProfile | `/Models/Entities/StudentProfile.cs` | Öğrenci profil bilgileri |
| CompanyProfile | `/Models/Entities/CompanyProfile.cs` | Firma profil bilgileri |
| JobPosting | `/Models/Entities/JobPosting.cs` | Staj ilanları |
| Application | `/Models/Entities/Application.cs` | Başvurular |
| Message | `/Models/Entities/Message.cs` | Mesajlaşma (kullanılmıyor) |

#### DTOs (API için)
| DTO | Dosya | Kullanım |
|-----|-------|----------|
| LoginRequestDto | `/Models/DTOs/LoginRequestDto.cs` | API Login |
| RegisterRequestDto | `/Models/DTOs/RegisterRequestDto.cs` | API Register |
| AuthResponseDto | `/Models/DTOs/AuthResponseDto.cs` | API Auth Response |
| JobPostingDto | `/Models/DTOs/JobPostingDto.cs` | API Job Response |

#### ViewModels
| ViewModel | Dosya | Kullanım |
|-----------|-------|----------|
| LoginViewModel | `/Models/ViewModels/LoginViewModel.cs` | MVC Login |
| RegisterViewModel | `/Models/ViewModels/RegisterViewModel.cs` | MVC Register |
| JobPostingViewModel | `/Models/ViewModels/JobPostingViewModel.cs` | MVC Job Form |

### Views Structure
```
Views/
├── Account/         # Login, Register, AccessDenied
├── Admin/           # Dashboard, JobApprovals
├── Company/         # Dashboard, Profile, PostJob, MyJobs, EditJob, Applications
├── Home/            # Index, Privacy
├── Jobs/            # Index, Details
├── Student/         # Dashboard, Profile, Applications
└── Shared/          # _Layout, Error, _ValidationScriptsPartial
```

### Services
| Service | Interface | Dosya | İşlev |
|---------|-----------|-------|-------|
| JwtTokenService | IJwtTokenService | `/Services/` | JWT Token oluşturma |

## Data Flow

### Başvuru Akışı
```mermaid
sequenceDiagram
    participant S as Student
    participant SC as StudentController
    participant DB as Database
    participant CC as CompanyController
    participant C as Company

    S->>SC: Apply(jobId)
    SC->>DB: Check existing application
    DB-->>SC: No duplicate
    SC->>DB: Create Application (Status: Pending)
    DB-->>SC: Success
    SC-->>S: Redirect to Applications

    C->>CC: Applications()
    CC->>DB: Get applications for company jobs
    DB-->>CC: Application list
    CC-->>C: Show applications

    C->>CC: AcceptApplication(id)
    CC->>DB: Update Status = "Accepted"
    DB-->>CC: Success
    CC-->>C: Updated status
```

### İlan Onay Akışı
```mermaid
sequenceDiagram
    participant C as Company
    participant CC as CompanyController
    participant DB as Database
    participant AC as AdminController
    participant A as Admin

    C->>CC: PostJob(model)
    CC->>DB: Create Job (IsApproved: false)
    DB-->>CC: Success
    CC-->>C: Redirect to MyJobs

    A->>AC: JobApprovals()
    AC->>DB: Get pending jobs
    DB-->>AC: Pending list
    AC-->>A: Show pending jobs

    A->>AC: ApproveJob(id)
    AC->>DB: Update IsApproved = true
    DB-->>AC: Success
    AC-->>A: Updated status
```

## Integration Points

### Authentication
- **Cookie Authentication**: MVC için - 7 gün süreli
- **JWT Bearer Authentication**: API için - 24 saat süreli
- **Identity**: Kullanıcı yönetimi, şifre politikaları

### Database Relationships
```mermaid
erDiagram
    ApplicationUser ||--o| StudentProfile : has
    ApplicationUser ||--o| CompanyProfile : has
    CompanyProfile ||--o{ JobPosting : creates
    JobPosting ||--o{ Application : receives
    StudentProfile ||--o{ Application : makes
    ApplicationUser ||--o{ Message : sends
    ApplicationUser ||--o{ Message : receives
    Application ||--o{ Message : contains
```

## Architectural Decisions

### ADR-001: Role Property in User
- **Karar**: Role bilgisi hem Identity Claim hem de User entity'de tutulur
- **Sebep**: Policy-based authorization ve kolay erişim

### ADR-002: Separate Profiles
- **Karar**: StudentProfile ve CompanyProfile ayrı tablolar
- **Sebep**: Farklı veri yapıları, kolay yönetim

### ADR-003: Job Approval Workflow
- **Karar**: İlanlar admin onayı gerektirir
- **Sebep**: Spam ve uygunsuz içerik kontrolü

## Non-Functional Requirements

### Security
- CSRF koruması (ValidateAntiForgeryToken)
- Şifre gereksinimleri: 6+ karakter, büyük/küçük harf, rakam
- Hesap kilitleme: 5 başarısız → 5 dakika
- HTTPS

### Performance
- Entity Framework lazy loading
- Database indexler (IsActive, IsApproved, Status, IsRead)

### Scalability
- Stateless API design
- JWT token based authentication

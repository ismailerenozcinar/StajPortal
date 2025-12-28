# StajPortal - Decision Log

## Decision Records

### DEC-001: ASP.NET Core MVC + Identity
**Tarih**: 2024-11  
**Bağlam**: Web framework ve authentication seçimi  
**Seçenekler**:
1. ASP.NET Core MVC + Identity
2. ASP.NET Core + Angular/React SPA
3. Blazor Server

**Karar**: ASP.NET Core MVC + Identity  
**Gerekçe**: 
- Hızlı geliştirme
- Built-in authentication
- Server-side rendering (SEO friendly)
- Daha az kompleks altyapı

**Etki**: Frontend için ayrı proje gerektirmeden hızlı MVP

---

### DEC-002: SQL Server + Entity Framework Core
**Tarih**: 2024-11  
**Bağlam**: Veritabanı ve ORM seçimi  
**Seçenekler**:
1. SQL Server + EF Core
2. PostgreSQL + EF Core
3. MongoDB

**Karar**: SQL Server + EF Core  
**Gerekçe**:
- Visual Studio ile entegrasyon
- LocalDB ile kolay development
- Code-first migrations

**Etki**: Hızlı schema değişiklikleri, güçlü relational model

---

### DEC-003: Role Property + Claims
**Tarih**: 2024-11  
**Bağlam**: Rol yönetimi stratejisi  
**Seçenekler**:
1. Sadece Identity Roles
2. Sadece Claims
3. User property + Claims (hybrid)

**Karar**: User property + Claims  
**Gerekçe**:
- Property: Kolay erişim ve query
- Claim: Policy-based authorization için gerekli
- Hybrid yaklaşım esneklik sağlar

**Etki**: İki yerde bakım gereksinimi, ancak kullanım kolaylığı

---

### DEC-004: Admin Onay Sistemi
**Tarih**: 2024-11  
**Bağlam**: İlan yayınlanma akışı  
**Seçenekler**:
1. Direkt yayınlama
2. Admin onayı (manuel)
3. AI-based moderation

**Karar**: Admin onayı (manuel)  
**Gerekçe**:
- Spam kontrolü
- İçerik kalitesi
- Güvenlik

**Etki**: Admin iş yükü artar, ancak kaliteli içerik garantisi

---

### DEC-005: REST API Eklenmesi
**Tarih**: 2024-11  
**Bağlam**: Mobil ve 3rd party erişim  
**Seçenekler**:
1. Sadece MVC
2. MVC + REST API
3. REST API only + SPA frontend

**Karar**: MVC + REST API  
**Gerekçe**:
- Mevcut MVC korunur
- Mobil app geliştirme imkanı
- Swagger ile dokümantasyon

**Etki**: İki farklı endpoint bakımı, ancak esneklik

---

### DEC-006: JWT Token Authentication (API)
**Tarih**: 2024-11  
**Bağlam**: API authentication  
**Seçenekler**:
1. Cookie (MVC ile aynı)
2. JWT Bearer Token
3. OAuth2/OpenID Connect

**Karar**: JWT Bearer Token  
**Gerekçe**:
- Stateless authentication
- Mobil app uyumlu
- Cross-origin support

**Etki**: Token yönetimi gerekli, 24 saat expiry

---

### DEC-007: Profil Tabloları Ayrımı
**Tarih**: 2024-11  
**Bağlam**: Öğrenci ve firma profilleri  
**Seçenekler**:
1. Tek tablo (tüm alanlar nullable)
2. Ayrı tablolar
3. EAV (Entity-Attribute-Value)

**Karar**: Ayrı tablolar (StudentProfile, CompanyProfile)  
**Gerekçe**:
- Farklı alanlar
- Clean data model
- Kolay query

**Etki**: İki profil tipi için ayrı CRUD işlemleri

---

### DEC-008: Bootstrap UI
**Tarih**: 2024-11  
**Bağlam**: Frontend framework  
**Seçenekler**:
1. Bootstrap
2. Tailwind CSS
3. Material UI

**Karar**: Bootstrap  
**Gerekçe**:
- ASP.NET MVC ile uyum
- Hızlı prototyping
- Responsive out-of-box

**Etki**: Standart görünüm, özelleştirme gerekebilir

---

## Pending Decisions

### PENDING-001: Mesajlaşma Sistemi Tasarımı
**Bağlam**: Kullanıcılar arası iletişim  
**Seçenekler**:
1. Başvuru bazlı mesajlaşma (Application → Messages)
2. Genel mesajlaşma (User → User)
3. Real-time chat (SignalR)

**Durum**: Tasarım bekliyor

---

### PENDING-002: CV Yükleme Stratejisi
**Bağlam**: CV dosyası saklama  
**Seçenekler**:
1. Sunucu dosya sistemi (wwwroot/uploads)
2. Azure Blob Storage
3. Sadece link (mevcut)

**Durum**: Karar bekliyor

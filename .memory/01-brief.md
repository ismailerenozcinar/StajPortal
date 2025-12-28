# StajPortal - Project Charter

## Project Outline
StajPortal (Internify), üniversite öğrencileri ile staj veren firmalar arasında köprü kuran kapsamlı bir staj portalı web uygulamasıdır. Platform, öğrencilerin staj ilanlarını keşfetmesine, başvuru yapmasına; firmaların ilan yayınlamasına ve başvuruları yönetmesine; admin kullanıcıların ise sistemi denetlemesine olanak tanır.

## Core Requirements

### Must-Have (P0)
1. **Kullanıcı Yönetimi**
   - Öğrenci ve Firma kayıt/giriş sistemi
   - Role-based yetkilendirme (Student, Company, Admin)
   - Profil düzenleme

2. **İlan Yönetimi**
   - Firma tarafından staj ilanı oluşturma
   - İlan listeleme ve filtreleme
   - Admin onay mekanizması

3. **Başvuru Sistemi**
   - Öğrenci başvuru yapabilme
   - Firma başvuruları görüntüleme
   - Başvuru kabul/red işlemleri

4. **Admin Panel**
   - Dashboard istatistikleri
   - İlan onaylama/reddetme
   - Kullanıcı yönetimi

### Should-Have (P1)
5. **Mesajlaşma Sistemi** (Planlandı, henüz tamamlanmadı)
6. **CV Yükleme** (Link olarak mevcut, dosya yükleme eksik)
7. **Raporlama** (View sayfası mevcut, içerik eksik)

### Nice-to-Have (P2)
8. E-posta bildirimleri
9. Gelişmiş arama/filtreleme
10. İstatistik grafikleri

## Success Criteria
- [x] Öğrenci ilanları görüntüleyebilir ve başvurabilir
- [x] Firma ilan oluşturabilir ve başvuruları yönetebilir  
- [x] Admin ilanları onaylayabilir
- [ ] Mesajlaşma sistemi çalışır durumda
- [ ] Kullanıcılar tam yönetilebilir (Admin/Users sayfası eksik)

## Stakeholders
- **Öğrenciler**: Staj arayan üniversite öğrencileri
- **Firmalar**: Stajyer arayan şirketler
- **Admin**: Platform yöneticisi

## Constraints
- **Teknoloji**: ASP.NET Core 8.0, SQL Server, Entity Framework Core
- **Güvenlik**: CSRF koruması, şifre politikaları, hesap kilitleme
- **Performans**: Standart web optimizasyonları

## Timeline
- **Faz 1** ✅: Temel kullanıcı yönetimi ve ilan sistemi
- **Faz 2** ✅: Başvuru sistemi ve Admin panel
- **Faz 3** 🔄: REST API (Swagger) implementasyonu
- **Faz 4** ⏳: Mesajlaşma ve gelişmiş özellikler

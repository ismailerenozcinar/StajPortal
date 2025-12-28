# StajPortal - Product Definition

## Problem Statements
1. Öğrenciler güvenilir staj fırsatlarını bulmakta zorlanıyor
2. Firmalar nitelikli stajyer adaylarına ulaşamıyor
3. Başvuru süreçleri dağınık ve takibi zor

## User Personas

### 1. Öğrenci (Student)
- Üniversite öğrencisi
- Staj arıyor
- Özgeçmiş/CV hazır
- Birden fazla ilana başvurmak istiyor

### 2. Firma (Company)
- Şirket yetkilisi/HR
- Stajyer adayı arıyor
- Birden fazla pozisyon için ilan verebilir
- Başvuruları değerlendirmek istiyor

### 3. Admin
- Platform yöneticisi
- İlanları onaylar/reddeder
- Kullanıcıları yönetir
- Sistem istatistiklerini takip eder

## User Journeys

### Öğrenci Akışı
```
Kayıt Ol → Profil Doldur → İlan Ara → İlan Detay → Başvur → Başvuru Takip
```

### Firma Akışı
```
Kayıt Ol → Profil Doldur → İlan Oluştur → (Admin Onayı) → Başvuru Al → Değerlendir
```

### Admin Akışı
```
Giriş → Dashboard → İlan Onaylama → Kullanıcı Yönetimi → Raporlar
```

## Feature Requirements

### 1. Anasayfa (Home/Index) ✅
- Son staj ilanları listelenir
- Her ilan kartında firma logosu/adı, pozisyon, şehir, departman, tarih
- İlan kartına tıklayarak detay sayfasına gidilir

### 2. Kayıt Ol (Account/Register) ✅
- Ad-soyad, e-posta, şifre, şifre tekrar, rol seçimi
- Form validasyonu (e-posta format, şifre kuralları)
- Otomatik giriş ve rol bazlı yönlendirme
- Otomatik profil oluşturma

### 3. Giriş Yap (Account/Login) ✅
- E-posta ve şifre alanları
- "Beni Hatırla" seçeneği (7 gün)
- Rol bazlı yönlendirme
- Hesap kilitleme (5 başarısız deneme → 5 dakika)

### 4. Çıkış Yap (Account/Logout) ✅
- Oturum sonlandırma
- Anasayfaya yönlendirme

### 5. İlanlar Sayfası (Jobs/Index) ✅
- Aktif ve onaylanmış ilanlar
- Filtreleme: Arama, Şehir, Departman
- İlan kartları: Logo, başlık, departman, şehir, tarihler
- Toplam ilan sayısı

### 6. İlan Detay (Jobs/Details) ✅
- Firma bilgileri ve ilan detayları
- Öğrenci için "Başvur" butonu
- Daha önce başvurulduysa uyarı
- Giriş yapmadan başvuru engeli

### 7. Öğrenci Dashboard (Student/Dashboard) ✅
- İstatistikler: Toplam/Bekleyen/Kabul başvuru, Aktif ilan
- Son 5 başvuru listesi
- Hızlı erişim butonları

### 8. Öğrenci Profil (Student/Profile) ✅
- Ad, üniversite, bölüm, mezuniyet yılı, GPA
- Telefon, şehir, hakkımda, CV linki

### 9. Öğrenci Başvurular (Student/Applications) ✅
- Tablo: Firma, İlan, Departman, Şehir, Tarih, Durum
- Son başvuru tarihine göre sıralama

### 10. Firma Dashboard (Company/Dashboard) ✅
- İstatistikler: Toplam/Aktif/Bekleyen ilan, Başvuru sayısı
- Hızlı erişim butonları

### 11. Firma Profil (Company/Profile) ✅
- Firma adı, sektör, adres, şehir
- Telefon, website, hakkımızda

### 12. İlan Yayınla (Company/PostJob) ✅
- Başlık, açıklama, departman, şehir, tarihler, aktiflik
- Admin onayı gerekir (IsApproved = false)

### 13. İlanlarım (Company/MyJobs) ✅
- İlan listesi: Başlık, departman, şehir, tarih, durum, onay
- Düzenle/Sil işlemleri

### 14. İlan Düzenle (Company/EditJob) ✅
- Mevcut bilgiler form alanlarında
- Güncelle butonu

### 15. Başvurular (Company/Applications) ✅
- Tüm başvurular: Öğrenci bilgileri, ilan, tarih, durum
- Kabul/Red işlemleri

### 16. Admin Dashboard (Admin/Dashboard) ✅
- Sistem istatistikleri
- Hızlı erişim butonları

### 17. Admin İlan Onayları (Admin/JobApprovals) ✅
- Onay bekleyen ilanlar
- Onayla/Reddet butonları

### 18. Admin Kullanıcılar (Admin/Users) ⚠️ EKSIK
- View sayfası mevcut ama boş
- Kullanıcı listeleme, aktif/pasif yapma eksik

### 19. Admin Raporlar (Admin/Reports) ⚠️ EKSIK
- View sayfası boş

### 20. Mesajlaşma Sistemi ⚠️ EKSIK
- Entity (Message) tanımlı
- DbContext ilişkileri mevcut
- Controller metodları boş: `return View();`

## UX Guidelines
- Modern, responsive tasarım
- Bootstrap tabanlı UI
- Türkçe arayüz
- TempData ile bildirim mesajları
- Form validasyonu

## User Metrics
- Kayıt yapan öğrenci sayısı
- Kayıt yapan firma sayısı
- Yayınlanan ilan sayısı
- Yapılan başvuru sayısı
- Kabul/Red oranı

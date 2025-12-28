# StajPortal - Current Focus & State

## Active Sprint/Cycle
**Sprint**: MVP Tamamlama ve Eksik Özelliklerin Eklenmesi

## Current State Summary
Proje büyük ölçüde tamamlanmış durumda. Temel işlevler (kayıt, giriş, ilan yönetimi, başvuru sistemi) çalışır vaziyette. Ancak bazı sayfalar boş/eksik ve mesajlaşma sistemi henüz implement edilmemiş.

## Recent Changes
- ✅ REST API eklendi (Auth + Jobs endpoints)
- ✅ JWT authentication implementasyonu
- ✅ Swagger UI entegrasyonu
- ✅ Admin ilan onaylama sistemi
- ✅ Başvuru kabul/red işlemleri

## Immediate Priorities

### P0 - Kritik (Öncelikli)
1. **Admin/Users sayfası** - View boş, kullanıcı yönetimi eksik
2. **Admin/Jobs sayfası** - View boş, tüm ilanları görme eksik
3. **Admin/Reports sayfası** - View boş, raporlama eksik

### P1 - Önemli
4. **Mesajlaşma sistemi** - Entity mevcut ama controller boş
5. **CV dosya yükleme** - Şu an sadece link var

### P2 - İyileştirme
6. **Test yazılması** - Unit/Integration test eksik
7. **E-posta bildirimleri** - Başvuru/onay bildirimleri
8. **UI/UX iyileştirmeleri**

## Open Questions
1. Admin kullanıcı yönetiminde hangi işlemler yapılabilmeli?
   - Kullanıcı silme?
   - Rol değiştirme?
   - Şifre sıfırlama?

2. Mesajlaşma sistemi nasıl çalışmalı?
   - Kimler mesajlaşabilmeli?
   - Başvuru bazlı mı genel mi?

3. Raporlar sayfasında hangi veriler gösterilmeli?
   - Tarih bazlı istatistikler?
   - Grafikler?
   - Export özelliği?

## Blockers
- Yok (aktif blocker bulunmuyor)

## Recent Learnings
- Swagger JWT authentication için Header'da "Bearer" prefix'siz token gönderilmeli
- Policy-based authorization claim tabanlı çalışıyor
- JsonSerializerOptions.ReferenceHandler.IgnoreCycles döngüsel referansları çözmek için gerekli

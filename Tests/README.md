# Röle 1 entegrasyonu

Mevcut Web Forms ekranı → BusinessLayer/Work/MakineRoleIslemleri → HW-584 HTTP akışı kullanılır. Veritabanı şeması veya stored procedure değişikliği gerekmez.

## Ayarlar

ModbusProjesi/Web.config:
- RoleBirMakineNo: varsayılan `18`; Makineler tablosundaki `makine_no` değeridir, `id` değildir. Aktif test makinesinin numarasıyla eşleşmelidir.
- RoleCihazAdresi: `http://192.168.5.190:8080/`.

Yalnızca eşleşen makinenin kontrol butonları kullanılabilir. Mevcut güncelleme yetkisi gereklidir. Diğer makineler sunucu tarafında da reddedilir.

Durdur → GET /01 → active-low röle bırakır.
Başlat → GET /00 → active-low röle çeker.

Başlat butonu mevcut akıştaki gibi açık duruş kaydı olduğunda görünür. İlk testte Durdur işlemi, ardından Başlat kullanılmalıdır. Uygulama açılışında otomatik donanım komutu gönderilmez.

Komutlar ortak HttpClient üzerinden, 4 saniye zaman aşımıyla gönderilir. HTTP yönlendirmeleri izlenmez. Duruş kaydı değişikliği işlem içinde hazırlanır; cihazdan başarılı yanıt alınırsa veritabanı işlemi tamamlanır. Hatalı yanıtta işlem geri alınır. Zaman aşımı komutun uygulanmadığını kanıtlamaz; kullanıcıya bu belirsizlik bildirilir. Cihaz yanıtından sonraki veritabanı hatası ayrıca bildirilir; otomatik ters komut gönderilmez.

Ekrandaki durum veritabanı kaydına dayanır; fiziksel röle geri bildirimi değildir. Eşzamanlı komut kilidi tek uygulama süreci içindir. Bu tek cihaz testi bir IIS worker süreciyle çalıştırılmalıdır.

## Doğrulama

Çözümü Debug yapılandırmasıyla derledikten sonra:

```powershell
.\Tests\RoleEntegrasyonTestleri.ps1
```

Testler yalnızca dinamik localhost portları kullanır; gerçek cihaza veya veritabanına bağlanmaz. Başlat/durdur yolları, başarısız HTTP yanıtı, yönlendirme, zaman aşımı, bozuk yanıt, yanlış/pasif makine ve hatalı ayarlar kontrol edilir. Test çalıştırıcısı geçici dizinde derlenir ve ayrı yapılandırma kullanır.

Gerçek cihaz ve veritabanıyla uçtan uca doğrulama ayrıca uygulama üzerinden yapılmalıdır: yetkili kullanıcıyla test makinesinde Durdur, ardından Başlat; LED değişimi ve duruş kaydı kontrol edilir.

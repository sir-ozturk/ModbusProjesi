# Röle 1 entegrasyonu

Mevcut Web Forms ekranı → BusinessLayer/Work/MakineRoleIslemleri → HW-584 HTTP akışı kullanılır.

## Çift yönlü durum senkronizasyonu

Kurulum: `./Tests/DonanimSenkronizasyonKur.ps1` komutunu çalıştırın. Web.config içindeki veritabanına tek transaction içinde nullable `relay_channel` alanını, kanal doğrulama/tekillik kısıtlarını, güncel makine sorgularını ve açık duruş indeksini uygular. Mevcut makine_no 18, kanal 1 olarak atanır; var olan atamalar korunur. İşlem hata verirse tamamı geri alınır.

Dashboard ilk açılışta ve görünürken yaklaşık 5 saniyede bir sunucu üzerinden GET /98 okur. Açık modal sırasında yenileme bekler. Dashboard kapalıyken arka plan takibi yapılmaz. Bir okuma 16 kanalın tamamını kapsar. Yanıt, çevresindeki boşluklar temizlendikten sonra tam 16 adet 0/1 olmalıdır; indeks `16 - relay_channel`, bit 1 duruyor, bit 0 çalışıyor anlamına gelir.

Yalnızca relay_channel atanmış aktif makineler aynı RoleCihazAdresi üzerinden izlenir. Kanal NULL ise donanım senkronizasyonuna dahil edilmez. Başka bir makineyi bu karta eşlemek için veritabanında relay_channel alanına 1–16 arasında benzersiz bir değer verin. Makine formu bu alanı değiştirmez. Birden fazla Ethernet kartı bu sürümün kapsamında değildir. Uygulamadan komut gönderme mevcut test kapsamındaki makine ve kanal 1 ile sınırlıdır; diğer kanallar okunabilir.

Donanım durumu açık MakineLoglari kaydıyla karşılaştırılır. Duruşa geçişte “Donanım üzerinden durduruldu (IO Control)” nedeni ile kayıt açılır, çalışmaya geçişte açık kayıt kapatılır. Aynı durum yeni kayıt oluşturmaz; mevcut duruş nedeni korunur. Ekleyen kullanıcı, gözlemi yapan dashboard oturumudur; bu kullanıcının donanım komutunu verdiği anlamına gelmez. Kapatma donanım gözlemi olduğundan kullanıcı alanı NULL kalır. Tüm gözlemler tek transaction içinde uygulanır. Hatalı yanıt veya kayıt hatasında değişiklikler geri alınır ve ekranda kayıtlı bilgilerin gösterildiği bildirilir.

Okuma ve uygulama komutları aynı süreç kilidini kullanır. Tek IIS worker süreci gereklidir. Gözlem gerçek IO çıkışını gösterir; motorun fiziksel hareketini doğrulamaz. İki okuma arasındaki kısa aç/kapa hareketleri ve dashboard kapalıyken gerçekleşen hareketlerin tam zamanı tespit edilemez; duruş zamanı tespit anıdır.

## Ayarlar

ModbusProjesi/Web.config:
- RoleBirMakineNo: varsayılan `18`; Makineler tablosundaki `makine_no` değeridir, `id` değildir. Aktif test makinesinin numarasıyla eşleşmelidir.
- RoleCihazAdresi: `http://192.168.5.190:8080/`.

Yalnızca eşleşen makinenin kontrol butonları kullanılabilir. Mevcut güncelleme yetkisi gereklidir. Diğer makineler sunucu tarafında da reddedilir.

Durdur → GET /01 → active-low röle bırakır.
Başlat → GET /00 → active-low röle çeker.

Başlat butonu mevcut akıştaki gibi açık duruş kaydı olduğunda görünür. İlk testte Durdur işlemi, ardından Başlat kullanılmalıdır. Uygulama açılışında otomatik donanım komutu gönderilmez.

Komutlar ortak HttpClient üzerinden, 4 saniye zaman aşımıyla gönderilir. HTTP yönlendirmeleri izlenmez. Duruş kaydı değişikliği işlem içinde hazırlanır; cihazdan başarılı yanıt alınırsa veritabanı işlemi tamamlanır. Hatalı yanıtta işlem geri alınır. Zaman aşımı komutun uygulanmadığını kanıtlamaz; kullanıcıya bu belirsizlik bildirilir. Cihaz yanıtından sonraki veritabanı hatası ayrıca bildirilir; otomatik ters komut gönderilmez.

Ekrandaki durum, başarılı IO okumalarıyla senkronize edilen veritabanı kaydına dayanır. Son okuma sonucu dashboard üzerinde gösterilir.

## Doğrulama

Çözümü Debug yapılandırmasıyla derledikten sonra:

```powershell
.\Tests\RoleEntegrasyonTestleri.ps1
```

Testler yalnızca dinamik localhost portları kullanır; gerçek cihaza veya veritabanına bağlanmaz. Başlat/durdur yolları, başarısız HTTP yanıtı, yönlendirme, zaman aşımı, bozuk yanıt, yanlış/pasif makine ve hatalı ayarlar kontrol edilir. Test çalıştırıcısı geçici dizinde derlenir ve ayrı yapılandırma kullanır.

Gerçek cihaz ve veritabanıyla uçtan uca doğrulama ayrıca uygulama üzerinden yapılmalıdır: yetkili kullanıcıyla test makinesinde Durdur, ardından Başlat; LED değişimi ve duruş kaydı kontrol edilir.

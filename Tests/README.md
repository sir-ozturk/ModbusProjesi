# SQL tabanlı röle yönetimi

## Yapı ve kullanım

Sol menü → Röle İşlemleri:
1. Ethernet Kartı Ekle: ad, model, IPv4 adresi, HTTP portu, aktiflik.
2. Röle Kartı Ekle: ad ve bağlı Ethernet kartı. Kanal sayısı 16'dır. Bir Ethernet kartına bir röle kartı atanır.
3. Makine–Röle Bağlantıları: röle kartı, 1–16 kanalı ve makine dropdown'larından seçim yapılır. Liste aynı sayfadadır; düzenleme ve silme buradan yapılır.

Her aktif kanal tek makineye, her makine tek aktif kanala bağlanır. Dolu seçenekler devre dışıdır; SQL benzersiz indeksleri eşzamanlı çakışmaları da engeller. Pasif kayıtları yeniden aktifleştirirken aynı kontroller geçerlidir. Pasif bağlantılar kanalı rezerve etmez.

IP ve HTTP portu EthernetKartlari tablosundadır. RoleKartlari fiziksel kartı, MakineRoleBaglantilari makinenin kayıt ID'si ile kanal ilişkisini tutar. Web.config içindeki eski röle ayarları artık kullanılmaz. Veritabanı bağlantısı ve diğer uygulama ayarları Web.config içinde kalır.

## Geçiş

Önce veritabanı yedeği alın. SQLCMD ile doğru sunucu/veritabanına aşağıdaki betiği `-b` seçeneğiyle çalıştırın:

```powershell
sqlcmd -S SUNUCU -d DB_MODBUS -E -C -b -i SP\Donanim\001_DonanimYonetimi.sql
```

Betik tek transaction içinde tabloları, indeksleri, prosedürleri, makine koruma trigger'ını ve dashboard sorgusunu kurar. İlk çalışmada Makine No 18'in tek aktif kaydını 192.168.5.190:8080 üzerindeki röle kartının Kanal 1 çıkışına taşır. Tek eşleşme yoksa veya çakışma varsa hata verir. Geçiş işareti ikinci çalışmada eski bağlantının tekrar oluşturulmasını önler; SQLCMD hata durumunda durmalıdır.

Mevcut makine yönetimi yetkileri denk yeni ekranlara taşınır. Mevcut rol/yetki ekranından daha sonra değiştirilebilir. Yeni ekranlar Ekranlar enum'unun sonuna eklenmiştir.

16 Eylül 2026 uygulaması: DB_MODBUS üzerinde geçiş uygulandı. Makine No 18 / Pekin / ID 8 → Kanal 1 korundu. Öncesinde SQL Server varsayılan yedek klasörüne DB_MODBUS_DonanimOncesi_20260916_01.bak COPY_ONLY yedeği alınıp VERIFYONLY ile doğrulandı. Geri dönüşte eski kod ile veritabanı sürümünün birlikte ele alınması gerekir; yedek sonrası işlemler geri yüklemede kaybolabileceğinden otomatik geri dönüş yapılmaz.

## Komut ve veri tutarlılığı

Ana sayfa, makine ID'siyle aktif bağlantıyı SQL'den bulur. HTTP komutları HW-584 için `(kanal-1)*2` (Başlat/OFF) ve bir sonraki sayı (Durdur/ON) olarak iki haneli hazırlanır. Kanal 1: /00 ve /01; Kanal 16: /30 ve /31. Fiziksel olarak doğrulanan mevcut bağlantı Kanal 1'dir. Diğer kanalların Ethernet IO–röle giriş kablolaması ayrıca doğrulanmalıdır.

Ortak HttpClient yönlendirmeyi takip etmez. Her istekte Hw584TimeoutSeconds (varsayılan 3 saniye) uygulanır. Komuttan önce /98 okunur; komuttan sonra hedef kanal tekrar doğrulanır. MakineLoglari ancak doğrulama sonrası güncellenir. Doğrulama tekrarları RelayVerificationAttempts ve RelayVerificationWaitMilliseconds ile ayarlanır. IO çıkışı, fiziksel motor hareketinin geri bildirimi değildir. Timeout halinde komut uygulanmış olabilir; otomatik ters komut gönderilmez.

## Çift yönlü durum senkronizasyonu

Dashboard ilk açılışta ve görünürken yaklaşık 5 saniyede bir /98 okur; açık modal sırasında yenileme bekler. Dashboard kapalıyken arka plan takibi yapılmaz. Aktif bağlantılar MakineRoleBaglantilari üzerinden bulunur; her Ethernet kartı kendi SQL IP/port adresinden bir kez okunur. Yanıt tam 16 bit olmalıdır; sağdaki bit kanal 1'dir.

Okuma ve log güncellemesi, komutlarla aynı SQL cihaz kilidini kullanır. Donanım durumu açık MakineLoglari ile karşılaştırılır; yalnızca değişiklik olduğunda duruş açılır veya kapatılır. Mevcut duruş nedeni korunur. Gözlenen duruşun nedeni “Donanım üzerinden durduruldu (IO Control)” olur. Ekleyen kullanıcı gözlemleyen oturumdur; komutu verdiği anlamına gelmez. Hatalı okuma veya kayıt hatasında transaction geri alınır ve son kayıtlı bilgilerin gösterildiği bildirilir. İki okuma arasındaki kısa hareketler tespit edilemeyebilir.

Eski DonanimSenkronizasyonKur.ps1 ve Makineler.relay_channel düzeni kullanılmaz; bağlantılar Röle İşlemleri ekranlarından yönetilir. Eski migration dosyası geçmiş için korunmuştur.


SQL sp_getapplock kilitleri uygulama süreçleri arasında da geçerlidir: komutlar ortak ayar kilidini Shared, cihaz kilidini Exclusive alır. Ayar yazmaları Exclusive ayar kilidi alır. Bekleme süresi sıfırdır; meşgulse mesaj döner. Farklı cihazlar paralel komut işleyebilir. Kilitler transaction bitince bırakılır. Yönetim değişiklikleri uygulamanın prosedürleri üzerinden yapılmalıdır.

Açık duruş kaydında makine bağlantısı ve cihaz adresi değiştirilemez. Aktif bağlantı varken makine/röle pasife alınamaz; bağlı üst kayıtlar silinemez. Yabancı anahtarlar silme zincirini korur. Tanımlama, pasifleştirme ve silme fiziksel komut göndermez. Süreli darbe kontrolü eklenmemiştir.

## Testler

Çözümü Debug derledikten sonra:

```powershell
.\Tests\RoleEntegrasyonTestleri.ps1
```

HTTP ve IO kontrolleri: Kanal 1/16 HTTP yolları, yönlendirme, hata, timeout, bozuk yanıt, geçersiz kanal/adres. Yalnızca localhost sahte HTTP sunucusu kullanılır.

SQL testleri için yedeği ayrı bir `DB_MODBUS_DonanimTest_*` veritabanına geri yükleyin, geçişi bu kopyada çalıştırın:

```powershell
.\Tests\DonanimVeritabaniTestleri.ps1 -Sunucu SUNUCU -Veritabani DB_MODBUS_DonanimTest_20260916
```

20 kontrol: atama sınırları, mükerrer makine/kanal, pasiflik, silme ilişkileri, açık duruş, iki bağlantıyla SQL kilitleri. Test betiği asıl DB_MODBUS adını reddeder; yalnızca test kopyasında sahte makine/kart kayıtları bırakır. Gerçek cihaza komut göndermez.

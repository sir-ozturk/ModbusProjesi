# Geliştirme ve doğrulama

Bu klasör uygulamanın çalışması için gerekli değildir. Mevcut davranışı korumak
 için röle ve talimat testleri tutulur. Test araçları uygulamayla dağıtılmaz.

## Mevcut akış

Web ekranı MakineDurdurmaTalimatlari kaydı oluşturur. Windows Forms uygulaması
Kendi projesindeki TalimatIsleyici ile kuyruğu takip eder. RelayPulseService, ON doğrulaması,
3 saniye bekleme ve OFF doğrulamasını gerçekleştirir. MakineRoleIslemleri
cihazın HTTP haberleşmesini yapar. Form oluşturulmuş olması bu sınıfları gereksiz
kılmaz. Eski RelayPulses/watchdog yapıları mevcut uygulamada kullanılmaz.

OFF dönüşü en çok üç kez denenir. ON doğrulandıysa OFF hatasında da duruş kaydı
korunur. Belirsiz sonuçta talimat Kontrol Gerekli olur ve işleyici durur. Açılışta
yarım kalan talimatlar için adres doğrulanarak yalnızca OFF dönüşü denenir;
eski ON komutu tekrarlanmaz. OFF, açık makine duruş kaydını kapatmaz.

Donanım yönetimi DonanimKontrolleri ve transaction kapsamlı uygulama kilitlerini
kullanır. Talimat işleyicisi aynı cihaz/ayar kaynaklarını oturum kilitleriyle
korur; ağ çağrısı sırasında transaction açık tutmaz. Doğrudan SQL çağrısı web
yetkilerini ve bütün C# kontrollerini çalıştırmaz.

## Cihaz ve veritabanı gerektirmeyen testler

Önce BusinessLayer projesini Debug olarak derleyin. Sonra proje kökünden:

```powershell
.\Tests\TalimatIsleyiciTests.ps1
.\Tests\RelayPulseTests.ps1
.\Tests\RoleEntegrasyonTestleri.ps1
.\Tests\ParametreKodTests.ps1
```

- TalimatIsleyiciTests: çift RUN, STOP, geçersiz adres, süre aşımı, belirsiz
  komut, kayıt hatası, açılış toparlaması ve oturum kilidi hataları.
  Formdaki işleyici kaynağını Visual Studio 2019 C# derleyicisiyle derler.
  Gerçek MakineDurdurmaTalimatlari metotlarını sahte VeritabaniIslemleri ve röleyle
  çalıştırır; ayrı bir depo/interface kullanılmaz.
- RelayPulseTests: ON/OFF, bekleme, tekrar sınırı ve aynı cihazda çakışma.
- ParametreKodTests: gerçek ParametreKontrolleri kaynağını sahte verilerle sınar.
  İlk boş numara, pasif kayıtların korunması, kilit hatası, güncellemede kodun
  korunması ve tekrar kullanılan numaranın eski nedenle karıştırılmaması doğrulanır.
- RoleEntegrasyonTestleri: yerel sahte HTTP sunucusuyla komut ve durum okuma.
  Gerçek röleye komut göndermez. Bazı senaryolar zaman aşımını bekler.

## Yalnızca ayrı test veritabanında

DonanimVeritabaniTestleri.ps1, DB_MODBUS_DonanimTest_* adlı ayrı veritabanında
altı SQL tablo kısıtını test eder ve işlemleri geri alır. Veritabanı bağlantısı
gerektirdiği için cihazsız testlerin parçası olarak otomatik çalıştırılmaz.

## SQL dosyaları

Otomatik parametre kodu geçişinde, eski kimliksiz duruş kayıtları için önce
`SP/SP_Parametreler/Parametreler_ESKI_DURUS_KIMLIKLERINI_BAGLA.sql` bir kez çalıştırılır.
Bu geçiş betiği kod numaraları yeniden kullanılmaya başladıktan sonra tekrar
çalıştırılmaz. Ardından `SP_Parametreler_KULLANIM_KAYITLARI_GETIR.sql` uygulanır.
Yeni parametre kodu mevcut bağlı işlem ve ModbusParametreAyar kilidi altında,
C# Work katmanında üretilir; mevcut EKLE prosedürü kodu parametre olarak alır.

Güncel prosedürler SP altındaki ilgili tablo klasörlerindedir. Eski tek cihaz
relay_channel geçişi ve artık bulunmayan SP/Donanim kurulum dosyalarına yapılan
yönlendirmeler kaldırılmıştır. Mevcut veritabanı tabloları, kolonları, geçmiş
kayıtları veya kurulu prosedürler bu kaynak temizliğiyle değiştirilmez.

Fiziksel makine duruşu, SQL bağlantı kesintisi ve süreç zorla kapatıldığında
sunucu/cihaz davranışı ayrıca saha veya ayrı test ortamında doğrulanmalıdır.

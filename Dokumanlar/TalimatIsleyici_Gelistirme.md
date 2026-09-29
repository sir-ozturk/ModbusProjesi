# Talimat işleyicisi ve uygulama yapısı

Web ekranı talimat oluşturur ve son talimatın durumunu gösterir. Windows Forms
uygulaması RUN durumunda talimatları işler. Ortak kod BusinessLayer içindedir.
Kullanıcıyla yapılan testlerde normal röle akışı, süre aşımı, bağlantı kesintisi
ve bağlantı geri geldikten sonraki çalışma doğrulandı. Fiziksel makine durdurma
testi henüz yapılmadı. Bu belge mevcut uygulamayı anlatır.

## Yapılar

- `BusinessLayer/Entity/MakineDurdurmaTalimatlari.cs`: OrtakAlanlar'dan türeyen
  talimat kaydı. Ekleme, listeleme, sıradaki talimatı alma, yarım kalanları bulma,
  komut öncesi kontrol ve sonuçlandırma mevcut VeritabaniIslemleri ile yapılır.
  URL, ekleme prosedüründe üretilir. Sonuç prosedürü duruş kaydını da açar;
  ayrıca MakineLoglari.Ekle çağrılmaz.
- `BusinessLayer/Work/VeritabaniIslemleri.cs`: Mevcut ortak veritabanı sınıfı.
  Form için bağlantı ve oturum kilidi metotları eklendi. Web tarafındaki mevcut
  transaction kilitleri ve bağlantı ayarları korunur.
- `MakineDurdurmaUygulamasi/MakineDurdurmaUygulamasi/TalimatIsleyici.cs`:
  Form projesindeki arka plan döngüsü; boşta saniyede bir kontrol eder.
  Aynı nesnede ikinci RUN reddedilir. Alınan talimat bitmeden sonraki işe geçilmez.
  Form1 bağlantı, kullanıcı, IP ve geçerlilik süresini doğrudan verir.
  Ayrı depo sınıfı veya talimat deposu interface'i bulunmaz.
- `RelayPulseService`: Mevcut ON/3 saniye/OFF davranışı korunur. Sonuca OFF
  doğrulaması ve komut gönderme girişimi bilgisi eklendi. Açılış toparlaması için
  yalnızca OFF gönderen GuvenliOffAsync metodu eklendi.

## Kilit ve bağlantı davranışı

Komut öncesi kontrol `SP_MakineDurdurmaTalimatlari_KOMUT_ONCESI_KONTROL`
prosedürüyle yapılır. Bu sürümü kullanmadan önce
`SP/SP_MakineDurdurmaTalimatlari/SP_MakineDurdurmaTalimatlari_KOMUT_ONCESI_KONTROL.sql`
hedef veritabanında çalıştırılmalıdır. Entity içinde doğrudan SQL sorgusu bulunmaz.

İşleyici SQL oturumu boyunca `ModbusTalimatIsleyici` kilidini tutar. Aynı
veritabanında bu işleyicinin ikinci örneği başlayamaz. Her komut için mevcut web
kodunun da kullandığı `ModbusDonanimAyar` (Shared) ve `ModbusRoleCihaz:<id>`
(Exclusive) kaynakları kullanılır. Web tarafındaki transaction kilitleriyle
uyumludur; işleyici bunları Session sahibiyle alır. Böylece cihaz çağrıları
sırasında SQL transaction açık kalmaz. Komut kilitleri sonuç yazıldıktan sonra
bırakılır; oturum kapanınca kalan kilitler de bırakılır.

Bağlantı havuzu ve otomatik reconnect kapalıdır. Bağlantı kaybında kilitlerin hâlâ
tutulduğu varsayılmaz. İlk bağlantı kurulamadığında tekrar denenir. Talimat alma
veya sonuç yazma yanıtı kaybolursa işlem uygulanmış olabilir; işleyici durur ve
aynı talimatı yeniden ON ile çalıştırmaz. Yeniden RUN öncesinde sonuç incelenmelidir.

## İş ve hata sonuçları

Komut öncesinde aktif bağlantı, aynı makine, IPv4/port/kanal, URL, SQL sunucu saatine
göre geçerlilik ve açık duruş kontrol edilir. URL'de sorgu, kullanıcı bilgisi ve
fragment kabul edilmez; varsayılan HTTP portu 80 doğru şekilde eşleştirilir.

ON ve OFF doğrulanmış ve hata yoksa Tamamlandı yazılır. Komut öncesi geçersizlik
Hatalı olur. Komut denendikten sonraki belirsizlik Kontrol Gerekli olur ve işleyici
yeni iş almadan durur. Sonuç yazılamazsa talimat numarası ve ON/OFF doğrulaması
mevcut yerel hata kayıt mekanizmasına iletilir. Bu kayıt da disk hatası nedeniyle
yazılamıyorsa kalıcılık garanti edilmez; veritabanındaki İşleniyor kaydı korunur.

Açılışta yarım kalan kayıtlar için önce tek işleyici kilidi alınır. Güncel bağlantı
kayıtlı URL ile eşleşiyorsa yalnızca OFF denenir; geçmiş ON sonucu varsayılmaz.
Kayıt Kontrol Gerekli yapılır. Açılış toparlaması sonrası yeni işler başlamadan
operatör kontrolü ve tekrar RUN gerekir. Değişmiş bağlantıya komut gönderilmez.

## Form uygulamasının kullanımı

Form, App.config içindeki ModbusDb bağlantısını, TalimatIsleyiciKullaniciId ve
TalimatGecerlilikSaniye ayarlarını okur. Mevcut seçimler kullanıcı ID 1 ve
180 saniyedir. Talimatı isteyen gerçek kullanıcının kimliği ekleyen_id ve duruş
kaydında korunur; işleyici kimliği sonuç güncellemesinde kullanılır.

RUN sırasında yeni CancellationTokenSource oluşturulur ve CalistirAsync sonucu
saklanır. Durum gösterimi için UI üzerinde oluşturulan Progress<string> verilir.
STOP ve EXIT token'ı iptal eder, ardından çalışan task beklenir. Token röle
çağrılarına verilmediği için devam eden OFF dönüşü ve sonuç kaydı yarıda kesilmez.
Task hatası formda gösterilir; otomatik RUN yapılmaz. Release çıktısındaki EXE,
BusinessLayer.dll ve EXE.config birlikte tutulur. Masaüstü kısayolu bu klasöre
bağlıdır. Form bir Windows servisi değildir; kullanıcı oturumu ve uygulama açık
kalmalıdır.

## Sonraki aşamalar ve doğrulama sınırı

Webde yetki kontrolü, bekleyen talimatın ekranda gösterilmesi ve başlat/duruşu kapat
işleminin bekleyen talimatla çakışmasını engelleyen kontroller bulunmaktadır.
Kontrol Gerekli kayıtlarının operatörce nasıl kapatılacağı henüz arayüze bağlanmadı.
Bu kayıtlar tarihçede kalır; yeniden RUN onları otomatik tekrar çalıştırmaz.

Testler gerçek SQL davranışını, ağ kopmasında sunucu kilitlerini veya fiziksel
makine duruşunu doğrulamaz. Ayrı test veritabanı ile prosedür/oturum kilidi
entegrasyon kontrolü sonraki onaya tabidir.

BusinessLayer Debug derlemesi sonrası `Tests/TalimatIsleyiciTests.ps1` ve
`Tests/RelayPulseTests.ps1` gerçek SQL/cihaz kullanmadan çalışır. Talimat testi,
form projesindeki işleyici kaynağını Visual Studio 2019 C# derleyicisiyle test
programına dahil eder. Gerçek entity metotları sahte VeritabaniIslemleri üzerinden
çalışır; ikinci işleyici, kilit bırakma hatası ve boş sonuç yanıtı da kontrol edilir.
Test için formun veya Release EXE'nin çalıştırılması gerekmez.

# Geliştirme yardımcıları

Bu klasör uygulamanın parçası değildir; site veya röle kontrolü için çalıştırılması gerekmez. Python ve ek test paketi kullanılmaz.

- `RoleEntegrasyonTestleri.ps1` / `.cs`: C# röle kodunu sahte HTTP sunucusuyla doğrular. Gerçek röleye komut göndermez.
- `DonanimVeritabaniTestleri.ps1`: Sadece ayrı test veritabanındaki 6 SQL tablo kısıtını doğrular; değişiklikleri geri alır. C# iş kurallarını test etmez.
- `DonanimSenkronizasyonKur.ps1`: Eski kurulumu engelleyen uyarı dosyasıdır; yeni kurulumda kullanılmaz.

## Uygulamanın yapısı

Prosedürler `SP/SP_EthernetKartlari`, `SP/SP_RoleKartlari` ve `SP/SP_MakineRoleBaglantilari` klasörlerindedir. Yalnızca kayıt ekler, günceller, siler veya okurlar.

İş kuralları `BusinessLayer/Work/DonanimKontrolleri.cs` dosyasındadır. Entity sınıfları yazmadan önce bu kontrolleri çağırır. Kilitler `VeritabaniIslemleri.UygulamaKilidiAl` ile aynı SQL bağlantısı ve transaction üzerinde alınır. Ekranlar `BAGIMLI` işlem açar; başarıda `Uygula`, hatada `GeriAl` çağırır.

Doğrudan prosedür çağrısı C# kontrollerini ve kilitlerini çalıştırmaz. Yönetim işlemleri uygulamadaki entity metotları üzerinden yapılmalıdır. Benzersiz indeksler, yabancı anahtarlar ve mevcut makine koruma trigger'ı SQL'de kalır.

Röle güvenlik mantığı güncellendi: ON normal/enerjisiz, OFF geçici durdurma tetiklemesidir. DURDUR artık merkezi `RelayPulseService` üzerinden kalıcı SQL kaydı, 10 saniyelik ON dönüşü, `/98` doğrulaması, retry, watchdog ve recovery kullanır. Dashboard IO bitine göre MakineLoglari kaydı açmaz/kapatmaz; makine duruş kaydı pulse tamamlandıktan sonra açık kalır.

Ayrıntılı kurulum ve çalışma açıklaması: [Röle pulse güvenliği](../RelayWatchdog/README.md). Kurulum sırası: `SP/Donanim/003_RelayPulseGuvenligi.sql`, uygulama kapalıyken `004_RelayPulseTurkiyeSaati.sql`, ardından `005_RelayPulseProsedurleri.sql`. IIS kesintisinde takip için harici Windows watchdog servisi kurulmalıdır.

Ek testler:

- `RelayPulseTests.ps1`: Sahte saat/depo/IO ile süre, duplicate, retry, recovery ve hata senaryoları.
- `RelayPulseSqlTests.ps1`: Yalnız ayrı pulse test veritabanında migration, SQL kilitleri ve kalıcı kayıt kontrolleri; gerçek röle kullanmaz.
## Veritabanı güncellemesi

Uygulama ve prosedürler birlikte güncellenmelidir. Yedek alındıktan sonra proje kök klasöründen SQLCMD ile:

```powershell
sqlcmd -S SUNUCU -d DB_MODBUS -E -C -b -i SP\Donanim\002_DonanimProsedurleriniGuncelle.sql
```

Bu betik ayrı prosedür dosyalarını yükler; makine atamalarını değiştirmez. İlk kurulumda `001_DonanimYonetimi.sql` kullanılır; mevcut Makine No 18'i `192.168.5.190:8080`, Kanal 1'e taşıyan geçişi de içerir.

## İsteğe bağlı doğrulama

Çözüm Debug derlendikten sonra:

```powershell
.\Tests\RoleEntegrasyonTestleri.ps1
.\Tests\DonanimVeritabaniTestleri.ps1 -Sunucu NEDEN10 -Veritabani DB_MODBUS_DonanimTest_20260917_Prosedur
```

Pulse veritabanı işlemleri mevcut Entity → VeritabaniIslemleri → stored procedure düzenini kullanır. Tarih alanları Türkiye saatinde tutulur. Yeni açıklama satırları Türkçedir.

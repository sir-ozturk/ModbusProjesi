# Röle pulse güvenliği ve kurulum

ON normal durumdur (röle bırakılmış/enerjisiz); OFF durdurma tetiklemesidir. Kanal 1 OFF `/00`, ON `/01`; kanal 2 OFF `/02`, ON `/03`. `/98` yanıtındaki 0 OFF, 1 ON'dur. Mevcut 16 bit protokol korunur; kanal 1 sağdaki bittir.

## Çalışma

- DURDUR, fiziksel komuttan **önce** `RelayPulses` kaydını bağımsız olarak commit eder. Kayıt yazılamazsa OFF gönderilmez. Sayfanın transaction'ı geri alınsa bile pulse kaydı korunur.
- OFF gönderilir ve `/98` okunur. HTTP yanıtı kaybolmuş olsa bile gerçek IO doğrulanır. OFF doğrulanamazsa başarı bildirilmez; hemen ON kurtarması denenir.
- `PulseBitis = PulseBaslangic + 10 saniye`. Tüm pulse zamanları Türkiye saatine göre tutulur (Turkey Standard Time, UTC+3). Uygulama ve SQL sorguları aynı saat dilimini kullanır; sunucuların sistem saatleri eşitlenmelidir.
- Uygulama içindeki worker 250 ms aralıklarla SQL'deki süresi dolan kayıtları tarar. OFF süresini belirleyen kaynak SQL kaydıdır; 10 saniyelik bellek içi timer kullanılmaz.
- Süre dolunca ON gönderilir ve `/98` ile doğrulanır. Başarısızlıkta ilk ON girişimine **ek olarak en fazla üç tekrar** vardır: 1, 2 ve 3 saniye sonra. Bekleme zamanları SQL'de saklanır. `ReleaseDenemeSayisi` ilk girişimi de sayar.
- 12 saniye içinde ON doğrulanmamışsa kritik durum kaydedilir. Watchdog, henüz işlenmemiş bu eşikte sıradaki tekrar beklemesini keserek ON dener. Devam eden HTTP isteği aynı kanalın kilidiyle korunur; başka bir ON/OFF işlemi onun önüne geçmez.
- Dört başarısız girişimden sonra kayıt aktif ve kritik kalır. Ayrı watchdog kurtarma girişimleri 5 saniye aralıklarla sürer. Bunlar yeni OFF pulse'ları değildir; toplam ON deneme sayısı artmaya devam eder. Her girişim `/98` ile kontrol edilir.
- Uygulama/servis başlangıcında süresi geçmiş aktif kayıtlar, eski tekrar bekleme zamanı dikkate alınmadan kurtarılır. Henüz süresi dolmayan kayıtlar normal taramaya bırakılır.
- SQL cihaz kilidi web uygulaması, farklı IIS worker'ları ve harici watchdog arasında ortaktır. Aktif makine ve fiziksel kanal için benzersiz indeksler vardır. İkinci DURDUR süreyi uzatmaz. Aktif pulse varken donanım adresi/bağlantı değişiklikleri ve kullanıcı BAŞLAT işlemi engellenir.
- ON doğrulanınca yalnız pulse kapanır. `MakineLoglari` açık DURDUR kaydı kapanmaz. Dashboard kayıtlı makine durumunu gösterir; kullanıcının mevcut BAŞLAT eylemi duruş kaydını açıkça sonlandırabilir. IO makinenin fiziksel hareketini ölçmez.

## Kurulum sırası

1. Mevcut veritabanı için `SP/Donanim/003_RelayPulseGuvenligi.sql` çalıştırılır. Betik mevcut makine/duruş kayıtlarını değiştirmez ve tekrar çalıştırılabilir. Önceki donanım şeması ve prosedürleri kurulu olmalıdır. Ardından uygulama kapalıyken `004_RelayPulseTurkiyeSaati.sql`, sonra `005_RelayPulseProsedurleri.sql` uygulanır. 004 mevcut UTC kayıtlarını yalnız bir kez Türkiye saatine dönüştürür; tekrar çalıştırmak saatleri yeniden kaydırmaz.
2. Web uygulaması ve `BusinessLayer.dll` birlikte yayımlanır. `Global.asax` yayına dahil olmalıdır. Uygulama kimliğine `App_Data` klasöründe yazma izni verilir.
3. IIS dışında çalışacak `RelayWatchdog` projesinin çıktısı ayrı bir klasöre kopyalanır. `ModbusRelayWatchdog.exe.config` içindeki `ModbusDb`, web uygulamasıyla **aynı veritabanına** ayarlanır. Örnekteki `SUNUCU` yer tutucudur. Servis hesabının SQL erişimi ve kurulum klasöründeki `App_Data` için yazma izni olmalıdır.
4. Yönetici terminalinde Windows servisi kaydedilir. Aşağıdaki yol örnektir; gerçek kurulum yolu kullanılmalıdır:

```powershell
sc.exe create ModbusRelayWatchdog binPath= '"C:\Services\ModbusRelayWatchdog\ModbusRelayWatchdog.exe"' start= auto
sc.exe failure ModbusRelayWatchdog reset= 86400 actions= restart/5000/restart/5000/restart/5000
sc.exe start ModbusRelayWatchdog
```

Servis hesabı kurumun SQL kimlik doğrulama düzenine göre ayarlanmalıdır. Konsol denemesi için `ModbusRelayWatchdog.exe --console` kullanılır; Ctrl+C ile durur. Bu işlem bağlı veritabanındaki aktif pulse'lara **gerçek ON komutu gönderir**. Gerçek cihaz üzerinde kontrollü devreye alma dışında çalıştırılmamalıdır.

Harici servis kurulmadan yalnız uygulama içi watchdog aktiftir; IIS kapalı veya uyku durumundayken çalışamaz. Uygulama yeniden açıldığında recovery çalışır. Bağımsız servis IIS kesintisinde takip eder; SQL, ağ veya tüm makine kapalıysa yazılım kesin 10 saniyelik fiziksel süre garantisi veremez.

## Kayıtlar ve hata takibi

Aynı IP ve porttaki farklı kanalların komut/doğrulama işlemleri cihaz kilidiyle sırayla yürütülür. Cihaz meşgulse yeni DURDUR reddedilir ve tekrar denenebilir. HTTP bağlantısı her yanıttan sonra kapatılır. Teknik exception ayrıntıları olay kayıtlarında saklanır; dashboard kısa hata mesajını gösterir.

- `dbo.RelayPulses`: adres ve port anlık görüntüsü, kanal, başlangıç/bitiş, ON doğrulama zamanı, aktiflik, deneme sayısı, sonraki deneme, son hata ve kritik işareti.
- `dbo.RelayPulseEvents`: komut hazırlığı, HTTP sonucu, ham `/98`, beklenen bitin doğrulanması, tekrar ve exception kayıtları.
- `dbo.RelayPulseAudit`: makine/adres/zaman bilgileriyle birleştirilmiş olay görünümü. `GercekOnZamani`, ON'un yazılım tarafından doğrulandığı zamandır; elektriksel geçişin hassas ölçümü değildir. OFF komut hazırlık ve yanıt zamanları olay tablosundadır.
- SQL hatasında bağımsız fallback: `App_Data/RelayPulseCritical-yyyyMMdd.log` (Türkiye tarihli). Dosyaya da yazılamazsa `Trace` ile hata bildirilir. Log dosyaları için kurumun arşivleme/temizleme politikasını uygulayın.
- Dashboard açıkken yaklaşık 5 saniyede bir kalıcı pulse uyarıları yenilenir. Veritabanı okunamıyorsa ayrıca kritik takip uyarısı gösterilir. Başarılı kurtarma sonrası aktif hata kaybolur, olay geçmişi ve kritik işareti saklanır.

`MakineLoglari` yazımı OFF doğrulamasından sonra mevcut transaction ile yapılır. Bu aşamada ayrı bir hata olursa sayfa başarı bildirmez; kalıcı pulse kaydı yine ON kurtarmasını sağlar. Makine duruş kaydı hatası ayrıca günlükten incelenmelidir.

`/98`, mekanik kontak yapışmasını veya makinenin gerçek hareketini tespit etmez. Bu yapı yazılım ve HW-584 IO katmanını takip eder.

## Doğrulama

Çözüm Debug derlendikten sonra:

```powershell
.\Tests\RoleEntegrasyonTestleri.ps1
.\Tests\RelayPulseTests.ps1
.\Tests\RelayPulseSqlTests.ps1 -Sunucu NEDEN10 -Veritabani DB_MODBUS_DonanimTest_Pulse_20260918
```

İlk test localhost HTTP sunucusunu, ikinci test sahte saat/depo/IO'yu kullanır. SQL testi yalnız `DB_MODBUS_DonanimTest_Pulse_` önekli ayrı test veritabanını kabul eder; gerçek röle kullanmaz. Test veritabanının önceden oluşturulması gerekir. SQL testi migration'ı iki kez uygular; kalıcılık, benzersizlik, oturum kilidi, recovery, retry ve hata sorgularını sınar. Test verileri inceleme için test veritabanında bırakılır.

## Projedeki yerleşim

İş akışı Work/RelayPulseService.cs içindedir. Veri modeli Entity/RelayPulse.cs, tablo işlemleri Entity/RelayPulses.cs ve Entity/RelayPulseEvents.cs içindedir. Depo, mevcut VeritabaniIslemleri sınıfını kullanır; SQL işlemleri SP/SP_RelayPulses ve SP/SP_RelayPulseEvents altındaki prosedürlerde bulunur. Test arayüzleri Interfaces klasöründedir. İşleyiş ve public servis çağrıları korunmuştur. Eski dosya logları geçmiş kayıt olarak korunur; yeni dosya logları Türkiye saatiyle yazılır.

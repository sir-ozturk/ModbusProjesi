window.onload = function () {
    // 1. C# tarafındaki lblYeniSifre'yi tarayıcıda yakalıyoruz
    var sifreLabel = document.getElementById('<%= lblYeniSifre.ClientID %>');

    // 2. Eğer Label içi boş değilse (yani bir şifre yazılmışsa) süreyi başlat
    if (sifreLabel && sifreLabel.innerText.trim() !== "") {

        // 3. 120000 milisaniye (yani tam 2 dakika) sonra bu fonksiyon çalışsın:
        setTimeout(function () {
            // Şifreyi ekrandan tamamen gizle
            sifreLabel.style.display = 'none';
        }, 120000);
    }
};
(function () {
    "use strict";
    var turler = {
        SUCCESS: { baslik: "Başarılı", renk: "#198754", yazi: "#ffffff", etiket: "lbl_success" },
        FAIL: { baslik: "Hata", renk: "#dc3545", yazi: "#ffffff", etiket: "lbl_error" },
        INFO: { baslik: "Bilgi", renk: "#0dcaf0", yazi: "#212529", etiket: "lbl_info" },
        WARNING: { baslik: "Uyarı", renk: "#ffc107", yazi: "#212529", etiket: "lbl_warning" }
    };
    window.modbusMesajiGoster = function (metin, tur, sure) {
        var element = document.getElementById("toastMesaj");
        if (!element || !metin) return;
        var ayar = turler[tur] || turler.INFO;
        var onceki = bootstrap.Toast.getInstance(element);
        if (onceki) onceki.dispose();
        element.classList.remove("text-bg-success", "text-bg-danger", "text-bg-info", "text-bg-warning");
        element.style.backgroundColor = ayar.renk;
        element.style.color = ayar.yazi;
        document.getElementById("toastBaslik").textContent = ayar.baslik;
        element.querySelector(".toast-body").textContent = metin;
        new bootstrap.Toast(element, { autohide: true, delay: sure || 5000 }).show();
    };
    function ilkMesajiGoster() {
        var element = document.getElementById("toastMesaj");
        if (!element) return;
        var anahtarlar = Object.keys(turler);
        for (var i = 0; i < anahtarlar.length; i++) {
            var tur = anahtarlar[i];
            var etiket = element.querySelector('[id$="' + turler[tur].etiket + '"]');
            if (etiket && etiket.textContent.trim()) {
                window.modbusMesajiGoster(etiket.textContent.trim(), tur);
                break;
            }
        }
    }
    if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", ilkMesajiGoster);
    else ilkMesajiGoster();
})();

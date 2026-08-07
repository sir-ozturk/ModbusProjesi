window.onload = function () {

    var telefonKutusu = document.querySelector('.telefon-kontrol-sinifi');

    if (telefonKutusu) {

        function TelefonFormatla() {

            var yazi = telefonKutusu.value;

            var sadeceSayilar = yazi.replace(/[^0-9]/g, '');

            if (sadeceSayilar.length > 0 &&
                sadeceSayilar.charAt(0) !== '5') {

                telefonKutusu.value = '';
                return;
            }

            var sonHali = '';

            if (sadeceSayilar.length > 0) {
                sonHali = sadeceSayilar.substring(0, 3);
            }

            if (sadeceSayilar.length > 3) {
                sonHali += '-' + sadeceSayilar.substring(3, 6);
            }

            if (sadeceSayilar.length > 6) {
                sonHali += '-' + sadeceSayilar.substring(6, 8);
            }

            if (sadeceSayilar.length > 8) {
                sonHali += '-' + sadeceSayilar.substring(8, 10);
            }

            telefonKutusu.value = sonHali;
        }

        // Sayfa ilk açıldığında
        TelefonFormatla();

        // Kullanıcı yazdıkça
        telefonKutusu.oninput = function () {
            TelefonFormatla();
        };
    }
};
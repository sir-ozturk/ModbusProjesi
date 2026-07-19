window.onload = function () {
    var mesajKutusu = document.querySelector('.mesaj-kutusu');
    
    if (mesajKutusu) {
        setTimeout(function () {
            mesajKutusu.style.transition = "opacity 0.6s ease";
            mesajKutusu.style.opacity = "0";
            
            setTimeout(function () {
                mesajKutusu.style.display = "none";
            }, 600);
            
        }, 120000); 
    }
};
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MODBUS KULLANICI GİRİŞ PANELİ</title>
    <link href="../Styles/Login.css?v=1" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/7.0.1/css/all.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://challenges.cloudflare.com/turnstile/v0/api.js" async="async" defer="defer"> </script>
</head>
<body>
    <form id="form1" runat="server" defaultbutton="btnGiris">
        <div class="toast-container position-fixed top-0 start-50 translate-middle-x p-3" style="z-index: 2000;">
            <div id="toastMesaj" class="toast" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="toast-header">
                    <strong id="toastBaslik" class="me-auto">Bildirim</strong>
                    <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Kapat"></button>
                </div>

                <div class="toast-body">
                    <asp:Label ID="lbl_success" runat="server"></asp:Label>
                    <asp:Label ID="lbl_error" runat="server"></asp:Label>
                    <asp:Label ID="lbl_info" runat="server"></asp:Label>
                    <asp:Label ID="lbl_warning" runat="server"></asp:Label>
                </div>
            </div>
        </div>
        <div class="giris-konteynır">
            <div class="logo">
                <asp:Image ID="loginLogo" runat="server" ImageUrl="~/Files/Images/LoginSayfasi_1.png" CssClass="profil-img" />
            </div>
            <div class="baslik">
                <h2>Modbus Kullanıcı Giriş Paneli</h2>
            </div>
            <div class="giris-alanlari">
                <asp:TextBox ID="txtKullaniciAdi" runat="server" placeholder="Kullanıcı Kodu"></asp:TextBox>
            </div>
            <div class="giris-alanlari">
                <asp:TextBox ID="txtSifre" runat="server" TextMode="Password" placeholder="Şifre"></asp:TextBox>
            </div>
            <div class="turnstile-alani">
                <div id="turnstileWidget" runat="server" class="cf-turnstile"></div>
            </div>
            <div class="buton-aksiyonu">
                <asp:Button ID="btnGiris" runat="server" Text="Giriş Yap" CssClass="giris-butonu" OnClick="btnGiris_Click" />
                <a href="SifremiUnuttum.aspx" class="sifremi-unuttum-link">Şifremi Unuttum</a>
            </div>
        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/js/bootstrap.bundle.min.js"></script>
     <script>
        document.addEventListener("DOMContentLoaded", function () {
            var success = document.getElementById("<%= lbl_success.ClientID %>");
            var error = document.getElementById("<%= lbl_error.ClientID %>");
            var info = document.getElementById("<%= lbl_info.ClientID %>");
            var warning = document.getElementById("<%= lbl_warning.ClientID %>");

            var toastElement = document.getElementById("toastMesaj");
            var toastBaslik = document.getElementById("toastBaslik");

            if (success && success.innerText.trim() !== "") {
                toastBaslik.innerText = "Başarılı";
                toastElement.classList.add("text-bg-success");
                var toast = new bootstrap.Toast(toastElement, {
                    autohide: true,
                    delay: 10000
                });

                toast.show();
            }
            else if (error && error.innerText.trim() !== "") {
                toastBaslik.innerText = "Hata";
                toastElement.classList.add("text-bg-danger");
                var toast = new bootstrap.Toast(toastElement, {
                    autohide: true,
                    delay: 10000
                });

                toast.show();
            }
            else if (info && info.innerText.trim() !== "") {
                toastBaslik.innerText = "Bilgi";
                toastElement.classList.add("text-bg-info");
                var toast = new bootstrap.Toast(toastElement, {
                    autohide: true,
                    delay: 10000
                });

                toast.show();
            }
            else if (warning && warning.innerText.trim() !== "") {
                toastBaslik.innerText = "Uyarı";
                toastElement.classList.add("text-bg-warning");
                var toast = new bootstrap.Toast(toastElement, {
                    autohide: true,
                    delay: 10000
                });

                toast.show();
            }
        });
     </script>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MODBUS KULLANICI GİRİŞ PANELİ</title>

    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/7.0.1/css/all.min.css" rel="stylesheet" />
    <script src="https://challenges.cloudflare.com/turnstile/v0/api.js" async="async" defer="defer"></script>
    <link href="../Styles/Login.css?v=4" rel="stylesheet" />
</head>

<body>
    <form id="form1" runat="server" defaultbutton="btnGiris">

        <div class="toast-container position-fixed start-50 translate-middle-x p-3" style="top: 25px; z-index: 2000;">
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

        <div class="container min-vh-100 d-flex align-items-center justify-content-center">

            <div class="row justify-content-center w-100">

                <div class="col-12 col-md-8 col-lg-5 col-xl-4">

                    <div class="card shadow border-0 login-kart">

                        <div class="card-body p-4 p-md-5">

                            <div class="text-center mb-4">

                                <asp:Image ID="loginLogo" runat="server" ImageUrl="~/Files/Images/Logo.png" CssClass="login-logo mb-3" />

                                <h2 class="fw-bold text-modbus mb-2">MODBUS</h2>

                                <p class="text-danger mb-0">Kullanıcı Giriş Paneli</p>

                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-bold text-modbus">Kullanıcı Adı</label>

                                <asp:TextBox ID="txtKullaniciAdi" runat="server" CssClass="form-control form-control-lg form-control-modbus" placeholder="Kullanıcı Kodu"></asp:TextBox>
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-bold text-modbus">Şifre</label>

                                <asp:TextBox ID="txtSifre" runat="server" TextMode="Password" CssClass="form-control form-control-lg form-control-modbus" placeholder="Şifre"></asp:TextBox>
                            </div>

                            <div class="mb-3 d-flex justify-content-center">
                                <div id="turnstileWidget" runat="server" class="cf-turnstile"></div>
                            </div>

                            <div class="d-grid mb-3">
                                <asp:Button ID="btnGiris" runat="server" Text="Giriş Yap" CssClass="btn btn-modbus-giris btn-lg fw-bold" OnClick="btnGiris_Click" />
                            </div>

                            <div class="text-center">
                                <a href="SifremiUnuttum.aspx" class="text-decoration-none text-modbus fw-bold">Şifremi Unuttum</a>
                            </div>

                        </div>

                    </div>

                </div>

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
                new bootstrap.Toast(toastElement, { autohide: true, delay: 10000 }).show();
            }
            else if (error && error.innerText.trim() !== "") {
                toastBaslik.innerText = "Hata";
                toastElement.classList.add("text-bg-danger");
                new bootstrap.Toast(toastElement, { autohide: true, delay: 8000 }).show();
            }
            else if (info && info.innerText.trim() !== "") {
                toastBaslik.innerText = "Bilgi";
                toastElement.classList.add("text-bg-info");
                new bootstrap.Toast(toastElement, { autohide: true, delay: 6000 }).show();
            }
            else if (warning && warning.innerText.trim() !== "") {
                toastBaslik.innerText = "Uyarı";
                toastElement.classList.add("text-bg-warning");
                new bootstrap.Toast(toastElement, { autohide: true, delay: 8000 }).show();
            }
        });
    </script>
</body>
</html>

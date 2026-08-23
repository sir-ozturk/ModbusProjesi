<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SifremiUnuttum.aspx.cs" Inherits="SifremiUnuttum" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MODBUS ŞİFREMİ UNUTTUM PANELİ</title>

    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link href="../Styles/SifremiUnuttum.css?v=3" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/7.0.1/css/all.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css" rel="stylesheet" />

    <script src="../Scripts/SifremiUnuttumZamanlayici.js"></script>
    <script src="https://challenges.cloudflare.com/turnstile/v0/api.js" async="async" defer="defer"></script>
</head>

<body>

    <form id="form1" runat="server" defaultbutton="btnSifirla">

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

        <div class="container min-vh-100 d-flex align-items-center justify-content-center">

            <div class="row justify-content-center w-100">

                <div class="col-12 col-md-8 col-lg-5 col-xl-4">

                    <div class="card shadow border-0 sifre-kart">

                        <div class="card-body p-4 p-md-5">

                            <div class="text-center mb-4">

                                <asp:Image ID="loginLogo" runat="server" ImageUrl="~/Files/Images/Logo.png" CssClass="sifre-logo mb-3" />

                                <h2 class="fw-bold text-modbus mb-2">Şifremi Unuttum
                                </h2>

                                <p class="text-danger mb-0">
                                    Hesabınızı doğrulayarak geçici şifre oluşturun
                                </p>

                            </div>

                            <div class="mb-3">

                                <label class="form-label fw-bold text-modbus">
                                    E-Mail
                                </label>

                                <asp:TextBox ID="TxtResetMail" runat="server" TextMode="Email" CssClass="form-control form-control-lg form-control-modbus" placeholder="Mail"></asp:TextBox>

                            </div>

                            <div class="mb-3">

                                <label class="form-label fw-bold text-modbus">
                                    Kullanıcı Kodu
                                </label>

                                <asp:TextBox ID="TxtResetKullaniciAdi" runat="server" CssClass="form-control form-control-lg form-control-modbus" placeholder="Kullanıcı Kodu"></asp:TextBox>

                            </div>

                            <div class="mb-3 d-flex justify-content-center">

                                <div id="turnstileWidget" runat="server" class="cf-turnstile"></div>

                            </div>

                            <div class="d-grid mb-3">

                                <asp:Button ID="btnSifirla" runat="server" Text="Sıfırla" CssClass="btn btn-modbus-sifirla btn-lg fw-bold" OnClick="btnSifirla_Click" />

                            </div>

                            <div class="text-center">

                                <a href="Login.aspx" class="text-decoration-none text-modbus fw-bold">
                                    <i class="fa-solid fa-arrow-left me-1"></i>
                                    Giriş ekranına dön
                                </a>

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

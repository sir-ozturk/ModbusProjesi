<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ModbusProjesi.Pages.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MODBUS KULLANICI GİRİŞ PANELİ</title>
    <link href="../Styles/Login.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/7.0.1/css/all.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server" defaultbutton="btnGiris">
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
            <div class="buton-aksiyonu">
                <asp:Button ID="btnGiris" runat="server" Text="Giriş Yap" CssClass="giris-butonu" OnClick="btnGiris_Click" />
                <a href="SifremiUnuttum.aspx" class="sifremi-unuttum-link">Şifremi Unuttum</a>
            </div>
        </div>
    </form>
</body>
</html>

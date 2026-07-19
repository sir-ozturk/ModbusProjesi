<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SifremiUnuttum.aspx.cs" Inherits="ModbusProjesi.Pages.SifremiUnuttum" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MODBUS ŞİFREMİ UNUTTUM PANELİ</title>
    <link href="../Styles/SifremiUnuttum.css" rel="stylesheet" />
    <script src="../Scripts/SifremiUnuttumZamanlayici.js"></script>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/7.0.1/css/all.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server" defaultbutton="btnSifirla">
        <div class="giris-konteynır">
            <div class="logo">
                <asp:Image ID="loginLogo" runat="server" ImageUrl="~/Files/Images/LoginSayfasi_1.png" CssClass="profil-img" />
            </div>
            <div class="baslik">
                <h2>Modbus Şifremi Unuttum Paneli</h2>
            </div>
            <div class="giris-alanlari">
                <asp:TextBox ID="TxtResetMail" runat="server" TextMode="Email" placeholder="Mail"></asp:TextBox>
            </div>
            <div class="giris-alanlari">
                <asp:TextBox ID="TxtResetKullaniciAdi" runat="server" placeholder="Kullanıcı Kodu"></asp:TextBox>
            </div>
            <div class="buton-aksiyonu">
                <asp:Button ID="btnSifirla" runat="server" Text="Sıfırla" CssClass="sifirla-butonu" OnClick="btnSifirla_Click" />
            </div>
            <div class="label-yeni-sifre">
                <asp:Label ID="lblYeniSifre" runat="server"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>

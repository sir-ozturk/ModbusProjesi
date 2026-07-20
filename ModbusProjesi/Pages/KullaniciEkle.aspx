<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="KullaniciEkle.aspx.cs" Inherits="ModbusProjesi.Pages.KullaniciEkle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/KullaniciEkle.css?v=1" rel="stylesheet" />
    <script src="../Scripts/KullaniciEkleZamanlayici.js"></script>
    <script src="../Scripts/KullaniciEkleTelefonMaskeleme.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="form-konteyner">

        <div class="sayfa-ust-alan">
            <div class="baslik">
                <h2>
                    <asp:Literal ID="litSayfaBaslik" runat="server" Text="Yeni Kullanıcı Ekle"></asp:Literal></h2>
                <p>Kullanıcı bilgilerini düzenleyin veya yeni kayıt oluşturun</p>
            </div>
        </div>

        <asp:Panel ID="pnlMesaj" runat="server" Visible="false" CssClass="mesaj-kutusu">
            <asp:Label ID="lblMesaj" runat="server" Text=""></asp:Label>
        </asp:Panel>

        <div class="form-kart">

            <div class="form-grup">
                <label>Ad</label>
                <asp:TextBox ID="txtAd" runat="server" CssClass="form-kontrol"></asp:TextBox>
            </div>

            <div class="form-grup">
                <label>Soyad</label>
                <asp:TextBox ID="txtSoyad" runat="server" CssClass="form-kontrol"></asp:TextBox>
            </div>

            <div class="form-grup">
                <label>Telefon</label>
                <asp:TextBox ID="txtTelefon" runat="server" CssClass="form-kontrol telefon-kontrol-sinifi" placeholder="5XX-XXX-XX-XX" TextMode="SingleLine"></asp:TextBox>
            </div>

            <div class="form-grup">
                <label>E-Mail</label>
                <asp:TextBox ID="txtMail" runat="server" CssClass="form-kontrol" TextMode="Email"></asp:TextBox>
            </div>

            <div class="form-grup">
                <label>Kullanıcı Adı</label>
                <asp:TextBox ID="txtKullaniciAdi" runat="server" CssClass="form-kontrol"></asp:TextBox>
            </div>

            <div class="form-grup">
                <label>Profil Fotoğrafı</label>
                <asp:FileUpload ID="fuProfilResmi" runat="server" CssClass="form-kontrol" />

                <div class="profil-onizleme-alani">
                    <div class="profil-resim-kutusu">
                        <asp:Image ID="imgProfil" runat="server" CssClass="profil-resim-onizleme" Style="display: none;" />
                    </div>
                </div>
            </div>

            <asp:PlaceHolder ID="phYeniKayitNotu" runat="server" Visible="true">
                <div class="form-bilgi-notu">
                    <i class="fa-solid fa-envelope-open-text"></i>Şifre, sistem tarafından otomatik oluşturulacaktır.
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phGuncellemeSifreAlani" runat="server" Visible="false">
                <div class="form-grup">
                    <label>Şifre</label>
                    <asp:TextBox ID="txtSifre" runat="server" CssClass="form-kontrol" placeholder="Yeni şifre belirleyin"></asp:TextBox>
                </div>
            </asp:PlaceHolder>

            <div class="form-ddl">
                <div class="form-grup">
                    <label>Rol</label>
                    <asp:DropDownList ID="ddlRoller" runat="server" CssClass="form-secim"></asp:DropDownList>
                </div>

                <div class="form-grup">
                    <label>Aktiflik Durumu</label>
                    <asp:DropDownList ID="ddlAktiflik" runat="server" CssClass="form-secim">
                        <asp:ListItem>Seçiniz...</asp:ListItem>
                        <asp:ListItem Value="True">Aktif</asp:ListItem>
                        <asp:ListItem Value="False">Aktif Değil</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="form-buton-alani">
                <asp:Button ID="btnKaydet" runat="server" Text="Kaydet" CssClass="btn-kaydet" OnClick="btnKaydet_Click" />
            </div>
        </div>
    </div>
</asp:Content>

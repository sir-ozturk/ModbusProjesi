<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="KullaniciEkle.aspx.cs" Inherits="KullaniciEkle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/KullaniciEkle.css?v=6" rel="stylesheet" />
    <script src="../Scripts/KullaniciEkleZamanlayici.js"></script>
    <script src="../Scripts/KullaniciEkleTelefonMaskeleme.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid px-4">

        <div class="row justify-content-center">

            <div class="col-12 col-md-10 col-lg-8 col-xl-7">

                <div class="text-center mb-4">
                    <h2 class="fw-bold text-modbus">
                        <asp:Literal ID="litSayfaBaslik" runat="server" Text="Yeni Kullanıcı Ekle"></asp:Literal>
                    </h2>

                    <p class="text-danger mb-0">
                        Kullanıcı bilgilerini düzenleyin veya yeni kayıt oluşturun
                    </p>
                </div>

                <div class="card-body p-4">

                    <div class="row mb-3">
                        <div class="col-12 col-md-3 d-flex align-items-center">
                            <label class="form-label fw-bold text-modbus mb-md-0">Ad</label>
                        </div>

                        <div class="col-12 col-md-9">
                            <asp:TextBox ID="txtAd" runat="server" CssClass="form-control form-control-modbus"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-12 col-md-3 d-flex align-items-center">
                            <label class="form-label fw-bold text-modbus mb-md-0">Soyad</label>
                        </div>

                        <div class="col-12 col-md-9">
                            <asp:TextBox ID="txtSoyad" runat="server" CssClass="form-control form-control-modbus"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-12 col-md-3 d-flex align-items-center">
                            <label class="form-label fw-bold text-modbus mb-md-0">Telefon</label>
                        </div>

                        <div class="col-12 col-md-9">
                            <asp:TextBox ID="txtTelefon" runat="server" CssClass="form-control form-control-modbus telefon-kontrol-sinifi" placeholder="5XX-XXX-XX-XX" TextMode="SingleLine"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-12 col-md-3 d-flex align-items-center">
                            <label class="form-label fw-bold text-modbus mb-md-0">E-Mail</label>
                        </div>

                        <div class="col-12 col-md-9">
                            <asp:TextBox ID="txtMail" runat="server" CssClass="form-control form-control-modbus" TextMode="Email"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-12 col-md-3 d-flex align-items-center">
                            <label class="form-label fw-bold text-modbus mb-md-0">Kullanıcı Adı</label>
                        </div>

                        <div class="col-12 col-md-9">
                            <asp:TextBox ID="txtKullaniciAdi" runat="server" ReadOnly="true" CssClass="form-control form-control-modbus"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-12 col-md-3">
                            <label class="form-label fw-bold text-modbus">Profil Fotoğrafı</label>
                        </div>

                        <div class="col-12 col-md-9">
                            <asp:FileUpload ID="fuProfilResmi" runat="server" CssClass="form-control form-control-modbus" />

                            <div class="mt-3">
                                <div class="profil-resim-kutusu">
                                    <asp:Image ID="imgProfil" runat="server" CssClass="profil-resim-onizleme" Style="display: none;" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <asp:PlaceHolder ID="phYeniKayitNotu" runat="server" Visible="true">
                        <div class="alert alert-info d-flex align-items-center gap-2 mb-3">
                            <i class="fa-solid fa-envelope-open-text"></i>
                            <span>Şifre, sistem tarafından otomatik oluşturulacaktır.</span>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phGuncellemeSifreAlani" runat="server" Visible="false">

                        <div class="row mb-3">
                            <div class="col-12 col-md-3 d-flex align-items-center">
                                <label class="form-label fw-bold text-modbus mb-md-0">Şifre</label>
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtSifre" runat="server" CssClass="form-control form-control-modbus" placeholder="Yeni şifre belirleyin"></asp:TextBox>
                            </div>
                        </div>

                    </asp:PlaceHolder>

                    <div class="row g-3 mb-4">

                        <div class="col-12 col-md-6">
                            <label class="form-label fw-bold text-modbus">Rol</label>
                            <asp:DropDownList ID="ddlRoller" runat="server" CssClass="form-select form-select-modbus"></asp:DropDownList>
                        </div>

                        <div class="col-12 col-md-6">
                            <label class="form-label fw-bold text-modbus">Aktiflik Durumu</label>

                            <asp:DropDownList ID="ddlAktiflik" runat="server" CssClass="form-select form-select-modbus">
                                <asp:ListItem>Seçiniz...</asp:ListItem>
                                <asp:ListItem Value="True">Aktif</asp:ListItem>
                                <asp:ListItem Value="False">Pasif</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                    </div>

                    <div class="d-flex justify-content-center">
                        <asp:Button ID="btnKaydet" runat="server" Text="Kaydet" CssClass="btn btn-modbus-kaydet px-5 py-2 fw-bold" OnClick="btnKaydet_Click" />
                    </div>

                </div>

            </div>

        </div>

    </div>

    </div>

</asp:Content>

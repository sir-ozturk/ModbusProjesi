<%@ Page Title="Profil Düzenle" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ProfilDuzenle.aspx.cs" Inherits="ProfilDuzenle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/ProfilDuzenle.css?v=1" rel="stylesheet" />
    <script src="../Scripts/KullaniciEkleTelefonMaskeleme.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid px-4">
        <div class="row justify-content-center">
            <div class="col-12 col-lg-10 col-xl-8">
                <div class="text-center mb-4">
                    <h2 class="fw-bold text-modbus">Profil Düzenle</h2>
                    <p class="text-danger mb-0">Kişisel bilgilerinizi ve hesabınızı güncelleyin</p>
                </div>

                <div class="profil-kart p-4 p-md-5">
                    <div class="row g-4">
                        <div class="col-12 col-md-4 text-center">
                            <div class="profil-resim-kutusu mx-auto">
                                <asp:Image ID="imgProfil" runat="server" CssClass="profil-resim-onizleme" />
                            </div>
                            <label class="form-label fw-bold text-modbus mt-3">Profil Fotoğrafı</label>
                            <asp:FileUpload ID="fuProfilResmi" runat="server" CssClass="form-control form-control-modbus profil-dosya" accept=".jpg,.jpeg,.png" />
                            <small class="text-muted d-block mt-2">JPG, JPEG veya PNG</small>
                        </div>

                        <div class="col-12 col-md-8">
                            <div class="row g-3">
                                <div class="col-12 col-sm-6">
                                    <label class="form-label fw-bold text-modbus">Ad</label>
                                    <asp:TextBox ID="txtAd" runat="server" CssClass="form-control form-control-modbus" MaxLength="50"></asp:TextBox>
                                </div>
                                <div class="col-12 col-sm-6">
                                    <label class="form-label fw-bold text-modbus">Soyad</label>
                                    <asp:TextBox ID="txtSoyad" runat="server" CssClass="form-control form-control-modbus" MaxLength="50"></asp:TextBox>
                                </div>
                                <div class="col-12">
                                    <label class="form-label fw-bold text-modbus">Telefon</label>
                                    <asp:TextBox ID="txtTelefon" runat="server" CssClass="form-control form-control-modbus telefon-kontrol-sinifi" placeholder="5XX-XXX-XX-XX" MaxLength="13"></asp:TextBox>
                                </div>
                                <div class="col-12">
                                    <label class="form-label fw-bold text-modbus">E-Posta</label>
                                    <asp:TextBox ID="txtMail" runat="server" CssClass="form-control form-control-modbus" TextMode="Email" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <hr class="my-4" />

                    <div class="mb-3">
                        <h5 class="fw-bold text-modbus mb-1"><i class="fa-solid fa-lock me-2"></i>Şifre Değiştir</h5>
                        <small class="text-muted">Değiştirmek istemiyorsanız bu alanları boş bırakın.</small>
                    </div>
                    <div class="row g-3">
                        <div class="col-12">
                            <label class="form-label fw-bold text-modbus">Mevcut Şifre</label>
                            <asp:TextBox ID="txtMevcutSifre" runat="server" CssClass="form-control form-control-modbus" TextMode="Password" MaxLength="50" autocomplete="current-password"></asp:TextBox>
                        </div>
                        <div class="col-12 col-md-6">
                            <label class="form-label fw-bold text-modbus">Yeni Şifre</label>
                            <asp:TextBox ID="txtYeniSifre" runat="server" CssClass="form-control form-control-modbus" TextMode="Password" MaxLength="50" autocomplete="new-password"></asp:TextBox>
                        </div>
                        <div class="col-12 col-md-6">
                            <label class="form-label fw-bold text-modbus">Yeni Şifre Tekrar</label>
                            <asp:TextBox ID="txtYeniSifreTekrar" runat="server" CssClass="form-control form-control-modbus" TextMode="Password" MaxLength="50" autocomplete="new-password"></asp:TextBox>
                        </div>
                    </div>

                    <div class="d-flex justify-content-center mt-4">
                        <asp:Button ID="btnKaydet" runat="server" Text="Değişiklikleri Kaydet" CssClass="btn btn-modbus-kaydet px-5 py-2 fw-bold" OnClick="btnKaydet_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

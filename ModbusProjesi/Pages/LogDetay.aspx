<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="LogDetay.aspx.cs" Inherits="LogDetay" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/LogDetay.css?v=1" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid px-4">

        <div class="row justify-content-center">

            <div class="col-12 col-lg-10">

                <div class="text-center mb-4">
                    <h2 class="log-detay-baslik">Log Detayı</h2>
                    <p class="log-detay-aciklama">
                        Seçilen sistem kaydına ait işlem bilgileri
                    </p>
                </div>

                <div class="card shadow-sm border-0">

                    <div class="card-header log-detay-kart-baslik text-center fw-bold py-3">
                        <i class="fa-solid fa-file-lines me-2"></i>
                        Log Bilgileri
                    </div>

                    <div class="card-body p-4">

                        <div class="row mb-3">
                            <div class="col-12 col-md-3 d-flex align-items-center">
                                <label class="form-label fw-bold mb-md-0">Kullanıcı ID</label>
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtKullaniciId" runat="server"
                                    CssClass="form-control log-detay-kontrol"
                                    ReadOnly="true" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3 d-flex align-items-center">
                                <label class="form-label fw-bold mb-md-0">URL</label>
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtUrl" runat="server"
                                    CssClass="form-control log-detay-kontrol"
                                    ReadOnly="true" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3 d-flex align-items-center">
                                <label class="form-label fw-bold mb-md-0">Tablo Adı</label>
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtTabloAdi" runat="server"
                                    CssClass="form-control log-detay-kontrol"
                                    ReadOnly="true" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3 d-flex align-items-center">
                                <label class="form-label fw-bold mb-md-0">İşlem Adı</label>
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtIslemAdi" runat="server"
                                    CssClass="form-control log-detay-kontrol"
                                    ReadOnly="true" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3 d-flex align-items-center">
                                <label class="form-label fw-bold mb-md-0">İşlem Tipi</label>
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtIslemTipi" runat="server"
                                    CssClass="form-control log-detay-kontrol"
                                    ReadOnly="true" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3 d-flex align-items-center">
                                <label class="form-label fw-bold mb-md-0">IP Adresi</label>
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtIpAdresi" runat="server"
                                    CssClass="form-control log-detay-kontrol"
                                    ReadOnly="true" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3 d-flex align-items-center">
                                <label class="form-label fw-bold mb-md-0">İşlem Tarihi</label>
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtIslemTarihi" runat="server"
                                    CssClass="form-control log-detay-kontrol"
                                    ReadOnly="true" />
                            </div>
                        </div>

                        <div class="row mb-4">
                            <div class="col-12 col-md-3">
                                <label class="form-label fw-bold">Detay</label>
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox ID="txtDetay"
                                    runat="server"
                                    CssClass="form-control log-detay-kontrol"
                                    TextMode="MultiLine"
                                    Rows="10"
                                    ReadOnly="true" />
                            </div>
                        </div>

                        <div class="d-flex justify-content-center">
                            <asp:Button
                                ID="btnGeri"
                                runat="server"
                                Text="Geri"
                                CssClass="btn btn-geri px-5"
                                OnClick="btnGeri_Click" />
                        </div>

                    </div>

                </div>

            </div>

        </div>

    </div>

</asp:Content>

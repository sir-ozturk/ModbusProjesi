<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="LogListesi.aspx.cs" Inherits="LogListesi" %>

<%@ Register Src="~/UserControls/ucMyGrid.ascx" TagPrefix="uc" TagName="MyGrid" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/LogListesi.css?v=2" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid px-4">

        <div class="text-center mb-4">
            <h2 class="log-sayfa-baslik">Log Yönetimi</h2>
            <p class="log-sayfa-aciklama">
                Sistem üzerinde gerçekleşen işlemleri görüntüleyin ve filtreleyin
            </p>
        </div>

        <!-- FİLTRELEME -->
        <div class="card shadow-sm border-0 mb-4">

            <div class="card-header log-kart-baslik text-center fw-bold py-3">
                <i class="fa-solid fa-filter me-2"></i>
                Log Filtreleme
            </div>

            <div class="card-body p-4">

                <div class="row g-3">

                    <div class="col-12 col-md-4">
                        <label class="form-label fw-bold">Kullanıcı Adı</label>

                        <asp:TextBox
                            ID="txtKullanici"
                            runat="server"
                            CssClass="form-control log-form-kontrol"
                            placeholder="Kullanıcı adı">
                        </asp:TextBox>
                    </div>

                    <div class="col-12 col-md-4">
                        <label class="form-label fw-bold">Tablo Adı</label>

                        <asp:DropDownList
                            ID="ddlTabloAdi"
                            runat="server"
                            CssClass="form-select log-form-kontrol">
                        </asp:DropDownList>
                    </div>

                    <div class="col-12 col-md-4">
                        <label class="form-label fw-bold">İşlem Adı</label>

                        <asp:DropDownList
                            ID="ddlIslemAdi"
                            runat="server"
                            CssClass="form-select log-form-kontrol">
                        </asp:DropDownList>
                    </div>

                    <div class="col-12 col-md-4">
                        <label class="form-label fw-bold">İşlem Tipi</label>

                        <asp:DropDownList
                            ID="ddlIslemTipi"
                            runat="server"
                            CssClass="form-select log-form-kontrol">

                            <asp:ListItem Text="Tümü" Value="" />
                            <asp:ListItem Text="Ekle" Value="I" />
                            <asp:ListItem Text="Güncelle" Value="U" />
                            <asp:ListItem Text="Sil" Value="D" />
                            <asp:ListItem Text="Sayfa Görüntüleme" Value="S" />
                            <asp:ListItem Text="Hata" Value="H" />

                        </asp:DropDownList>
                    </div>

                    <div class="col-12 col-md-4">
                        <label class="form-label fw-bold">Başlangıç Tarihi</label>
                        <asp:TextBox
                            ID="txtBaslangicTarih"
                            runat="server"
                            TextMode="Date"
                            CssClass="form-control log-form-kontrol">
                        </asp:TextBox>
                    </div>

                    <div class="col-12 col-md-4">
                        <label class="form-label fw-bold">Bitiş Tarihi</label>
                        <asp:TextBox
                            ID="txtBitisTarih"
                            runat="server"
                            TextMode="Date"
                            CssClass="form-control log-form-kontrol">
                        </asp:TextBox>
                    </div>

                </div>

                <div class="d-flex justify-content-end gap-2 mt-4">

                    <asp:Button
                        ID="btnTemizle"
                        runat="server"
                        Text="Temizle"
                        CssClass="btn btn-outline-secondary px-4"
                        OnClick="btnTemizle_Click" />

                    <asp:Button
                        ID="btnFiltrele"
                        runat="server"
                        Text="Filtrele"
                        CssClass="btn btn-filtrele px-4"
                        OnClick="btnFiltrele_Click" />

                </div>

            </div>

        </div>

        <!-- LOG LİSTESİ -->
        <div class="card shadow-sm border-0">

            <div class="card-header log-kart-baslik text-center fw-bold py-3">
                <i class="fa-solid fa-list me-2"></i>
                Modbus Log Listesi
            </div>

            <div class="card-body p-3">

                <uc:MyGrid
                    ID="ucLogGrid"
                    runat="server"
                    OnButonTiklandi="ucLogGrid_ButonTiklandi" />

            </div>

        </div>

    </div>

</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="KullaniciListele.aspx.cs" Inherits="KullaniciListele" %>
<%@ Register Src="~/UserControls/ucMyGrid.ascx" TagPrefix="uc" TagName="MyGrid" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/KullaniciListele.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/7.0.1/css/all.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid px-4">

    <div class="w-100">

        <div class="sayfa-ust-alan">

            <div class="baslik">
                <h2>Kullanıcı Yönetimi</h2>
                <p>Sistemde kayıtlı olan tüm kullanıcıların listesi</p>
            </div>

            <a href="KullaniciEkle.aspx" class="btn-ekle">
                <i class="fa-solid fa-user-plus"></i>
                Yeni Kullanıcı Ekle
            </a>

        </div>

        <uc:MyGrid
            ID="ucMyGrid"
            runat="server"
            OnButonTiklandi="ucMyGrid_ButonTiklandi" />

    </div>

</div>
</asp:Content>

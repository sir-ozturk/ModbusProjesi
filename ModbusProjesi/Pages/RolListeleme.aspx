<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="RolListeleme.aspx.cs" Inherits="RolListeleme" %>

<%@ Register Src="~/UserControls/ucMyGrid.ascx" TagPrefix="uc" TagName="MyGrid" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/RolListeleme.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/7.0.1/css/all.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid px-4">

        <div class="w-100">

            <div class="sayfa-ust-alan">

                <div class="baslik">
                    <h2>Rol Yönetimi</h2>
                    <p>Sistemde tanımlı olan rollerin listesi</p>
                </div>

                <a href="RolEkleme.aspx" class="btn-ekle">
                    <i class="fa-solid fa-plus"></i>
                    Yeni Rol Ekle
                </a>

            </div>

            <uc:MyGrid
                ID="ucRolGrid"
                runat="server"
                OnButonTiklandi="ucRolGrid_ButonTiklandi" />

        </div>

    </div>

</asp:Content>
<%@ Page Title="Röle Kartları" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="RoleKartListele.aspx.cs" Inherits="RoleKartListele" %>
<%@ Register Src="~/UserControls/ucMyGrid.ascx" TagPrefix="uc" TagName="MyGrid" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"><link href="../Styles/MakineListele.css" rel="stylesheet" /></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid px-4">
        <div class="sayfa-ust-alan"><div class="baslik"><h2>Röle Kartları</h2></div><a id="lnkEkle" runat="server" href="RoleKartEkle.aspx" class="btn-ekle">Yeni Kayıt Ekle</a></div>
        <asp:Panel ID="pnlHata" runat="server" Visible="false" CssClass="alert alert-danger"><asp:Label ID="lblHata" runat="server" /></asp:Panel>
        <asp:Panel ID="pnlBasari" runat="server" Visible="false" CssClass="alert alert-success"><asp:Label ID="lblBasari" runat="server" /></asp:Panel>
        <uc:MyGrid ID="ucGrid" runat="server" OnButonTiklandi="ucGrid_ButonTiklandi" />
    </div>
</asp:Content>

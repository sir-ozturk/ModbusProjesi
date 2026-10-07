<%@ Page Title="Ethernet Kartları" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="EthernetKartListele.aspx.cs" Inherits="EthernetKartListele" %>
<%@ Register Src="~/UserControls/ucMyGrid.ascx" TagPrefix="uc" TagName="MyGrid" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"><link href="../Styles/MakineListele.css" rel="stylesheet" /></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid px-4">
        <div class="sayfa-ust-alan"><div class="baslik"><h2>Ethernet Kartları</h2></div><a id="lnkEkle" runat="server" href="EthernetKartEkle.aspx" class="btn-ekle">Yeni Kayıt Ekle</a></div>


        <uc:MyGrid ID="ucGrid" runat="server" OnButonTiklandi="ucGrid_ButonTiklandi" />
    </div>
</asp:Content>

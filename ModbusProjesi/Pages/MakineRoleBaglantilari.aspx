<%@ Page Title="Makine–Röle Bağlantısı" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="MakineRoleBaglantilari.aspx.cs" Inherits="MakineRoleBaglanti" %>
<%@ Register Src="~/UserControls/ucMyGrid.ascx" TagPrefix="uc" TagName="MyGrid" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/MakineEkle.css" rel="stylesheet" />
    <link href="../Styles/MakineListele.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="text-center mb-4"><h2 class="text-modbus fw-bold"><asp:Literal ID="litBaslik" runat="server" /></h2></div>
        <asp:Panel ID="pnlHata" runat="server" Visible="false" CssClass="alert alert-danger"><asp:Label ID="lblHata" runat="server" /></asp:Panel>
        <asp:Panel ID="pnlBasari" runat="server" Visible="false" CssClass="alert alert-success"><asp:Label ID="lblBasari" runat="server" /></asp:Panel>
        <div class="row justify-content-center"><div class="col-12 col-xl-9">
            <div class="row mb-3 align-items-center"><asp:Label runat="server" AssociatedControlID="ddlRole" CssClass="col-md-3 fw-bold text-modbus" Text="Röle kartı" /><div class="col-md-9"><asp:DropDownList ID="ddlRole" runat="server" CssClass="form-select form-select-modbus" AutoPostBack="true" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged"></asp:DropDownList></div></div>
            <div class="row mb-3 align-items-center"><asp:Label runat="server" AssociatedControlID="ddlKanal" CssClass="col-md-3 fw-bold text-modbus" Text="Röle kanalı" /><div class="col-md-9"><asp:DropDownList ID="ddlKanal" runat="server" CssClass="form-select form-select-modbus"></asp:DropDownList></div></div>
            <div class="row mb-3 align-items-center"><asp:Label runat="server" AssociatedControlID="ddlMakine" CssClass="col-md-3 fw-bold text-modbus" Text="Makine" /><div class="col-md-9"><asp:DropDownList ID="ddlMakine" runat="server" CssClass="form-select form-select-modbus"></asp:DropDownList></div></div>
            <p class="text-secondary">Dolu kanallar ve başka bir kanala atanmış makineler seçilemez. Bağlantıyı kaydetmek fiziksel çıkışları tetiklemez.</p>
            <div class="row mb-3 align-items-center"><asp:Label runat="server" AssociatedControlID="ddlAktiflik" CssClass="col-md-3 fw-bold text-modbus" Text="Durum" /><div class="col-md-9"><asp:DropDownList ID="ddlAktiflik" runat="server" CssClass="form-select form-select-modbus"><asp:ListItem Value="1">Aktif</asp:ListItem><asp:ListItem Value="0">Pasif</asp:ListItem></asp:DropDownList></div></div>
            <div class="d-flex gap-2 mb-4"><asp:Button ID="btnKaydet" runat="server" Text="Kaydet" CssClass="btn btn-success" OnClick="btnKaydet_Click" />
                <a href="MakineRoleBaglantilari.aspx" class="btn btn-secondary">Yeni bağlantı / Temizle</a></div>
        </div></div>
        <uc:MyGrid ID="ucGrid" runat="server" OnButonTiklandi="ucGrid_ButonTiklandi" />
    </div>
</asp:Content>

<%@ Page Title="Parametre Listele" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ParametreListele.aspx.cs" Inherits="ParametreListele" %>
<%@ Register Src="~/UserControls/ucMyGrid.ascx" TagPrefix="uc" TagName="MyGrid" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/MakineListele.css" rel="stylesheet" />
    <link href="../Styles/ParametreListele.css?v=2" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlIcerik" runat="server" CssClass="container-fluid px-4 parametre-listele">
        <div class="sayfa-ust-alan">
            <div class="baslik"><h2>Parametre Listele</h2></div>
            <a id="lnkEkle" runat="server" visible="false" href="ParametreEkle.aspx" class="btn-ekle">Yeni Parametre Ekle</a>
        </div>

        <asp:Panel ID="pnlHata" runat="server" Visible="false" CssClass="alert alert-danger">
            <asp:Label ID="lblHata" runat="server" />
        </asp:Panel>

        <asp:Panel ID="pnlBasari" runat="server" Visible="false" CssClass="alert alert-success">
            <asp:Label ID="lblBasari" runat="server" />
        </asp:Panel>

        <asp:Panel ID="pnlFiltreler" runat="server" DefaultButton="btnListele" CssClass="row g-3 mb-4">
            <div class="col-md-4">
                <asp:Label ID="lblGrup" runat="server" AssociatedControlID="ddlGruplar" Text="Parametre Grubu" CssClass="form-label" />
                <asp:DropDownList ID="ddlGruplar" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-2">
                <asp:Label ID="lblDurum" runat="server" AssociatedControlID="ddlDurum" Text="Durum" CssClass="form-label" />
                <asp:DropDownList ID="ddlDurum" runat="server" CssClass="form-select">
                    <asp:ListItem Value="1">Aktif</asp:ListItem>
                    <asp:ListItem Value="0">Pasif</asp:ListItem>
                    <asp:ListItem Value="">Tümü</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <asp:Label ID="lblArama" runat="server" AssociatedControlID="txtArama" Text="Kod veya Ad" CssClass="form-label" />
                <asp:TextBox ID="txtArama" runat="server" MaxLength="150" CssClass="form-control" />
            </div>
            <div class="col-md-2 d-flex align-items-end">
                <asp:Button ID="btnListele" runat="server" Text="Listele" CssClass="btn btn-primary w-100" OnClick="btnListele_Click" />
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlBosListe" runat="server" Visible="false" CssClass="alert alert-info">
            Seçilen filtrelere uygun parametre bulunamadı.
        </asp:Panel>
        <uc:MyGrid ID="ucGrid" runat="server" />
    </asp:Panel>
</asp:Content>

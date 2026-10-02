<%@ Page Title="Parametre" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ParametreEkle.aspx.cs" Inherits="ParametreEkle" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/MakineEkle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlIcerik" runat="server" CssClass="container-fluid" DefaultButton="btnKaydet">
        <div class="text-center mb-4"><h2 class="text-modbus fw-bold"><asp:Literal ID="litBaslik" runat="server" /></h2></div>
        <asp:Panel ID="pnlHata" runat="server" Visible="false" CssClass="alert alert-danger"><asp:Label ID="lblHata" runat="server" /></asp:Panel>
        <div class="row justify-content-center"><div class="col-12 col-xl-9">
            <div class="row mb-3 align-items-center">
                <asp:Label runat="server" AssociatedControlID="ddlGruplar" CssClass="col-md-3 fw-bold text-modbus" Text="Parametre Grubu" />
                <div class="col-md-9"><asp:DropDownList ID="ddlGruplar" runat="server" CssClass="form-select form-select-modbus" /></div>
            </div>
            <div class="row mb-3 align-items-center">
                <asp:Label runat="server" AssociatedControlID="txtKod" CssClass="col-md-3 fw-bold text-modbus" Text="Kod" />
                <div class="col-md-9"><asp:TextBox ID="txtKod" runat="server" MaxLength="50" CssClass="form-control form-control-modbus" /></div>
            </div>
            <div class="row mb-3 align-items-center">
                <asp:Label runat="server" AssociatedControlID="txtAdi" CssClass="col-md-3 fw-bold text-modbus" Text="Ad" />
                <div class="col-md-9"><asp:TextBox ID="txtAdi" runat="server" MaxLength="150" CssClass="form-control form-control-modbus" /></div>
            </div>
            <div class="row mb-3 align-items-center">
                <asp:Label runat="server" AssociatedControlID="txtAciklama" CssClass="col-md-3 fw-bold text-modbus" Text="Tanım Açıklaması" />
                <div class="col-md-9"><asp:TextBox ID="txtAciklama" runat="server" TextMode="MultiLine" Rows="3" MaxLength="500" CssClass="form-control form-control-modbus" /></div>
            </div>
            <div class="row mb-3 align-items-center">
                <asp:Label runat="server" AssociatedControlID="txtSiraNo" CssClass="col-md-3 fw-bold text-modbus" Text="Sıra No" />
                <div class="col-md-9"><asp:TextBox ID="txtSiraNo" runat="server" TextMode="Number" Text="0" CssClass="form-control form-control-modbus" /></div>
            </div>
            <div class="row mb-3 align-items-center">
                <asp:Label runat="server" AssociatedControlID="chkAciklamaZorunlu" CssClass="col-md-3 fw-bold text-modbus" Text="Operatör Açıklaması" />
                <div class="col-md-9"><asp:CheckBox ID="chkAciklamaZorunlu" runat="server" Text=" Zorunlu" /></div>
            </div>
            <div class="row mb-3 align-items-center">
                <asp:Label runat="server" AssociatedControlID="ddlAktiflik" CssClass="col-md-3 fw-bold text-modbus" Text="Durum" />
                <div class="col-md-9"><asp:DropDownList ID="ddlAktiflik" runat="server" CssClass="form-select form-select-modbus"><asp:ListItem Value="1">Aktif</asp:ListItem><asp:ListItem Value="0">Pasif</asp:ListItem></asp:DropDownList></div>
            </div>
            <p class="text-secondary">Tanım açıklaması bu seçeneği açıklar. Operatör açıklaması ise duruş sırasında girilir. Grup ve kod oluşturulduktan sonra değiştirilemez.</p>
            <div class="d-flex gap-2 mb-4">
                <asp:Button ID="btnKaydet" runat="server" Text="Kaydet" CssClass="btn btn-success" OnClick="btnKaydet_Click" />
                <a id="lnkListe" runat="server" href="ParametreListele.aspx" class="btn btn-secondary">Listeye dön</a>
            </div>
        </div></div>
    </asp:Panel>
</asp:Content>

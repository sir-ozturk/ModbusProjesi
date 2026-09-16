<%@ Page Title="Makine Listele" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master"
    AutoEventWireup="true"
    CodeBehind="MakineListele.aspx.cs"
    Inherits="MakineListele" %>

<%@ Register Src="~/UserControls/ucMyGrid.ascx"
    TagPrefix="uc"
    TagName="MyGrid" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <link href="../Styles/MakineListele.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <div class="sayfa-ust-alan">

            <div class="baslik">
                <h2>Makine Yönetimi</h2>
                <p>Sistemde kayıtlı olan makinelerin listesi</p>
            </div>

            <asp:HyperLink
                ID="lnkYeniMakine"
                runat="server"
                NavigateUrl="~/Pages/MakineEkle.aspx"
                CssClass="btn-ekle">

                <i class="fa-solid fa-gears"></i>
                Yeni Makine Ekle

            </asp:HyperLink>

        </div>

        <uc:MyGrid
            ID="ucMyGrid"
            runat="server"
            OnButonTiklandi="ucMyGrid_ButonTiklandi" />

    </div>

</asp:Content>

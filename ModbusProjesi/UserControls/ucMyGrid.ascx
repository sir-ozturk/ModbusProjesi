<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucMyGrid.ascx.cs" Inherits="ucMyGrid" %>

<div class="container-fluid p-0 w-100">

    <div class="d-flex justify-content-between align-items-center flex-wrap gap-3 mb-3">

        <div class="d-flex align-items-center gap-3">

            <asp:Label ID="lblKayitSayisi" runat="server" CssClass="fw-bold px-3 py-2 rounded bg-dark text-white"></asp:Label>

            <div class="d-flex align-items-center gap-2">

                <span class="fw-semibold text-dark fs-6">Göster</span>

                <asp:DropDownList ID="ddlKayitSayisi" runat="server" CssClass="form-select" Style="width: 110px;" AutoPostBack="true" OnSelectedIndexChanged="ddlKayitSayisi_SelectedIndexChanged">
                    <asp:ListItem Value="10" Selected="True">10</asp:ListItem>
                    <asp:ListItem Value="20">20</asp:ListItem>
                    <asp:ListItem Value="50">50</asp:ListItem>
                    <asp:ListItem Value="0">Tümü</asp:ListItem>
                </asp:DropDownList>

            </div>

        </div>

        <asp:TextBox ID="txtArama" runat="server" CssClass="form-control" Style="width: 320px;" placeholder="Ara..." AutoPostBack="true" OnTextChanged="txtArama_TextChanged"></asp:TextBox>

    </div>

    <div class="table-responsive w-100">

        <asp:GridView ID="grdMyGrid"
            runat="server"
            AutoGenerateColumns="false"
            AllowPaging="true"
            AllowSorting="true"
            PageSize="10"
            CssClass="table table-hover align-middle w-100 mb-0"
            GridLines="None"
            PagerSettings-Mode="NumericFirstLast"
            PagerSettings-PageButtonCount="3"
            PagerSettings-FirstPageText="«"
            PagerSettings-LastPageText="»"
            OnPageIndexChanging="grdMyGrid_PageIndexChanging"
            OnSorting="grdMyGrid_Sorting"
            OnRowCommand="grdMyGrid_RowCommand"
            OnRowCreated="grdMyGrid_RowCreated">

            <HeaderStyle BackColor="CadetBlue" ForeColor="DarkBlue" Font-Bold="true" />

            <PagerStyle CssClass="grid-sayfalama" HorizontalAlign="Center" />

        </asp:GridView>

    </div>

</div>

<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="RolYetki.aspx.cs" Inherits="RolYetki" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/RolYetki.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid px-4">

        <div class="row justify-content-center">

            <div class="col-12">

                <div class="card shadow-sm border-0">

                    <div class="card-header rol-yetki-baslik text-center fw-bold py-3">
                        Rol Yetkileri Düzenleme
                    </div>

                    <div class="card-body p-4">

                        <div class="mb-4">
                            <asp:Label
                                ID="lblRolBilgisi"
                                runat="server"
                                CssClass="rol-bilgisi fw-bold">
                            </asp:Label>
                        </div>

                        <div class="table-responsive">

                            <asp:GridView
                                ID="grdYetkiler"
                                runat="server"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover align-middle text-center"
                                GridLines="None">

                                <Columns>

                                    <asp:BoundField
                                        DataField="Ekran"
                                        HeaderText="Sayfa" />

                                    <asp:TemplateField HeaderText="Görüntüleme">
                                        <ItemTemplate>
                                            <asp:CheckBox
                                                ID="chkGoruntuleme"
                                                runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Ekleme">
                                        <ItemTemplate>
                                            <asp:CheckBox
                                                ID="chkEkleme"
                                                runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Güncelleme">
                                        <ItemTemplate>
                                            <asp:CheckBox
                                                ID="chkGuncelleme"
                                                runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Silme">
                                        <ItemTemplate>
                                            <asp:CheckBox
                                                ID="chkSilme"
                                                runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Yazdırma">
                                        <ItemTemplate>
                                            <asp:CheckBox
                                                ID="chkYazdirma"
                                                runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>

                            </asp:GridView>

                        </div>

                        <div class="d-flex justify-content-center gap-2 mt-4">

                            <asp:Button
                                ID="btnKaydet"
                                runat="server"
                                Text="Yetkileri Kaydet"
                                CssClass="btn btn-success px-4"
                                OnClick="btnKaydet_Click" />

                            <asp:Button
                                ID="btnGeri"
                                runat="server"
                                Text="Geri"
                                CssClass="btn btn-dark px-4"
                                OnClick="btnGeri_Click" />

                        </div>

                    </div>

                </div>

            </div>

        </div>

    </div>

</asp:Content>
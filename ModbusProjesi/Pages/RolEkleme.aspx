<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="RolEkleme.aspx.cs" Inherits="RolEkleme" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/RolEkleme.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid px-4">

        <div class="row justify-content-center">

            <div class="col-12 col-lg-9">

                <div class="card shadow-sm border-0">

                    <div class="card-header rol-kart-baslik text-center fw-bold py-3">
                        Rol Yönetimi
                    </div>

                    <div class="card-body p-4">

                        <div class="row mb-3">
                            <div class="col-12 col-md-3 d-flex align-items-center">
                                <label class="form-label fw-bold mb-md-0">Rol Kodu</label>
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtRolKodu"
                                    runat="server"
                                    CssClass="form-control"
                                    ReadOnly="true">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3 d-flex align-items-center">
                                <label class="form-label fw-bold mb-md-0">Rol Adı</label>
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtRolAdi"
                                    runat="server"
                                    CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-12 col-md-3">
                                <label class="form-label fw-bold">Açıklama</label>
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:TextBox
                                    ID="txtAciklama"
                                    runat="server"
                                    CssClass="form-control"
                                    TextMode="MultiLine"
                                    Rows="4">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-4">
                            <div class="col-12 col-md-3 d-flex align-items-center">
                                <label class="form-label fw-bold mb-md-0">Durum</label>
                            </div>

                            <div class="col-12 col-md-9">
                                <asp:DropDownList
                                    ID="ddlAktiflik"
                                    runat="server"
                                    CssClass="form-select">

                                    <asp:ListItem Text="Seçiniz..." Value="" />
                                    <asp:ListItem Text="Aktif" Value="1" />
                                    <asp:ListItem Text="Pasif" Value="0" />

                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="d-flex justify-content-center gap-2">

                            <asp:Button
                                ID="btnKaydet"
                                runat="server"
                                Text="Kaydet"
                                CssClass="btn btn-success px-4"
                                OnClick="btnKaydet_Click" />

                            <asp:Button
                                ID="btnSil"
                                runat="server"
                                Text="Sil"
                                CssClass="btn btn-danger"
                                OnClientClick="var modal = new bootstrap.Modal(document.getElementById('silOnayModal')); modal.show(); return false;" />

                            <asp:Button
                                ID="btnYetkiler"
                                runat="server"
                                Text="Yetkileri Düzenle"
                                CssClass="btn btn-dark px-4"
                                Enabled="false"
                                OnClick="btnYetkiler_Click" />

                        </div>

                    </div>

                </div>

            </div>

        </div>

    </div>

    <div class="modal fade"
        id="silOnayModal"
        tabindex="-1"
        aria-labelledby="silOnayModalBaslik"
        aria-hidden="true">

        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">

                <div class="modal-header">
                    <h5 class="modal-title" id="silOnayModalBaslik">Silme Onayı
                    </h5>

                    <button type="button"
                        class="btn-close"
                        data-bs-dismiss="modal"
                        aria-label="Kapat">
                    </button>
                </div>

                <div class="modal-body">
                    Bu rolü silmek istediğinize emin misiniz?
                </div>

                <div class="modal-footer">

                    <button type="button"
                        class="btn btn-secondary"
                        data-bs-dismiss="modal">
                        İptal
                    </button>

                    <asp:Button
                        ID="btnModalSil"
                        runat="server"
                        Text="Sil"
                        CssClass="btn btn-danger"
                        OnClick="btnSil_Click" />

                </div>

            </div>
        </div>
    </div>

</asp:Content>

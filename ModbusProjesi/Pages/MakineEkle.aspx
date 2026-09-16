<%@ Page Title="Makine Ekle" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master"
    AutoEventWireup="true"
    CodeBehind="MakineEkle.aspx.cs"
    Inherits="MakineEkle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/MakineEkle.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <div class="text-center mb-5">
            <h2 class="text-modbus fw-bold">
                <asp:Literal
                    ID="litSayfaBaslik"
                    runat="server"
                    Text="Yeni Makine Ekle">
                </asp:Literal>
            </h2>

            <p class="text-danger fs-5">
                Makine bilgilerini düzenleyin veya yeni kayıt oluşturun
            </p>
        </div>

        <div class="row justify-content-center">

            <div class="col-12 col-xl-9">

                <!-- MODEL ADI -->
                <div class="row mb-3 align-items-center">
                    <label class="col-12 col-md-3 fw-bold text-modbus">
                        Model Adı
                    </label>

                    <div class="col-12 col-md-9">
                        <asp:TextBox
                            ID="txtModelAd"
                            runat="server"
                            CssClass="form-control form-control-modbus"
                            MaxLength="100">
                        </asp:TextBox>
                    </div>
                </div>

                <!-- ENTEGRASYON KODU -->
                <div class="row mb-3 align-items-center">
                    <label class="col-12 col-md-3 fw-bold text-modbus">
                        Entegrasyon Kodu
                    </label>

                    <div class="col-12 col-md-9">
                        <asp:TextBox
                            ID="txtEntegrasyonKod"
                            runat="server"
                            CssClass="form-control form-control-modbus"
                            MaxLength="100">
                        </asp:TextBox>
                    </div>
                </div>

                <!-- GG NO -->
                <div class="row mb-3 align-items-center">
                    <label class="col-12 col-md-3 fw-bold text-modbus">
                        GG No
                    </label>

                    <div class="col-12 col-md-9">
                        <asp:TextBox
                            ID="txtGgNo"
                            runat="server"
                            CssClass="form-control form-control-modbus"
                            MaxLength="5"
                            oninput="this.value=this.value.replace(/[^0-9]/g,'');">
                        </asp:TextBox>
                    </div>
                </div>

                <!-- MAKİNE NO -->
                <div class="row mb-3 align-items-center">
                    <label class="col-12 col-md-3 fw-bold text-modbus">
                        Makine No
                    </label>

                    <div class="col-12 col-md-9">
                        <asp:TextBox
                            ID="txtMakineNo"
                            runat="server"
                            CssClass="form-control form-control-modbus"
                            MaxLength="5"
                            oninput="this.value=this.value.replace(/[^0-9]/g,'');">
                        </asp:TextBox>
                    </div>
                </div>

                <!-- MAKİNE ADI -->
                <div class="row mb-3 align-items-center">
                    <label class="col-12 col-md-3 fw-bold text-modbus">
                        Makine Adı
                    </label>

                    <div class="col-12 col-md-9">
                        <asp:TextBox
                            ID="txtMakineAdi"
                            runat="server"
                            CssClass="form-control form-control-modbus"
                            MaxLength="100">
                        </asp:TextBox>
                    </div>
                </div>

                <!-- BAND NO -->
                <div class="row mb-3 align-items-center">
                    <label class="col-12 col-md-3 fw-bold text-modbus">
                        Band No
                    </label>

                    <div class="col-12 col-md-9">
                        <asp:TextBox
                            ID="txtBandNo"
                            runat="server"
                            CssClass="form-control form-control-modbus"
                            MaxLength="50">
                        </asp:TextBox>
                    </div>
                </div>

                <!-- IP -->
                <div class="row mb-3 align-items-center">
                    <label class="col-12 col-md-3 fw-bold text-modbus">
                        IP
                    </label>

                    <div class="col-12 col-md-9">
                        <asp:TextBox
                            ID="txtIp"
                            runat="server"
                            CssClass="form-control form-control-modbus"
                            MaxLength="15"
                            oninput="this.value=this.value.replace(/[^0-9.]/g,'');">
                        </asp:TextBox>
                    </div>
                </div>

                <!-- MFG -->
                <div class="row mb-3 align-items-center">
                    <label class="col-12 col-md-3 fw-bold text-modbus">
                        MFG
                    </label>

                    <div class="col-12 col-md-9">
                        <asp:TextBox
                            ID="txtMfg"
                            runat="server"
                            CssClass="form-control form-control-modbus"
                            MaxLength="50"
                            oninput="this.value=this.value.replace(/[^0-9]/g,'');">
                        </asp:TextBox>
                    </div>
                </div>

                <!-- AKTİFLİK -->
                <div class="row mb-4 align-items-center">
                    <label class="col-12 col-md-3 fw-bold text-modbus">
                        Aktiflik Durumu
                    </label>

                    <div class="col-12 col-md-9">
                        <asp:DropDownList
                            ID="ddlAktiflik"
                            runat="server"
                            CssClass="form-select form-select-modbus">

                            <asp:ListItem Text="Seçiniz..." Value="" />
                            <asp:ListItem Text="Aktif" Value="1" />
                            <asp:ListItem Text="Pasif" Value="0" />

                        </asp:DropDownList>
                    </div>
                </div>

                <!-- KAYDET -->
                <div class="d-flex justify-content-center">
                    <asp:Button
                        ID="btnKaydet"
                        runat="server"
                        Text="Kaydet"
                        CssClass="btn btn-modbus-kaydet px-5 fw-bold"
                        OnClick="btnKaydet_Click" />
                </div>

            </div>

        </div>

    </div>

</asp:Content>

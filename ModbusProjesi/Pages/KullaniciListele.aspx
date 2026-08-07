<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="KullaniciListele.aspx.cs" Inherits="ModbusProjesi.Pages.KullaniciListele" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/KullaniciListele.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/7.0.1/css/all.min.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="liste-konteynır">

        <div class="sayfa-ust-alan">
            <div class="baslik">
                <h2>Kullanıcı Yönetimi</h2>
                <p>Sistemde kayıtlı olan tüm kullanıcıların listesi</p>
            </div>
            <a href="KullaniciEkle.aspx" class="btn-ekle">
                <i class="fa-solid fa-user-plus"></i>Yeni Kullanıcı Ekle
            </a>
        </div>

        <table class="tablo">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Kullanıcı Adı</th>
                    <th>Ad Soyad</th>
                    <th>Telefon</th>
                    <th>Mail</th>
                    <th>Rol Adı</th>
                    <th>Durum</th>
                    <th>Güncelle/Sil</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="repeaterKullanicilar" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td class="id"><%# Eval("id") %></td>
                            <td><%# Eval("kullanici_adi") %></td>
                            <td><%# Eval("ad") %> <%# Eval("soyad") %></td>
                            <td><%# TelefonFormatla(Eval("telefon").ToString()) %></td>
                            <td><%# Eval("mail") %></td>
                            <td>
                                <span class="rol-adi"><%# Eval("rol_adi") %></span>
                            </td>
                            <td>
                                <span class='<%# Convert.ToBoolean(Eval("aktif_mi")) == true ? "stat-aktif" : "stat-pasif" %>'>
                                    <%# Convert.ToBoolean(Eval("aktif_mi")) == true ? "Aktif" : "Aktif Değil" %>
                                </span>
                            </td>
                            <td>
                                <div class="islem-butonlari">
                                    <a href='KullaniciEkle.aspx?id=<%# Eval("id") %>' class="btn-tablo-islem btn-guncelle">
                                        <i class="fa-solid fa-pen-to-square"></i>Güncelle
                                    </a>
                                    <asp:LinkButton ID="btnTabloSil" runat="server"
                                        CssClass="btn-tablo-islem btn-sil"
                                        CommandArgument='<%# Eval("id") %>'
                                        OnClick="btnTabloSil_Click">
                        <i class="fa-solid fa-trash-can"></i> Sil
                                    </asp:LinkButton>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>
</asp:Content>

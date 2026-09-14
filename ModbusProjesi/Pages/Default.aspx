<%@ Page Language="C#" Async="true" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/MakineDashboard.css?v=1" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="smDashboard" runat="server" />
    <div class="container-fluid px-0">
        <div class="d-flex flex-wrap align-items-center justify-content-between gap-2 mb-3">
            <div>
                <h1 class="dashboard-baslik mb-1">Makine Kontrol Ekranı</h1>
                <p class="text-secondary mb-0">Aktif makinelerin kayıtlı durumları</p>
            </div>

            <div class="d-flex align-items-center gap-2">
                <button id="btnSiralamaAc" runat="server" type="button" class="btn btn-outline-primary" data-bs-toggle="modal" data-bs-target="#makineSiralamaModal">
                    <i class="fa-solid fa-arrow-down-up-across-line me-1"></i>
                    Sıralamayı Düzenle
                </button>

                <span class="badge dashboard-mod-badge px-3 py-2">
                    <i class="fa-solid fa-flask me-1"></i>
                    Röle 1 Testi
                </span>
            </div>
        </div>

        <asp:UpdatePanel ID="upDurum" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        <asp:Button ID="btnDurumYenile" runat="server" OnClick="btnDurumYenile_Click" CausesValidation="false" Style="display:none" />
        <asp:Label ID="lblDonanimDurumu" runat="server" Visible="false" CssClass="d-block text-secondary mb-2" />
        <asp:Panel ID="pnlHata" runat="server" Visible="false" CssClass="alert alert-danger" role="alert">
            <asp:Label ID="lblHata" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlBasari" runat="server" Visible="false" CssClass="alert alert-success fade show" role="alert">
            <asp:Label ID="lblBasari" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlMakineYok" runat="server" Visible="false" CssClass="alert alert-info" role="alert">
            Gösterilecek aktif makine bulunamadı.
        </asp:Panel>

        <div class="row g-2 makine-grid">
            <asp:Repeater ID="rptMakineler" runat="server">
                <ItemTemplate>
                    <div class="col-12 col-sm-6 col-md-4 col-lg-3 makine-kolon">
                        <article class='<%# MakineKartSinifi(Eval("duruyor_mu")) %>'>
                            <header class="makine-kart-baslik text-center">
                                <%# Server.HtmlEncode(Eval("makine_adi").ToString()) %>
                            </header>

                            <div class="makine-kart-govde">
                                <div class="d-flex align-items-center justify-content-between gap-2 mb-3">
                                    <span class="makine-durum">
                                        <i class='<%# MakineDurumIkonu(Eval("duruyor_mu")) %>'></i>
                                        <%# MakineDurumMetni(Eval("duruyor_mu")) %>
                                    </span>

                                    <span class="makine-sure">
                                        <%# DurusSuresiMetni(Eval("duruyor_mu"), Eval("durus_dakika")) %>
                                    </span>
                                </div>

                                <dl class="makine-bilgiler mb-3">
                                    <div>
                                        <dt>Makine No</dt>
                                        <dd><%# Server.HtmlEncode(Eval("makine_no").ToString()) %></dd>
                                    </div>
                                    <div>
                                        <dt>IP</dt>
                                        <dd><%# Server.HtmlEncode(Eval("ip").ToString()) %></dd>
                                    </div>
                                    <div>
                                        <dt>MFG</dt>
                                        <dd><%# Server.HtmlEncode(Eval("mfg").ToString()) %></dd>
                                    </div>
                                    <div class="makine-neden-satiri">
                                        <dt>Neden</dt>
                                        <dd><%# DurusNedeniMetni(Eval("duruyor_mu"), Eval("islem_nedeni")) %></dd>
                                    </div>
                                </dl>

                                <asp:Button
                                    ID="btnMakineDurdur"
                                    runat="server"
                                    Text="Durdur"
                                    Visible='<%# !Convert.ToBoolean(Eval("duruyor_mu")) %>'
                                    Enabled='<%# MakineRoleKontrolYetkisiVarMi(Eval("makine_no"), Eval("relay_channel")) %>'
                                    OnClientClick='<%# DurdurmaModalAcmaKodu(Eval("id"), Eval("makine_adi"), Eval("makine_no"), Eval("ip")) %>'
                                    UseSubmitBehavior="false"
                                    CssClass="btn btn-danger w-100 fw-bold" />

                                <asp:Button
                                    ID="btnMakineCalistir"
                                    runat="server"
                                    Text="Başlat (Röle 1)"
                                    Visible='<%# Convert.ToBoolean(Eval("duruyor_mu")) %>'
                                    Enabled='<%# MakineRoleKontrolYetkisiVarMi(Eval("makine_no"), Eval("relay_channel")) %>'
                                    CommandArgument='<%# Eval("id") %>'
                                    OnCommand="btnMakineCalistir_Command"
                                    CssClass="btn btn-success w-100 fw-bold" />
                            </div>
                        </article>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        </ContentTemplate>
        </asp:UpdatePanel>
        <asp:HiddenField ID="hdnMakineSiralamasi" runat="server" />
        <asp:HiddenField ID="hdnDurdurMakineId" runat="server" />
        <asp:HiddenField ID="hdnDurusNedeni" runat="server" />

        <div class="modal fade" id="makineDurdurmaModal" tabindex="-1" aria-labelledby="makineDurdurmaBaslik" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content durdurma-modal-icerik">
                    <div class="modal-header">
                        <div>
                            <h2 class="modal-title fs-5" id="makineDurdurmaBaslik">Makine Durdurma Onayı</h2>
                            <div class="text-secondary small">
                                <span id="durdurMakineAdi"></span>
                                <span class="mx-1">—</span>
                                <span id="durdurMakineIp"></span>
                            </div>
                        </div>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Kapat"></button>
                    </div>

                    <div class="modal-body">
                        <div class="alert alert-warning small" role="alert">
                            <i class="fa-solid fa-triangle-exclamation me-1"></i>
                            1 numaralı röleye durdurma komutu gönderilecek ve duruş kaydı oluşturulacaktır.
                        </div>

                        <label class="form-label fw-bold">Duruş Nedeni *</label>

                        <div id="durusNedenleri" class="row g-2 mb-2">
                            <div class="col-6"><button type="button" class="btn btn-outline-secondary w-100 durus-nedeni" data-neden="Mekanik Arıza">Mekanik Arıza</button></div>
                            <div class="col-6"><button type="button" class="btn btn-outline-secondary w-100 durus-nedeni" data-neden="Hatalı Ölçü">Hatalı Ölçü</button></div>
                            <div class="col-6"><button type="button" class="btn btn-outline-secondary w-100 durus-nedeni" data-neden="Yüksek Fire">Yüksek Fire</button></div>
                            <div class="col-6"><button type="button" class="btn btn-outline-secondary w-100 durus-nedeni" data-neden="İplik Kopuşu">İplik Kopuşu</button></div>
                            <div class="col-6"><button type="button" class="btn btn-outline-secondary w-100 durus-nedeni" data-neden="Programlı Bakım">Programlı Bakım</button></div>
                            <div class="col-6"><button type="button" class="btn btn-outline-secondary w-100 durus-nedeni" data-neden="Fazla Adet">Fazla Adet</button></div>
                            <div class="col-6"><button type="button" class="btn btn-outline-secondary w-100 durus-nedeni" data-neden="Operatör Talebi">Operatör Talebi</button></div>
                            <div class="col-6"><button type="button" class="btn btn-outline-secondary w-100 durus-nedeni" data-neden="Fabrika Müdürü Talebi">Fabrika Müdürü Talebi</button></div>
                        </div>

                        <asp:TextBox
                            ID="txtOzelDurusNedeni"
                            runat="server"
                            MaxLength="500"
                            CssClass="form-control"
                            placeholder="Özel neden girin..."></asp:TextBox>
                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">İptal</button>
                        <asp:Button
                            ID="btnDurdurmayiOnayla"
                            runat="server"
                            Text="Durdur"
                            CssClass="btn btn-danger fw-bold"
                            OnClientClick="durusNedeniniHazirla();"
                            OnClick="btnDurdurmayiOnayla_Click" />
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" id="makineSiralamaModal" tabindex="-1" aria-labelledby="makineSiralamaBaslik" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h2 class="modal-title fs-5" id="makineSiralamaBaslik">Makine Sıralamasını Düzenle</h2>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Kapat"></button>
                    </div>

                    <div class="modal-body">
                        <p class="text-secondary mb-3">Makineleri tutup sürükleyerek fiziksel konumlarına göre sıralayın.</p>

                        <div id="makineSiralamaListesi" class="makine-siralama-listesi">
                            <asp:Repeater ID="rptSiralanabilirMakineler" runat="server">
                                <ItemTemplate>
                                    <div class="makine-siralama-ogesi" draggable="true" data-makine-id='<%# Eval("id") %>'>
                                        <i class="fa-solid fa-grip-vertical text-secondary"></i>
                                        <span class="makine-sira-degeri"><%# Eval("sira_no") %></span>
                                        <strong><%# Server.HtmlEncode(Eval("makine_adi").ToString()) %></strong>
                                        <span class="ms-auto text-secondary">Makine No: <%# Server.HtmlEncode(Eval("makine_no").ToString()) %></span>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">İptal</button>
                        <asp:Button
                            ID="btnSiralamayiKaydet"
                            runat="server"
                            Text="Sıralamayı Kaydet"
                            CssClass="btn btn-success"
                            OnClientClick="makineSiralamayiHazirla();"
                            OnClick="btnSiralamayiKaydet_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        (function () {
            var suruklenenOge = null;

            window.makineDurdurmaModaliniAc = function (makineId, makineAdi, makineNo, makineIp) {
                document.getElementById("<%= hdnDurdurMakineId.ClientID %>").value = makineId;
                document.getElementById("<%= hdnDurusNedeni.ClientID %>").value = "";
                document.getElementById("<%= txtOzelDurusNedeni.ClientID %>").value = "";
                document.getElementById("durdurMakineAdi").textContent = makineAdi + " (No: " + makineNo + ")";
                document.getElementById("durdurMakineIp").textContent = makineIp;

                document.querySelectorAll("#durusNedenleri .durus-nedeni").forEach(function (buton) {
                    buton.classList.remove("btn-primary", "active");
                    buton.classList.add("btn-outline-secondary");
                });

                bootstrap.Modal.getOrCreateInstance(document.getElementById("makineDurdurmaModal")).show();
            };

            document.addEventListener("click", function (event) {
                var nedenButonu = event.target.closest("#durusNedenleri .durus-nedeni");

                if (!nedenButonu) {
                    return;
                }

                document.querySelectorAll("#durusNedenleri .durus-nedeni").forEach(function (buton) {
                    buton.classList.remove("btn-primary", "active");
                    buton.classList.add("btn-outline-secondary");
                });

                nedenButonu.classList.remove("btn-outline-secondary");
                nedenButonu.classList.add("btn-primary", "active");
                document.getElementById("<%= hdnDurusNedeni.ClientID %>").value = nedenButonu.getAttribute("data-neden");
                document.getElementById("<%= txtOzelDurusNedeni.ClientID %>").value = "";
            });

            document.getElementById("<%= txtOzelDurusNedeni.ClientID %>").addEventListener("input", function () {
                if (this.value.trim() === "") {
                    return;
                }

                document.querySelectorAll("#durusNedenleri .durus-nedeni").forEach(function (buton) {
                    buton.classList.remove("btn-primary", "active");
                    buton.classList.add("btn-outline-secondary");
                });

                document.getElementById("<%= hdnDurusNedeni.ClientID %>").value = "";
            });

            window.durusNedeniniHazirla = function () {
                var ozelNeden = document.getElementById("<%= txtOzelDurusNedeni.ClientID %>").value.trim();

                if (ozelNeden !== "") {
                    document.getElementById("<%= hdnDurusNedeni.ClientID %>").value = ozelNeden;
                }
            };

            document.addEventListener("dragstart", function (event) {
                var oge = event.target.closest(".makine-siralama-ogesi");

                if (!oge) {
                    return;
                }

                suruklenenOge = oge;
                oge.classList.add("surukleniyor");
            });

            document.addEventListener("dragend", function () {
                if (suruklenenOge) {
                    suruklenenOge.classList.remove("surukleniyor");
                }

                suruklenenOge = null;
                siraNumaralariniYenile();
            });

            document.addEventListener("dragover", function (event) {
                var liste = event.target.closest("#makineSiralamaListesi");

                if (!liste || !suruklenenOge) {
                    return;
                }

                event.preventDefault();

                var hedef = event.target.closest(".makine-siralama-ogesi");

                if (!hedef || hedef === suruklenenOge) {
                    return;
                }

                var kutu = hedef.getBoundingClientRect();
                var ayniSatirda = event.clientY >= kutu.top && event.clientY <= kutu.bottom;
                var hedefSonrasi = ayniSatirda
                    ? event.clientX > kutu.left + kutu.width / 2
                    : event.clientY > kutu.top + kutu.height / 2;

                liste.insertBefore(suruklenenOge, hedefSonrasi ? hedef.nextSibling : hedef);
            });

            function siraNumaralariniYenile() {
                document.querySelectorAll("#makineSiralamaListesi .makine-siralama-ogesi").forEach(function (oge, index) {
                    oge.querySelector(".makine-sira-degeri").innerText = index + 1;
                });
            }

            window.makineSiralamayiHazirla = function () {
                var makineIdleri = [];

                document.querySelectorAll("#makineSiralamaListesi .makine-siralama-ogesi").forEach(function (oge) {
                    makineIdleri.push(oge.getAttribute("data-makine-id"));
                });

                document.getElementById("<%= hdnMakineSiralamasi.ClientID %>").value = makineIdleri.join(",");
            };

            document.addEventListener("DOMContentLoaded", function () {
                var basariPaneli = document.getElementById("<%= pnlBasari.ClientID %>");

                if (!basariPaneli) {
                    return;
                }

                window.setTimeout(function () {
                    basariPaneli.classList.remove("show");

                    window.setTimeout(function () {
                        basariPaneli.style.display = "none";
                    }, 150);
                }, 5000);
            });

            window.setTimeout(function sayfayiYenile() {
                if (!document.querySelector(".modal.show") && !document.hidden &&
                    !Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack()) {
                    document.getElementById("<%= btnDurumYenile.ClientID %>").click();
                }
                window.setTimeout(sayfayiYenile, 5000);
            }, 5000);
        })();
    </script>
</asp:Content>

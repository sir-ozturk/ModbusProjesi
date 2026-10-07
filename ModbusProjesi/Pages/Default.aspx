<%@ Page Title="Makine Kontrol" Language="C#" Async="true" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Styles/MakineDashboard.css?v=4" rel="stylesheet" />
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
            </div>
        </div>

        <asp:UpdatePanel ID="upDurum" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Button ID="btnDurumYenile" runat="server" OnClick="btnDurumYenile_Click" CausesValidation="false" Style="display: none" />
                <asp:Label ID="lblDonanimDurumu" runat="server" Visible="false" CssClass="d-block text-secondary mb-2" />
                <asp:Panel ID="pnlMakineYok" runat="server" Visible="false" CssClass="alert alert-info" role="alert">
                    Gösterilecek aktif makine bulunamadı.
                </asp:Panel>

                <div class="row g-2 makine-grid">
                    <asp:Repeater ID="rptMakineler" runat="server">
                        <ItemTemplate>
                            <div class="col-12 col-sm-6 col-md-4 col-lg-3 makine-kolon">
                                <article class='<%# MakineKartSinifi(Eval("duruyor_mu")) %>' data-makine-id='<%# Convert.ToInt32(Eval("id")) %>'>
                                    <header class="makine-kart-baslik"><span class="makine-adi">
                                        <%# Server.HtmlEncode(Eval("makine_adi").ToString()) %>
                                    </span>
                                        <button type="button" class="makine-bilgi-butonu" aria-label="Makine bilgilerini göster" title="Makine bilgileri" data-bs-toggle="modal" data-bs-target="#makineBilgiModal">
                                            <i class="fa-solid fa-info" aria-hidden="true"></i>
                                        </button>
                                    </header>

                                    <div class="makine-kart-govde">
                                        <div class="d-flex align-items-center justify-content-between gap-2 mb-3">
                                            <span class="makine-durum">
                                                <%# MakineDurumMetni(Eval("duruyor_mu")) %>
                                            </span>

                                            <span class="makine-sure">
                                                <%# DurusSuresiMetni(Eval("duruyor_mu"), Eval("durus_dakika")) %>
                                            </span>
                                        </div>

                                        <div class="makine-detay-kaynagi" hidden><dl class="makine-bilgiler mb-3">
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
                                            <div>
                                                <dt>Röle bağlantısı</dt>
                                                <dd><%# RoleBaglantiMetni(Eval("role_bagli_mi"), Eval("role_adi"), Eval("kanal_no")) %></dd>
                                            </div>
                                        </dl>

                                        <div class="small mb-3" aria-live="polite">
                                            <div class="fw-bold">
                                                <%# Server.HtmlEncode(Convert.ToString(Eval("talimat_durum_metni"))) %>
                                            </div>

                                            <div class="text-secondary"><%# Server.HtmlEncode(Convert.ToString(Eval("talimat_sonuc_metni")).Trim()) %></div>
                                        </div>

                                        </div>
                                        <asp:Button
                                            ID="btnMakineDurdur"
                                            runat="server"
                                            Text="Durdur"
                                            Visible='<%# !Convert.ToBoolean(Eval("duruyor_mu")) %>'
                                            Enabled='<%# MakineRoleKontrolYetkisiVarMi(Eval("role_bagli_mi")) && !Convert.ToBoolean(Eval("talimat_devam_ediyor_mu")) %>'
                                            OnClientClick='<%# DurdurmaModalAcmaKodu(Eval("id"), Eval("makine_adi"), Eval("makine_no"), Eval("role_ip")) %>'
                                            UseSubmitBehavior="false"
                                            CssClass="btn btn-danger w-100 fw-bold" />
                                    </div>
                                </article>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <asp:Panel ID="pnlTakip" runat="server" Visible="false" CssClass="makine-takip mt-4">
                    <h2 class="fs-5 fw-bold mb-3">Duruş ve talimat takibi</h2>
                    <div class="takip-ozet mb-3">
                        <span>Açık duruş <asp:Label ID="lblAcikDurus" runat="server" CssClass="fw-bold" /></span>
                        <span>Bekleyen / işlenen talimat <asp:Label ID="lblBekleyenTalimat" runat="server" CssClass="fw-bold" /></span>
                        <span>Kontrol gereken talimat <asp:Label ID="lblKontrolTalimat" runat="server" CssClass="fw-bold" /></span>
                    </div>
                    <asp:Repeater ID="rptTakip" runat="server">
                        <ItemTemplate>
                            <button type="button" class="takip-satir" data-takip-makine-id='<%# Eval("id") %>' data-bs-toggle="modal" data-bs-target="#makineBilgiModal" title="Makine bilgilerini göster">
                                <span class="fw-bold"><%# Server.HtmlEncode(Convert.ToString(Eval("makine"))) %></span>
                                <span class='takip-durum <%# Eval("sinif") %>'><%# Server.HtmlEncode(Convert.ToString(Eval("durum"))) %></span>
                                <span class="takip-aciklama"><%# Server.HtmlEncode(Convert.ToString(Eval("aciklama"))) %></span>
                            </button>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="pnlTakipBos" runat="server" CssClass="text-secondary small py-2">Açık duruş, bekleyen veya kontrol gerektiren işlem yok.</asp:Panel>
                    <asp:Label ID="lblTakipBilgi" runat="server" CssClass="d-block text-secondary small mt-2" />
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="modal fade" id="makineBilgiModal" tabindex="-1" aria-labelledby="makineBilgiBaslik" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h2 class="modal-title fs-5" id="makineBilgiBaslik">Makine bilgileri</h2>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Kapat"></button>
                    </div>
                    <div class="modal-body makine-detaylar" id="makineBilgiIcerik"></div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </div>
            </div>
        </div>
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
                            Durdurma talimatı oluşturulacaktır. İşleyici uygulama çalışırken talimatı uygulayacak ve sonucu bu ekranda gösterecektir.
                        </div>

                        <label class="form-label fw-bold">Duruş Nedeni *</label>

                        <div id="durusNedenleri" class="row g-2 mb-2">
                            <asp:Repeater ID="rptDurusNedenleri" runat="server">
                                <ItemTemplate>
                                    <div class="col-6">
                                        <button type="button" class="btn btn-outline-secondary w-100 durus-nedeni" data-neden='<%# Convert.ToInt32(Eval("id")) %>' data-aciklama-zorunlu='<%# AciklamaZorunlulukDegeri(Eval("aciklama_zorunlu_mu")) %>'><%# Server.HtmlEncode(Convert.ToString(Eval("adi"))) %></button>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <asp:Label ID="lblDurusNedeniBilgi" runat="server" CssClass="text-danger d-block" />
                        <label id="durusAciklamaEtiketi" class="form-label">Açıklama (isteğe bağlı)</label>
                        <asp:TextBox
                            ID="txtOzelDurusNedeni"
                            runat="server"
                            MaxLength="500"
                            CssClass="form-control"
                            placeholder="Duruş açıklaması girin..."></asp:TextBox>
                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">İptal</button>
                        <asp:Button
                            ID="btnDurdurmayiOnayla"
                            runat="server"
                            Text="Durdur"
                            CssClass="btn btn-danger fw-bold"
                            OnClientClick="return durusNedeniniHazirla();"
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
            document.getElementById("makineBilgiModal").addEventListener("show.bs.modal", function (event) {
                var kart = event.relatedTarget && event.relatedTarget.closest(".makine-kart");
                if (!kart && event.relatedTarget) {
                    var makineId = event.relatedTarget.getAttribute("data-takip-makine-id");
                    if (makineId && /^\d+$/.test(makineId)) {
                        kart = document.querySelector('.makine-kart[data-makine-id="' + makineId + '"]');
                    }
                }
                if (!kart) {
                    return;
                }
                document.getElementById("makineBilgiBaslik").textContent = kart.querySelector(".makine-adi").textContent.trim() + " — Makine bilgileri";
                document.getElementById("makineBilgiIcerik").innerHTML = kart.querySelector(".makine-detay-kaynagi").innerHTML;
            });


            window.makineDurdurmaModaliniAc = function (makineId, makineAdi, makineNo, makineIp) {
                document.getElementById("<%= hdnDurdurMakineId.ClientID %>").value = makineId;
                document.getElementById("<%= hdnDurusNedeni.ClientID %>").value = "";
                document.getElementById("<%= txtOzelDurusNedeni.ClientID %>").value = "";
                document.getElementById("durusAciklamaEtiketi").textContent = "Açıklama (isteğe bağlı)";
                document.getElementById("durdurMakineAdi").textContent = makineAdi + " (No: " + makineNo + ")";
                document.getElementById("durdurMakineIp").textContent = makineIp;

                document.querySelectorAll("#durusNedenleri .durus-nedeni").forEach(function (buton) {
                    buton.classList.remove("btn-primary", "active");
                    buton.classList.add("btn-outline-secondary");
                });

                bootstrap.Modal.getOrCreateInstance(document.getElementById("makineDurdurmaModal")).show();
            };

            window.durusSeciminiGeriYukle = function () {
                var makineId = document.getElementById("<%= hdnDurdurMakineId.ClientID %>").value;
                var kart = Array.from(document.querySelectorAll("[data-makine-id]")).find(function (oge) {
                    return oge.getAttribute("data-makine-id") === makineId;
                });
                if (!kart) {
                    return;
                }
                document.getElementById("durdurMakineAdi").textContent = kart.querySelector(".makine-adi").textContent.trim();
                document.getElementById("durdurMakineIp").textContent = "";
                var secilenId = document.getElementById("<%= hdnDurusNedeni.ClientID %>").value;
                document.querySelectorAll("#durusNedenleri .durus-nedeni").forEach(function (buton) {
                    if (buton.getAttribute("data-neden") === secilenId) {
                        buton.classList.remove("btn-outline-secondary");
                        buton.classList.add("btn-primary", "active");
                        if (buton.getAttribute("data-aciklama-zorunlu") === "1") {
                            document.getElementById("durusAciklamaEtiketi").textContent = "Açıklama *";
                        } else {
                            document.getElementById("durusAciklamaEtiketi").textContent = "Açıklama (isteğe bağlı)";
                        }
                    }
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
                if (nedenButonu.getAttribute("data-aciklama-zorunlu") === "1") {
                    document.getElementById("durusAciklamaEtiketi").textContent = "Açıklama *";
                } else {
                    document.getElementById("durusAciklamaEtiketi").textContent = "Açıklama (isteğe bağlı)";
                }
            });

            window.durusNedeniniHazirla = function () {
                var secili = document.querySelector("#durusNedenleri .durus-nedeni.active");
                if (!secili) {
                    window.modbusMesajiGoster("Duruş nedeni seçiniz.", "WARNING");
                    return false;
                }
                if (secili.getAttribute("data-aciklama-zorunlu") === "1"
                    && document.getElementById("<%= txtOzelDurusNedeni.ClientID %>").value.trim() === "") {
                    window.modbusMesajiGoster("Seçilen duruş nedeni için açıklama giriniz.", "WARNING");
                    return false;
                }
                return true;
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
                var hedefSonrasi;
                if (ayniSatirda) {
                    hedefSonrasi = event.clientX > kutu.left + kutu.width / 2;
                } else {
                    hedefSonrasi = event.clientY > kutu.top + kutu.height / 2;
                }

                if (hedefSonrasi) {
                    liste.insertBefore(suruklenenOge, hedef.nextSibling);
                } else {
                    liste.insertBefore(suruklenenOge, hedef);
                }
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

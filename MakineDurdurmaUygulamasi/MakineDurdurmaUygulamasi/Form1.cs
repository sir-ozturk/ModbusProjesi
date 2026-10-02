using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MakineDurdurmaUygulamasi
{
    public partial class Form1 : Form
    {
        private int animasyonAdimi = 0;
        private TalimatIsleyici talimatIsleyici;
        private CancellationTokenSource durdurmaIstegi;
        private Task isleyiciGorevi;
        private bool kapaniyor;
        public Form1()
        {
            InitializeComponent();
            pnlGosterge.Paint += pnlGosterge_Paint;
            tmrAnimasyon.Tick += tmrAnimasyon_Tick;
            btnRun.Click += btnRun_Click;
            btnExit.Click += btnExit_Click;
            FormClosing += Form1_FormClosing;
        }

        private void tmrAnimasyon_Tick(object sender, EventArgs e)
        {
            animasyonAdimi = (animasyonAdimi + 1) % 24;
            pnlGosterge.Invalidate();
        }

        private void pnlGosterge_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            float merkezX = pnlGosterge.ClientSize.Width / 2f;
            float merkezY = pnlGosterge.ClientSize.Height / 2f;
            for (int i = 0; i < 24; i++)
            {
                double aci = (i * 15 - 90) * Math.PI / 180;
                int uzaklik = (animasyonAdimi - i + 24) % 24;
                int gri;
                if (tmrAnimasyon.Enabled)
                {
                    gri = 65 + uzaklik * 7;
                }
                else
                {
                    gri = 180;
                }

                using (Pen kalem = new Pen(Color.FromArgb(gri, gri, gri), 5f))
                {
                    kalem.StartCap = LineCap.Round;
                    kalem.EndCap = LineCap.Round;
                    e.Graphics.DrawLine(kalem, merkezX + (float)Math.Cos(aci) * 76, merkezY + (float)Math.Sin(aci) * 76, merkezX + (float)Math.Cos(aci) * 96, merkezY + (float)Math.Sin(aci) * 96);
                }
            }
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            if (isleyiciGorevi != null && !isleyiciGorevi.IsCompleted)
            {
                btnRun.Enabled = false;
                lblDurum.Text = "Devam eden işlem tamamlanıyor...";
                durdurmaIstegi.Cancel();
                return;
            }

            try
            {
                string baglantiMetni;
                ConnectionStringSettings baglantiAyari = ConfigurationManager.ConnectionStrings["ModbusDb"];
                if (baglantiAyari != null)
                {
                    baglantiMetni = baglantiAyari.ConnectionString;
                }
                else
                {
                    baglantiMetni = null;
                }
                if (string.IsNullOrWhiteSpace(baglantiMetni))
                {
                    throw new InvalidOperationException("ModbusDb bağlantı ayarı bulunamadı.");
                }

                int kullaniciId;
                int gecerlilikSaniye;
                if (!int.TryParse(ConfigurationManager.AppSettings["TalimatIsleyiciKullaniciId"], out kullaniciId) || kullaniciId <= 0)
                {
                    throw new InvalidOperationException("İşleyici kullanıcı ID ayarı geçersiz.");
                }

                if (!int.TryParse(ConfigurationManager.AppSettings["TalimatGecerlilikSaniye"], out gecerlilikSaniye)
                    || gecerlilikSaniye < 1
                    || gecerlilikSaniye > 86400)
                {
                    throw new InvalidOperationException("Talimat geçerlilik süresi geçersiz.");
                }

                string ip = YerelIpGetir();
                durdurmaIstegi = new CancellationTokenSource();
                talimatIsleyici = new TalimatIsleyici(baglantiMetni, kullaniciId, ip, gecerlilikSaniye);
                var durumBildirimi = new Progress<string>(mesaj =>
                {
                    if (!kapaniyor && isleyiciGorevi != null && !isleyiciGorevi.IsCompleted && durdurmaIstegi != null && !durdurmaIstegi.IsCancellationRequested)
                    {
                        lblDurum.Text = mesaj;
                    }
                });
                btnRun.Text = "STOP";
                btnRun.BackColor = Color.Khaki;
                lblDurum.Text = "Bağlantı kuruluyor...";
                tmrAnimasyon.Start();
                isleyiciGorevi = talimatIsleyici.CalistirAsync(durdurmaIstegi.Token, durumBildirimi);
                await isleyiciGorevi;
                lblDurum.Text = "Durduruldu";
            }
            catch (Exception ex)
            {
                lblDurum.Text = "İşleyici durduruldu";
                MessageBox.Show(this, ex.Message, "İşlem bilgisi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                tmrAnimasyon.Stop();
                pnlGosterge.Invalidate();
                btnRun.Text = "RUN";
                btnRun.BackColor = Color.PaleGreen;
                btnRun.Enabled = !kapaniyor;
                if (durdurmaIstegi != null)
                {
                    durdurmaIstegi.Dispose();
                    durdurmaIstegi = null;
                }

                talimatIsleyici = null;
            }
        }

        private string YerelIpGetir()
        {
            foreach (IPAddress adres in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (adres.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(adres))
                {
                    return adres.ToString();
                }
            }

            throw new InvalidOperationException("Bilgisayarın IPv4 adresi bulunamadı.");
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isleyiciGorevi == null || isleyiciGorevi.IsCompleted)
            {
                return;
            }

            e.Cancel = true;
            if (kapaniyor)
            {
                return;
            }

            kapaniyor = true;
            btnRun.Enabled = false;
            btnExit.Enabled = false;
            lblDurum.Text = "İşlem tamamlanınca uygulama kapanacak...";
            durdurmaIstegi.Cancel();
            try
            {
                await isleyiciGorevi;
            }
            catch
            {
            // Hata, RUN olayında kullanıcıya gösterilir.
            }

            // Diğer bekleyen arayüz işlemlerinden sonra kapat.
            BeginInvoke(new Action(Close));
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

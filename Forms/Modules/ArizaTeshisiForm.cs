using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;
using TeknikServisOtomasyon.Helpers;

namespace TeknikServisOtomasyon.Forms.Modules
{
    public partial class ArizaTeshisiForm : XtraForm
    {
        private readonly LlmHelper.TeşhisResponse _teshisResponse;

        public ArizaTeshisiForm(LlmHelper.TeşhisResponse teshisResponse)
        {
            _teshisResponse = teshisResponse;
            InitializeComponent();
            LoadContent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "🤖 Otomatik Arıza Teşhisi";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Icon = SystemIcons.Information;

            // Scroll panel
            var scrollPanel = new XtraScrollableControl();
            scrollPanel.Dock = DockStyle.Fill;
            scrollPanel.Padding = new Padding(20);

            int y = 10;

            // Başlık
            if (string.IsNullOrEmpty(_teshisResponse.HataMesaji))
            {
                var lblBaslik = new LabelControl
                {
                    Text = "✅ Arıza Teşhisi Tamamlandı",
                    Location = new Point(20, y),
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    ForeColor = AppColors.Success
                };
                scrollPanel.Controls.Add(lblBaslik);
            }
            else
            {
                var lblBaslik = new LabelControl
                {
                    Text = "❌ Teşhis Yapılamadı",
                    Location = new Point(20, y),
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    ForeColor = Color.Red
                };
                scrollPanel.Controls.Add(lblBaslik);
            }

            y += 40;

            // Olası Sorunlar
            if (_teshisResponse.OlasıSorunlar != null && _teshisResponse.OlasıSorunlar.Count > 0)
            {
                var grpSorunlar = new GroupControl();
                grpSorunlar.Text = "🔍 Olası Sorunlar";
                grpSorunlar.Location = new Point(20, y);
                grpSorunlar.Size = new Size(840, 150);
                grpSorunlar.Appearance.BackColor = Color.FromArgb(255, 250, 240);

                int sorunY = 30;
                foreach (var sorun in _teshisResponse.OlasıSorunlar)
                {
                    var lblSorun = new LabelControl
                    {
                        Text = $"• {sorun}",
                        Location = new Point(20, sorunY),
                        Size = new Size(800, 30),
                        AutoSizeMode = LabelAutoSizeMode.Vertical,
                        Appearance = { ForeColor = Color.FromArgb(255, 87, 34) }
                    };
                    grpSorunlar.Controls.Add(lblSorun);
                    sorunY += 35;
                }

                scrollPanel.Controls.Add(grpSorunlar);
                y += 170;
            }

            // Çözüm Önerileri
            if (_teshisResponse.ÇözümÖnerileri != null && _teshisResponse.ÇözümÖnerileri.Count > 0)
            {
                var grpCozumler = new GroupControl();
                grpCozumler.Text = "💡 Önerilen Çözüm Adımları";
                grpCozumler.Location = new Point(20, y);
                grpCozumler.Size = new Size(840, 200);
                grpCozumler.Appearance.BackColor = Color.FromArgb(240, 255, 240);

                int cozumY = 30;
                int adimNo = 1;
                foreach (var cozum in _teshisResponse.ÇözümÖnerileri)
                {
                    var lblCozum = new LabelControl
                    {
                        Text = $"{adimNo}. {cozum}",
                        Location = new Point(20, cozumY),
                        Size = new Size(800, 35),
                        AutoSizeMode = LabelAutoSizeMode.Vertical,
                        Appearance = { ForeColor = AppColors.Success, Font = new Font("Segoe UI", 9) }
                    };
                    grpCozumler.Controls.Add(lblCozum);
                    cozumY += 40;
                    adimNo++;
                }

                scrollPanel.Controls.Add(grpCozumler);
                y += 220;
            }

            // Kontrol Noktaları
            if (_teshisResponse.Kontrol != null && _teshisResponse.Kontrol.Count > 0)
            {
                var grpKontrol = new GroupControl();
                grpKontrol.Text = "✓ Kontrol Noktaları";
                grpKontrol.Location = new Point(20, y);
                grpKontrol.Size = new Size(840, 150);
                grpKontrol.Appearance.BackColor = Color.FromArgb(240, 248, 255);

                int kontrolY = 30;
                foreach (var kontrol in _teshisResponse.Kontrol)
                {
                    var lblKontrol = new LabelControl
                    {
                        Text = $"□ {kontrol}",
                        Location = new Point(20, kontrolY),
                        Size = new Size(800, 30),
                        AutoSizeMode = LabelAutoSizeMode.Vertical,
                        Appearance = { ForeColor = Color.FromArgb(33, 150, 243) }
                    };
                    grpKontrol.Controls.Add(lblKontrol);
                    kontrolY += 35;
                }

                scrollPanel.Controls.Add(grpKontrol);
                y += 170;
            }

            // Uyarı
            if (!string.IsNullOrEmpty(_teshisResponse.Uyarı))
            {
                var grpUyari = new GroupControl();
                grpUyari.Text = "⚠️ Güvenlik Uyarısı";
                grpUyari.Location = new Point(20, y);
                grpUyari.Size = new Size(840, 80);
                grpUyari.Appearance.BackColor = Color.FromArgb(255, 243, 224);

                var lblUyari = new LabelControl
                {
                    Text = _teshisResponse.Uyarı,
                    Location = new Point(20, 30),
                    Size = new Size(800, 40),
                    AutoSizeMode = LabelAutoSizeMode.Vertical,
                    Appearance = { ForeColor = Color.FromArgb(255, 152, 0) }
                };
                grpUyari.Controls.Add(lblUyari);

                scrollPanel.Controls.Add(grpUyari);
                y += 100;
            }

            // Hata Mesajı
            if (!string.IsNullOrEmpty(_teshisResponse.HataMesaji))
            {
                var grpHata = new GroupControl();
                grpHata.Text = "Hata Bilgisi";
                grpHata.Location = new Point(20, y);
                grpHata.Size = new Size(840, 100);
                grpHata.Appearance.BackColor = Color.FromArgb(255, 240, 240);

                var lblHata = new LabelControl
                {
                    Text = _teshisResponse.HataMesaji,
                    Location = new Point(20, 30),
                    Size = new Size(800, 60),
                    AutoSizeMode = LabelAutoSizeMode.Vertical,
                    Appearance = { ForeColor = Color.Red }
                };
                grpHata.Controls.Add(lblHata);

                scrollPanel.Controls.Add(grpHata);
                y += 120;
            }

            // Butonlar
            var btnKapat = new SimpleButton
            {
                Text = "Kapat",
                Location = new Point(750, y + 20),
                Size = new Size(100, 35),
                DialogResult = DialogResult.OK
            };
            scrollPanel.Controls.Add(btnKapat);

            var btnKopyala = new SimpleButton
            {
                Text = "📋 Kopyala",
                Location = new Point(630, y + 20),
                Size = new Size(100, 35),
                Appearance = { BackColor = AppColors.Info, ForeColor = Color.White }
            };
            btnKopyala.Click += BtnKopyala_Click;
            scrollPanel.Controls.Add(btnKopyala);

            this.Controls.Add(scrollPanel);
            this.ResumeLayout(false);
        }

        private void LoadContent()
        {
            // İçerik InitializeComponent'te yükleniyor
        }

        private void BtnKopyala_Click(object? sender, EventArgs? e)
        {
            var text = GenerateText();
            Clipboard.SetText(text);
            XtraMessageBox.Show("Teşhis sonuçları panoya kopyalandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GenerateText()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("=== OTOMATIK ARIZA TEŞHİSİ SONUÇLARI ===\n");

            if (_teshisResponse.OlasıSorunlar != null && _teshisResponse.OlasıSorunlar.Count > 0)
            {
                sb.AppendLine("OLASI SORUNLAR:");
                foreach (var sorun in _teshisResponse.OlasıSorunlar)
                {
                    sb.AppendLine($"• {sorun}");
                }
                sb.AppendLine();
            }

            if (_teshisResponse.ÇözümÖnerileri != null && _teshisResponse.ÇözümÖnerileri.Count > 0)
            {
                sb.AppendLine("ÖNERILEN ÇÖZÜM ADAMLARI:");
                int no = 1;
                foreach (var cozum in _teshisResponse.ÇözümÖnerileri)
                {
                    sb.AppendLine($"{no}. {cozum}");
                    no++;
                }
                sb.AppendLine();
            }

            if (_teshisResponse.Kontrol != null && _teshisResponse.Kontrol.Count > 0)
            {
                sb.AppendLine("KONTROL NOKTALARI:");
                foreach (var kontrol in _teshisResponse.Kontrol)
                {
                    sb.AppendLine($"□ {kontrol}");
                }
                sb.AppendLine();
            }

            if (!string.IsNullOrEmpty(_teshisResponse.Uyarı))
            {
                sb.AppendLine($"UYARI: {_teshisResponse.Uyarı}");
            }

            return sb.ToString();
        }
    }
}

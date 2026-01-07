using DevExpress.XtraEditors;
using DevExpress.XtraCharts;
using System;
using System.Drawing;
using System.Windows.Forms;
using TeknikServisOtomasyon.Data.Repositories;
using TeknikServisOtomasyon.Helpers;

namespace TeknikServisOtomasyon.Forms.Modules
{
    public partial class RaporlarForm : XtraForm
    {
        private readonly ServisKaydiRepository _servisRepository;

        public RaporlarForm()
        {
            _servisRepository = new ServisKaydiRepository();
            InitializeComponent();
            LoadReportData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "Raporlar";
            this.Size = new Size(1000, 600);

            var panelMain = new PanelControl();
            panelMain.Dock = DockStyle.Fill;
            panelMain.Padding = new Padding(20);

            // Başlık
            var lblBaslik = new LabelControl();
            lblBaslik.Text = "Servis İstatistikleri";
            lblBaslik.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblBaslik.ForeColor = AppColors.Primary;
            lblBaslik.Location = new Point(20, 10);
            panelMain.Controls.Add(lblBaslik);

            // İstatistik Kartları
            int cardY = 60;
            int cardWidth = 200;
            int cardHeight = 100;

            // Toplam Servis
            var cardToplam = CreateStatCard("Toplam Servis", "0", AppColors.Primary, 20, cardY, cardWidth, cardHeight);
            cardToplam.Name = "cardToplam";
            panelMain.Controls.Add(cardToplam);

            // Beklemede
            var cardBeklemede = CreateStatCard("Beklemede", "0", AppColors.Beklemede, 240, cardY, cardWidth, cardHeight);
            cardBeklemede.Name = "cardBeklemede";
            panelMain.Controls.Add(cardBeklemede);

            // İşlemde
            var cardIslemde = CreateStatCard("İşlemde", "0", AppColors.Islemde, 460, cardY, cardWidth, cardHeight);
            cardIslemde.Name = "cardIslemde";
            panelMain.Controls.Add(cardIslemde);

            // Tamamlandı
            var cardTamamlandi = CreateStatCard("Tamamlandı", "0", AppColors.Tamamlandi, 680, cardY, cardWidth, cardHeight);
            cardTamamlandi.Name = "cardTamamlandi";
            panelMain.Controls.Add(cardTamamlandi);

            // Pie Chart - Durum Dağılımı
            var chartDurum = new ChartControl();
            chartDurum.Name = "chartDurum";
            chartDurum.Location = new Point(20, 180);
            chartDurum.Size = new Size(450, 350);

            var pieSeries = new Series("Durum Dağılımı", ViewType.Pie);
            chartDurum.Series.Add(pieSeries);
            chartDurum.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;

            var pieTitle = new ChartTitle();
            pieTitle.Text = "Servis Durum Dağılımı";
            chartDurum.Titles.Add(pieTitle);

            panelMain.Controls.Add(chartDurum);

            // Info Panel
            var grpInfo = new GroupControl();
            grpInfo.Text = "Özet Bilgiler";
            grpInfo.Location = new Point(500, 180);
            grpInfo.Size = new Size(450, 350);

            var lblInfo = new LabelControl();
            lblInfo.Name = "lblInfo";
            lblInfo.Text = "Yükleniyor...";
            lblInfo.Location = new Point(20, 40);
            lblInfo.AutoSize = true;
            grpInfo.Controls.Add(lblInfo);

            panelMain.Controls.Add(grpInfo);

            this.Controls.Add(panelMain);
            this.ResumeLayout(false);
        }

        private PanelControl CreateStatCard(string title, string value, Color color, int x, int y, int width, int height)
        {
            var card = new PanelControl();
            card.Size = new Size(width, height);
            card.Location = new Point(x, y);
            card.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;

            var colorBar = new Panel();
            colorBar.BackColor = color;
            colorBar.Height = 5;
            colorBar.Dock = DockStyle.Top;
            card.Controls.Add(colorBar);

            var lblTitle = new LabelControl();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 10);
            lblTitle.ForeColor = Color.Gray;
            lblTitle.Location = new Point(15, 20);
            card.Controls.Add(lblTitle);

            var lblValue = new LabelControl();
            lblValue.Name = $"lbl{title.Replace(" ", "")}Value";
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblValue.ForeColor = color;
            lblValue.Location = new Point(15, 45);
            lblValue.AutoSize = true;
            card.Controls.Add(lblValue);

            return card;
        }

        private async void LoadReportData()
        {
            try
            {
                // İstatistikleri al
                var beklemede = await _servisRepository.GetCountByDurumAsync("Beklemede");
                var islemde = await _servisRepository.GetCountByDurumAsync("İşlemde");
                var tamamlandi = await _servisRepository.GetCountByDurumAsync("Tamamlandı");
                var teslimEdildi = await _servisRepository.GetCountByDurumAsync("Teslim Edildi");
                var iptal = await _servisRepository.GetCountByDurumAsync("İptal");

                var toplam = beklemede + islemde + tamamlandi + teslimEdildi + iptal;

                // Kartları güncelle
                UpdateStatValue("cardToplam", toplam.ToString());
                UpdateStatValue("cardBeklemede", beklemede.ToString());
                UpdateStatValue("cardIslemde", islemde.ToString());
                UpdateStatValue("cardTamamlandi", tamamlandi.ToString());

                // Pie Chart'ı güncelle
                var chartDurum = this.Controls.Find("chartDurum", true)[0] as ChartControl;
                if (chartDurum != null && chartDurum.Series.Count > 0)
                {
                    var series = chartDurum.Series[0];
                    series.Points.Clear();
                    series.Points.Add(new SeriesPoint("Beklemede", beklemede));
                    series.Points.Add(new SeriesPoint("İşlemde", islemde));
                    series.Points.Add(new SeriesPoint("Tamamlandı", tamamlandi));
                    series.Points.Add(new SeriesPoint("Teslim Edildi", teslimEdildi));
                    series.Points.Add(new SeriesPoint("İptal", iptal));

                    // Renkleri ayarla
                    if (series.Points.Count > 0)
                    {
                        series.Points[0].Color = AppColors.Beklemede;
                        series.Points[1].Color = AppColors.Islemde;
                        series.Points[2].Color = AppColors.Tamamlandi;
                        series.Points[3].Color = AppColors.TeslimEdildi;
                        series.Points[4].Color = AppColors.Iptal;
                    }
                }

                // Info label
                var todayCount = await _servisRepository.GetTodayCountAsync();
                var todayIncome = await _servisRepository.GetTodayIncomeAsync();

                var lblInfo = this.Controls.Find("lblInfo", true)[0] as LabelControl;
                lblInfo!.Text = $@"📊 Genel İstatistikler

Toplam Servis Kaydı: {toplam}

📌 Aktif Durumlar:
  • Beklemede: {beklemede}
  • İşlemde: {islemde}

✅ Tamamlanan:
  • Tamamlandı: {tamamlandi}
  • Teslim Edildi: {teslimEdildi}

❌ İptal Edilen: {iptal}

📅 Bugünkü İşlemler:
  • Yeni Kayıt: {todayCount}
  • Tahsilat: ₺{todayIncome:N2}";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Rapor verileri yüklenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatValue(string cardName, string value)
        {
            var card = this.Controls.Find(cardName, true);
            if (card.Length > 0)
            {
                foreach (Control ctrl in card[0].Controls)
                {
                    if (ctrl is LabelControl lbl && lbl.Name.Contains("Value"))
                    {
                        lbl.Text = value;
                        break;
                    }
                }
            }
        }
    }
}

using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeknikServisOtomasyon.Data.Repositories;
using TeknikServisOtomasyon.Helpers;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Forms.Modules
{
    public partial class DashboardForm : XtraForm
    {
        private readonly ServisKaydiRepository _servisRepository;
        private readonly MusteriRepository _musteriRepository;
        private GridControl _gridSonKayitlar = null!;
        private GridView _gridView = null!;

        public DashboardForm()
        {
            _servisRepository = new ServisKaydiRepository();
            _musteriRepository = new MusteriRepository();
            InitializeComponent();
            this.Activated += DashboardForm_Activated;
            LoadDashboardData();
        }

        private void DashboardForm_Activated(object? sender, EventArgs e)
        {
            LoadDashboardData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "📊 Dashboard - Teknik Servis Yönetim Paneli";
            this.Size = new Size(1400, 800);

            // Ana Panel - Dark Theme
            var panelMain = new PanelControl();
            panelMain.Dock = DockStyle.Fill;
            panelMain.Padding = new Padding(15);
            panelMain.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            // Başlık Paneli
            var panelHeader = new PanelControl();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 80;
            panelHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            var lblTitle = new LabelControl();
            lblTitle.Text = "🏠 Hoş Geldiniz";
            lblTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitle.Location = new Point(10, 10);
            lblTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Default;
            panelHeader.Controls.Add(lblTitle);

            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("tr-TR"));
            lblSubtitle.Font = new Font("Segoe UI", 11);
            lblSubtitle.Location = new Point(12, 48);
            lblSubtitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Default;
            panelHeader.Controls.Add(lblSubtitle);

            // Yenile butonu
            var btnRefresh = new SimpleButton();
            btnRefresh.Text = "🔄 Yenile";
            btnRefresh.Size = new Size(110, 40);
            btnRefresh.Location = new Point(1250, 15);
            btnRefresh.Appearance.BackColor = AppColors.Primary;
            btnRefresh.Appearance.ForeColor = Color.White;
            btnRefresh.Appearance.BorderColor = AppColors.Primary;
            btnRefresh.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRefresh.Click += (s, e) => LoadDashboardData();
            panelHeader.Controls.Add(btnRefresh);

            // İstatistik Kartları Paneli
            var panelStats = new PanelControl();
            panelStats.Dock = DockStyle.Top;
            panelStats.Height = 130;
            panelStats.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            panelStats.Padding = new Padding(0, 10, 0, 10);

            // Kartları oluştur
            var cards = new[]
            {
                CreateModernCard("Beklemede", "0", "⏳", AppColors.Beklemede, 0),
                CreateModernCard("İşlemde", "0", "🔧", AppColors.Islemde, 1),
                CreateModernCard("Tamamlandı", "0", "✅", AppColors.Tamamlandi, 2),
                CreateModernCard("Bugün Kayıt", "0", "📝", AppColors.Info, 3),
                CreateModernCard("Toplam Müşteri", "0", "👥", Color.FromArgb(156, 39, 176), 4)
            };

            cards[0].Name = "cardBeklemede";
            cards[1].Name = "cardIslemde";
            cards[2].Name = "cardTamamlandi";
            cards[3].Name = "cardBugun";
            cards[4].Name = "cardMusteri";

            // Kartlara tıklama olayı ekle
            cards[0].Click += (s, e) => OpenServisListeForm("Beklemede");
            cards[1].Click += (s, e) => OpenServisListeForm("İşlemde");
            cards[2].Click += (s, e) => OpenServisListeForm("Tamamlandı");
            cards[3].Click += (s, e) => OpenServisListeForm(null); // Tüm servisler
            cards[4].Click += (s, e) => OpenMusteriListeForm();

            // Alt kontroller için de tıklama olayı ekle
            foreach (Control ctrl in cards[0].Controls)
                ctrl.Click += (s, e) => OpenServisListeForm("Beklemede");
            foreach (Control ctrl in cards[1].Controls)
                ctrl.Click += (s, e) => OpenServisListeForm("İşlemde");
            foreach (Control ctrl in cards[2].Controls)
                ctrl.Click += (s, e) => OpenServisListeForm("Tamamlandı");
            foreach (Control ctrl in cards[3].Controls)
                ctrl.Click += (s, e) => OpenServisListeForm(null);
            foreach (Control ctrl in cards[4].Controls)
                ctrl.Click += (s, e) => OpenMusteriListeForm();

            foreach (var card in cards)
                panelStats.Controls.Add(card);

            // Alt İçerik Paneli
            var panelContent = new PanelControl();
            panelContent.Dock = DockStyle.Fill;
            panelContent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            panelContent.Padding = new Padding(0, 10, 0, 0);

            // Sol Panel - Son Servis Kayıtları
            var groupSonKayitlar = new GroupControl();
            groupSonKayitlar.Text = "📋 Son Servis Kayıtları (Çift tıklayarak detay görüntüle)";
            groupSonKayitlar.Dock = DockStyle.Fill;
            groupSonKayitlar.AppearanceCaption.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            groupSonKayitlar.Padding = new Padding(10);

            _gridSonKayitlar = new GridControl();
            _gridSonKayitlar.Name = "gridSonKayitlar";
            _gridSonKayitlar.Dock = DockStyle.Fill;
            
            _gridView = new GridView(_gridSonKayitlar);
            _gridSonKayitlar.MainView = _gridView;
            
            // Grid ayarları - Dark Theme
            _gridView.OptionsView.ShowGroupPanel = false;
            _gridView.OptionsView.RowAutoHeight = true;
            _gridView.OptionsView.ShowIndicator = false;
            _gridView.OptionsBehavior.Editable = false;
            _gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            _gridView.RowHeight = 35;
            
            // Çift tıklama eventi
            _gridView.DoubleClick += GridView_DoubleClick;
            
            // Row style eventi - Durum renklerini göster
            _gridView.RowStyle += GridView_RowStyle;

            groupSonKayitlar.Controls.Add(_gridSonKayitlar);

            // Sağ Panel - Hızlı İşlemler
            var panelRight = new PanelControl();
            panelRight.Dock = DockStyle.Right;
            panelRight.Width = 280;
            panelRight.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            panelRight.Padding = new Padding(15, 0, 0, 0);

            var groupHizliIslem = new GroupControl();
            groupHizliIslem.Text = "⚡ Hızlı İşlemler";
            groupHizliIslem.Dock = DockStyle.Top;
            groupHizliIslem.Height = 320;
            groupHizliIslem.AppearanceCaption.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            groupHizliIslem.Padding = new Padding(10);

            var btnYeniServis = CreateQuickButton("➕ Yeni Servis Kaydı", AppColors.Primary, 20);
            btnYeniServis.Click += BtnYeniServis_Click;
            groupHizliIslem.Controls.Add(btnYeniServis);

            var btnYeniMusteri = CreateQuickButton("👤 Yeni Müşteri", AppColors.Info, 75);
            btnYeniMusteri.Click += BtnYeniMusteri_Click;
            groupHizliIslem.Controls.Add(btnYeniMusteri);

            var btnServisAra = CreateQuickButton("🔍 Servis Ara", Color.FromArgb(156, 39, 176), 130);
            btnServisAra.Click += BtnServisAra_Click;
            groupHizliIslem.Controls.Add(btnServisAra);

            var btnBekleyenler = CreateQuickButton("⏳ Bekleyen Servisler", AppColors.Beklemede, 185);
            btnBekleyenler.Click += BtnBekleyenler_Click;
            groupHizliIslem.Controls.Add(btnBekleyenler);

            var btnTamamlananlar = CreateQuickButton("✅ Tamamlanan Servisler", AppColors.Tamamlandi, 240);
            btnTamamlananlar.Click += BtnTamamlananlar_Click;
            groupHizliIslem.Controls.Add(btnTamamlananlar);

            panelRight.Controls.Add(groupHizliIslem);

            // Durum Özeti
            var groupDurumOzeti = new GroupControl();
            groupDurumOzeti.Text = "📊 Durum Dağılımı";
            groupDurumOzeti.Dock = DockStyle.Fill;
            groupDurumOzeti.AppearanceCaption.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            groupDurumOzeti.Padding = new Padding(10);

            var lblDurumInfo = new LabelControl();
            lblDurumInfo.Name = "lblDurumInfo";
            lblDurumInfo.Text = "Veriler yükleniyor...";
            lblDurumInfo.Font = new Font("Segoe UI", 10);
            lblDurumInfo.Location = new Point(15, 35);
            lblDurumInfo.AutoSize = true;
            groupDurumOzeti.Controls.Add(lblDurumInfo);

            panelRight.Controls.Add(groupDurumOzeti);

            panelContent.Controls.Add(groupSonKayitlar);
            panelContent.Controls.Add(panelRight);

            // Doğru sıralama ile ekle
            panelMain.Controls.Add(panelContent);
            panelMain.Controls.Add(panelStats);
            panelMain.Controls.Add(panelHeader);

            this.Controls.Add(panelMain);

            this.ResumeLayout(false);
        }

        private Panel CreateModernCard(string title, string value, string icon, Color color, int index)
        {
            var card = new Panel();
            card.Size = new Size(210, 100);
            card.Location = new Point(10 + (index * 225), 10);
            card.BackColor = AppColors.BackgroundDarkSecondary;
            card.Cursor = Cursors.Hand;

            // Gölge efekti için border - Dark Theme
            card.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                using (var path = CreateRoundedRectangle(rect, 12))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var brush = new SolidBrush(AppColors.BackgroundDarkSecondary))
                        e.Graphics.FillPath(brush, path);
                    using (var pen = new Pen(AppColors.BorderDark, 1))
                        e.Graphics.DrawPath(pen, path);
                }
            };

            // Sol renk çubuğu
            var colorBar = new Panel();
            colorBar.BackColor = color;
            colorBar.Size = new Size(5, 100);
            colorBar.Location = new Point(0, 0);
            card.Controls.Add(colorBar);

            // İkon
            var lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new Font("Segoe UI Emoji", 24);
            lblIcon.Location = new Point(15, 15);
            lblIcon.Size = new Size(50, 50);
            lblIcon.BackColor = Color.Transparent;
            card.Controls.Add(lblIcon);

            // Değer - daha sağa ve daha büyük alan
            var lblValue = new Label();
            lblValue.Name = $"lbl{title.Replace(" ", "")}Value";
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 26, FontStyle.Bold);
            lblValue.ForeColor = color;
            lblValue.Location = new Point(70, 15);
            lblValue.Size = new Size(130, 45);
            lblValue.BackColor = Color.Transparent;
            lblValue.TextAlign = ContentAlignment.MiddleLeft;
            card.Controls.Add(lblValue);

            // Başlık
            var lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 10);
            lblTitle.ForeColor = AppColors.TextSecondary;
            lblTitle.Location = new Point(15, 70);
            lblTitle.Size = new Size(180, 25);
            lblTitle.BackColor = Color.Transparent;
            card.Controls.Add(lblTitle);

            // Hover efekti - Dark Theme
            card.MouseEnter += (s, e) => card.BackColor = AppColors.BackgroundDarkTertiary;
            card.MouseLeave += (s, e) => card.BackColor = AppColors.BackgroundDarkSecondary;

            return card;
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        private SimpleButton CreateQuickButton(string text, Color color, int yPos)
        {
            var btn = new SimpleButton();
            btn.Text = text;
            btn.Size = new Size(230, 45);
            btn.Location = new Point(15, yPos);
            btn.Appearance.BackColor = color;
            btn.Appearance.ForeColor = Color.White;
            btn.Appearance.BorderColor = color;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void GridView_DoubleClick(object? sender, EventArgs e)
        {
            try
            {
                if (_gridView.FocusedRowHandle < 0) return;

                var servisKaydi = _gridView.GetRow(_gridView.FocusedRowHandle) as ServisKaydi;
                if (servisKaydi != null)
                {
                    var detayForm = new ServisDetayForm(servisKaydi.Id);
                    detayForm.MdiParent = this.MdiParent;
                    detayForm.Show();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GridView_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            var view = sender as GridView;
            if (view == null) return;

            var durum = view.GetRowCellValue(e.RowHandle, "Durum")?.ToString();
            
            switch (durum)
            {
                case "Beklemede":
                    e.Appearance.ForeColor = AppColors.Beklemede;
                    break;
                case "İşlemde":
                    e.Appearance.ForeColor = AppColors.Islemde;
                    break;
                case "Tamamlandı":
                    e.Appearance.ForeColor = AppColors.Tamamlandi;
                    break;
                case "Teslim Edildi":
                    e.Appearance.ForeColor = Color.FromArgb(0, 150, 136);
                    break;
            }
        }

        private void BtnYeniServis_Click(object? sender, EventArgs e)
        {
            var form = new ServisKayitForm();
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void BtnYeniMusteri_Click(object? sender, EventArgs e)
        {
            var form = new MusteriForm();
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void BtnServisAra_Click(object? sender, EventArgs e)
        {
            var servisNo = XtraInputBox.Show("Servis numarasını girin:", "Servis Ara", "");
            if (!string.IsNullOrWhiteSpace(servisNo))
            {
                SearchServis(servisNo);
            }
        }

        private async void SearchServis(string servisNo)
        {
            try
            {
                var servis = await _servisRepository.GetByServisNoAsync(servisNo);
                if (servis != null)
                {
                    var detayForm = new ServisDetayForm(servis.Id);
                    detayForm.MdiParent = this.MdiParent;
                    detayForm.Show();
                }
                else
                {
                    XtraMessageBox.Show($"'{servisNo}' numaralı servis kaydı bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Arama hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnBekleyenler_Click(object? sender, EventArgs e)
        {
            var form = new ServisListeForm("Beklemede");
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void BtnTamamlananlar_Click(object? sender, EventArgs e)
        {
            var form = new ServisListeForm("Tamamlandı");
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void OpenServisListeForm(string? durumFilter)
        {
            var form = new ServisListeForm(durumFilter);
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void OpenMusteriListeForm()
        {
            var form = new MusteriListeForm();
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private async void LoadDashboardData()
        {
            try
            {
                // İstatistikleri yükle
                var beklemede = await _servisRepository.GetCountByDurumAsync("Beklemede");
                var islemde = await _servisRepository.GetCountByDurumAsync("İşlemde");
                var tamamlandi = await _servisRepository.GetCountByDurumAsync("Tamamlandı");
                var bugunKayit = await _servisRepository.GetTodayCountAsync();
                var toplamMusteri = await _musteriRepository.GetCountAsync();

                // UI güncelle
                UpdateCardValue("cardBeklemede", beklemede.ToString());
                UpdateCardValue("cardIslemde", islemde.ToString());
                UpdateCardValue("cardTamamlandi", tamamlandi.ToString());
                UpdateCardValue("cardBugun", bugunKayit.ToString());
                UpdateCardValue("cardMusteri", toplamMusteri.ToString());

                // Durum özeti güncelle
                UpdateDurumOzeti(beklemede, islemde, tamamlandi);

                // Son kayıtları yükle
                await LoadSonKayitlar();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Dashboard verileri yüklenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadSonKayitlar()
        {
            var sonKayitlar = await _servisRepository.GetAllAsync();
            
            if (sonKayitlar != null && sonKayitlar.Count > 0)
            {
                // Sadece son 15 kaydı göster
                var displayData = sonKayitlar.Count > 15 ? sonKayitlar.GetRange(0, 15) : sonKayitlar;
                _gridSonKayitlar.DataSource = displayData;

                if (_gridView.Columns.Count > 0)
                {
                    // Gizlenecek kolonlar
                    var columnsToHide = new[] { "Id", "MusteriId", "CihazId", "TeknikerId", "ArizaDetay",
                        "YapilanIslemler", "KullanilanParcalar", "Notlar", "GarantiBitisTarihi",
                        "KayitTarihi", "GuncellenmeTarihi", "Musteri", "Cihaz", "Tekniker",
                        "TeslimTarihi", "IscilikUcreti", "ParcaUcreti", "TahsilatDurumu", "OdenenTutar" };

                    foreach (var colName in columnsToHide)
                    {
                        if (_gridView.Columns[colName] != null)
                            _gridView.Columns[colName].Visible = false;
                    }

                    // Kolon başlıkları ve sıralaması
                    if (_gridView.Columns["ServisNo"] != null)
                    {
                        _gridView.Columns["ServisNo"].Caption = "Servis No";
                        _gridView.Columns["ServisNo"].VisibleIndex = 0;
                        _gridView.Columns["ServisNo"].Width = 130;
                    }
                    if (_gridView.Columns["MusteriAdi"] != null)
                    {
                        _gridView.Columns["MusteriAdi"].Caption = "Müşteri";
                        _gridView.Columns["MusteriAdi"].VisibleIndex = 1;
                        _gridView.Columns["MusteriAdi"].Width = 150;
                    }
                    if (_gridView.Columns["CihazTuru"] != null)
                    {
                        _gridView.Columns["CihazTuru"].Caption = "Cihaz";
                        _gridView.Columns["CihazTuru"].VisibleIndex = 2;
                        _gridView.Columns["CihazTuru"].Width = 100;
                    }
                    if (_gridView.Columns["Marka"] != null)
                    {
                        _gridView.Columns["Marka"].Caption = "Marka";
                        _gridView.Columns["Marka"].VisibleIndex = 3;
                        _gridView.Columns["Marka"].Width = 100;
                    }
                    if (_gridView.Columns["Model"] != null)
                    {
                        _gridView.Columns["Model"].Caption = "Model";
                        _gridView.Columns["Model"].VisibleIndex = 4;
                        _gridView.Columns["Model"].Width = 100;
                    }
                    if (_gridView.Columns["Ariza"] != null)
                    {
                        _gridView.Columns["Ariza"].Caption = "Arıza";
                        _gridView.Columns["Ariza"].VisibleIndex = 5;
                        _gridView.Columns["Ariza"].Width = 180;
                    }
                    if (_gridView.Columns["Durum"] != null)
                    {
                        _gridView.Columns["Durum"].Caption = "Durum";
                        _gridView.Columns["Durum"].VisibleIndex = 6;
                        _gridView.Columns["Durum"].Width = 100;
                    }
                    if (_gridView.Columns["OncelikDurumu"] != null)
                    {
                        _gridView.Columns["OncelikDurumu"].Caption = "Öncelik";
                        _gridView.Columns["OncelikDurumu"].VisibleIndex = 7;
                        _gridView.Columns["OncelikDurumu"].Width = 80;
                    }
                    if (_gridView.Columns["GirisTarihi"] != null)
                    {
                        _gridView.Columns["GirisTarihi"].Caption = "Giriş Tarihi";
                        _gridView.Columns["GirisTarihi"].VisibleIndex = 8;
                        _gridView.Columns["GirisTarihi"].Width = 120;
                        _gridView.Columns["GirisTarihi"].DisplayFormat.FormatType = FormatType.DateTime;
                        _gridView.Columns["GirisTarihi"].DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
                    }
                    if (_gridView.Columns["ToplamUcret"] != null)
                    {
                        _gridView.Columns["ToplamUcret"].Caption = "Tutar";
                        _gridView.Columns["ToplamUcret"].VisibleIndex = 9;
                        _gridView.Columns["ToplamUcret"].Width = 100;
                        _gridView.Columns["ToplamUcret"].DisplayFormat.FormatType = FormatType.Numeric;
                        _gridView.Columns["ToplamUcret"].DisplayFormat.FormatString = "₺{0:N2}";
                    }
                }
            }
            else
            {
                _gridSonKayitlar.DataSource = null;
            }
        }

        private void UpdateCardValue(string cardName, string value)
        {
            var cards = this.Controls.Find(cardName, true);
            if (cards.Length > 0)
            {
                foreach (Control ctrl in cards[0].Controls)
                {
                    if (ctrl is Label lbl && lbl.Name.Contains("Value"))
                    {
                        lbl.Text = value;
                        break;
                    }
                }
            }
        }

        private void UpdateDurumOzeti(int beklemede, int islemde, int tamamlandi)
        {
            var lblDurumInfo = this.Controls.Find("lblDurumInfo", true);
            if (lblDurumInfo.Length > 0 && lblDurumInfo[0] is LabelControl lbl)
            {
                var toplam = beklemede + islemde + tamamlandi;
                lbl.Text = $"📊 Toplam Aktif Servis: {toplam}\n\n" +
                           $"⏳ Beklemede: {beklemede}\n" +
                           $"🔧 İşlemde: {islemde}\n" +
                           $"✅ Tamamlandı: {tamamlandi}";
            }
        }
    }
}

using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeknikServisOtomasyon.Data.Repositories;
using TeknikServisOtomasyon.Helpers;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Forms.Modules
{
    public partial class ServisListeForm : XtraForm
    {
        private readonly ServisKaydiRepository _servisRepository;
        private GridControl _gridServisler = null!;
        private GridView _gridView = null!;
        private string? _initialDurumFilter;

        public ServisListeForm() : this(null)
        {
        }

        public ServisListeForm(string? durumFilter)
        {
            _servisRepository = new ServisKaydiRepository();
            _initialDurumFilter = durumFilter;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            string titleIcon = _initialDurumFilter switch
            {
                "Beklemede" => "⏳",
                "İşlemde" => "🔧",
                "Tamamlandı" => "✅",
                "Teslim Edildi" => "📦",
                _ => "📋"
            };

            if (!string.IsNullOrEmpty(_initialDurumFilter))
                this.Text = $"{titleIcon} Servis Kayıtları - {_initialDurumFilter}";
            else
                this.Text = "📋 Servis Kayıtları";

            this.Size = new Size(1300, 750);

            var panelMain = new PanelControl();
            panelMain.Dock = DockStyle.Fill;
            panelMain.Padding = new Padding(15);

            // Başlık Paneli
            var panelHeader = new PanelControl();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 70;
            panelHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            var lblTitle = new LabelControl();
            lblTitle.Text = !string.IsNullOrEmpty(_initialDurumFilter)
                ? $"{titleIcon} {_initialDurumFilter} Servis Kayıtları"
                : "📋 Tüm Servis Kayıtları";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.Location = new Point(10, 15);
            panelHeader.Controls.Add(lblTitle);

            var lblSubtitle = new LabelControl();
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Text = "Yükleniyor...";
            lblSubtitle.Font = new Font("Segoe UI", 10);
            lblSubtitle.Location = new Point(12, 45);
            panelHeader.Controls.Add(lblSubtitle);

            // Toolbar Paneli
            var panelToolbar = new PanelControl();
            panelToolbar.Dock = DockStyle.Top;
            panelToolbar.Height = 70;
            panelToolbar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            panelToolbar.Padding = new Padding(5);

            // Arama Grubu
            var grpArama = new GroupControl();
            grpArama.Text = "🔍 Arama";
            grpArama.Location = new Point(10, 5);
            grpArama.Size = new Size(380, 60);
            grpArama.AppearanceCaption.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            var txtArama = new TextEdit();
            txtArama.Name = "txtArama";
            txtArama.Location = new Point(10, 25);
            txtArama.Size = new Size(270, 28);
            txtArama.Properties.NullValuePrompt = "Servis No, Müşteri, Cihaz ara...";
            txtArama.KeyPress += async (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                    await SearchData(txtArama.Text);
            };
            grpArama.Controls.Add(txtArama);

            var btnAra = new SimpleButton();
            btnAra.Text = "Ara";
            btnAra.Location = new Point(290, 24);
            btnAra.Size = new Size(75, 28);
            btnAra.Appearance.BackColor = AppColors.Primary;
            btnAra.Appearance.ForeColor = Color.White;
            btnAra.Click += async (s, e) => await SearchData(txtArama.Text);
            grpArama.Controls.Add(btnAra);

            panelToolbar.Controls.Add(grpArama);

            // Filtre Grubu
            var grpFiltre = new GroupControl();
            grpFiltre.Text = "🎯 Filtrele";
            grpFiltre.Location = new Point(400, 5);
            grpFiltre.Size = new Size(300, 60);
            grpFiltre.AppearanceCaption.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            var cmbDurum = new ComboBoxEdit();
            cmbDurum.Name = "cmbDurum";
            cmbDurum.Location = new Point(10, 25);
            cmbDurum.Size = new Size(180, 28);
            cmbDurum.Properties.Items.AddRange(new[] { "Tümü", "Beklemede", "İşlemde", "Tamamlandı", "Teslim Edildi", "İptal" });

            if (!string.IsNullOrEmpty(_initialDurumFilter))
            {
                var index = cmbDurum.Properties.Items.IndexOf(_initialDurumFilter);
                cmbDurum.SelectedIndex = index >= 0 ? index : 0;
            }
            else
            {
                cmbDurum.SelectedIndex = 0;
            }

            cmbDurum.SelectedIndexChanged += async (s, e) => await FilterByDurum(cmbDurum.Text);
            grpFiltre.Controls.Add(cmbDurum);

            var btnYenile = new SimpleButton();
            btnYenile.Text = "🔄";
            btnYenile.Location = new Point(200, 24);
            btnYenile.Size = new Size(40, 28);
            btnYenile.ToolTip = "Yenile";
            btnYenile.Click += (s, e) => LoadData();
            grpFiltre.Controls.Add(btnYenile);

            var btnTemizle = new SimpleButton();
            btnTemizle.Text = "✖";
            btnTemizle.Location = new Point(250, 24);
            btnTemizle.Size = new Size(40, 28);
            btnTemizle.ToolTip = "Filtreleri Temizle";
            btnTemizle.Click += (s, e) =>
            {
                txtArama.Text = "";
                cmbDurum.SelectedIndex = 0;
                _initialDurumFilter = null;
                LoadData();
            };
            grpFiltre.Controls.Add(btnTemizle);

            panelToolbar.Controls.Add(grpFiltre);

            // İşlemler Grubu
            var grpIslemler = new GroupControl();
            grpIslemler.Text = "⚡ İşlemler";
            grpIslemler.Location = new Point(710, 5);
            grpIslemler.Size = new Size(550, 60);
            grpIslemler.AppearanceCaption.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            var btnYeniServis = new SimpleButton();
            btnYeniServis.Text = "➕ Yeni Servis";
            btnYeniServis.Location = new Point(10, 24);
            btnYeniServis.Size = new Size(120, 28);
            btnYeniServis.Appearance.BackColor = AppColors.Success;
            btnYeniServis.Appearance.ForeColor = Color.White;
            btnYeniServis.Click += BtnYeniServis_Click;
            grpIslemler.Controls.Add(btnYeniServis);

            var btnDetay = new SimpleButton();
            btnDetay.Text = "👁 Detay";
            btnDetay.Location = new Point(140, 24);
            btnDetay.Size = new Size(100, 28);
            btnDetay.Appearance.BackColor = AppColors.Info;
            btnDetay.Appearance.ForeColor = Color.White;
            btnDetay.Click += (s, e) => ShowServisDetail();
            grpIslemler.Controls.Add(btnDetay);

            var btnDuzenle = new SimpleButton();
            btnDuzenle.Text = "✏ Düzenle";
            btnDuzenle.Location = new Point(250, 24);
            btnDuzenle.Size = new Size(100, 28);
            btnDuzenle.Appearance.BackColor = AppColors.Warning;
            btnDuzenle.Appearance.ForeColor = Color.Black;
            btnDuzenle.Click += (s, e) => EditServis();
            grpIslemler.Controls.Add(btnDuzenle);

            var btnDurumGuncelle = new SimpleButton();
            btnDurumGuncelle.Text = "🔄 Durum";
            btnDurumGuncelle.Location = new Point(360, 24);
            btnDurumGuncelle.Size = new Size(100, 28);
            btnDurumGuncelle.Appearance.BackColor = Color.FromArgb(156, 39, 176);
            btnDurumGuncelle.Appearance.ForeColor = Color.White;
            btnDurumGuncelle.Click += (s, e) => UpdateDurum();
            grpIslemler.Controls.Add(btnDurumGuncelle);

            var btnSil = new SimpleButton();
            btnSil.Text = "🗑 Sil";
            btnSil.Location = new Point(470, 24);
            btnSil.Size = new Size(70, 28);
            btnSil.Appearance.BackColor = AppColors.Danger;
            btnSil.Appearance.ForeColor = Color.White;
            btnSil.Click += async (s, e) => await DeleteServis();
            grpIslemler.Controls.Add(btnSil);

            panelToolbar.Controls.Add(grpIslemler);

            // Grid
            _gridServisler = new GridControl();
            _gridServisler.Dock = DockStyle.Fill;
            _gridView = new GridView(_gridServisler);
            _gridServisler.MainView = _gridView;

            _gridView.OptionsView.ShowGroupPanel = false;
            _gridView.OptionsView.ShowIndicator = false;
            _gridView.OptionsView.RowAutoHeight = true;
            _gridView.OptionsBehavior.Editable = false;
            _gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            _gridView.RowHeight = 35;
            _gridView.DoubleClick += GridView_DoubleClick;

            // Sağ tık menüsü
            var popupMenu = new PopupMenu();
            var barManager = new BarManager();
            barManager.Form = this;

            var menuDetay = new BarButtonItem(barManager, "👁 Detay Görüntüle");
            menuDetay.ItemClick += (s, e) => ShowServisDetail();
            popupMenu.AddItem(menuDetay);

            var menuDuzenle = new BarButtonItem(barManager, "✏ Düzenle");
            menuDuzenle.ItemClick += (s, e) => EditServis();
            popupMenu.AddItem(menuDuzenle);

            var menuDurumGuncelle = new BarButtonItem(barManager, "🔄 Durum Güncelle");
            menuDurumGuncelle.ItemClick += (s, e) => UpdateDurum();
            popupMenu.AddItem(menuDurumGuncelle);

            var menuSil = new BarButtonItem(barManager, "🗑 Sil");
            menuSil.ItemClick += async (s, e) => await DeleteServis();
            popupMenu.AddItem(menuSil);

            _gridView.PopupMenuShowing += (s, e) =>
            {
                if (e.MenuType == GridMenuType.Row)
                    popupMenu.ShowPopup(barManager, MousePosition);
            };

            panelMain.Controls.Add(_gridServisler);
            panelMain.Controls.Add(panelToolbar);
            panelMain.Controls.Add(panelHeader);
            this.Controls.Add(panelMain);

            this.ResumeLayout(false);
        }

        private async void LoadData()
        {
            try
            {
                System.Collections.Generic.List<ServisKaydi> servisler;

                if (!string.IsNullOrEmpty(_initialDurumFilter))
                    servisler = await _servisRepository.GetByDurumAsync(_initialDurumFilter);
                else
                    servisler = await _servisRepository.GetAllAsync();

                _gridServisler.DataSource = servisler;
                ConfigureGridColumns();

                var lblSubtitle = this.Controls.Find("lblSubtitle", true);
                if (lblSubtitle.Length > 0 && lblSubtitle[0] is LabelControl lbl)
                    lbl.Text = $"Toplam {servisler.Count} kayıt listeleniyor";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Veriler yüklenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureGridColumns()
        {
            if (_gridView.Columns.Count == 0) return;

            var hiddenColumns = new[] { "Id", "MusteriId", "CihazId", "TeknikerId", "ArizaDetay",
                "YapilanIslemler", "KullanilanParcalar", "Notlar", "GarantiBitisTarihi", "KayitTarihi",
                "GuncellenmeTarihi", "Musteri", "Cihaz", "Tekniker", "IscilikUcreti", "ParcaUcreti", "OdenenTutar" };

            foreach (var col in hiddenColumns)
                if (_gridView.Columns[col] != null)
                    _gridView.Columns[col].Visible = false;

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
                _gridView.Columns["CihazTuru"].Width = 80;
            }
            if (_gridView.Columns["Marka"] != null)
            {
                _gridView.Columns["Marka"].Caption = "Marka";
                _gridView.Columns["Marka"].VisibleIndex = 3;
                _gridView.Columns["Marka"].Width = 80;
            }
            if (_gridView.Columns["Model"] != null)
            {
                _gridView.Columns["Model"].Caption = "Model";
                _gridView.Columns["Model"].VisibleIndex = 4;
                _gridView.Columns["Model"].Width = 80;
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
                _gridView.Columns["GirisTarihi"].DisplayFormat.FormatType = FormatType.DateTime;
                _gridView.Columns["GirisTarihi"].DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
                _gridView.Columns["GirisTarihi"].VisibleIndex = 8;
                _gridView.Columns["GirisTarihi"].Width = 120;
            }
            if (_gridView.Columns["ToplamUcret"] != null)
            {
                _gridView.Columns["ToplamUcret"].Caption = "Tutar";
                _gridView.Columns["ToplamUcret"].DisplayFormat.FormatType = FormatType.Numeric;
                _gridView.Columns["ToplamUcret"].DisplayFormat.FormatString = "₺{0:N2}";
                _gridView.Columns["ToplamUcret"].VisibleIndex = 9;
                _gridView.Columns["ToplamUcret"].Width = 100;
            }
            if (_gridView.Columns["TahsilatDurumu"] != null)
            {
                _gridView.Columns["TahsilatDurumu"].Caption = "Tahsilat";
                _gridView.Columns["TahsilatDurumu"].VisibleIndex = 10;
                _gridView.Columns["TahsilatDurumu"].Width = 80;
            }

            _gridView.RowCellStyle += (s, e) =>
            {
                if (e.Column.FieldName == "Durum")
                {
                    var durum = e.CellValue?.ToString();
                    e.Appearance.ForeColor = AppColors.GetDurumRengi(durum ?? "");
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                if (e.Column.FieldName == "OncelikDurumu")
                {
                    var oncelik = e.CellValue?.ToString();
                    e.Appearance.ForeColor = AppColors.GetOncelikRengi(oncelik ?? "");
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            };
        }

        private async Task SearchData(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                LoadData();
                return;
            }

            try
            {
                var servisler = await _servisRepository.SearchAsync(searchTerm);
                _gridServisler.DataSource = servisler;

                var lblSubtitle = this.Controls.Find("lblSubtitle", true);
                if (lblSubtitle.Length > 0 && lblSubtitle[0] is LabelControl lbl)
                    lbl.Text = $"'{searchTerm}' için {servisler.Count} sonuç bulundu";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Arama yapılırken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task FilterByDurum(string durum)
        {
            try
            {
                if (durum == "Tümü")
                {
                    _initialDurumFilter = null;
                    LoadData();
                    return;
                }

                _initialDurumFilter = durum;
                var servisler = await _servisRepository.GetByDurumAsync(durum);
                _gridServisler.DataSource = servisler;

                var lblSubtitle = this.Controls.Find("lblSubtitle", true);
                if (lblSubtitle.Length > 0 && lblSubtitle[0] is LabelControl lbl)
                    lbl.Text = $"'{durum}' durumunda {servisler.Count} kayıt";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Filtreleme yapılırken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnYeniServis_Click(object? sender, EventArgs e)
        {
            var form = new ServisKayitForm();
            form.MdiParent = this.MdiParent;
            form.FormClosed += (s, args) => LoadData();
            form.Show();
        }

        private void GridView_DoubleClick(object? sender, EventArgs e)
        {
            ShowServisDetail();
        }

        private void ShowServisDetail()
        {
            var servis = _gridView.GetFocusedRow() as ServisKaydi;
            if (servis == null) return;

            var form = new ServisDetayForm(servis.Id);
            form.MdiParent = this.MdiParent;
            form.FormClosed += (s, args) => LoadData();
            form.Show();
        }

        private void EditServis()
        {
            var servis = _gridView.GetFocusedRow() as ServisKaydi;
            if (servis == null) return;

            var form = new ServisKayitForm(servis.Id);
            form.MdiParent = this.MdiParent;
            form.FormClosed += (s, args) => LoadData();
            form.Show();
        }

        private async Task DeleteServis()
        {
            var servis = _gridView.GetFocusedRow() as ServisKaydi;
            if (servis == null) return;

            var result = XtraMessageBox.Show(
                $"'{servis.ServisNo}' numaralı servis kaydını silmek istediğinize emin misiniz?",
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _servisRepository.DeleteAsync(servis.Id);
                    XtraMessageBox.Show("Servis kaydı silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Silme işlemi sırasında hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void UpdateDurum()
        {
            var servis = _gridView.GetFocusedRow() as ServisKaydi;
            if (servis == null) return;

            var durumlar = new[] { "Beklemede", "İşlemde", "Tamamlandı", "Teslim Edildi", "İptal" };

            using (var form = new XtraForm())
            {
                form.Text = "🔄 Durum Güncelle";
                form.Size = new Size(350, 180);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var lbl = new LabelControl();
                lbl.Text = $"Servis No: {servis.ServisNo}";
                lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                lbl.Location = new Point(20, 20);
                form.Controls.Add(lbl);

                var lblDurum = new LabelControl();
                lblDurum.Text = "Yeni Durum:";
                lblDurum.Location = new Point(20, 55);
                form.Controls.Add(lblDurum);

                var cmbDurum = new ComboBoxEdit();
                cmbDurum.Location = new Point(100, 52);
                cmbDurum.Size = new Size(220, 28);
                cmbDurum.Properties.Items.AddRange(durumlar);
                cmbDurum.EditValue = servis.Durum;
                cmbDurum.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                form.Controls.Add(cmbDurum);

                var btnTamam = new SimpleButton();
                btnTamam.Text = "✓ Güncelle";
                btnTamam.Location = new Point(120, 100);
                btnTamam.Size = new Size(100, 35);
                btnTamam.Appearance.BackColor = AppColors.Success;
                btnTamam.Appearance.ForeColor = Color.White;
                btnTamam.DialogResult = DialogResult.OK;
                form.Controls.Add(btnTamam);
                form.AcceptButton = btnTamam;

                if (form.ShowDialog() == DialogResult.OK && cmbDurum.Text != servis.Durum)
                {
                    try
                    {
                        await _servisRepository.UpdateDurumAsync(servis.Id, cmbDurum.Text);
                        XtraMessageBox.Show("Durum güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show($"Durum güncellenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

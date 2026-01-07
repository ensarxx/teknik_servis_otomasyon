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
    public partial class MusteriListeForm : XtraForm
    {
        private readonly MusteriRepository _musteriRepository;
        private GridControl _gridMusteriler = null!;
        private GridView _gridView = null!;

        public MusteriListeForm()
        {
            _musteriRepository = new MusteriRepository();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "👥 Müşteri Listesi";
            this.Size = new Size(1150, 700);

            var panelMain = new PanelControl();
            panelMain.Dock = DockStyle.Fill;
            panelMain.Padding = new Padding(15);

            // Başlık Paneli
            var panelHeader = new PanelControl();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 70;
            panelHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            var lblTitle = new LabelControl();
            lblTitle.Text = "👥 Müşteri Yönetimi";
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
            grpArama.Size = new Size(400, 60);
            grpArama.AppearanceCaption.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            var txtArama = new TextEdit();
            txtArama.Name = "txtArama";
            txtArama.Location = new Point(10, 25);
            txtArama.Size = new Size(280, 28);
            txtArama.Properties.NullValuePrompt = "Ad, Telefon, E-posta veya TC No ara...";
            txtArama.KeyPress += async (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                    await SearchData(txtArama.Text);
            };
            grpArama.Controls.Add(txtArama);

            var btnAra = new SimpleButton();
            btnAra.Text = "Ara";
            btnAra.Location = new Point(300, 24);
            btnAra.Size = new Size(85, 28);
            btnAra.Appearance.BackColor = AppColors.Primary;
            btnAra.Appearance.ForeColor = Color.White;
            btnAra.Click += async (s, e) => await SearchData(txtArama.Text);
            grpArama.Controls.Add(btnAra);

            panelToolbar.Controls.Add(grpArama);

            // Araçlar Grubu
            var grpAraclar = new GroupControl();
            grpAraclar.Text = "🛠️ Araçlar";
            grpAraclar.Location = new Point(420, 5);
            grpAraclar.Size = new Size(150, 60);
            grpAraclar.AppearanceCaption.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            var btnYenile = new SimpleButton();
            btnYenile.Text = "🔄 Yenile";
            btnYenile.Location = new Point(10, 24);
            btnYenile.Size = new Size(80, 28);
            btnYenile.Click += (s, e) =>
            {
                txtArama.Text = "";
                LoadData();
            };
            grpAraclar.Controls.Add(btnYenile);

            var btnTemizle = new SimpleButton();
            btnTemizle.Text = "✖";
            btnTemizle.Location = new Point(100, 24);
            btnTemizle.Size = new Size(40, 28);
            btnTemizle.ToolTip = "Aramayı Temizle";
            btnTemizle.Click += (s, e) =>
            {
                txtArama.Text = "";
                LoadData();
            };
            grpAraclar.Controls.Add(btnTemizle);

            panelToolbar.Controls.Add(grpAraclar);

            // İşlemler Grubu
            var grpIslemler = new GroupControl();
            grpIslemler.Text = "⚡ İşlemler";
            grpIslemler.Location = new Point(580, 5);
            grpIslemler.Size = new Size(520, 60);
            grpIslemler.AppearanceCaption.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            var btnYeniMusteri = new SimpleButton();
            btnYeniMusteri.Text = "➕ Yeni Müşteri";
            btnYeniMusteri.Location = new Point(10, 24);
            btnYeniMusteri.Size = new Size(130, 28);
            btnYeniMusteri.Appearance.BackColor = AppColors.Success;
            btnYeniMusteri.Appearance.ForeColor = Color.White;
            btnYeniMusteri.Click += BtnYeniMusteri_Click;
            grpIslemler.Controls.Add(btnYeniMusteri);

            var btnDuzenle = new SimpleButton();
            btnDuzenle.Text = "✏ Düzenle";
            btnDuzenle.Location = new Point(150, 24);
            btnDuzenle.Size = new Size(100, 28);
            btnDuzenle.Appearance.BackColor = AppColors.Warning;
            btnDuzenle.Appearance.ForeColor = Color.Black;
            btnDuzenle.Click += (s, e) => EditMusteri();
            grpIslemler.Controls.Add(btnDuzenle);

            var btnServisler = new SimpleButton();
            btnServisler.Text = "📋 Servisler";
            btnServisler.Location = new Point(260, 24);
            btnServisler.Size = new Size(110, 28);
            btnServisler.Appearance.BackColor = AppColors.Info;
            btnServisler.Appearance.ForeColor = Color.White;
            btnServisler.Click += (s, e) => ShowServisKayitlari();
            grpIslemler.Controls.Add(btnServisler);

            var btnCihazlar = new SimpleButton();
            btnCihazlar.Text = "📱 Cihazlar";
            btnCihazlar.Location = new Point(380, 24);
            btnCihazlar.Size = new Size(100, 28);
            btnCihazlar.Appearance.BackColor = Color.FromArgb(156, 39, 176);
            btnCihazlar.Appearance.ForeColor = Color.White;
            btnCihazlar.Click += (s, e) => ShowCihazlar();
            grpIslemler.Controls.Add(btnCihazlar);

            var btnSil = new SimpleButton();
            btnSil.Text = "🗑";
            btnSil.Location = new Point(490, 24);
            btnSil.Size = new Size(40, 28);
            btnSil.Appearance.BackColor = AppColors.Danger;
            btnSil.Appearance.ForeColor = Color.White;
            btnSil.ToolTip = "Sil";
            btnSil.Click += (s, e) => DeleteMusteri();
            grpIslemler.Controls.Add(btnSil);

            panelToolbar.Controls.Add(grpIslemler);

            // Grid
            _gridMusteriler = new GridControl();
            _gridMusteriler.Dock = DockStyle.Fill;
            _gridView = new GridView(_gridMusteriler);
            _gridMusteriler.MainView = _gridView;

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

            var menuDuzenle = new BarButtonItem(barManager, "✏ Düzenle");
            menuDuzenle.ItemClick += (s, e) => EditMusteri();
            popupMenu.AddItem(menuDuzenle);

            var menuServisler = new BarButtonItem(barManager, "📋 Servis Kayıtları");
            menuServisler.ItemClick += (s, e) => ShowServisKayitlari();
            popupMenu.AddItem(menuServisler);

            var menuCihazlar = new BarButtonItem(barManager, "📱 Cihazları Görüntüle");
            menuCihazlar.ItemClick += (s, e) => ShowCihazlar();
            popupMenu.AddItem(menuCihazlar);

            var menuSil = new BarButtonItem(barManager, "🗑 Sil");
            menuSil.ItemClick += (s, e) => DeleteMusteri();
            popupMenu.AddItem(menuSil);

            _gridView.PopupMenuShowing += (s, e) =>
            {
                if (e.MenuType == GridMenuType.Row)
                    popupMenu.ShowPopup(barManager, MousePosition);
            };

            panelMain.Controls.Add(_gridMusteriler);
            panelMain.Controls.Add(panelToolbar);
            panelMain.Controls.Add(panelHeader);
            this.Controls.Add(panelMain);

            this.ResumeLayout(false);
        }

        private async void LoadData()
        {
            try
            {
                var musteriler = await _musteriRepository.GetAllAsync();
                
                if (musteriler == null || musteriler.Count == 0)
                {
                    _gridMusteriler.DataSource = null;
                    var lblSubtitle = this.Controls.Find("lblSubtitle", true);
                    if (lblSubtitle.Length > 0 && lblSubtitle[0] is LabelControl lbl)
                        lbl.Text = "Henüz müşteri kaydı bulunmuyor";
                    return;
                }
                
                _gridMusteriler.DataSource = musteriler;
                _gridMusteriler.RefreshDataSource();
                ConfigureGridColumns();

                var lblSub = this.Controls.Find("lblSubtitle", true);
                if (lblSub.Length > 0 && lblSub[0] is LabelControl lblc)
                    lblc.Text = $"Toplam {musteriler.Count} müşteri listeleniyor";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Veriler yüklenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureGridColumns()
        {
            if (_gridView.Columns.Count == 0) return;

            if (_gridView.Columns["Id"] != null)
                _gridView.Columns["Id"].Visible = false;
            if (_gridView.Columns["Aktif"] != null)
                _gridView.Columns["Aktif"].Visible = false;
            if (_gridView.Columns["Cihazlar"] != null)
                _gridView.Columns["Cihazlar"].Visible = false;
            if (_gridView.Columns["ServisKayitlari"] != null)
                _gridView.Columns["ServisKayitlari"].Visible = false;

            if (_gridView.Columns["AdSoyad"] != null)
            {
                _gridView.Columns["AdSoyad"].Caption = "Ad Soyad";
                _gridView.Columns["AdSoyad"].VisibleIndex = 0;
                _gridView.Columns["AdSoyad"].Width = 180;
            }
            if (_gridView.Columns["Telefon"] != null)
            {
                _gridView.Columns["Telefon"].Caption = "Telefon";
                _gridView.Columns["Telefon"].VisibleIndex = 1;
                _gridView.Columns["Telefon"].Width = 130;
            }
            if (_gridView.Columns["Telefon2"] != null)
            {
                _gridView.Columns["Telefon2"].Caption = "Telefon 2";
                _gridView.Columns["Telefon2"].VisibleIndex = 2;
                _gridView.Columns["Telefon2"].Width = 130;
            }
            if (_gridView.Columns["Email"] != null)
            {
                _gridView.Columns["Email"].Caption = "E-posta";
                _gridView.Columns["Email"].VisibleIndex = 3;
                _gridView.Columns["Email"].Width = 180;
            }
            if (_gridView.Columns["Adres"] != null)
            {
                _gridView.Columns["Adres"].Caption = "Adres";
                _gridView.Columns["Adres"].VisibleIndex = 4;
                _gridView.Columns["Adres"].Width = 250;
            }
            if (_gridView.Columns["TCKimlikNo"] != null)
            {
                _gridView.Columns["TCKimlikNo"].Caption = "TC Kimlik No";
                _gridView.Columns["TCKimlikNo"].VisibleIndex = 5;
                _gridView.Columns["TCKimlikNo"].Width = 120;
            }
            if (_gridView.Columns["KayitTarihi"] != null)
            {
                _gridView.Columns["KayitTarihi"].Caption = "Kayıt Tarihi";
                _gridView.Columns["KayitTarihi"].DisplayFormat.FormatType = FormatType.DateTime;
                _gridView.Columns["KayitTarihi"].DisplayFormat.FormatString = "dd.MM.yyyy";
                _gridView.Columns["KayitTarihi"].VisibleIndex = 6;
                _gridView.Columns["KayitTarihi"].Width = 100;
            }
            if (_gridView.Columns["Notlar"] != null)
            {
                _gridView.Columns["Notlar"].Caption = "Notlar";
                _gridView.Columns["Notlar"].VisibleIndex = 7;
            }
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
                var musteriler = await _musteriRepository.SearchAsync(searchTerm);
                _gridMusteriler.DataSource = musteriler;

                var lblSubtitle = this.Controls.Find("lblSubtitle", true);
                if (lblSubtitle.Length > 0 && lblSubtitle[0] is LabelControl lbl)
                    lbl.Text = $"'{searchTerm}' için {musteriler.Count} sonuç bulundu";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Arama yapılırken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnYeniMusteri_Click(object? sender, EventArgs e)
        {
            var form = new MusteriForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void GridView_DoubleClick(object? sender, EventArgs e)
        {
            EditMusteri();
        }

        private void EditMusteri()
        {
            var musteri = _gridView.GetFocusedRow() as Musteri;
            if (musteri == null) return;

            var form = new MusteriForm(musteri.Id);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void ShowServisKayitlari()
        {
            var musteri = _gridView.GetFocusedRow() as Musteri;
            if (musteri == null) return;

            XtraMessageBox.Show($"{musteri.AdSoyad} adlı müşterinin servis kayıtları görüntülenecek.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowCihazlar()
        {
            var musteri = _gridView.GetFocusedRow() as Musteri;
            if (musteri == null) return;

            XtraMessageBox.Show($"{musteri.AdSoyad} adlı müşterinin cihazları görüntülenecek.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void DeleteMusteri()
        {
            var musteri = _gridView.GetFocusedRow() as Musteri;
            if (musteri == null) return;

            var result = XtraMessageBox.Show($"'{musteri.AdSoyad}' adlı müşteriyi silmek istediğinize emin misiniz?", 
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _musteriRepository.DeleteAsync(musteri.Id);
                    XtraMessageBox.Show("Müşteri silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Silme işlemi sırasında hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

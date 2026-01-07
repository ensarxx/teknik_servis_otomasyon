using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Drawing;
using System.Windows.Forms;
using TeknikServisOtomasyon.Data.Repositories;
using TeknikServisOtomasyon.Helpers;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Forms.Modules
{
    public partial class KullaniciListeForm : XtraForm
    {
        private readonly KullaniciRepository _kullaniciRepository;
        private GridControl _gridKullanicilar;
        private GridView _gridView;

        public KullaniciListeForm()
        {
            _kullaniciRepository = new KullaniciRepository();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "Kullanıcı Yönetimi";
            this.Size = new Size(900, 500);

            var panelMain = new PanelControl();
            panelMain.Dock = DockStyle.Fill;
            panelMain.Padding = new Padding(10);

            // Üst Toolbar
            var panelTop = new PanelControl();
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 60;
            panelTop.Padding = new Padding(10);

            var btnYenile = new SimpleButton();
            btnYenile.Text = "Yenile";
            btnYenile.Location = new Point(10, 15);
            btnYenile.Size = new Size(80, 30);
            btnYenile.Click += (s, e) => LoadData();
            panelTop.Controls.Add(btnYenile);

            var btnYeniKullanici = new SimpleButton();
            btnYeniKullanici.Text = "Yeni Kullanıcı";
            btnYeniKullanici.Location = new Point(750, 15);
            btnYeniKullanici.Size = new Size(120, 30);
            btnYeniKullanici.Appearance.BackColor = AppColors.Success;
            btnYeniKullanici.Appearance.ForeColor = Color.White;
            btnYeniKullanici.Click += BtnYeniKullanici_Click;
            panelTop.Controls.Add(btnYeniKullanici);

            // Grid
            _gridKullanicilar = new GridControl();
            _gridKullanicilar.Dock = DockStyle.Fill;
            _gridView = new GridView(_gridKullanicilar);
            _gridKullanicilar.MainView = _gridView;

            _gridView.OptionsView.ShowGroupPanel = false;
            _gridView.OptionsBehavior.Editable = false;
            _gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            _gridView.DoubleClick += GridView_DoubleClick;

            // Sağ tık menüsü
            var popupMenu = new DevExpress.XtraBars.PopupMenu();
            var barManager = new DevExpress.XtraBars.BarManager();
            barManager.Form = this;

            var btnDuzenle = new DevExpress.XtraBars.BarButtonItem(barManager, "Düzenle");
            btnDuzenle.ItemClick += (s, e) => EditKullanici();
            popupMenu.AddItem(btnDuzenle);

            var btnSifreDegistir = new DevExpress.XtraBars.BarButtonItem(barManager, "Şifre Değiştir");
            btnSifreDegistir.ItemClick += (s, e) => ChangePassword();
            popupMenu.AddItem(btnSifreDegistir);

            var btnSil = new DevExpress.XtraBars.BarButtonItem(barManager, "Sil");
            btnSil.ItemClick += (s, e) => DeleteKullanici();
            popupMenu.AddItem(btnSil);

            _gridView.PopupMenuShowing += (s, e) =>
            {
                if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
                {
                    popupMenu.ShowPopup(barManager, MousePosition);
                }
            };

            // Kontrolleri doğru sırada ekle (Fill olan EN SON eklenmeli)
            panelMain.Controls.Add(_gridKullanicilar);  // Fill - en son
            panelMain.Controls.Add(panelTop);            // Top - önce
            this.Controls.Add(panelMain);

            this.ResumeLayout(false);
        }

        private async void LoadData()
        {
            try
            {
                var kullanicilar = await _kullaniciRepository.GetAllAsync();
                _gridKullanicilar.DataSource = kullanicilar;
                ConfigureGridColumns();
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
            if (_gridView.Columns["Sifre"] != null)
                _gridView.Columns["Sifre"].Visible = false;
            if (_gridView.Columns["Aktif"] != null)
                _gridView.Columns["Aktif"].Visible = false;

            if (_gridView.Columns["KullaniciAdi"] != null)
                _gridView.Columns["KullaniciAdi"].Caption = "Kullanıcı Adı";
            if (_gridView.Columns["AdSoyad"] != null)
                _gridView.Columns["AdSoyad"].Caption = "Ad Soyad";
            if (_gridView.Columns["Email"] != null)
                _gridView.Columns["Email"].Caption = "E-posta";
            if (_gridView.Columns["Telefon"] != null)
                _gridView.Columns["Telefon"].Caption = "Telefon";
            if (_gridView.Columns["Rol"] != null)
                _gridView.Columns["Rol"].Caption = "Rol";
            if (_gridView.Columns["KayitTarihi"] != null)
            {
                _gridView.Columns["KayitTarihi"].Caption = "Kayıt Tarihi";
                _gridView.Columns["KayitTarihi"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                _gridView.Columns["KayitTarihi"].DisplayFormat.FormatString = "dd.MM.yyyy";
            }
            if (_gridView.Columns["SonGirisTarihi"] != null)
            {
                _gridView.Columns["SonGirisTarihi"].Caption = "Son Giriş";
                _gridView.Columns["SonGirisTarihi"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                _gridView.Columns["SonGirisTarihi"].DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
            }

            _gridView.BestFitColumns();
        }

        private void BtnYeniKullanici_Click(object? sender, EventArgs e)
        {
            var form = new KullaniciForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void GridView_DoubleClick(object? sender, EventArgs e)
        {
            EditKullanici();
        }

        private void EditKullanici()
        {
            var kullanici = _gridView.GetFocusedRow() as Kullanici;
            if (kullanici == null) return;

            var form = new KullaniciForm(kullanici.Id);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private async void ChangePassword()
        {
            var kullanici = _gridView.GetFocusedRow() as Kullanici;
            if (kullanici == null) return;

            var newPassword = XtraInputBox.Show("Yeni şifreyi girin:", "Şifre Değiştir", "");
            if (!string.IsNullOrEmpty(newPassword))
            {
                try
                {
                    await _kullaniciRepository.UpdatePasswordAsync(kullanici.Id, newPassword);
                    XtraMessageBox.Show("Şifre değiştirildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Şifre değiştirilirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void DeleteKullanici()
        {
            var kullanici = _gridView.GetFocusedRow() as Kullanici;
            if (kullanici == null) return;

            if (kullanici.Id == SessionManager.CurrentUser?.Id)
            {
                XtraMessageBox.Show("Kendinizi silemezsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = XtraMessageBox.Show($"{kullanici.AdSoyad} kullanıcısını silmek istediğinize emin misiniz?",
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _kullaniciRepository.DeleteAsync(kullanici.Id);
                    XtraMessageBox.Show("Kullanıcı silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

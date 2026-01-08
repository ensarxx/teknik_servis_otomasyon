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
    public partial class ParcaListeForm : XtraForm
    {
        private readonly ParcaRepository _parcaRepository;
        private GridControl _gridParcalar = null!;
        private GridView _gridView = null!;

        public ParcaListeForm()
        {
            _parcaRepository = new ParcaRepository();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "📦 Parça / Stok Listesi";
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
            lblTitle.Text = "📦 Parça & Stok Yönetimi";
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
            txtArama.Properties.NullValuePrompt = "Parça kodu, adı veya marka ara...";
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
            grpFiltre.Size = new Size(230, 60);
            grpFiltre.AppearanceCaption.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            var btnDusukStok = new SimpleButton();
            btnDusukStok.Text = "⚠️ Düşük Stok";
            btnDusukStok.Location = new Point(10, 24);
            btnDusukStok.Size = new Size(110, 28);
            btnDusukStok.Appearance.BackColor = AppColors.Warning;
            btnDusukStok.Appearance.ForeColor = Color.Black;
            btnDusukStok.Click += async (s, e) => await LoadLowStock();
            grpFiltre.Controls.Add(btnDusukStok);

            var btnTumu = new SimpleButton();
            btnTumu.Text = "🔄 Tümü";
            btnTumu.Location = new Point(130, 24);
            btnTumu.Size = new Size(90, 28);
            btnTumu.Click += (s, e) =>
            {
                txtArama.Text = "";
                LoadData();
            };
            grpFiltre.Controls.Add(btnTumu);

            panelToolbar.Controls.Add(grpFiltre);

            // İşlemler Grubu
            var grpIslemler = new GroupControl();
            grpIslemler.Text = "⚡ İşlemler";
            grpIslemler.Location = new Point(640, 5);
            grpIslemler.Size = new Size(620, 60);
            grpIslemler.AppearanceCaption.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            var btnYeniParca = new SimpleButton();
            btnYeniParca.Text = "➕ Yeni Parça";
            btnYeniParca.Location = new Point(10, 24);
            btnYeniParca.Size = new Size(120, 28);
            btnYeniParca.Appearance.BackColor = AppColors.Success;
            btnYeniParca.Appearance.ForeColor = Color.White;
            btnYeniParca.Click += BtnYeniParca_Click;
            grpIslemler.Controls.Add(btnYeniParca);

            var btnDuzenle = new SimpleButton();
            btnDuzenle.Text = "✏ Düzenle";
            btnDuzenle.Location = new Point(140, 24);
            btnDuzenle.Size = new Size(100, 28);
            btnDuzenle.Appearance.BackColor = AppColors.Warning;
            btnDuzenle.Appearance.ForeColor = Color.Black;
            btnDuzenle.Click += (s, e) => EditParca();
            grpIslemler.Controls.Add(btnDuzenle);

            var btnStokGiris = new SimpleButton();
            btnStokGiris.Text = "📥 Stok Girişi";
            btnStokGiris.Location = new Point(250, 24);
            btnStokGiris.Size = new Size(110, 28);
            btnStokGiris.Appearance.BackColor = AppColors.Info;
            btnStokGiris.Appearance.ForeColor = Color.White;
            btnStokGiris.Click += (s, e) => UpdateStock(true);
            grpIslemler.Controls.Add(btnStokGiris);

            var btnStokDus = new SimpleButton();
            btnStokDus.Text = "📤 Stok Düş";
            btnStokDus.Location = new Point(370, 24);
            btnStokDus.Size = new Size(110, 28);
            btnStokDus.Appearance.BackColor = AppColors.Danger;
            btnStokDus.Appearance.ForeColor = Color.White;
            btnStokDus.Click += (s, e) => UpdateStock(false);
            grpIslemler.Controls.Add(btnStokDus);

            var btnSil = new SimpleButton();
            btnSil.Text = "🗑 Sil";
            btnSil.Location = new Point(490, 24);
            btnSil.Size = new Size(70, 28);
            btnSil.Appearance.BackColor = AppColors.Danger;
            btnSil.Appearance.ForeColor = Color.White;
            btnSil.Click += (s, e) => DeleteParca();
            grpIslemler.Controls.Add(btnSil);

            var btnYenile = new SimpleButton();
            btnYenile.Text = "🔄";
            btnYenile.Location = new Point(570, 24);
            btnYenile.Size = new Size(40, 28);
            btnYenile.ToolTip = "Yenile";
            btnYenile.Click += (s, e) => LoadData();
            grpIslemler.Controls.Add(btnYenile);

            panelToolbar.Controls.Add(grpIslemler);

            // Grid
            _gridParcalar = new GridControl();
            _gridParcalar.Dock = DockStyle.Fill;
            _gridView = new GridView(_gridParcalar);
            _gridParcalar.MainView = _gridView;

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
            menuDuzenle.ItemClick += (s, e) => EditParca();
            popupMenu.AddItem(menuDuzenle);

            var menuStokGiris = new BarButtonItem(barManager, "📥 Stok Girişi");
            menuStokGiris.ItemClick += (s, e) => UpdateStock(true);
            popupMenu.AddItem(menuStokGiris);

            var menuStokCikis = new BarButtonItem(barManager, "📤 Stok Çıkışı");
            menuStokCikis.ItemClick += (s, e) => UpdateStock(false);
            popupMenu.AddItem(menuStokCikis);

            var menuSil = new BarButtonItem(barManager, "🗑 Sil");
            menuSil.ItemClick += (s, e) => DeleteParca();
            popupMenu.AddItem(menuSil);

            _gridView.PopupMenuShowing += (s, e) =>
            {
                if (e.MenuType == GridMenuType.Row)
                    popupMenu.ShowPopup(barManager, MousePosition);
            };

            // Stok uyarı renklendirmesi
            _gridView.RowCellStyle += (s, e) =>
            {
                if (e.Column.FieldName == "StokMiktari")
                {
                    var row = _gridView.GetRow(e.RowHandle) as Parca;
                    if (row != null && row.StokMiktari <= row.MinStokMiktari)
                    {
                        e.Appearance.ForeColor = AppColors.Danger;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                    else if (row != null)
                    {
                        e.Appearance.ForeColor = AppColors.Success;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                }
            };

            panelMain.Controls.Add(_gridParcalar);
            panelMain.Controls.Add(panelToolbar);
            panelMain.Controls.Add(panelHeader);
            this.Controls.Add(panelMain);

            this.ResumeLayout(false);
        }

        private async void LoadData()
        {
            try
            {
                var parcalar = await _parcaRepository.GetAllAsync();
                _gridParcalar.DataSource = parcalar;
                ConfigureGridColumns();

                var lblSubtitle = this.Controls.Find("lblSubtitle", true);
                if (lblSubtitle.Length > 0 && lblSubtitle[0] is LabelControl lbl)
                    lbl.Text = $"Toplam {parcalar.Count} parça listeleniyor";
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
            if (_gridView.Columns["Aciklama"] != null)
                _gridView.Columns["Aciklama"].Visible = false;
            if (_gridView.Columns["KayitTarihi"] != null)
                _gridView.Columns["KayitTarihi"].Visible = false;

            if (_gridView.Columns["ParcaKodu"] != null)
            {
                _gridView.Columns["ParcaKodu"].Caption = "Parça Kodu";
                _gridView.Columns["ParcaKodu"].VisibleIndex = 0;
                _gridView.Columns["ParcaKodu"].Width = 120;
            }
            if (_gridView.Columns["ParcaAdi"] != null)
            {
                _gridView.Columns["ParcaAdi"].Caption = "Parça Adı";
                _gridView.Columns["ParcaAdi"].VisibleIndex = 1;
                _gridView.Columns["ParcaAdi"].Width = 200;
            }
            if (_gridView.Columns["Kategori"] != null)
            {
                _gridView.Columns["Kategori"].Caption = "Kategori";
                _gridView.Columns["Kategori"].VisibleIndex = 2;
                _gridView.Columns["Kategori"].Width = 100;
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
            if (_gridView.Columns["StokMiktari"] != null)
            {
                _gridView.Columns["StokMiktari"].Caption = "Stok";
                _gridView.Columns["StokMiktari"].VisibleIndex = 5;
                _gridView.Columns["StokMiktari"].Width = 70;
            }
            if (_gridView.Columns["MinStokMiktari"] != null)
            {
                _gridView.Columns["MinStokMiktari"].Caption = "Min. Stok";
                _gridView.Columns["MinStokMiktari"].VisibleIndex = 6;
                _gridView.Columns["MinStokMiktari"].Width = 70;
            }
            if (_gridView.Columns["AlisFiyati"] != null)
            {
                _gridView.Columns["AlisFiyati"].Caption = "Alış Fiyatı";
                _gridView.Columns["AlisFiyati"].DisplayFormat.FormatType = FormatType.Numeric;
                _gridView.Columns["AlisFiyati"].DisplayFormat.FormatString = "₺{0:N2}";
                _gridView.Columns["AlisFiyati"].VisibleIndex = 7;
                _gridView.Columns["AlisFiyati"].Width = 100;
            }
            if (_gridView.Columns["SatisFiyati"] != null)
            {
                _gridView.Columns["SatisFiyati"].Caption = "Satış Fiyatı";
                _gridView.Columns["SatisFiyati"].DisplayFormat.FormatType = FormatType.Numeric;
                _gridView.Columns["SatisFiyati"].DisplayFormat.FormatString = "₺{0:N2}";
                _gridView.Columns["SatisFiyati"].VisibleIndex = 8;
                _gridView.Columns["SatisFiyati"].Width = 100;
            }
            if (_gridView.Columns["Birim"] != null)
            {
                _gridView.Columns["Birim"].Caption = "Birim";
                _gridView.Columns["Birim"].VisibleIndex = 9;
                _gridView.Columns["Birim"].Width = 70;
            }
            if (_gridView.Columns["Konum"] != null)
            {
                _gridView.Columns["Konum"].Caption = "Konum";
                _gridView.Columns["Konum"].VisibleIndex = 10;
                _gridView.Columns["Konum"].Width = 100;
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
                var parcalar = await _parcaRepository.SearchAsync(searchTerm);
                _gridParcalar.DataSource = parcalar;

                var lblSubtitle = this.Controls.Find("lblSubtitle", true);
                if (lblSubtitle.Length > 0 && lblSubtitle[0] is LabelControl lbl)
                    lbl.Text = $"'{searchTerm}' için {parcalar.Count} sonuç bulundu";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Arama yapılırken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadLowStock()
        {
            try
            {
                var parcalar = await _parcaRepository.GetLowStockAsync();
                _gridParcalar.DataSource = parcalar;

                var lblSubtitle = this.Controls.Find("lblSubtitle", true);
                if (lblSubtitle.Length > 0 && lblSubtitle[0] is LabelControl lbl)
                    lbl.Text = $"⚠️ Düşük stoklu {parcalar.Count} parça listeleniyor";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Veriler yüklenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnYeniParca_Click(object? sender, EventArgs e)
        {
            var form = new ParcaForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void GridView_DoubleClick(object? sender, EventArgs e)
        {
            EditParca();
        }

        private void EditParca()
        {
            var parca = _gridView.GetFocusedRow() as Parca;
            if (parca == null) return;

            var form = new ParcaForm(parca.Id);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private async void UpdateStock(bool isIncrease)
        {
            var selectedRow = _gridView.GetFocusedRow() as Parca;
            if (selectedRow == null)
            {
                XtraMessageBox.Show("Lütfen stok işlemi yapmak için bir parça seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string islemAdi = isIncrease ? "Stok Girişi" : "Stok Düşme";
            string prompt = isIncrease 
                ? $"'{selectedRow.ParcaAdi}' için eklenecek miktarı girin:"
                : $"'{selectedRow.ParcaAdi}' için düşülecek miktarı girin:";

            string? result = XtraInputBox.Show(prompt, islemAdi, "1");
            
            if (!string.IsNullOrEmpty(result) && int.TryParse(result, out int miktar) && miktar > 0)
            {
                try
                {
                    if (!isIncrease && selectedRow.StokMiktari < miktar)
                    {
                         XtraMessageBox.Show($"Yetersiz stok! Mevcut stok: {selectedRow.StokMiktari}", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                         return;
                    }

                    int updateAmount = isIncrease ? miktar : -miktar;
                    var success = await _parcaRepository.UpdateStokAsync(selectedRow.Id, updateAmount);

                    if (success)
                    {
                        var yeniMiktar = selectedRow.StokMiktari + updateAmount;
                        XtraMessageBox.Show(
                            $"{islemAdi} başarılı!\n" +
                            $"Eski Stok: {selectedRow.StokMiktari}\n" +
                            $"Yeni Stok: {yeniMiktar}",
                            "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        LoadData(); // Listeyi yenile
                    }
                    else
                    {
                        XtraMessageBox.Show("Stok güncellenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void DeleteParca()
        {
            var parca = _gridView.GetFocusedRow() as Parca;
            if (parca == null) return;

            var result = XtraMessageBox.Show($"{parca.ParcaAdi} parçasını silmek istediğinize emin misiniz?",
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _parcaRepository.DeleteAsync(parca.Id);
                    XtraMessageBox.Show("Parça silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

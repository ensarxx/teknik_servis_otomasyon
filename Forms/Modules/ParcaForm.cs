using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;
using TeknikServisOtomasyon.Data.Repositories;
using TeknikServisOtomasyon.Helpers;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Forms.Modules
{
    public partial class ParcaForm : XtraForm
    {
        private readonly ParcaRepository _parcaRepository;
        private int? _parcaId;
        private Parca? _parca;

        public ParcaForm(int? parcaId = null)
        {
            _parcaRepository = new ParcaRepository();
            _parcaId = parcaId;

            InitializeComponent();

            if (_parcaId.HasValue)
            {
                LoadParcaData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = _parcaId.HasValue ? "Parça Düzenle" : "Yeni Parça";
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var panelMain = new PanelControl();
            panelMain.Dock = DockStyle.Fill;
            panelMain.Padding = new Padding(20);

            int y = 20;
            int labelX = 20;
            int inputX = 130;
            int inputWidth = 320;

            // Parça Kodu
            var lblParcaKodu = new LabelControl { Text = "Parça Kodu *", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblParcaKodu);

            var txtParcaKodu = new TextEdit();
            txtParcaKodu.Name = "txtParcaKodu";
            txtParcaKodu.Location = new Point(inputX, y);
            txtParcaKodu.Size = new Size(inputWidth, 25);
            panelMain.Controls.Add(txtParcaKodu);

            y += 40;

            // Parça Adı
            var lblParcaAdi = new LabelControl { Text = "Parça Adı *", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblParcaAdi);

            var txtParcaAdi = new TextEdit();
            txtParcaAdi.Name = "txtParcaAdi";
            txtParcaAdi.Location = new Point(inputX, y);
            txtParcaAdi.Size = new Size(inputWidth, 25);
            panelMain.Controls.Add(txtParcaAdi);

            y += 40;

            // Kategori
            var lblKategori = new LabelControl { Text = "Kategori", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblKategori);

            var cmbKategori = new ComboBoxEdit();
            cmbKategori.Name = "cmbKategori";
            cmbKategori.Location = new Point(inputX, y);
            cmbKategori.Size = new Size(inputWidth, 25);
            cmbKategori.Properties.Items.AddRange(new[] { "Ekran", "Batarya", "Şarj Soketi", "Kamera", "Hoparlör", "Mikrofon", "Anakart", "RAM", "SSD/HDD", "Klavye", "Touchpad", "Kasa", "Diğer" });
            cmbKategori.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            panelMain.Controls.Add(cmbKategori);

            y += 40;

            // Marka
            var lblMarka = new LabelControl { Text = "Marka", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblMarka);

            var txtMarka = new TextEdit();
            txtMarka.Name = "txtMarka";
            txtMarka.Location = new Point(inputX, y);
            txtMarka.Size = new Size(150, 25);
            panelMain.Controls.Add(txtMarka);

            // Model
            var lblModel = new LabelControl { Text = "Model", Location = new Point(300, y + 3) };
            panelMain.Controls.Add(lblModel);

            var txtModel = new TextEdit();
            txtModel.Name = "txtModel";
            txtModel.Location = new Point(350, y);
            txtModel.Size = new Size(100, 25);
            panelMain.Controls.Add(txtModel);

            y += 40;

            // Stok Miktarı
            var lblStokMiktari = new LabelControl { Text = "Stok Miktarı", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblStokMiktari);

            var txtStokMiktari = new SpinEdit();
            txtStokMiktari.Name = "txtStokMiktari";
            txtStokMiktari.Location = new Point(inputX, y);
            txtStokMiktari.Size = new Size(100, 25);
            txtStokMiktari.Properties.IsFloatValue = false;
            txtStokMiktari.Properties.MinValue = 0;
            panelMain.Controls.Add(txtStokMiktari);

            // Min Stok
            var lblMinStok = new LabelControl { Text = "Min. Stok", Location = new Point(260, y + 3) };
            panelMain.Controls.Add(lblMinStok);

            var txtMinStok = new SpinEdit();
            txtMinStok.Name = "txtMinStok";
            txtMinStok.Location = new Point(330, y);
            txtMinStok.Size = new Size(80, 25);
            txtMinStok.Properties.IsFloatValue = false;
            txtMinStok.Properties.MinValue = 0;
            txtMinStok.Value = 5;
            panelMain.Controls.Add(txtMinStok);

            y += 40;

            // Alış Fiyatı
            var lblAlisFiyati = new LabelControl { Text = "Alış Fiyatı", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblAlisFiyati);

            var txtAlisFiyati = new SpinEdit();
            txtAlisFiyati.Name = "txtAlisFiyati";
            txtAlisFiyati.Location = new Point(inputX, y);
            txtAlisFiyati.Size = new Size(120, 25);
            txtAlisFiyati.Properties.DisplayFormat.FormatString = "₺{0:N2}";
            txtAlisFiyati.Properties.EditFormat.FormatString = "N2";
            panelMain.Controls.Add(txtAlisFiyati);

            // Satış Fiyatı
            var lblSatisFiyati = new LabelControl { Text = "Satış Fiyatı", Location = new Point(280, y + 3) };
            panelMain.Controls.Add(lblSatisFiyati);

            var txtSatisFiyati = new SpinEdit();
            txtSatisFiyati.Name = "txtSatisFiyati";
            txtSatisFiyati.Location = new Point(360, y);
            txtSatisFiyati.Size = new Size(100, 25);
            txtSatisFiyati.Properties.DisplayFormat.FormatString = "₺{0:N2}";
            txtSatisFiyati.Properties.EditFormat.FormatString = "N2";
            panelMain.Controls.Add(txtSatisFiyati);

            y += 40;

            // Birim
            var lblBirim = new LabelControl { Text = "Birim", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblBirim);

            var cmbBirim = new ComboBoxEdit();
            cmbBirim.Name = "cmbBirim";
            cmbBirim.Location = new Point(inputX, y);
            cmbBirim.Size = new Size(100, 25);
            cmbBirim.Properties.Items.AddRange(new[] { "Adet", "Metre", "Kg", "Lt" });
            cmbBirim.SelectedIndex = 0;
            panelMain.Controls.Add(cmbBirim);

            // Konum
            var lblKonum = new LabelControl { Text = "Konum", Location = new Point(260, y + 3) };
            panelMain.Controls.Add(lblKonum);

            var txtKonum = new TextEdit();
            txtKonum.Name = "txtKonum";
            txtKonum.Location = new Point(310, y);
            txtKonum.Size = new Size(150, 25);
            panelMain.Controls.Add(txtKonum);

            y += 40;

            // Açıklama
            var lblAciklama = new LabelControl { Text = "Açıklama", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblAciklama);

            var txtAciklama = new MemoEdit();
            txtAciklama.Name = "txtAciklama";
            txtAciklama.Location = new Point(inputX, y);
            txtAciklama.Size = new Size(inputWidth, 60);
            panelMain.Controls.Add(txtAciklama);

            y += 90;

            // Butonlar
            var btnKaydet = new SimpleButton();
            btnKaydet.Text = "Kaydet";
            btnKaydet.Location = new Point(250, y);
            btnKaydet.Size = new Size(100, 35);
            btnKaydet.Appearance.BackColor = AppColors.Success;
            btnKaydet.Appearance.ForeColor = Color.White;
            btnKaydet.Click += BtnKaydet_Click;
            panelMain.Controls.Add(btnKaydet);

            var btnIptal = new SimpleButton();
            btnIptal.Text = "İptal";
            btnIptal.Location = new Point(360, y);
            btnIptal.Size = new Size(100, 35);
            btnIptal.Click += (s, e) => this.Close();
            panelMain.Controls.Add(btnIptal);

            this.Controls.Add(panelMain);
            this.ResumeLayout(false);
        }

        private async void LoadParcaData()
        {
            try
            {
                _parca = await _parcaRepository.GetByIdAsync(_parcaId!.Value);
                if (_parca == null)
                {
                    XtraMessageBox.Show("Parça bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                var txtParcaKodu = this.Controls.Find("txtParcaKodu", true)[0] as TextEdit;
                txtParcaKodu!.Text = _parca.ParcaKodu;

                var txtParcaAdi = this.Controls.Find("txtParcaAdi", true)[0] as TextEdit;
                txtParcaAdi!.Text = _parca.ParcaAdi;

                var cmbKategori = this.Controls.Find("cmbKategori", true)[0] as ComboBoxEdit;
                cmbKategori!.Text = _parca.Kategori;

                var txtMarka = this.Controls.Find("txtMarka", true)[0] as TextEdit;
                txtMarka!.Text = _parca.Marka;

                var txtModel = this.Controls.Find("txtModel", true)[0] as TextEdit;
                txtModel!.Text = _parca.Model;

                var txtStokMiktari = this.Controls.Find("txtStokMiktari", true)[0] as SpinEdit;
                txtStokMiktari!.Value = _parca.StokMiktari;

                var txtMinStok = this.Controls.Find("txtMinStok", true)[0] as SpinEdit;
                txtMinStok!.Value = _parca.MinStokMiktari;

                var txtAlisFiyati = this.Controls.Find("txtAlisFiyati", true)[0] as SpinEdit;
                txtAlisFiyati!.Value = _parca.AlisFiyati;

                var txtSatisFiyati = this.Controls.Find("txtSatisFiyati", true)[0] as SpinEdit;
                txtSatisFiyati!.Value = _parca.SatisFiyati;

                var cmbBirim = this.Controls.Find("cmbBirim", true)[0] as ComboBoxEdit;
                cmbBirim!.Text = _parca.Birim;

                var txtKonum = this.Controls.Find("txtKonum", true)[0] as TextEdit;
                txtKonum!.Text = _parca.Konum;

                var txtAciklama = this.Controls.Find("txtAciklama", true)[0] as MemoEdit;
                txtAciklama!.Text = _parca.Aciklama;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Parça bilgileri yüklenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnKaydet_Click(object? sender, EventArgs e)
        {
            try
            {
                var txtParcaKodu = this.Controls.Find("txtParcaKodu", true)[0] as TextEdit;
                var txtParcaAdi = this.Controls.Find("txtParcaAdi", true)[0] as TextEdit;

                if (string.IsNullOrWhiteSpace(txtParcaKodu?.Text))
                {
                    XtraMessageBox.Show("Parça kodu boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtParcaKodu?.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtParcaAdi?.Text))
                {
                    XtraMessageBox.Show("Parça adı boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtParcaAdi?.Focus();
                    return;
                }

                // Parça kodu kontrolü
                if (await _parcaRepository.ParcaKoduMevcutMuAsync(txtParcaKodu.Text, _parcaId))
                {
                    XtraMessageBox.Show("Bu parça kodu zaten kullanılıyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtParcaKodu.Focus();
                    return;
                }

                var cmbKategori = this.Controls.Find("cmbKategori", true)[0] as ComboBoxEdit;
                var txtMarka = this.Controls.Find("txtMarka", true)[0] as TextEdit;
                var txtModel = this.Controls.Find("txtModel", true)[0] as TextEdit;
                var txtStokMiktari = this.Controls.Find("txtStokMiktari", true)[0] as SpinEdit;
                var txtMinStok = this.Controls.Find("txtMinStok", true)[0] as SpinEdit;
                var txtAlisFiyati = this.Controls.Find("txtAlisFiyati", true)[0] as SpinEdit;
                var txtSatisFiyati = this.Controls.Find("txtSatisFiyati", true)[0] as SpinEdit;
                var cmbBirim = this.Controls.Find("cmbBirim", true)[0] as ComboBoxEdit;
                var txtKonum = this.Controls.Find("txtKonum", true)[0] as TextEdit;
                var txtAciklama = this.Controls.Find("txtAciklama", true)[0] as MemoEdit;

                var parca = _parca ?? new Parca();
                parca.ParcaKodu = txtParcaKodu.Text.Trim();
                parca.ParcaAdi = txtParcaAdi.Text.Trim();
                parca.Kategori = cmbKategori?.Text ?? "";
                parca.Marka = txtMarka?.Text ?? "";
                parca.Model = txtModel?.Text ?? "";
                parca.StokMiktari = (int)(txtStokMiktari?.Value ?? 0);
                parca.MinStokMiktari = (int)(txtMinStok?.Value ?? 5);
                parca.AlisFiyati = txtAlisFiyati?.Value ?? 0;
                parca.SatisFiyati = txtSatisFiyati?.Value ?? 0;
                parca.Birim = cmbBirim?.Text ?? "Adet";
                parca.Konum = txtKonum?.Text ?? "";
                parca.Aciklama = txtAciklama?.Text ?? "";

                if (_parcaId.HasValue)
                {
                    await _parcaRepository.UpdateAsync(parca);
                    XtraMessageBox.Show("Parça güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await _parcaRepository.InsertAsync(parca);
                    XtraMessageBox.Show("Parça kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Kayıt sırasında hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

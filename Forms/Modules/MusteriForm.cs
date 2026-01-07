using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;
using TeknikServisOtomasyon.Data.Repositories;
using TeknikServisOtomasyon.Helpers;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Forms.Modules
{
    public partial class MusteriForm : XtraForm
    {
        private readonly MusteriRepository _musteriRepository;
        private int? _musteriId;
        private Musteri? _musteri;

        public MusteriForm(int? musteriId = null)
        {
            _musteriRepository = new MusteriRepository();
            _musteriId = musteriId;

            InitializeComponent();

            if (_musteriId.HasValue)
            {
                LoadMusteriData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = _musteriId.HasValue ? "Müşteri Düzenle" : "Yeni Müşteri";
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Ana Panel
            var panelMain = new PanelControl();
            panelMain.Dock = DockStyle.Fill;
            panelMain.Padding = new Padding(20);

            int y = 20;
            int labelX = 20;
            int inputX = 130;
            int inputWidth = 320;

            // Ad Soyad
            var lblAdSoyad = new LabelControl { Text = "Ad Soyad *", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblAdSoyad);

            var txtAdSoyad = new TextEdit();
            txtAdSoyad.Name = "txtAdSoyad";
            txtAdSoyad.Location = new Point(inputX, y);
            txtAdSoyad.Size = new Size(inputWidth, 25);
            panelMain.Controls.Add(txtAdSoyad);

            y += 40;

            // Telefon
            var lblTelefon = new LabelControl { Text = "Telefon *", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblTelefon);

            var txtTelefon = new TextEdit();
            txtTelefon.Name = "txtTelefon";
            txtTelefon.Location = new Point(inputX, y);
            txtTelefon.Size = new Size(inputWidth, 25);
            txtTelefon.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
            txtTelefon.Properties.Mask.EditMask = "(000) 000-0000";
            txtTelefon.Properties.Mask.UseMaskAsDisplayFormat = true;
            panelMain.Controls.Add(txtTelefon);

            y += 40;

            // Telefon 2
            var lblTelefon2 = new LabelControl { Text = "Telefon 2", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblTelefon2);

            var txtTelefon2 = new TextEdit();
            txtTelefon2.Name = "txtTelefon2";
            txtTelefon2.Location = new Point(inputX, y);
            txtTelefon2.Size = new Size(inputWidth, 25);
            txtTelefon2.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
            txtTelefon2.Properties.Mask.EditMask = "(000) 000-0000";
            txtTelefon2.Properties.Mask.UseMaskAsDisplayFormat = true;
            panelMain.Controls.Add(txtTelefon2);

            y += 40;

            // Email
            var lblEmail = new LabelControl { Text = "E-posta", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblEmail);

            var txtEmail = new TextEdit();
            txtEmail.Name = "txtEmail";
            txtEmail.Location = new Point(inputX, y);
            txtEmail.Size = new Size(inputWidth, 25);
            panelMain.Controls.Add(txtEmail);

            y += 40;

            // TC Kimlik No
            var lblTCKimlikNo = new LabelControl { Text = "TC Kimlik No", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblTCKimlikNo);

            var txtTCKimlikNo = new TextEdit();
            txtTCKimlikNo.Name = "txtTCKimlikNo";
            txtTCKimlikNo.Location = new Point(inputX, y);
            txtTCKimlikNo.Size = new Size(inputWidth, 25);
            txtTCKimlikNo.Properties.MaxLength = 11;
            panelMain.Controls.Add(txtTCKimlikNo);

            y += 40;

            // Adres
            var lblAdres = new LabelControl { Text = "Adres", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblAdres);

            var txtAdres = new MemoEdit();
            txtAdres.Name = "txtAdres";
            txtAdres.Location = new Point(inputX, y);
            txtAdres.Size = new Size(inputWidth, 60);
            panelMain.Controls.Add(txtAdres);

            y += 80;

            // Notlar
            var lblNotlar = new LabelControl { Text = "Notlar", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblNotlar);

            var txtNotlar = new MemoEdit();
            txtNotlar.Name = "txtNotlar";
            txtNotlar.Location = new Point(inputX, y);
            txtNotlar.Size = new Size(inputWidth, 60);
            panelMain.Controls.Add(txtNotlar);

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

        private async void LoadMusteriData()
        {
            try
            {
                _musteri = await _musteriRepository.GetByIdAsync(_musteriId!.Value);
                if (_musteri == null)
                {
                    XtraMessageBox.Show("Müşteri bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Form alanlarını doldur
                var txtAdSoyad = this.Controls.Find("txtAdSoyad", true)[0] as TextEdit;
                txtAdSoyad!.Text = _musteri.AdSoyad;

                var txtTelefon = this.Controls.Find("txtTelefon", true)[0] as TextEdit;
                txtTelefon!.Text = _musteri.Telefon;

                var txtTelefon2 = this.Controls.Find("txtTelefon2", true)[0] as TextEdit;
                txtTelefon2!.Text = _musteri.Telefon2;

                var txtEmail = this.Controls.Find("txtEmail", true)[0] as TextEdit;
                txtEmail!.Text = _musteri.Email;

                var txtTCKimlikNo = this.Controls.Find("txtTCKimlikNo", true)[0] as TextEdit;
                txtTCKimlikNo!.Text = _musteri.TCKimlikNo;

                var txtAdres = this.Controls.Find("txtAdres", true)[0] as MemoEdit;
                txtAdres!.Text = _musteri.Adres;

                var txtNotlar = this.Controls.Find("txtNotlar", true)[0] as MemoEdit;
                txtNotlar!.Text = _musteri.Notlar;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Müşteri bilgileri yüklenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnKaydet_Click(object? sender, EventArgs e)
        {
            try
            {
                // Validasyon
                var txtAdSoyad = this.Controls.Find("txtAdSoyad", true)[0] as TextEdit;
                var txtTelefon = this.Controls.Find("txtTelefon", true)[0] as TextEdit;

                if (string.IsNullOrWhiteSpace(txtAdSoyad?.Text))
                {
                    XtraMessageBox.Show("Ad Soyad boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtAdSoyad?.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTelefon?.Text))
                {
                    XtraMessageBox.Show("Telefon boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTelefon?.Focus();
                    return;
                }

                var txtTelefon2 = this.Controls.Find("txtTelefon2", true)[0] as TextEdit;
                var txtEmail = this.Controls.Find("txtEmail", true)[0] as TextEdit;
                var txtTCKimlikNo = this.Controls.Find("txtTCKimlikNo", true)[0] as TextEdit;
                var txtAdres = this.Controls.Find("txtAdres", true)[0] as MemoEdit;
                var txtNotlar = this.Controls.Find("txtNotlar", true)[0] as MemoEdit;

                var musteri = _musteri ?? new Musteri();
                musteri.AdSoyad = txtAdSoyad.Text.Trim();
                musteri.Telefon = txtTelefon.Text.Trim();
                musteri.Telefon2 = txtTelefon2?.Text?.Trim() ?? "";
                musteri.Email = txtEmail?.Text?.Trim() ?? "";
                musteri.TCKimlikNo = txtTCKimlikNo?.Text?.Trim() ?? "";
                musteri.Adres = txtAdres?.Text?.Trim() ?? "";
                musteri.Notlar = txtNotlar?.Text?.Trim() ?? "";
                musteri.Aktif = true;

                if (_musteriId.HasValue)
                {
                    await _musteriRepository.UpdateAsync(musteri);
                    XtraMessageBox.Show("Müşteri güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await _musteriRepository.InsertAsync(musteri);
                    XtraMessageBox.Show("Müşteri kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

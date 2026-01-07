using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;
using TeknikServisOtomasyon.Data.Repositories;
using TeknikServisOtomasyon.Helpers;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Forms.Modules
{
    public partial class KullaniciForm : XtraForm
    {
        private readonly KullaniciRepository _kullaniciRepository;
        private int? _kullaniciId;
        private Kullanici? _kullanici;

        public KullaniciForm(int? kullaniciId = null)
        {
            _kullaniciRepository = new KullaniciRepository();
            _kullaniciId = kullaniciId;

            InitializeComponent();

            if (_kullaniciId.HasValue)
            {
                LoadKullaniciData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = _kullaniciId.HasValue ? "Kullanıcı Düzenle" : "Yeni Kullanıcı";
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var panelMain = new PanelControl();
            panelMain.Dock = DockStyle.Fill;
            panelMain.Padding = new Padding(20);

            int y = 20;
            int labelX = 20;
            int inputX = 130;
            int inputWidth = 270;

            // Kullanıcı Adı
            var lblKullaniciAdi = new LabelControl { Text = "Kullanıcı Adı *", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblKullaniciAdi);

            var txtKullaniciAdi = new TextEdit();
            txtKullaniciAdi.Name = "txtKullaniciAdi";
            txtKullaniciAdi.Location = new Point(inputX, y);
            txtKullaniciAdi.Size = new Size(inputWidth, 25);
            panelMain.Controls.Add(txtKullaniciAdi);

            y += 40;

            // Şifre (sadece yeni kullanıcı için)
            if (!_kullaniciId.HasValue)
            {
                var lblSifre = new LabelControl { Text = "Şifre *", Location = new Point(labelX, y + 3) };
                panelMain.Controls.Add(lblSifre);

                var txtSifre = new TextEdit();
                txtSifre.Name = "txtSifre";
                txtSifre.Location = new Point(inputX, y);
                txtSifre.Size = new Size(inputWidth, 25);
                txtSifre.Properties.PasswordChar = '●';
                panelMain.Controls.Add(txtSifre);

                y += 40;
            }

            // Ad Soyad
            var lblAdSoyad = new LabelControl { Text = "Ad Soyad *", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblAdSoyad);

            var txtAdSoyad = new TextEdit();
            txtAdSoyad.Name = "txtAdSoyad";
            txtAdSoyad.Location = new Point(inputX, y);
            txtAdSoyad.Size = new Size(inputWidth, 25);
            panelMain.Controls.Add(txtAdSoyad);

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

            // Telefon
            var lblTelefon = new LabelControl { Text = "Telefon", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblTelefon);

            var txtTelefon = new TextEdit();
            txtTelefon.Name = "txtTelefon";
            txtTelefon.Location = new Point(inputX, y);
            txtTelefon.Size = new Size(inputWidth, 25);
            panelMain.Controls.Add(txtTelefon);

            y += 40;

            // Rol
            var lblRol = new LabelControl { Text = "Rol *", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblRol);

            var cmbRol = new ComboBoxEdit();
            cmbRol.Name = "cmbRol";
            cmbRol.Location = new Point(inputX, y);
            cmbRol.Size = new Size(inputWidth, 25);
            cmbRol.Properties.Items.AddRange(new[] { "Admin", "Tekniker", "Kasiyer" });
            cmbRol.SelectedIndex = 1;
            panelMain.Controls.Add(cmbRol);

            y += 60;

            // Butonlar
            var btnKaydet = new SimpleButton();
            btnKaydet.Text = "Kaydet";
            btnKaydet.Location = new Point(200, y);
            btnKaydet.Size = new Size(100, 35);
            btnKaydet.Appearance.BackColor = AppColors.Success;
            btnKaydet.Appearance.ForeColor = Color.White;
            btnKaydet.Click += BtnKaydet_Click;
            panelMain.Controls.Add(btnKaydet);

            var btnIptal = new SimpleButton();
            btnIptal.Text = "İptal";
            btnIptal.Location = new Point(310, y);
            btnIptal.Size = new Size(100, 35);
            btnIptal.Click += (s, e) => this.Close();
            panelMain.Controls.Add(btnIptal);

            this.Controls.Add(panelMain);
            this.ResumeLayout(false);
        }

        private async void LoadKullaniciData()
        {
            try
            {
                _kullanici = await _kullaniciRepository.GetByIdAsync(_kullaniciId!.Value);
                if (_kullanici == null)
                {
                    XtraMessageBox.Show("Kullanıcı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                var txtKullaniciAdi = this.Controls.Find("txtKullaniciAdi", true)[0] as TextEdit;
                txtKullaniciAdi!.Text = _kullanici.KullaniciAdi;

                var txtAdSoyad = this.Controls.Find("txtAdSoyad", true)[0] as TextEdit;
                txtAdSoyad!.Text = _kullanici.AdSoyad;

                var txtEmail = this.Controls.Find("txtEmail", true)[0] as TextEdit;
                txtEmail!.Text = _kullanici.Email;

                var txtTelefon = this.Controls.Find("txtTelefon", true)[0] as TextEdit;
                txtTelefon!.Text = _kullanici.Telefon;

                var cmbRol = this.Controls.Find("cmbRol", true)[0] as ComboBoxEdit;
                cmbRol!.Text = _kullanici.Rol;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Kullanıcı bilgileri yüklenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnKaydet_Click(object? sender, EventArgs e)
        {
            try
            {
                var txtKullaniciAdi = this.Controls.Find("txtKullaniciAdi", true)[0] as TextEdit;
                var txtAdSoyad = this.Controls.Find("txtAdSoyad", true)[0] as TextEdit;
                var cmbRol = this.Controls.Find("cmbRol", true)[0] as ComboBoxEdit;

                if (string.IsNullOrWhiteSpace(txtKullaniciAdi?.Text))
                {
                    XtraMessageBox.Show("Kullanıcı adı boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtKullaniciAdi?.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtAdSoyad?.Text))
                {
                    XtraMessageBox.Show("Ad Soyad boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtAdSoyad?.Focus();
                    return;
                }

                // Kullanıcı adı kontrolü
                if (await _kullaniciRepository.KullaniciAdiMevcutMuAsync(txtKullaniciAdi.Text, _kullaniciId))
                {
                    XtraMessageBox.Show("Bu kullanıcı adı zaten kullanılıyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtKullaniciAdi.Focus();
                    return;
                }

                var txtEmail = this.Controls.Find("txtEmail", true)[0] as TextEdit;
                var txtTelefon = this.Controls.Find("txtTelefon", true)[0] as TextEdit;

                var kullanici = _kullanici ?? new Kullanici();
                kullanici.KullaniciAdi = txtKullaniciAdi.Text.Trim();
                kullanici.AdSoyad = txtAdSoyad.Text.Trim();
                kullanici.Email = txtEmail?.Text?.Trim() ?? "";
                kullanici.Telefon = txtTelefon?.Text?.Trim() ?? "";
                kullanici.Rol = cmbRol?.Text ?? "Tekniker";

                if (_kullaniciId.HasValue)
                {
                    await _kullaniciRepository.UpdateAsync(kullanici);
                    XtraMessageBox.Show("Kullanıcı güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var txtSifre = this.Controls.Find("txtSifre", true);
                    if (txtSifre.Length > 0)
                    {
                        var sifre = (txtSifre[0] as TextEdit)?.Text;
                        if (string.IsNullOrWhiteSpace(sifre))
                        {
                            XtraMessageBox.Show("Şifre boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        kullanici.Sifre = sifre;
                    }

                    await _kullaniciRepository.InsertAsync(kullanici);
                    XtraMessageBox.Show("Kullanıcı kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

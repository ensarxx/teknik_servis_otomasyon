using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;
using TeknikServisOtomasyon.Data.Repositories;
using TeknikServisOtomasyon.Helpers;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Forms.Modules
{
    public partial class EmailAyarlariForm : XtraForm
    {
        private readonly EmailAyarlariRepository _repository;
        private TextEdit _txtSmtpServer = null!;
        private SpinEdit _txtSmtpPort = null!;
        private TextEdit _txtGondericiEmail = null!;
        private TextEdit _txtGondericiSifre = null!;
        private TextEdit _txtGondericiAdi = null!;
        private CheckEdit _chkSslKullan = null!;
        private CheckEdit _chkEmailAktif = null!;

        public EmailAyarlariForm()
        {
            _repository = new EmailAyarlariRepository();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "📧 E-Posta Ayarları";
            this.Size = new Size(550, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var panelMain = new XtraScrollableControl();
            panelMain.Dock = DockStyle.Fill;
            panelMain.Padding = new Padding(20);

            // Başlık
            var lblBaslik = new LabelControl();
            lblBaslik.Text = "📧 E-POSTA AYARLARI";
            lblBaslik.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblBaslik.ForeColor = AppColors.Primary;
            lblBaslik.Location = new Point(20, 10);
            panelMain.Controls.Add(lblBaslik);

            var lblAciklama = new LabelControl();
            lblAciklama.Text = "Gmail için 'Uygulama Şifresi' kullanmanız gerekir.\nGoogle Hesabı > Güvenlik > 2 Adımlı Doğrulama > Uygulama Şifreleri";
            lblAciklama.Location = new Point(20, 45);
            lblAciklama.ForeColor = Color.Gray;
            panelMain.Controls.Add(lblAciklama);

            int y = 100;
            int labelWidth = 140;
            int inputWidth = 320;

            // SMTP Sunucu
            var lblSmtp = new LabelControl();
            lblSmtp.Text = "SMTP Sunucu:";
            lblSmtp.Location = new Point(20, y);
            panelMain.Controls.Add(lblSmtp);

            _txtSmtpServer = new TextEdit();
            _txtSmtpServer.Location = new Point(20 + labelWidth, y);
            _txtSmtpServer.Size = new Size(inputWidth, 24);
            _txtSmtpServer.EditValue = "smtp.gmail.com";
            panelMain.Controls.Add(_txtSmtpServer);

            y += 40;

            // SMTP Port
            var lblPort = new LabelControl();
            lblPort.Text = "SMTP Port:";
            lblPort.Location = new Point(20, y);
            panelMain.Controls.Add(lblPort);

            _txtSmtpPort = new SpinEdit();
            _txtSmtpPort.Location = new Point(20 + labelWidth, y);
            _txtSmtpPort.Size = new Size(100, 24);
            _txtSmtpPort.Properties.MinValue = 1;
            _txtSmtpPort.Properties.MaxValue = 65535;
            _txtSmtpPort.EditValue = 587;
            panelMain.Controls.Add(_txtSmtpPort);

            y += 40;

            // Gönderici E-posta
            var lblEmail = new LabelControl();
            lblEmail.Text = "Gönderici E-posta:";
            lblEmail.Location = new Point(20, y);
            panelMain.Controls.Add(lblEmail);

            _txtGondericiEmail = new TextEdit();
            _txtGondericiEmail.Location = new Point(20 + labelWidth, y);
            _txtGondericiEmail.Size = new Size(inputWidth, 24);
            _txtGondericiEmail.Properties.NullValuePrompt = "ornek@gmail.com";
            panelMain.Controls.Add(_txtGondericiEmail);

            y += 40;

            // Şifre (App Password)
            var lblSifre = new LabelControl();
            lblSifre.Text = "Uygulama Şifresi:";
            lblSifre.Location = new Point(20, y);
            panelMain.Controls.Add(lblSifre);

            _txtGondericiSifre = new TextEdit();
            _txtGondericiSifre.Location = new Point(20 + labelWidth, y);
            _txtGondericiSifre.Size = new Size(inputWidth, 24);
            _txtGondericiSifre.Properties.PasswordChar = '*';
            _txtGondericiSifre.Properties.NullValuePrompt = "16 haneli uygulama şifresi";
            panelMain.Controls.Add(_txtGondericiSifre);

            y += 40;

            // Gönderici Adı
            var lblAd = new LabelControl();
            lblAd.Text = "Gönderici Adı:";
            lblAd.Location = new Point(20, y);
            panelMain.Controls.Add(lblAd);

            _txtGondericiAdi = new TextEdit();
            _txtGondericiAdi.Location = new Point(20 + labelWidth, y);
            _txtGondericiAdi.Size = new Size(inputWidth, 24);
            _txtGondericiAdi.EditValue = "Teknik Servis";
            panelMain.Controls.Add(_txtGondericiAdi);

            y += 50;

            // SSL Kullan
            _chkSslKullan = new CheckEdit();
            _chkSslKullan.Text = "SSL/TLS Kullan (Önerilen)";
            _chkSslKullan.Location = new Point(20 + labelWidth, y);
            _chkSslKullan.Checked = true;
            panelMain.Controls.Add(_chkSslKullan);

            y += 35;

            // E-posta Aktif
            _chkEmailAktif = new CheckEdit();
            _chkEmailAktif.Text = "E-posta Bildirimlerini Aktif Et";
            _chkEmailAktif.Location = new Point(20 + labelWidth, y);
            _chkEmailAktif.Properties.Appearance.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            panelMain.Controls.Add(_chkEmailAktif);

            y += 50;

            // Test Butonu
            var btnTest = new SimpleButton();
            btnTest.Text = "🔗 Bağlantıyı Test Et";
            btnTest.Location = new Point(20, y);
            btnTest.Size = new Size(180, 40);
            btnTest.Appearance.BackColor = AppColors.Info;
            btnTest.Appearance.ForeColor = Color.White;
            btnTest.Click += BtnTest_Click;
            panelMain.Controls.Add(btnTest);

            y += 60;

            // Kaydet ve İptal butonları
            var btnKaydet = new SimpleButton();
            btnKaydet.Text = "💾 Kaydet";
            btnKaydet.Location = new Point(200, y);
            btnKaydet.Size = new Size(120, 40);
            btnKaydet.Appearance.BackColor = AppColors.Success;
            btnKaydet.Appearance.ForeColor = Color.White;
            btnKaydet.Click += BtnKaydet_Click;
            panelMain.Controls.Add(btnKaydet);

            var btnIptal = new SimpleButton();
            btnIptal.Text = "İptal";
            btnIptal.Location = new Point(330, y);
            btnIptal.Size = new Size(100, 40);
            btnIptal.Click += (s, e) => this.Close();
            panelMain.Controls.Add(btnIptal);

            this.Controls.Add(panelMain);
        }

        private async void LoadData()
        {
            try
            {
                var ayarlar = await _repository.GetAsync();
                if (ayarlar != null)
                {
                    _txtSmtpServer.EditValue = ayarlar.SmtpServer;
                    _txtSmtpPort.EditValue = ayarlar.SmtpPort;
                    _txtGondericiEmail.EditValue = ayarlar.GondericiEmail;
                    _txtGondericiSifre.EditValue = ayarlar.GondericiSifre;
                    _txtGondericiAdi.EditValue = ayarlar.GondericiAdi;
                    _chkSslKullan.Checked = ayarlar.SslKullan;
                    _chkEmailAktif.Checked = ayarlar.EmailAktif;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Ayarlar yüklenirken hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnKaydet_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_txtGondericiEmail.Text))
                {
                    XtraMessageBox.Show("Gönderici e-posta adresi gereklidir.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var ayarlar = new EmailAyarlari
                {
                    SmtpServer = _txtSmtpServer.Text,
                    SmtpPort = Convert.ToInt32(_txtSmtpPort.EditValue),
                    GondericiEmail = _txtGondericiEmail.Text,
                    GondericiSifre = _txtGondericiSifre.Text,
                    GondericiAdi = _txtGondericiAdi.Text,
                    SslKullan = _chkSslKullan.Checked,
                    EmailAktif = _chkEmailAktif.Checked,
                    GuncellemeTarihi = DateTime.Now
                };

                await _repository.SaveAsync(ayarlar);
                
                // EmailHelper'ı güncelle
                await EmailHelper.LoadAyarlarAsync();

                XtraMessageBox.Show("E-posta ayarları kaydedildi.", "Bilgi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Kaydetme hatası: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnTest_Click(object? sender, EventArgs e)
        {
            try
            {
                // Önce kaydet
                var ayarlar = new EmailAyarlari
                {
                    SmtpServer = _txtSmtpServer.Text,
                    SmtpPort = Convert.ToInt32(_txtSmtpPort.EditValue),
                    GondericiEmail = _txtGondericiEmail.Text,
                    GondericiSifre = _txtGondericiSifre.Text,
                    GondericiAdi = _txtGondericiAdi.Text,
                    SslKullan = _chkSslKullan.Checked,
                    EmailAktif = true, // Test için aktif
                    GuncellemeTarihi = DateTime.Now
                };

                await _repository.SaveAsync(ayarlar);
                await EmailHelper.LoadAyarlarAsync();

                this.Cursor = Cursors.WaitCursor;
                var (success, message) = await EmailHelper.TestConnectionAsync();
                this.Cursor = Cursors.Default;

                if (success)
                {
                    XtraMessageBox.Show($"✅ {message}", "Başarılı",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    XtraMessageBox.Show($"❌ {message}", "Bağlantı Hatası",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                XtraMessageBox.Show($"Test hatası: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

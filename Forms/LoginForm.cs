using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeknikServisOtomasyon.Data;
using TeknikServisOtomasyon.Data.Repositories;
using TeknikServisOtomasyon.Helpers;

namespace TeknikServisOtomasyon.Forms
{
    public partial class LoginForm : XtraForm
    {
        private readonly KullaniciRepository _kullaniciRepository;

        public LoginForm()
        {
            InitializeComponent();
            _kullaniciRepository = new KullaniciRepository();
        }

        // Form sürükleme işlemi için WinAPI
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.Text = "";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Arka plan paneli (Kenarlık ve sürükleme için)
            var panelMain = new PanelControl();
            panelMain.Dock = DockStyle.Fill;
            panelMain.Appearance.BackColor = Color.FromArgb(32, 32, 32); 
            panelMain.Appearance.Options.UseBackColor = true;
            panelMain.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            panelMain.MouseDown += (s, e) => 
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };

            // Kapatma Butonu (Sağ Üst)
            var btnClose = new SimpleButton();
            btnClose.Text = "✕";
            btnClose.Font = new Font("Segoe UI", 12);
            btnClose.Location = new Point(360, 5);
            btnClose.Size = new Size(35, 35);
            btnClose.Appearance.BackColor = Color.Transparent;
            btnClose.Appearance.ForeColor = Color.Gray;
            btnClose.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => Application.Exit();
            btnClose.MouseEnter += (s, e) => btnClose.Appearance.ForeColor = Color.IndianRed;
            btnClose.MouseLeave += (s, e) => btnClose.Appearance.ForeColor = Color.Gray;
            panelMain.Controls.Add(btnClose);

            // Logo/Başlık
            var lblTitle = new LabelControl();
            lblTitle.Text = "TEKNİK SERVİS";
            lblTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitle.ForeColor = AppColors.Primary;
            lblTitle.Location = new Point(0, 60); // X dinamik hesaplanacak
            lblTitle.AutoSize = true;
            
            // Ortalamak için basit hesaplama yapamıyoruz designer kodunda, ama sabit layout olduğu için manuel ayarlayalım
            // Form width 400. Text approx 200px. 
            lblTitle.Location = new Point(85, 60);
            panelMain.Controls.Add(lblTitle);

            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = "OTOMASYON";
            lblSubtitle.Font = new Font("Segoe UI", 14, FontStyle.Regular);
            lblSubtitle.ForeColor = Color.LightGray;
            lblSubtitle.Location = new Point(140, 105);
            lblSubtitle.AutoSize = true;
            panelMain.Controls.Add(lblSubtitle);

            // Giriş Alanları Paneli
            int startY = 170;
            int margin = 20;

            // Kullanıcı Adı
            var lblUsername = new LabelControl();
            lblUsername.Text = "KULLANICI ADI";
            lblUsername.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblUsername.ForeColor = Color.Gray;
            lblUsername.Location = new Point(40, startY);
            panelMain.Controls.Add(lblUsername);

            var txtUsername = new TextEdit();
            txtUsername.Name = "txtUsername";
            txtUsername.Location = new Point(40, startY + 25);
            txtUsername.Size = new Size(320, 35);
            txtUsername.Properties.Appearance.Font = new Font("Segoe UI", 11);
            txtUsername.Properties.AutoHeight = false;
            txtUsername.Properties.NullValuePrompt = "Kullanıcı adınızı girin";
            // txtUsername.Properties.ContextImageOptions.Image = ... (Resim dosyamız yok)
            panelMain.Controls.Add(txtUsername);

            // Şifre
            startY += 80;
            var lblPassword = new LabelControl();
            lblPassword.Text = "ŞİFRE";
            lblPassword.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblPassword.ForeColor = Color.Gray;
            lblPassword.Location = new Point(40, startY);
            panelMain.Controls.Add(lblPassword);

            var txtPassword = new TextEdit();
            txtPassword.Name = "txtPassword";
            txtPassword.Location = new Point(40, startY + 25);
            txtPassword.Size = new Size(320, 35);
            txtPassword.Properties.PasswordChar = '●';
            txtPassword.Properties.Appearance.Font = new Font("Segoe UI", 11);
            txtPassword.Properties.AutoHeight = false;
            txtPassword.Properties.NullValuePrompt = "Şifrenizi girin";
            txtPassword.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                    BtnLogin_Click(null, null);
            };
            panelMain.Controls.Add(txtPassword);

            // Giriş Butonu
            startY += 90;
            var btnLogin = new SimpleButton();
            btnLogin.Name = "btnLogin";
            btnLogin.Text = "GİRİŞ YAP";
            btnLogin.Location = new Point(40, startY);
            btnLogin.Size = new Size(320, 50);
            btnLogin.Appearance.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLogin.Appearance.BackColor = AppColors.Primary;
            btnLogin.Appearance.ForeColor = Color.White;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;
            panelMain.Controls.Add(btnLogin);

            // Alt Telif/Bilgi
            var lblFooter = new LabelControl();
            lblFooter.Text = "© 2026 Teknik Servis Sistemi";
            lblFooter.Font = new Font("Segoe UI", 8);
            lblFooter.ForeColor = Color.DimGray;
            lblFooter.Location = new Point(125, 470);
            panelMain.Controls.Add(lblFooter);

            this.Controls.Add(panelMain);
            this.ResumeLayout(false);

            this.AcceptButton = btnLogin;
        }

        private async void BtnLogin_Click(object? sender, EventArgs? e)
        {
            var txtUsername = this.Controls.Find("txtUsername", true)[0] as TextEdit;
            var txtPassword = this.Controls.Find("txtPassword", true)[0] as TextEdit;
            var btnLogin = this.Controls.Find("btnLogin", true)[0] as SimpleButton;

            if (string.IsNullOrWhiteSpace(txtUsername?.Text))
            {
                XtraMessageBox.Show("Kullanıcı adı boş olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername?.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword?.Text))
            {
                XtraMessageBox.Show("Şifre boş olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword?.Focus();
                return;
            }

            try
            {
                btnLogin!.Enabled = false;
                btnLogin.Text = "Giriş yapılıyor...";

                var user = await _kullaniciRepository.LoginAsync(txtUsername.Text.Trim(), txtPassword.Text);

                if (user != null)
                {
                    SessionManager.Login(user);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    XtraMessageBox.Show("Kullanıcı adı veya şifre hatalı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Text = "";
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Giriş yapılırken bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin!.Enabled = true;
                btnLogin.Text = "GİRİŞ YAP";
            }
        }
    }
}

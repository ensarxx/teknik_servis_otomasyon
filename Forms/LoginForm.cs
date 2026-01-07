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

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.Text = "Teknik Servis Otomasyon - Giriş";
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Panel
            var panelMain = new PanelControl();
            panelMain.Dock = DockStyle.Fill;
            panelMain.Padding = new Padding(30);

            // Logo/Title Label
            var lblTitle = new LabelControl();
            lblTitle.Text = "TEKNİK SERVİS\nOTOMASYON";
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.ForeColor = AppColors.Primary;
            lblTitle.Location = new Point(90, 30);
            lblTitle.AutoSize = true;

            // Subtitle
            var lblSubtitle = new LabelControl();
            lblSubtitle.Text = "Telefon | Tablet | Laptop";
            lblSubtitle.Font = new Font("Segoe UI", 10);
            lblSubtitle.ForeColor = Color.Gray;
            lblSubtitle.Location = new Point(130, 110);
            lblSubtitle.AutoSize = true;

            // Username Label
            var lblUsername = new LabelControl();
            lblUsername.Text = "Kullanıcı Adı";
            lblUsername.Location = new Point(50, 160);

            // Username TextEdit
            var txtUsername = new TextEdit();
            txtUsername.Name = "txtUsername";
            txtUsername.Location = new Point(50, 180);
            txtUsername.Size = new Size(330, 30);
            txtUsername.Properties.Appearance.Font = new Font("Segoe UI", 11);

            // Password Label
            var lblPassword = new LabelControl();
            lblPassword.Text = "Şifre";
            lblPassword.Location = new Point(50, 220);

            // Password TextEdit
            var txtPassword = new TextEdit();
            txtPassword.Name = "txtPassword";
            txtPassword.Location = new Point(50, 240);
            txtPassword.Size = new Size(330, 30);
            txtPassword.Properties.PasswordChar = '●';
            txtPassword.Properties.Appearance.Font = new Font("Segoe UI", 11);
            txtPassword.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                    BtnLogin_Click(null, null);
            };

            // Login Button
            var btnLogin = new SimpleButton();
            btnLogin.Name = "btnLogin";
            btnLogin.Text = "GİRİŞ YAP";
            btnLogin.Location = new Point(50, 300);
            btnLogin.Size = new Size(330, 45);
            btnLogin.Appearance.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLogin.Appearance.BackColor = AppColors.Primary;
            btnLogin.Appearance.ForeColor = Color.White;
            btnLogin.Click += BtnLogin_Click;

            // Add controls
            panelMain.Controls.Add(lblTitle);
            panelMain.Controls.Add(lblSubtitle);
            panelMain.Controls.Add(lblUsername);
            panelMain.Controls.Add(txtUsername);
            panelMain.Controls.Add(lblPassword);
            panelMain.Controls.Add(txtPassword);
            panelMain.Controls.Add(btnLogin);

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

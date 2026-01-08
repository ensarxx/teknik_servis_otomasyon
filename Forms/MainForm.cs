using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraTabbedMdi;
using System;
using System.Drawing;
using System.Windows.Forms;
using TeknikServisOtomasyon.Forms.Modules;
using TeknikServisOtomasyon.Helpers;

namespace TeknikServisOtomasyon.Forms
{
    public partial class MainForm : RibbonForm
    {
        private XtraTabbedMdiManager _mdiManager;

        public MainForm()
        {
            InitializeComponent();
            SetupMdiManager();
            this.WindowState = FormWindowState.Maximized;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.Text = $"Teknik Servis Otomasyon - {SessionManager.CurrentUser?.AdSoyad}";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.IsMdiContainer = true;

            // Ribbon Control
            var ribbon = new RibbonControl();
            ribbon.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;

            // Ana Sayfa Page
            var pageAnaSayfa = new RibbonPage("Ana Sayfa");

            // Servis İşlemleri Group
            var groupServis = new RibbonPageGroup("Servis İşlemleri");

            var btnDashboard = new BarButtonItem();
            btnDashboard.Caption = "Dashboard";
            btnDashboard.LargeGlyph = CreateIconWithSymbol("📊", AppColors.Primary);
            btnDashboard.ItemClick += (s, e) => OpenForm<DashboardForm>();
            groupServis.ItemLinks.Add(btnDashboard);

            var btnYeniServis = new BarButtonItem();
            btnYeniServis.Caption = "Yeni Servis";
            btnYeniServis.LargeGlyph = CreateIconWithSymbol("➕", AppColors.Success);
            btnYeniServis.ItemClick += (s, e) => OpenServisKayitForm();
            groupServis.ItemLinks.Add(btnYeniServis);

            var btnServisListesi = new BarButtonItem();
            btnServisListesi.Caption = "Servis Listesi";
            btnServisListesi.LargeGlyph = CreateIconWithSymbol("📋", AppColors.Info);
            btnServisListesi.ItemClick += (s, e) => OpenForm<ServisListeForm>();
            groupServis.ItemLinks.Add(btnServisListesi);

            pageAnaSayfa.Groups.Add(groupServis);

            // Müşteri İşlemleri Group
            var groupMusteri = new RibbonPageGroup("Müşteri İşlemleri");

            var btnMusteriler = new BarButtonItem();
            btnMusteriler.Caption = "Müşteriler";
            btnMusteriler.LargeGlyph = CreateIconWithSymbol("👥", AppColors.Secondary);
            btnMusteriler.ItemClick += (s, e) => OpenForm<MusteriListeForm>();
            groupMusteri.ItemLinks.Add(btnMusteriler);

            var btnYeniMusteri = new BarButtonItem();
            btnYeniMusteri.Caption = "Yeni Müşteri";
            btnYeniMusteri.LargeGlyph = CreateIconWithSymbol("👤", AppColors.Success);
            btnYeniMusteri.ItemClick += (s, e) => OpenMusteriForm();
            groupMusteri.ItemLinks.Add(btnYeniMusteri);

            pageAnaSayfa.Groups.Add(groupMusteri);

            // Stok İşlemleri Group
            var groupStok = new RibbonPageGroup("Stok İşlemleri");

            var btnParcalar = new BarButtonItem();
            btnParcalar.Caption = "Parça Listesi";
            btnParcalar.LargeGlyph = CreateIconWithSymbol("🔧", AppColors.Warning);
            btnParcalar.ItemClick += (s, e) => OpenForm<ParcaListeForm>();
            groupStok.ItemLinks.Add(btnParcalar);

            var btnYeniParca = new BarButtonItem();
            btnYeniParca.Caption = "Yeni Parça";
            btnYeniParca.LargeGlyph = CreateIconWithSymbol("⚙️", AppColors.Success);
            btnYeniParca.ItemClick += (s, e) => OpenParcaForm();
            groupStok.ItemLinks.Add(btnYeniParca);

            pageAnaSayfa.Groups.Add(groupStok);

            ribbon.Pages.Add(pageAnaSayfa);

            // Yönetim Page (Sadece Admin için)
            if (SessionManager.IsAdmin)
            {
                var pageYonetim = new RibbonPage("Yönetim");

                var groupKullanici = new RibbonPageGroup("Kullanıcı İşlemleri");

                var btnKullanicilar = new BarButtonItem();
                btnKullanicilar.Caption = "Kullanıcılar";
                btnKullanicilar.LargeGlyph = CreateIconWithSymbol("👤", AppColors.PrimaryDark);
                btnKullanicilar.ItemClick += (s, e) => OpenForm<KullaniciListeForm>();
                groupKullanici.ItemLinks.Add(btnKullanicilar);

                pageYonetim.Groups.Add(groupKullanici);

                var groupRaporlar = new RibbonPageGroup("Raporlar");

                var btnRaporlar = new BarButtonItem();
                btnRaporlar.Caption = "Raporlar";
                btnRaporlar.LargeGlyph = CreateIconWithSymbol("📈", AppColors.Info);
                btnRaporlar.ItemClick += (s, e) => OpenForm<RaporlarForm>();
                groupRaporlar.ItemLinks.Add(btnRaporlar);

                pageYonetim.Groups.Add(groupRaporlar);

                // Ayarlar grubu
                var groupAyarlar = new RibbonPageGroup("Ayarlar");

                var btnEmailAyarlari = new BarButtonItem();
                btnEmailAyarlari.Caption = "E-Posta Ayarları";
                btnEmailAyarlari.LargeGlyph = CreateIconWithSymbol("📧", System.Drawing.Color.FromArgb(156, 39, 176));
                btnEmailAyarlari.ItemClick += (s, e) => {
                    var form = new EmailAyarlariForm();
                    form.ShowDialog();
                };
                groupAyarlar.ItemLinks.Add(btnEmailAyarlari);

                pageYonetim.Groups.Add(groupAyarlar);

                ribbon.Pages.Add(pageYonetim);
            }

            // Çıkış Butonu
            var pageKullanici = new RibbonPage("Kullanıcı");
            var groupCikis = new RibbonPageGroup("Oturum");

            var btnProfil = new BarButtonItem();
            btnProfil.Caption = $"Hoş geldiniz, {SessionManager.CurrentUser?.AdSoyad}";
            btnProfil.LargeGlyph = CreateIconWithSymbol("😊", AppColors.Info);
            groupCikis.ItemLinks.Add(btnProfil);

            var btnCikis = new BarButtonItem();
            btnCikis.Caption = "Çıkış Yap";
            btnCikis.LargeGlyph = CreateIconWithSymbol("🚪", AppColors.Danger);
            btnCikis.ItemClick += BtnCikis_Click;
            groupCikis.ItemLinks.Add(btnCikis);

            pageKullanici.Groups.Add(groupCikis);
            ribbon.Pages.Add(pageKullanici);

            this.Controls.Add(ribbon);
            ribbon.Dock = DockStyle.Top;

            // Status Bar
            var statusBar = new RibbonStatusBar();
            statusBar.Ribbon = ribbon;

            var lblStatus = new BarStaticItem();
            lblStatus.Caption = $"Kullanıcı: {SessionManager.CurrentUser?.AdSoyad} | Rol: {SessionManager.CurrentUser?.Rol}";
            statusBar.ItemLinks.Add(lblStatus);

            var lblTarih = new BarStaticItem();
            lblTarih.Caption = DateTime.Now.ToString("dd MMMM yyyy HH:mm");
            statusBar.ItemLinks.Add(lblTarih);

            this.Controls.Add(statusBar);

            this.ResumeLayout(false);
        }

        private void SetupMdiManager()
        {
            _mdiManager = new XtraTabbedMdiManager();
            _mdiManager.MdiParent = this;
            _mdiManager.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InAllTabPageHeaders;
            _mdiManager.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Top;

            // Başlangıçta Dashboard'u aç
            OpenForm<DashboardForm>();
        }

        private Bitmap CreateColoredIcon(Color color)
        {
            var bmp = new Bitmap(32, 32);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, 2, 2, 28, 28);
                }
            }
            return bmp;
        }

        private Bitmap CreateIconWithSymbol(string symbol, Color bgColor)
        {
            var bmp = new Bitmap(32, 32);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                
                // Arka plan daire
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillEllipse(brush, 1, 1, 30, 30);
                }
                
                // Sembol
                using (var font = new Font("Segoe UI Emoji", 14f, FontStyle.Regular))
                using (var brush = new SolidBrush(Color.White))
                {
                    var size = g.MeasureString(symbol, font);
                    var x = (32 - size.Width) / 2;
                    var y = (32 - size.Height) / 2;
                    g.DrawString(symbol, font, brush, x, y);
                }
            }
            return bmp;
        }

        private void OpenForm<T>() where T : Form, new()
        {
            // Aynı tip form açık mı kontrol et
            foreach (Form form in this.MdiChildren)
            {
                if (form is T)
                {
                    form.Activate();
                    return;
                }
            }

            var newForm = new T();
            newForm.MdiParent = this;
            newForm.Show();
        }

        private void OpenServisKayitForm()
        {
            var newForm = new ServisKayitForm();
            newForm.MdiParent = this;
            newForm.Show();
        }

        private void OpenMusteriForm()
        {
            var form = new MusteriForm();
            form.ShowDialog();
        }

        private void OpenParcaForm()
        {
            var form = new ParcaForm();
            form.ShowDialog();
        }

        private void BtnCikis_Click(object? sender, ItemClickEventArgs e)
        {
            var result = XtraMessageBox.Show("Çıkış yapmak istediğinize emin misiniz?", "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                SessionManager.Logout();
                this.Close();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (SessionManager.IsLoggedIn)
            {
                var result = XtraMessageBox.Show("Uygulamadan çıkmak istediğinize emin misiniz?", "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            base.OnFormClosing(e);
        }
    }
}

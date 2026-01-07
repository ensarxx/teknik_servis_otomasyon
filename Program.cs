using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using TeknikServisOtomasyon.Data;
using TeknikServisOtomasyon.Forms;

namespace TeknikServisOtomasyon;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // DevExpress Dark Mode tema ayarları
        SkinManager.EnableFormSkins();
        SkinManager.EnableMdiFormSkins();
        UserLookAndFeel.Default.SetSkinStyle("Office 2019 Black");
        
        ApplicationConfiguration.Initialize();

        // Veritabanı bağlantısını kontrol et
        if (!DatabaseConnection.Instance.TestConnection())
        {
            var result = XtraMessageBox.Show(
                "MySQL veritabanına bağlanılamadı.\n\n" +
                "Lütfen aşağıdakileri kontrol edin:\n" +
                "1. MySQL sunucusunun çalıştığından emin olun\n" +
                "2. Bağlantı bilgilerini doğrulayın\n\n" +
                "Uygulama yine de başlatılsın mı?",
                "Veritabanı Bağlantı Hatası",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.No)
                return;
        }
        else
        {
            // Veritabanını ve tabloları oluştur
            try
            {
                var initializer = new DatabaseInitializer();
                initializer.InitializeDatabaseAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Veritabanı başlatılırken hata oluştu:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Login formu
        var loginForm = new LoginForm();
        if (loginForm.ShowDialog() == DialogResult.OK)
        {
            // Ana form
            Application.Run(new MainForm());
        }
    }
}
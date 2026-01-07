using DevExpress.Utils;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TeknikServisOtomasyon.Data.Repositories;
using TeknikServisOtomasyon.Helpers;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Forms.Modules
{
    public partial class ServisFotografForm : XtraForm
    {
        private readonly ServisFotografRepository _fotografRepository;
        private readonly int _servisId;
        private readonly string _servisNo;
        private FlowLayoutPanel _flowPanel = null!;

        public ServisFotografForm(int servisId, string servisNo)
        {
            _fotografRepository = new ServisFotografRepository();
            _servisId = servisId;
            _servisNo = servisNo;
            InitializeComponent();
            LoadFotograflar();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = $"📷 Servis Fotoğrafları - {_servisNo}";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterParent;

            var panelMain = new PanelControl();
            panelMain.Dock = DockStyle.Fill;
            panelMain.Padding = new Padding(15);

            // Başlık
            var panelHeader = new PanelControl();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 60;
            panelHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            var lblTitle = new LabelControl();
            lblTitle.Text = $"📷 {_servisNo} - Cihaz Fotoğrafları";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.Location = new Point(10, 15);
            panelHeader.Controls.Add(lblTitle);

            // Toolbar
            var panelToolbar = new PanelControl();
            panelToolbar.Dock = DockStyle.Top;
            panelToolbar.Height = 60;
            panelToolbar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            var grpEkle = new GroupControl();
            grpEkle.Text = "📤 Fotoğraf Ekle";
            grpEkle.Location = new Point(10, 5);
            grpEkle.Size = new Size(600, 50);
            grpEkle.AppearanceCaption.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            var cmbTip = new ComboBoxEdit();
            cmbTip.Location = new Point(15, 20);
            cmbTip.Size = new Size(120, 24);
            cmbTip.Properties.Items.AddRange(new[] { "Giriş", "Arıza", "Onarım", "Çıkış", "Diğer" });
            cmbTip.SelectedIndex = 0;
            cmbTip.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            grpEkle.Controls.Add(cmbTip);

            var btnDosyaSec = new SimpleButton();
            btnDosyaSec.Text = "📁 Dosya Seç";
            btnDosyaSec.Location = new Point(150, 18);
            btnDosyaSec.Size = new Size(110, 28);
            btnDosyaSec.Click += (s, e) => SelectAndAddPhoto(cmbTip.Text);
            grpEkle.Controls.Add(btnDosyaSec);

            var btnKamera = new SimpleButton();
            btnKamera.Text = "📷 Kameradan Çek";
            btnKamera.Location = new Point(270, 18);
            btnKamera.Size = new Size(130, 28);
            btnKamera.Appearance.BackColor = AppColors.Info;
            btnKamera.Appearance.ForeColor = Color.White;
            btnKamera.Click += (s, e) => CaptureFromCamera(cmbTip.Text);
            grpEkle.Controls.Add(btnKamera);

            var btnYenile = new SimpleButton();
            btnYenile.Text = "🔄";
            btnYenile.Location = new Point(410, 18);
            btnYenile.Size = new Size(40, 28);
            btnYenile.ToolTip = "Yenile";
            btnYenile.Click += (s, e) => LoadFotograflar();
            grpEkle.Controls.Add(btnYenile);

            panelToolbar.Controls.Add(grpEkle);

            var lblInfo = new LabelControl();
            lblInfo.Name = "lblInfo";
            lblInfo.Text = "Yükleniyor...";
            lblInfo.Location = new Point(630, 25);
            lblInfo.Font = new Font("Segoe UI", 10);
            panelToolbar.Controls.Add(lblInfo);

            // Fotoğraf Alanı
            var panelFoto = new PanelControl();
            panelFoto.Dock = DockStyle.Fill;
            panelFoto.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            _flowPanel = new FlowLayoutPanel();
            _flowPanel.Dock = DockStyle.Fill;
            _flowPanel.AutoScroll = true;
            _flowPanel.Padding = new Padding(10);
            _flowPanel.BackColor = Color.FromArgb(40, 40, 40);
            panelFoto.Controls.Add(_flowPanel);

            // Alt butonlar
            var panelBottom = new PanelControl();
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Height = 50;
            panelBottom.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            var btnKapat = new SimpleButton();
            btnKapat.Text = "Kapat";
            btnKapat.Location = new Point(780, 10);
            btnKapat.Size = new Size(90, 30);
            btnKapat.Click += (s, e) => this.Close();
            panelBottom.Controls.Add(btnKapat);

            panelMain.Controls.Add(panelFoto);
            panelMain.Controls.Add(panelToolbar);
            panelMain.Controls.Add(panelHeader);
            panelMain.Controls.Add(panelBottom);

            this.Controls.Add(panelMain);
            this.ResumeLayout(false);
        }

        private async void LoadFotograflar()
        {
            try
            {
                _flowPanel.Controls.Clear();
                var fotograflar = await _fotografRepository.GetByServisIdAsync(_servisId);

                var lblInfo = this.Controls.Find("lblInfo", true);
                if (lblInfo.Length > 0 && lblInfo[0] is LabelControl lbl)
                    lbl.Text = $"Toplam {fotograflar.Count} fotoğraf";

                if (fotograflar.Count == 0)
                {
                    var lblEmpty = new Label();
                    lblEmpty.Text = "Henüz fotoğraf eklenmemiş.\nYukarıdaki butonları kullanarak fotoğraf ekleyebilirsiniz.";
                    lblEmpty.Font = new Font("Segoe UI", 12);
                    lblEmpty.ForeColor = Color.Gray;
                    lblEmpty.Size = new Size(400, 60);
                    lblEmpty.TextAlign = ContentAlignment.MiddleCenter;
                    _flowPanel.Controls.Add(lblEmpty);
                    return;
                }

                foreach (var foto in fotograflar)
                {
                    var card = CreateFotoCard(foto);
                    _flowPanel.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Fotoğraflar yüklenirken hata oluştu:\n{ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateFotoCard(ServisFotograf foto)
        {
            var card = new Panel();
            card.Size = new Size(200, 220);
            card.BackColor = Color.FromArgb(55, 55, 55);
            card.Margin = new Padding(10);
            card.Cursor = Cursors.Hand;

            // Fotoğraf
            var pictureBox = new PictureBox();
            pictureBox.Size = new Size(180, 140);
            pictureBox.Location = new Point(10, 10);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BackColor = Color.FromArgb(30, 30, 30);
            pictureBox.Cursor = Cursors.Hand;

            if (File.Exists(foto.DosyaYolu))
            {
                try
                {
                    pictureBox.Image = ImageHelper.CreateThumbnail(foto.DosyaYolu, 180, 140);
                }
                catch
                {
                    pictureBox.Image = null;
                }
            }

            // Çift tıklayınca büyük görüntüle
            pictureBox.DoubleClick += (s, e) => ShowFullImage(foto);
            card.Controls.Add(pictureBox);

            // Tip etiketi
            var lblTip = new Label();
            lblTip.Text = $"📷 {foto.FotografTipi}";
            lblTip.Location = new Point(10, 155);
            lblTip.Size = new Size(120, 20);
            lblTip.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblTip.ForeColor = Color.White;
            card.Controls.Add(lblTip);

            // Tarih
            var lblTarih = new Label();
            lblTarih.Text = foto.YuklenmeTarihi.ToString("dd.MM.yy HH:mm");
            lblTarih.Location = new Point(10, 175);
            lblTarih.Size = new Size(120, 18);
            lblTarih.Font = new Font("Segoe UI", 8);
            lblTarih.ForeColor = Color.Gray;
            card.Controls.Add(lblTarih);

            // Sil butonu
            var btnSil = new SimpleButton();
            btnSil.Text = "🗑";
            btnSil.Location = new Point(160, 155);
            btnSil.Size = new Size(30, 30);
            btnSil.Appearance.BackColor = AppColors.Danger;
            btnSil.Appearance.ForeColor = Color.White;
            btnSil.ToolTip = "Sil";
            btnSil.Click += async (s, e) =>
            {
                var result = XtraMessageBox.Show("Bu fotoğrafı silmek istediğinize emin misiniz?",
                    "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        await _fotografRepository.DeleteAsync(foto.Id);
                        ImageHelper.DeleteFotograf(foto.DosyaYolu);
                        LoadFotograflar();
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show($"Silme hatası: {ex.Message}", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };
            card.Controls.Add(btnSil);

            return card;
        }

        private void ShowFullImage(ServisFotograf foto)
        {
            if (!File.Exists(foto.DosyaYolu))
            {
                XtraMessageBox.Show("Fotoğraf dosyası bulunamadı.", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var form = new XtraForm())
            {
                form.Text = $"📷 {foto.FotografTipi} - {foto.YuklenmeTarihi:dd.MM.yyyy HH:mm}";
                form.Size = new Size(900, 700);
                form.StartPosition = FormStartPosition.CenterParent;
                form.KeyPreview = true;
                form.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) form.Close(); };

                var pictureBox = new PictureBox();
                pictureBox.Dock = DockStyle.Fill;
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.BackColor = Color.Black;

                try
                {
                    pictureBox.Image = Image.FromFile(foto.DosyaYolu);
                }
                catch
                {
                    pictureBox.Image = null;
                }

                form.Controls.Add(pictureBox);
                form.ShowDialog();
            }
        }

        private async void SelectAndAddPhoto(string fotografTipi)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Fotoğraf Seç";
                dialog.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Tüm Dosyalar|*.*";
                dialog.Multiselect = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in dialog.FileNames)
                    {
                        await AddPhotoAsync(file, fotografTipi);
                    }
                    LoadFotograflar();
                }
            }
        }

        private async System.Threading.Tasks.Task AddPhotoAsync(string sourceFilePath, string fotografTipi)
        {
            try
            {
                // Görseli kaydet
                var hedefYol = ImageHelper.SaveImage(sourceFilePath, _servisId, fotografTipi);

                // Veritabanına ekle
                var foto = new ServisFotograf
                {
                    ServisId = _servisId,
                    DosyaAdi = Path.GetFileName(hedefYol),
                    DosyaYolu = hedefYol,
                    FotografTipi = fotografTipi,
                    YuklenmeTarihi = DateTime.Now
                };

                await _fotografRepository.AddAsync(foto);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Fotoğraf eklenirken hata oluştu:\n{ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CaptureFromCamera(string fotografTipi)
        {
            XtraMessageBox.Show("Kamera özelliği Windows Forms'da doğrudan desteklenmemektedir.\n\n" +
                "Alternatif olarak:\n" +
                "• Telefonunuzla fotoğraf çekip bilgisayara aktarabilirsiniz\n" +
                "• Windows Kamera uygulamasını kullanabilirsiniz\n" +
                "• Ekran görüntüsü alabilirsiniz",
                "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

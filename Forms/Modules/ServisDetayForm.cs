using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeknikServisOtomasyon.Data.Repositories;
using TeknikServisOtomasyon.Helpers;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Forms.Modules
{
    public partial class ServisDetayForm : XtraForm
    {
        private readonly ServisKaydiRepository _servisRepository;
        private readonly ServisFotografRepository _fotografRepository;
        private readonly int _servisId;
        private ServisKaydi? _servis;
        private FlowLayoutPanel? _fotografPanel;
        private PictureBox? _buyukResim;
        private List<ServisFotograf> _fotograflar = new();

        public ServisDetayForm(int servisId)
        {
            _servisRepository = new ServisKaydiRepository();
            _fotografRepository = new ServisFotografRepository();
            _servisId = servisId;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "📋 Servis Detayı";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(1200, 750);

            // Sol Panel (Bilgiler)
            var panelSol = new XtraScrollableControl();
            panelSol.Dock = DockStyle.Left;
            panelSol.Width = 780;
            panelSol.Padding = new Padding(20);

            // Başlık
            var lblBaslik = new LabelControl();
            lblBaslik.Name = "lblBaslik";
            lblBaslik.Text = "📋 SERVİS DETAYI";
            lblBaslik.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblBaslik.ForeColor = AppColors.Primary;
            lblBaslik.Location = new Point(20, 10);
            panelSol.Controls.Add(lblBaslik);

            // Servis No ve Durum
            var lblServisNo = new LabelControl();
            lblServisNo.Name = "lblServisNo";
            lblServisNo.Text = "Servis No: -";
            lblServisNo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblServisNo.Location = new Point(20, 50);
            panelSol.Controls.Add(lblServisNo);

            var lblDurum = new LabelControl();
            lblDurum.Name = "lblDurum";
            lblDurum.Text = "DURUM";
            lblDurum.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblDurum.Location = new Point(500, 50);
            panelSol.Controls.Add(lblDurum);

            // Müşteri Bilgileri
            var grpMusteri = new GroupControl();
            grpMusteri.Text = "Müşteri Bilgileri";
            grpMusteri.Location = new Point(20, 90);
            grpMusteri.Size = new Size(350, 110);

            var lblMusteriAdi = new LabelControl();
            lblMusteriAdi.Name = "lblMusteriAdi";
            lblMusteriAdi.Text = "Müşteri: -";
            lblMusteriAdi.Location = new Point(15, 30);
            grpMusteri.Controls.Add(lblMusteriAdi);

            var lblMusteriTelefon = new LabelControl();
            lblMusteriTelefon.Name = "lblMusteriTelefon";
            lblMusteriTelefon.Text = "Telefon: -";
            lblMusteriTelefon.Location = new Point(15, 55);
            grpMusteri.Controls.Add(lblMusteriTelefon);

            var lblGirisTarihi = new LabelControl();
            lblGirisTarihi.Name = "lblGirisTarihi";
            lblGirisTarihi.Text = "Giriş: -";
            lblGirisTarihi.Location = new Point(15, 80);
            grpMusteri.Controls.Add(lblGirisTarihi);

            panelSol.Controls.Add(grpMusteri);

            // Cihaz Bilgileri
            var grpCihaz = new GroupControl();
            grpCihaz.Text = "Cihaz Bilgileri";
            grpCihaz.Location = new Point(390, 90);
            grpCihaz.Size = new Size(350, 110);

            var lblCihazTuru = new LabelControl();
            lblCihazTuru.Name = "lblCihazTuru";
            lblCihazTuru.Text = "Cihaz Türü: -";
            lblCihazTuru.Location = new Point(15, 30);
            grpCihaz.Controls.Add(lblCihazTuru);

            var lblCihazMarka = new LabelControl();
            lblCihazMarka.Name = "lblCihazMarka";
            lblCihazMarka.Text = "Marka/Model: -";
            lblCihazMarka.Location = new Point(15, 55);
            grpCihaz.Controls.Add(lblCihazMarka);

            var lblSeriNo = new LabelControl();
            lblSeriNo.Name = "lblSeriNo";
            lblSeriNo.Text = "Seri No: -";
            lblSeriNo.Location = new Point(15, 80);
            grpCihaz.Controls.Add(lblSeriNo);

            panelSol.Controls.Add(grpCihaz);

            // Arıza Bilgileri
            var grpAriza = new GroupControl();
            grpAriza.Text = "Arıza Bilgileri";
            grpAriza.Location = new Point(20, 215);
            grpAriza.Size = new Size(720, 100);

            var lblAriza = new LabelControl();
            lblAriza.Name = "lblAriza";
            lblAriza.Text = "Arıza: -";
            lblAriza.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblAriza.Location = new Point(15, 30);
            lblAriza.AutoSize = true;
            grpAriza.Controls.Add(lblAriza);

            var lblArizaDetay = new LabelControl();
            lblArizaDetay.Name = "lblArizaDetay";
            lblArizaDetay.Text = "Detay: -";
            lblArizaDetay.Location = new Point(15, 55);
            lblArizaDetay.AutoSize = true;
            grpAriza.Controls.Add(lblArizaDetay);

            panelSol.Controls.Add(grpAriza);

            // Yapılan İşlemler
            var grpIslemler = new GroupControl();
            grpIslemler.Text = "Yapılan İşlemler";
            grpIslemler.Location = new Point(20, 330);
            grpIslemler.Size = new Size(720, 90);

            var lblYapilanIslemler = new LabelControl();
            lblYapilanIslemler.Name = "lblYapilanIslemler";
            lblYapilanIslemler.Text = "-";
            lblYapilanIslemler.Location = new Point(15, 30);
            lblYapilanIslemler.AutoSize = true;
            grpIslemler.Controls.Add(lblYapilanIslemler);

            panelSol.Controls.Add(grpIslemler);

            // Ücret Bilgileri
            var grpUcret = new GroupControl();
            grpUcret.Text = "Ücret Bilgileri";
            grpUcret.Location = new Point(20, 435);
            grpUcret.Size = new Size(720, 80);

            var lblIscilik = new LabelControl();
            lblIscilik.Name = "lblIscilik";
            lblIscilik.Text = "İşçilik: ₺0";
            lblIscilik.Location = new Point(15, 35);
            grpUcret.Controls.Add(lblIscilik);

            var lblParcaUcreti = new LabelControl();
            lblParcaUcreti.Name = "lblParcaUcreti";
            lblParcaUcreti.Text = "Parça: ₺0";
            lblParcaUcreti.Location = new Point(150, 35);
            grpUcret.Controls.Add(lblParcaUcreti);

            var lblToplam = new LabelControl();
            lblToplam.Name = "lblToplam";
            lblToplam.Text = "TOPLAM: ₺0";
            lblToplam.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblToplam.ForeColor = AppColors.Success;
            lblToplam.Location = new Point(500, 30);
            grpUcret.Controls.Add(lblToplam);

            var lblTahsilat = new LabelControl();
            lblTahsilat.Name = "lblTahsilat";
            lblTahsilat.Text = "Tahsilat: -";
            lblTahsilat.Location = new Point(300, 35);
            grpUcret.Controls.Add(lblTahsilat);

            panelSol.Controls.Add(grpUcret);

            // Butonlar
            var btnPdfExport = new SimpleButton();
            btnPdfExport.Text = "📄 PDF Kaydet";
            btnPdfExport.Location = new Point(20, 530);
            btnPdfExport.Size = new Size(120, 35);
            btnPdfExport.Appearance.BackColor = AppColors.Danger;
            btnPdfExport.Appearance.ForeColor = Color.White;
            btnPdfExport.Click += BtnPdfExport_Click;
            panelSol.Controls.Add(btnPdfExport);

            var btnDuzenle = new SimpleButton();
            btnDuzenle.Text = "✏ Düzenle";
            btnDuzenle.Location = new Point(400, 530);
            btnDuzenle.Size = new Size(100, 35);
            btnDuzenle.Appearance.BackColor = AppColors.Warning;
            btnDuzenle.Appearance.ForeColor = Color.Black;
            btnDuzenle.Click += BtnDuzenle_Click;
            panelSol.Controls.Add(btnDuzenle);

            var btnDurumGuncelle = new SimpleButton();
            btnDurumGuncelle.Text = "🔄 Durum";
            btnDurumGuncelle.Location = new Point(510, 530);
            btnDurumGuncelle.Size = new Size(100, 35);
            btnDurumGuncelle.Appearance.BackColor = AppColors.Info;
            btnDurumGuncelle.Appearance.ForeColor = Color.White;
            btnDurumGuncelle.Click += BtnDurumGuncelle_Click;
            panelSol.Controls.Add(btnDurumGuncelle);

            var btnKapat = new SimpleButton();
            btnKapat.Text = "Kapat";
            btnKapat.Location = new Point(620, 530);
            btnKapat.Size = new Size(100, 35);
            btnKapat.Click += (s, e) => this.Close();
            panelSol.Controls.Add(btnKapat);

            this.Controls.Add(panelSol);

            // Sağ Panel (Fotoğraflar)
            var panelSag = new GroupControl();
            panelSag.Text = "📷 Cihaz Fotoğrafları";
            panelSag.Dock = DockStyle.Fill;
            panelSag.Padding = new Padding(10);

            // Fotoğraf Ekle Butonu - En üstte
            var btnFotoEkle = new SimpleButton();
            btnFotoEkle.Text = "📷 Fotoğraf Ekle";
            btnFotoEkle.Location = new Point(10, 30);
            btnFotoEkle.Size = new Size(175, 35);
            btnFotoEkle.Appearance.BackColor = Color.FromArgb(156, 39, 176);
            btnFotoEkle.Appearance.ForeColor = Color.White;
            btnFotoEkle.Click += BtnFotoEkle_Click;
            panelSag.Controls.Add(btnFotoEkle);

            // Fotoğraf Yönetimi Butonu - En üstte
            var btnFotoYonetim = new SimpleButton();
            btnFotoYonetim.Text = "⚙ Fotoğraf Yönetimi";
            btnFotoYonetim.Location = new Point(195, 30);
            btnFotoYonetim.Size = new Size(175, 35);
            btnFotoYonetim.Appearance.BackColor = AppColors.Info;
            btnFotoYonetim.Appearance.ForeColor = Color.White;
            btnFotoYonetim.Click += BtnFotograflar_Click;
            panelSag.Controls.Add(btnFotoYonetim);

            // Büyük Resim
            _buyukResim = new PictureBox();
            _buyukResim.Location = new Point(10, 75);
            _buyukResim.Size = new Size(360, 320);
            _buyukResim.SizeMode = PictureBoxSizeMode.Zoom;
            _buyukResim.BackColor = Color.FromArgb(40, 40, 40);
            _buyukResim.BorderStyle = BorderStyle.FixedSingle;
            panelSag.Controls.Add(_buyukResim);

            // Küçük Resimler (Thumbnails)
            var lblThumbnails = new LabelControl();
            lblThumbnails.Text = "Fotoğraflar (tıklayarak büyütün, çift tıklayarak açın):";
            lblThumbnails.Location = new Point(10, 405);
            panelSag.Controls.Add(lblThumbnails);

            _fotografPanel = new FlowLayoutPanel();
            _fotografPanel.Location = new Point(10, 425);
            _fotografPanel.Size = new Size(360, 230);
            _fotografPanel.AutoScroll = true;
            _fotografPanel.BackColor = Color.FromArgb(40, 40, 40);
            _fotografPanel.BorderStyle = BorderStyle.FixedSingle;
            panelSag.Controls.Add(_fotografPanel);

            this.Controls.Add(panelSag);

            this.ResumeLayout(false);
        }

        private async void LoadData()
        {
            try
            {
                _servis = await _servisRepository.GetByIdAsync(_servisId);
                if (_servis == null)
                {
                    XtraMessageBox.Show("Servis kaydı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Servis No
                var lblServisNo = this.Controls.Find("lblServisNo", true)[0] as LabelControl;
                lblServisNo!.Text = $"Servis No: {_servis.ServisNo}";

                // Durum
                var lblDurum = this.Controls.Find("lblDurum", true)[0] as LabelControl;
                lblDurum!.Text = _servis.Durum;
                lblDurum.ForeColor = AppColors.GetDurumRengi(_servis.Durum);

                // Müşteri Bilgileri - Dynamic property kullanımı yerine repository'den çek
                var musteriRepository = new MusteriRepository();
                var musteri = await musteriRepository.GetByIdAsync(_servis.MusteriId);

                var lblMusteriAdi = this.Controls.Find("lblMusteriAdi", true)[0] as LabelControl;
                lblMusteriAdi!.Text = $"Müşteri: {musteri?.AdSoyad ?? "-"}";

                var lblMusteriTelefon = this.Controls.Find("lblMusteriTelefon", true)[0] as LabelControl;
                lblMusteriTelefon!.Text = $"Telefon: {musteri?.Telefon ?? "-"}";

                var lblGirisTarihi = this.Controls.Find("lblGirisTarihi", true)[0] as LabelControl;
                lblGirisTarihi!.Text = $"Giriş: {_servis.GirisTarihi:dd.MM.yyyy HH:mm}";

                // Cihaz Bilgileri
                var cihazRepository = new CihazRepository();
                var cihaz = await cihazRepository.GetByIdAsync(_servis.CihazId);

                var lblCihazTuru = this.Controls.Find("lblCihazTuru", true)[0] as LabelControl;
                lblCihazTuru!.Text = $"Cihaz Türü: {cihaz?.CihazTuru ?? "-"}";

                var lblCihazMarka = this.Controls.Find("lblCihazMarka", true)[0] as LabelControl;
                lblCihazMarka!.Text = $"Marka/Model: {cihaz?.Marka} {cihaz?.Model}";

                var lblSeriNo = this.Controls.Find("lblSeriNo", true)[0] as LabelControl;
                lblSeriNo!.Text = $"Seri No: {cihaz?.SeriNo ?? "-"}";

                // Arıza Bilgileri
                var lblAriza = this.Controls.Find("lblAriza", true)[0] as LabelControl;
                lblAriza!.Text = $"Arıza: {_servis.Ariza}";

                var lblArizaDetay = this.Controls.Find("lblArizaDetay", true)[0] as LabelControl;
                lblArizaDetay!.Text = $"Detay: {(string.IsNullOrEmpty(_servis.ArizaDetay) ? "-" : _servis.ArizaDetay)}";

                // Yapılan İşlemler
                var lblYapilanIslemler = this.Controls.Find("lblYapilanIslemler", true)[0] as LabelControl;
                lblYapilanIslemler!.Text = string.IsNullOrEmpty(_servis.YapilanIslemler) ? "-" : _servis.YapilanIslemler;

                // Ücret Bilgileri
                var lblIscilik = this.Controls.Find("lblIscilik", true)[0] as LabelControl;
                lblIscilik!.Text = $"İşçilik: ₺{_servis.IscilikUcreti:N2}";

                var lblParcaUcreti = this.Controls.Find("lblParcaUcreti", true)[0] as LabelControl;
                lblParcaUcreti!.Text = $"Parça: ₺{_servis.ParcaUcreti:N2}";

                var lblToplam = this.Controls.Find("lblToplam", true)[0] as LabelControl;
                lblToplam!.Text = $"TOPLAM: ₺{_servis.ToplamUcret:N2}";

                var lblTahsilat = this.Controls.Find("lblTahsilat", true)[0] as LabelControl;
                lblTahsilat!.Text = $"Tahsilat: {_servis.TahsilatDurumu} (₺{_servis.OdenenTutar:N2})";

                // Fotoğrafları yükle
                await LoadFotograflarAsync();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Veriler yüklenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadFotograflarAsync()
        {
            try
            {
                _fotograflar = (await _fotografRepository.GetByServisIdAsync(_servisId)).ToList();
                
                if (_fotografPanel == null) return;
                
                _fotografPanel.Controls.Clear();
                
                if (_fotograflar.Count == 0)
                {
                    var lblBos = new Label();
                    lblBos.Text = "Henüz fotoğraf eklenmemiş";
                    lblBos.ForeColor = Color.Gray;
                    lblBos.AutoSize = true;
                    lblBos.Padding = new Padding(10);
                    _fotografPanel.Controls.Add(lblBos);
                    
                    if (_buyukResim != null)
                    {
                        _buyukResim.Image = null;
                    }
                    return;
                }

                bool ilkResimYuklendi = false;
                
                foreach (var foto in _fotograflar)
                {
                    if (!File.Exists(foto.DosyaYolu)) continue;

                    var thumbnail = new PictureBox();
                    thumbnail.Size = new Size(100, 75);
                    thumbnail.SizeMode = PictureBoxSizeMode.Zoom;
                    thumbnail.BackColor = Color.FromArgb(50, 50, 50);
                    thumbnail.BorderStyle = BorderStyle.FixedSingle;
                    thumbnail.Cursor = Cursors.Hand;
                    thumbnail.Margin = new Padding(5);
                    thumbnail.Tag = foto;

                    try
                    {
                        using (var fs = new FileStream(foto.DosyaYolu, FileMode.Open, FileAccess.Read))
                        {
                            thumbnail.Image = Image.FromStream(fs);
                        }
                        
                        // İlk resmi büyük resim alanına yükle
                        if (!ilkResimYuklendi && _buyukResim != null)
                        {
                            using (var fs = new FileStream(foto.DosyaYolu, FileMode.Open, FileAccess.Read))
                            {
                                _buyukResim.Image = Image.FromStream(fs);
                            }
                            ilkResimYuklendi = true;
                        }
                    }
                    catch
                    {
                        thumbnail.BackColor = Color.DarkRed;
                    }

                    thumbnail.Click += Thumbnail_Click;
                    thumbnail.DoubleClick += Thumbnail_DoubleClick;
                    
                    _fotografPanel.Controls.Add(thumbnail);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fotoğraflar yüklenirken hata: {ex.Message}");
            }
        }

        private void Thumbnail_Click(object? sender, EventArgs e)
        {
            if (sender is PictureBox pb && pb.Tag is ServisFotograf foto && _buyukResim != null)
            {
                try
                {
                    if (File.Exists(foto.DosyaYolu))
                    {
                        using (var fs = new FileStream(foto.DosyaYolu, FileMode.Open, FileAccess.Read))
                        {
                            _buyukResim.Image?.Dispose();
                            _buyukResim.Image = Image.FromStream(fs);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Resim yüklenirken hata: {ex.Message}");
                }
            }
        }

        private void Thumbnail_DoubleClick(object? sender, EventArgs e)
        {
            if (sender is PictureBox pb && pb.Tag is ServisFotograf foto)
            {
                try
                {
                    if (File.Exists(foto.DosyaYolu))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = foto.DosyaYolu,
                            UseShellExecute = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Fotoğraf açılamadı:\n{ex.Message}", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnFotoEkle_Click(object? sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Fotoğraf Seç";
                openFileDialog.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Tüm Dosyalar|*.*";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;
                        
                        foreach (var dosya in openFileDialog.FileNames)
                        {
                            var kaydedilenYol = ImageHelper.SaveImage(dosya, _servisId, "Cihaz");
                            
                            var fotograf = new ServisFotograf
                            {
                                ServisId = _servisId,
                                DosyaAdi = Path.GetFileName(kaydedilenYol),
                                DosyaYolu = kaydedilenYol,
                                Aciklama = "",
                                FotografTipi = "Cihaz",
                                YuklenmeTarihi = DateTime.Now
                            };
                            
                            await _fotografRepository.AddAsync(fotograf);
                        }
                        
                        this.Cursor = Cursors.Default;
                        
                        await LoadFotograflarAsync();
                        
                        XtraMessageBox.Show($"{openFileDialog.FileNames.Length} fotoğraf başarıyla eklendi.", "Bilgi",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        this.Cursor = Cursors.Default;
                        XtraMessageBox.Show($"Fotoğraf eklenirken hata oluştu:\n{ex.Message}", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnFotograflar_Click(object? sender, EventArgs e)
        {
            if (_servis == null) return;

            var form = new ServisFotografForm(_servisId, _servis.ServisNo);
            form.FormClosed += async (s, args) => await LoadFotograflarAsync();
            form.ShowDialog();
        }

        private async void BtnPdfExport_Click(object? sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var pdfYolu = await PdfExportHelper.ExportServisDetayAsync(_servisId);

                this.Cursor = Cursors.Default;

                var result = XtraMessageBox.Show(
                    $"PDF başarıyla oluşturuldu!\n\nDosya: {pdfYolu}\n\nDosyayı açmak ister misiniz?",
                    "PDF Oluşturuldu",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = pdfYolu,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                XtraMessageBox.Show($"PDF oluşturulurken hata oluştu:\n{ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDuzenle_Click(object? sender, EventArgs e)
        {
            var form = new ServisKayitForm(_servisId);
            form.MdiParent = this.MdiParent;
            form.FormClosed += (s, args) => LoadData();
            form.Show();
        }

        private async void BtnDurumGuncelle_Click(object? sender, EventArgs e)
        {
            var durumlar = new[] { "Beklemede", "İşlemde", "Tamamlandı", "Teslim Edildi", "İptal" };
            
            using (var form = new DevExpress.XtraEditors.XtraForm())
            {
                form.Text = "Durum Güncelle";
                form.Size = new Size(300, 150);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                
                var cmbDurum = new DevExpress.XtraEditors.ComboBoxEdit();
                cmbDurum.Location = new Point(20, 30);
                cmbDurum.Size = new Size(240, 22);
                cmbDurum.Properties.Items.AddRange(durumlar);
                cmbDurum.EditValue = _servis?.Durum ?? "";
                cmbDurum.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                
                var btnTamam = new DevExpress.XtraEditors.SimpleButton();
                btnTamam.Text = "Tamam";
                btnTamam.Location = new Point(100, 70);
                btnTamam.DialogResult = DialogResult.OK;
                
                form.Controls.Add(cmbDurum);
                form.Controls.Add(btnTamam);
                form.AcceptButton = btnTamam;
                
                if (form.ShowDialog() == DialogResult.OK && cmbDurum.Text != _servis?.Durum)
                {
                    try
                    {
                        await _servisRepository.UpdateDurumAsync(_servisId, cmbDurum.Text);
                        XtraMessageBox.Show("Durum güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show($"Durum güncellenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

using DevExpress.XtraEditors;
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
    public partial class ServisKayitForm : XtraForm
    {
        private readonly ServisKaydiRepository _servisRepository;
        private readonly MusteriRepository _musteriRepository;
        private readonly CihazRepository _cihazRepository;
        private readonly KullaniciRepository _kullaniciRepository;
        private readonly ServisFotografRepository _fotografRepository;
        private int? _servisId;
        private ServisKaydi? _servis;
        private FlowLayoutPanel? _fotografPanel;
        private PictureBox? _buyukResim;
        private List<ServisFotograf> _fotograflar = new();

        public ServisKayitForm(int? servisId = null)
        {
            _servisRepository = new ServisKaydiRepository();
            _musteriRepository = new MusteriRepository();
            _cihazRepository = new CihazRepository();
            _kullaniciRepository = new KullaniciRepository();
            _fotografRepository = new ServisFotografRepository();
            _servisId = servisId;

            InitializeComponent();
            LoadComboData();

            if (_servisId.HasValue)
            {
                LoadServisData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = _servisId.HasValue ? "Servis Kaydı Düzenle" : "Yeni Servis Kaydı";
            this.Size = new Size(1300, 800);
            this.StartPosition = FormStartPosition.CenterParent;

            // Sol Panel (Servis Bilgileri)
            var panelSol = new XtraScrollableControl();
            panelSol.Dock = DockStyle.Left;
            panelSol.Width = 780;
            panelSol.Padding = new Padding(20);

            int y = 10;
            int inputWidth = 300;
            int col2X = 420;

            // Müşteri Seçimi
            var lblMusteri = new LabelControl { Text = "Müşteri *", Location = new Point(20, y) };
            panelSol.Controls.Add(lblMusteri);

            var cmbMusteri = new LookUpEdit();
            cmbMusteri.Name = "cmbMusteri";
            cmbMusteri.Location = new Point(120, y);
            cmbMusteri.Size = new Size(inputWidth, 25);
            cmbMusteri.Properties.NullText = "Müşteri Seçin";
            cmbMusteri.Properties.DisplayMember = "AdSoyad";
            cmbMusteri.Properties.ValueMember = "Id";
            cmbMusteri.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("AdSoyad", "Müşteri"));
            cmbMusteri.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Telefon", "Telefon"));
            cmbMusteri.EditValueChanged += CmbMusteri_EditValueChanged;
            panelSol.Controls.Add(cmbMusteri);

            var btnYeniMusteri = new SimpleButton();
            btnYeniMusteri.Text = "+";
            btnYeniMusteri.Location = new Point(430, y);
            btnYeniMusteri.Size = new Size(30, 25);
            btnYeniMusteri.Click += BtnYeniMusteri_Click;
            panelSol.Controls.Add(btnYeniMusteri);

            y += 40;

            // Cihaz Seçimi
            var lblCihaz = new LabelControl { Text = "Cihaz *", Location = new Point(20, y) };
            panelSol.Controls.Add(lblCihaz);

            var cmbCihaz = new LookUpEdit();
            cmbCihaz.Name = "cmbCihaz";
            cmbCihaz.Location = new Point(120, y);
            cmbCihaz.Size = new Size(inputWidth, 25);
            cmbCihaz.Properties.NullText = "Önce müşteri seçin";
            cmbCihaz.Properties.DisplayMember = "CihazBilgisi";
            cmbCihaz.Properties.ValueMember = "Id";
            cmbCihaz.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CihazTuru", "Tür"));
            cmbCihaz.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Marka", "Marka"));
            cmbCihaz.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Model", "Model"));
            panelSol.Controls.Add(cmbCihaz);

            var btnYeniCihaz = new SimpleButton();
            btnYeniCihaz.Text = "+";
            btnYeniCihaz.Location = new Point(430, y);
            btnYeniCihaz.Size = new Size(30, 25);
            btnYeniCihaz.Click += BtnYeniCihaz_Click;
            panelSol.Controls.Add(btnYeniCihaz);

            y += 40;

            // Giriş Tarihi
            var lblGirisTarihi = new LabelControl { Text = "Giriş Tarihi", Location = new Point(20, y) };
            panelSol.Controls.Add(lblGirisTarihi);

            var dtGirisTarihi = new DateEdit();
            dtGirisTarihi.Name = "dtGirisTarihi";
            dtGirisTarihi.Location = new Point(120, y);
            dtGirisTarihi.Size = new Size(inputWidth, 25);
            dtGirisTarihi.DateTime = DateTime.Now;
            dtGirisTarihi.Properties.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
            dtGirisTarihi.Properties.EditFormat.FormatString = "dd.MM.yyyy HH:mm";
            panelSol.Controls.Add(dtGirisTarihi);

            // Tekniker
            var lblTekniker = new LabelControl { Text = "Tekniker", Location = new Point(col2X, y) };
            panelSol.Controls.Add(lblTekniker);

            var cmbTekniker = new LookUpEdit();
            cmbTekniker.Name = "cmbTekniker";
            cmbTekniker.Location = new Point(col2X + 80, y);
            cmbTekniker.Size = new Size(200, 25);
            cmbTekniker.Properties.NullText = "Tekniker Seçin";
            cmbTekniker.Properties.DisplayMember = "AdSoyad";
            cmbTekniker.Properties.ValueMember = "Id";
            cmbTekniker.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("AdSoyad", "Tekniker"));
            panelSol.Controls.Add(cmbTekniker);

            y += 50;

            // Arıza
            var lblAriza = new LabelControl { Text = "Arıza *", Location = new Point(20, y) };
            panelSol.Controls.Add(lblAriza);

            var txtAriza = new TextEdit();
            txtAriza.Name = "txtAriza";
            txtAriza.Location = new Point(120, y);
            txtAriza.Size = new Size(410, 25);
            panelSol.Controls.Add(txtAriza);

            // AI Teşhis Butonu
            var btnAiTeshis = new SimpleButton();
            btnAiTeshis.Name = "btnAiTeshis";
            btnAiTeshis.Text = "🤖 AI Teşhis";
            btnAiTeshis.Location = new Point(540, y);
            btnAiTeshis.Size = new Size(140, 25);
            btnAiTeshis.Appearance.BackColor = Color.FromArgb(103, 58, 183);
            btnAiTeshis.Appearance.ForeColor = Color.White;
            btnAiTeshis.ToolTip = "Girilen arızaya göre AI tarafından teşhis önerileri al";
            btnAiTeshis.Click += BtnAiTeshis_Click;
            panelSol.Controls.Add(btnAiTeshis);

            y += 40;

            // Arıza Detay
            var lblArizaDetay = new LabelControl { Text = "Arıza Detayı", Location = new Point(20, y) };
            panelSol.Controls.Add(lblArizaDetay);

            var txtArizaDetay = new MemoEdit();
            txtArizaDetay.Name = "txtArizaDetay";
            txtArizaDetay.Location = new Point(120, y);
            txtArizaDetay.Size = new Size(560, 80);
            panelSol.Controls.Add(txtArizaDetay);

            y += 100;

            // Durum
            var lblDurum = new LabelControl { Text = "Durum", Location = new Point(20, y) };
            panelSol.Controls.Add(lblDurum);

            var cmbDurum = new ComboBoxEdit();
            cmbDurum.Name = "cmbDurum";
            cmbDurum.Location = new Point(120, y);
            cmbDurum.Size = new Size(170, 25);
            cmbDurum.Properties.Items.AddRange(new[] { "Beklemede", "İşlemde", "Tamamlandı", "Teslim Edildi", "İptal" });
            cmbDurum.SelectedIndex = 0;
            panelSol.Controls.Add(cmbDurum);

            // Öncelik
            var lblOncelik = new LabelControl { Text = "Öncelik", Location = new Point(col2X, y) };
            panelSol.Controls.Add(lblOncelik);

            var cmbOncelik = new ComboBoxEdit();
            cmbOncelik.Name = "cmbOncelik";
            cmbOncelik.Location = new Point(col2X + 80, y);
            cmbOncelik.Size = new Size(120, 25);
            cmbOncelik.Properties.Items.AddRange(new[] { "Düşük", "Normal", "Yüksek", "Acil" });
            cmbOncelik.SelectedIndex = 1;
            panelSol.Controls.Add(cmbOncelik);

            y += 50;

            // Yapılan İşlemler
            var lblYapilanIslemler = new LabelControl { Text = "Yapılan İşlemler", Location = new Point(20, y) };
            panelSol.Controls.Add(lblYapilanIslemler);

            var txtYapilanIslemler = new MemoEdit();
            txtYapilanIslemler.Name = "txtYapilanIslemler";
            txtYapilanIslemler.Location = new Point(120, y);
            txtYapilanIslemler.Size = new Size(560, 80);
            panelSol.Controls.Add(txtYapilanIslemler);

            y += 100;

            // Ücretler
            var grpUcret = new GroupControl();
            grpUcret.Text = "Ücret Bilgileri";
            grpUcret.Location = new Point(20, y);
            grpUcret.Size = new Size(740, 100);

            var lblIscilik = new LabelControl { Text = "İşçilik Ücreti", Location = new Point(20, 35) };
            grpUcret.Controls.Add(lblIscilik);

            var txtIscilik = new SpinEdit();
            txtIscilik.Name = "txtIscilik";
            txtIscilik.Location = new Point(120, 32);
            txtIscilik.Size = new Size(120, 25);
            txtIscilik.Properties.DisplayFormat.FormatString = "₺{0:N2}";
            txtIscilik.Properties.EditFormat.FormatString = "N2";
            txtIscilik.EditValueChanged += CalculateTotal;
            grpUcret.Controls.Add(txtIscilik);

            var lblParcaUcreti = new LabelControl { Text = "Parça Ücreti", Location = new Point(270, 35) };
            grpUcret.Controls.Add(lblParcaUcreti);

            var txtParcaUcreti = new SpinEdit();
            txtParcaUcreti.Name = "txtParcaUcreti";
            txtParcaUcreti.Location = new Point(370, 32);
            txtParcaUcreti.Size = new Size(120, 25);
            txtParcaUcreti.Properties.DisplayFormat.FormatString = "₺{0:N2}";
            txtParcaUcreti.Properties.EditFormat.FormatString = "N2";
            txtParcaUcreti.EditValueChanged += CalculateTotal;
            grpUcret.Controls.Add(txtParcaUcreti);

            // Toplam Tutar Etiketini Düzenle (Daha geniş alana sığdırmak için)
            var lblToplam = new LabelControl { Text = "TOPLAM:", Location = new Point(500, 35), Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            grpUcret.Controls.Add(lblToplam);

            var lblToplamDeger = new LabelControl();
            lblToplamDeger.Name = "lblToplamDeger";
            lblToplamDeger.Text = "₺0,00";
            lblToplamDeger.Location = new Point(585, 33); // Konumu biraz daha sağa ve hizalı
            lblToplamDeger.Font = new Font("Segoe UI", 16, FontStyle.Bold); // Puntosunu büyüttüm
            lblToplamDeger.ForeColor = AppColors.Success;
            lblToplamDeger.AutoSizeMode = LabelAutoSizeMode.None; // Otomatik boyutlandırma yerine sabit genişlik verelim
            lblToplamDeger.Size = new Size(150, 30); // Genişlik verildi
            grpUcret.Controls.Add(lblToplamDeger);

            panelSol.Controls.Add(grpUcret);

            y += 120;

            // Notlar
            var lblNotlar = new LabelControl { Text = "Notlar", Location = new Point(20, y) };
            panelSol.Controls.Add(lblNotlar);

            var txtNotlar = new MemoEdit();
            txtNotlar.Name = "txtNotlar";
            txtNotlar.Location = new Point(120, y);
            txtNotlar.Size = new Size(560, 60);
            panelSol.Controls.Add(txtNotlar);

            y += 80;

            // Butonlar
            var btnKaydet = new SimpleButton();
            btnKaydet.Text = "Kaydet";
            btnKaydet.Location = new Point(520, y);
            btnKaydet.Size = new Size(100, 35);
            btnKaydet.Appearance.BackColor = AppColors.Success;
            btnKaydet.Appearance.ForeColor = Color.White;
            btnKaydet.Click += BtnKaydet_Click;
            panelSol.Controls.Add(btnKaydet);

            var btnIptal = new SimpleButton();
            btnIptal.Text = "İptal";
            btnIptal.Location = new Point(630, y);
            btnIptal.Size = new Size(100, 35);
            btnIptal.Click += (s, e) => this.Close();
            panelSol.Controls.Add(btnIptal);

            // Sağ Panel (Fotoğraflar)
            var panelSag = new GroupControl();
            panelSag.Text = "📷 Cihaz Fotoğrafları";
            panelSag.Dock = DockStyle.Fill;
            panelSag.Padding = new Padding(10);

            // Fotoğraf Ekle Butonu
            var btnFotoEkle = new SimpleButton();
            btnFotoEkle.Text = "📷 Fotoğraf Ekle";
            btnFotoEkle.Location = new Point(10, 30);
            btnFotoEkle.Size = new Size(175, 35);
            btnFotoEkle.Appearance.BackColor = Color.FromArgb(156, 39, 176);
            btnFotoEkle.Appearance.ForeColor = Color.White;
            btnFotoEkle.Click += BtnFotoEkle_Click;
            panelSag.Controls.Add(btnFotoEkle);

            // Fotoğraf Yönetimi Butonu
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

            // Panelleri forma ekle - Sağ panel ÖNCE (Dock.Fill), sonra sol panel (Dock.Left)
            this.Controls.Add(panelSag);
            this.Controls.Add(panelSol);

            this.ResumeLayout(false);
        }

        private async void LoadComboData()
        {
            try
            {
                // Müşteriler
                var musteriler = await _musteriRepository.GetAllAsync();
                var cmbMusteri = this.Controls.Find("cmbMusteri", true)[0] as LookUpEdit;
                cmbMusteri!.Properties.DataSource = musteriler;

                // Teknikerler
                var teknikerler = await _kullaniciRepository.GetTeknikerlerAsync();
                var cmbTekniker = this.Controls.Find("cmbTekniker", true)[0] as LookUpEdit;
                cmbTekniker!.Properties.DataSource = teknikerler;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Veriler yüklenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadServisData()
        {
            try
            {
                _servis = await _servisRepository.GetByIdAsync(_servisId!.Value);
                if (_servis == null)
                {
                    XtraMessageBox.Show("Servis kaydı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Form alanlarını doldur
                var cmbMusteri = this.Controls.Find("cmbMusteri", true)[0] as LookUpEdit;
                cmbMusteri!.EditValue = _servis.MusteriId;

                // Cihazları yükle ve seç
                await LoadCihazlar(_servis.MusteriId);
                var cmbCihaz = this.Controls.Find("cmbCihaz", true)[0] as LookUpEdit;
                cmbCihaz!.EditValue = _servis.CihazId;

                var dtGirisTarihi = this.Controls.Find("dtGirisTarihi", true)[0] as DateEdit;
                dtGirisTarihi!.DateTime = _servis.GirisTarihi;

                var cmbTekniker = this.Controls.Find("cmbTekniker", true)[0] as LookUpEdit;
                cmbTekniker!.EditValue = _servis.TeknikerId;

                var txtAriza = this.Controls.Find("txtAriza", true)[0] as TextEdit;
                txtAriza!.Text = _servis.Ariza;

                var txtArizaDetay = this.Controls.Find("txtArizaDetay", true)[0] as MemoEdit;
                txtArizaDetay!.Text = _servis.ArizaDetay;

                var cmbDurum = this.Controls.Find("cmbDurum", true)[0] as ComboBoxEdit;
                cmbDurum!.Text = _servis.Durum;

                var cmbOncelik = this.Controls.Find("cmbOncelik", true)[0] as ComboBoxEdit;
                cmbOncelik!.Text = _servis.OncelikDurumu;

                var txtYapilanIslemler = this.Controls.Find("txtYapilanIslemler", true)[0] as MemoEdit;
                txtYapilanIslemler!.Text = _servis.YapilanIslemler;

                var txtIscilik = this.Controls.Find("txtIscilik", true)[0] as SpinEdit;
                txtIscilik!.Value = _servis.IscilikUcreti;

                var txtParcaUcreti = this.Controls.Find("txtParcaUcreti", true)[0] as SpinEdit;
                txtParcaUcreti!.Value = _servis.ParcaUcreti;

                var txtNotlar = this.Controls.Find("txtNotlar", true)[0] as MemoEdit;
                txtNotlar!.Text = _servis.Notlar;

                CalculateTotal(null, null);

                // Fotoğrafları yükle
                await LoadFotograflarAsync();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Servis bilgileri yüklenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadFotograflarAsync()
        {
            try
            {
                _fotograflar = (await _fotografRepository.GetByServisIdAsync(_servisId!.Value)).ToList();
                
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
            // Eğer yeni bir servis kaydı ise, önce kaydetmeli
            if (!_servisId.HasValue)
            {
                XtraMessageBox.Show("Önce servis kaydını kaydetmelisiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
                            var kaydedilenYol = ImageHelper.SaveImage(dosya, _servisId.Value, "Cihaz");
                            
                            var fotograf = new ServisFotograf
                            {
                                ServisId = _servisId.Value,
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
            if (!_servisId.HasValue || _servis == null) return;

            var form = new ServisFotografForm(_servisId.Value, _servis.ServisNo);
            form.FormClosed += async (s, args) => await LoadFotograflarAsync();
            form.ShowDialog();
        }

        private async void CmbMusteri_EditValueChanged(object? sender, EventArgs e)
        {
            var cmbMusteri = sender as LookUpEdit;
            if (cmbMusteri?.EditValue == null) return;

            var musteriId = Convert.ToInt32(cmbMusteri.EditValue);
            await LoadCihazlar(musteriId);
        }

        private async Task LoadCihazlar(int musteriId)
        {
            var cihazlar = await _cihazRepository.GetByMusteriIdAsync(musteriId);
            var cmbCihaz = this.Controls.Find("cmbCihaz", true)[0] as LookUpEdit;

            var cihazListesi = cihazlar.Select(c => new
            {
                c.Id,
                CihazBilgisi = $"{c.CihazTuru} - {c.Marka} {c.Model}",
                c.CihazTuru,
                c.Marka,
                c.Model
            }).ToList();

            cmbCihaz!.Properties.DataSource = cihazListesi;
            cmbCihaz.Properties.NullText = cihazListesi.Any() ? "Cihaz Seçin" : "Cihaz bulunamadı";
        }

        private void BtnYeniMusteri_Click(object? sender, EventArgs e)
        {
            var form = new MusteriForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadComboData();
            }
        }

        private async void BtnYeniCihaz_Click(object? sender, EventArgs e)
        {
            var cmbMusteri = this.Controls.Find("cmbMusteri", true)[0] as LookUpEdit;
            if (cmbMusteri?.EditValue == null)
            {
                XtraMessageBox.Show("Önce müşteri seçmelisiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var musteriId = Convert.ToInt32(cmbMusteri.EditValue);
            var form = new CihazForm(musteriId);
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadCihazlar(musteriId);
            }
        }

        private void CalculateTotal(object? sender, EventArgs? e)
        {
            var txtIscilik = this.Controls.Find("txtIscilik", true)[0] as SpinEdit;
            var txtParcaUcreti = this.Controls.Find("txtParcaUcreti", true)[0] as SpinEdit;
            var lblToplamDeger = this.Controls.Find("lblToplamDeger", true)[0] as LabelControl;

            var iscilik = txtIscilik?.Value ?? 0;
            var parca = txtParcaUcreti?.Value ?? 0;
            var toplam = iscilik + parca;

            lblToplamDeger!.Text = $"₺{toplam:N2}";
        }

        private async void BtnKaydet_Click(object? sender, EventArgs e)
        {
            try
            {
                // Validasyon
                var cmbMusteri = this.Controls.Find("cmbMusteri", true)[0] as LookUpEdit;
                var cmbCihaz = this.Controls.Find("cmbCihaz", true)[0] as LookUpEdit;
                var txtAriza = this.Controls.Find("txtAriza", true)[0] as TextEdit;

                if (cmbMusteri?.EditValue == null)
                {
                    XtraMessageBox.Show("Müşteri seçmelisiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbCihaz?.EditValue == null)
                {
                    XtraMessageBox.Show("Cihaz seçmelisiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtAriza?.Text))
                {
                    XtraMessageBox.Show("Arıza bilgisi girmelisiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Verileri al
                var dtGirisTarihi = this.Controls.Find("dtGirisTarihi", true)[0] as DateEdit;
                var cmbTekniker = this.Controls.Find("cmbTekniker", true)[0] as LookUpEdit;
                var txtArizaDetay = this.Controls.Find("txtArizaDetay", true)[0] as MemoEdit;
                var cmbDurum = this.Controls.Find("cmbDurum", true)[0] as ComboBoxEdit;
                var cmbOncelik = this.Controls.Find("cmbOncelik", true)[0] as ComboBoxEdit;
                var txtYapilanIslemler = this.Controls.Find("txtYapilanIslemler", true)[0] as MemoEdit;
                var txtIscilik = this.Controls.Find("txtIscilik", true)[0] as SpinEdit;
                var txtParcaUcreti = this.Controls.Find("txtParcaUcreti", true)[0] as SpinEdit;
                var txtNotlar = this.Controls.Find("txtNotlar", true)[0] as MemoEdit;

                var servis = _servis ?? new ServisKaydi();
                servis.MusteriId = Convert.ToInt32(cmbMusteri.EditValue);
                servis.CihazId = Convert.ToInt32(cmbCihaz.EditValue);
                servis.GirisTarihi = dtGirisTarihi!.DateTime;
                servis.TeknikerId = cmbTekniker?.EditValue != null ? Convert.ToInt32(cmbTekniker.EditValue) : null;
                servis.Ariza = txtAriza.Text;
                servis.ArizaDetay = txtArizaDetay?.Text ?? "";
                servis.Durum = cmbDurum?.Text ?? "Beklemede";
                servis.OncelikDurumu = cmbOncelik?.Text ?? "Normal";
                servis.YapilanIslemler = txtYapilanIslemler?.Text ?? "";
                servis.IscilikUcreti = txtIscilik?.Value ?? 0;
                servis.ParcaUcreti = txtParcaUcreti?.Value ?? 0;
                servis.ToplamUcret = servis.IscilikUcreti + servis.ParcaUcreti;
                servis.Notlar = txtNotlar?.Text ?? "";

                if (_servisId.HasValue)
                {
                    await _servisRepository.UpdateAsync(servis);
                    XtraMessageBox.Show("Servis kaydı güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    servis.ServisNo = await _servisRepository.GenerateServisNoAsync();
                    var id = await _servisRepository.InsertAsync(servis);
                    servis.Id = id;
                    _servisId = id;
                    
                    // Müşteriye e-posta gönder
                    await SendEmailToCustomerAsync(servis);
                    
                    XtraMessageBox.Show($"Servis kaydı oluşturuldu.\nServis No: {servis.ServisNo}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Kayıt sırasında hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SendEmailToCustomerAsync(ServisKaydi servis)
        {
            try
            {
                // E-posta ayarlarını yükle
                var emailAktif = await EmailHelper.LoadAyarlarAsync();
                if (!emailAktif) return;

                // Müşteri ve cihaz bilgilerini al
                var musteri = await _musteriRepository.GetByIdAsync(servis.MusteriId);
                var cihaz = await _cihazRepository.GetByIdAsync(servis.CihazId);

                if (musteri == null || cihaz == null) return;
                if (string.IsNullOrEmpty(musteri.Email)) return;

                // E-posta gönder
                var (success, message) = await EmailHelper.SendServisKayitEmailAsync(servis, musteri, cihaz);
                
                if (success)
                {
                    // Başarılı - sessiz bildirim (ana mesajda gösterilecek)
                }
                else
                {
                    // Hata durumunda kullanıcıyı bilgilendir (opsiyonel)
                    // XtraMessageBox.Show($"E-posta gönderilemedi: {message}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                // E-posta hatası servis kaydını etkilememeli
            }
        }

        private async void BtnAiTeshis_Click(object? sender, EventArgs e)
        {
            try
            {
                // Yapılandırma kontrolü
                if (!LlmHelper.IsConfigured())
                {
                    XtraMessageBox.Show(
                        "OpenRouter API anahtarı yapılandırılmadı.\n\n" +
                        "Lütfen sistem yöneticisine başvurun ve OPENROUTER_API_KEY environment variable'ını ayarlamasını isteyin.\n\n" +
                        "OpenRouter.ai'den ücretsiz API anahtarı alabilirsiniz.",
                        "Yapılandırma Hatası",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Form verilerini al
                var txtAriza = this.Controls.Find("txtAriza", true)[0] as TextEdit;
                var txtArizaDetay = this.Controls.Find("txtArizaDetay", true)[0] as MemoEdit;
                var cmbCihaz = this.Controls.Find("cmbCihaz", true)[0] as LookUpEdit;

                if (string.IsNullOrWhiteSpace(txtAriza?.Text))
                {
                    XtraMessageBox.Show("Lütfen arıza bilgisi girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbCihaz?.EditValue == null)
                {
                    XtraMessageBox.Show("Lütfen cihaz seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cihaz bilgilerini al
                var cihazId = Convert.ToInt32(cmbCihaz.EditValue);
                var cihaz = await _cihazRepository.GetByIdAsync(cihazId);

                if (cihaz == null)
                {
                    XtraMessageBox.Show("Cihaz bilgileri yüklenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Loading göster
                this.Cursor = Cursors.WaitCursor;
                var btnAiTeshis = this.Controls.Find("btnAiTeshis", true)[0] as SimpleButton;
                var originalText = btnAiTeshis?.Text ?? "";
                if (btnAiTeshis != null)
                    btnAiTeshis.Text = "⏳ İşleniyor...";

                // AI teşhisini çağır
                var teshisResponse = await LlmHelper.ArizaTeshisiAsync(
                    cihaz.CihazTuru,
                    cihaz.Marka,
                    cihaz.Model,
                    txtAriza.Text,
                    txtArizaDetay?.Text ?? "");

                this.Cursor = Cursors.Default;
                if (btnAiTeshis != null)
                    btnAiTeshis.Text = originalText;

                // Sonuçları göster
                var form = new ArizaTeshisiForm(teshisResponse);
                form.ShowDialog();

                // Eğer başarılı ise olası sorunları Arıza Detayına ekle
                if (teshisResponse.Success && teshisResponse.OlasıSorunlar.Count > 0)
                {
                    var txtArizaDetay2 = this.Controls.Find("txtArizaDetay", true)[0] as MemoEdit;
                    if (txtArizaDetay2 != null)
                    {
                        var sb = new System.Text.StringBuilder();
                        
                        // Mevcut metni koru
                        if (!string.IsNullOrWhiteSpace(txtArizaDetay2.Text))
                        {
                            sb.AppendLine(txtArizaDetay2.Text);
                            sb.AppendLine();
                        }
                        
                        // Olası Sorunlar başlığı ekle
                        sb.AppendLine("=== OLASI SORUNLAR (AI Teşhis) ===");
                        int no = 1;
                        foreach (var sorun in teshisResponse.OlasıSorunlar)
                        {
                            sb.AppendLine($"{no}. {sorun}");
                            no++;
                        }
                        
                        txtArizaDetay2.Text = sb.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                XtraMessageBox.Show($"AI Teşhis sırasında hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

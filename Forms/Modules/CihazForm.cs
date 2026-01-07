using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;
using TeknikServisOtomasyon.Data.Repositories;
using TeknikServisOtomasyon.Helpers;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Forms.Modules
{
    public partial class CihazForm : XtraForm
    {
        private readonly CihazRepository _cihazRepository;
        private int _musteriId;
        private int? _cihazId;
        private Cihaz? _cihaz;

        public CihazForm(int musteriId, int? cihazId = null)
        {
            _cihazRepository = new CihazRepository();
            _musteriId = musteriId;
            _cihazId = cihazId;

            InitializeComponent();

            if (_cihazId.HasValue)
            {
                LoadCihazData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = _cihazId.HasValue ? "Cihaz Düzenle" : "Yeni Cihaz";
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var panelMain = new PanelControl();
            panelMain.Dock = DockStyle.Fill;
            panelMain.Padding = new Padding(20);

            int y = 20;
            int labelX = 20;
            int inputX = 120;
            int inputWidth = 280;

            // Cihaz Türü
            var lblCihazTuru = new LabelControl { Text = "Cihaz Türü *", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblCihazTuru);

            var cmbCihazTuru = new ComboBoxEdit();
            cmbCihazTuru.Name = "cmbCihazTuru";
            cmbCihazTuru.Location = new Point(inputX, y);
            cmbCihazTuru.Size = new Size(inputWidth, 25);
            cmbCihazTuru.Properties.Items.AddRange(new[] { "Telefon", "Tablet", "Laptop", "Masaüstü Bilgisayar", "Monitör", "Yazıcı", "Diğer" });
            panelMain.Controls.Add(cmbCihazTuru);

            y += 40;

            // Marka
            var lblMarka = new LabelControl { Text = "Marka *", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblMarka);

            var cmbMarka = new ComboBoxEdit();
            cmbMarka.Name = "cmbMarka";
            cmbMarka.Location = new Point(inputX, y);
            cmbMarka.Size = new Size(inputWidth, 25);
            cmbMarka.Properties.Items.AddRange(new[] { "Apple", "Samsung", "Huawei", "Xiaomi", "Oppo", "Vivo", "OnePlus", "Sony", "LG", "HP", "Dell", "Lenovo", "Asus", "Acer", "MSI", "Microsoft", "Diğer" });
            cmbMarka.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            panelMain.Controls.Add(cmbMarka);

            y += 40;

            // Model
            var lblModel = new LabelControl { Text = "Model", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblModel);

            var txtModel = new TextEdit();
            txtModel.Name = "txtModel";
            txtModel.Location = new Point(inputX, y);
            txtModel.Size = new Size(inputWidth, 25);
            panelMain.Controls.Add(txtModel);

            y += 40;

            // Seri No
            var lblSeriNo = new LabelControl { Text = "Seri No", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblSeriNo);

            var txtSeriNo = new TextEdit();
            txtSeriNo.Name = "txtSeriNo";
            txtSeriNo.Location = new Point(inputX, y);
            txtSeriNo.Size = new Size(inputWidth, 25);
            panelMain.Controls.Add(txtSeriNo);

            y += 40;

            // IMEI
            var lblIMEI = new LabelControl { Text = "IMEI", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblIMEI);

            var txtIMEI = new TextEdit();
            txtIMEI.Name = "txtIMEI";
            txtIMEI.Location = new Point(inputX, y);
            txtIMEI.Size = new Size(inputWidth, 25);
            txtIMEI.Properties.MaxLength = 15;
            panelMain.Controls.Add(txtIMEI);

            y += 40;

            // Açıklama
            var lblAciklama = new LabelControl { Text = "Açıklama", Location = new Point(labelX, y + 3) };
            panelMain.Controls.Add(lblAciklama);

            var txtAciklama = new MemoEdit();
            txtAciklama.Name = "txtAciklama";
            txtAciklama.Location = new Point(inputX, y);
            txtAciklama.Size = new Size(inputWidth, 60);
            panelMain.Controls.Add(txtAciklama);

            y += 90;

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

        private async void LoadCihazData()
        {
            try
            {
                _cihaz = await _cihazRepository.GetByIdAsync(_cihazId!.Value);
                if (_cihaz == null)
                {
                    XtraMessageBox.Show("Cihaz bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                var cmbCihazTuru = this.Controls.Find("cmbCihazTuru", true)[0] as ComboBoxEdit;
                cmbCihazTuru!.Text = _cihaz.CihazTuru;

                var cmbMarka = this.Controls.Find("cmbMarka", true)[0] as ComboBoxEdit;
                cmbMarka!.Text = _cihaz.Marka;

                var txtModel = this.Controls.Find("txtModel", true)[0] as TextEdit;
                txtModel!.Text = _cihaz.Model;

                var txtSeriNo = this.Controls.Find("txtSeriNo", true)[0] as TextEdit;
                txtSeriNo!.Text = _cihaz.SeriNo;

                var txtIMEI = this.Controls.Find("txtIMEI", true)[0] as TextEdit;
                txtIMEI!.Text = _cihaz.IMEI;

                var txtAciklama = this.Controls.Find("txtAciklama", true)[0] as MemoEdit;
                txtAciklama!.Text = _cihaz.Aciklama;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Cihaz bilgileri yüklenirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnKaydet_Click(object? sender, EventArgs e)
        {
            try
            {
                var cmbCihazTuru = this.Controls.Find("cmbCihazTuru", true)[0] as ComboBoxEdit;
                var cmbMarka = this.Controls.Find("cmbMarka", true)[0] as ComboBoxEdit;

                if (string.IsNullOrWhiteSpace(cmbCihazTuru?.Text))
                {
                    XtraMessageBox.Show("Cihaz türü seçmelisiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(cmbMarka?.Text))
                {
                    XtraMessageBox.Show("Marka girmelisiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var txtModel = this.Controls.Find("txtModel", true)[0] as TextEdit;
                var txtSeriNo = this.Controls.Find("txtSeriNo", true)[0] as TextEdit;
                var txtIMEI = this.Controls.Find("txtIMEI", true)[0] as TextEdit;
                var txtAciklama = this.Controls.Find("txtAciklama", true)[0] as MemoEdit;

                var cihaz = _cihaz ?? new Cihaz();
                cihaz.MusteriId = _musteriId;
                cihaz.CihazTuru = cmbCihazTuru.Text;
                cihaz.Marka = cmbMarka.Text;
                cihaz.Model = txtModel?.Text ?? "";
                cihaz.SeriNo = txtSeriNo?.Text ?? "";
                cihaz.IMEI = txtIMEI?.Text ?? "";
                cihaz.Aciklama = txtAciklama?.Text ?? "";

                if (_cihazId.HasValue)
                {
                    await _cihazRepository.UpdateAsync(cihaz);
                    XtraMessageBox.Show("Cihaz güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await _cihazRepository.InsertAsync(cihaz);
                    XtraMessageBox.Show("Cihaz kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

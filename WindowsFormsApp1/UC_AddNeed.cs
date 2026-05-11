using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    public partial class UC_AddNeed : UserControl
    {
        private CharityService _charityService;
        private string selectedImagePath = "";

        // عناصر الواجهة
        private TextBox txtProductName, txtDescription, txtQuantity;
        private ComboBox cbCategory, cbUnit, cbPriority;
        private PictureBox pbProductImage;
        private Button btnSelectImage, btnSave;
        private Label lblStatus;

        public UC_AddNeed()
        {
            _charityService = new CharityService();
            InitializeCustomComponents();
            LoadEnums();
        }

        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.RightToLeft = RightToLeft.Yes;

            Label lblTitle = new Label { Text = "إضافة احتياج جديد", Font = new Font("Segoe UI", 18, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };

            int startX = 20, startY = 80, spacing = 60;

            AddLabelAndControl("اسم المنتج:", txtProductName = new TextBox { Width = 300 }, startX, startY);
            AddLabelAndControl("القسم:", cbCategory = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList }, startX, startY + spacing);
            AddLabelAndControl("الكمية:", txtQuantity = new TextBox { Width = 300 }, startX, startY + (spacing * 2));
            AddLabelAndControl("الوحدة:", cbUnit = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList }, startX, startY + (spacing * 3));
            AddLabelAndControl("الأولوية:", cbPriority = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList }, startX, startY + (spacing * 4));
            AddLabelAndControl("وصف إضافي (اختياري):", txtDescription = new TextBox { Width = 300, Multiline = true, Height = 80 }, startX, startY + (spacing * 5));

            pbProductImage = new PictureBox { Size = new Size(250, 250), Location = new Point(450, 100), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.White };
            btnSelectImage = new Button { Text = "اختر صورة المنتج", Location = new Point(450, 360), Size = new Size(250, 40), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnSelectImage.Click += BtnSelectImage_Click;

            lblStatus = new Label { Text = "", Location = new Point(20, 475), AutoSize = true, ForeColor = Color.Blue };

            btnSave = new Button { Text = "حفظ وإرسال الاحتياج", Location = new Point(20, 500), Size = new Size(680, 50), BackColor = Color.ForestGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnSave.Click += BtnSave_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(pbProductImage);
            this.Controls.Add(btnSelectImage);
            this.Controls.Add(lblStatus);
            this.Controls.Add(btnSave);
        }

        private void AddLabelAndControl(string text, Control ctrl, int x, int y)
        {
            Label lbl = new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 10) };
            ctrl.Location = new Point(x, y + 25);
            this.Controls.Add(lbl);
            this.Controls.Add(ctrl);
        }

        private void LoadEnums()
        {
            cbCategory.DataSource = Enum.GetValues(typeof(ProductCategory));
            cbUnit.DataSource = Enum.GetValues(typeof(MeasurementUnit));
            cbPriority.DataSource = Enum.GetValues(typeof(CharityNeedPriority));
        }

        private void BtnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Images|*.jpg;*.jpeg;*.png;*.webp" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = ofd.FileName;
                    if (pbProductImage.Image != null) pbProductImage.Image.Dispose();
                    pbProductImage.Image = Image.FromFile(selectedImagePath);
                }
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            // 1. التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(txtProductName.Text) || string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                MessageBox.Show("يرجى إدخال اسم المنتج والكمية بشكل صحيح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtQuantity.Text, out double quantityValue))
            {
                MessageBox.Show("يرجى إدخال قيمة عددية صحيحة للكمية", "خطأ في البيانات", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. تجهيز البيانات للإرسال
            var need = new CreateCharityNeedDTO
            {
                ProductName = txtProductName.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                Quantity = quantityValue,
                Category = (int)cbCategory.SelectedValue,
                Unit = (int)cbUnit.SelectedValue,
                Priority = (int)cbPriority.SelectedValue,
                ProductImagePath = selectedImagePath
            };

            try
            {
                // تعطيل الزر ومنع النقرات المتكررة
                btnSave.Enabled = false;
                lblStatus.Text = "جاري إرسال البيانات للسيرفر...";
                lblStatus.ForeColor = Color.Blue;

                // 3. استدعاء السيرفيس للإرسال
                // ملاحظة: تأكد أن CreateNeedAsync موجودة في CharityService وتستقبل CreateCharityNeedDTO
                bool success = await _charityService.CreateNeedAsync(need);

                if (success)
                {
                    lblStatus.Text = "✅ تم إرسال الطلب بنجاح وهو بانتظار موافقة الإدارة";
                    lblStatus.ForeColor = Color.Green;
                    MessageBox.Show("تمت إضافة الاحتياج بنجاح و يتم انتظار موافقة المسئول!", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm(); // مسح البيانات استعداداً لطلب آخر
                }
                else
                {
                    lblStatus.Text = "❌ فشل الإرسال، يرجى المحاولة لاحقاً";
                    lblStatus.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ غير متوقع: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSave.Enabled = true;
            }
        }

        private void ClearForm()
        {
            txtProductName.Clear();
            txtDescription.Clear();
            txtQuantity.Clear();
            selectedImagePath = "";
            if (pbProductImage.Image != null)
            {
                pbProductImage.Image.Dispose();
                pbProductImage.Image = null;
            }
            cbCategory.SelectedIndex = 0;
            cbUnit.SelectedIndex = 0;
            cbPriority.SelectedIndex = 0;
        }
    }
}
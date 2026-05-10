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
        private Label lblStatus; // لإظهار حالة التحميل

        public UC_AddNeed()
        {
            // InitializeComponent(); // لو بتستخدم Designer فك الكومنت ده، لو كود فقط سيبه مقفول
            _charityService = new CharityService();
            InitializeCustomComponents();
            LoadEnums();
        }

        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.RightToLeft = RightToLeft.Yes; // دعم اللغة العربية

            Label lblTitle = new Label { Text = "إضافة احتياج جديد", Font = new Font("Segoe UI", 18, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };

            // القائمة اليمنى (البيانات)
            int startX = 20, startY = 80, spacing = 60;

            AddLabelAndControl("اسم المنتج:", txtProductName = new TextBox { Width = 300 }, startX, startY);
            AddLabelAndControl("القسم:", cbCategory = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList }, startX, startY + spacing);
            AddLabelAndControl("الكمية:", txtQuantity = new TextBox { Width = 300 }, startX, startY + (spacing * 2));
            AddLabelAndControl("الوحدة:", cbUnit = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList }, startX, startY + (spacing * 3));
            AddLabelAndControl("الأولوية:", cbPriority = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList }, startX, startY + (spacing * 4));
            AddLabelAndControl("وصف إضافي (اختياري):", txtDescription = new TextBox { Width = 300, Multiline = true, Height = 80  }, startX, startY + (spacing * 5));

            // الجهة اليسرى (الصورة)
            pbProductImage = new PictureBox { Size = new Size(250, 250), Location = new Point(450, 100), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.White };
            btnSelectImage = new Button { Text = "اختر صورة المنتج", Location = new Point(450, 360), Size = new Size(250, 40), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSelectImage.Click += BtnSelectImage_Click;

            // حالة التحميل
            lblStatus = new Label { Text = "", Location = new Point(20, 475), AutoSize = true, ForeColor = Color.Blue };

            // زر الحفظ
            btnSave = new Button { Text = "حفظ وإرسال الاحتياج", Location = new Point(20, 500), Size = new Size(680, 50), BackColor = Color.ForestGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
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
            // تعبئة الكومبوبوكس بالقيم بناءً على الـ Enums الموجودة في الموديلز
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
                    // التخلص من الصورة القديمة قبل شحن الجديدة لتوفير الرامات
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

            // 3. الإرسال عبر الـ Service
            try
            {
                btnSave.Enabled = false;
                lblStatus.Text = "\nجاري إرسال البيانات للـ API...\n";

                bool success = await _charityService.CreateNeedAsync(need);

                if (success)
                {
                    MessageBox.Show("تم إضافة الاحتياج بنجاح وهو الآن بانتظار مراجعة المسؤول", "تمت العملية", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("فشل إرسال الاحتياج. يرجى التأكد من اتصال الإنترنت أو صلاحية الحساب.", "خطأ في الإرسال", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ غير متوقع: {ex.Message}", "خطأ نظام", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSave.Enabled = true;
                lblStatus.Text = "";
            }
        }

        private void ClearForm()
        {
            txtProductName.Clear();
            txtDescription.Clear();
            txtQuantity.Clear();
            if (pbProductImage.Image != null)
            {
                pbProductImage.Image.Dispose();
                pbProductImage.Image = null;
            }
            selectedImagePath = "";
            cbCategory.SelectedIndex = 0;
            cbUnit.SelectedIndex = 0;
            cbPriority.SelectedIndex = 0;
        }
    }
}
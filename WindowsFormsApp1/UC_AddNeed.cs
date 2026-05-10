using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
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
            this.RightToLeft = RightToLeft.Yes; // دعم اللغة العربية

            Label lblTitle = new Label { Text = "إضافة احتياج جديد", Font = new Font("Segoe UI", 18, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };

            // القائمة اليمنى (البيانات)
            int startX = 20, startY = 80, spacing = 60;

            AddLabelAndControl("اسم المنتج:", txtProductName = new TextBox { Width = 300 }, startX, startY);
            AddLabelAndControl("القسم:", cbCategory = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList }, startX, startY + spacing);
            AddLabelAndControl("الكمية:", txtQuantity = new TextBox { Width = 300 }, startX, startY + (spacing * 2));
            AddLabelAndControl("الوحدة:", cbUnit = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList }, startX, startY + (spacing * 3));
            AddLabelAndControl("الأولوية:", cbPriority = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList }, startX, startY + (spacing * 4));
            AddLabelAndControl("وصف إضافي:", txtDescription = new TextBox { Width = 300, Multiline = true, Height = 80 }, startX, startY + (spacing * 5));

            // الجهة اليسرى (الصورة)
            pbProductImage = new PictureBox { Size = new Size(250, 250), Location = new Point(450, 100), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.White };
            btnSelectImage = new Button { Text = "اختر صورة المنتج", Location = new Point(450, 360), Size = new Size(250, 40), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSelectImage.Click += BtnSelectImage_Click;

            // زر الحفظ
            btnSave = new Button { Text = "حفظ وإرسال الاحتياج", Location = new Point(20, 500), Size = new Size(680, 50), BackColor = Color.ForestGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            btnSave.Click += BtnSave_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(pbProductImage);
            this.Controls.Add(btnSelectImage);
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
            // تعبئة الكومبوبوكس بالقيم (يمكنك تحسينها لاحقاً بأسماء عربية)
            cbCategory.DataSource = Enum.GetValues(typeof(ProductCategory));
            cbUnit.DataSource = Enum.GetValues(typeof(MeasurementUnit));
            cbPriority.DataSource = Enum.GetValues(typeof(CharityNeedPriority));
        }

        private void BtnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Images|*.jpg;*.jpeg;*.png" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = ofd.FileName;
                    pbProductImage.Image = Image.FromFile(selectedImagePath);
                }
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProductName.Text) || string.IsNullOrEmpty(txtQuantity.Text))
            {
                MessageBox.Show("يرجى إدخال البيانات الأساسية");
                return;
            }

            var need = new CreateCharityNeedDTO
            {
                ProductName = txtProductName.Text,
                Description = txtDescription.Text,
                Quantity = double.Parse(txtQuantity.Text),
                Category = (int)cbCategory.SelectedValue,
                Unit = (int)cbUnit.SelectedValue,
                Priority = (int)cbPriority.SelectedValue,
                ProductImagePath = selectedImagePath
            };

            btnSave.Enabled = false;
            bool success = await _charityService.CreateNeedAsync(need);
            btnSave.Enabled = true;

            if (success)
            {
                MessageBox.Show("تم إضافة الاحتياج بنجاح!");
                // تفريغ الحقول
            }
            else MessageBox.Show("حدث خطأ أثناء الإرسال");
        }
    }
}
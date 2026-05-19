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
        private Panel mainContainer; // الحاوية الرئيسية (البطاقة البيضاء)

        public UC_AddNeed()
        {
            _charityService = new CharityService();
            InitializeCustomComponents();
            LoadEnums();
        }

        private void InitializeCustomComponents()
        {
            // إعدادات الـ UserControl الرئيسية
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(240, 242, 245); // خلفية رمادية ناعمة وعصرية (مثل فيسبوك وويب 3)
            this.RightToLeft = RightToLeft.Yes;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // إنشاء الحاوية المركزية بأبعاد مدروسة ومضمونة الاستجابة
            mainContainer = new Panel
            {
                Size = new Size(880, 660), // تم زيادة الطول والعرض لاستيعاب العناصر بمرونة
                BackColor = Color.White,   // شكل البطاقة النظيفة Card Design
                BorderStyle = BorderStyle.None
            };

            // وضع الحاوية في المنتصف تلقائياً عند تغيير حجم الشاشة أو تكبيرها
            this.SizeChanged += (s, e) => {
                mainContainer.Location = new Point((this.Width - mainContainer.Width) / 2, (this.Height - mainContainer.Height) / 2);
            };
            this.Controls.Add(mainContainer);

            // عنوان الصفحة
            Label lblTitle = new Label
            {
                Text = "إضافة احتياج جديد",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(43, 54, 116), // أزرق نيلي فخم
                Location = new Point(620, 25),
                AutoSize = true
            };
            mainContainer.Controls.Add(lblTitle);

            // إحداثيات توزيع عناصر الإدخال في الجهة اليمنى
            int startX = 500;
            int startY = 100;
            int spacing = 75;
            int controlWidth = 330; // عرض مريح وموحد للصناديق

            // الحقول النصية والقوائم المنسدلة بتصميم Flat ناعم
            AddLabelAndControl("اسم المنتج أو الغرض *", txtProductName = new TextBox { Width = controlWidth, Font = new Font("Segoe UI", 11) }, startX, startY);

            cbCategory = new ComboBox { Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11) };
            AddLabelAndControl("القسم الدراسي / التصنيف *", cbCategory, startX, startY + spacing);

            AddLabelAndControl("الكمية المطلوبة *", txtQuantity = new TextBox { Width = controlWidth, Font = new Font("Segoe UI", 11) }, startX, startY + (spacing * 2));

            cbUnit = new ComboBox { Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11) };
            AddLabelAndControl("الوحدة (كيلو، قطعة، إلخ) *", cbUnit, startX, startY + (spacing * 3));

            cbPriority = new ComboBox { Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11) };
            AddLabelAndControl("درجة الأولوية والأهمية *", cbPriority, startX, startY + (spacing * 4));

            // حقل الوصف الإضافي (يمتد بعرض مناسب)
            txtDescription = new TextBox { Width = 780, Multiline = true, Height = 75, Font = new Font("Segoe UI", 11), ScrollBars = ScrollBars.Vertical };
            AddLabelAndControl("وصف إضافي تفصيلي (اختياري):", txtDescription, 50, startY + (spacing * 5));

            // --- قسم رفع الصورة في الجهة اليسرى ---
            int imageX = 50;

            Label lblImgTitle = new Label { Text = "صورة المنتج المرفقة", Location = new Point(imageX, startY - 25), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.DimGray };
            mainContainer.Controls.Add(lblImgTitle);

            pbProductImage = new PictureBox
            {
                Size = new Size(400, 250), // مساحة عرض الصورة ممتازة الآن
                Location = new Point(imageX, startY),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            btnSelectImage = new Button
            {
                Text = "📁 تصفح واختيار صورة للمنتج",
                Location = new Point(imageX, startY + 265),
                Size = new Size(400, 40),
                BackColor = Color.FromArgb(52, 152, 219), // أزرق مودرن مريح
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSelectImage.FlatAppearance.BorderSize = 0;
            btnSelectImage.Click += BtnSelectImage_Click;

            mainContainer.Controls.Add(pbProductImage);
            mainContainer.Controls.Add(btnSelectImage);

           
            // --- زر الحفظ الاحترافي (كبير وبارز ويمتد على كامل العرض السفلي) ---
            btnSave = new Button
            {
                Text = "💾 حفظ وإرسال طلب الاحتياج للمراجعة",
                Location = new Point(50, 590), // موقعه في أسفل الكارد الأبيض تماماً
                Size = new Size(780, 50),     // عرض ضخم وارتفاع مريح جداً للضغط
                BackColor = Color.FromArgb(46, 204, 113), // أخضر مبهج واحترافي للعمليات الناجحة
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13, FontStyle.Bold), // خط عريض وواضح
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            mainContainer.Controls.Add(btnSave);
        }

        private void AddLabelAndControl(string text, Control ctrl, int x, int y)
        {
            // عنوان الحقل بلون داكن أنيق
            Label lbl = new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 90, 100)
            };

            if (ctrl is TextBox txt)
            {
                txt.BorderStyle = BorderStyle.FixedSingle;
            }

            ctrl.Location = new Point(x, y + 25);
            mainContainer.Controls.Add(lbl);

            // ضبط محاذاة النص تلقائياً ليبدأ مع حافة الصندوق من جهة اليمين تماماً (RTL الصحيح)
            lbl.Location = new Point(x + ctrl.Width - lbl.Width, y);

            mainContainer.Controls.Add(ctrl);
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
                btnSave.Enabled = false;
                lblStatus.Text = "⏳ جاري إرسال البيانات إلى السيرفر بآمان...";
                lblStatus.ForeColor = Color.FromArgb(230, 126, 34); // برتقالي أثناء الرفع

                bool success = await _charityService.CreateNeedAsync(need);

                if (success)
                {
                    lblStatus.Text = "✅ تم إرسال الطلب بنجاح وهو بانتظار موافقة الإدارة";
                    lblStatus.ForeColor = Color.FromArgb(39, 174, 96); // أخضر للنجاح
                    MessageBox.Show("تمت إضافة الاحتياج بنجاح ويتم انتظار موافقة المسؤول!", "نجاح العملية", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
                else
                {
                    lblStatus.Text = "❌ فشل الإرسال، يرجى التحقق من الاتصال والمحاولة لاحقاً";
                    lblStatus.ForeColor = Color.FromArgb(192, 57, 43); // أحمر للفشل
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ غير متوقع: " + ex.Message, "خطأ في النظام", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
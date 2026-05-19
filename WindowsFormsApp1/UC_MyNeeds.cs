using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    public partial class UC_MyNeeds : UserControl
    {
        private DataGridView dgvMyNeeds;
        private CharityService _charityService;
        private Label lblTitle;
        private Label lblLoading;

        public UC_MyNeeds()
        {
            _charityService = new CharityService();
            InitializeCustomComponents();

            // تحميل البيانات تلقائياً عند تحميل الواجهة
            this.Load += async (s, e) => await LoadMyNeeds();
        }

        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(248, 249, 250); // خلفية رمادية ناعمة وموحدة مع باقي الصفحات
            this.RightToLeft = RightToLeft.Yes;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // عنوان الصفحة بشكل فخم ومحاذاة مرنة ديناميكية
            lblTitle = new Label
            {
                Text = "📋 قائمة احتياجاتي المرفوعة بالنظام",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(43, 54, 116), // الأزرق النيلي المعتمد للهوية
                Location = new Point(20, 20),
                AutoSize = true
            };

            // ضبط موقع العنوان ليكون دائماً مضبوطاً جهة اليمين حتى عند تكبير الشاشة
            this.SizeChanged += (s, e) => {
                lblTitle.Location = new Point(this.Width - lblTitle.Width - 25, 20);
            };

            // مؤشر جاري التحميل بتصميم أنيق ومنتصف الشاشة
            lblLoading = new Label
            {
                Text = "⏳ جاري جلب وتحديث قائمة الاحتياجات من السيرفر...",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 126, 34), // لون برتقالي دلالي على الانتظار
                Location = new Point(25, 25),
                AutoSize = true,
                Visible = false
            };

            // إعداد الجدول بتصميم Flat عصري ونظيف
           
            dgvMyNeeds = new DataGridView
            {
                Location = new Point(25, 75),
                Size = new Size(this.Width - 50, this.Height - 110), // تم جعله يملأ الشاشة بالكامل بمرونة مريحة
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                GridColor = Color.FromArgb(235, 238, 242), // شبكة داخلية ناعمة جداً
                EnableHeadersVisualStyles = false,
                RowTemplate = { Height = 45 } // ارتفاع سطر ممتاز ومريح لعين المستخدم
            };

            // تنسيق رأس الجدول (Header) ليعبر عن احترافية البيانات
            dgvMyNeeds.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(43, 54, 116);
            dgvMyNeeds.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMyNeeds.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            dgvMyNeeds.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMyNeeds.ColumnHeadersHeight = 48;

            // تنسيق خلايا البيانات العادية
            dgvMyNeeds.DefaultCellStyle.BackColor = Color.White;
            dgvMyNeeds.DefaultCellStyle.ForeColor = Color.FromArgb(45, 55, 72);
            dgvMyNeeds.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvMyNeeds.DefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 244, 253); // تظليل خفيف عند الاختيار
            dgvMyNeeds.DefaultCellStyle.SelectionForeColor = Color.FromArgb(43, 54, 116);
            dgvMyNeeds.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // تفعيل ميزة الأسطر التبادلية لمنع تداخل القراءة
            dgvMyNeeds.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 253, 254);

            // ربط حدث اكتمال البيانات لتلوين الحالات تلقائياً (Status Badges Look)
            dgvMyNeeds.DataBindingComplete += dgvMyNeeds_DataBindingComplete;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblLoading);
            this.Controls.Add(dgvMyNeeds);
        }

        private async Task LoadMyNeeds()
        {
            try
            {
                lblLoading.Visible = true;
                var needsList = await _charityService.GetAllNeedsAsync();

                dgvMyNeeds.Invoke((MethodInvoker)delegate {
                    dgvMyNeeds.DataSource = null;
                    if (needsList != null)
                    {
                        dgvMyNeeds.DataSource = needsList;
                        FormatGrid();
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل قائمة احتياجاتك: " + ex.Message, "خطأ نظام", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                lblLoading.Visible = false;
            }
        }

        private void FormatGrid()
        {
            if (dgvMyNeeds.Columns.Count == 0) return;

            // إخفاء الأعمدة البرمجية أو غير الضرورية للمستخدم النهائي بشكل آمن وسلس (تأمين الحالتين camelCase أو PascalCase)
            string[] toHide = { "id", "Id", "category", "Category", "unit", "Unit", "priority", "Priority", "status", "Status", "imageUrl", "ImageUrl", "description", "Description" };
            foreach (var col in toHide)
            {
                if (dgvMyNeeds.Columns.Contains(col))
                    dgvMyNeeds.Columns[col].Visible = false;
            }

            // تعريب وتنسيق مسميات رؤوس الأعمدة بناءً على خصائص الـ DTO المتوقعة
            // تم دعم المسميات الإنجليزية والعربية معاً لضمان عدم حدوث أي استثناء أو اختفاء للأعمدة
            RenameColumnIfExists("productName", "اسم المنتج المرفوع");
            RenameColumnIfExists("ProductName", "اسم المنتج المرفوع");

            RenameColumnIfExists("quantity", "الكمية المطلوبة");
            RenameColumnIfExists("Quantity", "الكمية المطلوبة");

            RenameColumnIfExists("الوحدة", "وحدة القياس");
            RenameColumnIfExists("UnitName", "وحدة القياس");

            RenameColumnIfExists("حالة_الطلب", "حالة الطلب الحالية");
            RenameColumnIfExists("StatusName", "حالة الطلب الحالية");
            RenameColumnIfExists("statusName", "حالة الطلب الحالية");
        }

        private void RenameColumnIfExists(string columnName, string headerText)
        {
            if (dgvMyNeeds.Columns.Contains(columnName))
            {
                dgvMyNeeds.Columns[columnName].HeaderText = headerText;
            }
        }

        // تلوين ذكي لحالة الطلبات لإعطاء لمحة بصرية سريعة واحترافية للمسؤول
        private void dgvMyNeeds_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            string[] statusColumnNames = { "حالة_الطلب", "StatusName", "statusName" };
            string targetColumn = "";

            // البحث عن العمود النشط للحالة
            foreach (var colName in statusColumnNames)
            {
                if (dgvMyNeeds.Columns.Contains(colName))
                {
                    targetColumn = colName;
                    break;
                }
            }

            if (string.IsNullOrEmpty(targetColumn)) return;

            foreach (DataGridViewRow row in dgvMyNeeds.Rows)
            {
                var cell = row.Cells[targetColumn];
                if (cell.Value != null)
                {
                    string status = cell.Value.ToString().Trim();

                    // تطبيق تأثير الألوان المريحة بناءً على حالة الطلب
                    if (status.Contains("مقبول") || status.Contains("تم") || status.Contains("ناجح"))
                    {
                        cell.Style.ForeColor = Color.FromArgb(39, 174, 96); // أخضر
                        cell.Style.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    }
                    else if (status.Contains("انتظار") || status.Contains("مراجعة") || status.Contains("جاري"))
                    {
                        cell.Style.ForeColor = Color.FromArgb(230, 126, 34); // برتقالي
                        cell.Style.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    }
                    else if (status.Contains("مرفوض") || status.Contains("ملغي"))
                    {
                        cell.Style.ForeColor = Color.FromArgb(192, 57, 43); // أحمر
                        cell.Style.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // UC_MyNeeds
            // 
            this.Name = "UC_MyNeeds";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ResumeLayout(false);
        }
    }
}
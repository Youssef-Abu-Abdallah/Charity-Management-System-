using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    public partial class UC_SentApplications : UserControl
    {
        private DataGridView dgvSentApps;
        private CharityService _charityService;
        private Label lblTitle;

        public UC_SentApplications()
        {
            // استدعاء الميثود من ملف الـ Designer.cs تلقائياً بدون تكرارها هنا
            InitializeComponent();

            _charityService = new CharityService();
            InitializeCustomComponents();

            // ربط حدث التحميل لجلب البيانات
            this.Load += async (s, e) => await LoadMyApplications();
        }

        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(248, 249, 250); // خلفية رمادية ناعمة وموحدة مع باقي النظام
            this.RightToLeft = RightToLeft.Yes;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // عنوان الصفحة محاذاته مرنة وديناميكية مع اليمين
            lblTitle = new Label
            {
                Text = "📤 طلبات التبرع التي قدمت عليها",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(43, 54, 116), // الأزرق النيلي الاحترافي المعتمد للهوية
                Location = new Point(20, 20),
                AutoSize = true
            };

            // ضبط موقع العنوان ديناميكياً مع تغير حجم الشاشة لضمان ثباته باليمين
            this.SizeChanged += (s, e) => {
                lblTitle.Location = new Point(this.Width - lblTitle.Width - 25, 20);
            };

            // إعداد الجدول بتصميم Flat حديث ونظيف يملأ الشاشة بمرونة
            dgvSentApps = new DataGridView
            {
                Location = new Point(25, 75),
                Size = new Size(this.Width - 50, this.Height - 110),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                GridColor = Color.FromArgb(235, 238, 242), // لون شبكة ناعم جداً
                EnableHeadersVisualStyles = false,
                RowTemplate = { Height = 45 } // ارتفاع سطر مريح جداً للقراءة
            };

            // تنسيق رأس الجدول (Header) ليعبر عن احترافية البيانات
            dgvSentApps.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(43, 54, 116);
            dgvSentApps.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSentApps.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            dgvSentApps.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSentApps.ColumnHeadersHeight = 48;

            // تنسيق خلايا البيانات العادية داخل الجدول
            dgvSentApps.DefaultCellStyle.BackColor = Color.White;
            dgvSentApps.DefaultCellStyle.ForeColor = Color.FromArgb(45, 55, 72);
            dgvSentApps.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvSentApps.DefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 244, 253); // تظليل أزرق خفيف عند التحديد
            dgvSentApps.DefaultCellStyle.SelectionForeColor = Color.FromArgb(43, 54, 116);
            dgvSentApps.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // تفعيل ميزة الأسطر التبادلية لمنع تداخل الأسطر أثناء المراجعة
            dgvSentApps.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 253, 254);

            // ربط حدث اكتمال ربط البيانات لتلوين الحالات تلقائياً
            dgvSentApps.DataBindingComplete += dgvSentApps_DataBindingComplete;

            this.Controls.Add(lblTitle);
            this.Controls.Add(dgvSentApps);
        }

        private async Task LoadMyApplications()
        {
            try
            {
                var apps = await _charityService.GetMySentApplicationsAsync();

                if (apps == null)
                {
                    MessageBox.Show("السيرفر لم يرسل أي بيانات (Null)", "تنبيه النظام", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (apps.Count == 0)
                {
                    MessageBox.Show("لا يوجد لديك أي طلبات مرسلة حتى الآن.", "ملاحظة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // تحضير البيانات للعرض بشكل منظم وتاريخ منسق
                var displayList = apps.Select(a => new {
                    a.ProductName,
                    a.DonorOrganizationName,
                    a.Quantity,
                    UnitName = GetUnitName(a.Unit),
                    StatusName = GetStatusName(a.Status),
                    Date = a.CreatedAt.ToString("yyyy-MM-dd"),
                    a.Phone
                }).ToList();

                dgvSentApps.Invoke((MethodInvoker)delegate {
                    dgvSentApps.DataSource = null; // تنظيف دائم لمنع التداخل
                    dgvSentApps.DataSource = displayList;
                    FormatGrid();
                    dgvSentApps.Visible = true;
                    dgvSentApps.BringToFront();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ تقني أثناء تحميل الطلبات: " + ex.Message, "خطأ نظام", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatGrid()
        {
            if (dgvSentApps.Columns.Count == 0) return;

            // تعريب وتنسيق مسميات رؤوس الأعمدة بدقة
            if (dgvSentApps.Columns.Contains("ProductName")) dgvSentApps.Columns["ProductName"].HeaderText = "اسم المنتج المطلق";
            if (dgvSentApps.Columns.Contains("DonorOrganizationName")) dgvSentApps.Columns["DonorOrganizationName"].HeaderText = "الجهة المتبرعة المانحة";
            if (dgvSentApps.Columns.Contains("Quantity")) dgvSentApps.Columns["Quantity"].HeaderText = "الكمية المحجوزة";
            if (dgvSentApps.Columns.Contains("UnitName")) dgvSentApps.Columns["UnitName"].HeaderText = "الوحدة";
            if (dgvSentApps.Columns.Contains("StatusName")) dgvSentApps.Columns["StatusName"].HeaderText = "حالة الطلب الحالية";
            if (dgvSentApps.Columns.Contains("Date")) dgvSentApps.Columns["Date"].HeaderText = "تاريخ التقديم";
            if (dgvSentApps.Columns.Contains("Phone")) dgvSentApps.Columns["Phone"].HeaderText = "أرقام التواصل للجهة";
        }

        // تلوين ذكي واحترافي لعمود الحالة يعطي انطباعاً وتفاعلاً رائعاً للمستخدم
        private void dgvSentApps_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (!dgvSentApps.Columns.Contains("StatusName")) return;

            foreach (DataGridViewRow row in dgvSentApps.Rows)
            {
                var cell = row.Cells["StatusName"];
                if (cell.Value != null)
                {
                    string status = cell.Value.ToString();

                    if (status.Contains("تم القبول") || status.Contains("تم الاستلام"))
                    {
                        cell.Style.ForeColor = Color.FromArgb(39, 174, 96); // لون أخضر مريح ومبهج
                        cell.Style.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    }
                    else if (status.Contains("قيد الانتظار"))
                    {
                        cell.Style.ForeColor = Color.FromArgb(230, 126, 34); // لون برتقالي دلالي ممتاز
                        cell.Style.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    }
                    else if (status.Contains("مرفوض"))
                    {
                        cell.Style.ForeColor = Color.FromArgb(192, 57, 43); // لون أحمر صريح للرفض
                        cell.Style.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    }
                }
            }
        }

        // دوال التحويل المتوافقة مع بنية قاعدة البيانات لديك ومعدلة لتدعم الحالات المتنوعة
        private string GetUnitName(int unit)
        {
            switch (unit)
            {
                case 0: return "طن";
                case 1: return "كيلو جرام";
                case 2: return "جرام";
                case 3: return "لتر";
                case 4: return "ملي لتر";
                case 5: return "عبوة";
                case 6: return "صندوق";
                case 7: return "علبة";
                case 8: return "قطعة";
                default: return "وحدة";
            }
        }

        private string GetStatusName(int status)
        {
            switch (status)
            {
                case 0: return "⏳ قيد الانتظار";
                case 1: return "✅ تم القبول";
                case 2: return "❌ مرفوض";
                case 3: return "📦 تم الاستلام";
                default: return "غير محدد";
            }
        }
    }
}
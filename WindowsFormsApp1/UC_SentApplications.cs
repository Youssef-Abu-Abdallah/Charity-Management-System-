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
            InitializeComponent(); // هذا السطر مهم جداً لربط ملف الـ Designer

            _charityService = new CharityService();
            InitializeCustomComponents();

            // ربط حدث التحميل لجلب البيانات
            this.Load += async (s, e) => await LoadMyApplications();
        }

        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.RightToLeft = RightToLeft.Yes;

            lblTitle = new Label
            {
                Text = "📤 طلبات التبرع التي قدمت عليها",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 45),
                Location = new Point(900, 20),
                AutoSize = true
            };

            dgvSentApps = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(this.Width - 40, this.Height - 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AllowUserToAddRows = false
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(dgvSentApps);
        }

        private async Task LoadMyApplications()
        {
            try
            {
                // 1. تنبيه للتأكد أن الميثود بدأت
                Console.WriteLine("بدء جلب البيانات...");

                var apps = await _charityService.GetMySentApplicationsAsync();

                // 2. فحص هل الداتا رجعت فاضية؟
                if (apps == null)
                {
                    MessageBox.Show("السيرفر لم يرسل أي بيانات (Null)");
                    return;
                }

                if (apps.Count == 0)
                {
                    MessageBox.Show("لا يوجد لديك أي طلبات مرسلة حتى الآن.");
                    return;
                }

                // 3. لو في داتا، هنعرضها
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
                    dgvSentApps.DataSource = displayList;
                    FormatGrid();
                    dgvSentApps.Visible = true; // نأكد إن الجدول ظاهر
                    dgvSentApps.BringToFront(); // نأكد إنه مش مستخبي ورا حاجة
                });
            }
            catch (Exception ex)
            {
                // 4. لو في خطأ في الكود هيظهر هنا
                MessageBox.Show("حدث خطأ تقني: " + ex.Message);
            }
        }

        private void FormatGrid()
        {
            // تعريب رؤوس الأعمدة
            if (dgvSentApps.Columns.Contains("ProductName")) dgvSentApps.Columns["ProductName"].HeaderText = "اسم المنتج";
            if (dgvSentApps.Columns.Contains("DonorOrganizationName")) dgvSentApps.Columns["DonorOrganizationName"].HeaderText = "الجهة المتبرعة";
            if (dgvSentApps.Columns.Contains("Quantity")) dgvSentApps.Columns["Quantity"].HeaderText = "الكمية";
            if (dgvSentApps.Columns.Contains("UnitName")) dgvSentApps.Columns["UnitName"].HeaderText = "الوحدة";
            if (dgvSentApps.Columns.Contains("StatusName")) dgvSentApps.Columns["StatusName"].HeaderText = "الحالة";
            if (dgvSentApps.Columns.Contains("Date")) dgvSentApps.Columns["Date"].HeaderText = "تاريخ التقديم";
            if (dgvSentApps.Columns.Contains("Phone")) dgvSentApps.Columns["Phone"].HeaderText = "التواصل";

            // تنسيق شكل الجدول
            dgvSentApps.RowTemplate.Height = 40;
            dgvSentApps.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219); // لون أزرق احترافي
            dgvSentApps.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSentApps.EnableHeadersVisualStyles = false;
        }

        // دوال التحويل المتوافقة مع C# 7.3
        private string GetUnitName(int unit)
        {
            switch (unit)
            {
                case 8: return "قطعة";
                case 1: return "كيلو جرام";
                case 6: return "صندوق";
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
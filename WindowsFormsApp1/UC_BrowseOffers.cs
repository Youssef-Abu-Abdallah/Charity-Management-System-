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
    public partial class UC_BrowseOffers : UserControl
    {
        private DataGridView dgvOffers;
        private CharityService _charityService;
        private Label lblTitle;

        public UC_BrowseOffers()
        {
            _charityService = new CharityService();
            InitializeCustomComponents();
            // تشغيل جلب البيانات في الخلفية

            

            _ = LoadOffers();
        }

        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.RightToLeft = RightToLeft.Yes;

            lblTitle = new Label
            {
                Text = "🍎 التبرعات المتاحة من المؤسسات",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 45),
                Location = new Point(20, 20),
                AutoSize = true
            };

            dgvOffers = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(100,100), // تأكد من أن المقاس مناسب للـ Panel
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AllowUserToAddRows = false
            };

            // أهم خطوة: إضافة العناصر للكنترول نفسه
            this.Controls.Add(lblTitle);
            this.Controls.Add(dgvOffers);


            this.BorderStyle = BorderStyle.FixedSingle;
        }

        private async Task LoadOffers()
        {
            var offers = await _charityService.GetAllOffersAsync();

            if (offers != null && offers.Count > 0)
            {
                var displayList = offers.Select(o => new {
                    o.OfferId,
                    o.ProductName,
                    o.DonorOrganizationName,
                    o.Quantity,
                    UnitName = GetUnitName(o.Unit), // تحويل الوحدة لنص
                    CategoryName = GetCategoryName(o.Category), // تحويل القسم لنص
                    StatusName = GetStatusName(o.Status), // تحويل الحالة لنص
                                                          // نحتفظ بالأرقام الأصلية مخفية لو احتجناها في العمليات الحسابية
                    CategoryVal = o.Category,
                    UnitVal = o.Unit,
                    StatusVal = o.Status
                }).ToList();

                dgvOffers.Invoke((MethodInvoker)delegate {
                    dgvOffers.DataSource = displayList;
                    FormatGrid();
                });
            }
        }

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
                case 0: return "قيد الانتظار";
                case 1: return "مقبول";
                case 2: return "مرفوض";
                case 3: return "تم التنفيذ";
                default: return "غير محدد";
            }
        }

        // دالة بسيطة لتحويل أرقام الأقسام لكلمات
        private string GetCategoryName(int category)
        {
            switch (category)
            {
                case 0: return "طعام";
                case 1: return "ملابس";
                case 2: return "مستلزمات طبية";
                case 3: return "أدوات تعليمية";
                default: return "أخرى";
            }
        }

        private void FormatGrid()
        {
            // تعريب العناوين وتنسيق الأعمدة
            if (dgvOffers.Columns.Contains("ProductName")) dgvOffers.Columns["ProductName"].HeaderText = "اسم المنتج";
            if (dgvOffers.Columns.Contains("DonorOrganizationName")) dgvOffers.Columns["DonorOrganizationName"].HeaderText = "المؤسسة المتبرعة";
            if (dgvOffers.Columns.Contains("Quantity")) dgvOffers.Columns["Quantity"].HeaderText = "الكمية";
            if (dgvOffers.Columns.Contains("UnitName")) dgvOffers.Columns["UnitName"].HeaderText = "الوحدة";
            if (dgvOffers.Columns.Contains("CategoryName")) dgvOffers.Columns["CategoryName"].HeaderText = "النوع";
            if (dgvOffers.Columns.Contains("StatusName")) dgvOffers.Columns["StatusName"].HeaderText = "الحالة";

            // إخفاء الأعمدة اللي الكود بس بيستخدمها
            string[] hide = { "OfferId", "CategoryVal", "UnitVal", "StatusVal" };
            foreach (var col in hide)
            {
                if (dgvOffers.Columns.Contains(col)) dgvOffers.Columns[col].Visible = false;
            }

            // --- لمسات التصميم ---
            dgvOffers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // توزيع الأعمدة بالتساوي
            dgvOffers.BackgroundColor = Color.White; // لون الخلفية
            dgvOffers.RowTemplate.Height = 40; // زيادة ارتفاع الصف عشان الخط الكبير
            dgvOffers.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // تحديد الصف بالكامل
            dgvOffers.ReadOnly = true; // منع التعديل اليدوي في الجدول
            dgvOffers.AllowUserToAddRows = false; // منع إضافة صفوف فاضية
        }





    }
}
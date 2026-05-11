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

            // تحميل البيانات عند بدء التشغيل
            this.Load += async (s, e) => await LoadOffers();
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
                Size = new Size(this.Width - 40, this.Height - 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = false, // جعلناه false للسماح بالضغط على الأزرار
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                GridColor = Color.FromArgb(240, 240, 240),
                EnableHeadersVisualStyles = false
            };

            // تنسيق رأس الجدول
            dgvOffers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgvOffers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvOffers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvOffers.ColumnHeadersHeight = 40;

            // ربط حدث الضغط على الأزرار
            dgvOffers.CellContentClick += dgvOffers_CellContentClick;

            this.Controls.Add(lblTitle);
            this.Controls.Add(dgvOffers);
        }

        private async Task LoadOffers()
        {
            try
            {
                var offers = await _charityService.GetAllOffersAsync();

                if (offers != null)
                {
                    var displayList = offers.Select(o => new {
                        o.OfferId,
                        o.ProductName,
                        o.DonorOrganizationName,
                        o.Quantity,
                        UnitName = GetUnitName(o.Unit),
                        CategoryName = GetCategoryName(o.Category),
                        StatusName = GetStatusName(o.Status)
                    }).ToList();

                    dgvOffers.Invoke((MethodInvoker)delegate {
                        dgvOffers.DataSource = displayList;
                        FormatGrid();
                        AddApplyButton(); // إضافة الزر بعد تعبئة البيانات
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل البيانات: {ex.Message}");
            }
        }

        private void FormatGrid()
        {
            if (dgvOffers.Columns.Contains("OfferId")) dgvOffers.Columns["OfferId"].Visible = false;

            if (dgvOffers.Columns.Contains("ProductName")) dgvOffers.Columns["ProductName"].HeaderText = "اسم المنتج";
            if (dgvOffers.Columns.Contains("DonorOrganizationName")) dgvOffers.Columns["DonorOrganizationName"].HeaderText = "المؤسسة المتبرعة";
            if (dgvOffers.Columns.Contains("Quantity")) dgvOffers.Columns["Quantity"].HeaderText = "الكمية";
            if (dgvOffers.Columns.Contains("UnitName")) dgvOffers.Columns["UnitName"].HeaderText = "الوحدة";
            if (dgvOffers.Columns.Contains("CategoryName")) dgvOffers.Columns["CategoryName"].HeaderText = "النوع";
            if (dgvOffers.Columns.Contains("StatusName")) dgvOffers.Columns["StatusName"].HeaderText = "الحالة";

            dgvOffers.RowTemplate.Height = 45;

            // جعل كل الأعمدة قراءة فقط ماعدا عمود الزر
            foreach (DataGridViewColumn col in dgvOffers.Columns)
            {
                if (col.Name != "ApplyButton") col.ReadOnly = true;
            }
        }

        private void AddApplyButton()
        {
            if (dgvOffers.Columns["ApplyButton"] == null)
            {
                DataGridViewButtonColumn applyButton = new DataGridViewButtonColumn();
                applyButton.Name = "ApplyButton";
                applyButton.HeaderText = "إجراء";
                applyButton.Text = "تقديم الآن";
                applyButton.UseColumnTextForButtonValue = true;
                applyButton.FlatStyle = FlatStyle.Flat;

                // تنسيق الزر
                applyButton.DefaultCellStyle.BackColor = Color.FromArgb(46, 204, 113);
                applyButton.DefaultCellStyle.ForeColor = Color.White;
                applyButton.DefaultCellStyle.SelectionBackColor = Color.FromArgb(39, 174, 96);

                dgvOffers.Columns.Add(applyButton);
            }
        }

        private async void dgvOffers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // التأكد أن الضغط تم على عمود الزر "ApplyButton"
            if (e.RowIndex >= 0 && dgvOffers.Columns[e.ColumnIndex].Name == "ApplyButton")
            {
                var offerIdValue = dgvOffers.Rows[e.RowIndex].Cells["OfferId"].Value;
                if (offerIdValue == null) return;

                string offerId = offerIdValue.ToString();

                var confirm = MessageBox.Show("هل أنت متأكد من الرغبة في التقديم على هذا التبرع؟",
                                            "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    // تعطيل الجدول مؤقتاً لمنع نقرات مزدوجة
                    dgvOffers.Enabled = false;

                    bool success = await _charityService.ApplyForOfferAsync(offerId);

                    if (success)
                    {
                        MessageBox.Show("تم إرسال طلبك بنجاح!", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("فشل إرسال الطلب. يرجى المحاولة لاحقاً.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    dgvOffers.Enabled = true;
                }
            }
        }

        // --- دوال التحويل المساعدة ---

        // --- دوال التحويل المساعدة بصيغة C# 7.3 المتوافقة مع مشروعك ---
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

    }
}